using DbCommunication.Entities;
using DbCommunication.Entities.DbRows;
using DbCommunication.Enums;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using Utilities.Classes;

namespace DbCommunication
{
    public class DbServerWrite
    {
        private IDbConnection ServerConnection { get; set; }
        private IDbConnection LocalConnection { get; set; }

        private DbServerWrite() { }
        public DbServerWrite(IDbConnection serverConnection, IDbConnection localConnection)
        {
            ServerConnection = serverConnection;
            LocalConnection = localConnection;
        }

        public bool ExportTransaction(TransactionDbRow transaction)
        {
            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@id", transaction.Id));
            parameters.Add(new SqlParameter("@kioskId", transaction.KioskId));
            parameters.Add(new SqlParameter("@rodzajSciekowId", transaction.RodzajSciekowId));
            parameters.Add(new SqlParameter("@klientId", transaction.KlientId));
            parameters.Add(new SqlParameter("@samochodId", transaction.SamochodId));
            parameters.Add(new SqlParameter("@kartaId", transaction.KartaId));
            if (!string.IsNullOrEmpty(transaction.NumerUmowy))
                parameters.Add(new SqlParameter("@numerUmowy", transaction.NumerUmowy));
            else
                parameters.Add(new SqlParameter("@numerUmowy", DBNull.Value));
            parameters.Add(new SqlParameter("@zadeklarowanaIlosc", transaction.ZadeklarowanaIlosc));
            parameters.Add(new SqlParameter("@zlanaIlosc", transaction.ZlanaIlosc));
            parameters.Add(new SqlParameter("@transakcjaStart", transaction.TransakcjaStart));
            parameters.Add(new SqlParameter("@zrzutStart", transaction.ZrzutStart));
            parameters.Add(new SqlParameter("@zrzutKoniec", transaction.ZrzutKoniec));
            parameters.Add(new SqlParameter("@transakcjaKoniec", transaction.TransakcjaKoniec));
            parameters.Add(new SqlParameter("@pobranoProbeHarm", transaction.PobranoProbeHarm));
            parameters.Add(new SqlParameter("@pobranoProbeAlrm", transaction.PobranoProbeAlrm));
            parameters.Add(new SqlParameter("@zakonczonaPoprawnie", transaction.ZakonczonaPoprawnie));
            if (!string.IsNullOrEmpty(transaction.PowodZakonczenia))
                parameters.Add(new SqlParameter("@powodZakonczenia", transaction.PowodZakonczenia));
            else
                parameters.Add(new SqlParameter("@powodZakonczenia", DBNull.Value));
            string query = "INSERT INTO [dbo].[Transakcja] ([Id],[KioskId],[RodzajSciekowId],[KlientId],[SamochodId],[KartaId],[NumerUmowy],[ZadeklarowanaIlosc],[ZlanaIlosc],[TransakcjaStart],[ZrzutStart],[ZrzutKoniec],[TransakcjaKoniec],[PobranoProbeHarm],[PobranoProbeAlrm],[ZakonczonaPoprawnie],[PowodZakonczenia]) output INSERTED.ID "
                         + "VALUES (@id, @kioskId, @rodzajSciekowId, @klientId, @samochodId, @kartaId, @numerUmowy, @zadeklarowanaIlosc, @zlanaIlosc, @transakcjaStart, @zrzutStart, @zrzutKoniec, @transakcjaKoniec, @pobranoProbeHarm, @pobranoProbeAlrm, @zakonczonaPoprawnie, @powodZakonczenia);";
            
            var localParameters = new List<SqlParameter>();
            localParameters.Add(new SqlParameter("@id", transaction.Id));
            string localQuery = "INSERT INTO [TransakcjaExport] (TransakcjaId, DataExportu) output INSERTED.ID "
                         + "VALUES (@id, GETDATE());";
            
            var sqlTransaction = ServerConnection.BeginTransaction();
            var result = ServerConnection.InsertRow(query, parameters, sqlTransaction);
            var sqlLocalTransaction = LocalConnection.BeginTransaction();
            var localResult = LocalConnection.InsertRow(localQuery, localParameters, sqlLocalTransaction);

            if ((int)result == 0 || (int)localResult == 0)
            {
                sqlTransaction.Rollback();
                sqlLocalTransaction.Rollback();
                return false;
            }

            sqlTransaction.Commit();
            sqlLocalTransaction.Commit();
            PositionForEmailSendout(transaction.Id);
            return true;
        }
        public void PositionForEmailSendout(int transactionId)
        {
            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@id", transactionId));
            string query = "INSERT INTO TransakcjaEmail (TransakcjaId, TransakcjaStart, Odbiorca, OdbiorcaNazwa) "
                         + "SELECT t.Id, t.TransakcjaStart, 'jakub.hubisz@gmail.com', 'Jakub Hubisz' "
                         + "FROM Transakcja t INNER JOIN Klient k ON t.KlientId = k.Id "
                         + "WHERE t.Id = @id";
            ServerConnection.InsertRow(query, parameters);
        }
        public bool ExportTransactionAddress(TransactionAddressDbRow transactionAddress)
        {
            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@id", transactionAddress.Id));
            parameters.Add(new SqlParameter("@kioskId", transactionAddress.KioskId));
            parameters.Add(new SqlParameter("@transakcjaId", transactionAddress.TransakcjaId));
            parameters.Add(new SqlParameter("@zadeklarowanaIlosc", transactionAddress.ZadeklarowanaIlosc));
            if (transactionAddress.ZlanaIlosc.HasValue)
                parameters.Add(new SqlParameter("@zlanaIlosc", transactionAddress.ZlanaIlosc.Value));
            else
                parameters.Add(new SqlParameter("@zlanaIlosc", DBNull.Value));
            if (!string.IsNullOrEmpty(transactionAddress.NumerUmowy))
                parameters.Add(new SqlParameter("@numerUmowy", transactionAddress.NumerUmowy));
            else
                parameters.Add(new SqlParameter("@numerUmowy", DBNull.Value));
            if (transactionAddress.AdresId.HasValue)
                parameters.Add(new SqlParameter("@adresId", transactionAddress.AdresId));
            else
                parameters.Add(new SqlParameter("@adresId", DBNull.Value));
            if (transactionAddress.FirmaId.HasValue)
                parameters.Add(new SqlParameter("@firmaId", transactionAddress.FirmaId));
            else
                parameters.Add(new SqlParameter("@firmaId", DBNull.Value));
            if (transactionAddress.RodId.HasValue)
                parameters.Add(new SqlParameter("@rodId", transactionAddress.RodId));
            else
                parameters.Add(new SqlParameter("@rodId", DBNull.Value));
            if (!string.IsNullOrEmpty(transactionAddress.RodNrDzialki))
                parameters.Add(new SqlParameter("@rodNrDzialki", transactionAddress.RodNrDzialki));
            else
                parameters.Add(new SqlParameter("@rodNrDzialki", DBNull.Value));
            string query = "INSERT INTO [dbo].[TransakcjaAdres] ([Id],[KioskId],[TransakcjaId],[ZadeklarowanaIlosc],[ZlanaIlosc],[NumerUmowy],[AdresId],[FirmaId],[RodId],[RodNrDzialki]) output INSERTED.ID "
                         + "VALUES (@id, @kioskId, @transakcjaId, @zadeklarowanaIlosc, @zlanaIlosc, @numerUmowy, @adresId, @firmaId, @rodId, @rodNrDzialki);";

            var localParameters = new List<SqlParameter>();
            localParameters.Add(new SqlParameter("@id", transactionAddress.Id));
            string localQuery = "INSERT INTO [TransakcjaAdresExport] (TransakcjaAdresId, DataExportu) output INSERTED.ID "
                         + "VALUES (@id, GETDATE());";

            var sqlTransaction = ServerConnection.BeginTransaction();
            var result = ServerConnection.InsertRow(query, parameters, sqlTransaction);
            var sqlLocalTransaction = LocalConnection.BeginTransaction();
            var localResult = LocalConnection.InsertRow(localQuery, localParameters, sqlLocalTransaction);

            if ((int)result == 0 || (int)localResult == 0)
            {
                sqlTransaction.Rollback();
                sqlLocalTransaction.Rollback();
                return false;
            }

            sqlTransaction.Commit();
            sqlLocalTransaction.Commit();
            return true;
        }
        public bool ExportTransactionParameters(TransactionParametersDbRow transactionParameters)
        {
            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@id", transactionParameters.Id));
            parameters.Add(new SqlParameter("@kioskId", transactionParameters.KioskId));
            parameters.Add(new SqlParameter("@transakcjaId", transactionParameters.TransakcjaId));
            if (transactionParameters.PhMin.HasValue)
                parameters.Add(new SqlParameter("@phMin", transactionParameters.PhMin));
            else
                parameters.Add(new SqlParameter("@phMin", DBNull.Value));
            if (transactionParameters.PhMax.HasValue)
                parameters.Add(new SqlParameter("@phMax", transactionParameters.PhMax));
            else
                parameters.Add(new SqlParameter("@phMax", DBNull.Value));
            if (transactionParameters.PhAvg.HasValue)
                parameters.Add(new SqlParameter("@phAvg", transactionParameters.PhAvg));
            else
                parameters.Add(new SqlParameter("@phAvg", DBNull.Value));

            if (transactionParameters.PrzewodnoscMin.HasValue)
                parameters.Add(new SqlParameter("@przewodnoscMin", transactionParameters.PrzewodnoscMin));
            else
                parameters.Add(new SqlParameter("@przewodnoscMin", DBNull.Value));
            if (transactionParameters.PrzewodnoscMax.HasValue)
                parameters.Add(new SqlParameter("@przewodnoscMax", transactionParameters.PrzewodnoscMax));
            else
                parameters.Add(new SqlParameter("@przewodnoscMax", DBNull.Value));
            if (transactionParameters.PrzewodnoscAvg.HasValue)
                parameters.Add(new SqlParameter("@przewodnoscAvg", transactionParameters.PrzewodnoscAvg));
            else
                parameters.Add(new SqlParameter("@przewodnoscAvg", DBNull.Value));

            if (transactionParameters.TempMin.HasValue)
                parameters.Add(new SqlParameter("@tempMin", transactionParameters.TempMin));
            else
                parameters.Add(new SqlParameter("@tempMin", DBNull.Value));
            if (transactionParameters.TempMax.HasValue)
                parameters.Add(new SqlParameter("@tempMax", transactionParameters.TempMax));
            else
                parameters.Add(new SqlParameter("@tempMax", DBNull.Value));
            if (transactionParameters.TempAvg.HasValue)
                parameters.Add(new SqlParameter("@tempAvg", transactionParameters.TempAvg));
            else
                parameters.Add(new SqlParameter("@tempAvg", DBNull.Value));

            if (transactionParameters.ChztMin.HasValue)
                parameters.Add(new SqlParameter("@chztMin", transactionParameters.ChztMin));
            else
                parameters.Add(new SqlParameter("@chztMin", DBNull.Value));
            if (transactionParameters.ChztMax.HasValue)
                parameters.Add(new SqlParameter("@chztMax", transactionParameters.ChztMax));
            else
                parameters.Add(new SqlParameter("@chztMax", DBNull.Value));
            if (transactionParameters.ChztAvg.HasValue)
                parameters.Add(new SqlParameter("@chztAvg", transactionParameters.ChztAvg));
            else
                parameters.Add(new SqlParameter("@chztAvg", DBNull.Value));

            if (transactionParameters.ChztAvg.HasValue)
                parameters.Add(new SqlParameter("@cisnienie", transactionParameters.Cisnienie));
            else
                parameters.Add(new SqlParameter("@cisnienie", DBNull.Value));

            string query = "INSERT INTO [dbo].[TransakcjaParametry] ([Id],[KioskId],[TransakcjaId],[PhMin],[PhMax],[PhAvg],[PrzewodnoscMin],[PrzewodnoscMax],[PrzewodnoscAvg],[TempMin],[TempMax],[TempAvg],[ChztMin],[ChztMax],[ChztAvg],[Cisnienie]) output INSERTED.ID "
                         + "VALUES (@id, @kioskId, @transakcjaId, @phMin, @phMax, @phAvg, @przewodnoscMin, @przewodnoscMax, @przewodnoscAvg, @tempMin, @tempMax, @tempAvg, @chztMin, @chztMax, @chztAvg, @cisnienie);";

            var localParameters = new List<SqlParameter>();
            localParameters.Add(new SqlParameter("@id", transactionParameters.Id));
            string localQuery = "INSERT INTO [TransakcjaParametryExport] (TransakcjaParametryId, DataExportu) output INSERTED.ID "
                         + "VALUES (@id, GETDATE());";

            var sqlTransaction = ServerConnection.BeginTransaction();
            var result = ServerConnection.InsertRow(query, parameters, sqlTransaction);
            var sqlLocalTransaction = LocalConnection.BeginTransaction();
            var localResult = LocalConnection.InsertRow(localQuery, localParameters, sqlLocalTransaction);

            if ((int)result == 0 || (int)localResult == 0)
            {
                sqlTransaction.Rollback();
                sqlLocalTransaction.Rollback();
                return false;
            }

            sqlTransaction.Commit();
            sqlLocalTransaction.Commit();
            return true;
        }

