namespace FlukeConnect_Fluke289
{
    public class FlukeData_DeviceInfo
    {
        public string Model { get; set; }
        public string SerialNumber { get; set; }
        public string SoftwareVerion { get; set; }

        public FlukeData_DeviceInfo(string responseString)
        {
            string[] items = responseString.Split(',');
            Model = items[0];
            SerialNumber = items[2];
            SoftwareVerion = items[1];
        }
    }
}