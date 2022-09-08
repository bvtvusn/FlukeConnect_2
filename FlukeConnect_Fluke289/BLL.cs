using System;
using System.Timers;

namespace FlukeConnect_Fluke289
{
    internal class BLL
    {
        DAL dal;
        public Fluke289Communication flukeInstr;
        Timer connectionTimer;
        Timer pollTimer;
        public event Action<string> InformationMEssage;
        public event Action<FlukeData_Measurement> ValueUpdatedEvent;
        private bool pollingState;

        public bool PollingState
        {
            get { return pollingState; }
            set { 
                pollingState = value;
                pollTimer.Enabled = pollingState; // ON / Off
                
            
            }
        }

        public BLL(DAL dal)
        {
            //ConnectedState = true;
            this.dal = dal;

            connectionTimer = new Timer(1000);
            connectionTimer.Start();
            connectionTimer.Elapsed += ConnectionTimer_Elapsed;

            pollTimer = new Timer(250);
            pollTimer.Elapsed += PollTimer_Elapsed;
            pollTimer.Stop();
        }

        private async void PollTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (flukeInstr.ConnectedState == false)
            {
                pollTimer.Stop();
            }

            try
            {
                FlukeData_Measurement data = await flukeInstr.GetMeasurementAsync();
                ValueUpdatedEvent?.Invoke(data);
            }
            catch (Exception ex)
            {
                InformationMEssage?.Invoke(ex.Message);
                InformationMEssage?.Invoke("Polling Timeout");
            }
            
            
        }

        private void ConnectionTimer_Elapsed(object sender, ElapsedEventArgs e)
        {

            if (flukeInstr != null)
            {
                //flukeInstr.RefreshConnectionState();
                try
                {
                    flukeInstr.RefreshConnectionStateAsync();

                }
                catch (InvalidOperationException ex)
                {
                    InformationMEssage?.Invoke(ex.Message);
                }
                catch (TimeoutException ex)
                {
                    InformationMEssage?.Invoke(ex.Message);
                }
                catch(Exception ex)
                {
                    throw;
                }
            }
            

         }

        internal void CreateInstrument(string comport)
        {
            //instr = new Instrument(dal, comport);
            flukeInstr = new Fluke289Communication(comport);

            flukeInstr.ConnectionStatusChangedEvent += FlukeInstr_ConnectionStatusChangedEvent;

            //flukeInstr.StatusChangedEvent += FlukeInstr_StatusChangedEvent; 
        }

        private void FlukeInstr_ConnectionStatusChangedEvent(bool obj)
        {
            if (obj)
            {
                pollTimer.Start();
            }
        }

        internal void Disconnect()
        {
            pollTimer.Stop();

            flukeInstr.Dispose();
            flukeInstr = null;
        }
    }
}
