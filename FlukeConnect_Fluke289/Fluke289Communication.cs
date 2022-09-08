using System;
using System.IO.Ports;
using System.Threading.Tasks;
using System.Timers;

namespace FlukeConnect_Fluke289
{
    public class Fluke289Communication : IDisposable
    {
        CommandSerialPort commandInterface;
        private bool connectedState;

        public bool ConnectedState
        {
            get { return connectedState; }
            set {
                if (value != connectedState)
                {
                    
                    ConnectionStatusChangedEvent?.Invoke(value);
                }
                connectedState = value; }
        }


        public event Action<bool> ConnectionStatusChangedEvent;
        public Fluke289Communication(string ComPort)
        {
            SerialPort ser = new SerialPort(ComPort);
            ser.BaudRate = 115200;
            ser.Parity = Parity.None;
            ser.DataBits = 8;
            ser.StopBits = StopBits.One;

            commandInterface = new CommandSerialPort(ser);
                       
        }

        private async Task<CommandResult> RunCommandAndCheckState(string command)
        {
            try
            {
                CommandResult res = await commandInterface.RequestCommand(command);
                //if (res.ResponseString == null)
                //{
                //    res.ResponseState = CommandResult.ResponseStates.ExecutionError;
                //}
                //FlukeData_DeviceInfo info = await GetDeviceInfoAsync();
                return res;
            }
            catch (OperationCanceledException)
            {
                // Thw port has most likely been closed. Let the last waiting calls be forgotten.
                CommandResult com = new CommandResult();
                com.ResponseState = CommandResult.ResponseStates.ExecutionError;
                return new CommandResult();
            }
            catch (TimeoutException)
            {
                CommandResult com = new CommandResult();
                com.ResponseState = CommandResult.ResponseStates.ExecutionError;
                return new CommandResult();
            }
            catch (Exception)
            {
                ConnectedState = false;
                throw;
            }
        }

        public async void RefreshConnectionStateAsync()
        {
            ConnectedState = await CheckConnectionAsync();
        }

        public async Task<bool> CheckConnectionAsync()
        {
            try
            {
                FlukeData_DeviceInfo info = await GetDeviceInfoAsync();
                return info.Model.StartsWith("FLUKE");
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<FlukeData_DeviceInfo> GetDeviceInfoAsync()
        {
            //CommandResult res = await commandInterface.GetCommand("id");
            CommandResult res = await RunCommandAndCheckState("id");
            if (res.ResponseState == CommandResult.ResponseStates.OK && res.ResponseString != null)
            {
                try
                {
                    FlukeData_DeviceInfo idData = new FlukeData_DeviceInfo(res.ResponseString);
                    return idData;

                }
                catch (Exception)
                {

                    throw;
                }
            }
            else
            {
                throw new Exception("Id Not Available");
            }
            
        }

        internal async Task<FlukeData_Measurement> GetMeasurementAsync()
        {
            //CommandResult res = await commandInterface.GetCommand("qm");
            CommandResult res = await RunCommandAndCheckState("qm");
            if (res.ResponseState == CommandResult.ResponseStates.OK && res.ResponseString != null)
            {
                try
                {
                    FlukeData_Measurement dataMeas = new FlukeData_Measurement(res.ResponseString);
                    return dataMeas;

                }
                catch (Exception)
                {

                    throw;
                }
            }
            else
            {
                throw new Exception("Measurement Not Available");
            }
            //return new Task<FlukeData_Measurement>();
        }

        public void Dispose()
        {
            commandInterface?.Dispose();
        }

    }
}
