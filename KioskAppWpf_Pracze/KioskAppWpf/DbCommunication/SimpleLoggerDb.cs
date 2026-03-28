using DbCommunication;
using System;
using System.Configuration;
using System.IO;

public class SimpleLoggerDb
{
    private string _logPath
    {
        get
        {
            return ConfigurationManager.AppSettings["_logPath"];
        }
    }
    private int _removeLogsAfterDays
    {
        get
        {
            return Int32.Parse(ConfigurationManager.AppSettings["_removeLogsAfterDays"]);
        }
    }

    private readonly string datetimeFormat;
    private readonly string logFilename;

    /// <summary>
    /// Initiate an instance of SimpleLogger class constructor.
    /// If log file does not exist, it will be created automatically.
    /// </summary>
    public SimpleLoggerDb()
    {
        datetimeFormat = "yyyy-MM-dd HH:mm:ss.fff";
        logFilename = _logPath + "KioskAppWpf_db_errors" + "_" + DateTime.Now.ToString("yyyy-MM-dd") + ".log";

        // Log file header line
        string logHeader = logFilename + " is created.";
        if (!System.IO.File.Exists(logFilename))
        {
            WriteLine(System.DateTime.Now.ToString(datetimeFormat) + " " + logHeader, false);
            ClearOldFiles("KioskAppWpf_db_errors");
        }
    }
    public SimpleLoggerDb(IDbConfiguration configuration)
    {
        string dbConnection = GetConnectionName(configuration);
        datetimeFormat = "yyyy-MM-dd HH:mm:ss.fff";
        logFilename = _logPath + "KioskAppWpf_db_errors" + "_" + dbConnection + "_" + DateTime.Now.ToString("yyyy-MM-dd") + ".log";

        // Log file header line
        string logHeader = logFilename + " is created.";
        if (!System.IO.File.Exists(logFilename))
        {
            WriteLine(System.DateTime.Now.ToString(datetimeFormat) + " " + logHeader, false);
            ClearOldFiles("KioskAppWpf_db_errors" + "_" + dbConnection);
        }
    }

    private void ClearOldFiles(string fileNamePart)
    {
        var files = Directory.GetFiles(_logPath, "*" + fileNamePart + "*.log");
        foreach (var file in files)
        {
            var modification = File.GetLastWriteTime(file);
            if (modification < DateTime.Now.AddDays(_removeLogsAfterDays * -1))
            {
                File.Delete(file);
            }
        }
    }

    private string GetConnectionName(IDbConfiguration configuration)
    {
        if (configuration is DbLocalConfiguration) return "LocalConnection";
        if (configuration is DbSyncConfiguration) return "SynchronizationConnection";
        if (configuration is DbServerConfiguration) return "ServerConnection";
        if (configuration is DbSavingServiceConfiguration) return "SavingServiceConnection";
        return "UnknownConnection";
    }

    /// <summary>
    /// Log a DEBUG message
    /// </summary>
    /// <param name="text">Message</param>
    public void Debug(string text)
    {
        WriteFormattedLog(LogLevel.DEBUG, text);
    }

    /// <summary>
    /// Log an ERROR message
    /// </summary>
    /// <param name="text">Message</param>
    public void Error(string text)
    {
        WriteFormattedLog(LogLevel.ERROR, text);
    }

    /// <summary>
    /// Log a FATAL ERROR message
    /// </summary>
    /// <param name="text">Message</param>
    public void Fatal(string text)
    {
        WriteFormattedLog(LogLevel.FATAL, text);
    }

    /// <summary>
    /// Log an INFO message
    /// </summary>
    /// <param name="text">Message</param>
    public void Info(string text)
    {
        WriteFormattedLog(LogLevel.INFO, text);
    }

    /// <summary>
    /// Log a TRACE message
    /// </summary>
    /// <param name="text">Message</param>
    public void Trace(string text)
    {
        WriteFormattedLog(LogLevel.TRACE, text);
    }

    /// <summary>
    /// Log a WARNING message
    /// </summary>
    /// <param name="text">Message</param>
    public void Warning(string text)
    {
        WriteFormattedLog(LogLevel.WARNING, text);
    }

    private void WriteFormattedLog(LogLevel level, string text)
    {
        string pretext;
        switch (level)
        {
            case LogLevel.TRACE:
                pretext = System.DateTime.Now.ToString(datetimeFormat) + " [TRACE]   ";
                break;
            case LogLevel.INFO:
                pretext = System.DateTime.Now.ToString(datetimeFormat) + " [INFO]    ";
                break;
            case LogLevel.DEBUG:
                pretext = System.DateTime.Now.ToString(datetimeFormat) + " [DEBUG]   ";
                break;
            case LogLevel.WARNING:
                pretext = System.DateTime.Now.ToString(datetimeFormat) + " [WARNING] ";
                break;
            case LogLevel.ERROR:
                pretext = System.DateTime.Now.ToString(datetimeFormat) + " [ERROR]   ";
                break;
            case LogLevel.FATAL:
                pretext = System.DateTime.Now.ToString(datetimeFormat) + " [FATAL]   ";
                break;
            default:
                pretext = "";
                break;
        }

        WriteLine(pretext + text);
    }

    private void WriteLine(string text, bool append = true)
    {
        try
        {
            using (System.IO.StreamWriter writer = new System.IO.StreamWriter(logFilename, append, System.Text.Encoding.UTF8))
            {
                if (text != "")
                {
                    writer.WriteLine(text);
                }
            }
        }
        catch
        {
            throw;
        }
    }

    [System.Flags]
    private enum LogLevel
    {
        TRACE,
        INFO,
        DEBUG,
        WARNING,
        ERROR,
        FATAL
    }
}