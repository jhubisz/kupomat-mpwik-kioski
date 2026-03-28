using System;
using DbCommunication.Entities;
using DbCommunication.Enums;
using Utilities.Classes;

namespace DbCommunication
{
    public class ExportManager
    {
        public KioskConfiguration KioskConfiguration { get; set; }

        public DbConnection Connection { get; set; }
        public DbRead DbRead { get; set; }
        public DbWrite DbWrite { get; set; }

        public DbConnection ServerConnection { get; set; }
        public DbServerRead DbServerRead { get; set; }
        public DbServerWrite DbServerWrite { get; set; }

        private ExportManager() { }
        public ExportManager(DbConnection connection, DbRead dbRead, DbWrite dbWrite, DbConnection serverConnection, DbServerRead dbServerRead, DbServerWrite dbServerWrite, KioskConfiguration kioskConfiguration)
        {
            KioskConfiguration = kioskConfiguration;

            Connection = connection;
            DbRead = dbRead;
            DbWrite = dbWrite;

            ServerConnection = serverConnection;
            DbServerRead = dbServerRead;
            DbServerWrite = dbServerWrite;
        }

        public void ServerSync(KioskConfiguration currenctConfiguration, PrinterPaperStatus currentPaperStatus)
        {
            KioskConfiguration = currenctConfiguration;
            ImportKioskData();
            ImportConfiguration();
            //ImportKioskWiadomosc();
            ExportPaperUsage(currentPaperStatus);

            ExportGminy();
            ExportMiejscowosci();
            ExportUlice();
            ExportAddresses();

            ExportFirmy();
            ExportFirmaAdresy();

            ExportStaliKlienci();

            ExportTransactions();
            ExportTransactionsAddresses();
            ExportTransactionsParameters();

            ExportProbaTacka();
            ExportProbaButelka();
            ExportProbaTackaParagon();
            
            ImportKlient();
            ImportSamochod();
            ImportKarta();
            ImportLicencja();
            ImportHarmonogram();

            ImportRodzajSciekow();

            ImportGminy();
            ImportMiejscowosci();
            ImportUlice();
            ImportAdresy();

            ImportFirmy();
            ImportFirmyAdresy();

            ImportStaliKlienci();
        }

        public void ExportGminy()
        {
            var gminyToExport = DbRead.GetGminaDbRowsToExport();
            if (gminyToExport.Count > 0)
            {
                foreach (var gminaToExport in gminyToExport)
                {
                    DbServerWrite.ExportGmina(gminaToExport);
                }
            }
        }
        public void ExportMiejscowosci()
        {
            var miejscowosciToExport = DbRead.GetMiejscowoscDbRowsToExport();
            if (miejscowosciToExport.Count > 0)
            {
                foreach (var miejscowoscToExport in miejscowosciToExport)
                {
                    DbServerWrite.ExportMiejscowosc(miejscowoscToExport);
                }
            }
        }
        public void ExportUlice()
        {
            var uliceToExport = DbRead.GetUlicaDbRowsToExport();
            if (uliceToExport.Count > 0)
            {
                foreach (var ulicaToExport in uliceToExport)
                {
                    DbServerWrite.ExportUlica(ulicaToExport);
                }
            }
        }
        public void ExportAddresses()
        {
            var addressesToExport = DbRead.GetAddressesDbRowsToExport();
            if (addressesToExport.Count >0)
            {
                foreach(var addressToExport in addressesToExport)
                {
                    DbServerWrite.ExportAddress(addressToExport);
                }
            }
        }

        public void ExportFirmy()
        {
            var firmyToExport = DbRead.GetFirmaDbRowsToExport();
            if (firmyToExport.Count > 0)
            {
                foreach (var firmaToExport in firmyToExport)
                {
                    DbServerWrite.ExportFirma(firmaToExport);
                }
            }
        }
        public void ExportFirmaAdresy()
        {
            var addressesToExport = DbRead.GetFirmaAdresDbRowsToExport();
            if (addressesToExport.Count > 0)
            {
                foreach (var addressToExport in addressesToExport)
                {
                    DbServerWrite.ExportFirmaAdres(addressToExport);
                }
            }
        }

        public void ExportStaliKlienci()
        {
            var rowsToExport = DbRead.GetStalyKlientDbRowsToExport();
            if (rowsToExport.Count > 0)
            {
                foreach (var rowToExport in rowsToExport)
                {
                    DbServerWrite.ExportStalyKlient(rowToExport);
                }
            }
        }