        public bool ExportGmina(GminaDbRow gmina)
        {
            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@id", gmina.Id));
            parameters.Add(new SqlParameter("@kioskId", gmina.KioskId));
            parameters.Add(new SqlParameter("@powiatId", gmina.PowiatId));
            parameters.Add(new SqlParameter("@nazwa", gmina.Nazwa));
            parameters.Add(new SqlParameter("@rodzajId", gmina.RodzajId));
            parameters.Add(new SqlParameter("@dataStart", gmina.DataStart));
            string query = "EXECUTE [dbo].[_sp_add_from_kiosk_gmina] @id,@kioskId,@powiatId,@nazwa,@rodzajId,@dataStart;";

            var localParameters = new List<SqlParameter>();
            localParameters.Add(new SqlParameter("@id", gmina.Id));
            string localQuery = "INSERT INTO [GminaExport] (GminaId, DataExportu) output INSERTED.ID "
                              + "VALUES (@id, GETDATE());";

            var sqlTransaction = ServerConnection.BeginTransaction();
            var result = ServerConnection.InsertRow(query, parameters, sqlTransaction);
            var sqlLocalTransaction = LocalConnection.BeginTransaction();
            var localResult = LocalConnection.InsertRow(localQuery, localParameters, sqlLocalTransaction);

            if ((int)result == 0 || (int)localResult == 0)
            {
                sqlTransaction.Rollback();
                sqlLocalTransaction.Rollback();
                return false;
            }

            sqlTransaction.Commit();
            sqlLocalTransaction.Commit();
            return true;
        }
        public bool ExportMiejscowosc(MiejscowoscDbRow gmina)
        {
            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@id", gmina.Id));
            parameters.Add(new SqlParameter("@kioskId", gmina.KioskId));
            parameters.Add(new SqlParameter("@nazwa", gmina.Nazwa));
            parameters.Add(new SqlParameter("@gminaId", gmina.GminaId));
            parameters.Add(new SqlParameter("@rodzajId", gmina.RodzajId));
            parameters.Add(new SqlParameter("@dataStart", gmina.DataStart));
            string query = "EXECUTE [dbo].[_sp_add_from_kiosk_miejscowosc] @id,@kioskId,@nazwa,@gminaId,@rodzajId,@dataStart;";

            var localParameters = new List<SqlParameter>();
            localParameters.Add(new SqlParameter("@id", gmina.Id));
            string localQuery = "INSERT INTO [MiejscowoscExport] (MiejscowoscId, DataExportu) output INSERTED.ID "
                              + "VALUES (@id, GETDATE());";

            var sqlTransaction = ServerConnection.BeginTransaction();
            var result = ServerConnection.InsertRow(query, parameters, sqlTransaction);
            var sqlLocalTransaction = LocalConnection.BeginTransaction();
            var localResult = LocalConnection.InsertRow(localQuery, localParameters, sqlLocalTransaction);

            if ((int)result == 0 || (int)localResult == 0)
            {
                sqlTransaction.Rollback();
                sqlLocalTransaction.Rollback();
                return false;
            }

            sqlTransaction.Commit();
            sqlLocalTransaction.Commit();
            return true;
        }
        public bool ExportUlica(UlicaDbRow ulica)
        {
            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@id", ulica.Id));
            parameters.Add(new SqlParameter("@kioskId", ulica.KioskId));
            parameters.Add(new SqlParameter("@nazwa", ulica.Nazwa));
            parameters.Add(new SqlParameter("@miejscowoscId", ulica.MiejscowoscId));
            parameters.Add(new SqlParameter("@gminaId", ulica.GminaId));
            parameters.Add(new SqlParameter("@rodzajId", ulica.RodzajId));
            parameters.Add(new SqlParameter("@dataStart", ulica.DataStart));
            string query = "EXECUTE [dbo].[_sp_add_from_kiosk_ulica] @id,@kioskId,@nazwa,@miejscowoscId,@gminaId,@rodzajId,@dataStart;";

            var localParameters = new List<SqlParameter>();
            localParameters.Add(new SqlParameter("@id", ulica.Id));
            string localQuery = "INSERT INTO [UlicaExport] (UlicaId, DataExportu) output INSERTED.ID "
                              + "VALUES (@id, GETDATE());";

            var sqlTransaction = ServerConnection.BeginTransaction();
            var result = ServerConnection.InsertRow(query, parameters, sqlTransaction);
            var sqlLocalTransaction = LocalConnection.BeginTransaction();
            var localResult = LocalConnection.InsertRow(localQuery, localParameters, sqlLocalTransaction);

            if ((int)result == 0 || (int)localResult == 0)
            {
                sqlTransaction.Rollback();
                sqlLocalTransaction.Rollback();
                return false;
            }

            sqlTransaction.Commit();
            sqlLocalTransaction.Commit();
            return true;
        }

        public bool ExportFirma(FirmaDbRow firma)
        {
            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@id", firma.Id));
            parameters.Add(new SqlParameter("@kioskId", firma.KioskId));
            parameters.Add(new SqlParameter("@nazwa", firma.Nazwa));
            parameters.Add(new SqlParameter("@dataStart", firma.DataStart));
            string query = "EXECUTE [dbo].[_sp_add_from_kiosk_firma] @id,@kioskId,@nazwa,@dataStart;";

            var localParameters = new List<SqlParameter>();
            localParameters.Add(new SqlParameter("@id", firma.Id));
            string localQuery = "INSERT INTO [FirmaExport] (FirmaId, DataExportu) output INSERTED.ID "
                              + "VALUES (@id, GETDATE());";

            var sqlTransaction = ServerConnection.BeginTransaction();
            var result = ServerConnection.InsertRow(query, parameters, sqlTransaction);
            var sqlLocalTransaction = LocalConnection.BeginTransaction();
            var localResult = LocalConnection.InsertRow(localQuery, localParameters, sqlLocalTransaction);

            if ((int)result == 0 || (int)localResult == 0)
            {
                sqlTransaction.Rollback();
                sqlLocalTransaction.Rollback();
                return false;
            }

            sqlTransaction.Commit();
            sqlLocalTransaction.Commit();
            return true;
        }
        public bool ExportFirmaAdres(FirmaAdresDbRow firma)
        {
            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@id", firma.Id));
            parameters.Add(new SqlParameter("@kioskId", firma.KioskId));
            parameters.Add(new SqlParameter("@adresId", firma.AdresId));
            parameters.Add(new SqlParameter("@firmaId", firma.FirmaId));
            parameters.Add(new SqlParameter("@dataStart", firma.DataStart));
            string query = "EXECUTE [dbo].[_sp_add_from_kiosk_firmaAdres] @id,@kioskId,@adresId,@firmaId,@dataStart;";

            var localParameters = new List<SqlParameter>();
            localParameters.Add(new SqlParameter("@id", firma.Id));
            string localQuery = "INSERT INTO [FirmaAdresExport] (FirmaAdresId, DataExportu) output INSERTED.ID "
                              + "VALUES (@id, GETDATE());";

            var sqlTransaction = ServerConnection.BeginTransaction();
            var result = ServerConnection.InsertRow(query, parameters, sqlTransaction);
            var sqlLocalTransaction = LocalConnection.BeginTransaction();
            var localResult = LocalConnection.InsertRow(localQuery, localParameters, sqlLocalTransaction);

            if ((int)result == 0 || (int)localResult == 0)
            {
                sqlTransaction.Rollback();
                sqlLocalTransaction.Rollback();
                return false;
            }

            sqlTransaction.Commit();
            sqlLocalTransaction.Commit();
            return true;
        }

        public bool ExportStalyKlient(StalyKlientDbRow klient)
        {
            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@id", klient.Id));
            parameters.Add(new SqlParameter("@kioskId", klient.KioskId));
            parameters.Add(new SqlParameter("@adresId", klient.AdresId));
            if (klient.FirmaId.HasValue)
                parameters.Add(new SqlParameter("@firmaId", klient.FirmaId.Value));
            else
                parameters.Add(new SqlParameter("@firmaId", DBNull.Value));
            parameters.Add(new SqlParameter("@klientId", klient.KlientId));
            parameters.Add(new SqlParameter("@dataStart", klient.DataStart));
            string query = "EXECUTE [dbo].[_sp_add_from_kiosk_stalyKlient] @id,@kioskId,@adresId,@firmaId,@klientId,@dataStart;";

            var localParameters = new List<SqlParameter>();
            localParameters.Add(new SqlParameter("@id", klient.Id));
            string localQuery = "INSERT INTO [StalyKlientExport] (StalyKlientId, DataExportu) output INSERTED.ID "
                              + "VALUES (@id, GETDATE());";

            var sqlTransaction = ServerConnection.BeginTransaction();
            var result = ServerConnection.InsertRow(query, parameters, sqlTransaction);
            var sqlLocalTransaction = LocalConnection.BeginTransaction();
            var localResult = LocalConnection.InsertRow(localQuery, localParameters, sqlLocalTransaction);

            if ((int)result == 0 || (int)localResult == 0)
            {
                sqlTransaction.Rollback();
                sqlLocalTransaction.Rollback();
                return false;
            }

            sqlTransaction.Commit();
            sqlLocalTransaction.Commit();
            return true;
        }

        public bool ExportAddress(AdresDbRow address)
        {
            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@id", address.Id));
            parameters.Add(new SqlParameter("@kioskId", address.KioskId));
            parameters.Add(new SqlParameter("@nazwa", address.Nazwa));
            if (address.UlicaId.HasValue)
                parameters.Add(new SqlParameter("@ulicaId", address.UlicaId));
            else
                parameters.Add(new SqlParameter("@ulicaId", DBNull.Value));
            parameters.Add(new SqlParameter("@miejscowoscId", address.MiejscowoscId));
            parameters.Add(new SqlParameter("@gminaId", address.GminaId));
            parameters.Add(new SqlParameter("@numer", address.Numer));
            if (!string.IsNullOrEmpty(address.NumerUmowy))
                parameters.Add(new SqlParameter("@numerUmowy", address.NumerUmowy));
            else
                parameters.Add(new SqlParameter("@numerUmowy", DBNull.Value));
            parameters.Add(new SqlParameter("@rodzajId", address.RodzajId));
            parameters.Add(new SqlParameter("@dataStart", address.DataStart));
            string query = "EXECUTE [_sp_add_from_kiosk_adres] @id, @kioskId, @nazwa, @ulicaId, @miejscowoscId, @gminaId, @numer, @numerUmowy, @rodzajId, @dataStart;";

            var localParameters = new List<SqlParameter>();
            localParameters.Add(new SqlParameter("@id", address.Id));
            string localQuery = "INSERT INTO [AdresExport] (AdresId, DataExportu) output INSERTED.ID "
                              + "VALUES (@id, GETDATE());";

            var sqlTransaction = ServerConnection.BeginTransaction();
            var result = ServerConnection.InsertRow(query, parameters, sqlTransaction);
            var sqlLocalTransaction = LocalConnection.BeginTransaction();
            var localResult = LocalConnection.InsertRow(localQuery, localParameters, sqlLocalTransaction);

            if ((int)result == 0 || (int)localResult == 0)
            {
                sqlTransaction.Rollback();
                sqlLocalTransaction.Rollback();
                return false;
            }

            sqlTransaction.Commit();
            sqlLocalTransaction.Commit();
            return true;
        }
        
        public bool ExportProbaTacka(ProbaTackaDbRow tacka)
        {
            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@id", tacka.Id));
            parameters.Add(new SqlParameter("@kioskId", tacka.KioskId));
            parameters.Add(new SqlParameter("@tackaNr", tacka.TackaNr));
            parameters.Add(new SqlParameter("@dataStart", tacka.DataStart));
            string query = "EXECUTE [dbo].[_sp_add_from_kiosk_probaTacka] @id,@kioskId,@tackaNr,@dataStart;";

            var localParameters = new List<SqlParameter>();
            localParameters.Add(new SqlParameter("@id", tacka.Id));
            string localQuery = "INSERT INTO [ProbaTackaExport] (ProbaTackaId, DataExportu) output INSERTED.ID "
                              + "VALUES (@id, GETDATE());";

            var sqlTransaction = ServerConnection.BeginTransaction();
            var result = ServerConnection.InsertRow(query, parameters, sqlTransaction);
            var sqlLocalTransaction = LocalConnection.BeginTransaction();
            var localResult = LocalConnection.InsertRow(localQuery, localParameters, sqlLocalTransaction);

            if ((int)result == 0 || (int)localResult == 0)
            {
                sqlTransaction.Rollback();
                sqlLocalTransaction.Rollback();
                return false;
            }

            sqlTransaction.Commit();
            sqlLocalTransaction.Commit();
            return true;
        }
        public bool ExportProbaButelka(ProbaButelkaDbRow butelka)
        {
            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@id", butelka.Id));
            parameters.Add(new SqlParameter("@kioskId", butelka.KioskId));
            parameters.Add(new SqlParameter("@tackaId", butelka.TackaId));
            parameters.Add(new SqlParameter("@numerButelki", butelka.NumerButelki));
            parameters.Add(new SqlParameter("@probaPobrana", butelka.ProbaPobrana));
            parameters.Add(new SqlParameter("@probaData", butelka.ProbaData));
            parameters.Add(new SqlParameter("@probaTypId", butelka.ProbaTypId));
            parameters.Add(new SqlParameter("@probaTransakcjaId", butelka.ProbaTransakcjaId));
            parameters.Add(new SqlParameter("@dataStart", butelka.DataStart));
            string query = "INSERT INTO [dbo].[ProbaButelka] ([Id],[KioskId],[TackaId],[NumerButelki],[ProbaPobrana],[ProbaData],[ProbaTypId],[ProbaTransakcjaId],[DataStart],[DataKoniec],[Deleted]) output INSERTED.ID "
                         + "VALUES (@id, @kioskId, @tackaId, @numerButelki, @probaPobrana, @probaData, @probaTypId, @probaTransakcjaId, @dataStart, NULL, 0);";

            var localParameters = new List<SqlParameter>();
            localParameters.Add(new SqlParameter("@id", butelka.Id));
            string localQuery = "INSERT INTO [ProbaButelkaExport] (ProbaButelkaId, DataExportu) output INSERTED.ID "
                              + "VALUES (@id, GETDATE());";

            var sqlTransaction = ServerConnection.BeginTransaction();
            var result = ServerConnection.InsertRow(query, parameters, sqlTransaction);
            var sqlLocalTransaction = LocalConnection.BeginTransaction();
            var localResult = LocalConnection.InsertRow(localQuery, localParameters, sqlLocalTransaction);

            if ((int)result == 0 || (int)localResult == 0)
            {
                sqlTransaction.Rollback();
                sqlLocalTransaction.Rollback();
                return false;
            }

            sqlTransaction.Commit();
            sqlLocalTransaction.Commit();
            return true;
        }
        public bool ExportProbaTackaParagon(ProbaTackaParagonDbRow paragon)
        {
            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@id", paragon.Id));
            parameters.Add(new SqlParameter("@kioskId", paragon.KioskId));
            parameters.Add(new SqlParameter("@tackaId", paragon.TackaId));
            parameters.Add(new SqlParameter("@liczbaProbHarm", paragon.LiczbaProbHarm));
            parameters.Add(new SqlParameter("@liczbaProbAlrm", paragon.LiczbaProbAlrm));
            parameters.Add(new SqlParameter("@wydrukowano", paragon.Wydrukowano));
            if (paragon.DataWydruku.HasValue)
                parameters.Add(new SqlParameter("@dataWydruku", paragon.DataWydruku));
            else
                parameters.Add(new SqlParameter("@dataWydruku", DBNull.Value));
            string query = "INSERT INTO [dbo].[ProbaTackaParagon] ([Id],[KioskId],[TackaId],[LiczbaProbHarm],[LiczbaProbAlrm],[Wydrukowano],[DataWydruku]) output INSERTED.ID "
                         + "VALUES (@id, @kioskId, @tackaId, @liczbaProbHarm, @liczbaProbAlrm, @wydrukowano, @dataWydruku);";

            var localParameters = new List<SqlParameter>();
            localParameters.Add(new SqlParameter("@id", paragon.Id));
            string localQuery = "INSERT INTO [ProbaTackaParagonExport] (ProbaTackaParagonId, DataExportu) output INSERTED.ID "
                              + "VALUES (@id, GETDATE());";

            var sqlTransaction = ServerConnection.BeginTransaction();
            var result = ServerConnection.InsertRow(query, parameters, sqlTransaction);
            var sqlLocalTransaction = LocalConnection.BeginTransaction();
            var localResult = LocalConnection.InsertRow(localQuery, localParameters, sqlLocalTransaction);

            if ((int)result == 0 || (int)localResult == 0)
            {
                sqlTransaction.Rollback();
                sqlLocalTransaction.Rollback();
                return false;
            }

            sqlTransaction.Commit();
            sqlLocalTransaction.Commit();
            return true;
        }

