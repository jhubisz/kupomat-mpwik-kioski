using Opc.Ua;
using Siemens.OpcUA;
using System;

namespace OpcCommunication
{
    public class OpcClient : IOpcClient
    {
        private IOpcConfiguration configuration;

        private Server server = null;
        private UInt16 nameSpaceIndex = 0;

        public IOpcConfiguration Configuration
        {
            get
            {
                return configuration;
            }
        }
        private Server Server
        {
            get
            {
                if (server == null || server.Session == null || !server.Session.Connected)
                {
                    server = new Server();

                    server.CertificateEvent += new certificateValidation(EventServerCertificate);
                    server.Connect(Configuration.ServerAddress);
                }

                return server;
            }
        }
        private UInt16 NameSpaceIndex
        {
            get
            {
                if (nameSpaceIndex == 0)
                {
                    NodeIdCollection nodesToRead = new NodeIdCollection();
                    DataValueCollection results;

                    nodesToRead.Add(Variables.Server_NamespaceArray);

                    Server.ReadValues(nodesToRead, out results);

                    if ((results.Count != 1) || (results[0].Value.GetType() != typeof(string[])))
                    {
                        throw new Exception("Odczyt z tabeli przestrzeni nazw zwrócił nieoczekiwany wynik.");
                    }

                    string[] nameSpaceArray = (string[])results[0].Value;
                    ushort i;
                    for (i = 0; i < nameSpaceArray.Length; i++)
                    {
                        if (nameSpaceArray[i] == Configuration.NamespaceUri)
                        {
                            nameSpaceIndex = i;
                        }
                    }

                    if (nameSpaceIndex == 0)
                    {
                        throw new Exception("Przestrzeń nazw " + Configuration.NamespaceUri + " nie została odnaleziona");
                    }
                }

                return nameSpaceIndex;
            }
        }

        public bool BadTimeout { get; set; }

        public bool IsConnected
        {
            get
            {
                return Server != null && server.Session != null && server.Session.Connected;
            }
        }

        #region Singleton implementation
        private static OpcClient instance;
        public static OpcClient GetInstance(IOpcConfiguration configuration)
        {
            if (instance == null)
            {
                instance = new OpcClient(configuration);
            }
            return instance;
        }

        private OpcClient(IOpcConfiguration config)
        {
            configuration = config;
        }
        #endregion

        public string ReadValue(string valueAddress)
        {
            if (BadTimeout && valueAddress != Configuration.VarAddressLifeBit)
            {
                return "True";
            }

            try
            {
                NodeIdCollection nodesToRead = new NodeIdCollection();
                DataValueCollection results;

                // Add the two variable NodeIds to the list of nodes to read
                // NodeId is constructed from 
                // - the identifier text in the text box
                // - the namespace index collected during the server connect
                nodesToRead.Add(new NodeId(valueAddress, NameSpaceIndex));

                // Read the values
                Server.ReadValues(nodesToRead, out results);

                if (results.Count != 1)
                {
                    throw new Exception("Odczyt zmiennej nie udał się.");
                }

                // Print result for first variable - check first the result code
                if (StatusCode.IsBad(results[0].StatusCode))
                {
                    BadTimeout = true;
                    // The node failed - print the symbolic name of the status code
                    return "False";
                }
                else
                {
                    BadTimeout = false;
                    // The node succeeded - print the value as string
                    return results[0].Value.ToString();
                }
            }
            catch (Exception ex)
            {
               return "Błąd odczytu:\n\n" + ex.Message;
            }
        }

        public void WriteValueBool(string valueAddress, bool value)
        {
            if (BadTimeout)
            {
                return;
            }
            WriteValueBool(valueAddress, value ? (byte)1 : (byte)0);
        }
        private string WriteValueBool(string valueAddress, byte value)
        {
            try
            {
                int writeLength = 4096;
                byte[] rawValue = new byte[writeLength];
                byte currentValue = value;
                object writeValue;

                for (int i = 0; i < rawValue.Length; i++)
                {
                    rawValue[i] = currentValue;
                    currentValue++;
                }

                writeValue = rawValue;

                WriteNewBlockValue(
                    new NodeId(valueAddress, NameSpaceIndex), // NodeId = identifier + namespace index
                    writeValue); // Value to write as byte array

                return "";
            }
            catch (Exception ex)
            {
                return "Błąd zapisu:\n\n" + ex.Message;
            }
        }

        public void WriteNewBlockValue(string valueAddress, object valueToWrite)
        {
            if (BadTimeout)
            {
                return;
            }
            WriteNewBlockValue(new NodeId(valueAddress, NameSpaceIndex), valueToWrite);
        }

        public void WriteNewBlockValue(NodeId nodeToWrite, object valueToWrite)
        {
            NodeIdCollection nodesToWrite = new NodeIdCollection();
            DataValueCollection values = new DataValueCollection();
            StatusCodeCollection results;
            Variant value = new Variant(valueToWrite);

            nodesToWrite.Add(nodeToWrite);
            values.Add(new DataValue(value));

            Server.WriteValues(
                nodesToWrite,
                values,
                out results);
        }

        void EventServerCertificate(CertificateValidator validator, CertificateValidationEventArgs e)
        {
            e.Accept = true;
        }
    }
}
