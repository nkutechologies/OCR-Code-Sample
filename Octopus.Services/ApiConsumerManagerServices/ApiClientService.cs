using Newtonsoft.Json;
using Octopus.Dtos.Common;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Octopus.Services.ApiConsumerManagerServices
{
    public class ApiClientService : IDisposable
    {
        private readonly HttpClient _httpClient;

        public ApiClientService()
        {
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<TResponse> GetAsync<TResponse>(string url, string Token, string apiEndPoint)
        {
            if (!string.IsNullOrEmpty(Token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Token);
            }

            _httpClient.BaseAddress = new Uri(apiEndPoint);
            HttpResponseMessage response = await _httpClient.GetAsync(_httpClient.BaseAddress + url);

            if (response.IsSuccessStatusCode)
            {
                string responseBody = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<TResponse>(responseBody);
            }
            else
            {
                throw new Exception("" + response.StatusCode);
            }
        }
        public async Task<ResponseModel<T>> HttpPostClientAsync<T>(string url, string input, string token)
        {
            ResponseModel<T> response = new ResponseModel<T>();
            try
            {
                if (!string.IsNullOrEmpty(token))
                {
                    _httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);
                }

                var content = new StringContent(input, Encoding.UTF8, "application/json");

                var clientresponse =await _httpClient.PostAsync(url, content).ConfigureAwait(false);

                if (clientresponse.IsSuccessStatusCode)
                {
                    string responseRulst = await clientresponse.Content.ReadAsStringAsync();
                    response.Response = "SUCCESS";
                    response.Message = "SUCCESS";
                    response.StatusCode = clientresponse.StatusCode;
                    response.Data = JsonConvert.DeserializeObject<T>(responseRulst);

                }
                else
                {
                    string responseRulst =await clientresponse.Content.ReadAsStringAsync();
                    response.Response = "ERROR";
                    response.StatusCode = clientresponse.StatusCode;
                    response.Message = "Failed to access API..";


                }
            }
            catch (Exception)
            {
                response.Response = "ERROR";
                response.Message = "System Error";
                response.StatusCode = response.StatusCode = System.Net.HttpStatusCode.InternalServerError;
            }

            return response;
        }
        public async Task<TResponse> PostAsync<TRequest, TResponse>(string url, string Token, TRequest requestData)
        {
            if (!string.IsNullOrEmpty(Token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Token);
            }

            string jsonContent = JsonConvert.SerializeObject(requestData);
            var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await _httpClient.PostAsync(url, httpContent).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                string responseBody = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<TResponse>(responseBody);
            }
            else
            {
                throw new Exception($"API request failed with status code: {response.StatusCode}");
            }
        }

        public void Dispose()
        {
            _httpClient.Dispose();
        }
    }
}