        public bool KioskWiadomoscUpdate(KioskWiadomoscDbRow kioskWiadomosc)
        { 
            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@id", kioskWiadomosc.Id));
            parameters.Add(new SqlParameter("@kioskId", kioskWiadomosc.KioskId));
            parameters.Add(new SqlParameter("@wiadomosc", kioskWiadomosc.Wiadomosc));
            parameters.Add(new SqlParameter("@dataStart", kioskWiadomosc.DataStart));
            if (kioskWiadomosc.DataKoniec.HasValue)
                parameters.Add(new SqlParameter("@dataKoniec", kioskWiadomosc.DataKoniec));
            else
                parameters.Add(new SqlParameter("@dataKoniec", DBNull.Value));
            string query = query = "UPDATE [KioskWiadomosc] "
                                 + "SET Wiadomosc = @wiadomosc, DataStart = @dataStart, DataKoniec = @dataKoniec "
                                 + "WHERE Id = @id;";

            var serverParameters = new List<SqlParameter>();
            serverParameters.Add(new SqlParameter("@exportRowId", kioskWiadomosc.ExportRowId));
            serverParameters.Add(new SqlParameter("@kioskId", LocalConnection.KioskId));
            string localQuery = "INSERT INTO [ExportRowExported] (ExportRowId, KioskId, ExportedDate) output INSERTED.ID "
                              + "VALUES (@exportRowId, @kioskId, GetDate());";

            var sqlTransaction = LocalConnection.BeginTransaction();
            var result = LocalConnection.UpdateRow(query, parameters, sqlTransaction);
            var sqlServerTransaction = ServerConnection.BeginTransaction();
            var localResult = ServerConnection.InsertRow(localQuery, serverParameters, sqlServerTransaction);

            if ((int)result == 0 || (int)localResult == 0)
            {
                sqlTransaction.Rollback();
                sqlServerTransaction.Rollback();
                return false;
            }

            sqlTransaction.Commit();
            sqlServerTransaction.Commit();
            return true;
        }
        public bool KioskWiadomoscAdd(KioskWiadomoscDbRow kioskWiadomosc)
        {
            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@id", kioskWiadomosc.Id));
            parameters.Add(new SqlParameter("@kioskId", kioskWiadomosc.KioskId));
            parameters.Add(new SqlParameter("@wiadomosc", kioskWiadomosc.Wiadomosc));
            parameters.Add(new SqlParameter("@dataStart", kioskWiadomosc.DataStart));
            if (kioskWiadomosc.DataKoniec.HasValue)
                parameters.Add(new SqlParameter("@dataKoniec", kioskWiadomosc.DataKoniec));
            else
                parameters.Add(new SqlParameter("@dataKoniec", DBNull.Value));
            string query = query = "INSERT INTO [dbo].[KioskWiadomosc]([Id],[KioskId],[Wiadomosc],[DataStart],[DataKoniec]) output INSERTED.ID "
                                 + "VALUES (@id, @kioskId, @wiadomosc, @dataStart, @dataKoniec);";

            var serverParameters = new List<SqlParameter>();
            serverParameters.Add(new SqlParameter("@exportRowId", kioskWiadomosc.ExportRowId));
            serverParameters.Add(new SqlParameter("@kioskId", LocalConnection.KioskId));
            string localQuery = "INSERT INTO [ExportRowExported] (ExportRowId, KioskId, ExportedDate) output INSERTED.ID "
                              + "VALUES (@exportRowId, @kioskId, GetDate());";

            var sqlTransaction = LocalConnection.BeginTransaction();
            var result = LocalConnection.InsertRow(query, parameters, sqlTransaction);
            var sqlServerTransaction = ServerConnection.BeginTransaction();
            var localResult = ServerConnection.InsertRow(localQuery, serverParameters, sqlServerTransaction);

            if ((int)result == 0 || (int)localResult == 0)
            {
                sqlTransaction.Rollback();
                sqlServerTransaction.Rollback();
                return false;
            }

            sqlTransaction.Commit();
            sqlServerTransaction.Commit();
            return true;
        }

        public bool CustomerUpdate(KlientDbRow customer)
        {
            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@id", customer.Id));
            parameters.Add(new SqlParameter("@nazwa", customer.Nazwa));
            parameters.Add(new SqlParameter("@nip", customer.Nip));
            parameters.Add(new SqlParameter("@regon", customer.Regon));
            parameters.Add(new SqlParameter("@kodPocztowy", customer.KodPocztowy));
            parameters.Add(new SqlParameter("@miejscowosc", customer.Miejscowosc));
            parameters.Add(new SqlParameter("@adresLinia1", customer.AdresLinia1));
            parameters.Add(new SqlParameter("@adresLinia2", customer.AdresLinia2));
            parameters.Add(new SqlParameter("@tel1", customer.Tel1));
            parameters.Add(new SqlParameter("@tel2", customer.Tel2));
            parameters.Add(new SqlParameter("@numerUmowy", customer.NumerUmowy));
            parameters.Add(new SqlParameter("@pobieranieProbek", customer.PobieranieProbek));
            parameters.Add(new SqlParameter("@blokada", customer.Blokada));
            parameters.Add(new SqlParameter("@dataStart", customer.DataStart));
            if (customer.DataKoniec.HasValue)
                parameters.Add(new SqlParameter("@dataKoniec", customer.DataKoniec));
            else
                parameters.Add(new SqlParameter("@dataKoniec", DBNull.Value));
            parameters.Add(new SqlParameter("@deleted", customer.Deleted));
            string query = query = "UPDATE [Klient] "
                                 + "SET Nazwa = @nazwa, NIP = @nip, REGON = @regon, KodPocztowy = @kodPocztowy, Miejscowosc = @miejscowosc "
                                 + "    , AdresLinia1 = @adresLinia1, AdresLinia2 = @adresLinia2, Tel1 = @tel1, Tel2 = @tel2, NumerUmowy = @numerUmowy "
                                 + "    , PobieranieProbek = @pobieranieProbek, Blokada = @blokada, DataStart = @dataStart, DataKoniec = @dataKoniec, Deleted = @deleted "
                                 + "WHERE Id = @id;";

            var serverParameters = new List<SqlParameter>();
            serverParameters.Add(new SqlParameter("@exportRowId", customer.ExportRowId));
            serverParameters.Add(new SqlParameter("@kioskId", LocalConnection.KioskId));
            string localQuery = "INSERT INTO [ExportRowExported] (ExportRowId, KioskId, ExportedDate) output INSERTED.ID "
                              + "VALUES (@exportRowId, @kioskId, GetDate());";

            var sqlTransaction = LocalConnection.BeginTransaction();
            var result = LocalConnection.UpdateRow(query, parameters, sqlTransaction);
            var sqlServerTransaction = ServerConnection.BeginTransaction();
            var localResult = ServerConnection.InsertRow(localQuery, serverParameters, sqlServerTransaction);

            if ((int)result == 0 || (int)localResult == 0)
            {
                sqlTransaction.Rollback();
                sqlServerTransaction.Rollback();
                return false;
            }

            sqlTransaction.Commit();
            sqlServerTransaction.Commit();
            return true;
        }
        public bool CustomerAdd(KlientDbRow customer)
        {
            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@id", customer.Id));
            parameters.Add(new SqlParameter("@nazwa", customer.Nazwa));
            parameters.Add(new SqlParameter("@nip", customer.Nip));
            parameters.Add(new SqlParameter("@regon", customer.Regon));
            parameters.Add(new SqlParameter("@kodPocztowy", customer.KodPocztowy));
            parameters.Add(new SqlParameter("@miejscowosc", customer.Miejscowosc));
            parameters.Add(new SqlParameter("@adresLinia1", customer.AdresLinia1));
            parameters.Add(new SqlParameter("@adresLinia2", customer.AdresLinia2));
            parameters.Add(new SqlParameter("@tel1", customer.Tel1));
            parameters.Add(new SqlParameter("@tel2", customer.Tel2));
            parameters.Add(new SqlParameter("@numerUmowy", customer.NumerUmowy));
            parameters.Add(new SqlParameter("@pobieranieProbek", customer.PobieranieProbek));
            parameters.Add(new SqlParameter("@blokada", customer.Blokada));
            parameters.Add(new SqlParameter("@dataStart", customer.DataStart));
            if (customer.DataKoniec.HasValue)
                parameters.Add(new SqlParameter("@dataKoniec", customer.DataKoniec));
            else
                parameters.Add(new SqlParameter("@dataKoniec", DBNull.Value));
            parameters.Add(new SqlParameter("@deleted", customer.Deleted));
            string query = query = "INSERT INTO [dbo].[Klient]([Id],[Nazwa],[NIP],[REGON],[KodPocztowy],[Miejscowosc],[AdresLinia1],[AdresLinia2],[Tel1],[Tel2],[NumerUmowy],[PobieranieProbek],[Blokada],[DataStart],[DataKoniec],[Deleted]) output INSERTED.ID "
                                 + "VALUES (@id, @nazwa, @nip, @regon, @kodPocztowy, @miejscowosc, @adresLinia1, @adresLinia2, @tel1, @tel2, @numerUmowy, @pobieranieProbek, @blokada, @dataStart, @dataKoniec, @deleted);";

            var serverParameters = new List<SqlParameter>();
            serverParameters.Add(new SqlParameter("@exportRowId", customer.ExportRowId));
            serverParameters.Add(new SqlParameter("@kioskId", LocalConnection.KioskId));
            string localQuery = "INSERT INTO [ExportRowExported] (ExportRowId, KioskId, ExportedDate) output INSERTED.ID "
                              + "VALUES (@exportRowId, @kioskId, GetDate());";

            var sqlTransaction = LocalConnection.BeginTransaction();
            var result = LocalConnection.InsertRow(query, parameters, sqlTransaction);
            var sqlServerTransaction = ServerConnection.BeginTransaction();
            var localResult = ServerConnection.InsertRow(localQuery, serverParameters, sqlServerTransaction);

            if ((int)result == 0 || (int)localResult == 0)
            {
                sqlTransaction.Rollback();
                sqlServerTransaction.Rollback();
                return false;
            }

            sqlTransaction.Commit();
            sqlServerTransaction.Commit();
            return true;
        }

