using Dalamud.Interface.Windowing;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System.Net;
using Newtonsoft.Json;
using System.Linq;

namespace AetherFM.Windows;

public class RadioBrowserStation
{
    public string name { get; set; }
    public string url_resolved { get; set; }
    public string country { get; set; }
    public string tags { get; set; }
}

public class RadioWindow : Window, IDisposable
{
    private readonly Plugin plugin;
    private string radioUrl;
    private string status = "Stopped";
    private bool isPlaying = false;
    private List<RadiosureStation> stations = new();
    private int selectedStation = -1;
    private string stationSearch = string.Empty;
    private List<string> availableCountries = new List<string> { "All", "Italy", "United States", "United Kingdom", "France", "Germany", "Spain", "Japan", "Brazil", "Canada", "Australia" };
    private int selectedCountryIndex = 0;
    private float volume = 50f; // Default volume (50%)

    public RadioWindow(Plugin plugin) : base("AetherFM - Radio Player")
    {
        this.plugin = plugin;
        this.radioUrl = plugin.Configuration.RadioUrl ?? string.Empty;
        DownloadStationsFromRadioBrowser(); // Automatic loading on window open
        this.SizeConstraints = new WindowSizeConstraints
        {
            MinimumSize = new Vector2(600, 400),
            MaximumSize = new Vector2(2000, 1200)
        };
    }

    private void DownloadStationsFromRadioBrowser(string country = null)
    {
        try
        {
            string url;
            if (string.IsNullOrEmpty(country) || country == "All")
                url = "https://de1.api.radio-browser.info/json/stations";
            else
                url = $"https://de1.api.radio-browser.info/json/stations/bycountry/{country}";
            using var client = new WebClient();
            var json = client.DownloadString(url);
            var rbStations = JsonConvert.DeserializeObject<List<RadioBrowserStation>>(json);
            stations = rbStations
                .Where(s => s.url_resolved != null && s.url_resolved.EndsWith(".mp3") && s.url_resolved.StartsWith("http://"))
                .Select(s => new RadiosureStation
                {
                    Name = s.name,
                    Url = s.url_resolved,
                    Country = s.country,
                    Genre = s.tags,
                    Language = ""
                })
                .GroupBy(s => s.Name + "|" + s.Url)
                .Select(g => g.First())
                .ToList();
            status = $"Loaded {stations.Count} stations from Radio Browser{(string.IsNullOrEmpty(country) || country == "All" ? "" : " for " + country)}";
        }
        catch (Exception ex)
        {
            status = $"Error loading from Radio Browser: {ex.Message}";
        }
    }

    public void Dispose() { }

