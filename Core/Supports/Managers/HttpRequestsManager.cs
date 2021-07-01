using Core.Supports.Enums;
using Core.Supports.Exceptions;
using Core.Supports.Helpers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Core.Supports.Managers
{
    public static class HttpRequestsManager
    {
        public static async Task<T> ExecuteRequest<T>(string url, HttpMethod httpMethod, string body = null, ICollection<KeyValuePair<string, string>> formData = null, AuthCredentialsHelper authenticationCredentials = null, int secondsTimeout = 10)
        {
            var client = new HttpClient();
            client.Timeout = new TimeSpan(0, 0, secondsTimeout);

            if (authenticationCredentials != null)
            {
                switch (authenticationCredentials.AuthenticationType)
                {
                    case AuthenticationTypesEnum.Basic:
                        {
                            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
                                authenticationCredentials.Token);
                            break;
                        }
                    case AuthenticationTypesEnum.Bearer:
                        {
                            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer",
                                authenticationCredentials.Token);
                            break;
                        }
                }
            }

            var requestMessage = new HttpRequestMessage();
            requestMessage.RequestUri = new Uri(url);
            requestMessage.Method = httpMethod;


            if (!string.IsNullOrWhiteSpace(body) && httpMethod != HttpMethod.Get && httpMethod != HttpMethod.Delete)
            {
                requestMessage.Content = new StringContent(body, Encoding.UTF8, "application/json");
            }
            else if (formData != null && formData.Count > 0)
            {
                requestMessage.Content = new FormUrlEncodedContent(formData);
            }

            HttpResponseMessage response = await client.SendAsync(requestMessage);

            var content = await response.Content.ReadAsStringAsync();

            if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.Created)
            {
                throw new HttpFailedStatusException(response.StatusCode, content);
            }

            T result = default;

            if (!string.IsNullOrWhiteSpace(content))
            {
                result = JsonConvert.DeserializeObject<T>(content);
            }

            return result;
        }

        public static AuthCredentialsHelper GetCredentials(string token, AuthenticationTypesEnum authenticationType)
        {
            var authCredentials = new AuthCredentialsHelper()
            {
                AuthenticationType = authenticationType,
                Token = token
            };
            return authCredentials;
        }
    }
}
