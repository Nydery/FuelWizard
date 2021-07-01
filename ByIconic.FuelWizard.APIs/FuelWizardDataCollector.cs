using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace ByIconic.FuelWizard
{
    public class FuelWizardDataCollector
    {
        private Thread collectingThread = null;
        private volatile bool isCollecting = false;

        public bool IsCollecting => isCollecting;

        public delegate void DataCollected(object sender, int gasStationId, string fuelType, double price);
        public event DataCollected OnDataCollected;
        public bool StartCollectingData(TimeSpan delay, bool startNow)
        {
            if (isCollecting)
                return false;

            isCollecting = true;
            collectingThread = new Thread(() => CollectData(delay, startNow));
            collectingThread.IsBackground = true;
            collectingThread.Start();

            Debug.WriteLine("StartCollectingData() : Started collecting");

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

            while(isCollecting)
            {
                List<int> gasStationIds = new List<int>() { 1, 2};
                //Get Locations and Gas Stations from DB and collect corresponding gas and diesel prices using the APIs.

                foreach (int i in gasStationIds)
                {
                    double latitude, longitude;
                    latitude = longitude = 0;
                    //Fetch coordinates from Geocode API

                    double price = 0;
                    //Fetch price of gasstation from E-Control API

                    OnDataCollected?.Invoke(this, i, "haven't thought about that yet", price);
                }


                DelayThread(delay);
            }
        }

        private void DelayThread(TimeSpan delay)
        {
            Debug.WriteLine($"Data collection delayed by {delay.TotalSeconds} seconds");
            Thread.Sleep(delay);
        }
    }
}
