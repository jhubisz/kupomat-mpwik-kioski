using RfidCommunication.Enums;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO.Ports;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Xml;

namespace RfidCommunication
{
    public class RfidManager
    {
        private RfidConfig Configuration { get; set; }
        private Dictionary<ReaderLocation, RfidReaderStatus> ReaderStatus { get; set; }
        private SerialPort serialPort;
        private Queue<byte> recievedData = new Queue<byte>();

        private RfidCamera CameraReader { get; set; }

        private bool _RfidReaderReconnectAfterRead
        {
            get
            {
                return bool.Parse(ConfigurationManager.AppSettings["_RfidReaderReconnectAfterRead"]);
            }
        }

        public RfidManager() { }
        public RfidManager(RfidConfig config)
        {
            Configuration = config;
            ReaderStatus = new Dictionary<ReaderLocation, RfidReaderStatus>();
            CameraReader = new RfidCamera();
            foreach (var reader in config.RfidCardReader.Values)
            {
                ReaderStatus.Add(reader.ReaderLocation, CheckCard(reader.ReaderLocation, reader));
            }
        }

        public List<TouchedCard> CheckCardStatus()
        {
            List<TouchedCard> eventList = new List<TouchedCard>();
            foreach(var readerLocation in Configuration.RfidCardReader.Keys)
            {
                var status = CheckCard(readerLocation, Configuration.RfidCardReader[readerLocation]);
                if (status == null)
                    return eventList;

                if (status.Error)
                {
                    eventList.Add(new TouchedCard() { CardLocation = readerLocation, Error = status.Error, ErrorMsg = status.ErrorMsg });
                }
                else if (ReaderStatus[readerLocation] == null || ReaderStatus[readerLocation].LastReadId == null)
                {
                    ReaderStatus[readerLocation] = status;
                }
                else if (ReaderStatus[readerLocation].NumberOfReadId != status.NumberOfReadId)
                {
                    eventList.Add(new TouchedCard() { CardId = status.LastReadId, LicensePlate = status.LastReadLicensePlate, CardLocation = readerLocation });
                    ReaderStatus[readerLocation] = status;
                }
            }
            return eventList;
        }

        public RfidReaderStatus CheckCard(ReaderLocation configItem, RfidReaderConfig reader)
        {
            if (reader.ReaderType == ReaderType.Ethernet)
            {
                return CheckCardEthernet(configItem, reader);
            }
            if (reader.ReaderType == ReaderType.Com)
            {
                return CheckCardCom(configItem, reader);
            }
            if (reader.ReaderType == ReaderType.Camera)
            {
                var readerStatus = ReaderStatus.ContainsKey(configItem) ? ReaderStatus[configItem] : new RfidReaderStatus();
                return CameraReader.CheckCameraCom(configItem, reader, readerStatus);
            }
            return null;
        }

        private RfidReaderStatus CheckCardEthernet(ReaderLocation configItem, RfidReaderConfig reader)
        {
            HttpClient httpClient = new HttpClient();
            httpClient.Timeout = TimeSpan.FromMilliseconds(7000);
            HttpResponseMessage response = null;
            try
            {
                var credentials = Encoding.ASCII.GetBytes(reader.Login + ":" + reader.Password);
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(credentials));

                response = httpClient.GetAsync(reader.Uri).Result;
            }
            catch(Exception ex)
            {
                return new RfidReaderStatus() { Error = true, ErrorMsg = ex.Message };
            }

            if (response != null)
                return ParseRfidReaderXmlToStatusObject(response.Content.ReadAsStringAsync().Result);
            else
                return null;
        }
        private RfidReaderStatus CheckCardCom(ReaderLocation configItem, RfidReaderConfig reader)
        {
            if (serialPort != null && serialPort.IsOpen)
            {
                if (recievedData.Count >= 5)
                {
                    var packet = Enumerable.Range(0, 5).Select(i => recievedData.Dequeue());
                    string cardId = PacketToCardId(packet.ToArray());
                    recievedData.Clear();
                    if (_RfidReaderReconnectAfterRead)
                    {
                        ReconnectSerialPort(serialPort.PortName, serialPort.DataBits, serialPort.BaudRate, serialPort.Parity, serialPort.StopBits);
                    }
                    return new RfidReaderStatus() { LastReadId = cardId, NumberOfReadId = ReaderStatus[configItem].NumberOfReadId++ };
                }
            }
            else
            {
                try
                {
                    StartCardCom(configItem, reader);
                }
                catch (Exception ex)
                {
                    return new RfidReaderStatus() { Error = true, ErrorMsg = ex.Message };
                }
            }
            return new RfidReaderStatus() { LastReadId = "", NumberOfReadId = 0 }; ;
        }

        private void StartCardCom(ReaderLocation configItem, RfidReaderConfig reader)
        {
            if (serialPort == null || !serialPort.IsOpen)
            {
                serialPort = new SerialPort(reader.ComName);
                serialPort.DataBits = reader.ComDataBits;
                serialPort.BaudRate = reader.ComBoundRate;
                serialPort.Parity = reader.ComParity;
                serialPort.StopBits = reader.ComStopBits;
                serialPort.Open();
                serialPort.DataReceived += SerialPortDataReceived;
            }
        }
        void SerialPortDataReceived(object s, SerialDataReceivedEventArgs e)
        {
            byte[] data = new byte[serialPort.BytesToRead];
            serialPort.Read(data, 0, data.Length);
            data.ToList().ForEach(b => recievedData.Enqueue(b));
        }

        private void ReconnectSerialPort(string portName, int dataBits, int baudRate, Parity parity, StopBits stopBits)
        {
            serialPort.Close();

            serialPort = new SerialPort(portName);
            serialPort.DataBits = dataBits;
            serialPort.BaudRate = baudRate;
            serialPort.Parity = parity;
            serialPort.StopBits = stopBits;
            serialPort.Open();
            serialPort.DataReceived += SerialPortDataReceived;
        }

        private string PacketToCardId(byte[] packet)
        {
            var cardId = "";
            if (packet.Length >= 5)
                cardId = BitConverter.ToString(packet).Replace("-", string.Empty);
            return cardId;
        }

        public void Dispose()
        {
            if (serialPort != null)
            {
                serialPort.Dispose();
            }
        }
        
        private RfidReaderStatus ParseRfidReaderXmlToStatusObject(string rfidXml)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(rfidXml);

            string xpathNoOfRead = "status/cnt";
            var numberOfRead = Int32.Parse(xmlDoc.SelectNodes(xpathNoOfRead)[0].InnerText);

            string xpathCardId = "status/id";
            var cardId = xmlDoc.SelectNodes(xpathCardId)[0].InnerText;
            
            return new RfidReaderStatus() { LastReadId = cardId, NumberOfReadId = numberOfRead };
        }
    }
}