        public bool AddressUpdate(AdresDbRow address)
        {
            //var parameters = new List<SqlParameter>();
            //parameters.Add(new SqlParameter("@id", address.Id));
            //parameters.Add(new SqlParameter("@customerId", address.CustomerId));
            //parameters.Add(new SqlParameter("@name", address.Name));
            //parameters.Add(new SqlParameter("@nip", address.Nip));
            //parameters.Add(new SqlParameter("@regon", address.Regon));
            //parameters.Add(new SqlParameter("@postCode", address.PostCode));
            //parameters.Add(new SqlParameter("@city", address.City));
            //parameters.Add(new SqlParameter("@addrLine1", address.AddressLine1));
            //parameters.Add(new SqlParameter("@addrLine2", address.AddressLine2));
            //parameters.Add(new SqlParameter("@addrLine3", address.AddressLine3));
            //parameters.Add(new SqlParameter("@tel1", address.Tel1));
            //parameters.Add(new SqlParameter("@tel2", address.Tel2));
            //parameters.Add(new SqlParameter("@contractNo", address.ContractNo));
            //parameters.Add(new SqlParameter("@startDate", address.DateStart));
            //if (address.DateEnd.HasValue)
            //    parameters.Add(new SqlParameter("@endDate", address.DateEnd));
            //else
            //    parameters.Add(new SqlParameter("@endDate", DBNull.Value));
            //parameters.Add(new SqlParameter("@deleted", address.Deleted));
            //string query = query = "UPDATE [Address] "
            //                     + "SET CustomerId = @customerId, Name = @name, NIP = @nip, REGON = @regon, PostCode = @postCode, City = @city "
            //                     + "    , AddrLine1 = @addrLine1, AddrLine2 = @addrLine2, AddrLine3 = @addrLine3, Tel1 = @tel1, Tel2 = tel2, ContractNo = @contractNo "
            //                     + "    , DateStart = @startDate, DateEnd = @endDate, Deleted = @deleted "
            //                     + "WHERE Id = @id;";

            //var serverParameters = new List<SqlParameter>();
            //serverParameters.Add(new SqlParameter("@exportRowId", address.ExportRowId));
            //serverParameters.Add(new SqlParameter("@kioskId", 1));
            //string localQuery = "INSERT INTO [ExportRowExported] (ExportRowId, KioskId, ExportedDate) output INSERTED.ID "
            //                  + "VALUES (@exportRowId, @kioskId, GetDate());";

            //var sqlTransaction = LocalConnection.BeginTransaction();
            //var result = LocalConnection.UpdateRow(query, parameters, sqlTransaction);
            //var sqlServerTransaction = ServerConnection.BeginTransaction();
            //var localResult = ServerConnection.InsertRow(localQuery, serverParameters, sqlServerTransaction);

            //if ((int)result == 0 || (int)localResult == 0)
            //{
            //    sqlTransaction.Rollback();
            //    sqlServerTransaction.Rollback();
            //    return false;
            //}

            //sqlTransaction.Commit();
            //sqlServerTransaction.Commit();
            return true;
        }
        public bool AddressAdd(AdresDbRow address)
        {
            //var parameters = new List<SqlParameter>();
            //parameters.Add(new SqlParameter("@id", address.Id));
            //parameters.Add(new SqlParameter("@customerId", address.CustomerId));
            //parameters.Add(new SqlParameter("@name", address.Name));
            //parameters.Add(new SqlParameter("@nip", address.Nip));
            //parameters.Add(new SqlParameter("@regon", address.Regon));
            //parameters.Add(new SqlParameter("@postCode", address.PostCode));
            //parameters.Add(new SqlParameter("@city", address.City));
            //parameters.Add(new SqlParameter("@addrLine1", address.AddressLine1));
            //parameters.Add(new SqlParameter("@addrLine2", address.AddressLine2));
            //parameters.Add(new SqlParameter("@addrLine3", address.AddressLine3));
            //parameters.Add(new SqlParameter("@tel1", address.Tel1));
            //parameters.Add(new SqlParameter("@tel2", address.Tel2));
            //parameters.Add(new SqlParameter("@contractNo", address.ContractNo));
            //parameters.Add(new SqlParameter("@startDate", address.DateStart));
            //if (address.DateEnd.HasValue)
            //    parameters.Add(new SqlParameter("@endDate", address.DateEnd));
            //else
            //    parameters.Add(new SqlParameter("@endDate", DBNull.Value));
            //parameters.Add(new SqlParameter("@deleted", address.Deleted));
            //string query = query = "INSERT INTO [Address] (Id, CustomerId, Name, NIP, REGON, PostCode, City, AddrLine1, AddrLine2, AddrLine3, Tel1, Tel2, ContractNo "
            //                     + "    , DateStart, DateEnd, Deleted) output INSERTED.ID "
            //                     + "VALUES (@id, @customerId, @name, @nip, @regon, @postCode, @city, @addrLine1, @addrLine2, @addrLine3, @tel1, @tel2, @contractNo, @startDate, @endDate, @deleted);";

            //var serverParameters = new List<SqlParameter>();
            //serverParameters.Add(new SqlParameter("@exportRowId", address.ExportRowId));
            //serverParameters.Add(new SqlParameter("@kioskId", 1));
            //string localQuery = "INSERT INTO [ExportRowExported] (ExportRowId, KioskId, ExportedDate) output INSERTED.ID "
            //                  + "VALUES (@exportRowId, @kioskId, GetDate());";

            //var sqlTransaction = LocalConnection.BeginTransaction();
            //var result = LocalConnection.InsertRow(query, parameters, sqlTransaction);
            //var sqlServerTransaction = ServerConnection.BeginTransaction();
            //var localResult = ServerConnection.InsertRow(localQuery, serverParameters, sqlServerTransaction);

            //if ((int)result == 0 || (int)localResult == 0)
            //{
            //    sqlTransaction.Rollback();
            //    sqlServerTransaction.Rollback();
            //    return false;
            //}

            //sqlTransaction.Commit();
            //sqlServerTransaction.Commit();
            return true;
        }

        public bool SamochodUpdate(SamochodDbRow vechicle)
        {
            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@id", vechicle.Id));
            parameters.Add(new SqlParameter("@klientId", vechicle.KlientId));
            parameters.Add(new SqlParameter("@rejestracja", vechicle.Rejestracja));
            parameters.Add(new SqlParameter("@dataStart", vechicle.DataStart));
            if (vechicle.DataKoniec.HasValue)
                parameters.Add(new SqlParameter("@dataKoniec", vechicle.DataKoniec));
            else
                parameters.Add(new SqlParameter("@dataKoniec", DBNull.Value));
            parameters.Add(new SqlParameter("@deleted", vechicle.Deleted));
            string query = query = "UPDATE [Samochod] "
                                 + "SET KlientId = @klientId, Rejestracja = @rejestracja "
                                 + "    , DataStart = @dataStart, DataKoniec = @dataKoniec, Deleted = @deleted "
                                 + "WHERE Id = @id;";

            var serverParameters = new List<SqlParameter>();
            serverParameters.Add(new SqlParameter("@exportRowId", vechicle.ExportRowId));
            serverParameters.Add(new SqlParameter("@kioskId", LocalConnection.KioskId));
            string localQuery = "INSERT INTO [ExportRowExported] (ExportRowId, KioskId, ExportedDate) output INSERTED.ID "
                              + "VALUES (@exportRowId, @kioskId, GetDate());";

            var sqlTransaction = LocalConnection.BeginTransaction();
            var result = LocalConnection.UpdateRow(query, parameters, sqlTransaction);
            var sqlServerTransaction = ServerConnection.BeginTransaction();
            var localResult = ServerConnection.InsertRow(localQuery, serverParameters, sqlServerTransaction);

            if ((int)result == 0 || (int)localResult == 0)
            {
                sqlTransaction.Rollback();
                sqlServerTransaction.Rollback();
                return false;
            }

            sqlTransaction.Commit();
            sqlServerTransaction.Commit();
            return true;
        }
        public bool SamochodAdd(SamochodDbRow vechicle)
        {
            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@id", vechicle.Id));
            parameters.Add(new SqlParameter("@klientId", vechicle.KlientId));
            parameters.Add(new SqlParameter("@rejestracja", vechicle.Rejestracja));
            parameters.Add(new SqlParameter("@dataStart", vechicle.DataStart));
            if (vechicle.DataKoniec.HasValue)
                parameters.Add(new SqlParameter("@dataKoniec", vechicle.DataKoniec));
            else
                parameters.Add(new SqlParameter("@dataKoniec", DBNull.Value));
            parameters.Add(new SqlParameter("@deleted", vechicle.Deleted));
            string query = query = "INSERT INTO [Samochod] (Id, KlientId, Rejestracja"
                                 + "    , DataStart, DataKoniec, Deleted) output INSERTED.ID "
                                 + "VALUES (@id, @klientId, @rejestracja, @dataStart, @dataKoniec, @deleted);";

            var serverParameters = new List<SqlParameter>();
            serverParameters.Add(new SqlParameter("@exportRowId", vechicle.ExportRowId));
            serverParameters.Add(new SqlParameter("@kioskId", LocalConnection.KioskId));
            string localQuery = "INSERT INTO [ExportRowExported] (ExportRowId, KioskId, ExportedDate) output INSERTED.ID "
                              + "VALUES (@exportRowId, @kioskId, GetDate());";

            var sqlTransaction = LocalConnection.BeginTransaction();
            var result = LocalConnection.InsertRow(query, parameters, sqlTransaction);
            var sqlServerTransaction = ServerConnection.BeginTransaction();
            var localResult = ServerConnection.InsertRow(localQuery, serverParameters, sqlServerTransaction);

            if ((int)result == 0 || (int)localResult == 0)
            {
                sqlTransaction.Rollback();
                sqlServerTransaction.Rollback();
                return false;
            }

            sqlTransaction.Commit();
            sqlServerTransaction.Commit();
            return true;
        }

        public bool KartaUpdate(KartaDbRow card)
        {
            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@id", card.Id));
            parameters.Add(new SqlParameter("@kartaId", card.KartaId));
            parameters.Add(new SqlParameter("@kartaWymianyTacki", card.KartaWymianyTacki));
            if (card.SamochodId.HasValue)
                parameters.Add(new SqlParameter("@samochodId", card.SamochodId.Value));
            else
                parameters.Add(new SqlParameter("@samochodId", DBNull.Value));
            if (card.Blokada.HasValue)
                parameters.Add(new SqlParameter("@blokada", card.Blokada.Value));
            else
                parameters.Add(new SqlParameter("@blokada", DBNull.Value));
            parameters.Add(new SqlParameter("@dataStart", card.DataStart));
            if (card.DataKoniec.HasValue)
                parameters.Add(new SqlParameter("@dataKoniec", card.DataKoniec.Value));
            else
                parameters.Add(new SqlParameter("@dataKoniec", DBNull.Value));
            parameters.Add(new SqlParameter("@deleted", card.Deleted));
            string query = query = "UPDATE [Karta] "
                                 + "SET KartaId = @kartaId, SamochodId = @samochodId, Blokada = @blokada "
                                 + "    , DataStart = @dataStart, DataKoniec = @dataKoniec, Deleted = @deleted "
                                 + "WHERE Id = @id;";

            var serverParameters = new List<SqlParameter>();
            serverParameters.Add(new SqlParameter("@exportRowId", card.ExportRowId));
            serverParameters.Add(new SqlParameter("@kioskId", LocalConnection.KioskId));
            string localQuery = "INSERT INTO [ExportRowExported] (ExportRowId, KioskId, ExportedDate) output INSERTED.ID "
                              + "VALUES (@exportRowId, @kioskId, GetDate());";

            var sqlTransaction = LocalConnection.BeginTransaction();
            var result = LocalConnection.UpdateRow(query, parameters, sqlTransaction);
            var sqlServerTransaction = ServerConnection.BeginTransaction();
            var localResult = ServerConnection.InsertRow(localQuery, serverParameters, sqlServerTransaction);

            if ((int)result == 0 || (int)localResult == 0)
            {
                sqlTransaction.Rollback();
                sqlServerTransaction.Rollback();
                return false;
            }

            sqlTransaction.Commit();
            sqlServerTransaction.Commit();
            return true;
        }
        public bool KartaAdd(KartaDbRow card)
        {
            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@id", card.Id));
            parameters.Add(new SqlParameter("@kartaId", card.KartaId));
            parameters.Add(new SqlParameter("@kartaWymianyTacki", card.KartaWymianyTacki));
            if (card.SamochodId.HasValue)
                parameters.Add(new SqlParameter("@samochodId", card.SamochodId.Value));
            else
                parameters.Add(new SqlParameter("@samochodId", DBNull.Value));
            if (card.Blokada.HasValue)
                parameters.Add(new SqlParameter("@blokada", card.Blokada.Value));
            else
                parameters.Add(new SqlParameter("@blokada", DBNull.Value));
            parameters.Add(new SqlParameter("@dataStart", card.DataStart));
            if (card.DataKoniec.HasValue)
                parameters.Add(new SqlParameter("@dataKoniec", card.DataKoniec.Value));
            else
                parameters.Add(new SqlParameter("@dataKoniec", DBNull.Value));
            parameters.Add(new SqlParameter("@deleted", card.Deleted));
            string query = query = "INSERT INTO [Karta] (Id, KartaId, KartaWymianyTacki, SamochodId, Blokada "
                                 + "    , DataStart, DataKoniec, Deleted) output INSERTED.ID "
                                 + "VALUES (@id, @kartaId, @kartaWymianyTacki, @samochodId, @blokada, @dataStart, @dataKoniec, @deleted);";

            var serverParameters = new List<SqlParameter>();
            serverParameters.Add(new SqlParameter("@exportRowId", card.ExportRowId));
            serverParameters.Add(new SqlParameter("@kioskId", LocalConnection.KioskId));
            string localQuery = "INSERT INTO [ExportRowExported] (ExportRowId, KioskId, ExportedDate) output INSERTED.ID "
                              + "VALUES (@exportRowId, @kioskId, GetDate());";

            var sqlTransaction = LocalConnection.BeginTransaction();
            var result = LocalConnection.InsertRow(query, parameters, sqlTransaction);
            var sqlServerTransaction = ServerConnection.BeginTransaction();
            var localResult = ServerConnection.InsertRow(localQuery, serverParameters, sqlServerTransaction);

            if ((int)result == 0 || (int)localResult == 0)
            {
                sqlTransaction.Rollback();
                sqlServerTransaction.Rollback();
                return false;
            }

            sqlTransaction.Commit();
            sqlServerTransaction.Commit();
            return true;
        }

