using System;
using System.Globalization;

namespace FlukeConnect_Fluke289
{
    public class FlukeData_Measurement
    {
        public double Measurement { get; set; }
        public string BaseUnit { get; set; }
        public MeasurementStates MeasurementState { get; set; }
        public MeasurementAttributes MeasurementAttribute { get; set; }

        public enum MeasurementStates
        {
            INVALID,
            NORMAL,
            BLANK,
            DISCHARGE,
            OL,
            OL_MINUS,
            OPEN_TC
        }
        public enum MeasurementAttributes
        {
            NONE,
            OPEN_CIRCUIT,
            SHORT_CIRCUIT,
            GLITCH_CIRCUIT,
            GOOD_DIODE,
            LO_OHMS,
            NEGATIVE_EDGE,
            POSITIVE_EDGE,
            HIGH_CURRENT
        }
        public FlukeData_Measurement(string indata)
        {
            // 0.000E-3,VDC,NORMAL,NONE
            string[] parts = indata.Split(',');
            //try
            //{
                Measurement = Double.Parse(parts[0], System.Globalization.NumberStyles.Float,CultureInfo.InvariantCulture);
                BaseUnit = parts[1];
                MeasurementState = (MeasurementStates)Enum.Parse(typeof(MeasurementStates), parts[2],true);
                MeasurementAttribute = (MeasurementAttributes)Enum.Parse(typeof(MeasurementAttributes), parts[3], true);


            //}
            //catch (ArgumentException ex)
            //{

            //    throw new ArgumentException(;
            //}

        }
    }
}