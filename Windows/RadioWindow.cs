using Dalamud.Interface.Windowing;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System.Net;

namespace AetherFM.Windows;

public class RadioWindow : Window, IDisposable
{
    private readonly Plugin plugin;
    private string radioUrl;
    private string status = "Stopped";
    private bool isPlaying = false;
    private List<RadiosureStation> stations = new();
    private int selectedStation = -1;
    private string stationsFile = "stations.rsd";
    private string stationSearch = string.Empty;

    public RadioWindow(Plugin plugin) : base("AetherFM - Radio Player", ImGuiWindowFlags.AlwaysAutoResize)
    {
        this.plugin = plugin;
        this.radioUrl = plugin.Configuration.RadioUrl ?? string.Empty;
        LoadStations();
    }

    private void LoadStations()
    {
        if (File.Exists(stationsFile))
        {
            stations = RadiosureParser.Parse(stationsFile);
        }
    }

    private void DownloadLatestStations()
    {
        try
        {
            var today = DateTime.UtcNow.ToString("yyyy-MM-dd");
            var url = $"http://82.66.77.189/stations-{today}.rsd";
            using var client = new WebClient();
            client.DownloadFile(url, stationsFile);
            LoadStations();
            status = "Stations updated!";
        }
        catch (Exception ex)
        {
            status = $"Error updating stations: {ex.Message}";
        }
    }

    public void Dispose() { }

    public override void Draw()
    {
        ImGui.TextUnformatted("Select a radio station or enter a custom URL:");
        ImGui.SetNextItemWidth(300f);
        if (ImGui.InputText("##radioUrl", ref radioUrl, 256))
        {
            plugin.Configuration.RadioUrl = radioUrl;
            plugin.Configuration.Save();
            selectedStation = -1;
        }

        ImGui.TextUnformatted("Search station:");
        ImGui.SetNextItemWidth(200f);
        ImGui.InputText("##stationSearch", ref stationSearch, 100);

        if (ImGui.BeginCombo("Stations", selectedStation >= 0 && selectedStation < stations.Count ? stations[selectedStation].Name : "Select station"))
        {
            for (int i = 0; i < stations.Count; i++)
            {
                if (!string.IsNullOrEmpty(stationSearch) && !stations[i].Name.Contains(stationSearch, StringComparison.OrdinalIgnoreCase))
                    continue;
                bool isSelected = (selectedStation == i);
                if (ImGui.Selectable(stations[i].Name, isSelected))
                {
                    selectedStation = i;
                    radioUrl = stations[i].Url;
                    plugin.Configuration.RadioUrl = radioUrl;
                    plugin.Configuration.Save();
                }
                if (isSelected)
                    ImGui.SetItemDefaultFocus();
            }
            ImGui.EndCombo();
        }

        if (ImGui.Button(isPlaying ? "Stop" : "Play Radio", new Vector2(120, 0)))
        {
            if (!isPlaying)
            {
                var started = plugin.RadioManager.Start(radioUrl, OnStatusChanged);
                if (started)
                {
                    isPlaying = true;
                    status = "Playing";
                }
                else
                {
                    status = "Error: could not start stream";
                }
            }
            else
            {
                plugin.RadioManager.Stop();
                isPlaying = false;
                status = "Stopped";
            }
        }

        ImGui.SameLine();
        if (ImGui.Button("Refresh status"))
        {
            status = plugin.RadioManager.GetStatus();
        }

        ImGui.SameLine();
        if (ImGui.Button("Update stations"))
        {
            DownloadLatestStations();
        }

        ImGui.Spacing();
        ImGui.TextUnformatted($"Status: {status}");
    }

    private void OnStatusChanged(string newStatus)
    {
        status = newStatus;
        if (newStatus == "Stopped" || newStatus.StartsWith("Error"))
            isPlaying = false;
    }
} 