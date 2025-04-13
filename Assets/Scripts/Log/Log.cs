using UnityEngine;
using System;
using System.IO;
using System.Text;

public static class Log
{
    private static readonly string LogsFolderPath = Path.Combine(Application.persistentDataPath, "Logs");
    private static string CurrentLogFilePath => Path.Combine(LogsFolderPath, $"log_{DateTime.Now:yyyyMMdd}.txt");

    public enum LogLevel
    {
        Debug,
        Info,
        Warning,
        Error,
        Critical
    }

    private static LogLevel _minLogLevel = LogLevel.Debug;

    /// <summary>
    /// Устанавливает минимальный уровень логирования (сообщения ниже этого уровня игнорируются)
    /// </summary>
    public static void SetMinLogLevel(LogLevel level) => _minLogLevel = level;

    static Log()
    {
        // Создаем папку для логов при первом использовании
        if (!Directory.Exists(LogsFolderPath))
        {
            Directory.CreateDirectory(LogsFolderPath);
        }

        // Очищаем старые логи (сохраняем только последние 7 дней)
        CleanupOldLogs(7);

        // Записываем заголовок нового лог-файла
        WriteToFile($"=== Session started at {DateTime.Now:yyyy-MM-dd HH:mm:ss} ===");
        WriteToFile($"Unity version: {Application.unityVersion}");
        WriteToFile($"Platform: {Application.platform}");
        WriteToFile($"Product: {Application.productName}");
    }

    public static void Debug(string message) => LogMessage(LogLevel.Debug, message);
    public static void Info(string message) => LogMessage(LogLevel.Info, message);
    public static void Warning(string message) => LogMessage(LogLevel.Warning, message);
    public static void Error(string message) => LogMessage(LogLevel.Error, message);
    public static void Critical(string message) => LogMessage(LogLevel.Critical, message);

    private static void LogMessage(LogLevel level, string message)
    {
        if (level < _minLogLevel) return;

        string formattedMessage = $"[{DateTime.Now:HH:mm:ss.fff}] [{level}] {message}";

        // Запись в файл
        WriteToFile(formattedMessage);

        // Вывод в консоль Unity с цветами
#if UNITY_EDITOR
        string coloredMessage = GetColoredMessage(level, formattedMessage);
        UnityEngine.Debug.Log(coloredMessage);
#else
        UnityEngine.Debug.Log(formattedMessage);
#endif

        // Для критических ошибок - дополнительное оповещение
        if (level == LogLevel.Critical)
        {
            UnityEngine.Debug.LogError($"CRITICAL ERROR: {message}");
        }
    }

    private static void WriteToFile(string message)
    {
        try
        {
            File.AppendAllText(CurrentLogFilePath, message + Environment.NewLine, Encoding.UTF8);
        }
        catch (Exception ex)
        {
            UnityEngine.Debug.LogError($"Failed to write to log file: {ex.Message}");
        }
    }

    private static void CleanupOldLogs(int daysToKeep)
    {
        try
        {
            var cutoff = DateTime.Now.AddDays(-daysToKeep);
            foreach (var file in Directory.GetFiles(LogsFolderPath, "log_*.txt"))
            {
                var fileDate = ExtractDateFromFileName(file);
                if (fileDate < cutoff)
                {
                    File.Delete(file);
                }
            }
        }
        catch (Exception ex)
        {
            UnityEngine.Debug.LogWarning($"Log cleanup failed: {ex.Message}");
        }
    }

    private static DateTime ExtractDateFromFileName(string filePath)
    {
        try
        {
            string fileName = Path.GetFileNameWithoutExtension(filePath);
            string dateStr = fileName.Substring(4, 8); // Извлекаем дату из "log_YYYYMMDD.txt"
            return DateTime.ParseExact(dateStr, "yyyyMMdd", null);
        }
        catch
        {
            return DateTime.MinValue;
        }
    }

#if UNITY_EDITOR
    private static string GetColoredMessage(LogLevel level, string message)
    {
        return level switch
        {
            LogLevel.Debug => $"<color=#a0a0a0>{message}</color>",    // серый
            LogLevel.Info => $"<color=#ffffff>{message}</color>",     // белый
            LogLevel.Warning => $"<color=#ffff00>{message}</color>",  // желтый
            LogLevel.Error => $"<color=#ff8080>{message}</color>",    // красный
            LogLevel.Critical => $"<color=#ff0000><b>{message}</b></color>", // красный жирный
            _ => message
        };
    }
#endif
}