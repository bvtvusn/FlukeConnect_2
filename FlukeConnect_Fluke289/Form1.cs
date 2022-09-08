using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace FlukeConnect_Fluke289
{
    public partial class Form1 : Form
    {
        DAL dal;
        BLL bll;

        //CommandSerialPort testSerial;
        public Form1()
        {
            InitializeComponent();

            dal = new DAL();
            bll = new BLL(dal);

            bll.InformationMEssage += Bll_InformationMEssage;
            bll.ValueUpdatedEvent += Bll_ValueUpdatedEvent;
            
            
            ListSerialPorts();
        }

        private void FlukeInstr_ConnectionStatusChangedEvent(bool obj)
        {
            DisplayConnectionStatus(obj);

            if (!obj)
            {
                toolStripStatusLabel1.Text = "Connection Lost";
            }
            else
            {
                toolStripStatusLabel1.Text = "Connection Established";
            }

            
        }

        private void Bll_ValueUpdatedEvent(FlukeData_Measurement data)
        {
            //FlukeData_Measurement data = await bll.flukeInstr.GetMeasurementAsync();

            string humanReadable = Extensions.Nice(data.Measurement, 5);

            BeginInvoke((Action)(() =>
            {
            txtMeasurement.Text = humanReadable; // data.Measurement.ToString();
            lblUnit.Text = data.BaseUnit;
            lblState.Text = data.MeasurementState.ToString();
            lblAttribute.Text = data.MeasurementAttribute.ToString();

            }));
        }

        private void Bll_InformationMEssage(string obj)
        {
            toolStripStatusLabel1.Text = obj;
        }


        private void ListSerialPorts()
        {
            string[] ports = dal.GetPorts();

            cmbComPort.Items.Clear();
            for (int i = 0; i < ports.Length; i++)
            {
                cmbComPort.Items.Add(ports[i].ToString());
            }
            if (cmbComPort.Items.Count > 0) cmbComPort.SelectedIndex = 0;
        }
        private async void btnConnect_Click(object sender, EventArgs e)
        {
            if (bll.flukeInstr == null)
            {
                // Try to Connect
                if (cmbComPort.Items.Count > 0)
                {
                    ShowAsConnect();

                    try
                    {
                        bll.CreateInstrument((string)cmbComPort.SelectedItem);
                        bll.flukeInstr.ConnectionStatusChangedEvent += FlukeInstr_ConnectionStatusChangedEvent;

                        bool conState = await bll.flukeInstr.CheckConnectionAsync();
                        DisplayConnectionStatus(conState);


                        FlukeData_DeviceInfo info = await bll.flukeInstr.GetDeviceInfoAsync();
                        txtModel.Text = info.Model;
                        txtSerialNum.Text = info.SerialNumber;
                        txtSoftwareVersion.Text = info.SoftwareVerion;

                    }
                    catch (Exception er)
                    {
                        Disconect();
                        MessageBox.Show(er.Message);
                    }
                }
                else
                {
                    toolStripStatusLabel1.Text = "No Serial ports available";
                }
            }
            else // Disconnect from instrument
            {
                Disconect();
            }
        }

        private void Disconect()
        {
            bll.Disconnect();
            btnConnect.Text = "Connect";
            DisplayConnectionStatus(false);
            toolStripStatusLabel1.Text = "Disconnected";
            cmbComPort.Enabled = true;
        }
        private void ShowAsConnect()
        {
            btnConnect.Text = "Disconnect";
            cmbComPort.Enabled = false;
        }
            private void DisplayConnectionStatus(bool isConnected)
        {
            BeginInvoke((Action)(() =>
            {
                if (isConnected)
                {
                    panConnectionStatus.BackColor = Color.ForestGreen;
                    lblConnectionStatus.Text = "Connected";
                }
                else
                {
                    panConnectionStatus.BackColor = Color.Red;
                    lblConnectionStatus.Text = "Not Connected";
                }
            }));
            
        }


        private void Form1_Load(object sender, EventArgs e)
        {
            DisplayConnectionStatus(false);
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

    }
}