        public bool LicencjaUpdate(LicencjaDbRow license)
        {
            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@id", license.Id));
            parameters.Add(new SqlParameter("@licencjaNumer", license.LicencjaNumer));
            parameters.Add(new SqlParameter("@dataStart", license.DataStart));
            if (license.DataKoniec.HasValue)
                parameters.Add(new SqlParameter("@dataKoniec", license.DataKoniec.Value));
            else
                parameters.Add(new SqlParameter("@dataKoniec", DBNull.Value));
            parameters.Add(new SqlParameter("@deleted", license.Deleted));
            string query = query = "UPDATE [Licencja] "
                                 + "SET LicencjaNumer = @licencjaNumer "
                                 + "    , DataStart = @dataStart, DataKoniec = @dataKoniec, Deleted = @deleted "
                                 + "WHERE Id = @id;";

            var serverParameters = new List<SqlParameter>();
            serverParameters.Add(new SqlParameter("@exportRowId", license.ExportRowId));
            serverParameters.Add(new SqlParameter("@kioskId", LocalConnection.KioskId));
            string localQuery = "INSERT INTO [ExportRowExported] (ExportRowId, KioskId, ExportedDate) output INSERTED.ID "
                              + "VALUES (@exportRowId, @kioskId, GetDate());";

            var sqlTransaction = LocalConnection.BeginTransaction();
            var result = LocalConnection.UpdateRow(query, parameters, sqlTransaction);
            var sqlServerTransaction = ServerConnection.BeginTransaction();
            var localResult = ServerConnection.InsertRow(localQuery, serverParameters, sqlServerTransaction);

            if ((int)result == 0 || (int)localResult == 0)
            {
                sqlTransaction.Rollback();
                sqlServerTransaction.Rollback();
                return false;
            }

            sqlTransaction.Commit();
            sqlServerTransaction.Commit();
            return true;
        }
        public bool LicencjaAdd(LicencjaDbRow license)
        { 
            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@id", license.Id));
            parameters.Add(new SqlParameter("@klientId", license.KlientId));
            parameters.Add(new SqlParameter("@lokalizacjaId", license.LokalizacjaId));
            parameters.Add(new SqlParameter("@licencjaNumer", license.LicencjaNumer));
            parameters.Add(new SqlParameter("@dataStart", license.DataStart));
            if (license.DataKoniec.HasValue)
                parameters.Add(new SqlParameter("@dataKoniec", license.DataKoniec.Value));
            else
                parameters.Add(new SqlParameter("@dataKoniec", DBNull.Value));
            parameters.Add(new SqlParameter("@deleted", license.Deleted));
            string query = query = "INSERT INTO [Licencja] (Id, KlientId, LokalizacjaId, LicencjaNumer "
                                 + "    , DataStart, DataKoniec, Deleted) output INSERTED.ID "
                                 + "VALUES (@id, @klientId, @lokalizacjaId, @licencjaNumer, @dataStart, @dataKoniec, @deleted);";

            var serverParameters = new List<SqlParameter>();
            serverParameters.Add(new SqlParameter("@exportRowId", license.ExportRowId));
            serverParameters.Add(new SqlParameter("@kioskId", LocalConnection.KioskId));
            string localQuery = "INSERT INTO [ExportRowExported] (ExportRowId, KioskId, ExportedDate) output INSERTED.ID "
                              + "VALUES (@exportRowId, @kioskId, GetDate());";

            var sqlTransaction = LocalConnection.BeginTransaction();
            var result = LocalConnection.InsertRow(query, parameters, sqlTransaction);
            var sqlServerTransaction = ServerConnection.BeginTransaction();
            var localResult = ServerConnection.InsertRow(localQuery, serverParameters, sqlServerTransaction);

            if ((int)result == 0 || (int)localResult == 0)
            {
                sqlTransaction.Rollback();
                sqlServerTransaction.Rollback();
                return false;
            }

            sqlTransaction.Commit();
            sqlServerTransaction.Commit();
            return true;
        }

        public bool HarmonogramUpdate(HarmonogramDbRow harm)
        {
            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@id", harm.Id));
            parameters.Add(new SqlParameter("@data", harm.Data));
            parameters.Add(new SqlParameter("@typPoboruId", harm.TypPoboruId));
            if (harm.TypSciekowId.HasValue)
                parameters.Add(new SqlParameter("@typSciekowId", harm.TypSciekowId.Value));
            else
                parameters.Add(new SqlParameter("@typSciekowId", DBNull.Value));
            if (harm.AdresGminaId.HasValue)
                parameters.Add(new SqlParameter("@adresGminaId", harm.AdresGminaId.Value));
            else
                parameters.Add(new SqlParameter("@adresGminaId", DBNull.Value));
            if (harm.AdresMiejscowoscId.HasValue)
                parameters.Add(new SqlParameter("@adresMiejscowoscId", harm.AdresMiejscowoscId.Value));
            else
                parameters.Add(new SqlParameter("@adresMiejscowoscId", DBNull.Value));
            if (harm.AdresUlicaId.HasValue)
                parameters.Add(new SqlParameter("@adresUlicaId", harm.AdresUlicaId.Value));
            else
                parameters.Add(new SqlParameter("@adresUlicaId", DBNull.Value));
            if (!string.IsNullOrEmpty(harm.AdresNumer))
                parameters.Add(new SqlParameter("@adresNumer", harm.AdresNumer));
            else
                parameters.Add(new SqlParameter("@adresNumer", DBNull.Value));
            if (harm.LiczbaPoborow.HasValue)
                parameters.Add(new SqlParameter("@liczbaPoborow", harm.LiczbaPoborow.Value));
            else
                parameters.Add(new SqlParameter("@liczbaPoborow", DBNull.Value));
            if (harm.DataStart.HasValue)
                parameters.Add(new SqlParameter("@dataStart", harm.DataStart.Value));
            else
                parameters.Add(new SqlParameter("@dataStart", DBNull.Value));
            if (harm.DataKoniec.HasValue)
                parameters.Add(new SqlParameter("@dataKoniec", harm.DataKoniec.Value));
            else
                parameters.Add(new SqlParameter("@dataKoniec", DBNull.Value));
            parameters.Add(new SqlParameter("@dataDodania", harm.DataDodania));
            string query = query = "UPDATE [HarmonogramPoboruProb] "
                                 + "SET Data = @data, TypPoboruId = @typPoboruId, TypSciekowId = @typSciekowId, AdresGminaId = @adresGminaId, AdresMiejscowoscId = @adresMiejscowoscId, AdresUlicaId = @adresUlicaId, AdresNumer = @adresNumer, LiczbaPoborow = @liczbaPoborow "
                                 + "    , DataStart = @dataStart, DataKoniec = @dataKoniec "
                                 + "WHERE Id = @id;";

            var serverParameters = new List<SqlParameter>();
            serverParameters.Add(new SqlParameter("@exportRowId", harm.ExportRowId));
            serverParameters.Add(new SqlParameter("@kioskId", LocalConnection.KioskId));
            string localQuery = "INSERT INTO [ExportRowExported] (ExportRowId, KioskId, ExportedDate) output INSERTED.ID "
                              + "VALUES (@exportRowId, @kioskId, GetDate());";

            var sqlTransaction = LocalConnection.BeginTransaction();
            var result = LocalConnection.UpdateRow(query, parameters, sqlTransaction);
            var sqlServerTransaction = ServerConnection.BeginTransaction();
            var localResult = ServerConnection.InsertRow(localQuery, serverParameters, sqlServerTransaction);

            if ((int)result == 0 || (int)localResult == 0)
            {
                sqlTransaction.Rollback();
                sqlServerTransaction.Rollback();
                return false;
            }

            sqlTransaction.Commit();
            sqlServerTransaction.Commit();
            return true;
        }
        public bool HarmonogramAdd(HarmonogramDbRow harm)
        {
            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@id", harm.Id));
            parameters.Add(new SqlParameter("@kioskId", harm.KioskId));
            parameters.Add(new SqlParameter("@data", harm.Data));
            parameters.Add(new SqlParameter("@typPoboruId", harm.TypPoboruId));
            if (harm.TypSciekowId.HasValue)
                parameters.Add(new SqlParameter("@typSciekowId", harm.TypSciekowId.Value));
            else
                parameters.Add(new SqlParameter("@typSciekowId", DBNull.Value));
            if (harm.AdresGminaId.HasValue)
                parameters.Add(new SqlParameter("@adresGminaId", harm.AdresGminaId.Value));
            else
                parameters.Add(new SqlParameter("@adresGminaId", DBNull.Value));
            if (harm.AdresMiejscowoscId.HasValue)
                parameters.Add(new SqlParameter("@adresMiejscowoscId", harm.AdresMiejscowoscId.Value));
            else
                parameters.Add(new SqlParameter("@adresMiejscowoscId", DBNull.Value));
            if (harm.AdresUlicaId.HasValue)
                parameters.Add(new SqlParameter("@adresUlicaId", harm.AdresUlicaId.Value));
            else
                parameters.Add(new SqlParameter("@adresUlicaId", DBNull.Value));
            if (!string.IsNullOrEmpty(harm.AdresNumer))
                parameters.Add(new SqlParameter("@adresNumer", harm.AdresNumer));
            else
                parameters.Add(new SqlParameter("@adresNumer", DBNull.Value));
            if (harm.LiczbaPoborow.HasValue)
                parameters.Add(new SqlParameter("@liczbaPoborow", harm.LiczbaPoborow.Value));
            else
                parameters.Add(new SqlParameter("@liczbaPoborow", DBNull.Value));
            if (harm.DataStart.HasValue)
                parameters.Add(new SqlParameter("@dataStart", harm.DataStart.Value));
            else
                parameters.Add(new SqlParameter("@dataStart", DBNull.Value));
            if (harm.DataKoniec.HasValue)
                parameters.Add(new SqlParameter("@dataKoniec", harm.DataKoniec.Value));
            else
                parameters.Add(new SqlParameter("@dataKoniec", DBNull.Value));
            parameters.Add(new SqlParameter("@dataDodania", harm.DataDodania));
            string query = query = "INSERT INTO [HarmonogramPoboruProb] (Id, KioskId, Data, TypPoboruId, TypSciekowId, AdresGminaId, AdresMiejscowoscId, AdresUlicaId, AdresNumer, LiczbaPoborow "
                                 + "    , DataStart, DataKoniec, DataDodania) output INSERTED.ID "
                                 + "VALUES (@id, @kioskId, @data, @typPoboruId, @typSciekowId, @adresGminaId, @adresMiejscowoscId, @adresUlicaId, @adresNumer, @liczbaPoborow, @dataStart, @dataKoniec, @dataDodania);";

            var serverParameters = new List<SqlParameter>();
            serverParameters.Add(new SqlParameter("@exportRowId", harm.ExportRowId));
            serverParameters.Add(new SqlParameter("@kioskId", LocalConnection.KioskId));
            string localQuery = "INSERT INTO [ExportRowExported] (ExportRowId, KioskId, ExportedDate) output INSERTED.ID "
                              + "VALUES (@exportRowId, @kioskId, GetDate());";

            var sqlTransaction = LocalConnection.BeginTransaction();
            var result = LocalConnection.InsertRow(query, parameters, sqlTransaction);
            var sqlServerTransaction = ServerConnection.BeginTransaction();
            var localResult = ServerConnection.InsertRow(localQuery, serverParameters, sqlServerTransaction);

            if ((int)result == 0 || (int)localResult == 0)
            {
                sqlTransaction.Rollback();
                sqlServerTransaction.Rollback();
                return false;
            }

            sqlTransaction.Commit();
            sqlServerTransaction.Commit();
            return true;
        }

        public bool HarmonogramDostawcaUpdate(HarmonogramDostawcaDbRow dost)
        {
            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@id", dost.Id));
            parameters.Add(new SqlParameter("@harmonogramId", dost.HarmonogramId));
            parameters.Add(new SqlParameter("@dostawcaId", dost.DostawcaId));
            parameters.Add(new SqlParameter("@deleted", dost.Deleted));
            string query = query = "UPDATE [HarmonogramDostawca] "
                                 + "SET HarmonogramId = @harmonogramId, DostawcaId = @dostawcaId, Deleted = @deleted "
                                 + "WHERE Id = @id;";

            var serverParameters = new List<SqlParameter>();
            serverParameters.Add(new SqlParameter("@exportRowId", dost.ExportRowId));
            serverParameters.Add(new SqlParameter("@kioskId", LocalConnection.KioskId));
            string localQuery = "INSERT INTO [ExportRowExported] (ExportRowId, KioskId, ExportedDate) output INSERTED.ID "
                              + "VALUES (@exportRowId, @kioskId, GetDate());";

            var sqlTransaction = LocalConnection.BeginTransaction();
            var result = LocalConnection.UpdateRow(query, parameters, sqlTransaction);
            var sqlServerTransaction = ServerConnection.BeginTransaction();
            var localResult = ServerConnection.InsertRow(localQuery, serverParameters, sqlServerTransaction);

            if ((int)result == 0 || (long)localResult == 0)
            {
                sqlTransaction.Rollback();
                sqlServerTransaction.Rollback();
                return false;
            }

            sqlTransaction.Commit();
            sqlServerTransaction.Commit();
            return true;
        }

        public bool HarmonogramDostawcaAdd(HarmonogramDostawcaDbRow dost)
        {
            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@id", dost.Id));
            parameters.Add(new SqlParameter("@harmonogramId", dost.HarmonogramId));
            parameters.Add(new SqlParameter("@dostawcaId", dost.DostawcaId));
            parameters.Add(new SqlParameter("@deleted", dost.Deleted));
            string query = query = "INSERT INTO [HarmonogramDostawca] (Id, HarmonogramId, DostawcaId, Deleted) output INSERTED.ID "
                                 + "VALUES (@id, @harmonogramId, @dostawcaId, @deleted);";

            var serverParameters = new List<SqlParameter>();
            serverParameters.Add(new SqlParameter("@exportRowId", dost.ExportRowId));
            serverParameters.Add(new SqlParameter("@kioskId", LocalConnection.KioskId));
            string localQuery = "INSERT INTO [ExportRowExported] (ExportRowId, KioskId, ExportedDate) output INSERTED.ID "
                              + "VALUES (@exportRowId, @kioskId, GetDate());";

            var sqlTransaction = LocalConnection.BeginTransaction();
            var result = LocalConnection.InsertRow(query, parameters, sqlTransaction);
            var sqlServerTransaction = ServerConnection.BeginTransaction();
            var localResult = ServerConnection.InsertRow(localQuery, serverParameters, sqlServerTransaction);

            if ((int)result == 0 || (long)localResult == 0)
            {
                sqlTransaction.Rollback();
                sqlServerTransaction.Rollback();
                return false;
            }

            sqlTransaction.Commit();
            sqlServerTransaction.Commit();
            return true;
        }

