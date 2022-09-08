using System;
using System.Collections.Generic;
using System.IO.Ports;
//using System.IO.Ports;

namespace FlukeConnect_Fluke289
{
    internal class DAL
    {
        internal string[] GetPorts()
        {
            //SerialPort dw = new SerialPort()
            List<string> ports = new List<string>();
            foreach (string s in SerialPort.GetPortNames())
            {
                ports.Add(s);
            }
            return ports.ToArray();
        }

        public SerialPort ConnectSerial(string comPort)
        {
            SerialPort ser = new SerialPort(comPort);
            ser.BaudRate = 115200;
            ser.Parity = Parity.None;
            ser.DataBits = 8;
            ser.StopBits = StopBits.One;
            return ser;
        }
    }
}