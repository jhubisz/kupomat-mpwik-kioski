using DbCommunication.Entities;
using DbCommunication.Enums;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace DbCommunication
{
    public class DbWrite
    {
        private IDbConnection connection;

        private DbWrite() { }
        public DbWrite(IDbConnection conn)
        {
            connection = conn;
        }

        public bool SaveTransaction(Transaction transaction)
        {
            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@kioskId", connection.KioskId));
            parameters.Add(new SqlParameter("@rodzajSciekowId", transaction.Cargo.Id));
            parameters.Add(new SqlParameter("@klientId", transaction.Card.Customer.Id));
            parameters.Add(new SqlParameter("@samochodId", transaction.Card.Vechicle.Id));
            parameters.Add(new SqlParameter("@kartaId", transaction.Card.Id));
            parameters.Add(new SqlParameter("@numerUmowa", transaction.Card.Address.ContractNo));
            parameters.Add(new SqlParameter("@zadeklarowanaIlosc", transaction.CustomerAddresses.Sum(item => item.DeclaredAmount)));
            parameters.Add(new SqlParameter("@zlanaIlosc", transaction.ActualAmount));
            parameters.Add(new SqlParameter("@transakcjaStart", transaction.TransactionStart));
            parameters.Add(new SqlParameter("@zrzutStart", transaction.DumpStart));
            parameters.Add(new SqlParameter("@zrzutKoniec", transaction.DumpEnd));
            parameters.Add(new SqlParameter("@transakcjaKoniec", transaction.TransactionEnd));
            parameters.Add(new SqlParameter("@pobranoProbeHarm", transaction.ScheduledSample != null));
            parameters.Add(new SqlParameter("@pobranoProbeAlrm", transaction.AlarmSample != null));
            parameters.Add(new SqlParameter("@zakonczonaPoprawnie", transaction.FinishedCorrectly));
            parameters.Add(new SqlParameter("@powodZakonczeniaId", (int)transaction.FinishReason));
            string query = "EXECUTE [dbo].[_sp_add_transaction] @kioskId, @rodzajSciekowId, @klientId, @samochodId, @kartaId, @numerUmowa, "
                                + "@zadeklarowanaIlosc, @zlanaIlosc, @transakcjaStart, @zrzutStart, @zrzutKoniec, @transakcjaKoniec, @pobranoProbeHarm, @pobranoProbeAlrm, "
                                + "@zakonczonaPoprawnie, @powodZakonczeniaId; ";

            int insertedId = connection.InsertRow(query, parameters);

            if (insertedId == 0)
            {
                return false;
            }
            transaction.Id = insertedId;

            if (transaction.Parameters != null || transaction.PressurePumpOn)
            {
                SaveTransactionParameters(transaction);
            }

            if (transaction.ScheduledSample != null)
            {
                SaveTransactionSample(transaction.Id.Value, transaction.ScheduledSample);
            }
            if (transaction.AlarmSample != null)
            {
                SaveTransactionSample(transaction.Id.Value, transaction.AlarmSample);
            }

            foreach (CustomerAddress address in transaction.CustomerAddresses)
            {
                SaveTransactionAddress(transaction.Id.Value, transaction.Card.Customer.Id, address);
            }

            return true;
        }
        public bool SaveTransactionParameters(Transaction transaction)
        {
            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@kioskId", connection.KioskId));
            parameters.Add(new SqlParameter("@transakcjaId", transaction.Id));
            if (transaction.Parameters != null)
            {
                parameters.Add(new SqlParameter("@phMin", transaction.Parameters.FlowphMin));
                parameters.Add(new SqlParameter("@phMax", transaction.Parameters.FlowphMax));
                parameters.Add(new SqlParameter("@phAvg", transaction.Parameters.FlowphMed));
                parameters.Add(new SqlParameter("@przewodnoscMin", transaction.Parameters.FlowCondMin));
                parameters.Add(new SqlParameter("@przewodnoscMax", transaction.Parameters.FlowCondMax));
                parameters.Add(new SqlParameter("@przewodnoscAvg", transaction.Parameters.FlowCondMed));
                parameters.Add(new SqlParameter("@tempMin", transaction.Parameters.FlowTempMin));
                parameters.Add(new SqlParameter("@tempMax", transaction.Parameters.FlowTempMax));
                parameters.Add(new SqlParameter("@tempAvg", transaction.Parameters.FlowTempMed));
                parameters.Add(new SqlParameter("@chztMin", transaction.Parameters.FlowChztMin));
                parameters.Add(new SqlParameter("@chztMax", transaction.Parameters.FlowChztMax));
                parameters.Add(new SqlParameter("@chztAvg", transaction.Parameters.FlowChztMed));
            }
            else
            {
                parameters.Add(new SqlParameter("@phMin", DBNull.Value));
                parameters.Add(new SqlParameter("@phMax", DBNull.Value));
                parameters.Add(new SqlParameter("@phAvg", DBNull.Value));
                parameters.Add(new SqlParameter("@przewodnoscMin", DBNull.Value));
                parameters.Add(new SqlParameter("@przewodnoscMax", DBNull.Value));
                parameters.Add(new SqlParameter("@przewodnoscAvg", DBNull.Value));
                parameters.Add(new SqlParameter("@tempMin", DBNull.Value));
                parameters.Add(new SqlParameter("@tempMax", DBNull.Value));
                parameters.Add(new SqlParameter("@tempAvg", DBNull.Value));
                parameters.Add(new SqlParameter("@chztMin", DBNull.Value));
                parameters.Add(new SqlParameter("@chztMax", DBNull.Value));
                parameters.Add(new SqlParameter("@chztAvg", DBNull.Value));
            }
            parameters.Add(new SqlParameter("@cisnienie", transaction.PressurePumpOn ? 1 : 0));
            string query = "EXECUTE [dbo].[_sp_add_transaction_parameters] @kioskId, @transakcjaId "
                         + "  , @phMin, @phMax, @phAvg, @przewodnoscMin, @przewodnoscMax, @przewodnoscAvg, @tempMin, @tempMax, @tempAvg, @chztMin, @chztMax, @chztAvg, @cisnienie; ";

            int insertedId = connection.InsertRow(query, parameters);

            if (insertedId == 0)
            {
                return false;
            }
            return true;
        }
        public bool SaveTransactionSample(int transactionId, Sample sample)
        {
            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@kioskId", connection.KioskId));
            parameters.Add(new SqlParameter("@numerButelki", sample.BottleNo));
            parameters.Add(new SqlParameter("@probaPobrana", true));
            parameters.Add(new SqlParameter("@probaData", sample.ProbeTime));
            parameters.Add(new SqlParameter("@probaTypId", sample.Type));
            parameters.Add(new SqlParameter("@transakcjaId", transactionId));
            string query = "EXECUTE [dbo].[_sp_add_transaction_sample] @kioskId, @numerButelki, @probaPobrana, @probaData, @probaTypId, @transakcjaId;";

            int insertedId = connection.InsertRow(query, parameters);

            if (insertedId == 0)
            {
                return false;
            }
            return true;
        }
        public bool SaveTransactionAddress(int transactionId, int customerId, CustomerAddress address)
        {
            if (address.DbType == AddressDbType.ToiToi)
            {
                return SaveTransactionAddressTransactionAddress(transactionId, address.DeclaredAmount, address.ContractNo, null, null, null, null);
            }
            else if (address.DbType == AddressDbType.Rod)
            {
                return SaveTransactionAddressTransactionAddress(transactionId, address.DeclaredAmount, address.ContractNo, null, null, address.RodId, address.AddressNumber);
            }
            else if (address.Company != null)
            {
                return SaveTransactionAddressCompany(transactionId, customerId, address);
            }
            else
            {
                return SaveTransactionAddressCustomer(transactionId, customerId, address);
            }
        }
        public bool SaveTransactionAddressRod(int transactionId, CustomerAddress address)
        {
            return true;
        }
        public bool SaveTransactionAddressCustomer(int transactionId, int customerId, CustomerAddress address)
        {
            if (address.GminaId == 0)
            {
                address.GminaId = SaveTransactionAddressGmina(address.GminaName);
            }
            if (address.MiejscowoscId == 0)
            {
                address.MiejscowoscId = SaveTransactionAddressMiejscowosc(address.GminaId, address.MiejscowoscName);
            }
            if (address.UlicaId == 0 && address.UlicaName != "")
            {
                address.UlicaId = SaveTransactionAddressUlica(address.GminaId, address.MiejscowoscId, address.UlicaName);
            }
            address.Id = SaveTransactionAddressCustomerAddress(address);
            SaveTransacitonAddressRegularCustomer(customerId, address);
            SaveTransactionAddressTransactionAddress(transactionId, address.DeclaredAmount, address.ContractNo, address.Id, null, null, null);
            return true;
        }
        public bool SaveTransactionAddressCompany(int transactionId, int customerId, CustomerAddress address)
        {
            if (address.GminaId == 0)
            {
                address.GminaId = SaveTransactionAddressGmina(address.GminaName);
            }
            if (address.MiejscowoscId == 0)
            {
                address.MiejscowoscId = SaveTransactionAddressMiejscowosc(address.GminaId, address.MiejscowoscName);
            }
            if (address.UlicaId == 0 && !string.IsNullOrEmpty(address.UlicaName))
            {
                address.UlicaId = SaveTransactionAddressUlica(address.GminaId, address.MiejscowoscId, address.UlicaName);
            }
            address.Id = SaveTransactionAddressCustomerAddress(address);
            address.Company.Id = SaveTransactionAddressCompanyCompany(address.Company.Name, address.Id.Value);
            SaveTransacitonAddressRegularCustomer(customerId, address);
            SaveTransactionAddressTransactionAddress(transactionId, address.DeclaredAmount, address.ContractNo, address.Id, address.Company.Id, null, null);
            return true;
        }

        public int SaveTransactionAddressCompanyCompany(string firmaName, int addressId)
        {
            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@kioskId", connection.KioskId));
            parameters.Add(new SqlParameter("@firma", firmaName));
            string query = "EXECUTE [dbo].[_sp_add_company] @kioskId, @firma; ";

            int insertedId = connection.InsertRow(query, parameters);
            if (insertedId > 0)
            {
                int comanyAddressId = SaveTransactionAddressCompanyCompanyAddress(insertedId, addressId);
                if (comanyAddressId > 0)
                {
                    return insertedId;
                }
                return 0;
            }
            return 0;
        }
        public int SaveTransactionAddressCompanyCompanyAddress(int firmaId, int addressId)
        {
            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@kioskId", connection.KioskId));
            parameters.Add(new SqlParameter("@firmaId", firmaId));
            parameters.Add(new SqlParameter("@adresId", addressId));
            string query = "EXECUTE [dbo].[_sp_add_companyAddress] @kioskId, @firmaId, @adresId; ";

            int insertedId = connection.InsertRow(query, parameters);

            return insertedId;
        }

        private bool SaveTransacitonAddressRegularCustomer(int customerId, CustomerAddress address)
        {
            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@kioskId", connection.KioskId));
            parameters.Add(new SqlParameter("@adresId", address.Id.Value));
            if (address.Company == null)
            {
                parameters.Add(new SqlParameter("@firmaId", DBNull.Value));
            }
            else
            {
                parameters.Add(new SqlParameter("@firmaId", address.Company.Id));
            }
            parameters.Add(new SqlParameter("@klientId", customerId));
            string query = "EXECUTE [dbo].[_sp_add_regularCustomer] @kioskId, @adresId, @firmaId, @klientId; ";

            int insertedId = connection.InsertRow(query, parameters);

            if (insertedId > 0)
            {
                return true;
            }

            return false;
        }

        private int SaveTransactionAddressCustomerAddress(CustomerAddress address)
        {
            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@kioskId", connection.KioskId));
            if (address.Name == null)
            {
                parameters.Add(new SqlParameter("@nazwa", ""));
            }
            else
            {
                parameters.Add(new SqlParameter("@nazwa", address.Name));
            }
            if (address.UlicaId == 0)
            {
                parameters.Add(new SqlParameter("@ulicaId", DBNull.Value));
            }
            else
            {
                parameters.Add(new SqlParameter("@ulicaId", address.UlicaId));
            }
            parameters.Add(new SqlParameter("@miejscowoscId", address.MiejscowoscId));
            parameters.Add(new SqlParameter("@gminaId", address.GminaId));
            parameters.Add(new SqlParameter("@numer", address.AddressNumber));
            parameters.Add(new SqlParameter("@numerUmowy", address.ContractNo));
            string query = "EXECUTE [dbo].[_sp_add_address] @kioskId, @nazwa, @ulicaId, @miejscowoscId, @gminaId, @numer, @numerUmowy; ";

            int insertedId = connection.InsertRow(query, parameters);

            return insertedId;
        }
        private int SaveTransactionAddressGmina(string gminaName)
        {
            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@kioskId", connection.KioskId));
            parameters.Add(new SqlParameter("@gmina", gminaName));
            string query = "EXECUTE [dbo].[_sp_add_gmina] @kioskId, @gmina; ";

            int insertedId = connection.InsertRow(query, parameters);

            return insertedId;
        }
        private int SaveTransactionAddressMiejscowosc(int gminaId, string miejscowoscName)
        {
            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@kioskId", connection.KioskId));
            parameters.Add(new SqlParameter("@gminaId", gminaId));
            parameters.Add(new SqlParameter("@miejscowosc", miejscowoscName));
            string query = "EXECUTE [dbo].[_sp_add_miejscowosc] @kioskId, @gminaId, @miejscowosc; ";

            int insertedId = connection.InsertRow(query, parameters);

            return insertedId;
        }
        private int SaveTransactionAddressUlica(int gminaId, int miejscowoscId, string ulicaName)
        {
            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@kioskId", connection.KioskId));
            parameters.Add(new SqlParameter("@gminaId", gminaId));
            parameters.Add(new SqlParameter("@miejscowoscId", miejscowoscId));
            parameters.Add(new SqlParameter("@ulica", ulicaName));
            string query = "EXECUTE [dbo].[_sp_add_ulica] @kioskId, @gminaId, @miejscowoscId, @ulica; ";

            int insertedId = connection.InsertRow(query, parameters);

            return insertedId;
        }

        private bool SaveTransactionAddressTransactionAddress(int transactionId, decimal declaredAmount, string contractNo, int? addressId, int? companyId, int? rodId, string rodPlotNo)
        {
            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@kioskId", connection.KioskId));
            parameters.Add(new SqlParameter("@transakcjaId", transactionId));
            parameters.Add(new SqlParameter("@zadeklarowanaIlosc", declaredAmount));
            if (contractNo == null || contractNo == "")
            {
                parameters.Add(new SqlParameter("@numerUmowy", DBNull.Value));
            }
            else
            {
                parameters.Add(new SqlParameter("@numerUmowy", contractNo));
            }
            if (!addressId.HasValue)
            {
                parameters.Add(new SqlParameter("@adresId", DBNull.Value));
            }
            else
            {
                parameters.Add(new SqlParameter("@adresId", addressId.Value));
            }
            if (!companyId.HasValue)
            {
                parameters.Add(new SqlParameter("@firmaId", DBNull.Value));
            }
            else
            {
                parameters.Add(new SqlParameter("@firmaId", companyId.Value));
            }
            if (!rodId.HasValue)
            {
                parameters.Add(new SqlParameter("@rodId", DBNull.Value));
            }
            else
            {
                parameters.Add(new SqlParameter("@rodId", rodId.Value));
            }
            if (rodPlotNo == null || rodPlotNo == "")
            {
                parameters.Add(new SqlParameter("@rodNrDzialki", DBNull.Value));
            }
            else
            {
                parameters.Add(new SqlParameter("@rodNrDzialki", rodPlotNo));
            }
            string query = "EXECUTE [dbo].[_sp_add_transaction_address] @kioskId, @transakcjaId, @zadeklarowanaIlosc, @numerUmowy, @adresId, @firmaId, @rodId, @rodNrDzialki; ";

            int insertedId = connection.InsertRow(query, parameters);

            if (insertedId > 0)
            {
                return true;
            }
            return false;
        }
                
        public bool UpdatePrintedPaperLenght(int lenghtInMm)
        {
            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@leght", lenghtInMm));
            parameters.Add(new SqlParameter("@kioskId", connection.KioskId));
            string query = "UPDATE [PrinterPaperStatus] "
                         + "SET PrintedReciptLenghtInMilimeters = PrintedReciptLenghtInMilimeters + @leght "
                         + "WHERE KioskId = @kioskId;";

            int rowsAffected = connection.UpdateRow(query, parameters);

            if (rowsAffected > 0)
                return true;
            else
                return false;
        }

        public bool ResetPrintedPaperLenght()
        {
            string query = "UPDATE [PrinterPaperStatus] "
                         + "SET PrintedReciptLenghtInMilimeters = 0;";

            int rowsAffected = connection.UpdateRow(query);

            if (rowsAffected > 0)
                return true;
            else
                return false;
        }
    }
}
