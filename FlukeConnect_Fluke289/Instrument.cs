using System;
using System.IO.Ports;

namespace FlukeConnect_Fluke289
{
    internal class Instrument
    {
        public event Action<string> StringReceived;
        public SerialPort ser;
        DAL dal;
        public string Name { get; set; }
        public Instrument(DAL dal, string port)
        {
            this.dal = dal;
            ser = dal.ConnectSerial(port);

            ser.DataReceived += Ser_DataReceived;
            ser.Open();
            ////UpdateID();
        }

        public void UpdateID()
        {
            ser.Write("id\r\n");
        }

        private void Ser_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            
            while (ser.BytesToRead > 0)
            {
                string inData = ser.ReadTo("\r");
                StringReceived(inData);
            }
            

            //byte[] data = new byte[ser.BytesToRead];
            //ser.Read(data, 0, data.Length);
            //string text = System.Text.Encoding.ASCII.GetString(data);
            
        }

        internal void SendData(string text)
        {
            ser.Write(text + "\r");
        }
    }
}