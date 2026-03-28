using DbCommunication.Enums;
using KioskAppWpf.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DbCommunication.Entities
{
    [Serializable]
    public class Transaction
    {
        public string TimeStamp { get; set; }

        public int? Id { get; set; }

        public int? ReciptId { get; set; }
        public int? ReciptNo { get; set; }
        public int? ReciptYear { get; set; }

        public RfidCard Card { get; set; }
        public CargoType Cargo { get; set; }
        public Address Address { get; set; }

        public string ContractNo { get; set; }
        private decimal actualAmount;
        public decimal ActualAmount {
            get
            {
                return actualAmount;
            }
            set
            {
                actualAmount = value;
                var declaredAmount = CustomerAddresses.Sum(item => item.DeclaredAmount);

                if (declaredAmount == 0)
                {
                    foreach (var addr in CustomerAddresses)
                    {
                        addr.ActualAmount = addr.DeclaredAmount;
                    }
                    return;
                }

                var proportional = value / declaredAmount;
                foreach(var addr in CustomerAddresses)
                {
                    addr.ActualAmount = proportional * addr.DeclaredAmount;
                }
            }
        }

        public DateTime TransactionStart{ get; set; }
        public DateTime DumpStart { get; set; }
        public DateTime DumpEnd { get; set; }
        public DateTime TransactionEnd { get; set; }

        public bool DumpStarted { get; set; }

        public StationLicense License { get; set; }

        public List<CustomerAddress>  CustomerAddresses{ get; set; }
        public int EditedAddress { get; set; }
        public decimal EditedSewageAmount { get; set; }

        public List<HarmonogramedSample> HarmonogramedSample { get; set; }

        public TransactionStep TransactionStep { get; set; }

        public bool TransactionChargable { get; set; }
        public bool Finished { get; set; }

        public bool FinishedCorrectly { get; set; }
        public string FinishReason { get; set; }
        public TransactionParameters Parameters { get; set; }
        public bool PressurePumpOn { get; set; }

        public Sample ScheduledSample { get; set; }
        public Sample AlarmSample { get; set; }

        public bool WaterForDriverUsed { get; set; }

        public bool Saved { get; set; }

        public Transaction()
        {
            TimeStamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
            CustomerAddresses = new List<CustomerAddress>();
            HarmonogramedSample = new List<HarmonogramedSample>();
            TransactionStep = TransactionStep.TransactionCreated;
        }
    }
}
