﻿using System;
using System.Collections.ObjectModel;
using TelemetryApp.Models;
using TelemetryApp.Models.GForce;
using TelemetryApp.Models.GPS;
using Windows.UI.Xaml;

namespace TelemetryApp.ViewModels
{
    public class ViewModel : NotifyPropertyChanged
    {
        // Timer for calling Update
        private readonly DispatcherTimer _timer;

        public Settings SettingsModel { get; private set; }

        public Serial SerialModel { get; private set; }

        // Update Period in milliseconds
        public int UpdatePeriod { get; private set; }

        // Steering
        public SteeringWheel SteeringWheelModel { get; private set; }

        // G-Force
        public GForce GForceModel { get; private set; }

        // Tire PSI
        public Pressure PressureFrontLeftTireModel { get; private set; }
        public Pressure PressureFrontRightTireModel { get; private set; }
        public Pressure PressureBackLeftTireModel { get; private set; }
        public Pressure PressureBackRightTireModel { get; private set; }

        // Tire Temperature
        public Temperature TemperatureFrontLeftTireModel { get; private set; }
        public Temperature TemperatureFrontRightTireModel { get; private set; }
        public Temperature TemperatureBackLeftTireModel { get; private set; }
        public Temperature TemperatureBackRightTireModel { get; private set; }

        // GPS
        public Latitude LatitudeModel { get; private set; }
        public Longitude LongitudeModel { get; private set; }

        // Graphs
        public ObservableCollection<Graph> Graphs { get; private set; }

        private Graph _currentGraph;
        public Graph CurrentGraph
        {
            get => _currentGraph;
            set
            {
                _currentGraph = value;
                OnPropertyChanged(nameof(CurrentGraph));
            }
        }

        public ViewModel()
        {
            Init();
            // Timer for updating once a second
            _timer = new DispatcherTimer() { Interval = TimeSpan.FromMilliseconds(UpdatePeriod) };
            _timer.Tick += Tick;
        }

        private void Init()
        {
            // Update Period
            UpdatePeriod = 500;
            
            // Settings
            SettingsModel = new Settings();

            // Provide access to serial data
            SerialModel = new Serial(SettingsModel.SerialPortName, SettingsModel.SerialBaudRate);

            // SteeringWheel
            SteeringWheelModel = new SteeringWheel();
            // GForce
            GForceModel = new GForce();
            // Pressure
            PressureFrontLeftTireModel = new Pressure();
            PressureFrontRightTireModel = new Pressure();
            PressureBackLeftTireModel = new Pressure();
            PressureBackRightTireModel = new Pressure();
            // Temperature
            TemperatureFrontLeftTireModel = new Temperature();
            TemperatureFrontRightTireModel = new Temperature();
            TemperatureBackLeftTireModel = new Temperature();
            TemperatureBackRightTireModel = new Temperature();
            // GPS
            LatitudeModel = new Latitude();
            LongitudeModel = new Longitude();

            // Graphs
            Graphs = new ObservableCollection<Graph>();
            CurrentGraph = new Graph(
                () => Data.RandomDouble(0, 100),
                "Random",
                maximum: 100
            );
            Graphs.Add(_currentGraph);
            //var fibonacci = Data.FibonacciRange(0, 10);
            //Graphs.Add(new GraphViewModel(
            //    () => new DataPoint<double>(Data.EnumerateInteger(fibonacci, loop: true)),
            //    "Fibonacci",
            //    maximum: 5000
            //));
            Graphs.Add(new Graph(
                () => 50,
                "Constant",
                maximum: 100
            ));
            //var csvEnum = new DataPoints<string>(Csv.CsvReader.ReadFromText("Assets/Data/short.csv"));
            //Graphs.Add(new GraphViewModel(
            //    () => new DataPoint<double>(Data.EnumerateDouble(csvEnum["Steering Position [Deg]"], loop: true)),
            //    "CSV Data Loop",
            //    minimum: -100, maximum: 100
            //));
        }

        public void Start()
        {
            _timer.Start();
        }

        public void Stop()
        {
            _timer.Stop();
        }

        protected void Tick(object sender, object e)
        {
            Update();
        }