        public void ExportTransactions()
        {
            var transactionsToExport = DbRead.GetTransactionDbRowsToExport();
            if (transactionsToExport.Count > 0)
            {
                foreach (var transactionToExport in transactionsToExport)
                {
                    DbServerWrite.ExportTransaction(transactionToExport);
                }
            }
        }
        public void ExportTransactionsAddresses()
        {
            var transactionAddressesToExport = DbRead.GetTransactionAddressesDbRowsToExport();
            if (transactionAddressesToExport.Count > 0)
            {
                foreach (var transactionAddressToExport in transactionAddressesToExport)
                {
                    DbServerWrite.ExportTransactionAddress(transactionAddressToExport);
                }
            }
        }
        public void ExportTransactionsParameters()
        {
            var transactionsParametersToExport = DbRead.GetTransactionParametersDbRowsToExport();
            if (transactionsParametersToExport.Count > 0)
            {
                foreach (var transactionParametersToExport in transactionsParametersToExport)
                {
                    DbServerWrite.ExportTransactionParameters(transactionParametersToExport);
                }
            }
        }

        public void ExportProbaTacka()
        {
            var tackiToExport = DbRead.GetProbaTackaDbRowsToExport();
            if (tackiToExport.Count > 0)
            {
                foreach (var tackaToExport in tackiToExport)
                {
                    DbServerWrite.ExportProbaTacka(tackaToExport);
                }
            }
        }
        public void ExportProbaButelka()
        {
            var butelkiToExport = DbRead.GetProbaButelkaDbRowsToExport();
            if (butelkiToExport.Count > 0)
            {
                foreach (var butelkaToExport in butelkiToExport)
                {
                    DbServerWrite.ExportProbaButelka(butelkaToExport);
                }
            }
        }
        public void ExportProbaTackaParagon()
        {
            var paragonyToExport = DbRead.GetProbaTackaParagonDbRowsToExport();
            if (paragonyToExport.Count > 0)
            {
                foreach (var paragonToExport in paragonyToExport)
                {
                    DbServerWrite.ExportProbaTackaParagon(paragonToExport);
                }
            }
        }

        public void ExportPaperUsage(PrinterPaperStatus currentPaperStatus)
        {
            if (currentPaperStatus != null)
            {
                DbServerWrite.UpdatePaperUsage(currentPaperStatus);
            }
        }

        public void ImportConfiguration()
        {
            var confChangesPendingId = DbServerRead.CheckConfigurationUpdatePending();
            if (confChangesPendingId == 0)
                return;

            var config = DbServerRead.GetConfiguration(KioskConfiguration);
            if (config != null)
            {
                DbServerWrite.UpdateConfigurations(config);
                DbServerWrite.UpdateRowExported(confChangesPendingId);
            }
        }
        public void ImportKioskData()
        {
            var kioskDataPendingId = DbServerRead.CheckKioskDataUpdatePending();
            if (kioskDataPendingId == 0)
                return;

            var kiosk = DbServerRead.GetKioskData();
            if (kiosk != null)
            {
                DbServerWrite.UpdateKioskData(kiosk);
                DbServerWrite.UpdateRowExported(kioskDataPendingId);
            }
        }

        public void ImportKioskWiadomosc()
        {
            var rowsToImport = DbServerRead.GetKioskWiadomoscDbRowsToExport();
            if (rowsToImport.Count > 0)
            {
                foreach (var rowToImport in rowsToImport)
                {
                    if (rowToImport.ExportAction == ExportAction.Insert)
                        DbServerWrite.KioskWiadomoscAdd(rowToImport);
                    else
                        DbServerWrite.KioskWiadomoscUpdate(rowToImport);
                }
            }
        }

        public void ImportKlient()
        {
            var rowsToImport = DbServerRead.GetCustomerDbRowsToExport();
            if (rowsToImport.Count > 0)
            {
                foreach(var rowToImport in rowsToImport)
                {
                    if (rowToImport.ExportAction == ExportAction.Insert)
                        DbServerWrite.CustomerAdd(rowToImport);
                    else
                        DbServerWrite.CustomerUpdate(rowToImport);
                }
            }
        }

        public void ImportRodzajSciekow()
        {
            var rowsToImport = DbServerRead.GetCargoTypeDbRowsToExport();
            if (rowsToImport.Count > 0)
            {
                foreach (var rowToImport in rowsToImport)
                {
                    if (rowToImport.ExportAction == ExportAction.Insert)
                        DbServerWrite.CargoTypeAdd(rowToImport);
                    else
                        DbServerWrite.CargoTypeUpdate(rowToImport);
                }
            }
        }

        public void ImportSamochod()
        {
            var rowsToImport = DbServerRead.GetSamochodDbRowsToExport();
            if (rowsToImport.Count > 0)
            {
                foreach (var rowToImport in rowsToImport)
                {
                    if (rowToImport.ExportAction == ExportAction.Insert)
                        DbServerWrite.SamochodAdd(rowToImport);
                    else
                        DbServerWrite.SamochodUpdate(rowToImport);
                }
            }
        }

