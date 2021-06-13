using System;
using System.Collections;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace DiscordBotProject.APICalls
{

    public class Condition
    {
        public string Text { get; set; }
        public string Icon { get; set; }
        public int Code { get; set; }

    }

    public class Day
    {
        public float Maxtemp_C { get; set; }
        public float Maxtemp_F { get; set; }
        public float Mintemp_C { get; set; }
        public float Mintemp_F { get; set; }
        public float Avgtemp_C { get; set; }
        public float Avgtemp_F { get; set; }
        public float Maxwind_Kph { get; set; }
        public float Maxwind_Mph { get; set; }
        public float TotalPrecip_mm { get; set; }
        
        public float TotalPrecip_in { get; set; }
        public float AvgHumidity { get; set; }
        public bool Daily_Will_It_Rain { get; set; }
        public string Daily_Chance_of_Rain { get; set; }
        public Condition condition { get; set; }
     
    }

    class WeatherAPI
    {

        public static async Task<string> GetWeatherData()
        {
            string API_URI = "http://api.weatherapi.com/v1/forecast.json?key=5c390c29e1604d7fb2f162303211306&q=bucaramanga&days=1&aqi=no&alerts=no";

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(API_URI);

                HttpResponseMessage response = await client.GetAsync(API_URI);

                if (response.IsSuccessStatusCode)
                {
                    string strResult = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    return strResult;
                }
                else
                {
                    return null;
                }

            }

        }

        public Day GetParsedData()
        {
            string result = GetWeatherData().Result;

      
            var index = result.IndexOf("\"day\"")+6;
            var end = result.IndexOf("\"astro\"")-1;
   
            string forecastday =  result.Substring(index, end-index);
           
           
            Day Day = JsonConvert.DeserializeObject<Day>(forecastday);

            return Day;
        }
        
        public string GetBucaramangaForecast()
        {
            try
            {
                Day response = GetParsedData();
                string bucaramangaForecast = "For today, the minimum temperature will be: " + response.Mintemp_C + " °C\n" +
                "The maximum temperature will be: " + response.Maxtemp_C + " °C\nWith a chance of " + response.Daily_Chance_of_Rain + "% that it rains \n" +
                "And humidity of " + response.AvgHumidity + " g/kg";
                return bucaramangaForecast;
            }
            catch (Exception e)
            {
                return "Something wrong happened! :(";
            }
            
        }


    }
}
