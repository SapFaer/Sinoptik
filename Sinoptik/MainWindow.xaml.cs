using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;

namespace Sinoptik
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            string apiKey = "5a948e028a08b6de7d1d92ca8524b50a";
            string city = "Boyarka";

            WeatherDataResponse weatherDataResponse = await GetWeatherData(apiKey, city);

            if (weatherDataResponse != null)
            {
                List<WeatherData> weatherData = weatherDataResponse.List;

                DateTime currentTime = DateTime.Now;
                DateTime baseTime = GetBaseTime(currentTime);

                foreach (var data in weatherData)
                {
                    float tempCelsius = data.Main.Temp - 273.15f;
                    data.Main.Temp = tempCelsius;

                    data.ForecastTime = baseTime;
                    baseTime = baseTime.AddHours(3);
                }

                datagrid.ItemsSource = weatherData;
            }
        }

        private async Task<WeatherDataResponse> GetWeatherData(string apiKey, string city)
        {
            HttpClient client = new HttpClient();
            string apiUrl = $"http://api.openweathermap.org/data/2.5/forecast?q={city}&appid={apiKey}";

            try
            {
                HttpResponseMessage response = await client.GetAsync(apiUrl);
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();

                WeatherDataResponse weatherDataResponse = JsonConvert.DeserializeObject<WeatherDataResponse>(responseBody);

                return weatherDataResponse;
            }
            catch (HttpRequestException ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
                return null;
            }
        }

        private DateTime GetBaseTime(DateTime currentTime)
        {
            DateTime baseTime = currentTime;
            baseTime = baseTime.AddMinutes(30);
            baseTime = baseTime.AddTicks(-(baseTime.Ticks % TimeSpan.FromHours(1).Ticks));
            return baseTime;
        }

        public class WeatherDataResponse
        {
            [JsonProperty("list")]
            public List<WeatherData> List { get; set; }
        }

        public class WeatherData
        {
            [JsonProperty("main")]
            public WeatherMain Main { get; set; }

            [JsonProperty("dt_txt")]
            public string DateTimeText { get; set; }

            public DateTime ForecastTime { get; set; }

            [JsonProperty("weather")]
            public List<WeatherDescription> Weather { get; set; }
        }

        public class WeatherMain
        {
            [JsonProperty("temp")]
            public float Temp { get; set; }

            [JsonProperty("humidity")]
            public float Humidity { get; set; }
        }

        public class WeatherDescription
        {
            [JsonProperty("description")]
            public string Description { get; set; }
        }

        public class Weather
        {
            [JsonProperty("weather")]
            public string main { get; set; }
        }

        private void RadioButton1_Checked(object sender, RoutedEventArgs e)
        {
            // Логіка для першої опції
        }

        private void RadioButton2_Checked(object sender, RoutedEventArgs e)
        {
            // Логіка для другої опції
        }
    }
}

