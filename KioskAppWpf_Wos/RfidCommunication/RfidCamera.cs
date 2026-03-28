using RfidCommunication.Enums;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RfidCommunication
{
    public class RfidCamera
    {
        private SerialPort serialPort;
        private Queue<byte> recievedData = new Queue<byte>();

        public RfidReaderStatus CheckCameraCom(ReaderLocation configItem, RfidReaderConfig reader, RfidReaderStatus readerStatus)
        {
            if (serialPort != null && serialPort.IsOpen)
            {
                if (recievedData.Count > 0)
                {
                    var packet = Enumerable.Range(0, recievedData.Count).Select(i => recievedData.Dequeue());
                    string licPlate = PacketToLicensePlate(packet.ToArray());
                    recievedData.Clear();

                    if (licPlate != "")
                        return new RfidReaderStatus() { LastReadLicensePlate = licPlate, NumberOfReadId = readerStatus.NumberOfReadId++ };
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

        private void SerialPortDataReceived(object s, SerialDataReceivedEventArgs e)
        {
            byte[] data = new byte[serialPort.BytesToRead];
            serialPort.Read(data, 0, data.Length);
            data.ToList().ForEach(b => recievedData.Enqueue(b));
        }
        
        private string PacketToLicensePlate(byte[] packet)
        {
            var licPlate = "";
            if (packet.Length > 0)
            {
                var licString = BitConverter.ToString(packet);
                var licAscii = ConvertHex(licString.Replace("-", String.Empty));
                if (licAscii.Contains("\r\n\0"))
                    licPlate = licAscii.Substring(0, licAscii.IndexOf("\r\n\0"));
            }
            return licPlate;
        }

        public string ConvertHex(String hexString)
        {
            try
            {
                string ascii = string.Empty;

                for (int i = 0; i < hexString.Length; i += 2)
                {
                    String hs = string.Empty;

                    hs = hexString.Substring(i, 2);
                    uint decval = System.Convert.ToUInt32(hs, 16);
                    char character = System.Convert.ToChar(decval);
                    ascii += character;

                }

                return ascii;
            }
            catch (Exception ex) {  }

            return string.Empty;
        }

        private void Dispose()
        {
            if (serialPort != null)
            {
                serialPort.Dispose();
            }
        }
    }
}
