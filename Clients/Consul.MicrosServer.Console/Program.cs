﻿using IdentityModel.Client;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Consul.MicrosServer.Consoles
{
    class Program
    {
        static async Task Main(string[] args)
        {

            Console.ReadLine();
            var client = new HttpClient();
            var request = new DiscoveryDocumentRequest
            {
                Address = "http://u.ke.me/",
                Policy =
                {
                       RequireHttps =false
                }
            };
            var disco = await client.GetDiscoveryDocumentAsync(request);
            if (disco.IsError)
            {
                Console.WriteLine(disco.Error);
                Console.ReadLine();
                return;
            }

            var tokenResponse = await client.RequestPasswordTokenAsync(new PasswordTokenRequest
            {
                Address = disco.TokenEndpoint,
                ClientId = "api",
                ClientSecret = "secret",
                UserName = "zhangfaguo98@163.com",
                Password = "198822",
                Scope = "api"
            });
            if (tokenResponse.IsError)
            {
                Console.WriteLine(tokenResponse.Error);
                Console.ReadLine();
                return;
            }

            do
            {
                Console.ReadLine();
                var apiClient = new HttpClient();
                apiClient.SetBearerToken(tokenResponse.AccessToken);

                var response = await apiClient.GetAsync("http://localhost:5000/userservice/user/index");
                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine(response.StatusCode);
                }
                else
                {
                    var content = await response.Content.ReadAsStringAsync();
                    Console.WriteLine(content);
                }

            } while (true);
        }
    }
}
