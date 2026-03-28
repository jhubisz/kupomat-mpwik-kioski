using RfidCommunication.Enums;
using System.IO.Ports;

namespace RfidCommunication
{
    public class RfidReaderConfig
    {
        public string Uri { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }

        public string ComName { get; set; }
        public int ComDataBits { get; set; }
        public int ComBoundRate { get; set; }
        public Parity ComParity { get; set; }
        public StopBits ComStopBits { get; set; }

        public ReaderLocation ReaderLocation{ get; set; }
        public ReaderType ReaderType { get; set; }
    }
}
