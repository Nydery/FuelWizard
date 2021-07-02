using ByIconic.FuelWizard.DataCollector;
using NUnit.Framework;

namespace FuelWizard.UnitTests
{
    public class DataCollectorTests
    {
        FuelWizardDataCollector dataCollector;
        [SetUp]
        public void Setup()
        {
            dataCollector = new FuelWizardDataCollector();
        }

        [Test]
        public void Test1()
        {
            Assert.IsTrue(dataCollector.StartCollectingData(new System.TimeSpan(0, 0, 10), false), "Data Collection didnt start.");
            dataCollector.StopCollectingData();
            Assert.IsFalse(dataCollector.IsCollecting, "Data Collection didnt stop.");
        }
    }
}