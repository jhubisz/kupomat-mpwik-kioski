using DbCommunication.Entities;
using DbCommunication.Entities.DbRows;
using DbCommunication.Enums;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace DbCommunication
{
    public class DbRead
    {
        private IDbConnection connection;

        private DbRead() { }
        public DbRead(IDbConnection conn)
        {
            connection = conn;
        }

        public bool SqlServerLiveBeat()
        {
            try
            {
                string query = "SELECT 1;";
                var result = connection.GetScalar(query);
                if (result != null && result is int && (int)result == 1)
                    return true;
            }
            catch
            {
                return false;
            }
            return false;
        }

        public List<CargoType> GetCargoTypes()
        {
            string query = "SELECT rs.Id, rs.Nazwa FROM dbo.RodzajSciekow rs WHERE rs.Deleted = 0;";
            var result = connection.GetTable(query);

            if (result == null || result.Rows.Count == 0)
                throw new Exception("Brak typów ścieku");

            var cargoTypes = new List<CargoType>();
            foreach (DataRow row in result.Rows)
                cargoTypes.Add(new CargoType() { Id = (int)row["Id"], Name = row["Nazwa"].ToString() });

            return cargoTypes;
        }

        public RfidCard GetRfidCard(string cardId)
        {
            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@kartaId", cardId));
            string query = "SELECT k.Id, k.KartaId, k.SamochodId, k.Blokada as BlokadaKarty, KartaWymianyTacki "
                         + "FROM dbo.Karta k "
                         + "WHERE k.KartaId = @kartaId AND k.Deleted = 0; ";
            var result = connection.GetTable(query, parameters);

            if (result == null || result.Rows.Count != 1)
                return null;

            DataRow row = result.Rows[0];

            RfidCard card;

            var cardType = (bool)row["KartaWymianyTacki"] ? RfidCardType.Superuser : RfidCardType.Customer;

            if (cardType == RfidCardType.Customer)
            {
                var vechicle = GetVechicle((int)row["SamochodId"]);

                card = new RfidCard()
                {
                    Id = (int)row["Id"],
                    CardId = cardId,
                    Type = cardType,
                    BlockedCard = (bool)row["BlokadaKarty"],
                    Vechicle = vechicle,
                    Customer = vechicle.Customer,
                    Address = GetAddress(vechicle.Customer)
                };
            }
            else 
            {
                card = new RfidCard()
                {
                    Id = (int)row["Id"],
                    CardId = cardId,
                    Type = cardType,
                };
            }

            return card;
        }

        public RfidCard GetRfidCardByLicensePlate(string licensePlate)
        {
            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@rejestracja", licensePlate));
            string query = "SELECT k.Id, k.KartaId, k.SamochodId, k.Blokada as BlokadaKarty, KartaWymianyTacki "
                         + "FROM dbo.Karta k "
                         + "    INNER JOIN dbo.Samochod s ON k.SamochodId = s.Id "
                         + "    INNER JOIN dbo.Klient kl ON kl.Id = s.KlientId AND kl.Deleted = 0 "
                         + "WHERE REPLACE(s.Rejestracja, ' ', '') = @rejestracja AND k.Deleted = 0 AND s.Deleted = 0";
            var result = connection.GetTable(query, parameters);

            if (result == null || result.Rows.Count == 0)
                return null;

            DataRow row = result.Rows[0];

            RfidCard card;

            var cardType = (bool)row["KartaWymianyTacki"] ? RfidCardType.Superuser : RfidCardType.Customer;

            if (cardType == RfidCardType.Customer)
            {
                var vechicle = GetVechicle((int)row["SamochodId"]);

                card = new RfidCard()
                {
                    Id = (int)row["Id"],
                    CardId = row["KartaId"].ToString(),
                    Type = cardType,
                    BlockedCard = (bool)row["BlokadaKarty"],
                    Vechicle = vechicle,
                    Customer = vechicle.Customer,
                    Address = GetAddress(vechicle.Customer)
                };
            }
            else
            {
                card = new RfidCard()
                {
                    Id = (int)row["Id"],
                    CardId = row["KartaId"].ToString(),
                    Type = cardType,
                };
            }

            return card;
        }

        public Vechicle GetVechicle(int vechicleId)
        {
            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@samochodId", vechicleId));
            string query = "SELECT s.Id, s.Rejestracja, s.KlientId FROM dbo.Samochod s WHERE s.Id = @samochodId AND s.Deleted = 0;";
            var result = connection.GetTable(query, parameters);

            if (result == null || result.Rows.Count != 1)
                return null;

            DataRow row = result.Rows[0];
            var vechicle = new Vechicle()
            {
                Id = (int)row["Id"],
                LicensePlate = row["Rejestracja"].ToString(),
                Customer = GetCustomer((int)row["KlientId"])
            };

            return vechicle;
        }

        public Customer GetCustomer(int customerId)
        {
            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@klientId", customerId));
            string query = "SELECT k.Id, k.Nazwa, k.PobieranieProbek, k.Blokada FROM dbo.Klient k WHERE k.Id = @klientId AND k.Deleted = 0;";
            var result = connection.GetTable(query, parameters);

            if (result == null || result.Rows.Count != 1)
                return null;

            DataRow row = result.Rows[0];
            var customer = new Customer() { Id = (int)row["Id"], Name = row["Nazwa"].ToString(), TakingSamples = (bool)row["PobieranieProbek"], Blocked = (bool)row["Blokada"] };

            return customer;
        }

        public Address GetAddress(Customer customer)
        {
            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@klientId", customer.Id));
            string query = "SELECT k.Id, k.Nazwa, k.NIP, k.REGON, k.KodPocztowy, k.Miejscowosc, k.AdresLinia1, k.AdresLinia2, k.Tel1, k.Tel2, k.NumerUmowy "
                         + "FROM dbo.Klient k "
                         + "WHERE k.Id = @klientId AND k.Deleted = 0;";
            var result = connection.GetTable(query, parameters);

            if (result == null || result.Rows.Count != 1)
                return null;

            DataRow row = result.Rows[0];
            var address = new Address() {
                Id = (int)row["Id"],
                Name = row["Nazwa"].ToString(),
                Nip = row["NIP"].ToString(),
                Regon = row["REGON"].ToString(),
                PostCode = row["KodPocztowy"].ToString(),
                City = row["Miejscowosc"].ToString(),
                AddressLine1 = row["AdresLinia1"].ToString(),
                AddressLine2 = row["AdresLinia2"].ToString(),
                Tel1 = row["Tel1"].ToString(),
                Tel2 = row["Tel2"].ToString(),
                ContractNo = row["NumerUmowy"].ToString(),
                Customer = customer
            };

            return address;
        }
        
        public StationLicense GetLicense(int customerId)
        {
            return GetLicense(customerId, connection.KioskId);
        }
        public StationLicense GetLicense(int customerId, int kioskId)
        {
            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@klientId", customerId));
            parameters.Add(new SqlParameter("@kioskId", kioskId));
            string query = "SELECT l.Id, l.LicencjaNumer "
                         + "    , l.DataStart, ISNULL(l.DataKoniec, '2100-01-01') AS DataKoniec "
                         + "	, DATEDIFF(DAY, GETDATE(), ISNULL(l.DataKoniec, '2100-01-01')) AS DniDoKonca "
                         + "FROM dbo.Licencja l "
                         + "    INNER JOIN dbo.Lokalizacja lk ON l.LokalizacjaId = lk.Id AND lk.Deleted = 0 "
                         + "    INNER JOIN dbo.Kiosk k ON k.LokalizacjaId = lk.Id AND k.Deleted = 0 "
                         + "WHERE k.Id = @kioskId AND l.KlientId = @klientId AND l.Deleted = 0 AND GETDATE() BETWEEN l.DataStart AND ISNULL(l.DataKoniec, '2100-01-01');";
            var result = connection.GetTable(query, parameters);

            if (result == null || result.Rows.Count != 1)
                return null;

            DataRow row = result.Rows[0];
            var address = new StationLicense()
            {
                Id = (int)row["Id"],
                Number = row["LicencjaNumer"].ToString(),
                DataStart = (DateTime)row["DataStart"],
                DataKoniec = (DateTime)row["DataKoniec"],
                DaysTillEnd = (int)row["DniDoKonca"],
            };

            return address;
        }

        public KioskConfiguration GetKioskConfiguration()
        {
            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@kioskId", connection.KioskId));
            parameters.Add(new SqlParameter("@dzienTygodnia", ((int)DateTime.Now.DayOfWeek == 0) ? 7 : (int)DateTime.Now.DayOfWeek));
            
            string query = "SELECT c.OtwarteOd, c.OtwarteDo "
                         + "FROM dbo.GodzinyOtwarcia c "
                         + "WHERE c.KioskId = @kioskId AND c.DzienTygodnia = @dzienTygodnia;";
            var result = connection.GetTable(query, parameters);

            if (result == null || result.Rows.Count != 1)
                return null;

            DataRow row = result.Rows[0];
            var kioskConf = new KioskConfiguration()
            {
                OpenHoursFromInMinutesFromMidninght = (int)row["OtwarteOd"],
                OpenHoursToInMinutesFromMidninght = (int)row["OtwarteDo"],
            };
            kioskConf.KioskBlockages = GetConfigurationBlockages(kioskConf.KioskBlockages);
            kioskConf.ConfigurationSettings = GetConfigurationSettings(kioskConf.ConfigurationSettings);
            kioskConf.KioskWiadomosc = GetConfigurationKioskWiadomosc();

            return kioskConf;
        }
        public Dictionary<KioskBlockageType, bool> GetConfigurationBlockages(Dictionary<KioskBlockageType, bool> currentBlockages)
        {
            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@kioskId", connection.KioskId));

            string query = "SELECT kbt.Id as BlokadaTypId, CASE WHEN kb.Id IS NOT NULL THEN 1 ELSE 0 END as Blokada "
                         + "FROM KioskBlokadaTyp kbt "
                         + "    LEFT JOIN KioskBlokada kb ON kbt.Id = kb.BlokadaTypId AND kb.KioskId = @kioskId AND kb.Deleted = 0 "
                         + "        AND GETDATE() BETWEEN kb.DataStart AND ISNULL(kb.DataKoniec, '2100-01-01');";
            var result = connection.GetTable(query, parameters);

            if (result == null || result.Rows.Count == 0)
            {
                return currentBlockages;
            }
            
            foreach (DataRow blockage in result.Rows)
            {
                var blockageId = (int)blockage["BlokadaTypId"];
                var blockageType = (KioskBlockageType)blockageId;
                currentBlockages[blockageType] = blockage["Blokada"].ToString() == "1";
            }

            return currentBlockages;
        }
        public Dictionary<KioskConfigurationType, short> GetConfigurationSettings(Dictionary<KioskConfigurationType, short> currentSettings)
        {
            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@kioskId", connection.KioskId));

            string query = "SELECT k.KonfiguracjaTypId, k.Wartosc "
                         + "FROM KioskKonfiguracja k "
                         + "WHERE KioskId = @kioskId "
                         + "        AND GETDATE() BETWEEN k.DataStart AND ISNULL(k.DataKoniec, '2100-01-01');";
            var result = connection.GetTable(query, parameters);

            if (result == null || result.Rows.Count == 0)
            {
                return currentSettings;
            }

            foreach (DataRow settingsItem in result.Rows)
            {
                var configType = (KioskConfigurationType)settingsItem["KonfiguracjaTypId"];
                var value = (short)settingsItem["Wartosc"];
                currentSettings[configType] = value;
            }

            return currentSettings;
        }
        public string GetConfigurationKioskWiadomosc()
        {
            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@kioskId", connection.KioskId));

            string query = "SELECT Wiadomosc "
                         + "FROM KioskWiadomosc kw "
                         + "WHERE kw.KioskId = @kioskId AND GETDATE() BETWEEN kw.DataStart AND ISNULL(kw.DataKoniec, '2100-01-01');";
            var result = connection.GetTable(query, parameters);

            if (result == null || result.Rows.Count == 0)
            {
                return "";
            }

            return result.Rows[0]["Wiadomosc"].ToString();
        }

        public bool CheckIfKioskIsBlocked(int kioskId)
        {
            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@kioskId", kioskId));
            string query = "SELECT k.Blokada "
                         + "FROM dbo.Kiosk k "
                         + "WHERE k.Id = @kioskId;";
            var result = connection.GetTable(query, parameters);

            if (result == null || result.Rows.Count != 1)
                return false;

            DataRow row = result.Rows[0];
            var kioskBlocked = (bool)row["Blokada"];

            return kioskBlocked;
        }

        public List<TransactionDbRow> GetTransactionDbRowsToExport()
        {
            List<TransactionDbRow> list = new List<TransactionDbRow>();
            string query = "SELECT Id, KioskId, RodzajSciekowId, KlientId, SamochodId, KartaId, NumerUmowy, ZadeklarowanaIlosc, ZlanaIlosc "
                         + "    , TransakcjaStart, ZrzutStart, ZrzutKoniec, TransakcjaKoniec "
                         + "    , PobranoProbeHarm, PobranoProbeAlrm, ZakonczonaPoprawnie, POwodZakonczenia "
                         + "FROM dbo.[Transakcja] t "
                         + "WHERE NOT EXISTS (SELECT 1 FROM dbo.TransakcjaExport te WHERE te.TransakcjaId = t.Id);";
            var result = connection.GetTable(query);

            if (result == null)
                return list;

            foreach (DataRow row in result.Rows)
            {
                var transaction = new TransactionDbRow()
                {
                    Id = (int)row["Id"],

                    KioskId = (int)row["KioskId"],

                    RodzajSciekowId = (int)row["RodzajSciekowId"],
                    KlientId = (int)row["KlientId"],
                    SamochodId = (int)row["SamochodId"],
                    KartaId = (int)row["KartaId"],
                    NumerUmowy = row["NumerUmowy"].ToString(),

                    ZadeklarowanaIlosc = (int)row["ZadeklarowanaIlosc"],
                    ZlanaIlosc = (decimal)row["ZlanaIlosc"],

                    TransakcjaStart = (DateTime)row["TransakcjaStart"],
                    ZrzutStart = (DateTime)row["ZrzutStart"],
                    ZrzutKoniec = (DateTime)row["ZrzutKoniec"],
                    TransakcjaKoniec = (DateTime)row["TransakcjaKoniec"],

                    PobranoProbeHarm = (bool)row["PobranoProbeHarm"],
                    PobranoProbeAlrm = (bool)row["PobranoProbeAlrm"],

                    ZakonczonaPoprawnie = (bool)row["ZakonczonaPoprawnie"],
                    PowodZakonczenia = row["PowodZakonczenia"].ToString()
                };

                list.Add(transaction);
            }

            return list;
        }
        public List<TransactionAddressDbRow> GetTransactionAddressesDbRowsToExport()
        {
            var list = new List<TransactionAddressDbRow>();

            var parameters = new List<SqlParameter>();
            string query = "SELECT Id, KioskId, TransakcjaId, ZadeklarowanaIlosc, NumerUmowy, AdresId, FirmaId, RodId, RodNrDzialki "
                         + "FROM dbo.[TransakcjaAdres] ta "
                         + "WHERE NOT EXISTS (SELECT 1 FROM dbo.TransakcjaAdresExport te WHERE te.TransakcjaAdresId = ta.Id);";
            var result = connection.GetTable(query);

            if (result == null)
                return list;

            foreach (DataRow row in result.Rows)
            {
                var transactionAddress = new TransactionAddressDbRow()
                {
                    Id = (int)row["Id"],

                    KioskId = (int)row["KioskId"],

                    TransakcjaId = (int)row["TransakcjaId"],
                    ZadeklarowanaIlosc = (decimal)row["ZadeklarowanaIlosc"],
                    NumerUmowy = row["NumerUmowy"].ToString(),

                    RodNrDzialki = row["RodNrDzialki"].ToString()
                };

                if (row["AdresId"].ToString() != "")
                    transactionAddress.AdresId = (int)row["AdresId"];
                if (row["FirmaId"].ToString() != "")
                    transactionAddress.FirmaId = (int)row["FirmaId"];
                if (row["RodId"].ToString() != "")
                    transactionAddress.RodId = (int)row["RodId"];

                list.Add(transactionAddress);
            }

            return list;
        }
        public List<TransactionParametersDbRow> GetTransactionParametersDbRowsToExport()
        {
            var list = new List<TransactionParametersDbRow>();

            var parameters = new List<SqlParameter>();
            string query = "SELECT Id, KioskId, TransakcjaId, PhMin, PhMax, PhAvg, PrzewodnoscMin, PrzewodnoscMax, PrzewodnoscAvg, TempMin, TempMax, TempAvg, ChztMin, ChztMax, ChztAvg "
                         + "FROM dbo.[TransakcjaParametry] tp "
                         + "WHERE NOT EXISTS (SELECT 1 FROM dbo.TransakcjaParametryExport te WHERE te.TransakcjaParametryId = tp.Id);";
            var result = connection.GetTable(query);

            if (result == null)
                return list;

            foreach (DataRow row in result.Rows)
            {
                var transactionParameter = new TransactionParametersDbRow()
                {
                    Id = (int)row["Id"],

                    KioskId = (int)row["KioskId"],

                    TransakcjaId = (int)row["TransakcjaId"]
                };

                if (row["PhMin"].ToString() != "")
                    transactionParameter.PhMin = (decimal)row["PhMin"];
                if (row["PhMax"].ToString() != "")
                    transactionParameter.PhMax = (decimal)row["PhMax"];
                if (row["PhAvg"].ToString() != "")
                    transactionParameter.PhAvg = (decimal)row["PhAvg"];

                if (row["PrzewodnoscMin"].ToString() != "")
                    transactionParameter.PrzewodnoscMin = (decimal)row["PrzewodnoscMin"];
                if (row["PrzewodnoscMax"].ToString() != "")
                    transactionParameter.PrzewodnoscMax = (decimal)row["PrzewodnoscMax"];
                if (row["PrzewodnoscAvg"].ToString() != "")
                    transactionParameter.PrzewodnoscAvg = (decimal)row["PrzewodnoscAvg"];

                if (row["TempMin"].ToString() != "")
                    transactionParameter.TempMin = (decimal)row["TempMin"];
                if (row["TempMax"].ToString() != "")
                    transactionParameter.TempMax = (decimal)row["TempMax"];
                if (row["TempAvg"].ToString() != "")
                    transactionParameter.TempAvg = (decimal)row["TempAvg"];

                if (row["ChztMin"].ToString() != "")
                    transactionParameter.ChztMin = (decimal)row["ChztMin"];
                if (row["ChztMax"].ToString() != "")
                    transactionParameter.ChztMax = (decimal)row["ChztMax"];
                if (row["ChztAvg"].ToString() != "")
                    transactionParameter.ChztAvg = (decimal)row["ChztAvg"];

                list.Add(transactionParameter);
            }

            return list;
        }

        public List<GminaDbRow> GetGminaDbRowsToExport()
        {
            var list = new List<GminaDbRow>();

            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@kioskId", connection.KioskId));
            string query = "SELECT Id, KioskId, PowiatId, Nazwa, RodzajId, DataStart "
                         + "FROM dbo.[Gmina] g "
                         + "WHERE g.KioskId = @kioskId AND NOT EXISTS (SELECT 1 FROM dbo.GminaExport ge WHERE ge.GminaId = g.Id);";
            var result = connection.GetTable(query, parameters);

            if (result == null)
                return list;

            foreach (DataRow row in result.Rows)
            {
                var address = new GminaDbRow()
                {
                    Id = (int)row["Id"],
                    KioskId = (int)row["KioskId"],
                    PowiatId = (int)row["PowiatId"],
                    Nazwa = row["Nazwa"].ToString(),
                    RodzajId = (int)row["RodzajId"],
                    DataStart = (DateTime)row["DataStart"],
                };

                list.Add(address);
            }

            return list;
        }
        public List<MiejscowoscDbRow> GetMiejscowoscDbRowsToExport()
        {
            var list = new List<MiejscowoscDbRow>();

            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@kioskId", connection.KioskId));
            string query = "SELECT Id, KioskId, Nazwa, GminaId, RodzajId, DataStart " 
                         + "FROM dbo.[Miejscowosc] m "
                         + "WHERE m.KioskId = @kioskId AND NOT EXISTS(SELECT 1 FROM dbo.MiejscowoscExport me WHERE me.MiejscowoscId = m.Id);";
            var result = connection.GetTable(query, parameters);

            if (result == null)
                return list;

            foreach (DataRow row in result.Rows)
            {
                var address = new MiejscowoscDbRow()
                {
                    Id = (int)row["Id"],
                    KioskId = (int)row["KioskId"],
                    Nazwa = row["Nazwa"].ToString(),
                    GminaId = (int)row["GminaId"],
                    RodzajId = (int)row["RodzajId"],
                    DataStart = (DateTime)row["DataStart"],
                };

                list.Add(address);
            }

            return list;
        }
        public List<UlicaDbRow> GetUlicaDbRowsToExport()
        {
            var list = new List<UlicaDbRow>();

            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@kioskId", connection.KioskId));
            string query = "SELECT Id, KioskId, Nazwa, MiejscowoscId, GminaId, RodzajId, DataStart "
                         + "FROM dbo.[Ulica] u "
                         + "WHERE u.KioskId = @kioskId AND NOT EXISTS(SELECT 1 FROM dbo.UlicaExport ue WHERE ue.UlicaId = u.Id);";
            var result = connection.GetTable(query, parameters);

            if (result == null)
                return list;

            foreach (DataRow row in result.Rows)
            {
                var address = new UlicaDbRow()
                {
                    Id = (int)row["Id"],
                    KioskId = (int)row["KioskId"],
                    Nazwa = row["Nazwa"].ToString(),
                    MiejscowoscId = (int)row["MiejscowoscId"],
                    GminaId = (int)row["GminaId"],
                    RodzajId = (int)row["RodzajId"],
                    DataStart = (DateTime)row["DataStart"],
                };

                list.Add(address);
            }

            return list;
        }

        public List<FirmaDbRow> GetFirmaDbRowsToExport()
        {
            var list = new List<FirmaDbRow>();

            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@kioskId", connection.KioskId));
            string query = "SELECT Id, KioskId, Nazwa, DataStart "
                         + "FROM dbo.[Firma] f "
                         + "WHERE f.KioskId = @kioskId AND NOT EXISTS(SELECT 1 FROM dbo.FirmaExport fe WHERE fe.FirmaId = f.Id);";
            var result = connection.GetTable(query, parameters);

            if (result == null)
                return list;

            foreach (DataRow row in result.Rows)
            {
                var address = new FirmaDbRow()
                {
                    Id = (int)row["Id"],
                    KioskId = (int)row["KioskId"],
                    Nazwa = row["Nazwa"].ToString(),
                    DataStart = (DateTime)row["DataStart"],
                };

                list.Add(address);
            }

            return list;
        }
        public List<FirmaAdresDbRow> GetFirmaAdresDbRowsToExport()
        {
            var list = new List<FirmaAdresDbRow>();

            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@kioskId", connection.KioskId));
            string query = "SELECT Id, KioskId, AdresId, FirmaId, DataStart "
                         + "FROM dbo.[FirmaAdres] f "
                         + "WHERE f.KioskId = @kioskId AND NOT EXISTS(SELECT 1 FROM dbo.FirmaAdresExport fe WHERE fe.FirmaAdresId = f.Id);";
            var result = connection.GetTable(query, parameters);

            if (result == null)
                return list;

            foreach (DataRow row in result.Rows)
            {
                var address = new FirmaAdresDbRow()
                {
                    Id = (int)row["Id"],
                    KioskId = (int)row["KioskId"],
                    AdresId = (int)row["AdresId"],
                    FirmaId = (int)row["FirmaId"],
                    DataStart = (DateTime)row["DataStart"],
                };

                list.Add(address);
            }

            return list;
        }

        public List<StalyKlientDbRow> GetStalyKlientDbRowsToExport()
        {
            var list = new List<StalyKlientDbRow>();

            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@kioskId", connection.KioskId));
            string query = "SELECT Id, KioskId, AdresId, FirmaId, KlientId, DataStart "
                         + "FROM dbo.[StalyKlient] sk "
                         + "WHERE sk.KioskId = @kioskId AND NOT EXISTS(SELECT 1 FROM dbo.StalyKlientExport ske WHERE ske.StalyKlientId = sk.Id);";
            var result = connection.GetTable(query, parameters);

            if (result == null)
                return list;

            foreach (DataRow row in result.Rows)
            {
                var stalyKlient = new StalyKlientDbRow()
                {
                    Id = (int)row["Id"],
                    KioskId = (int)row["KioskId"],
                    AdresId = (int)row["AdresId"],
                    KlientId = (int)row["KlientId"],
                    DataStart = (DateTime)row["DataStart"],
                };

                if (row["FirmaId"] is DBNull)
                    stalyKlient.FirmaId = null;
                else
                    stalyKlient.FirmaId = (int)row["FirmaId"];

                list.Add(stalyKlient);
            }

            return list;
        }

        public List<AdresDbRow> GetAddressesDbRowsToExport()
        {
            List<AdresDbRow> list = new List<AdresDbRow>();

            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@kioskId", connection.KioskId));
            string query = "SELECT Id, KioskId, Nazwa, UlicaId, MiejscowoscId, GminaId, Numer, NumerUmowy, RodzajId, DataStart, DataKoniec, Deleted "
                         + "FROM dbo.[Adres] a "
                         + "WHERE a.KioskId = @kioskId AND NOT EXISTS (SELECT 1 FROM dbo.AdresExport ae WHERE ae.AdresId = a.Id);";
            var result = connection.GetTable(query, parameters);

            if (result == null)
                return list;

            foreach (DataRow row in result.Rows)
            {
                var address = new AdresDbRow()
                {
                    Id = (int)row["Id"],
                    KioskId = (int)row["KioskId"],

                    Nazwa = row["Nazwa"].ToString(),
                    MiejscowoscId = (int)row["MiejscowoscId"],
                    GminaId = (int)row["GminaId"],
                    Numer = row["Numer"].ToString(),
                    NumerUmowy = row["NumerUmowy"].ToString(),
                    RodzajId = (int)row["RodzajId"],
                    DataStart = (DateTime)row["DataStart"],
                };
                if (row["UlicaId"].ToString() != "")
                {
                    address.UlicaId = (int)row["UlicaId"];
                }

                list.Add(address);
            }

            return list;
        }

        public List<ProbaTackaDbRow> GetProbaTackaDbRowsToExport()
        {
            List<ProbaTackaDbRow> list = new List<ProbaTackaDbRow>();

            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@kioskId", connection.KioskId));
            string query = "SELECT Id, KioskId, TackaNr, DataStart "
                         + "FROM dbo.[ProbaTacka] pt "
                         + "WHERE NOT EXISTS (SELECT 1 FROM dbo.ProbaTackaExport pte WHERE pte.ProbaTackaId = pt.Id) "
                         + "ORDER BY Id;";
            var result = connection.GetTable(query, parameters);

            if (result == null)
                return list;

            foreach (DataRow row in result.Rows)
            {
                var probaTacka = new ProbaTackaDbRow()
                {
                    Id = (int)row["Id"],
                    KioskId = (int)row["KioskId"],
                    TackaNr = (int)row["TackaNr"],

                    DataStart = (DateTime)row["DataStart"],
                };

                list.Add(probaTacka);
            }

            return list;
        }
        public List<ProbaButelkaDbRow> GetProbaButelkaDbRowsToExport()
        {
            List<ProbaButelkaDbRow> list = new List<ProbaButelkaDbRow>();

            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@kioskId", connection.KioskId));
            string query = "SELECT Id, KioskId, TackaId, NumerButelki, ProbaPobrana, ProbaData, ProbaTypId, ProbaTransakcjaId, DataStart "
                         + "FROM dbo.[ProbaButelka] pb "
                         + "WHERE NOT EXISTS (SELECT 1 FROM dbo.ProbaButelkaExport pbe WHERE pbe.ProbaButelkaId = pb.Id);";
            var result = connection.GetTable(query, parameters);

            if (result == null)
                return list;

            foreach (DataRow row in result.Rows)
            {
                var probaTacka = new ProbaButelkaDbRow()
                {
                    Id = (int)row["Id"],
                    KioskId = (int)row["KioskId"],
                    TackaId = (int)row["TackaId"],
                    NumerButelki = (int)row["NumerButelki"],
                    ProbaPobrana = (bool)row["ProbaPobrana"],
                    ProbaData = (DateTime)row["ProbaData"],
                    ProbaTypId = (int)row["ProbaTypId"],
                    ProbaTransakcjaId = (int)row["ProbaTransakcjaId"],
                    DataStart = (DateTime)row["DataStart"],
                };

                list.Add(probaTacka);
            }

            return list;
        }
        public List<ProbaTackaParagonDbRow> GetProbaTackaParagonDbRowsToExport()
        {
            List<ProbaTackaParagonDbRow> list = new List<ProbaTackaParagonDbRow>();

            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@kioskId", connection.KioskId));
            string query = "SELECT Id, KioskId, TackaId, LiczbaProbHarm, LiczbaProbAlrm, Wydrukowano, DataWydruku "
                         + "FROM dbo.[ProbaTackaParagon] pt "
                         + "WHERE NOT EXISTS (SELECT 1 FROM dbo.ProbaTackaParagonExport pte WHERE pte.ProbaTackaParagonId = pt.Id);";
            var result = connection.GetTable(query, parameters);

            if (result == null)
                return list;

            foreach (DataRow row in result.Rows)
            {
                var probaTacka = new ProbaTackaParagonDbRow()
                {
                    Id = (int)row["Id"],
                    KioskId = (int)row["KioskId"],
                    TackaId = (int)row["TackaId"],
                    LiczbaProbHarm = (int)row["LiczbaProbHarm"],
                    LiczbaProbAlrm = (int)row["LiczbaProbAlrm"],
                    Wydrukowano = (int)row["Wydrukowano"] == 1,
                };
                if (row["DataWydruku"].ToString() != "")
                    probaTacka.DataWydruku = (DateTime)row["DataWydruku"];

                list.Add(probaTacka);
            }

            return list;
        }

        public List<Gmina> GetAllGmina()
        {
            List<Gmina> list = new List<Gmina>();
            string query = "SELECT g.Id, g.Nazwa, [dbo].[fUsunPolskieZnaki](g.Nazwa) as NazwaBezPl, p.Nazwa as Powiat, w.Nazwa as Wojewodztwo "
                         + "FROM dbo.[Gmina] g " 
                         + "    INNER JOIN dbo.[Powiat] p ON g.PowiatId = p.Id "
                         + "    INNER JOIN dbo.[Wojewodztwo] w ON p.WojewodztwoId = w.Id "
                         + "WHERE w.Id = 1 AND g.Deleted = 0;";
            var result = connection.GetTable(query);

            if (result == null)
                return list;

            foreach (DataRow row in result.Rows)
            {
                var gmina = new Gmina()
                {
                    Id = (int)row["Id"],
                    Name = row["Nazwa"].ToString(),
                    NameWithoutPl = row["NazwaBezPl"].ToString(),
                    Powiat = row["Powiat"].ToString(),
                    Wojewodztwo = row["Wojewodztwo"].ToString(),
                };

                list.Add(gmina);
            }

            return list;
        }
        public List<Gmina> GetAllRegularGmina(int customerId) 
        {
            List<Gmina> list = new List<Gmina>();

            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@customerId", customerId));
            string query = "SELECT DISTINCT g.Id, g.Nazwa, [dbo].[fUsunPolskieZnaki](g.Nazwa) as NazwaBezPl, p.Nazwa as Powiat, w.Nazwa as Wojewodztwo "
                         + "FROM dbo.[Gmina] g "
                         + "    INNER JOIN dbo.[Powiat] p ON g.PowiatId = p.Id "
                         + "    INNER JOIN dbo.[Wojewodztwo] w ON p.WojewodztwoId = w.Id "
                         + "    INNER JOIN dbo.[Adres] a ON a.GminaId = g.Id "
                         + "    INNER JOIN dbo.[StalyKlient] sk ON sk.AdresId = a.Id "
                         + "WHERE sk.KlientId = @customerId AND g.Deleted = 0;";        
            var result = connection.GetTable(query, parameters);

            if (result == null)
                return list;

            foreach (DataRow row in result.Rows)
            {
                var gmina = new Gmina()
                {
                    Id = (int)row["Id"],
                    Name = row["Nazwa"].ToString(),
                    NameWithoutPl = row["NazwaBezPl"].ToString(),
                    Powiat = row["Powiat"].ToString(),
                    Wojewodztwo = row["Wojewodztwo"].ToString(),
                };

                list.Add(gmina);
            }

            return list;
        }

        public List<Miejscowosc> GetAllMiasta(int gminaId)
        {
            List<Miejscowosc> list = new List<Miejscowosc>();

            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@gminaId", gminaId));
            string query = "SELECT m.Id, m.Nazwa, [dbo].[fUsunPolskieZnaki](m.Nazwa) as NazwaBezPl, g.Nazwa as Gmina "
                         + "FROM dbo.[Miejscowosc] m "
                         + "    INNER JOIN dbo.[Gmina] g ON m.GminaId = g.Id "
                         + "WHERE m.GminaId = @gminaId";
            var result = connection.GetTable(query, parameters);

            if (result == null)
                return list;

            foreach (DataRow row in result.Rows)
            {
                var miejscowosc = new Miejscowosc()
                {
                    Id = (int)row["Id"],
                    Name = row["Nazwa"].ToString(),
                    NameWithoutPl = row["NazwaBezPl"].ToString(),
                    Gmina = row["Gmina"].ToString(),
                    GminaId = gminaId,
                };

                list.Add(miejscowosc);
            }

            return list;
        }

        public List<Miejscowosc> GetAllRegularMiasta(int gminaId, int customerId) 
        {
            List<Miejscowosc> list = new List<Miejscowosc>();

            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@gminaId", gminaId));
            parameters.Add(new SqlParameter("@customerId", customerId));
            string query = "SELECT DISTINCT m.Id, m.Nazwa, [dbo].[fUsunPolskieZnaki](m.Nazwa) as NazwaBezPl, g.Nazwa as Gmina "
                         + "FROM dbo.[Miejscowosc] m "
                         + "    INNER JOIN dbo.[Gmina] g ON m.GminaId = g.Id "
                         + "    INNER JOIN dbo.[Adres] a ON a.GminaId = g.Id AND a.MiejscowoscId = m.Id "
                         + "    INNER JOIN dbo.[StalyKlient] sk ON sk.AdresId = a.Id "
                         + "WHERE m.GminaId = @gminaId "
                         + "    AND sk.KlientId = @customerId;";
            var result = connection.GetTable(query, parameters);

            if (result == null)
                return list;

            foreach (DataRow row in result.Rows)
            {
                var miejscowosc = new Miejscowosc()
                {
                    Id = (int)row["Id"],
                    Name = row["Nazwa"].ToString(),
                    NameWithoutPl = row["NazwaBezPl"].ToString(),
                    Gmina = row["Gmina"].ToString(),
                    GminaId = gminaId,
                };

                list.Add(miejscowosc);
            }

            return list;
        }

        public List<Ulica> GetAllUlice(int miejscowoscId)
        {
            List<Ulica> list = new List<Ulica>();

            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@miejscowoscId", miejscowoscId));
            string query = "SELECT u.Id, u.Nazwa, [dbo].[fUsunPolskieZnaki](u.Nazwa) as NazwaBezPl, m.Nazwa as Miejscowosc "
                         + "FROM dbo.[Ulica] u "
                         + "    INNER JOIN dbo.[Miejscowosc] m ON u.MiejscowoscId = m.Id "
                         + "WHERE u.MiejscowoscId = @miejscowoscId";
            var result = connection.GetTable(query, parameters);

            if (result == null)
                return list;

            foreach (DataRow row in result.Rows)
            {
                var ulica = new Ulica()
                {
                    Id = (int)row["Id"],
                    Name = row["Nazwa"].ToString(),
                    NameWithoutPl = row["NazwaBezPl"].ToString(),
                    Miejscowosc = row["Miejscowosc"].ToString(),
                    MiejscowoscId = miejscowoscId,
                };

                list.Add(ulica);
            }

            return list;
        }

        public List<Ulica> GetAllRegularUlice(int miejscowoscId, int customerId) 
        {
            List<Ulica> list = new List<Ulica>();

            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@miejscowoscId", miejscowoscId));
            parameters.Add(new SqlParameter("@customerId", customerId));
            string query = "SELECT DISTINCT u.Id, u.Nazwa, [dbo].[fUsunPolskieZnaki](u.Nazwa) as NazwaBezPl, m.Nazwa as Miejscowosc "
                         + "FROM dbo.[Ulica] u "
                         + "    INNER JOIN dbo.[Miejscowosc] m ON u.MiejscowoscId = m.Id "
                         + "    INNER JOIN dbo.[Adres] a ON a.MiejscowoscId = m.Id AND a.UlicaId = u.Id "
                         + "    INNER JOIN dbo.[StalyKlient] sk ON sk.AdresId = a.Id "
                         + "WHERE u.MiejscowoscId = @miejscowoscId "
                         + "    AND sk.KlientId = @customerId;";
            var result = connection.GetTable(query, parameters);

            if (result == null)
                return list;

            foreach (DataRow row in result.Rows)
            {
                var ulica = new Ulica()
                {
                    Id = (int)row["Id"],
                    Name = row["Nazwa"].ToString(),
                    NameWithoutPl = row["NazwaBezPl"].ToString(),
                    Miejscowosc = row["Miejscowosc"].ToString(),
                    MiejscowoscId = miejscowoscId,
                };

                list.Add(ulica);
            }

            return list;
        }

        public List<Company> GetAllCompanies()
        {
            List<Company> list = new List<Company>();
            
            string query = "SELECT f.Id, f.Nazwa "
                         + "    , a.Id as AdresId, a.GminaId, g.Nazwa as GminaNazwa, a.MiejscowoscId, m.Nazwa as MiejscowoscNazwa, a.UlicaId, u.Nazwa as UlicaNazwa "
                         + "    , a.Numer, a.NumerUmowy "
                         + "FROM Firma f "
                         + "    INNER JOIN FirmaAdres fa ON f.Id = fa.FirmaId "
                         + "    INNER JOIN Adres a ON fa.AdresId = a.Id "
                         + "    INNER JOIN Ulica u ON a.UlicaId = u.Id "
                         + "    INNER JOIN Miejscowosc m ON a.MiejscowoscId = m.Id "
                         + "    INNER JOIN Gmina g ON a.GminaId = g.Id";
            var result = connection.GetTable(query);

            if (result == null)
                return list;

            foreach (DataRow row in result.Rows)
            {
                var customerAddress = new CustomerAddress()
                {
                    Id = (int)row["Id"],
                    GminaId = (int)row["GminaId"],
                    GminaName = row["GminaNazwa"].ToString(),
                    MiejscowoscId = (int)row["MiejscowoscId"],
                    MiejscowoscName = row["MiejscowoscNazwa"].ToString(),
                    UlicaId = (int)row["UlicaId"],
                    UlicaName = row["UlicaNazwa"].ToString(),
                    AddressNumber = row["Numer"].ToString(),
                    ContractNo = row["NumerUmowy"].ToString(),
                };
                var company = new Company()
                {
                    Id = (int)row["Id"],
                    Name = row["Nazwa"].ToString(),
                    Address = customerAddress,
                };

                list.Add(company);
            }

            return list;
        }
        public List<Company> GetAllRegularCompanies(int customerId)
        {
            List<Company> list = new List<Company>();

            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@customerId", customerId));
            string query = "SELECT f.Id, f.Nazwa "
                         + "    , a.Id as AdresId, a.GminaId, g.Nazwa as GminaNazwa, a.MiejscowoscId, m.Nazwa as MiejscowoscNazwa, a.UlicaId, u.Nazwa as UlicaNazwa "
                         + "    , a.Numer, a.NumerUmowy "
                         + "FROM Firma f "
                         + "    INNER JOIN FirmaAdres fa ON f.Id = fa.FirmaId "
                         + "    INNER JOIN Adres a ON fa.AdresId = a.Id "
                         + "    LEFT JOIN Ulica u ON a.UlicaId = u.Id "
                         + "    INNER JOIN Miejscowosc m ON a.MiejscowoscId = m.Id "
                         + "    INNER JOIN Gmina g ON a.GminaId = g.Id " 
                         + "    INNER JOIN StalyKlient sk ON sk.AdresId = a.Id AND sk.FirmaId = f.Id AND sk.KlientId = @customerId;";
            var result = connection.GetTable(query, parameters);

            if (result == null)
                return list;

            foreach (DataRow row in result.Rows)
            {
                var customerAddress = new CustomerAddress()
                {
                    Id = (int)row["Id"],
                    GminaId = (int)row["GminaId"],
                    GminaName = row["GminaNazwa"].ToString(),
                    MiejscowoscId = (int)row["MiejscowoscId"],
                    MiejscowoscName = row["MiejscowoscNazwa"].ToString(),
                    AddressNumber = row["Numer"].ToString(),
                    ContractNo = row["NumerUmowy"].ToString(),
                };
                if (!(row["UlicaId"] is DBNull))
                {
                    customerAddress.UlicaId = (int)row["UlicaId"];
                    customerAddress.UlicaName = row["UlicaNazwa"].ToString();
                }
                var company = new Company()
                {
                    Id = (int)row["Id"],
                    Name = row["Nazwa"].ToString(),
                    Address = customerAddress,
                };

                list.Add(company);
            }

            return list;
        }
        public List<Company> GetAllCompanies(CustomerAddress address)
        {
            List<Company> list = new List<Company>();

            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@gminaId", address.GminaId));
            parameters.Add(new SqlParameter("@miejscowoscId", address.MiejscowoscId));
            parameters.Add(new SqlParameter("@ulicaId", address.UlicaId));
            parameters.Add(new SqlParameter("@numer", address.AddressNumber));
            string query = "SELECT f.Id, f.Nazwa "
                         + "    , a.Id as AdresId, a.GminaId, g.Nazwa as GminaNazwa, a.MiejscowoscId, m.Nazwa as MiejscowoscNazwa, a.UlicaId, u.Nazwa as UlicaNazwa "
                         + "    , a.Numer, a.NumerUmowy "
                         + "FROM Firma f "
                         + "    INNER JOIN FirmaAdres fa ON f.Id = fa.FirmaId "
                         + "    INNER JOIN Adres a ON fa.AdresId = a.Id "
                         + "    INNER JOIN Ulica u ON a.UlicaId = u.Id "
                         + "    INNER JOIN Miejscowosc m ON a.MiejscowoscId = m.Id "
                         + "    INNER JOIN Gmina g ON a.GminaId = g.Id " 
                         + "WHERE a.GminaId = @gminaId "
                         + "    AND a.MiejscowoscId = @miejscowoscId "
                         + "    AND a.UlicaId = @ulicaId "
                         + "    AND a.Numer = @numer";
            var result = connection.GetTable(query, parameters);

            if (result == null)
                return list;

            foreach (DataRow row in result.Rows)
            {
                var customerAddress = new CustomerAddress()
                {
                    Id = (int)row["Id"],
                    GminaId = (int)row["GminaId"],
                    GminaName = row["GminaNazwa"].ToString(),
                    MiejscowoscId = (int)row["MiejscowoscId"],
                    MiejscowoscName = row["MiejscowoscNazwa"].ToString(),
                    UlicaId = (int)row["UlicaId"],
                    UlicaName = row["UlicaNazwa"].ToString(),
                    AddressNumber = row["Numer"].ToString(),
                    ContractNo = row["NumerUmowy"].ToString(),
                };
                var company = new Company()
                {
                    Id = (int)row["Id"],
                    Name = row["Nazwa"].ToString(),
                    Address = customerAddress,
                };

                list.Add(company);
            }

            return list;
        }
        public List<Company> GetAllRegularCompanies(CustomerAddress address, int customerId)
        {
            List<Company> list = new List<Company>();

            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@customerId", customerId));
            parameters.Add(new SqlParameter("@gminaId", address.GminaId));
            parameters.Add(new SqlParameter("@miejscowoscId", address.MiejscowoscId));
            parameters.Add(new SqlParameter("@ulicaId", address.UlicaId));
            parameters.Add(new SqlParameter("@numer", address.AddressNumber));
            string query = "SELECT f.Id, f.Nazwa "
                         + "    , a.Id as AdresId, a.GminaId, g.Nazwa as GminaNazwa, a.MiejscowoscId, m.Nazwa as MiejscowoscNazwa, a.UlicaId, u.Nazwa as UlicaNazwa "
                         + "    , a.Numer, a.NumerUmowy "
                         + "FROM Firma f "
                         + "    INNER JOIN FirmaAdres fa ON f.Id = fa.FirmaId "
                         + "    INNER JOIN Adres a ON fa.AdresId = a.Id "
                         + "    LEFT JOIN Ulica u ON a.UlicaId = u.Id "
                         + "    INNER JOIN Miejscowosc m ON a.MiejscowoscId = m.Id "
                         + "    INNER JOIN Gmina g ON a.GminaId = g.Id "
                         + "    INNER JOIN StalyKlient sk ON sk.AdresId = a.Id AND sk.FirmaId = f.Id AND sk.KlientId = @customerId "
                         + "WHERE a.GminaId = @gminaId "
                         + "    AND a.MiejscowoscId = @miejscowoscId "
                         + "    AND ISNULL(a.UlicaId, 0) = @ulicaId "
                         + "    AND a.Numer = @numer";
            var result = connection.GetTable(query, parameters);

            if (result == null)
                return list;

            foreach (DataRow row in result.Rows)
            {
                var customerAddress = new CustomerAddress()
                {
                    Id = (int)row["Id"],
                    GminaId = (int)row["GminaId"],
                    GminaName = row["GminaNazwa"].ToString(),
                    MiejscowoscId = (int)row["MiejscowoscId"],
                    MiejscowoscName = row["MiejscowoscNazwa"].ToString(),
                    AddressNumber = row["Numer"].ToString(),
                    ContractNo = row["NumerUmowy"].ToString(),
                };
                if (row["UlicaId"] is DBNull)
                {
                    customerAddress.UlicaId = 0;
                    customerAddress.UlicaName = "";
                }
                else
                {
                    customerAddress.UlicaId = (int)row["UlicaId"];
                    customerAddress.UlicaName = row["UlicaNazwa"].ToString();
                }
                var company = new Company()
                {
                    Id = (int)row["Id"],
                    Name = row["Nazwa"].ToString(),
                    Address = customerAddress,
                };

                list.Add(company);
            }

            return list;
        }

        public List<Rod> GetAllRod()
        {
            List<Rod> list = new List<Rod>();
            string query = "SELECT Id, Nazwa FROM Rod;";
            var result = connection.GetTable(query);

            if (result == null)
                return list;

            foreach (DataRow row in result.Rows)
            {
                var rod = new Rod()
                {
                    Id = (int)row["Id"],
                    Name = row["Nazwa"].ToString(),
                };

                list.Add(rod);
            }

            return list;
        }

        public int GetPrintedReciptsLenght()
        {
            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@kioskId", connection.KioskId));
            string query = "SELECT p.PrintedReciptLenghtInMilimeters "
                         + "FROM dbo.[PrinterPaperStatus] p "
                         + "WHERE p.KioskId = @kioskId;";
            var result = connection.GetScalar(query, parameters);

            if (result == null)
                return 0;

            return (int)result;
        }

        public HarmonogramedSample CheckHarmonogramedSample(RfidCard card, CargoType cargoType, CustomerAddress customerAddress)
        {
            HarmonogramedSample sample;
            sample = CheckHarmonogramedSampleByOrder();
            if (sample != null) return sample;

            sample = CheckHarmonogramedSampleBySewageType(cargoType);
            if (sample != null) return sample;

            sample = CheckHarmonogramedSampleByCompany(card);
            if (sample != null) return sample;

            //sample = CheckHarmonogramedSampleByCompanyAndSewageType(card, cargoType);
            //if (sample != null) return sample;

            sample = CheckHarmonogramedSampleByCustomerAddress(customerAddress);
            if (sample != null) return sample;

            //sample = CheckHarmonogramedSampleByParameters();
            //if (sample != null) return sample;

            //sample = CheckHarmonogramedSampleByTimeStamp(card);
            //if (sample != null) return sample;

            return null;
        }
        private HarmonogramedSample CheckHarmonogramedSampleByOrder()
        {
            var harmonogramType = HarmonogramType.DumpOrder;

            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@typPoboruId", harmonogramType));
            parameters.Add(new SqlParameter("@data", DateTime.Today));
            parameters.Add(new SqlParameter("@kioskId", connection.KioskId));
            string query = "SELECT h.Id "
                         + "FROM dbo.[HarmonogramPoboruProb] h LEFT JOIN dbo.[HarmonogramDostawca] hd ON h.Id = hd.HarmonogramId "
                         + "WHERE h.KioskId = @kioskId AND TypPoboruId = @typPoboruId AND Data = @data "
                         + "    AND h.LiczbaPoborow > (SELECT COUNT(*) FROM ProbaButelka pb WHERE cast(floor(cast(pb.ProbaData as float)) as datetime) = h.Data AND pb.ProbaTypId = 1) "
                         + "    AND hd.Id IS NULL;";
            var result = connection.GetTable(query, parameters);

            if (result == null || result.Rows.Count == 0)
                return null;

            var harmonogramedSample = new HarmonogramedSample()
            {
                Id = (int)result.Rows[0]["Id"],
                TakeSample = true,
                Type = harmonogramType
            };

            return harmonogramedSample;
        }
        private HarmonogramedSample CheckHarmonogramedSampleBySewageType(CargoType cargoType)
        {
            var harmonogramType = HarmonogramType.SewageType;

            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@typPoboruId", harmonogramType));
            parameters.Add(new SqlParameter("@data", DateTime.Today));
            parameters.Add(new SqlParameter("@kioskId", connection.KioskId));
            parameters.Add(new SqlParameter("@typSciekowId", cargoType.Id));
            string query = "SELECT h.Id "
                         + "FROM dbo.[HarmonogramPoboruProb] h "
                         + "WHERE h.KioskId = @kioskId AND TypPoboruId = @typPoboruId AND Data = @data "
                         + "    AND TypSciekowId = @typSciekowId;";
            var result = connection.GetTable(query, parameters);

            if (result == null || result.Rows.Count == 0)
                return null;

            var harmonogramedSample = new HarmonogramedSample()
            {
                Id = (int)result.Rows[0]["Id"],
                TakeSample = true,
                Type = harmonogramType
            };

            return harmonogramedSample;
        }
        private HarmonogramedSample CheckHarmonogramedSampleByCompany(RfidCard card)
        {
            var harmonogramType = HarmonogramType.DumpOrder;

            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@typPoboruId", (int)harmonogramType));
            parameters.Add(new SqlParameter("@data", DateTime.Today));
            parameters.Add(new SqlParameter("@kioskId", connection.KioskId));
            parameters.Add(new SqlParameter("@KlientId", card.Customer.Id));
            string query = "SELECT h.Id "
                         + "FROM dbo.[HarmonogramPoboruProb] h INNER JOIN dbo.[HarmonogramDostawca] hd ON h.Id = hd.HarmonogramId "
                         + "WHERE hd.Deleted = 0 "
                         + "    AND h.KioskId = @kioskId AND TypPoboruId = @typPoboruId AND Data = @data "
                         + "    AND hd.DostawcaId = @klientId "
                         + "    AND ISNULL(h.LiczbaPoborow, 1000) > (SELECT COUNT(*) FROM ProbaButelka pb WHERE cast(floor(cast(pb.ProbaData as float)) as datetime) = h.Data AND pb.ProbaTypId = 1);";
            var result = connection.GetTable(query, parameters);

            if (result == null || result.Rows.Count == 0)
                return null;

            var harmonogramedSample = new HarmonogramedSample()
            {
                Id = (int)result.Rows[0]["Id"],
                TakeSample = true,
                Type = harmonogramType
            };

            return harmonogramedSample;
        }
        //private HarmonogramedSample CheckHarmonogramedSampleByCompanyAndSewageType(RfidCard card, CargoType cargoType)
        //{
        //    var harmonogramType = HarmonogramType.CompanyAndSewageType;

        //    var parameters = new List<SqlParameter>();
        //    parameters.Add(new SqlParameter("@typPoboruId", harmonogramType));
        //    parameters.Add(new SqlParameter("@date", DateTime.Now));
        //    parameters.Add(new SqlParameter("@kioskId", connection.KioskId));
        //    parameters.Add(new SqlParameter("@KlientId", card.Customer.Id));
        //    parameters.Add(new SqlParameter("@typSciekowId", cargoType.Id));
        //    string query = "SELECT h.Id "
        //                 + "FROM dbo.[HarmonogramPoboruProb] h "
        //                 + "WHERE h.KioskId = @kioskId AND TypPoboruId = @typPoboruId AND Data = @data "
        //                 + "    AND TypSciekowId = @typSciekowId AND KlientId = @klientId;";
        //    var result = connection.GetTable(query, parameters);

        //    if (result == null || result.Rows.Count == 0)
        //        return null;

        //    var harmonogramedSample = new HarmonogramedSample()
        //    {
        //        Id = (int)result.Rows[0]["Id"],
        //        TakeSample = true,
        //        Type = harmonogramType
        //    };

        //    return harmonogramedSample;
        //}
        private HarmonogramedSample CheckHarmonogramedSampleByCustomerAddress(CustomerAddress customerAddress)
        {
            var harmonogramType = HarmonogramType.CustomerAddress;
            if (customerAddress.Id == null)
            {
                return null;
            }

            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@typPoboruId", harmonogramType));
            parameters.Add(new SqlParameter("@data", DateTime.Today));
            parameters.Add(new SqlParameter("@kioskId", connection.KioskId));
            parameters.Add(new SqlParameter("@adresGminaId", customerAddress.GminaId));
            parameters.Add(new SqlParameter("@adresMiejscowoscId", customerAddress.MiejscowoscId));
            parameters.Add(new SqlParameter("@adresUlicaId", customerAddress.UlicaId));
            parameters.Add(new SqlParameter("@adresNumer", customerAddress.AddressNumber));
            string query = "SELECT h.Id "
                         + "FROM dbo.[HarmonogramPoboruProb] h "
                         + "WHERE h.KioskId = @kioskId AND TypPoboruId = @typPoboruId AND Data = @data "
                         + "    AND AdresGminaId = @adresGminaId "
                         + "    AND AdresMiejscowoscId = @adresMiejscowoscId "
                         + "    AND ISNULL(AdresUlicaId, 0) = @adresUlicaId "
                         + "    AND AdresNumer = @adresNumer;";
            var result = connection.GetTable(query, parameters);

            if (result == null || result.Rows.Count == 0)
                return null;

            var harmonogramedSample = new HarmonogramedSample()
            {
                Id = (int)result.Rows[0]["Id"],
                TakeSample = true,
                Type = harmonogramType
            };

            return harmonogramedSample;
        }
        //private HarmonogramedSample CheckHarmonogramedSampleByTimeStamp(RfidCard card)
        //{
        //    var harmonogramType = HarmonogramType.TimeStamp;

        //    var parameters = new List<SqlParameter>();
        //    parameters.Add(new SqlParameter("@typPoboruId", harmonogramType));
        //    parameters.Add(new SqlParameter("@date", DateTime.Now));
        //    parameters.Add(new SqlParameter("@kioskId", connection.KioskId));
        //    parameters.Add(new SqlParameter("@KlientId", card.Customer.Id));
        //    string query = "SELECT h.Id "
        //                 + "FROM dbo.[HarmonogramPoboruProb] h "
        //                 + "WHERE h.KioskId = @kioskId AND TypPoboruId = @typPoboruId AND Data = @data "
        //                 + "    AND ISNULL(KlientId, @klientId) = @klientId AND @date BETWEEN DataStart AND DataKoniec;";
        //    var result = connection.GetTable(query, parameters);

        //    if (result == null || result.Rows.Count == 0)
        //        return null;

        //    var harmonogramedSample = new HarmonogramedSample()
        //    {
        //        Id = (int)result.Rows[0]["Id"],
        //        TakeSample = true,
        //        Type = harmonogramType
        //    };

        //    return harmonogramedSample;
        //}

        public SampleTrayInfo GetStampleTrayInfo()
        {
            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@kioskId", connection.KioskId));
            string query = "SELECT pt.Id as TackaId "
                         + "    , TackaNr " 
                         + "    , (SELECT COUNT(*) FROM ProbaButelka pb WHERE pt.Id = pb.TackaId AND pb.ProbaTypId = 1) AS LiczbaProbHarm "
                         + "    , (SELECT COUNT(*) FROM ProbaButelka pb WHERE pt.Id = pb.TackaId AND pb.ProbaTypId = 2) AS LiczbaProbAlrm "
                         + "FROM[dbo].[ProbaTacka] pt "
                         + "WHERE pt.KioskId = @kioskId AND pt.DataKoniec IS NULL;";
            var result = connection.GetTable(query, parameters);

            if (result == null || result.Rows.Count == 0)
                return null;

            var sampleTrayInfo = new SampleTrayInfo()
            {
                Id = (int)result.Rows[0]["TackaId"],
                TrayNo = (int)result.Rows[0]["TackaNr"],
                NoOfScheduledSamples = (int)result.Rows[0]["LiczbaProbHarm"],
                NoOfAlarmSamples = (int)result.Rows[0]["LiczbaProbAlrm"]
            };

            return sampleTrayInfo;
        }
        public int ResetSampleTray(SampleTrayInfo sampleTrayInfo)
        {
            var parametersRecipt = new List<SqlParameter>();
            parametersRecipt.Add(new SqlParameter("@kioskId", connection.KioskId));
            parametersRecipt.Add(new SqlParameter("@tackaId", sampleTrayInfo.Id));
            parametersRecipt.Add(new SqlParameter("@liczbaHarm", sampleTrayInfo.NoOfScheduledSamples));
            parametersRecipt.Add(new SqlParameter("@liczbaAlrm", sampleTrayInfo.NoOfAlarmSamples));
            string queryRecipt = "EXECUTE [_sp_add_sampler_tray_recipt] @kioskId, @tackaId, @liczbaHarm, @liczbaAlrm;";
            var insertedReciptId = connection.InsertRow(queryRecipt, parametersRecipt);

            if (insertedReciptId == 0)
            {
                return 0;
            }

            var parametersTray = new List<SqlParameter>();
            parametersTray.Add(new SqlParameter("@kioskId", connection.KioskId));
            parametersTray.Add(new SqlParameter("@tackaId", sampleTrayInfo.Id));
            string queryTray = "UPDATE ProbaTacka "
                             + "SET DataKoniec = GETDATE() " 
                             + "WHERE KioskId = @kioskId AND Id = @tackaId;";
            var updatedRows = connection.UpdateRow(queryTray, parametersTray);

            if (updatedRows == 0)
            {
                return 0;
            }

            var parametersNewTray = new List<SqlParameter>();
            parametersNewTray.Add(new SqlParameter("@kioskId", connection.KioskId));
            parametersNewTray.Add(new SqlParameter("@tackaNr", sampleTrayInfo.TrayNo == 1 ? 2 : 1));
            string queryNewTray = "EXECUTE [_sp_add_sampler_tray] @kioskId, @tackaNr;";
            var insertedTray = connection.InsertRow(queryNewTray, parametersNewTray);

            if (insertedTray == 0)
            {
                return 0;
            }

            return sampleTrayInfo.Id;
        }
        public SampleTray GetSampleTray(int trayId)
        {
            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@kioskId", connection.KioskId));
            parameters.Add(new SqlParameter("@tackaId", trayId));
            string query = "SELECT pt.Id as TackaId, pt.TackaNr, pt.DataStart as TackaStart, pt.DataKoniec as TackaKoniec "
                         + "    , pb.Id as PoborButelkaId, pb.NumerButelki as NumerButelki, pb.ProbaData as DataPoboru, typ.Nazwa as TypProby "
                         + "	, ki.Nazwa as KioskNazwa, l.Nazwa as LokalizacjaNazwa "
                         + "    , t.Id as TransakcjaId, rs.Nazwa as RodzajSciekow, s.Rejestracja as SamochodRejestracja, k.KartaId "
                         + "    , kl.Nazwa as KlientNazwa, kl.AdresLinia1 as KlientAdres1, kl.AdresLinia2 as KlientAdres2, kl.KodPocztowy as KlientKodPocztowy, kl.Miejscowosc as KlientMiejscowosc "
                         + "FROM[dbo].[ProbaTacka] pt "
                         + "    LEFT JOIN ProbaButelka pb ON pt.Id = pb.TackaId "
                         + "    LEFT JOIN ProbaTyp typ ON typ.Id = pb.ProbaTypId "
                         + "    INNER JOIN Kiosk ki ON ki.Id = pt.KioskId "
                         + "    INNER JOIN Lokalizacja l ON l.Id = ki.LokalizacjaId "
                         + "    LEFT JOIN Transakcja t ON t.Id = pb.ProbaTransakcjaId "
                         + "    LEFT JOIN RodzajSciekow rs ON rs.Id = t.RodzajSciekowId "
                         + "    LEFT JOIN Samochod s ON s.Id = t.SamochodId "
                         + "    LEFT JOIN Karta k ON k.Id = t.KartaId "
                         + "    LEFT JOIN Klient kl ON kl.Id = s.KlientId "
                         + "WHERE pt.KioskId = @kioskId AND pt.Id = @tackaId;";
            var result = connection.GetTable(query, parameters);

            if (result == null || result.Rows.Count == 0)
                return null;

            var sampleTray = new SampleTray()
            {
                TrayId = (int)result.Rows[0]["TackaId"],
                TrayNo = (int)result.Rows[0]["TackaNr"],
                TrayStartDate = (DateTime)result.Rows[0]["TackaStart"],
                TrayEndDate = (DateTime)result.Rows[0]["TackaKoniec"], 

                KioskName = result.Rows[0]["KioskNazwa"].ToString(),
                LocationName = result.Rows[0]["LokalizacjaNazwa"].ToString(),

                Bottles = new List<SampleBottle>()
            };

            if (!(result.Rows[0]["TransakcjaId"] is DBNull))
            {
                foreach (DataRow dr in result.Rows)
                {
                    var transactionId = (int)dr["TransakcjaId"];
                    sampleTray.Bottles.Add(
                        new SampleBottle()
                        {
                            BottleId = (int)dr["PoborButelkaId"],
                            BottleNo = (int)dr["NumerButelki"],
                            ProbeTakenTime = (DateTime)dr["DataPoboru"],
                            ProbeType = dr["TypProby"].ToString(),
                            TransactionId = transactionId,
                            SampleType = dr["RodzajSciekow"].ToString(),
                            VechicleRegistration = dr["SamochodRejestracja"].ToString(),
                            RfidCardId = dr["KartaId"].ToString(),

                            CustomerName = dr["KlientNazwa"].ToString(),
                            Addr1 = dr["KlientAdres1"].ToString(),
                            Addr2 = dr["KlientAdres2"].ToString(),
                            PostCode = dr["KlientKodPocztowy"].ToString(),
                            City = dr["KlientMiejscowosc"].ToString(),

                            Addresses = GetTransactionAddresses(transactionId)
                        });
                }
            }

            return sampleTray;
        }
        public void MarkTrayReciptAsPrinted(int trayId)
        {
            var parametersTray = new List<SqlParameter>();
            parametersTray.Add(new SqlParameter("@kioskId", connection.KioskId));
            parametersTray.Add(new SqlParameter("@tackaId", trayId));
            string queryTray = "UPDATE ProbaTackaParagon "
                             + "SET Wydrukowano = 1, DataWydruku = GETDATE() "
                             + "WHERE KioskId = @kioskId AND TackaId = @tackaId;";
            var updatedRows = connection.UpdateRow(queryTray, parametersTray);
        }

        public List<CustomerAddress> GetTransactionAddresses(int transactionId)
        {
            var addresses = new List<CustomerAddress>();
            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@transakcjaId", transactionId));
            string query = "SELECT ta.Id, f.Id AS FirmaId, f.Nazwa AS FirmaNazwa"
                         + "	, ag.Id as GminaId, ag.Nazwa as GminaNazwa "
                         + "	, am.Id as MiejscowoscId, am.Nazwa as MiejscowoscNazwa "
                         + "    , au.Id as UlicaId, au.Nazwa as UlicaNazwa "
                         + "    , r.Id as RodId, r.Nazwa as RodNazwa "
                         + "    , ISNULL(ta.RodNrDzialki, a.Numer) AS Numer, ta.NumerUmowy, ta.ZadeklarowanaIlosc "
                         + "FROM TransakcjaAdres ta "
                         + "    LEFT JOIN Adres a ON a.Id = ta.AdresId "
                         + "    LEFT JOIN Gmina ag ON ag.Id = a.GminaId "
                         + "    LEFT JOIN Miejscowosc am ON am.Id = a.MiejscowoscId "
                         + "    LEFT JOIN Ulica au ON au.Id = a.UlicaId "
                         + "    LEFT JOIN Firma f ON f.id = ta.FirmaId "
                         + "    LEFT JOIN Rod r ON r.Id = ta.RodId "
                         + "WHERE TransakcjaId = @transakcjaId; ";
            var result = connection.GetTable(query, parameters);

            if (result == null || result.Rows.Count == 0)
                return addresses;

            foreach (DataRow dr in result.Rows)
            {
                var address = new CustomerAddress()
                {
                    Id = (int)dr["Id"]
                };

                if (!(dr["RodId"] is DBNull))
                {
                    address.RodId = (int)dr["RodId"];
                    address.RodName = dr["RodNazwa"].ToString();
                    address.AddressNumber = dr["Numer"].ToString();
                }
                else
                {
                    if (!(dr["GminaId"] is DBNull))
                    {
                        address.GminaId = (int)dr["GminaId"];
                        address.GminaName = dr["GminaNazwa"].ToString();
                    }
                    if (!(dr["MiejscowoscId"] is DBNull))
                    {
                        address.MiejscowoscId = (int)dr["MiejscowoscId"];
                        address.MiejscowoscName = dr["MiejscowoscNazwa"].ToString();
                    }
                    if (!(dr["UlicaId"] is DBNull))
                    {
                        address.UlicaId = (int)dr["UlicaId"];
                        address.UlicaName = dr["UlicaNazwa"].ToString();
                    }
                    address.AddressNumber = dr["Numer"].ToString();
                    address.ContractNo = dr["NumerUmowy"].ToString();
                    address.DeclaredAmount = (decimal)dr["ZadeklarowanaIlosc"];

                    if (!(dr["FirmaId"] is DBNull))
                    {
                        var company = new Company()
                        {
                            Id = (int)dr["FirmaId"],
                            Name = dr["FirmaNazwa"].ToString(),
                            Address = address
                        };
                        address.Company = company;
                    }
                }

                addresses.Add(address);
            }

            return addresses;
        }
    }
}
 