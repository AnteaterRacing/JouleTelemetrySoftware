﻿namespace TelemetryApp.Models.GPS
{
    public class Longitude : DecimalDegree
    {
        public Longitude() : base(() => Data.RandomDouble(-180, 180))
        {
        }

        public Longitude(DataDelegate dataGenerator) : base(dataGenerator)
        {
        }

        public override string ToString()
        {
            if (Value == 0) return "0 °";
            else if (Value < 0) return $"{base.ToString()} W";
            else return $"{base.ToString()} E";
        }
    }
}