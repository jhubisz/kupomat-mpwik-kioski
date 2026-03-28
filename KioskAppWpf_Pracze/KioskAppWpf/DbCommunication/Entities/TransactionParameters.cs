using System;

namespace DbCommunication.Entities
{
    [Serializable]
    public class TransactionParameters
    {
        public decimal FlowphMin { get; set; }
        public decimal FlowphMed { get; set; }
        public decimal FlowphMax { get; set; }
        public decimal FlowCondCurr { get; set; }
        public decimal FlowCondMin { get; set; }
        public decimal FlowCondMed { get; set; }
        public decimal FlowCondMax { get; set; }
        public decimal FlowTempCurr { get; set; }
        public decimal FlowTempMin { get; set; }
        public decimal FlowTempMed { get; set; }
        public decimal FlowTempMax { get; set; }
    }
}