        public void Update()
        {
            // Steering
            SteeringWheelModel.Update();
            // GForce
            GForceModel.Update();
            // Pressure
            PressureFrontLeftTireModel.Update();
            PressureFrontRightTireModel.Update();
            PressureBackLeftTireModel.Update();
            PressureBackRightTireModel.Update();
            // Temperature
            TemperatureFrontLeftTireModel.Update();
            TemperatureFrontRightTireModel.Update();
            TemperatureBackLeftTireModel.Update();
            TemperatureBackRightTireModel.Update();
            // GPS
            LatitudeModel.Update();
            LongitudeModel.Update();
            // Graphs
            foreach (var graph in Graphs) graph.Update();
            // Notify all properties have changed as mentioned here:
            // https://docs.microsoft.com/en-us/dotnet/api/system.componentmodel.inotifypropertychanged.propertychanged?redirectedfrom=MSDN&view=netframework-4.7.2#remarks
            OnPropertyChanged(null);
        }

        public void UpdateDataSource()
        {
            switch (SettingsModel.SelectedDataSource)
            {
                case DataSource.Serial:
                    SetSerialDataSource();
                    break;
                case DataSource.Csv:
                    SetRandomDataSource();
                    break;
                default:
                    SetRandomDataSource();
                    break;
            }
        }

        public void SetRandomDataSource()
        {
            SettingsModel.SelectedDataSource = DataSource.Random;
            // SteeringWheel
            SteeringWheelModel.DataGenerator = SteeringWheel.Default;
            // GForce
            GForceModel.X.DataGenerator = GForceAxis.Default;
            GForceModel.Y.DataGenerator = GForceAxis.Default;
            // Pressure
            PressureFrontLeftTireModel.DataGenerator = Pressure.Default;
            PressureFrontRightTireModel.DataGenerator = Pressure.Default;
            PressureBackLeftTireModel.DataGenerator = Pressure.Default;
            PressureBackRightTireModel.DataGenerator = Pressure.Default;
            // Temperature
            TemperatureFrontLeftTireModel.DataGenerator = Temperature.Default;
            TemperatureFrontRightTireModel.DataGenerator = Temperature.Default;
            TemperatureBackLeftTireModel.DataGenerator = Temperature.Default;
            TemperatureBackRightTireModel.DataGenerator = Temperature.Default;
            // GPS
            LatitudeModel.DataGenerator = Latitude.Default;
            LongitudeModel.DataGenerator = Longitude.Default;
        }

        public void SetSerialDataSource()
        {
            SettingsModel.SelectedDataSource = DataSource.Serial;
            // SteeringWheel
            SteeringWheelModel.DataGenerator = SerialModel.GetData;
            // GForce
            GForceModel.X.DataGenerator = SerialModel.GetData;
            GForceModel.Y.DataGenerator = SerialModel.GetData;
            // Pressure
            PressureFrontLeftTireModel.DataGenerator = SerialModel.GetData;
            PressureFrontRightTireModel.DataGenerator = SerialModel.GetData;
            PressureBackLeftTireModel.DataGenerator = SerialModel.GetData;
            PressureBackRightTireModel.DataGenerator = SerialModel.GetData;
            // Temperature
            TemperatureFrontLeftTireModel.DataGenerator = SerialModel.GetData;
            TemperatureFrontRightTireModel.DataGenerator = SerialModel.GetData;
            TemperatureBackLeftTireModel.DataGenerator = SerialModel.GetData;
            TemperatureBackRightTireModel.DataGenerator = SerialModel.GetData;
            // GPS
            LatitudeModel.DataGenerator = SerialModel.GetData;
            LongitudeModel.DataGenerator = SerialModel.GetData;
        }

        public void SetCsvDataSource()
        {
            SettingsModel.SelectedDataSource = DataSource.Csv;
            //// SteeringWheel
            //SteeringWheelModel.DataGenerator = SteeringWheel.Default;
            //// GForce
            //GForceXModel.DataGenerator = GForce.Default;
            //GForceYModel.DataGenerator = GForce.Default;
            //// Pressure
            //PressureFrontLeftTireModel.DataGenerator = Pressure.Default;
            //PressureFrontRightTireModel.DataGenerator = Pressure.Default;
            //PressureBackLeftTireModel.DataGenerator = Pressure.Default;
            //PressureBackRightTireModel.DataGenerator = Pressure.Default;
            //// Temperature
            //TemperatureFrontLeftTireModel.DataGenerator = Temperature.Default;
            //TemperatureFrontRightTireModel.DataGenerator = Temperature.Default;
            //TemperatureBackLeftTireModel.DataGenerator = Temperature.Default;
            //TemperatureBackRightTireModel.DataGenerator = Temperature.Default;
            //// GPS
            //LatitudeModel.DataGenerator = Latitude.Default;
            //LongitudeModel.DataGenerator = Longitude.Default;
        }
    }
}