        public bool GminaUpdate(GminaDbRow gmina)
        {
            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@id", gmina.Id));
            parameters.Add(new SqlParameter("@kioskId", gmina.KioskId));
            if (gmina.TerytId.HasValue)
                parameters.Add(new SqlParameter("@terytId", gmina.TerytId.Value));
            else
                parameters.Add(new SqlParameter("@terytId", DBNull.Value));
            if (gmina.TerytStanNa.HasValue)
                parameters.Add(new SqlParameter("@terytStanNa", gmina.TerytStanNa.Value));
            else
                parameters.Add(new SqlParameter("@terytStanNa", DBNull.Value));
            parameters.Add(new SqlParameter("@powiatId", gmina.PowiatId));
            parameters.Add(new SqlParameter("@nazwa", gmina.Nazwa));
            parameters.Add(new SqlParameter("@rodzajId", gmina.RodzajId));
            parameters.Add(new SqlParameter("@dataStart", gmina.DataStart));
            if (gmina.DataKoniec.HasValue)
                parameters.Add(new SqlParameter("@dataKoniec", gmina.DataKoniec.Value));
            else
                parameters.Add(new SqlParameter("@dataKoniec", DBNull.Value));
            parameters.Add(new SqlParameter("@deleted", gmina.Deleted));
            string query = query = "UPDATE [Gmina] "
                                 + "SET KioskId = @kioskId, TerytId = @terytId, TerytStanNa = @terytStanNa, PowiatId = @powiatId, Nazwa = @nazwa, RodzajId = @rodzajId "
                                 + "    , DataStart = @dataStart, DataKoniec = @dataKoniec, Deleted = @deleted "
                                 + "WHERE Id = @id;";

            var serverParameters = new List<SqlParameter>();
            serverParameters.Add(new SqlParameter("@exportRowId", gmina.ExportRowId));
            serverParameters.Add(new SqlParameter("@kioskId", LocalConnection.KioskId));
            string localQuery = "INSERT INTO [ExportRowExported] (ExportRowId, KioskId, ExportedDate) output INSERTED.ID "
                              + "VALUES (@exportRowId, @kioskId, GetDate());";

            var sqlTransaction = LocalConnection.BeginTransaction();
            var result = LocalConnection.UpdateRow(query, parameters, sqlTransaction);
            var sqlServerTransaction = ServerConnection.BeginTransaction();
            var localResult = ServerConnection.InsertRow(localQuery, serverParameters, sqlServerTransaction);

            if ((int)result == 0 || (int)localResult == 0)
            {
                sqlTransaction.Rollback();
                sqlServerTransaction.Rollback();
                return false;
            }

            sqlTransaction.Commit();
            sqlServerTransaction.Commit();
            return true;
        }
        public bool GminaAdd(GminaDbRow gmina)
        {
            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@id", gmina.Id));
            parameters.Add(new SqlParameter("@kioskId", gmina.KioskId));
            if (gmina.TerytId.HasValue)
                parameters.Add(new SqlParameter("@terytId", gmina.TerytId.Value));
            else
                parameters.Add(new SqlParameter("@terytId", DBNull.Value));
            if (gmina.TerytStanNa.HasValue)
                parameters.Add(new SqlParameter("@terytStanNa", gmina.TerytStanNa.Value));
            else
                parameters.Add(new SqlParameter("@terytStanNa", DBNull.Value));
            parameters.Add(new SqlParameter("@powiatId", gmina.PowiatId));
            parameters.Add(new SqlParameter("@nazwa", gmina.Nazwa));
            parameters.Add(new SqlParameter("@rodzajId", gmina.RodzajId));
            parameters.Add(new SqlParameter("@dataStart", gmina.DataStart));
            if (gmina.DataKoniec.HasValue)
                parameters.Add(new SqlParameter("@dataKoniec", gmina.DataKoniec.Value));
            else
                parameters.Add(new SqlParameter("@dataKoniec", DBNull.Value));
            parameters.Add(new SqlParameter("@deleted", gmina.Deleted));
            string query = query = "INSERT INTO [Gmina] (Id, KioskId, TerytId, TerytStanNa, PowiatId, Nazwa, RodzajId "
                                 + "    , DataStart, DataKoniec, Deleted) output INSERTED.ID "
                                 + "VALUES (@id, @kioskId, @terytId, @terytStanNa, @powiatId, @nazwa, @rodzajId, @dataStart, @dataKoniec, @deleted);";

            var serverParameters = new List<SqlParameter>();
            serverParameters.Add(new SqlParameter("@exportRowId", gmina.ExportRowId));
            serverParameters.Add(new SqlParameter("@kioskId", LocalConnection.KioskId));
            string localQuery = "INSERT INTO [ExportRowExported] (ExportRowId, KioskId, ExportedDate) output INSERTED.ID "
                              + "VALUES (@exportRowId, @kioskId, GetDate());";

            var sqlTransaction = LocalConnection.BeginTransaction();
            var result = LocalConnection.InsertRow(query, parameters, sqlTransaction);
            var sqlServerTransaction = ServerConnection.BeginTransaction();
            var localResult = ServerConnection.InsertRow(localQuery, serverParameters, sqlServerTransaction);

            if ((int)result == 0 || (int)localResult == 0)
            {
                sqlTransaction.Rollback();
                sqlServerTransaction.Rollback();
                return false;
            }

            sqlTransaction.Commit();
            sqlServerTransaction.Commit();
            return true;
        }

        public bool MiejscowoscUpdate(MiejscowoscDbRow miejscowosc)
        {
            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@id", miejscowosc.Id));
            parameters.Add(new SqlParameter("@kioskId", miejscowosc.KioskId));
            if (miejscowosc.TerytId.HasValue)
                parameters.Add(new SqlParameter("@terytId", miejscowosc.TerytId.Value));
            else
                parameters.Add(new SqlParameter("@terytId", DBNull.Value));
            if (miejscowosc.TerytStanNa.HasValue)
                parameters.Add(new SqlParameter("@terytStanNa", miejscowosc.TerytStanNa.Value));
            else
                parameters.Add(new SqlParameter("@terytStanNa", DBNull.Value));
            parameters.Add(new SqlParameter("@nazwa", miejscowosc.Nazwa));
            parameters.Add(new SqlParameter("@gminaId", miejscowosc.GminaId));
            parameters.Add(new SqlParameter("@rodzajId", miejscowosc.RodzajId));
            parameters.Add(new SqlParameter("@dataStart", miejscowosc.DataStart));
            if (miejscowosc.DataKoniec.HasValue)
                parameters.Add(new SqlParameter("@dataKoniec", miejscowosc.DataKoniec.Value));
            else
                parameters.Add(new SqlParameter("@dataKoniec", DBNull.Value));
            parameters.Add(new SqlParameter("@deleted", miejscowosc.Deleted));
            string query = query = "UPDATE [Miejscowosc] "
                                 + "SET KioskId = @kioskId, TerytId = @terytId, TerytStanNa = @terytStanNa, Nazwa = @nazwa, GminaId = @gminaId, RodzajId = @rodzajId "
                                 + "    , DataStart = @dataStart, DataKoniec = @dataKoniec, Deleted = @deleted "
                                 + "WHERE Id = @id;";

            var serverParameters = new List<SqlParameter>();
            serverParameters.Add(new SqlParameter("@exportRowId", miejscowosc.ExportRowId));
            serverParameters.Add(new SqlParameter("@kioskId", LocalConnection.KioskId));
            string localQuery = "INSERT INTO [ExportRowExported] (ExportRowId, KioskId, ExportedDate) output INSERTED.ID "
                              + "VALUES (@exportRowId, @kioskId, GetDate());";

            var sqlTransaction = LocalConnection.BeginTransaction();
            var result = LocalConnection.UpdateRow(query, parameters, sqlTransaction);
            var sqlServerTransaction = ServerConnection.BeginTransaction();
            var localResult = ServerConnection.InsertRow(localQuery, serverParameters, sqlServerTransaction);

            if ((int)result == 0 || (int)localResult == 0)
            {
                sqlTransaction.Rollback();
                sqlServerTransaction.Rollback();
                return false;
            }

            sqlTransaction.Commit();
            sqlServerTransaction.Commit();
            return true;
        }
        public bool MiejscowoscAdd(MiejscowoscDbRow miejscowosc)
        {
            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@id", miejscowosc.Id));
            parameters.Add(new SqlParameter("@kioskId", miejscowosc.KioskId));
            if (miejscowosc.TerytId.HasValue)
                parameters.Add(new SqlParameter("@terytId", miejscowosc.TerytId.Value));
            else
                parameters.Add(new SqlParameter("@terytId", DBNull.Value));
            if (miejscowosc.TerytStanNa.HasValue)
                parameters.Add(new SqlParameter("@terytStanNa", miejscowosc.TerytStanNa.Value));
            else
                parameters.Add(new SqlParameter("@terytStanNa", DBNull.Value));
            parameters.Add(new SqlParameter("@nazwa", miejscowosc.Nazwa));
            parameters.Add(new SqlParameter("@gminaId", miejscowosc.GminaId));
            parameters.Add(new SqlParameter("@rodzajId", miejscowosc.RodzajId));
            parameters.Add(new SqlParameter("@dataStart", miejscowosc.DataStart));
            if (miejscowosc.DataKoniec.HasValue)
                parameters.Add(new SqlParameter("@dataKoniec", miejscowosc.DataKoniec.Value));
            else
                parameters.Add(new SqlParameter("@dataKoniec", DBNull.Value));
            parameters.Add(new SqlParameter("@deleted", miejscowosc.Deleted));
            string query = query = "INSERT INTO [Miejscowosc] (Id, KioskId, TerytId, TerytStanNa, Nazwa, GminaId, RodzajId "
                                 + "    , DataStart, DataKoniec, Deleted) output INSERTED.ID "
                                 + "VALUES (@id, @kioskId, @terytId, @terytStanNa, @nazwa, @gminaId, @rodzajId, @dataStart, @dataKoniec, @deleted);";

            var serverParameters = new List<SqlParameter>();
            serverParameters.Add(new SqlParameter("@exportRowId", miejscowosc.ExportRowId));
            serverParameters.Add(new SqlParameter("@kioskId", LocalConnection.KioskId));
            string localQuery = "INSERT INTO [ExportRowExported] (ExportRowId, KioskId, ExportedDate) output INSERTED.ID "
                              + "VALUES (@exportRowId, @kioskId, GetDate());";

            var sqlTransaction = LocalConnection.BeginTransaction();
            var result = LocalConnection.InsertRow(query, parameters, sqlTransaction);
            var sqlServerTransaction = ServerConnection.BeginTransaction();
            var localResult = ServerConnection.InsertRow(localQuery, serverParameters, sqlServerTransaction);

            if ((int)result == 0 || (int)localResult == 0)
            {
                sqlTransaction.Rollback();
                sqlServerTransaction.Rollback();
                return false;
            }

            sqlTransaction.Commit();
            sqlServerTransaction.Commit();
            return true;
        }
        public bool UlicaUpdate(UlicaDbRow ulica)
        {
            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@id", ulica.Id));
            parameters.Add(new SqlParameter("@kioskId", ulica.KioskId));
            if (ulica.TerytId.HasValue)
                parameters.Add(new SqlParameter("@terytId", ulica.TerytId.Value));
            else
                parameters.Add(new SqlParameter("@terytId", DBNull.Value));
            if (ulica.TerytStanNa.HasValue)
                parameters.Add(new SqlParameter("@terytStanNa", ulica.TerytStanNa.Value));
            else
                parameters.Add(new SqlParameter("@terytStanNa", DBNull.Value));
            parameters.Add(new SqlParameter("@nazwa", ulica.Nazwa));
            parameters.Add(new SqlParameter("@miejscowoscId", ulica.MiejscowoscId));
            parameters.Add(new SqlParameter("@gminaId", ulica.GminaId));
            parameters.Add(new SqlParameter("@rodzajId", ulica.RodzajId));
            parameters.Add(new SqlParameter("@dataStart", ulica.DataStart));
            if (ulica.DataKoniec.HasValue)
                parameters.Add(new SqlParameter("@dataKoniec", ulica.DataKoniec.Value));
            else
                parameters.Add(new SqlParameter("@dataKoniec", DBNull.Value));
            parameters.Add(new SqlParameter("@deleted", ulica.Deleted));
            string query = query = "UPDATE [Ulica] "
                                 + "SET KioskId = @kioskId, TerytId = @terytId, TerytStanNa = @terytStanNa, Nazwa = @nazwa, MiejscowoscId = @miejscowoscId, GminaId = @gminaId, RodzajId = @rodzajId "
                                 + "    , DataStart = @dataStart, DataKoniec = @dataKoniec, Deleted = @deleted "
                                 + "WHERE Id = @id;";

            var serverParameters = new List<SqlParameter>();
            serverParameters.Add(new SqlParameter("@exportRowId", ulica.ExportRowId));
            serverParameters.Add(new SqlParameter("@kioskId", LocalConnection.KioskId));
            string localQuery = "INSERT INTO [ExportRowExported] (ExportRowId, KioskId, ExportedDate) output INSERTED.ID "
                              + "VALUES (@exportRowId, @kioskId, GetDate());";

            var sqlTransaction = LocalConnection.BeginTransaction();
            var result = LocalConnection.UpdateRow(query, parameters, sqlTransaction);
            var sqlServerTransaction = ServerConnection.BeginTransaction();
            var localResult = ServerConnection.InsertRow(localQuery, serverParameters, sqlServerTransaction);

            if ((int)result == 0 || (int)localResult == 0)
            {
                sqlTransaction.Rollback();
                sqlServerTransaction.Rollback();
                return false;
            }

            sqlTransaction.Commit();
            sqlServerTransaction.Commit();
            return true;
        }
        public bool UlicaAdd(UlicaDbRow ulica)
        {
            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@id", ulica.Id));
            parameters.Add(new SqlParameter("@kioskId", ulica.KioskId));
            if (ulica.TerytId.HasValue)
                parameters.Add(new SqlParameter("@terytId", ulica.TerytId.Value));
            else
                parameters.Add(new SqlParameter("@terytId", DBNull.Value));
            if (ulica.TerytStanNa.HasValue)
                parameters.Add(new SqlParameter("@terytStanNa", ulica.TerytStanNa.Value));
            else
                parameters.Add(new SqlParameter("@terytStanNa", DBNull.Value));
            parameters.Add(new SqlParameter("@nazwa", ulica.Nazwa));
            parameters.Add(new SqlParameter("@miejscowoscId", ulica.MiejscowoscId));
            parameters.Add(new SqlParameter("@gminaId", ulica.GminaId));
            parameters.Add(new SqlParameter("@rodzajId", ulica.RodzajId));
            parameters.Add(new SqlParameter("@dataStart", ulica.DataStart));
            if (ulica.DataKoniec.HasValue)
                parameters.Add(new SqlParameter("@dataKoniec", ulica.DataKoniec.Value));
            else
                parameters.Add(new SqlParameter("@dataKoniec", DBNull.Value));
            parameters.Add(new SqlParameter("@deleted", ulica.Deleted));
            string query = query = "INSERT INTO [Ulica] (Id, KioskId, TerytId, TerytStanNa, Nazwa, MiejscowoscId, GminaId, RodzajId "
                                 + "    , DataStart, DataKoniec, Deleted) output INSERTED.ID "
                                 + "VALUES (@id, @kioskId, @terytId, @terytStanNa, @nazwa, @miejscowoscId, @gminaId, @rodzajId, @dataStart, @dataKoniec, @deleted);";

            var serverParameters = new List<SqlParameter>();
            serverParameters.Add(new SqlParameter("@exportRowId", ulica.ExportRowId));
            serverParameters.Add(new SqlParameter("@kioskId", LocalConnection.KioskId));
            string localQuery = "INSERT INTO [ExportRowExported] (ExportRowId, KioskId, ExportedDate) output INSERTED.ID "
                              + "VALUES (@exportRowId, @kioskId, GetDate());";

            var sqlTransaction = LocalConnection.BeginTransaction();
            var result = LocalConnection.InsertRow(query, parameters, sqlTransaction);
            var sqlServerTransaction = ServerConnection.BeginTransaction();
            var localResult = ServerConnection.InsertRow(localQuery, serverParameters, sqlServerTransaction);

            if ((int)result == 0 || (int)localResult == 0)
            {
                sqlTransaction.Rollback();
                sqlServerTransaction.Rollback();
                return false;
            }

            sqlTransaction.Commit();
            sqlServerTransaction.Commit();
            return true;
        }
        public bool AdresUpdate(AdresDbRow adres)
        {
            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@id", adres.Id));
            parameters.Add(new SqlParameter("@kioskId", adres.KioskId));
            parameters.Add(new SqlParameter("@nazwa", adres.Nazwa));
            if (adres.UlicaId.HasValue)
                parameters.Add(new SqlParameter("@ulicaId", adres.UlicaId.Value));
            else
                parameters.Add(new SqlParameter("@ulicaId", DBNull.Value));
            parameters.Add(new SqlParameter("@miejscowoscId", adres.MiejscowoscId));
            parameters.Add(new SqlParameter("@gminaId", adres.GminaId));
            parameters.Add(new SqlParameter("@numer", adres.Numer));
            if (string.IsNullOrEmpty(adres.NumerUmowy))
                parameters.Add(new SqlParameter("@numerUmowy", DBNull.Value));
            else
                parameters.Add(new SqlParameter("@numerUmowy", adres.NumerUmowy));
            parameters.Add(new SqlParameter("@rodzajId", adres.RodzajId));
            parameters.Add(new SqlParameter("@dataStart", adres.DataStart));
            if (adres.DataKoniec.HasValue)
                parameters.Add(new SqlParameter("@dataKoniec", adres.DataKoniec.Value));
            else
                parameters.Add(new SqlParameter("@dataKoniec", DBNull.Value));
            parameters.Add(new SqlParameter("@deleted", adres.Deleted));
            string query = query = "UPDATE [Adres] "
                                 + "SET KioskId = @kioskId, Nazwa = @nazwa, UlicaId = @ulicaId, MiejscowoscId = @miejscowoscId, GminaId = @gminaId, Numer = @numer, NumerUmowy = @numerUmowy, RodzajId = @rodzajId "
                                 + "    , DataStart = @dataStart, DataKoniec = @dataKoniec, Deleted = @deleted "
                                 + "WHERE Id = @id;";

            var serverParameters = new List<SqlParameter>();
            serverParameters.Add(new SqlParameter("@exportRowId", adres.ExportRowId));
            serverParameters.Add(new SqlParameter("@kioskId", LocalConnection.KioskId));
            string localQuery = "INSERT INTO [ExportRowExported] (ExportRowId, KioskId, ExportedDate) output INSERTED.ID "
                              + "VALUES (@exportRowId, @kioskId, GetDate());";

            var sqlTransaction = LocalConnection.BeginTransaction();
            var result = LocalConnection.UpdateRow(query, parameters, sqlTransaction);
            var sqlServerTransaction = ServerConnection.BeginTransaction();
            var localResult = ServerConnection.InsertRow(localQuery, serverParameters, sqlServerTransaction);

            if ((int)result == 0 || (int)localResult == 0)
            {
                sqlTransaction.Rollback();
                sqlServerTransaction.Rollback();
                return false;
            }

            sqlTransaction.Commit();
            sqlServerTransaction.Commit();
            return true;
        }
        public bool AdresAdd(AdresDbRow adres)
        {
            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@id", adres.Id));
            parameters.Add(new SqlParameter("@kioskId", adres.KioskId));
            parameters.Add(new SqlParameter("@nazwa", adres.Nazwa));
            if (adres.UlicaId.HasValue)
                parameters.Add(new SqlParameter("@ulicaId", adres.UlicaId.Value));
            else
                parameters.Add(new SqlParameter("@ulicaId", DBNull.Value));
            parameters.Add(new SqlParameter("@miejscowoscId", adres.MiejscowoscId));
            parameters.Add(new SqlParameter("@gminaId", adres.GminaId));
            parameters.Add(new SqlParameter("@numer", adres.Numer));
            if (string.IsNullOrEmpty(adres.NumerUmowy))
                parameters.Add(new SqlParameter("@numerUmowy", DBNull.Value));
            else
                parameters.Add(new SqlParameter("@numerUmowy", adres.NumerUmowy));
            parameters.Add(new SqlParameter("@rodzajId", adres.RodzajId));
            parameters.Add(new SqlParameter("@dataStart", adres.DataStart));
            if (adres.DataKoniec.HasValue)
                parameters.Add(new SqlParameter("@dataKoniec", adres.DataKoniec.Value));
            else
                parameters.Add(new SqlParameter("@dataKoniec", DBNull.Value));
            parameters.Add(new SqlParameter("@deleted", adres.Deleted));
            string query = query = "INSERT INTO [Adres] (Id, KioskId, Nazwa, UlicaId, MiejscowoscId, GminaId, Numer, NumerUmowy, RodzajId "
                                 + "    , DataStart, DataKoniec, Deleted) output INSERTED.ID "
                                 + "VALUES (@id, @kioskId, @nazwa, @ulicaId, @miejscowoscId, @gminaId, @numer, @numerUmowy, @rodzajId, @dataStart, @dataKoniec, @deleted);";

            var serverParameters = new List<SqlParameter>();
            serverParameters.Add(new SqlParameter("@exportRowId", adres.ExportRowId));
            serverParameters.Add(new SqlParameter("@kioskId", LocalConnection.KioskId));
            string localQuery = "INSERT INTO [ExportRowExported] (ExportRowId, KioskId, ExportedDate) output INSERTED.ID "
                              + "VALUES (@exportRowId, @kioskId, GetDate());";
            
            var sqlTransaction = LocalConnection.BeginTransaction();
            var result = LocalConnection.UpdateRow(query, parameters, sqlTransaction);
            var sqlServerTransaction = ServerConnection.BeginTransaction();
            var localResult = ServerConnection.InsertRow(localQuery, serverParameters, sqlServerTransaction);

            if ((int)result == 0 || (int)localResult == 0)
            {
                sqlTransaction.Rollback();
                sqlServerTransaction.Rollback();
                return false;
            }

            sqlTransaction.Commit();
            sqlServerTransaction.Commit();
            return true;
        }

