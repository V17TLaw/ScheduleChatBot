using Newtonsoft.Json;
using Schedule.ChatBot.Models;
using System;
using System.Configuration;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Schedule.ChatBot.Utility
{
    public static class RestApiClient
    {
        private static string _baseAddress;

        public static string BaseAddress
        {
            get
            {
                return _baseAddress ?? (_baseAddress = ConfigurationManager.AppSettings["ScheduleSystemApiBaseAddress"]);
            }
            
            set
            {
                _baseAddress = value;
            }
        }

        private static HttpClient _httpClient;

        public static HttpClient Api
        {
            get
            {
                return _httpClient ?? (_httpClient = InitializeHttpClient());
            }
        }

        private static HttpClient InitializeHttpClient()
        {
            var httpClient = new HttpClient
            {
                BaseAddress = new Uri(BaseAddress)
            };

            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

            return httpClient;
        }
        
        public static async Task<Appointment> GetAppointmentByPhoneNumberAsync(string phoneNumber)
        {
            var response = await Api.GetAsync($"patient/{phoneNumber}/appointment")
                                    .ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
                return null;

            var json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<Appointment>(json);
        }

        public static async Task<Appointment> CancelAppointmentAsync(int id, string disposition)
        {
            var response = await Api.PostAsync($"appointment/{id}/cancel", new StringContent("\"" + disposition + "\"", Encoding.UTF8, "application/json"))
                                    .ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
                return null;

            var json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<Appointment>(json);
        }
    }
}