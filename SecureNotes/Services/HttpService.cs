using SecureNotes.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;

namespace SecureNotes.Services
{
    public class HttpService
    {
        public static readonly HttpClient client = new HttpClient();
        public static readonly string API = "https://localhost:7042/api";
        public static readonly string API_SEND_PAYLOAD = API + "/payload/send";
        public static readonly string API_GET_ALL_MESSAGES = API + "/payload/received_messages/";
        private static readonly JsonSerializerOptions JsonOptions = new JsonSerializerOptions()
        {
            PropertyNameCaseInsensitive = true
        };
        public HttpService()
        {
        }

        public async Task<List<UserAuth>> GetUsers()
        {
            using HttpResponseMessage response = await HttpService.client.GetAsync(API + "/userauth/get_users");
            if (response.IsSuccessStatusCode)
            {
                string responseBody = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<List<UserAuth>>(responseBody, JsonOptions) ?? new List<UserAuth>();
            }
            return new List<UserAuth>();
        }

        public async Task<List<Payload>> GetAllMessages(string username)
        {
            using HttpResponseMessage response = await HttpService.client.GetAsync(API_GET_ALL_MESSAGES + username);
            if (response.IsSuccessStatusCode)
            {
                string responseBody = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<List<Payload>>(responseBody, JsonOptions) ?? new List<Payload>();
            }
            return new List<Payload>();
        }

    }
}
