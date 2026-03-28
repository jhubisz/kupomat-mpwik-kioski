using RfidCommunication.Enums;

namespace RfidCommunication
{
    public class TouchedCard
    {
        public ReaderLocation CardLocation { get; set; }
        public string CardId { get; set; }
        public string LicensePlate { get; set; }

        public bool Error { get; set; }
        public string ErrorMsg { get; set; }
    }
}
