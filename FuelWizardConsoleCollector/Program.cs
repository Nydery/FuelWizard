using System;
using ByIconic.FuelWizard;

namespace FuelWizardConsoleCollector
{
    class Program
    {
        static void Main(string[] args)
        {
            FuelWizardDataCollector collector = new FuelWizardDataCollector();
            collector.OnDataCollected += Collector_OnDataCollected;
            collector.StartCollectingData(new TimeSpan(0, 0, 1), false);

            Console.ReadKey();
        }

        private static void Collector_OnDataCollected(object sender, int gasStationId, string fuelType, double price)
        {
            Console.WriteLine("Notified");
        }
    }
}
