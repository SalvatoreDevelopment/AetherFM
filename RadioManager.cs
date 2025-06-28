using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using NAudio.Wave;

namespace AetherFM;

public class RadioManager : IDisposable
{
    private IWavePlayer? waveOut;
    private MediaFoundationReader? reader;
    private Thread? radioThread;
    private CancellationTokenSource? cts;
    private string status = "Fermata";
    private string? currentUrl;
    private Action<string>? statusCallback;

    public bool Start(string url, Action<string>? onStatusChanged = null)
    {
        if (waveOut != null || radioThread != null)
        {
            Stop();
        }
        if (string.IsNullOrWhiteSpace(url))
        {
            status = "Errore: URL vuoto";
            onStatusChanged?.Invoke(status);
            return false;
        }
        statusCallback = onStatusChanged;
        cts = new CancellationTokenSource();
        radioThread = new Thread(() => PlayRadio(url, cts.Token));
        radioThread.IsBackground = true;
        radioThread.Start();
        return true;
    }

    private void PlayRadio(string url, CancellationToken token)
    {
        try
        {
            status = "Connessione...";
            statusCallback?.Invoke(status);
            currentUrl = url;
            using (reader = new MediaFoundationReader(url))
            using (waveOut = new WaveOutEvent())
            {
                waveOut.Init(reader);
                waveOut.Play();
                status = "In riproduzione";
                statusCallback?.Invoke(status);
                while (waveOut.PlaybackState == PlaybackState.Playing && !token.IsCancellationRequested)
                {
                    Thread.Sleep(200);
                }
                waveOut.Stop();
            }
            status = "Fermata";
            statusCallback?.Invoke(status);
        }
        catch (Exception ex)
        {
            status = $"Errore: {ex.Message}";
            statusCallback?.Invoke(status);
        }
        finally
        {
            waveOut?.Dispose();
            reader?.Dispose();
            waveOut = null;
            reader = null;
            radioThread = null;
            cts = null;
        }
    }

    public void Stop()
    {
        cts?.Cancel();
        waveOut?.Stop();
        status = "Fermata";
        statusCallback?.Invoke(status);
    }

    public string GetStatus() => status;

    public void Dispose()
    {
        Stop();
    }
} 