        public bool FirmaUpdate(FirmaDbRow firma)
        {
            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@id", firma.Id));
            parameters.Add(new SqlParameter("@kioskId", firma.KioskId));
            parameters.Add(new SqlParameter("@nazwa", firma.Nazwa));
            parameters.Add(new SqlParameter("@dataStart", firma.DataStart));
            if (firma.DataKoniec.HasValue)
                parameters.Add(new SqlParameter("@dataKoniec", firma.DataKoniec.Value));
            else
                parameters.Add(new SqlParameter("@dataKoniec", DBNull.Value));
            parameters.Add(new SqlParameter("@deleted", firma.Deleted));
            string query = query = "UPDATE [Firma] "
                                 + "SET KioskId = @kioskId, Nazwa = @nazwa "
                                 + "    , DataStart = @dataStart, DataKoniec = @dataKoniec, Deleted = @deleted "
                                 + "WHERE Id = @id;";

            var serverParameters = new List<SqlParameter>();
            serverParameters.Add(new SqlParameter("@exportRowId", firma.ExportRowId));
            serverParameters.Add(new SqlParameter("@kioskId", LocalConnection.KioskId));
            string localQuery = "INSERT INTO [ExportRowExported] (ExportRowId, KioskId, ExportedDate) output INSERTED.ID "
                              + "VALUES (@exportRowId, @kioskId, GetDate());";

            var sqlTransaction = LocalConnection.BeginTransaction();
            var result = LocalConnection.UpdateRow(query, parameters, sqlTransaction);
            var sqlServerTransaction = ServerConnection.BeginTransaction();
            var localResult = ServerConnection.InsertRow(localQuery, serverParameters, sqlServerTransaction);

            if ((int)result == 0 || (int)localResult == 0)
            {
                sqlTransaction.Rollback();
                sqlServerTransaction.Rollback();
                return false;
            }

            sqlTransaction.Commit();
            sqlServerTransaction.Commit();
            return true;
        }
        public bool FirmaAdd(FirmaDbRow firma)
        {
            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@id", firma.Id));
            parameters.Add(new SqlParameter("@kioskId", firma.KioskId));
            parameters.Add(new SqlParameter("@nazwa", firma.Nazwa));
            parameters.Add(new SqlParameter("@dataStart", firma.DataStart));
            if (firma.DataKoniec.HasValue)
                parameters.Add(new SqlParameter("@dataKoniec", firma.DataKoniec.Value));
            else
                parameters.Add(new SqlParameter("@dataKoniec", DBNull.Value));
            parameters.Add(new SqlParameter("@deleted", firma.Deleted));
            string query = query = "INSERT INTO [Firma] (Id, KioskId, Nazwa "
                                 + "    , DataStart, DataKoniec, Deleted) output INSERTED.ID "
                                 + "VALUES (@id, @kioskId, @nazwa, @dataStart, @dataKoniec, @deleted);";

            var serverParameters = new List<SqlParameter>();
            serverParameters.Add(new SqlParameter("@exportRowId", firma.ExportRowId));
            serverParameters.Add(new SqlParameter("@kioskId", LocalConnection.KioskId));
            string localQuery = "INSERT INTO [ExportRowExported] (ExportRowId, KioskId, ExportedDate) output INSERTED.ID "
                              + "VALUES (@exportRowId, @kioskId, GetDate());";

            var sqlTransaction = LocalConnection.BeginTransaction();
            var result = LocalConnection.InsertRow(query, parameters, sqlTransaction);
            var sqlServerTransaction = ServerConnection.BeginTransaction();
            var localResult = ServerConnection.InsertRow(localQuery, serverParameters, sqlServerTransaction);

            if ((int)result == 0 || (int)localResult == 0)
            {
                sqlTransaction.Rollback();
                sqlServerTransaction.Rollback();
                return false;
            }

            sqlTransaction.Commit();
            sqlServerTransaction.Commit();
            return true;
        }
        public bool FirmaAdresUpdate(FirmaAdresDbRow firma)
        {
            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@id", firma.Id));
            parameters.Add(new SqlParameter("@kioskId", firma.KioskId));
            parameters.Add(new SqlParameter("@adresId", firma.AdresId));
            parameters.Add(new SqlParameter("@firmaId", firma.FirmaId));
            parameters.Add(new SqlParameter("@dataStart", firma.DataStart));
            if (firma.DataKoniec.HasValue)
                parameters.Add(new SqlParameter("@dataKoniec", firma.DataKoniec.Value));
            else
                parameters.Add(new SqlParameter("@dataKoniec", DBNull.Value));
            parameters.Add(new SqlParameter("@deleted", firma.Deleted));
            string query = query = "UPDATE [FirmaAdres] "
                                 + "SET KioskId = @kioskId, AdresId = @adresId, FirmaId = @firmaId "
                                 + "    , DataStart = @dataStart, DataKoniec = @dataKoniec, Deleted = @deleted "
                                 + "WHERE Id = @id;";

            var serverParameters = new List<SqlParameter>();
            serverParameters.Add(new SqlParameter("@exportRowId", firma.ExportRowId));
            serverParameters.Add(new SqlParameter("@kioskId", LocalConnection.KioskId));
            string localQuery = "INSERT INTO [ExportRowExported] (ExportRowId, KioskId, ExportedDate) output INSERTED.ID "
                              + "VALUES (@exportRowId, @kioskId, GetDate());";

            var sqlTransaction = LocalConnection.BeginTransaction();
            var result = LocalConnection.UpdateRow(query, parameters, sqlTransaction);
            var sqlServerTransaction = ServerConnection.BeginTransaction();
            var localResult = ServerConnection.InsertRow(localQuery, serverParameters, sqlServerTransaction);

            if ((int)result == 0 || (int)localResult == 0)
            {
                sqlTransaction.Rollback();
                sqlServerTransaction.Rollback();
                return false;
            }

            sqlTransaction.Commit();
            sqlServerTransaction.Commit();
            return true;
        }
        public bool FirmaAdresAdd(FirmaAdresDbRow firma)
        {
            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@id", firma.Id));
            parameters.Add(new SqlParameter("@kioskId", firma.KioskId));
            parameters.Add(new SqlParameter("@adresId", firma.AdresId));
            parameters.Add(new SqlParameter("@firmaId", firma.FirmaId));
            parameters.Add(new SqlParameter("@dataStart", firma.DataStart));
            if (firma.DataKoniec.HasValue)
                parameters.Add(new SqlParameter("@dataKoniec", firma.DataKoniec.Value));
            else
                parameters.Add(new SqlParameter("@dataKoniec", DBNull.Value));
            parameters.Add(new SqlParameter("@deleted", firma.Deleted));
            string query = query = "INSERT INTO [FirmaAdres] (Id, KioskId, AdresId, FirmaId "
                                 + "    , DataStart, DataKoniec, Deleted) output INSERTED.ID "
                                 + "VALUES (@id, @kioskId, @adresId, @firmaId, @dataStart, @dataKoniec, @deleted);";

            var serverParameters = new List<SqlParameter>();
            serverParameters.Add(new SqlParameter("@exportRowId", firma.ExportRowId));
            serverParameters.Add(new SqlParameter("@kioskId", LocalConnection.KioskId));
            string localQuery = "INSERT INTO [ExportRowExported] (ExportRowId, KioskId, ExportedDate) output INSERTED.ID "
                              + "VALUES (@exportRowId, @kioskId, GetDate());";

            var sqlTransaction = LocalConnection.BeginTransaction();
            var result = LocalConnection.InsertRow(query, parameters, sqlTransaction);
            var sqlServerTransaction = ServerConnection.BeginTransaction();
            var localResult = ServerConnection.InsertRow(localQuery, serverParameters, sqlServerTransaction);

            if ((int)result == 0 || (int)localResult == 0)
            {
                sqlTransaction.Rollback();
                sqlServerTransaction.Rollback();
                return false;
            }

            sqlTransaction.Commit();
            sqlServerTransaction.Commit();
            return true;
        }
        
