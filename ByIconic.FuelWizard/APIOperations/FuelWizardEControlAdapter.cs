using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using static ByIconic.FuelWizard.Models.API.EControlClasses;

namespace ByIconic.FuelWizard.APIOperations
{
    class FuelWizardEControlAdapter
    {
        static HttpClient httpClient = new HttpClient();

        static DateTime lastApiCallTime = DateTime.MinValue;
        static TimeSpan delayBetweenAPICalls = new TimeSpan(0, 0, 0, 0, 200);

        internal static IEnumerable<GasStationPublic> FetchGasStationsOfLocation(Location location)
        {
            List<GasStationPublic> result = new List<GasStationPublic>();

            string latitude = location.latitude.ToString().Replace(',', '.');
            string longitude = location.longitude.ToString().Replace(',', '.');

            string baseRequest = $"https://api.e-control.at/sprit/1.0/search/gas-stations/by-address?latitude={latitude}&longitude={longitude}";

            //Add Prices for Diesel
            var apiTask = GetApiResponseAsync($"{baseRequest}&fuelType=DIE");
            apiTask.Wait();

            var replyDeserialized = JsonSerializer.Deserialize(apiTask.Result, typeof(GasStationPublic[]));
            result.AddRange((GasStationPublic[])replyDeserialized);


            //Add Prices for Super aka Gasoline or Petrol
            apiTask = GetApiResponseAsync($"{baseRequest}&fuelType=SUP");
            apiTask.Wait();

            replyDeserialized = JsonSerializer.Deserialize(apiTask.Result, typeof(GasStationPublic[]));
            result.AddRange((GasStationPublic[])replyDeserialized);


            return result;
        }


        internal static async Task<string> GetApiResponseAsync(string request)
        {
            //Check if the delay between API Calls is already over, and wait if not
            while ((lastApiCallTime + delayBetweenAPICalls) > DateTime.Now);

            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.DefaultRequestHeaders.Add("User-Agent", ".NET Foundation Repository Reporter");

            var stringTask = httpClient.GetStringAsync(request);

            var msg = await stringTask;

            //Reset last API Call time
            lastApiCallTime = DateTime.Now;

            return msg;
        }
    }
}
