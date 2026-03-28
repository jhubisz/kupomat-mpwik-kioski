using DbCommunication.Entities;
using DbCommunication.Entities.DbRows;
using DbCommunication.Enums;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace DbCommunication
{
    public class DbServerRead
    {
        private IDbConnection connection;

        private DbServerRead() { }
        public DbServerRead(IDbConnection conn)
        {
            connection = conn;
        }

        public List<KioskWiadomoscDbRow> GetKioskWiadomoscDbRowsToExport()
        {
            List<KioskWiadomoscDbRow> list = new List<KioskWiadomoscDbRow>();

            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@rowTypeId", (int)ExportRowType.KioskWiadomosc));
            parameters.Add(new SqlParameter("@kioskId", connection.KioskId));
            string query = "SELECT kw.Id, kw.KioskId, kw.Wiadomosc, kw.DataStart, kw.DataKoniec "
                         + "    , er.Id as ExportRowId, er.RowActionId as ExportActionId "
                         + "FROM dbo.[KioskWiadomosc] kw "
                         + "    INNER JOIN dbo.[ExportRow] er ON kw.Id = er.RowId AND er.RowTypeId = @rowTypeId AND er.kioskId = @kioskId "
                         + "WHERE NOT EXISTS (SELECT 1 FROM dbo.ExportRowExported ee WHERE ee.ExportRowId = er.Id AND ee.KioskId = @kioskId);";
            var result = connection.GetTable(query, parameters);

            if (result == null)
                return list;

            foreach (DataRow row in result.Rows)
            {
                var kioskWiadomosc = new KioskWiadomoscDbRow()
                {
                    ExportRowId = (int)row["ExportRowId"],
                    ExportRowType = ExportRowType.Customer,
                    ExportAction = (ExportAction)row["ExportActionId"],

                    Id = (int)row["Id"],
                    KioskId = (int)row["KioskId"],
                    Wiadomosc = row["Wiadomosc"].ToString(),
                    
                    DataStart = (DateTime)row["DataStart"],
                };

                if (row["DataKoniec"] is DBNull)
                    kioskWiadomosc.DataKoniec = null;
                else
                    kioskWiadomosc.DataKoniec = (DateTime)row["DataKoniec"];

                list.Add(kioskWiadomosc);
            }

            return list;
        }

        public List<KlientDbRow> GetCustomerDbRowsToExport()
        {
            List<KlientDbRow> list = new List<KlientDbRow>();

            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@rowTypeId", (int)ExportRowType.Customer));
            parameters.Add(new SqlParameter("@kioskId", connection.KioskId));
            string query = "SELECT c.Id, c.Nazwa, c.NIP, c.REGON, c.KodPocztowy, c.Miejscowosc, c.AdresLinia1, c.AdresLinia2, c.Tel1, c.Tel2, c.NumerUmowy, c.PobieranieProbek, c.Blokada, c.DataStart, c.DataKoniec, c.Deleted "
                         + "    , er.Id as ExportRowId, er.RowActionId as ExportActionId "
                         + "FROM dbo.[Klient] c "
                         + "    INNER JOIN dbo.[ExportRow] er ON c.Id = er.RowId AND er.RowTypeId = @rowTypeId AND er.kioskId = @kioskId "
                         + "WHERE NOT EXISTS (SELECT 1 FROM dbo.ExportRowExported ee WHERE ee.ExportRowId = er.Id AND ee.KioskId = @kioskId);";
            var result = connection.GetTable(query, parameters);

            if (result == null)
                return list;

            foreach (DataRow row in result.Rows)
            {
                var customer = new KlientDbRow()
                {
                    ExportRowId = (int)row["ExportRowId"],
                    ExportRowType = ExportRowType.Customer,
                    ExportAction = (ExportAction)row["ExportActionId"],

                    Id = (int)row["Id"],
                    Nazwa = row["Nazwa"].ToString(),
                    Nip = row["NIP"].ToString(),
                    Regon = row["REGON"].ToString(),
                    KodPocztowy = row["KodPocztowy"].ToString(),
                    Miejscowosc = row["Miejscowosc"].ToString(),
                    AdresLinia1 = row["AdresLinia1"].ToString(),
                    AdresLinia2 = row["AdresLinia2"].ToString(),
                    Tel1 = row["Tel1"].ToString(),
                    Tel2 = row["Tel2"].ToString(),
                    NumerUmowy = row["NumerUmowy"].ToString(),
                    PobieranieProbek = (bool)row["PobieranieProbek"],
                    Blokada = (bool)row["Blokada"],

                    DataStart = (DateTime)row["DataStart"],
                    Deleted = (bool)row["Deleted"],
                };

                if (row["DataKoniec"] is DBNull)
                    customer.DataKoniec = null;
                else
                    customer.DataKoniec = (DateTime)row["DataKoniec"];

                list.Add(customer);
            }

            return list;
        }

        public List<AdresDbRow> GetAddressDbRowsToExport()
        {
            List<AdresDbRow> list = new List<AdresDbRow>();

            //var parameters = new List<SqlParameter>();
            //parameters.Add(new SqlParameter("@rowTypeId", (int)ExportRowType.Address));
            //parameters.Add(new SqlParameter("@kioskId", 1));
            //string query = "SELECT a.Id, a.CustomerId, a.Name, a.NIP, a.REGON, a.PostCode, a.City, a.AddrLine1, a.AddrLine2, a.AddrLine3, a.Tel1, a.Tel2, a.ContractNo, a.DateStart, a.DateEnd, a.Deleted "
            //             + "    , er.Id as ExportRowId, er.RowActionId as ExportActionId "
            //             + "FROM dbo.[Address] a "
            //             + "    INNER JOIN dbo.[ExportRow] er ON a.Id = er.RowId AND er.RowTypeId = @rowTypeId "
            //             + "WHERE NOT EXISTS (SELECT 1 FROM dbo.ExportRowExported ee WHERE ee.ExportRowId = er.Id AND ee.KioskId = @kioskId);";
            //var result = connection.GetTable(query, parameters);

            //if (result == null)
            //    return list;

            //foreach (DataRow row in result.Rows)
            //{
            //    var customer = new AddressDbRow()
            //    {
            //        ExportRowId = (int)row["ExportRowId"],
            //        ExportRowType = ExportRowType.Customer,
            //        ExportAction = (ExportAction)row["ExportActionId"],

            //        Id = (int)row["Id"],
            //        CustomerId = (int)row["CustomerId"],
            //        Name = row["Name"].ToString(),
            //        Nip = row["NIP"].ToString(),
            //        Regon = row["REGON"].ToString(),
            //        PostCode = row["PostCode"].ToString(),
            //        City = row["City"].ToString(),
            //        AddressLine1 = row["AddrLine1"].ToString(),
            //        AddressLine2 = row["AddrLine2"].ToString(),
            //        AddressLine3 = row["AddrLine3"].ToString(),
            //        Tel1 = row["Tel1"].ToString(),
            //        Tel2 = row["Tel2"].ToString(),
            //        ContractNo = row["ContractNo"].ToString(),

            //        DateStart = (DateTime)row["DateStart"],
            //        Deleted = (bool)row["Deleted"],
            //    };

            //    if (row["DateEnd"] is DBNull)
            //        customer.DateEnd = null;
            //    else
            //        customer.DateEnd = (DateTime)row["DateEnd"];

            //    list.Add(customer);
            //}

            return list;
        }

        public List<SamochodDbRow> GetSamochodDbRowsToExport()
        {
            List<SamochodDbRow> list = new List<SamochodDbRow>();

            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@rowTypeId", (int)ExportRowType.Vechicle));
            parameters.Add(new SqlParameter("@kioskId", connection.KioskId));
            string query = "SELECT s.Id, s.KlientId, s.Rejestracja, s.DataStart, s.DataKoniec, s.Deleted "
                         + "    , er.Id as ExportRowId, er.RowActionId as ExportActionId "
                         + "FROM dbo.[Samochod] s "
                         + "    INNER JOIN dbo.[ExportRow] er ON s.Id = er.RowId AND er.RowTypeId = @rowTypeId AND er.KioskId = @kioskId "
                         + "WHERE NOT EXISTS (SELECT 1 FROM dbo.ExportRowExported ee WHERE ee.ExportRowId = er.Id AND ee.KioskId = @kioskId);";
            var result = connection.GetTable(query, parameters);

            if (result == null)
                return list;

            foreach (DataRow row in result.Rows)
            {
                var customer = new SamochodDbRow()
                {
                    ExportRowId = (int)row["ExportRowId"],
                    ExportRowType = ExportRowType.Customer,
                    ExportAction = (ExportAction)row["ExportActionId"],

                    Id = (int)row["Id"],
                    KlientId = (int)row["KlientId"],
                    Rejestracja = row["Rejestracja"].ToString(),

                    DataStart = (DateTime)row["DataStart"],
                    Deleted = (bool)row["Deleted"],
                };

                if (row["DataKoniec"] is DBNull)
                    customer.DataKoniec = null;
                else
                    customer.DataKoniec = (DateTime)row["DataKoniec"];

                list.Add(customer);
            }

            return list;
        }

        public List<KartaDbRow> GetKartaDbRowsToExport()
        {
            List<KartaDbRow> list = new List<KartaDbRow>();

            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@rowTypeId", (int)ExportRowType.RfidCard));
            parameters.Add(new SqlParameter("@kioskId", connection.KioskId));
            string query = "SELECT k.Id, k.KartaId, k.SamochodId, k.KartaWymianyTacki, k.Blokada, k.DataStart, k.DataKoniec, k.Deleted "
                         + "    , er.Id as ExportRowId, er.RowActionId as ExportActionId "
                         + "FROM dbo.[Karta] k "
                         + "    INNER JOIN dbo.[ExportRow] er ON k.Id = er.RowId AND er.RowTypeId = @rowTypeId AND er.KioskId = @kioskId "
                         + "WHERE NOT EXISTS (SELECT 1 FROM dbo.ExportRowExported ee WHERE ee.ExportRowId = er.Id AND ee.KioskId = @kioskId);";
            var result = connection.GetTable(query, parameters);

            if (result == null)
                return list;

            foreach (DataRow row in result.Rows)
            {
                var card = new KartaDbRow()
                {
                    ExportRowId = (int)row["ExportRowId"],
                    ExportRowType = ExportRowType.Customer,
                    ExportAction = (ExportAction)row["ExportActionId"],

                    Id = (int)row["Id"],
                    KartaId = row["KartaId"].ToString(),
                    KartaWymianyTacki = (bool)row["KartaWymianyTacki"],
                    
                    DataStart = (DateTime)row["DataStart"],
                    Deleted = (bool)row["Deleted"],
                };

                if (row["SamochodId"] is DBNull)
                    card.SamochodId = null;
                else
                    card.SamochodId = (int)row["SamochodId"];
                if (row["Blokada"] is DBNull)
                    card.Blokada = null;
                else
                    card.Blokada = (bool)row["Blokada"];
                if (row["DataKoniec"] is DBNull)
                    card.DataKoniec = null;
                else
                    card.DataKoniec = (DateTime)row["DataKoniec"];
                
                list.Add(card);
            }

            return list;
        }

        public List<LicencjaDbRow> GetLicencjaDbRowsToExport()
        {
            List<LicencjaDbRow> list = new List<LicencjaDbRow>();

            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@rowTypeId", (int)ExportRowType.Licencja));
            parameters.Add(new SqlParameter("@kioskId", connection.KioskId));
            string query = "SELECT l.Id, l.KlientId, l.LokalizacjaId, l.LicencjaNumer, l.DataStart, l.DataKoniec, l.Deleted "
                         + "    , er.Id as ExportRowId, er.RowActionId as ExportActionId "
                         + "FROM dbo.[Licencja] l "
                         + "    INNER JOIN dbo.[ExportRow] er ON l.Id = er.RowId AND er.RowTypeId = @rowTypeId AND er.KioskId = @kioskId "
                         + "WHERE NOT EXISTS (SELECT 1 FROM dbo.ExportRowExported ee WHERE ee.ExportRowId = er.Id AND ee.KioskId = @kioskId);";
            var result = connection.GetTable(query, parameters);

            if (result == null)
                return list;

            foreach (DataRow row in result.Rows)
            {
                var card = new LicencjaDbRow()
                {
                    ExportRowId = (int)row["ExportRowId"],
                    ExportRowType = ExportRowType.Customer,
                    ExportAction = (ExportAction)row["ExportActionId"],

                    Id = (int)row["Id"],
                    KlientId = (int)row["KlientId"],
                    LokalizacjaId = (int)row["LokalizacjaId"],
                    LicencjaNumer = row["LicencjaNumer"].ToString(),

                    DataStart = (DateTime)row["DataStart"],
                    Deleted = (bool)row["Deleted"],
                };

                if (row["DataKoniec"] is DBNull)
                    card.DataKoniec = null;
                else
                    card.DataKoniec = (DateTime)row["DataKoniec"];

                list.Add(card);
            }

            return list;
        }

        public List<HarmonogramDbRow> GetHarmonogramDbRowsToExport()
        {
            List<HarmonogramDbRow> list = new List<HarmonogramDbRow>();

            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@rowTypeId", (int)ExportRowType.Harmonogram));
            parameters.Add(new SqlParameter("@kioskId", connection.KioskId));
            string query = "SELECT h.Id, h.KioskId, h.Data, h.TypPoboruId, h.TypSciekowId, h.AdresGminaId, h.AdresMiejscowoscId, h.AdresUlicaId, h.AdresNumer, h.LiczbaPoborow, h.DataStart, h.DataKoniec, h.DataDodania "
                         + "    , er.Id as ExportRowId, er.RowActionId as ExportActionId "
                         + "FROM dbo.[HarmonogramPoboruProb] h "
                         + "    INNER JOIN dbo.[ExportRow] er ON h.Id = er.RowId AND er.RowTypeId = @rowTypeId AND er.KioskId = @kioskId "
                         + "WHERE NOT EXISTS (SELECT 1 FROM dbo.ExportRowExported ee WHERE ee.ExportRowId = er.Id AND ee.KioskId = @kioskId);";
            var result = connection.GetTable(query, parameters);

            if (result == null)
                return list;

            foreach (DataRow row in result.Rows)
            {
                var card = new HarmonogramDbRow()
                {
                    ExportRowId = (int)row["ExportRowId"],
                    ExportRowType = ExportRowType.Customer,
                    ExportAction = (ExportAction)row["ExportActionId"],

                    Id = (int)row["Id"],
                    KioskId = (int)row["KioskId"],
                    Data = (DateTime)row["Data"],
                    TypPoboruId = (int)row["TypPoboruId"],

                    DataDodania = (DateTime)row["DataDodania"]
                };

                if (row["TypSciekowId"] is DBNull)
                    card.TypSciekowId = null;
                else
                    card.TypSciekowId = (int)row["TypSciekowId"];

                if (row["AdresGminaId"] is DBNull)
                    card.AdresGminaId = null;
                else
                    card.AdresGminaId = (int)row["AdresGminaId"];
                if (row["AdresMiejscowoscId"] is DBNull)
                    card.AdresMiejscowoscId = null;
                else
                    card.AdresMiejscowoscId = (int)row["AdresMiejscowoscId"];
                if (row["AdresUlicaId"] is DBNull)
                    card.AdresUlicaId = null;
                else
                    card.AdresUlicaId = (int)row["AdresUlicaId"];
                if (row["AdresNumer"] is DBNull)
                    card.AdresNumer = null;
                else
                    card.AdresNumer = row["AdresNumer"].ToString();

                if (row["LiczbaPoborow"] is DBNull)
                    card.LiczbaPoborow = null;
                else
                    card.LiczbaPoborow = (int)row["LiczbaPoborow"];
                if (row["DataStart"] is DBNull)
                    card.DataStart = null;
                else
                    card.DataStart = (DateTime)row["DataStart"];
                if (row["DataKoniec"] is DBNull)
                    card.DataKoniec = null;
                else
                    card.DataKoniec = (DateTime)row["DataKoniec"];

                list.Add(card);
            }

            return list;
        }
        public List<HarmonogramDostawcaDbRow> GetHarmonogramDostawcyDbRowsToExport()
        {
            List<HarmonogramDostawcaDbRow> list = new List<HarmonogramDostawcaDbRow>();

            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@rowTypeId", (int)ExportRowType.HarmonogramDostawca));
            parameters.Add(new SqlParameter("@kioskId", connection.KioskId));
            string query = "SELECT h.Id, h.HarmonogramId, h.DostawcaId, h.Deleted "
                         + "    , er.Id as ExportRowId, er.RowActionId as ExportActionId "
                         + "FROM dbo.[HarmonogramDostawca] h "
                         + "    INNER JOIN dbo.[ExportRow] er ON h.Id = er.RowId AND er.RowTypeId = @rowTypeId AND er.KioskId = @kioskId "
                         + "WHERE NOT EXISTS (SELECT 1 FROM dbo.ExportRowExported ee WHERE ee.ExportRowId = er.Id AND ee.KioskId = @kioskId);";
            var result = connection.GetTable(query, parameters);

            if (result == null)
                return list;

            foreach (DataRow row in result.Rows)
            {
                var dostawca = new HarmonogramDostawcaDbRow()
                {
                    ExportRowId = (int)row["ExportRowId"],
                    ExportRowType = ExportRowType.Customer,
                    ExportAction = (ExportAction)row["ExportActionId"],

                    Id = (int)row["Id"],
                    HarmonogramId = (int)row["HarmonogramId"],
                    DostawcaId = (int)row["DostawcaId"],

                    Deleted = (bool)row["Deleted"]
                };
                list.Add(dostawca);
            }

            return list;
        }

        public List<GminaDbRow> GetGminaDbRowsToExport()
        {
            List<GminaDbRow> list = new List<GminaDbRow>();

            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@rowTypeId", (int)ExportRowType.Gmina));
            parameters.Add(new SqlParameter("@kioskId", connection.KioskId));
            string query = "SELECT g.Id, g.KioskId, g.TerytId, g.TerytStanNa, g.PowiatId, g.Nazwa, g.RodzajId, g.DataStart, g.DataKoniec, g.Deleted "
                         + "    , er.Id as ExportRowId, er.RowActionId as ExportActionId "
                         + "FROM dbo.[Gmina] g "
                         + "    INNER JOIN dbo.[ExportRow] er ON g.Id = er.RowId AND er.RowTypeId = @rowTypeId AND er.KioskId = @kioskId "
                         + "WHERE NOT EXISTS (SELECT 1 FROM dbo.ExportRowExported ee WHERE ee.ExportRowId = er.Id AND ee.KioskId = @kioskId);";
            var result = connection.GetTable(query, parameters);

            if (result == null)
                return list;

            foreach (DataRow row in result.Rows)
            {
                var card = new GminaDbRow()
                {
                    ExportRowId = (int)row["ExportRowId"],
                    ExportRowType = ExportRowType.Customer,
                    ExportAction = (ExportAction)row["ExportActionId"],

                    Id = (int)row["Id"],
                    KioskId = (int)row["KioskId"],
                    PowiatId = (int)row["PowiatId"],
                    Nazwa = row["Nazwa"].ToString(),
                    RodzajId = (int)row["RodzajId"],

                    DataStart = (DateTime)row["DataStart"],
                    Deleted = (bool)row["Deleted"],
                };
                if (row["TerytId"] is DBNull)
                    card.TerytId = null;
                else
                    card.TerytId = (int)row["TerytId"];
                if (row["TerytStanNa"] is DBNull)
                    card.TerytStanNa = null;
                else
                    card.TerytStanNa = (DateTime)row["TerytStanNa"];
                if (row["DataKoniec"] is DBNull)
                    card.DataKoniec = null;
                else
                    card.DataKoniec = (DateTime)row["DataKoniec"];

                list.Add(card);
            }

            return list;
        }
        public List<MiejscowoscDbRow> GetMiejscowoscDbRowsToExport()
        {
            List<MiejscowoscDbRow> list = new List<MiejscowoscDbRow>();

            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@rowTypeId", (int)ExportRowType.Miejscowosc));
            parameters.Add(new SqlParameter("@kioskId", connection.KioskId));
            string query = "SELECT m.Id, m.KioskId, m.TerytId, m.TerytStanNa, m.Nazwa, m.GminaId, m.RodzajId, m.DataStart, m.DataKoniec, m.Deleted "
                         + "    , er.Id as ExportRowId, er.RowActionId as ExportActionId "
                         + "FROM dbo.[Miejscowosc] m "
                         + "    INNER JOIN dbo.[ExportRow] er ON m.Id = er.RowId AND er.RowTypeId = @rowTypeId AND er.KioskId = @kioskId "
                         + "WHERE NOT EXISTS (SELECT 1 FROM dbo.ExportRowExported ee WHERE ee.ExportRowId = er.Id AND ee.KioskId = @kioskId);";
            var result = connection.GetTable(query, parameters);

            if (result == null)
                return list;

            foreach (DataRow row in result.Rows)
            {
                var card = new MiejscowoscDbRow()
                {
                    ExportRowId = (int)row["ExportRowId"],
                    ExportRowType = ExportRowType.Customer,
                    ExportAction = (ExportAction)row["ExportActionId"],

                    Id = (int)row["Id"],
                    KioskId = (int)row["KioskId"],
                    Nazwa = row["Nazwa"].ToString(),
                    GminaId = (int)row["GminaId"],
                    RodzajId = (int)row["RodzajId"],

                    DataStart = (DateTime)row["DataStart"],
                    Deleted = (bool)row["Deleted"],
                };
                if (row["TerytId"] is DBNull)
                    card.TerytId = null;
                else
                    card.TerytId = (int)row["TerytId"];
                if (row["TerytStanNa"] is DBNull)
                    card.TerytStanNa = null;
                else
                    card.TerytStanNa = (DateTime)row["TerytStanNa"];
                if (row["DataKoniec"] is DBNull)
                    card.DataKoniec = null;
                else
                    card.DataKoniec = (DateTime)row["DataKoniec"];

                list.Add(card);
            }

            return list;
        }
        public List<UlicaDbRow> GetUliceDbRowsToExport()
        {
            List<UlicaDbRow> list = new List<UlicaDbRow>();

            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@rowTypeId", (int)ExportRowType.Ulica));
            parameters.Add(new SqlParameter("@kioskId", connection.KioskId));
            string query = "SELECT u.Id, u.KioskId, u.TerytId, u.TerytStanNa, u.Nazwa, u.MiejscowoscId, u.GminaId, u.RodzajId, u.DataStart, u.DataKoniec, u.Deleted "
                         + "    , er.Id as ExportRowId, er.RowActionId as ExportActionId "
                         + "FROM dbo.[Ulica] u "
                         + "    INNER JOIN dbo.[ExportRow] er ON u.Id = er.RowId AND er.RowTypeId = @rowTypeId AND er.KioskId = @kioskId "
                         + "WHERE NOT EXISTS (SELECT 1 FROM dbo.ExportRowExported ee WHERE ee.ExportRowId = er.Id AND ee.KioskId = @kioskId);";
            var result = connection.GetTable(query, parameters);

            if (result == null)
                return list;

            foreach (DataRow row in result.Rows)
            {
                var card = new UlicaDbRow()
                {
                    ExportRowId = (int)row["ExportRowId"],
                    ExportRowType = ExportRowType.Customer,
                    ExportAction = (ExportAction)row["ExportActionId"],

                    Id = (int)row["Id"],
                    KioskId = (int)row["KioskId"],
                    Nazwa = row["Nazwa"].ToString(),
                    MiejscowoscId = (int)row["MiejscowoscId"],
                    GminaId = (int)row["GminaId"],
                    RodzajId = (int)row["RodzajId"],

                    DataStart = (DateTime)row["DataStart"],
                    Deleted = (bool)row["Deleted"],
                };
                if (row["TerytId"] is DBNull)
                    card.TerytId = null;
                else
                    card.TerytId = (int)row["TerytId"];
                if (row["TerytStanNa"] is DBNull)
                    card.TerytStanNa = null;
                else
                    card.TerytStanNa = (DateTime)row["TerytStanNa"];
                if (row["DataKoniec"] is DBNull)
                    card.DataKoniec = null;
                else
                    card.DataKoniec = (DateTime)row["DataKoniec"];

                list.Add(card);
            }

            return list;
        }
        public List<AdresDbRow> GetAdresyDbRowsToExport()
        {
            List<AdresDbRow> list = new List<AdresDbRow>();

            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@rowTypeId", (int)ExportRowType.Address));
            parameters.Add(new SqlParameter("@kioskId", connection.KioskId));
            string query = "SELECT a.Id, a.KioskId, a.Nazwa, a.UlicaId, a.MiejscowoscId, a.GminaId, a.Numer, a.NumerUmowy,  a.RodzajId, a.DataStart, a.DataKoniec, a.Deleted "
                         + "    , er.Id as ExportRowId, er.RowActionId as ExportActionId "
                         + "FROM dbo.[Adres] a "
                         + "    INNER JOIN dbo.[ExportRow] er ON a.Id = er.RowId AND er.RowTypeId = @rowTypeId AND er.KioskId = @kioskId "
                         + "WHERE NOT EXISTS (SELECT 1 FROM dbo.ExportRowExported ee WHERE ee.ExportRowId = er.Id AND ee.KioskId = @kioskId);";
            var result = connection.GetTable(query, parameters);

            if (result == null)
                return list;

            foreach (DataRow row in result.Rows)
            {
                var card = new AdresDbRow()
                {
                    ExportRowId = (int)row["ExportRowId"],
                    ExportRowType = ExportRowType.Customer,
                    ExportAction = (ExportAction)row["ExportActionId"],

                    Id = (int)row["Id"],
                    KioskId = (int)row["KioskId"],
                    Nazwa = row["Nazwa"].ToString(),
                    MiejscowoscId = (int)row["MiejscowoscId"],
                    GminaId = (int)row["GminaId"],
                    Numer = row["Numer"].ToString(),
                    RodzajId = (int)row["RodzajId"],

                    DataStart = (DateTime)row["DataStart"],
                    Deleted = (bool)row["Deleted"],
                };
                if (row["UlicaId"] is DBNull)
                    card.UlicaId = null;
                else
                    card.UlicaId = (int)row["UlicaId"];
                if (row["NumerUmowy"].ToString() != "")
                    card.NumerUmowy = row["NumerUmowy"].ToString();
                if (row["DataKoniec"] is DBNull)
                    card.DataKoniec = null;
                else
                    card.DataKoniec = (DateTime)row["DataKoniec"];

                list.Add(card);
            }

            return list;
        }

        public List<FirmaDbRow> GetFirmyDbRowsToExport()
        {
            List<FirmaDbRow> list = new List<FirmaDbRow>();

            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@rowTypeId", (int)ExportRowType.Firma));
            parameters.Add(new SqlParameter("@kioskId", connection.KioskId));
            string query = "SELECT f.Id, f.KioskId, f.Nazwa, f.DataStart, f.DataKoniec, f.Deleted "
                         + "    , er.Id as ExportRowId, er.RowActionId as ExportActionId "
                         + "FROM dbo.[Firma] f "
                         + "    INNER JOIN dbo.[ExportRow] er ON f.Id = er.RowId AND er.RowTypeId = @rowTypeId AND er.KioskId = @kioskId "
                         + "WHERE NOT EXISTS (SELECT 1 FROM dbo.ExportRowExported ee WHERE ee.ExportRowId = er.Id AND ee.KioskId = @kioskId);";
            var result = connection.GetTable(query, parameters);

            if (result == null)
                return list;

            foreach (DataRow row in result.Rows)
            {
                var card = new FirmaDbRow()
                {
                    ExportRowId = (int)row["ExportRowId"],
                    ExportRowType = ExportRowType.Customer,
                    ExportAction = (ExportAction)row["ExportActionId"],

                    Id = (int)row["Id"],
                    KioskId = (int)row["KioskId"],
                    Nazwa = row["Nazwa"].ToString(),

                    DataStart = (DateTime)row["DataStart"],
                    Deleted = (bool)row["Deleted"],
                };
                if (row["DataKoniec"] is DBNull)
                    card.DataKoniec = null;
                else
                    card.DataKoniec = (DateTime)row["DataKoniec"];

                list.Add(card);
            }

            return list;
        }
        public List<FirmaAdresDbRow> GetFirmaAdresyDbRowsToExport()
        {
            List<FirmaAdresDbRow> list = new List<FirmaAdresDbRow>();

            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@rowTypeId", (int)ExportRowType.FirmaAdres));
            parameters.Add(new SqlParameter("@kioskId", connection.KioskId));
            string query = "SELECT f.Id, f.KioskId, f.AdresId, f.FirmaId, f.DataStart, f.DataKoniec, f.Deleted "
                         + "    , er.Id as ExportRowId, er.RowActionId as ExportActionId "
                         + "FROM dbo.[FirmaAdres] f "
                         + "    INNER JOIN dbo.[ExportRow] er ON f.Id = er.RowId AND er.RowTypeId = @rowTypeId AND er.KioskId = @kioskId "
                         + "WHERE NOT EXISTS (SELECT 1 FROM dbo.ExportRowExported ee WHERE ee.ExportRowId = er.Id AND ee.KioskId = @kioskId);";
            var result = connection.GetTable(query, parameters);

            if (result == null)
                return list;

            foreach (DataRow row in result.Rows)
            {
                var card = new FirmaAdresDbRow()
                {
                    ExportRowId = (int)row["ExportRowId"],
                    ExportRowType = ExportRowType.Customer,
                    ExportAction = (ExportAction)row["ExportActionId"],

                    Id = (int)row["Id"],
                    KioskId = (int)row["KioskId"],
                    AdresId = (int)row["AdresId"],
                    FirmaId = (int)row["FirmaId"],
                    
                    DataStart = (DateTime)row["DataStart"],
                    Deleted = (bool)row["Deleted"],
                };
                if (row["DataKoniec"] is DBNull)
                    card.DataKoniec = null;
                else
                    card.DataKoniec = (DateTime)row["DataKoniec"];

                list.Add(card);
            }

            return list;
        }


        public List<StalyKlientDbRow> GetStalyKlientDbRowsToExport()
        {
            List<StalyKlientDbRow> list = new List<StalyKlientDbRow>();

            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@rowTypeId", (int)ExportRowType.StalyKlient));
            parameters.Add(new SqlParameter("@kioskId", connection.KioskId));
            string query = "SELECT sk.Id, sk.KioskId, sk.AdresId, sk.FirmaId, sk.KlientId, sk.DataStart, sk.DataKoniec, sk.Deleted "
                         + "    , er.Id as ExportRowId, er.RowActionId as ExportActionId "
                         + "FROM dbo.[StalyKlient] sk "
                         + "    INNER JOIN dbo.[ExportRow] er ON sk.Id = er.RowId AND er.RowTypeId = @rowTypeId AND er.KioskId = @kioskId "
                         + "WHERE NOT EXISTS (SELECT 1 FROM dbo.ExportRowExported ee WHERE ee.ExportRowId = er.Id AND ee.KioskId = @kioskId);";
            var result = connection.GetTable(query, parameters);

            if (result == null)
                return list;

            foreach (DataRow row in result.Rows)
            {
                var card = new StalyKlientDbRow()
                {
                    ExportRowId = (int)row["ExportRowId"],
                    ExportRowType = ExportRowType.Customer,
                    ExportAction = (ExportAction)row["ExportActionId"],

                    Id = (int)row["Id"],
                    KioskId = (int)row["KioskId"],
                    AdresId = (int)row["AdresId"],
                    KlientId = (int)row["KlientId"],

                    DataStart = (DateTime)row["DataStart"],
                    Deleted = (bool)row["Deleted"],
                };
                if (row["FirmaId"] is DBNull)
                    card.FirmaId = null;
                else
                    card.FirmaId = (int)row["FirmaId"];
                if (row["DataKoniec"] is DBNull)
                    card.DataKoniec = null;
                else
                    card.DataKoniec = (DateTime)row["DataKoniec"];

                list.Add(card);
            }

            return list;
        }

        public List<CargoTypeDbRow> GetCargoTypeDbRowsToExport()
        {
            List<CargoTypeDbRow> list = new List<CargoTypeDbRow>();

            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@rowTypeId", (int)ExportRowType.CargoType));
            parameters.Add(new SqlParameter("@kioskId", 1));
            string query = "SELECT c.Id, c.Name, c.CargoCode, c.DateStart, c.DateEnd, c.Deleted "
                         + "    , er.Id as ExportRowId, er.RowActionId as ExportActionId "
                         + "FROM dbo.[CargoType] c "
                         + "    INNER JOIN dbo.[ExportRow] er ON c.Id = er.RowId AND er.RowTypeId = @rowTypeId "
                         + "WHERE NOT EXISTS (SELECT 1 FROM dbo.ExportRowExported ee WHERE ee.ExportRowId = er.Id AND ee.KioskId = @kioskId);";
            var result = connection.GetTable(query, parameters);

            if (result == null)
                return list;

            foreach (DataRow row in result.Rows)
            {
                var customer = new CargoTypeDbRow()
                {
                    ExportRowId = (int)row["ExportRowId"],
                    ExportRowType = ExportRowType.Customer,
                    ExportAction = (ExportAction)row["ExportActionId"],

                    Id = (int)row["Id"],
                    Name = row["Name"].ToString(),
                    CargoCode = row["CargoCode"].ToString(),

                    DateStart = (DateTime)row["DateStart"],
                    Deleted = (bool)row["Deleted"],
                };

                if (row["DateEnd"] is DBNull)
                    customer.DateEnd = null;
                else
                    customer.DateEnd = (DateTime)row["DateEnd"];

                list.Add(customer);
            }

            return list;
        }

        public int CheckConfigurationUpdatePending()
        {
            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@kioskId", connection.KioskId));
            parameters.Add(new SqlParameter("@rowTypeId", (int)ExportRowType.Configuration));
            string query = "SELECT Id FROM ExportRow er WHERE KioskId = @kioskId AND RowTypeId = @rowTypeId "
                         + "    AND NOT EXISTS (SELECT 1 FROM dbo.ExportRowExported ee WHERE ee.ExportRowId = er.Id AND ee.KioskId = @kioskId);";
            var result = connection.GetScalar(query, parameters);

            if (result == null)
            {
                return 0;
            }

            return (int)result;
        }
        public int CheckKioskDataUpdatePending()
        {
            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@kioskId", connection.KioskId));
            parameters.Add(new SqlParameter("@rowTypeId", (int)ExportRowType.Kiosk));
            string query = "SELECT Id FROM ExportRow er WHERE KioskId = @kioskId AND RowTypeId = @rowTypeId "
                         + "    AND NOT EXISTS (SELECT 1 FROM dbo.ExportRowExported ee WHERE ee.ExportRowId = er.Id AND ee.KioskId = @kioskId);";
            var result = connection.GetScalar(query, parameters);

            if (result == null)
            {
                return 0;
            }

            return (int)result;
        }
        public Dictionary<int, KioskConfiguration> GetConfiguration(KioskConfiguration currentConfiguration)
        {
            var configurations = new Dictionary<int, KioskConfiguration>();
            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@kioskId", connection.KioskId));
            string query = "SELECT DzienTygodnia, OtwarteOd, OtwarteDo "
                         + "FROM [GodzinyOtwarcia] "
                         + "WHERE KioskId = @kioskId;";
            var result = connection.GetTable(query, parameters);

            if (result == null || result.Rows.Count == 0)
            {
                return null;
            }

            foreach(DataRow dr in result.Rows)
            {
                var dayNo = (int)dr["DzienTygodnia"];
                configurations.Add(dayNo, new KioskConfiguration()
                {
                    OpenHoursFromInMinutesFromMidninght = (int)dr["OtwarteOd"],
                    OpenHoursToInMinutesFromMidninght = (int)dr["OtwarteDo"],
                });
            }

            DataRow row = result.Rows[0];
            configurations[1].KioskBlockages = GetConfigurationBlockages(configurations[1].KioskBlockages);
            configurations[1].ConfigurationSettings = GetConfigurationSettings(configurations[1].ConfigurationSettings);

            return configurations;
        }

        public KioskDbRow GetKioskData()
        {
            var kiosks = new Dictionary<int, KioskDbRow>();
            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@kioskId", connection.KioskId));
            parameters.Add(new SqlParameter("@rowTypeId", (int)ExportRowType.Kiosk));
            string query = "SELECT k.Id, k.LokalizacjaId, k.Nazwa, k.PobieranieProbek, k.Blokada, k.DataStart, k.DataKoniec, k.Deleted "
                         + "    , er.Id as ExportRowId, er.RowActionId as ExportActionId "
                         + "FROM [Kiosk] k "
                         + "    INNER JOIN dbo.[ExportRow] er ON k.Id = er.RowId AND er.RowTypeId = @rowTypeId "
                         + "WHERE k.Id = @kioskId "
                         + "    AND NOT EXISTS (SELECT 1 FROM dbo.ExportRowExported ee WHERE ee.ExportRowId = er.Id AND ee.KioskId = @kioskId);";
            var result = connection.GetTable(query, parameters);

            if (result == null || result.Rows.Count == 0)
            {
                return null;
            }

            DataRow row = result.Rows[0];
            var kiosk = new KioskDbRow()
            {
                ExportRowId = (int)row["ExportRowId"],
                ExportRowType = ExportRowType.Customer,
                ExportAction = (ExportAction)row["ExportActionId"],

                Id = (int)row["Id"],
                LokalizacjaId = (int)row["LokalizacjaId"],
                Nazwa = row["Nazwa"].ToString(),
                PobieranieProbek = (bool)row["PobieranieProbek"],
                Blokada = (bool)row["Blokada"],

                DataStart = (DateTime)row["DataStart"],
                Deleted = (bool)row["Deleted"],
            };
            if (row["DataKoniec"] is DBNull)
                kiosk.DataKoniec = null;
            else
                kiosk.DataKoniec = (DateTime)row["DataKoniec"];
            
            return kiosk;
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

        public ResetApp? GetResetApp()
        {
            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@kioskId", connection.KioskId));
            string query = "SELECT ResetApp, ResetSequence FROM [ResetApp] WHERE KioskId = @kioskId;";
            var result = connection.GetTable(query, parameters);

            if (result == null || result.Rows.Count != 1)
                return null;

            string delete = "DELETE FROM [ResetApp] WHERE KioskId = @kioskId;";
            connection.GetScalar(delete, parameters);

            DataRow row = result.Rows[0];
            var resetApp = (bool)row["ResetApp"];
            var resetSequence = (bool)row["ResetSequence"];

            if (resetSequence)
            {
                return ResetApp.ResetSequence;
            }
            else if (resetApp)
            {
                return ResetApp.ResetApp;
            }

            return null;
        }
    }
}
