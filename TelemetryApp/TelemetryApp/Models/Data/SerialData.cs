﻿using System.IO.Ports;
using System.Threading;

namespace TelemetryApp.Models.Data
{
    public class SerialData : Data
    {
        // Set up sizes for receiving serial data
        private const int DataSize = 88; // bytes

        private const int ValidateSize = 2; // bytes
        // private const int PacketSize = DataSize + ValidateSize; // bytes

        // Synchronization characters for receiving serial data
        private const char Syn0 = '_';
        private const char Syn1 = 'c';

        // Allocate buffer for storing most recently received serial data
        private static readonly byte[] DataBuffer = new byte[DataSize];

        // Serial port to receive data from
        private readonly SerialPort _port;
        private readonly Thread _updateDataBufferThread;

        private bool _updateDataBuffer;

        public SerialData(string portName)
        {
            _port = new SerialPort
            {
                // Serial port properties
                PortName = portName,
                BaudRate = 9600,
                DataBits = 8,
                StopBits = StopBits.One,
                Parity = Parity.None,
                // Read/Write timeouts
                ReadTimeout = 500,
                WriteTimeout = 500
            };

            _port.Open();

            _updateDataBuffer = true;
            _updateDataBufferThread = new Thread(UpdateDataBuffer);
            _updateDataBufferThread.Start();

            Steering = new Steering(() => DataBuffer[SteeringIndex]);
            GForce = new GForce.GForce(() => DataBuffer[GForceXIndex], () => DataBuffer[GForceYIndex]);
            Gps = new Gps.Gps(() => DataBuffer[GpsLatitudeIndex], () => DataBuffer[GpsLongitudeIndex]);
            FrontLeftTire = new Tire.Tire(() => DataBuffer[FrontLeftTireTemperatureIndex],
                () => DataBuffer[FrontLeftTirePressureIndex]);
            FrontRightTire = new Tire.Tire(() => DataBuffer[FrontRightTireTemperatureIndex],
                () => DataBuffer[FrontRightTirePressureIndex]);
            BackLeftTire = new Tire.Tire(() => DataBuffer[BackLeftTireTemperatureIndex],
                () => DataBuffer[BackLeftTirePressureIndex]);
            BackRightTire = new Tire.Tire(() => DataBuffer[BackRightTireTemperatureIndex],
                () => DataBuffer[BackRightTirePressureIndex]);
        }

        public override void Unload()
        {
            _updateDataBuffer = false;
            if (_updateDataBufferThread != null && _updateDataBufferThread.IsAlive)
                _updateDataBufferThread.Join();
            if (_port.IsOpen)
                _port.Close();
        }

        public override void Update()
        {
            Steering.Update();
            GForce.Update();
            Gps.Update();
            FrontLeftTire.Update();
            FrontRightTire.Update();
            BackLeftTire.Update();
            BackRightTire.Update();
        }

        public void UpdateDataBuffer()
        {
            if (!_port.IsOpen) _port.Open();
            while (_updateDataBuffer)
            {
                while (_port.ReadByte() != Syn0 && _port.ReadByte() != Syn1)
                {
                }

                _port.Read(DataBuffer, 0, DataSize);
            }
        }

        #region Property Array Indexes

        private const int SteeringIndex = 0;
        private const int GForceXIndex = 0;
        private const int GForceYIndex = 0;
        private const int GpsLatitudeIndex = 0;
        private const int GpsLongitudeIndex = 0;
        private const int FrontLeftTireTemperatureIndex = 0;
        private const int FrontRightTireTemperatureIndex = 0;
        private const int BackLeftTireTemperatureIndex = 0;
        private const int BackRightTireTemperatureIndex = 0;
        private const int FrontLeftTirePressureIndex = 0;
        private const int FrontRightTirePressureIndex = 0;
        private const int BackLeftTirePressureIndex = 0;
        private const int BackRightTirePressureIndex = 0;

        #endregion Array indexes for properties

        //private async Task GetSerialPortCollection()
        //{
        //    var aqsFilter = SerialDevice.GetDeviceSelector();
        //    var deviceInfoCollection = await DeviceInformation.FindAllAsync(aqsFilter);
        //    var deviceCollection = new ObservableCollection<string>();

        //    foreach (var device in deviceInfoCollection)
        //    {
        //        var d = await SerialDevice.FromIdAsync(device.Id);
        //        var portName = d.PortName;
        //        deviceCollection.Add(portName);
        //    }
        //}
    }
}