        public void ImportKarta()
        {
            var rowsToImport = DbServerRead.GetKartaDbRowsToExport();
            if (rowsToImport.Count > 0)
            {
                foreach (var rowToImport in rowsToImport)
                {
                    if (rowToImport.ExportAction == ExportAction.Insert)
                        DbServerWrite.KartaAdd(rowToImport);
                    else
                        DbServerWrite.KartaUpdate(rowToImport);
                }
            }
        }

        public void ImportLicencja()
        {
            var rowsToImport = DbServerRead.GetLicencjaDbRowsToExport();
            if (rowsToImport.Count > 0)
            {
                foreach (var rowToImport in rowsToImport)
                {
                    if (rowToImport.ExportAction == ExportAction.Insert)
                        DbServerWrite.LicencjaAdd(rowToImport);
                    else
                        DbServerWrite.LicencjaUpdate(rowToImport);
                }
            }
        }

        public void ImportHarmonogram()
        {
            var rowsToImport = DbServerRead.GetHarmonogramDbRowsToExport();
            if (rowsToImport.Count > 0)
            {
                foreach (var rowToImport in rowsToImport)
                {
                    if (rowToImport.ExportAction == ExportAction.Insert)
                        DbServerWrite.HarmonogramAdd(rowToImport);
                    else
                        DbServerWrite.HarmonogramUpdate(rowToImport);
                }
            }
        }

        public void ImportGminy()
        {
            var rowsToImport = DbServerRead.GetGminaDbRowsToExport();
            if (rowsToImport.Count > 0)
            {
                foreach (var rowToImport in rowsToImport)
                {
                    if (rowToImport.ExportAction == ExportAction.Insert)
                        DbServerWrite.GminaAdd(rowToImport);
                    else
                        DbServerWrite.GminaUpdate(rowToImport);
                }
            }
        }
        public void ImportMiejscowosci()
        {
            var rowsToImport = DbServerRead.GetMiejscowoscDbRowsToExport();
            if (rowsToImport.Count > 0)
            {
                foreach (var rowToImport in rowsToImport)
                {
                    if (rowToImport.ExportAction == ExportAction.Insert)
                        DbServerWrite.MiejscowoscAdd(rowToImport);
                    else
                        DbServerWrite.MiejscowoscUpdate(rowToImport);
                }
            }
        }
        public void ImportUlice()
        {
            var rowsToImport = DbServerRead.GetUliceDbRowsToExport();
            if (rowsToImport.Count > 0)
            {
                foreach (var rowToImport in rowsToImport)
                {
                    if (rowToImport.ExportAction == ExportAction.Insert)
                        DbServerWrite.UlicaAdd(rowToImport);
                    else
                        DbServerWrite.UlicaUpdate(rowToImport);
                }
            }
        }
        public void ImportAdresy()
        {
            var rowsToImport = DbServerRead.GetAdresyDbRowsToExport();
            if (rowsToImport.Count > 0)
            {
                foreach (var rowToImport in rowsToImport)
                {
                    if (rowToImport.ExportAction == ExportAction.Insert)
                        DbServerWrite.AdresAdd(rowToImport);
                    else
                        DbServerWrite.AdresUpdate(rowToImport);
                }
            }
        }

        public void ImportFirmy()
        {
            var rowsToImport = DbServerRead.GetFirmyDbRowsToExport();
            if (rowsToImport.Count > 0)
            {
                foreach (var rowToImport in rowsToImport)
                {
                    if (rowToImport.ExportAction == ExportAction.Insert)
                        DbServerWrite.FirmaAdd(rowToImport);
                    else
                        DbServerWrite.FirmaUpdate(rowToImport);
                }
            }
        }
        public void ImportFirmyAdresy()
        {
            var rowsToImport = DbServerRead.GetFirmaAdresyDbRowsToExport();
            if (rowsToImport.Count > 0)
            {
                foreach (var rowToImport in rowsToImport)
                {
                    if (rowToImport.ExportAction == ExportAction.Insert)
                        DbServerWrite.FirmaAdresAdd(rowToImport);
                    else
                        DbServerWrite.FirmaAdresUpdate(rowToImport);
                }
            }
        }

        public void ImportStaliKlienci()
        {
            var rowsToImport = DbServerRead.GetStalyKlientDbRowsToExport();
            if (rowsToImport.Count > 0)
            {
                foreach (var rowToImport in rowsToImport)
                {
                    if (rowToImport.ExportAction == ExportAction.Insert)
                        DbServerWrite.StalyKlientAdd(rowToImport);
                    else
                        DbServerWrite.StalyKlientUpdate(rowToImport);
                }
            }
        }

        public ResetApp? CheckResetApp()
        {
            return DbServerRead.GetResetApp();
        }
    }
}
