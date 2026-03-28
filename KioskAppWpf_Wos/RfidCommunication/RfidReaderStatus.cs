namespace RfidCommunication
{
    public class RfidReaderStatus
    {
        public string LastReadId { get; set; }
        public string LastReadLicensePlate { get; set; }
        public int NumberOfReadId { get; set; }

        public bool Error { get; set; }
        public string ErrorMsg { get; set; }
    }
}
