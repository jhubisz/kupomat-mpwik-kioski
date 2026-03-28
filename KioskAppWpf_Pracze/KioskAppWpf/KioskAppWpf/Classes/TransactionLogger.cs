using DbCommunication.Entities;
using System;
using System.Configuration;
using System.IO;
using System.Text;
using System.Xml.Serialization;

public class TransactionLogger
{
    private string _logPath
    {
        get
        {
            return ConfigurationManager.AppSettings["_logPath"];
        }
    }

    private readonly string datetimeFormat;
    private readonly string savedTransactionLogFilename;
    private readonly string unSavedTransactionLogFilename;

    /// <summary>
    /// Initiate an instance of SimpleLogger class constructor.
    /// If log file does not exist, it will be created automatically.
    /// </summary>
    public TransactionLogger()
    {
        datetimeFormat = "yyyy-MM-dd HH:mm:ss.fff";
        savedTransactionLogFilename = _logPath + "KioskAppWpf" + "_savedTransactions_" + DateTime.Now.ToString("yyyy-MM-dd") + ".log";
        unSavedTransactionLogFilename = _logPath + "KioskAppWpf" + "_unSavedTransactions_" + DateTime.Now.ToString("yyyy-MM-dd") + ".log";

        // Log file header line
        string logHeader = savedTransactionLogFilename + " is created.";
        if (!System.IO.File.Exists(savedTransactionLogFilename))
        {
            WriteLine(savedTransactionLogFilename, System.DateTime.Now.ToString(datetimeFormat) + " " + logHeader, false);
        }
        //if (!System.IO.File.Exists(unSavedTransactionLogFilename))
        //{
        //    WriteLine(unSavedTransactionLogFilename, System.DateTime.Now.ToString(datetimeFormat) + " " + logHeader, false);
        //}
    }

    public void LogSavedTransaction(Transaction transaction)
    {
        LogXmlTransaction(savedTransactionLogFilename, transaction);
    }
    public void LogUnSavedTransaction(Transaction transaction)
    {
        LogXmlTransaction(savedTransactionLogFilename, transaction);
    }

    private void LogXmlTransaction(string file, Transaction transaction)
    {
        XmlSerializer ser = new XmlSerializer(typeof(Transaction));
        StringBuilder sb = new StringBuilder();
        TextWriter writer = new StringWriter(sb);
        ser.Serialize(writer, transaction);

        WriteEmptyLine(file);
        WriteLine(file, "------------");
        WriteEmptyLine(file);
        WriteLine(file, System.DateTime.Now.ToString(datetimeFormat) + " ");
        WriteLine(file, sb.ToString());
    }

    private void WriteEmptyLine(string file)
    {
        try
        {
            using (System.IO.StreamWriter writer = new System.IO.StreamWriter(file, true, System.Text.Encoding.UTF8))
            {
                writer.WriteLine("");
            }
        }
        catch
        {
            throw;
        }
    }
    private void WriteLine(string file, string text, bool append = true)
    {
        try
        {
            using (System.IO.StreamWriter writer = new System.IO.StreamWriter(file, append, System.Text.Encoding.UTF8))
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
}