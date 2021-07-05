using ByIconic.FuelWizard.DataCollector;
using System;

namespace FuelWizardConsoleCollector
{
    class Program
    {
        static void Main(string[] args)
        {
            FuelWizardDataCollector collector = new FuelWizardDataCollector();
            collector.OnDataCollected += Collector_OnDataCollected;
            collector.StartCollectingData(new TimeSpan(0, 0, 10));

            Console.ReadKey();
        }

        private static void Collector_OnDataCollected(object sender, int gasStationId, string fuelType, double price, DateTime time)
        {
            Console.WriteLine($"[{time}] {gasStationId}: {fuelType} {price}");
        }
    }
}
