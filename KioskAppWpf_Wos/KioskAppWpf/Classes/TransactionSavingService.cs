using DbCommunication;
using DbCommunication.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.Xml.Serialization;

namespace KioskAppWpf.Classes
{
    class TransactionSavingService
    {
        private bool _importTransactionLogs
        {
            get
            {
                return ConfigurationManager.AppSettings["_importTransactionLogs"] == "1";
            }
        }
        private DbSavingServiceConfiguration Config { get; set; }
        private int KioskId { get; set; }
        private IDbConnection Connection { get; set; }
        private DbWrite DbWrite { get; set; }

        private List<Transaction> TransactionsToSave { get; set; }
        private static BackgroundWorker workerDbSaving = new BackgroundWorker();

        public TransactionSavingService(DbSavingServiceConfiguration config, int kioskId)
        {
            Config = config;
            KioskId = kioskId;
            TransactionsToSave = new List<Transaction>();
            RunDbSaveWorker();
        }

        private void RunDbSaveWorker()
        {
            workerDbSaving.DoWork -= WorkerDbSaveWork;
            workerDbSaving.DoWork += WorkerDbSaveWork;
            workerDbSaving.WorkerReportsProgress = true;

            if (!workerDbSaving.IsBusy)
                workerDbSaving.RunWorkerAsync();
        }
        private void WorkerDbSaveWork(object sender, DoWorkEventArgs e)
        {
            while (true)
            {
                try
                {
                    Connection = DbConnection.GetInstance(Config, KioskId);
                    DbWrite = new DbWrite(Connection);
                    TransactionsToSave = TransactionsToSave.Where(tran => !tran.Saved).ToList();

                    if (_importTransactionLogs)
                        CheckTransactionsToSaveFromBackup();

                    foreach (Transaction tran in TransactionsToSave)
                    {
                        tran.Saved = DbWrite.SaveTransaction(tran);
                    }
                    DbConnection.CloseInstance(Config);
                    DbWrite = null;
                }
                catch (Exception ex)
                {

                }

                Thread.Sleep(1000);
            }
        }

        private void CheckTransactionsToSaveFromBackup()
        {
            DirectoryInfo di = new DirectoryInfo(AssemblyDirectory());
            FileInfo[] Files = di.GetFiles("*.log.back");
            foreach (FileInfo f in Files)
            {
                var allTransactions = File.ReadAllText(f.FullName);
                var transactionXml = allTransactions.Split(new string[] { "------------" }, StringSplitOptions.None);
                foreach (string xml in transactionXml)
                {
                    var tran = DeserializeTransaction(xml.Replace("\r\n", string.Empty).Replace(@"<?xml version=""1.0"" encoding=""utf-16""?>", string.Empty));
                    if (tran.ActualAmount > 0)
                        TransactionsToSave.Add(tran);
                }

                Directory.CreateDirectory(di.FullName + "\\processed\\");
                File.Move(f.FullName, di.FullName + "\\processed\\" + f.Name);
            }
        }
        public string AssemblyDirectory()
        {
            string codeBase = Assembly.GetExecutingAssembly().CodeBase;
            UriBuilder uri = new UriBuilder(codeBase);
            string path = Uri.UnescapeDataString(uri.Path);
            return Path.GetDirectoryName(path);
        }
        public Transaction DeserializeTransaction(string xml)
        {
            var serializer = new XmlSerializer(typeof(Transaction));

            Transaction result;

            using (TextReader reader = new StringReader(xml))
            {
                result = (Transaction)serializer.Deserialize(reader);
            }

            return result;
        }

        public bool Enqueue(Transaction transaction)
        {
            var clonedTransaction = Clone(transaction);
            TransactionLogger tl = new TransactionLogger();
            tl.LogSavedTransaction(clonedTransaction);
            TransactionsToSave.Add(clonedTransaction);

            return true;
        }

        private Transaction Clone(Transaction transaction)
        {
            IFormatter formatter = new BinaryFormatter();
            Stream stream = new MemoryStream();
            using (stream)
            {
                formatter.Serialize(stream, transaction);
                stream.Seek(0, SeekOrigin.Begin);
                return (Transaction)formatter.Deserialize(stream);
            }
        }
    }
}