    public override void Draw()
    {
        ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, new Vector2(16, 16));
        ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, new Vector2(10, 8));

        ImGui.TextUnformatted("🎵  AetherFM Radio Player");
        ImGui.Separator();
        ImGui.Spacing();

        ImGui.TextUnformatted("Search station:");
        ImGui.SetNextItemWidth(220f);
        ImGui.InputTextWithHint("##stationSearch", "Search by name...", ref stationSearch, 100);

        // Volume slider at the top with icon
        ImGui.SameLine();
        string volIcon = volume == 0 ? "🔇" : (volume < 50 ? "🔈" : "🔊");
        ImGui.TextUnformatted(volIcon);
        ImGui.SameLine();
        ImGui.TextUnformatted("Volume:");
        ImGui.SameLine();
        ImGui.SetNextItemWidth(140f);
        float prevVolume = volume;
        ImGui.SliderFloat("##volumeSlider", ref volume, 0f, 100f, "%.0f%%", ImGuiSliderFlags.AlwaysClamp);
        if (isPlaying && Math.Abs(volume - prevVolume) > 0.01f)
        {
            plugin.RadioManager.SetVolume(volume / 100f);
        }
        ImGui.Spacing();

        ImGui.TextUnformatted("Country filter:");
        ImGui.SetNextItemWidth(220f);
        if (ImGui.BeginCombo("##countryCombo", availableCountries[selectedCountryIndex]))
        {
            for (int i = 0; i < availableCountries.Count; i++)
            {
                bool isSelected = (selectedCountryIndex == i);
                if (ImGui.Selectable(availableCountries[i], isSelected))
                {
                    if (selectedCountryIndex != i) {
                        selectedCountryIndex = i;
                        var country = availableCountries[selectedCountryIndex];
                        DownloadStationsFromRadioBrowser(country == "All" ? null : country);
                    }
                }
                if (isSelected)
                    ImGui.SetItemDefaultFocus();
            }
            ImGui.EndCombo();
        }

        ImGui.SameLine();
        if (ImGui.Button("Load from Radio Browser"))
        {
            var country = availableCountries[selectedCountryIndex];
            DownloadStationsFromRadioBrowser(country == "All" ? null : country);
        }

        ImGui.Spacing();
        ImGui.TextUnformatted("Stations:");
        Vector2 tableSize = new Vector2(ImGui.GetContentRegionAvail().X, 400); // Fixed height for scroll
        if (ImGui.BeginTable("##stationsTable", 4, ImGuiTableFlags.Borders | ImGuiTableFlags.RowBg | ImGuiTableFlags.ScrollY | ImGuiTableFlags.ScrollX, tableSize))
        {
            ImGui.TableSetupColumn("Name", ImGuiTableColumnFlags.WidthStretch, 0.40f);
            ImGui.TableSetupColumn("Country", ImGuiTableColumnFlags.WidthFixed, 90f);
            ImGui.TableSetupColumn("Genre", ImGuiTableColumnFlags.WidthStretch, 0.45f);
            ImGui.TableSetupColumn("Action", ImGuiTableColumnFlags.WidthFixed, 70f); // Button column with text
            ImGui.TableHeadersRow();
            for (int i = 0; i < stations.Count; i++)
            {
                // Search by name, genre, and country
                bool matches = true;
                if (!string.IsNullOrEmpty(stationSearch)) {
                    string search = stationSearch.ToLowerInvariant();
                    matches = stations[i].Name.ToLowerInvariant().Contains(search)
                        || stations[i].Genre.ToLowerInvariant().Contains(search)
                        || stations[i].Country.ToLowerInvariant().Contains(search);
                }
                if (!matches)
                    continue;
                ImGui.TableNextRow();
                // Highlight the row if the station is playing (by URL only)
                bool isActive = isPlaying && radioUrl == stations[i].Url;
                if (isActive) {
                    ImGui.TableSetBgColor(ImGuiTableBgTarget.RowBg0, ImGui.GetColorU32(new Vector4(0.1f, 0.7f, 0.1f, 0.45f))); // More contrast
                }
                ImGui.TableSetColumnIndex(0);
                if (isActive) {
                    ImGui.TextUnformatted($"▶ {stations[i].Name}"); // Icon also in the name
                } else {
                    ImGui.TextUnformatted(stations[i].Name);
                }
                if (ImGui.IsItemHovered() && stations[i].Name.Length > 30)
                    ImGui.SetTooltip(stations[i].Name);
                ImGui.TableSetColumnIndex(1);
                ImGui.TextUnformatted(stations[i].Country);
                ImGui.TableSetColumnIndex(2);
                // Wrapping and preview for Genre
                string genre = stations[i].Genre;
                string genrePreview = genre;
                if (genre.Length > 40) genrePreview = genre.Substring(0, 37) + "...";
                ImGui.TextWrapped(genrePreview);
                if (ImGui.IsItemHovered() && genre.Length > 40)
                    ImGui.SetTooltip(genre);
                ImGui.TableSetColumnIndex(3);
                // Play/Stop button with text
                if (isActive) {
                    ImGui.PushStyleColor(ImGuiCol.Button, new Vector4(0.1f, 0.7f, 0.1f, 1f));
                    if (ImGui.Button($"■ Stop##{i}"))
                    {
                        plugin.RadioManager.Stop();
                        // The state will be updated by the OnStatusChanged callback
                    }
                    ImGui.PopStyleColor();
                } else {
                    if (ImGui.Button($"▶ Play##{i}"))
                    {
                        // Immediately update selection and URL
                        selectedStation = i;
                        radioUrl = stations[i].Url;
                        plugin.Configuration.RadioUrl = radioUrl;
                        plugin.Configuration.Save();
                        // Always stop the current radio before starting a new one
                        plugin.RadioManager.Stop();
                        // Start the new radio with the current volume
                        plugin.RadioManager.Start(radioUrl, OnStatusChanged, volume / 100f);
                        // The state will be updated ONLY in the OnStatusChanged callback
                    }
                }
                // Error message near the row if the radio fails to start
                if (isActive && status.StartsWith("Error")) {
                    ImGui.TableSetColumnIndex(0);
                    ImGui.SameLine();
                    ImGui.TextColored(new Vector4(1,0.2f,0.2f,1), "! Error: could not start stream");
                }
                // Extra spacing between rows
                ImGui.TableSetColumnIndex(0);
                ImGui.Dummy(new Vector2(0, 2));
            }
            ImGui.EndTable();
        }

        // --- OPTIONAL CUSTOM URL FIELD ---
        ImGui.Spacing();
        ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(0.7f, 0.7f, 0.7f, 1f));
        ImGui.TextUnformatted("(Optional) Enter a custom stream URL:");
        ImGui.PopStyleColor();
        ImGui.SetNextItemWidth(320f);
        ImGui.InputTextWithHint("##radioUrl", "http://...", ref radioUrl, 256);
        ImGui.SameLine();
        ImGui.TextDisabled("(?)");
        if (ImGui.IsItemHovered())
            ImGui.SetTooltip("Use this field only if you want to listen to a radio not present in the list.");
        // --- END OPTIONAL FIELD ---

        ImGui.Spacing();
        if (ImGui.Button("Refresh status"))
        {
            status = plugin.RadioManager.GetStatus();
        }

        ImGui.Spacing();
        // Colored status
        Vector4 statusColor = status.StartsWith("Playing") ? new Vector4(0,1,0,1) :
                              status.StartsWith("Error") ? new Vector4(1,0,0,1) :
                              status.StartsWith("Stopped") ? new Vector4(1,1,0,1) :
                              new Vector4(1,1,1,1);
        ImGui.TextColored(statusColor, $"Status: {status}");

        ImGui.PopStyleVar(2);
    }

    private void OnStatusChanged(string newStatus)
    {
        status = newStatus;
        isPlaying = newStatus == "Playing";
    }
} 