using ByIconic.FuelWizard.APIOperations;
using ByIconic.FuelWizard.DatabaseOperations;
using log4net;
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

        private DateTime lastExecution;

        public delegate void DataCollected(object sender, int gasStationId, string fuelType, double price, DateTime time);
        public event DataCollected OnDataCollected;
        public event EventHandler OnDataCollectionStarted;
        public event EventHandler OnDataCollectionStopped;

        private static readonly ILog log = LogManager.GetLogger(typeof(FuelWizardDataCollector));

        public bool StartCollectingData(TimeSpan delay)
        {
            if (isCollecting)
                return false;

            isCollecting = true;
            collectingThread = new Thread(() => CollectData(delay));
            collectingThread.IsBackground = true;
            collectingThread.Start();

            log.Info("StartCollectingData() : Started thread");

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
                log.Info("StopCollectingData() : Thread abort threw exception");
            }

            isCollecting = false;

            log.Info("StopCollectingData() : Collecting stopped");
        }

        private void CollectData(TimeSpan delay)
        {
            //WaitForNextRepetition(delay, true);

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
                    log.Error("Failed to connect to Database", e);
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
                    List<GasStationPublic> gasStations;
                    try
                    {
                        gasStations = new List<GasStationPublic>(FuelWizardEControlAdapter.FetchGasStationsOfLocation(l));
                    }
                    catch (Exception e)
                    {
                        log.Error($"Fetching gasstations from API failed for location:  {l.postalCode} {l.city}, {l.address}", e);
                        continue;
                    }
                    

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


                WaitForNextRepetition(delay);
            }

            OnDataCollectionStopped?.Invoke(this, EventArgs.Empty);
        }

        private void WaitForNextRepetition(TimeSpan delay, bool firstMethodCall = false)
        {
            if(firstMethodCall)
            {
                DateTime dateTime = DateTime.Now;

                //Change DateTime to full hour
                dateTime = dateTime.AddSeconds(-dateTime.Second);
                dateTime = dateTime.AddMinutes(-dateTime.Minute);
                dateTime = dateTime.AddMilliseconds(-dateTime.Millisecond);

                //Add 1 hour, to reach next full hour
                dateTime = dateTime.AddHours(1);

                //No need to check if the time is on the next day or not, bc DateTime handles that itself :)

                WaitForDateTime(dateTime);        //Comment out for debugging
                lastExecution = dateTime;


                //Put this in for debugging
                //lastExecution = DateTime.Now;
            }
            else
            {
                DateTime dateTime = lastExecution.Add(delay);
                WaitForDateTime(dateTime);
                lastExecution = dateTime;
            }
        }

        private void WaitForDateTime(DateTime dateTime)
        {
            TimeSpan timeToWait = dateTime - DateTime.Now;
            DelayThread(timeToWait);
        }

        private void DelayThread(TimeSpan delay)
        {
            log.Info($"Data collection delayed by {delay.TotalSeconds} seconds (until {DateTime.Now.AddSeconds(delay.TotalSeconds)})");
            Thread.Sleep(delay);
        }
    }
}
