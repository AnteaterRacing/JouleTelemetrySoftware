﻿using TelemetryApp.Models.DataPoint;

namespace TelemetryApp.Models
{
    public class Pressure : DataPointDelegate<double>
    {
        public Pressure() : base(() => Data.RandomDouble(0, 100))
        {
        }

        public Pressure(DataDelegate dataGenerator) : base(dataGenerator)
        {
        }

        public override string ToString()
        {
            return $"{Value:N2} PSI";
        }
    }
}