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

        internal static IEnumerable<GasStationPublic> FetchGasStationsOfLocation(Location location)
        {
            List<GasStationPublic> result = new List<GasStationPublic>();

            string latitude = location.latitude.ToString().Replace(',', '.');
            string longitude = location.longitude.ToString().Replace(',', '.');

            var apiTask = GetApiResponseAsync($"https://api.e-control.at/sprit/1.0/search/gas-stations/by-address?latitude={latitude}&longitude={longitude}&fuelType=DIE");
            apiTask.Wait();

            var replyDeserialized = JsonSerializer.Deserialize(apiTask.Result, typeof(GasStationPublic[]));
            result.AddRange((GasStationPublic[])replyDeserialized);

            return result;
        }

        internal static async Task<string> GetApiResponseAsync(string request)
        {
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.DefaultRequestHeaders.Add("User-Agent", ".NET Foundation Repository Reporter");

            var stringTask = httpClient.GetStringAsync(request);

            var msg = await stringTask;

            return msg;
        }
    }
}
