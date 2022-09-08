using System;
using System.IO.Ports;
using System.Threading;
using System.Threading.Tasks;

namespace FlukeConnect_Fluke289
{
    public class CommandSerialPort : IDisposable
    {
        private readonly SerialPort port;

        /// <summary>
        /// Timeout in milliseconds
        /// </summary>
        private const int Timeout = 500;
        SemaphoreSlim semaphore;

        // Further work
        // Add retry functionality. Then throw error.

        public CommandSerialPort(SerialPort port)
        {
            // port initializing here
            this.port = port;
            //port = new SerialPort("COM9",115200);

            semaphore = new SemaphoreSlim(1, 1);


            port.Open();
        }

        public async Task<CommandResult> RequestCommand(string command)
        {
            // set up the task completion source
            var tcs = new TaskCompletionSource<CommandResult>();

            // handler of DataReceived event of port
            var handler = default(SerialDataReceivedEventHandler);
            handler = (sender, eventArgs) =>
            {
                try
                {
                    CommandResult result = new CommandResult();
                    try
                    {
                        result.ResponseState = (CommandResult.ResponseStates)Convert.ToInt32(port.ReadTo("\r"));
                    }
                    catch (Exception)
                    {
                        result.ResponseState = CommandResult.ResponseStates.ExecutionError;
                    }
                    
                    if (result.ResponseState == CommandResult.ResponseStates.OK)
                    {
                        result.ResponseString = port.ReadTo("\r");
                    }


                    tcs.SetResult(result);

                }
                finally
                {
                    port.DataReceived -= handler;
                }
            };
            port.DataReceived += handler;

            // Wait for semaphore before sending request;
            //bool SuccessfulEnter = await semaphore.WaitAsync(1000);
            bool ComportAvailable = semaphore.Wait(1000);

            if (!port.IsOpen) // If port is closed, abort immediately.
            {
                throw new OperationCanceledException("The port has been closed");
            }
            if (!ComportAvailable)
            {
                throw new TimeoutException("Semaphore on serial port not available");
            }
            //semaphore.Wai
            //bool SuccessfulEnter = await semaphore.WaitAsync(1000);

            // send request
            if (command[command.Length - 1] == '\r') command = command.Remove(command.Length - 1);
            port.Write(command + "\r");


            bool inTime = await Task.WhenAny(tcs.Task, Task.Delay(Timeout)) == tcs.Task;

            //Release semaphore here;
            
            semaphore.Release();

            if (inTime)
            {
                return tcs.Task.Result;
            }
            else
            {
                port.DataReceived -= handler;
                throw new TimeoutException("Receive Timeout has expired (BV)");
            }
        }

        public void Dispose()
        {
            port?.Dispose();
        }
    }
}
