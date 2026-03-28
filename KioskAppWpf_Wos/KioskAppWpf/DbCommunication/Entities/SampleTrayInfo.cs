using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbCommunication.Entities
{
    public class SampleTrayInfo
    {
        public int Id { get; set; }
        public int TrayNo { get; set; }
        public DateTime TrayStartDate { get; set; }
        public int NoOfScheduledSamples { get; set; }
        public int NoOfAlarmSamples { get; set; }
    }
}
