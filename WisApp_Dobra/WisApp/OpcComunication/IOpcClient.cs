namespace OpcCommunication
{
    public interface IOpcClient
    {
        string ReadValue(string valueAddress);
        void WriteValueBool(string valueAddress, bool value);
    }
}
