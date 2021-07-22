using ByIconic.FuelWizard;
using ByIconic.FuelWizard.DataCollector;
using System;

namespace FuelWizardConsoleCollector
{
    class Program
    {
        static void Main(string[] args)
        {
            if(args.Length < 1)
            {
                Console.WriteLine("You must specify the collection delay. Usage: \"current.exe <delay in following format (hh:mm)>\"\nExample: " +
                    "\n\"FuelWizardConsoleCollector.exe 01:00\" for 1 hour delay");
                return;
            }

            TimeSpan delay;

            try
            {
                delay = TimeSpan.Parse(args[0]);
            }
            catch (FormatException)
            {
                Console.WriteLine("Delay is not parsable. Specify the delay in this format: hh:mm:ss!");
                return;
            }

            //Print current status
            Console.WriteLine("FuelWizard - DataCollector");
            Console.WriteLine(" - ByIconic 2021");
            Console.WriteLine($"Delay: {delay.Hours.ToString("00")}h {delay.Minutes.ToString("00")}min");
            Console.WriteLine("Starting: At next full hour");

            Console.WriteLine();

            FuelWizardDataCollector collector = new FuelWizardDataCollector();
            collector.OnDataCollected += Collector_OnDataCollected;
            collector.OnDataCollectionStarted += Collector_OnDataCollectionStarted;
            collector.OnDataCollectionStopped += Collector_OnDataCollectionStopped;
            collector.StartCollectingData(delay);

            Console.ReadKey();

            collector.OnDataCollected -= Collector_OnDataCollected;
            collector.StopCollectingData();
        }

        private static void Collector_OnDataCollectionStopped(object sender, EventArgs e)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Stopped collecting data.");
            Console.ResetColor();
        }

        private static void Collector_OnDataCollectionStarted(object sender, EventArgs e)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Started collecting data.");
            Console.ResetColor();
        }

        private static void Collector_OnDataCollected(object sender, int gasStationId, string fuelType, double price, DateTime time)
        {
            Console.BackgroundColor = ConsoleColor.Gray;
            Console.ForegroundColor = ConsoleColor.DarkBlue;
            Console.WriteLine($"[{time}] {gasStationId}: {fuelType} {price}");
            Console.ResetColor();
        }
    }
}
