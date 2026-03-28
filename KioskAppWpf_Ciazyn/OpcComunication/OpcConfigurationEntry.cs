using OpcCommunication.Enums;

namespace OpcCommunication
{
    public class OpcConfigurationEntry<T>
    {
        public uint Mask { get; set; }
        public T Type { get; set; }
        public string Msg { get; set; }
    }
}
