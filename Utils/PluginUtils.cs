using Dalamud.Plugin.Services;
using System;

namespace SamplePlugin.Utils;

public static class PluginUtils
{
    /// <summary>
    /// Formatta un timestamp in formato leggibile
    /// </summary>
    /// <param name="timestamp">Timestamp da formattare</param>
    /// <returns>Stringa formattata</returns>
    public static string FormatTimestamp(DateTime timestamp)
    {
        return timestamp.ToString("HH:mm:ss");
    }

    /// <summary>
    /// Calcola la distanza tra due posizioni
    /// </summary>
    /// <param name="pos1">Prima posizione</param>
    /// <param name="pos2">Seconda posizione</param>
    /// <returns>Distanza in unità di gioco</returns>
    public static float CalculateDistance(System.Numerics.Vector3 pos1, System.Numerics.Vector3 pos2)
    {
        return System.Numerics.Vector3.Distance(pos1, pos2);
    }

    /// <summary>
    /// Logga un messaggio con timestamp
    /// </summary>
    /// <param name="log">Servizio di log</param>
    /// <param name="message">Messaggio da loggare</param>
    public static void LogWithTimestamp(IPluginLog log, string message)
    {
        var timestamp = FormatTimestamp(DateTime.Now);
        var formattedMessage = $"[{timestamp}] {message}";
        log.Information(formattedMessage);
    }

    /// <summary>
    /// Logga un messaggio di debug con timestamp
    /// </summary>
    /// <param name="log">Servizio di log</param>
    /// <param name="message">Messaggio da loggare</param>
    public static void LogDebugWithTimestamp(IPluginLog log, string message)
    {
        var timestamp = FormatTimestamp(DateTime.Now);
        var formattedMessage = $"[{timestamp}] {message}";
        log.Debug(formattedMessage);
    }

    /// <summary>
    /// Logga un messaggio di errore con timestamp
    /// </summary>
    /// <param name="log">Servizio di log</param>
    /// <param name="message">Messaggio da loggare</param>
    public static void LogErrorWithTimestamp(IPluginLog log, string message)
    {
        var timestamp = FormatTimestamp(DateTime.Now);
        var formattedMessage = $"[{timestamp}] {message}";
        log.Error(formattedMessage);
    }
} 