        public bool StalyKlientUpdate(StalyKlientDbRow klient)
        {
            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@id", klient.Id));
            parameters.Add(new SqlParameter("@kioskId", klient.KioskId));
            parameters.Add(new SqlParameter("@adresId", klient.AdresId));
            if (klient.FirmaId.HasValue)
                parameters.Add(new SqlParameter("@firmaId", klient.FirmaId.Value));
            else
                parameters.Add(new SqlParameter("@firmaId", DBNull.Value));
            parameters.Add(new SqlParameter("@klientId", klient.KlientId));
            parameters.Add(new SqlParameter("@dataStart", klient.DataStart));
            if (klient.DataKoniec.HasValue)
                parameters.Add(new SqlParameter("@dataKoniec", klient.DataKoniec.Value));
            else
                parameters.Add(new SqlParameter("@dataKoniec", DBNull.Value));
            parameters.Add(new SqlParameter("@deleted", klient.Deleted));
            string query = query = "UPDATE [StalyKlient] "
                                 + "SET KioskId = @kioskId, AdresId = @adresId, FirmaId = @firmaId, AdresId = @adresId "
                                 + "    , DataStart = @dataStart, DataKoniec = @dataKoniec, Deleted = @deleted "
                                 + "WHERE Id = @id;";

            var serverParameters = new List<SqlParameter>();
            serverParameters.Add(new SqlParameter("@exportRowId", klient.ExportRowId));
            serverParameters.Add(new SqlParameter("@kioskId", LocalConnection.KioskId));
            string localQuery = "INSERT INTO [ExportRowExported] (ExportRowId, KioskId, ExportedDate) output INSERTED.ID "
                              + "VALUES (@exportRowId, @kioskId, GetDate());";

            var sqlTransaction = LocalConnection.BeginTransaction();
            var result = LocalConnection.UpdateRow(query, parameters, sqlTransaction);
            var sqlServerTransaction = ServerConnection.BeginTransaction();
            var localResult = ServerConnection.InsertRow(localQuery, serverParameters, sqlServerTransaction);

            if ((int)result == 0 || (int)localResult == 0)
            {
                sqlTransaction.Rollback();
                sqlServerTransaction.Rollback();
                return false;
            }

            sqlTransaction.Commit();
            sqlServerTransaction.Commit();
            return true;
        }
        public bool StalyKlientAdd(StalyKlientDbRow klient)
        {
            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@id", klient.Id));
            parameters.Add(new SqlParameter("@kioskId", klient.KioskId));
            parameters.Add(new SqlParameter("@adresId", klient.AdresId));
            if (klient.FirmaId.HasValue)
                parameters.Add(new SqlParameter("@firmaId", klient.FirmaId.Value));
            else
                parameters.Add(new SqlParameter("@firmaId", DBNull.Value));
            parameters.Add(new SqlParameter("@klientId", klient.KlientId));
            parameters.Add(new SqlParameter("@dataStart", klient.DataStart));
            if (klient.DataKoniec.HasValue)
                parameters.Add(new SqlParameter("@dataKoniec", klient.DataKoniec.Value));
            else
                parameters.Add(new SqlParameter("@dataKoniec", DBNull.Value));
            parameters.Add(new SqlParameter("@deleted", klient.Deleted));
            string query = query = "INSERT INTO [StalyKlient] (Id, KioskId, AdresId, FirmaId, KlientId "
                                 + "    , DataStart, DataKoniec, Deleted) output INSERTED.ID "
                                 + "VALUES (@id, @kioskId, @adresId, @firmaId, @klientId, @dataStart, @dataKoniec, @deleted);";

            var serverParameters = new List<SqlParameter>();
            serverParameters.Add(new SqlParameter("@exportRowId", klient.ExportRowId));
            serverParameters.Add(new SqlParameter("@kioskId", LocalConnection.KioskId));
            string localQuery = "INSERT INTO [ExportRowExported] (ExportRowId, KioskId, ExportedDate) output INSERTED.ID "
                              + "VALUES (@exportRowId, @kioskId, GetDate());";

            var sqlTransaction = LocalConnection.BeginTransaction();
            var result = LocalConnection.InsertRow(query, parameters, sqlTransaction);
            var sqlServerTransaction = ServerConnection.BeginTransaction();
            var localResult = ServerConnection.InsertRow(localQuery, serverParameters, sqlServerTransaction);

            if ((int)result == 0 || (int)localResult == 0)
            {
                sqlTransaction.Rollback();
                sqlServerTransaction.Rollback();
                return false;
            }

            sqlTransaction.Commit();
            sqlServerTransaction.Commit();
            return true;
        }

        public bool CargoTypeUpdate(CargoTypeDbRow cargo)
        {
            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@id", cargo.Id));
            parameters.Add(new SqlParameter("@name", cargo.Name));
            parameters.Add(new SqlParameter("@cargoCode", cargo.CargoCode));
            parameters.Add(new SqlParameter("@startDate", cargo.DateStart));
            if (cargo.DateEnd.HasValue)
                parameters.Add(new SqlParameter("@endDate", cargo.DateEnd));
            else
                parameters.Add(new SqlParameter("@endDate", DBNull.Value));
            parameters.Add(new SqlParameter("@deleted", cargo.Deleted));
            string query = query = "UPDATE [RfidCard] "
                                 + "SET Name = @name "
                                 + "    , CargoCode = @cargoCode "
                                 + "    , DateStart = @startDate, DateEnd = @endDate, Deleted = @deleted "
                                 + "WHERE Id = @id;";

            var serverParameters = new List<SqlParameter>();
            serverParameters.Add(new SqlParameter("@exportRowId", cargo.ExportRowId));
            serverParameters.Add(new SqlParameter("@kioskId", 1));
            string localQuery = "INSERT INTO [ExportRowExported] (ExportRowId, KioskId, ExportedDate) output INSERTED.ID "
                              + "VALUES (@exportRowId, @kioskId, GetDate());";

            var sqlTransaction = LocalConnection.BeginTransaction();
            var result = LocalConnection.UpdateRow(query, parameters, sqlTransaction);
            var sqlServerTransaction = ServerConnection.BeginTransaction();
            var localResult = ServerConnection.InsertRow(localQuery, serverParameters, sqlServerTransaction);

            if ((int)result == 0 || (int)localResult == 0)
            {
                sqlTransaction.Rollback();
                sqlServerTransaction.Rollback();
                return false;
            }

            sqlTransaction.Commit();
            sqlServerTransaction.Commit();
            return true;
        }
        public bool CargoTypeAdd(CargoTypeDbRow cargo)
        {
            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@id", cargo.Id));
            parameters.Add(new SqlParameter("@name", cargo.Name));
            parameters.Add(new SqlParameter("@cargoCode", cargo.CargoCode));
            parameters.Add(new SqlParameter("@startDate", cargo.DateStart));
            if (cargo.DateEnd.HasValue)
                parameters.Add(new SqlParameter("@endDate", cargo.DateEnd));
            else
                parameters.Add(new SqlParameter("@endDate", DBNull.Value));
            parameters.Add(new SqlParameter("@deleted", cargo.Deleted));
            string query = query = "INSERT INTO [CargoType] (Id, Name, CargoCode "
                                 + "    , DateStart, DateEnd, Deleted) output INSERTED.ID "
                                 + "VALUES (@id, @name, @cargoCode, @startDate, @endDate, @deleted);";

            var serverParameters = new List<SqlParameter>();
            serverParameters.Add(new SqlParameter("@exportRowId", cargo.ExportRowId));
            serverParameters.Add(new SqlParameter("@kioskId", 1));
            string localQuery = "INSERT INTO [ExportRowExported] (ExportRowId, KioskId, ExportedDate) output INSERTED.ID "
                              + "VALUES (@exportRowId, @kioskId, GetDate());";

            var sqlTransaction = LocalConnection.BeginTransaction();
            var result = LocalConnection.InsertRow(query, parameters, sqlTransaction);
            var sqlServerTransaction = ServerConnection.BeginTransaction();
            var serverResult = ServerConnection.InsertRow(localQuery, serverParameters, sqlServerTransaction);

            if ((int)result == 0 || (int)serverResult == 0)
            {
                sqlTransaction.Rollback();
                sqlServerTransaction.Rollback();
                return false;
            }

            sqlTransaction.Commit();
            sqlServerTransaction.Commit();
            return true;
        }

        public void UpdateConfigurations(Dictionary<int, KioskConfiguration> configs)
        {
            foreach (int dayNo in configs.Keys)
            {
                UpdateConfigurations(configs[dayNo], dayNo);
            }

            UpdateConfigurationBlockages(configs[1].KioskBlockages);
            UpdateConfigurationParameters(configs[1].ConfigurationSettings);
        }
        private void UpdateConfigurations(KioskConfiguration configuration, int dayNo)
        {
            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@openFrom", configuration.OpenHoursFromInMinutesFromMidninght));
            parameters.Add(new SqlParameter("@openTo", configuration.OpenHoursToInMinutesFromMidninght));
            parameters.Add(new SqlParameter("@dayNo", dayNo));
            parameters.Add(new SqlParameter("@kioskId", LocalConnection.KioskId));
            string query = query = "UPDATE [GodzinyOtwarcia] "
                                 + "SET OtwarteOd = @openFrom, OtwarteDo = @openTo "
                                 + "WHERE KioskId = @kioskId AND DzienTygodnia = @dayNo;";

            var localResult = LocalConnection.UpdateRow(query, parameters);
        }
        public void UpdateKioskData(KioskDbRow kioskData)
        {
            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@pobieranieProbek", kioskData.PobieranieProbek));
            parameters.Add(new SqlParameter("@blokada", kioskData.Blokada));
            parameters.Add(new SqlParameter("@kioskId", LocalConnection.KioskId));
            string query = query = "UPDATE [Kiosk] "
                                 + "SET PobieranieProbek = @pobieranieProbek, Blokada = @blokada "
                                 + "WHERE Id = @kioskId;";

            var localResult = LocalConnection.UpdateRow(query, parameters);
        }
        private void UpdateConfigurationBlockages(Dictionary<KioskBlockageType, bool> blockages)
        {
            foreach(var blockType in blockages.Keys)
            {
                UpdateConfigurationBlockage(blockType, blockages[blockType]);
            }
        }
        private void UpdateConfigurationBlockage(KioskBlockageType blockageType, bool blocked)
        {
            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@kioskId", LocalConnection.KioskId));
            parameters.Add(new SqlParameter("@blockageTypeId", (int)blockageType));
            parameters.Add(new SqlParameter("@blocked", blocked));
            var query = "EXECUTE [_sp_update_configuration_blockage] @kioskId, @blockageTypeId, @blocked;";
            LocalConnection.UpdateRow(query, parameters);
        }
        private void UpdateConfigurationParameters(Dictionary<KioskConfigurationType, short> configuration)
        {
            foreach (var paramType in configuration.Keys)
            {
                UpdateConfigurationParameter(paramType, configuration[paramType]);
            }
        }
        private void UpdateConfigurationParameter(KioskConfigurationType paramType, short value)
        {
            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@kioskId", LocalConnection.KioskId));
            parameters.Add(new SqlParameter("@paramTypeId", (int)paramType));
            parameters.Add(new SqlParameter("@value", value));
            var query = "UPDATE KioskKonfiguracja "
                      + "SET Wartosc = @value " 
                      + "WHERE KioskId = @kioskId AND KonfiguracjaTypId = @paramTypeId;";
            LocalConnection.UpdateRow(query, parameters);
        }

        public bool UpdatePaperUsage(PrinterPaperStatus currentPaperStatus)
        {
            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@kioskId", ServerConnection.KioskId));
            parameters.Add(new SqlParameter("@paperUsage", currentPaperStatus.ReciptPrintedLenghtInMilimeters));
            string query = query = "UPDATE [dbo].[PrinterPaperStatus] "
                                 + "SET [PrintedReciptLenghtInMilimeters] = @paperUsage "
                                 + "WHERE KioskId = @kioskId;";

            var result = ServerConnection.UpdateRow(query, parameters);

            if ((int)result == 0)
            {
                return false;
            }
            return true;
        }

        public bool UpdateProbeStatus(ProbeStatus probeStatus)
        {
            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@kioskId", ServerConnection.KioskId));
            parameters.Add(new SqlParameter("@awaria", probeStatus.Error));
            parameters.Add(new SqlParameter("@programWlaczony", probeStatus.ProgramOn));
            parameters.Add(new SqlParameter("@butelka", probeStatus.BottleNo));
            string query = query = "UPDATE [dbo].[PobierakStatus] "
                                 + "SET Awaria = @awaria, "
                                 + "    ProgramWlaczony = @programWlaczony, "
                                 + "    Butelka = @butelka, "
                                 + "    DataAktualizacji = GETDATE() "
                                 + "WHERE KioskId = @kioskId;";

            var result = ServerConnection.UpdateRow(query, parameters);

            if ((int)result == 0)
            {
                return false;
            }
            return true;
        }

        public void UpdateRowExported(int exportRowId)
        {
            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@exportRowId", exportRowId));
            parameters.Add(new SqlParameter("@kioskId", LocalConnection.KioskId));
            string query = query = "INSERT INTO [ExportRowExported] (ExportRowId, KioskId, ExportedDate) "
                                 + "VALUES (@exportRowId, @kioskId, GETDATE());";

            var localResult = ServerConnection.UpdateRow(query, parameters);
        }
    }
}
