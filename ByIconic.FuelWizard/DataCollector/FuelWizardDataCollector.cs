﻿using ByIconic.FuelWizard.APIOperations;
using ByIconic.FuelWizard.DatabaseOperations;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using static ByIconic.FuelWizard.Models.API.EControlClasses;

namespace ByIconic.FuelWizard.DataCollector
{
    public class FuelWizardDataCollector
    {
        private Thread collectingThread = null;
        private volatile bool isCollecting = false;

        public bool IsCollecting => isCollecting;

        public delegate void DataCollected(object sender, int gasStationId, string fuelType, double price, DateTime time);
        public event DataCollected OnDataCollected;
        public event EventHandler OnDataCollectionStarted;
        public event EventHandler OnDataCollectionStopped;

        public bool StartCollectingData(TimeSpan delay, bool startNow = true)
        {
            if (isCollecting)
                return false;

            isCollecting = true;
            collectingThread = new Thread(() => CollectData(delay, startNow));
            collectingThread.IsBackground = true;
            collectingThread.Start();

            Debug.WriteLine("StartCollectingData() : Started thread");

            return true;
        }

        public void StopCollectingData()
        {
            if (!isCollecting || collectingThread == null)
                return;

            try
            {
                //collectingThread.Abort();
            }
            catch (ThreadAbortException)
            {
                Debug.WriteLine("StopCollectingData() : Thread abort threw exception");
            }

            isCollecting = false;

            Debug.WriteLine("StopCollectingData() : Collecting stopped");
        }

        private void CollectData(TimeSpan delay, bool startNow)
        {
            int execCount = 0;

            if (!startNow)
                DelayThread(delay);

            OnDataCollectionStarted?.Invoke(this, EventArgs.Empty);

            while (isCollecting)
            {
                List<Location> locations;

                try
                {
                    IEnumerable<Location> fetchedLocations = FuelWizardDatabaseConnector.GetLocations();
                    locations = new List<Location>(fetchedLocations);
                }catch (Exception e)
                {
                    Debug.WriteLine($"Failed to connect to Database ({e.Message})");
                    StopCollectingData();
                    OnDataCollectionStopped?.Invoke(this, EventArgs.Empty);
                    return;
                }
                
                //Get Locations from DB and collect gas and diesel prices of 
                // corresponding gasstations using the APIs.

                foreach (Location l in locations)
                {
                    //Fetch the 5 nearest gasstations of Location l, 
                    // including their prices for diesel and gasoline
                    List<GasStationPublic> gasStations = new List<GasStationPublic>(FuelWizardEControlAdapter.FetchGasStationsOfLocation(l));

                    foreach(GasStationPublic gasStation in gasStations)
                    {   
                        if(!FuelWizardDatabaseConnector.ExistsGasStation(gasStation.id))
                        {
                            FuelWizardDatabaseConnector.InsertGasStation(gasStation);
                        }

                        if(gasStation.prices.Length > 0)
                        {
                            string fuelType = gasStation.prices[0].fuelType;
                            double price = gasStation.prices[0].amount;

                            FuelWizardDatabaseConnector.InsertPriceData(gasStation.id, fuelType, DateTime.Now, price);
                            OnDataCollected?.Invoke(this, gasStation.id, fuelType, price, DateTime.Now);
                        }
                    }
                }


                DelayThread(delay);
                execCount++;
            }

            OnDataCollectionStopped?.Invoke(this, EventArgs.Empty);
        }

        private void DelayThread(TimeSpan delay)
        {
            Debug.WriteLine($"Data collection delayed by {delay.TotalSeconds} seconds");
            Thread.Sleep(delay);
        }
    }
}
