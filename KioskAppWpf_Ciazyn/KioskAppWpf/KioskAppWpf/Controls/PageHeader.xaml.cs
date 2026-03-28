using DbCommunication.Entities;
using DbCommunication.Enums;
using System.Linq;
using System.Windows.Controls;

namespace KioskAppWpf.Controls
{
    /// <summary>
    /// Interaction logic for PageFooter.xaml
    /// </summary>
    public partial class PageHeader : UserControl
    {
        private Transaction transaction;
        public Transaction Transaction
        {
            get
            {
                return transaction;
            }
            set
            {
                transaction = value;
                BindHead();
            }
        }
        private AddressDbType dbType;
        public AddressDbType DbType
        {
            get
            {
                return dbType;
            }
            set
            {
                dbType = value;
                if (Transaction != null)
                {
                    BindHead();
                }
            }
        }

        public PageHeader()
        {
            InitializeComponent();
        }

        private void BindHead()
        {
            var headText = "";
            var cargoType = StringCargoType();
            var dbType = StringDbType();
            headText = StringCargoType();
            if (dbType != "")
            {
                headText += " - " + dbType;
            }
            tbHead.Text = headText;
        }

        private string StringCargoType()
        {
            switch(transaction.Cargo.Id)
            {
                case 1: return "Ścieki bytowe";
                case 2: return "Ścieki przemysłowe";
                case 3: return "Ścieki ROD";
                case 4: return "Ścieki z toalet przenośnych";
            }
            return "";
        }
        private string StringDbType()
        {
            switch(DbType)
            {
                case AddressDbType.RegularCustomers: return "Baza stałych klientów";
                case AddressDbType.Teryt: return "Baza adresów Teryt";
                case AddressDbType.ManualEntry: return "Wpisywanie ręczne";
                case AddressDbType.Rod: return "Baza ROD";
            }
            return "";
        }
    }
}
