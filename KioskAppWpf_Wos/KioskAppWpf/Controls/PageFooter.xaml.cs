using DbCommunication.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace KioskAppWpf.Controls
{
    /// <summary>
    /// Interaction logic for PageFooter.xaml
    /// </summary>
    public partial class PageFooter : UserControl
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
                BindCustomer();
            }
        }

        public PageFooter()
        {
            InitializeComponent();
        }
        private void BindCustomer()
        {
            if (Transaction == null) return; 

            tbCustomerName.Text = Transaction.Address.Customer.Name;
            tbCustomerAddress1.Text = Transaction.Address.AddressLine1;
            if (Transaction.Address.AddressLine2 != "")
            {
                tbCustomerAddress2.Text = Transaction.Address.AddressLine2;
                tbCustomerAddress3.Text = Transaction.Address.PostCode + " " + Transaction.Address.City;
            }
            else
            {
                tbCustomerAddress2.Text = Transaction.Address.PostCode + " " + Transaction.Address.City;
                tbCustomerAddress3.Text = "";
            }

            tbRegistrationNo.Text = "Nr. rej.: " + Transaction.Card.Vechicle.LicensePlate;

            tbNoOfAddresses.Text = (Transaction.CustomerAddresses.Count + Transaction.EditedAddress).ToString();
            if (Transaction != null && Transaction.Cargo != null && Transaction.Cargo.Id == 2)
            {
                tbMaxAddresses.Text =  "z 1";
            }
            else
            {
                tbMaxAddresses.Text =  "z 15";
            }

            var sewageAmount = Transaction.CustomerAddresses.Sum(item => item.DeclaredAmount) + Transaction.EditedSewageAmount;
            tbTotalSewageAmount.Text = sewageAmount.ToString();
        }
    }
}
