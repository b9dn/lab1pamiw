using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using P04WeatherForecastAPI.Client.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace P04WeatherForecastAPI.Client.Services {
    internal class AccuWeatherService {
        private const string base_url = "http://dataservice.accuweather.com";
        private const string autocomplete_endpoint = "locations/v1/cities/autocomplete?apikey={0}&q={1}&language={2}";
        private const string current_conditions_endpoint = "currentconditions/v1/{0}?apikey={1}&language={2}";

        private const string historical24h_endpoint = "currentconditions/v1/{0}/historical/24?apikey={1}&language={2}";
        private const string historical6h_endpoint = "currentconditions/v1/{0}/historical?apikey={1}&language={2}";
        private const string near_cities_endpoint = "locations/v1/cities/neighbors/{0}?apikey={1}&language={2}";
        private const string indices_swimming_endpoint = "indices/v1/daily/1day/{0}/10?apikey={1}&language={2}&details=true";
        private const string indices_tennis_endpoint = "indices/v1/daily/1day/{0}/6?apikey={1}&language={2}&details=true";
        private const string indices_running_endpoint = "indices/v1/daily/1day/{0}/1?apikey={1}&language={2}&details=true";



        //private const string api_key = "GnLggAsdZrHoSEOJxNX8rlthA7aLht2z";
        string api_key;
        //private const string language = "pl";
        string language;

        public AccuWeatherService() {
            var builder = new ConfigurationBuilder()
                .AddUserSecrets<App>()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsetings.json");

            var configuration = builder.Build();
            api_key = configuration["api_key"];
            language = configuration["default_language"];
        }


        public async Task<City[]> GetLocations(string locationName) {
            string uri = base_url + "/" + string.Format(autocomplete_endpoint, api_key, locationName, language);
            using (HttpClient client = new HttpClient()) {
                var response = await client.GetAsync(uri);
                string json = await response.Content.ReadAsStringAsync();
                City[] cities = JsonConvert.DeserializeObject<City[]>(json);
                return cities;
            }
        }

        public async Task<Weather> GetCurrentConditions(string cityKey) {
            string uri = base_url + "/" + string.Format(current_conditions_endpoint, cityKey, api_key, language);
            using (HttpClient client = new HttpClient()) {
                var response = await client.GetAsync(uri);
                string json = await response.Content.ReadAsStringAsync();
                Weather[] weathers = JsonConvert.DeserializeObject<Weather[]>(json);
                return weathers.FirstOrDefault();
            }
        }

        public async Task<Weather> GetHistoricalConditions24h(string cityKey) {
            string uri = base_url + "/" + string.Format(historical24h_endpoint, cityKey, api_key, language);
            using (HttpClient client = new HttpClient()) {
                var response = await client.GetAsync(uri);
                string json = await response.Content.ReadAsStringAsync();
                Weather[] weathers = JsonConvert.DeserializeObject<Weather[]>(json);
                return weathers.FirstOrDefault();
            }
        }

        public async Task<Weather> GetHistoricalConditions6h(string cityKey) {
            string uri = base_url + "/" + string.Format(historical6h_endpoint, cityKey, api_key, language);
            using (HttpClient client = new HttpClient()) {
                var response = await client.GetAsync(uri);
                string json = await response.Content.ReadAsStringAsync();
                Weather[] weathers = JsonConvert.DeserializeObject<Weather[]>(json);
                return weathers.FirstOrDefault();
            }
        }

        public async Task<City[]> GetNearCities(string cityKey) {
            string uri = base_url + "/" + string.Format(near_cities_endpoint, cityKey, api_key, language);
            using (HttpClient client = new HttpClient()) {
                var response = await client.GetAsync(uri);
                string json = await response.Content.ReadAsStringAsync();
                City[] cities = JsonConvert.DeserializeObject<City[]>(json);
                return cities;
            }
        }

        public enum Activity {
            Swimming,
            Tennis,
            Running
        }

        public async Task<IndexValues> GetActivityIndex(string cityKey, Activity activity) {
            string endpoint;
            switch (activity) {
                case Activity.Swimming:
                    endpoint = indices_swimming_endpoint;
                    break;
                case Activity.Tennis:
                    endpoint = indices_tennis_endpoint;
                    break;
                case Activity.Running:
                    endpoint = indices_running_endpoint;
                    break;
                default:
                    endpoint = indices_tennis_endpoint;
                    break;
            }

            string uri = base_url + "/" + string.Format(endpoint, cityKey, api_key, language);
            using (HttpClient client = new HttpClient()) {
                var response = await client.GetAsync(uri);
                string json = await response.Content.ReadAsStringAsync();
                IndexValues[] index = JsonConvert.DeserializeObject<IndexValues[]>(json);
                return index.FirstOrDefault();
            }
        }
    }
}
