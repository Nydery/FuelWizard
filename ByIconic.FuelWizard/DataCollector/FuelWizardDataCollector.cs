using ByIconic.FuelWizard.DatabaseOperations;
using ByIconic.FuelWizard.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;

namespace ByIconic.FuelWizard.DataCollector
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

            while (isCollecting)
            {
                List<Location> locations = new List<Location>(FuelWizardDatabaseConnector.GetLocations());
                //Get Locations from DB and collect gas and diesel prices of 
                // corresponding gasstations using the APIs.

                foreach (Location l in locations)
                {
                    //Fetch the 5 nearest gasstations of Location l, 
                    // including their prices for diesel and gasoline

                    OnDataCollected?.Invoke(this, -1, "haven't thought about that yet", -1);
                }


                DelayThread(delay);
                execCount++;
            }
        }

        private void DelayThread(TimeSpan delay)
        {
            Debug.WriteLine($"Data collection delayed by {delay.TotalSeconds} seconds");
            Thread.Sleep(delay);
        }
    }
}
