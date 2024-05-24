using Azure.Core;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
using RestSharp;
using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using Amazon.S3.Model;
using System.Drawing.Drawing2D;
using System.Drawing;

namespace EventlyBackEnd.Functions
{
    public class Backblaze
    {
        public async Task<string> UploadImageToBackblaze(IFormFile imageFile,string folder)
        {
            // Basic authentication credentials
            string username = Environment.GetEnvironmentVariable("BB_username");
            string password = Environment.GetEnvironmentVariable("BB_password");

            // Create HttpClient instance
            using (HttpClient client = new HttpClient())
            {
                // Base64 encode the username and password for Basic authentication
                string base64Credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{username}:{password}"));
                string authorizationHeader = $"Basic {base64Credentials}";

                // Set the Authorization header for Basic authentication
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", base64Credentials);

                // Define the endpoint URL
                string url = "https://api.backblazeb2.com/b2api/v2/b2_authorize_account";

                try
                {
                    // Send POST request with empty JSON body
                    HttpResponseMessage response = await client.PostAsync(url, new StringContent("{}", Encoding.UTF8, "application/json"));

                    // Check if request was successful
                    if (response.IsSuccessStatusCode)
                    {
                        // Read and display response content
                        string responseBody = await response.Content.ReadAsStringAsync();
                        // Parse JSON response
                        JObject jsonResponse = JObject.Parse(responseBody);

                        // Get the apiUrl value from the JSON response
                        string apiUrl = jsonResponse["apiUrl"].ToString();
                        string authToken = jsonResponse["authorizationToken"].ToString();

                        // Define the URL for the second request
                        string newUrl = apiUrl;
                        var endpoint = new RestClient(newUrl);
                        var request = new RestRequest("b2api/v2/b2_get_upload_url");
                        request.AddHeader("Authorization", authToken);
                        request.AddBody(new
                        {
                            bucketId = "d5ed705093db2f448efb0016"
                        });
                        var response2 = endpoint.ExecutePost(request);

                        // deserialize json string response to JsonNode object
                        var data = System.Text.Json.JsonSerializer.Deserialize<JsonNode>(response2.Content!)!;

                        // ---------------------------------------------------------------------------------------
                        // third and final request
                        string finalUrl = data["uploadUrl"].ToString();
                        string uploadToken = data["authorizationToken"].ToString();
                        var finalEndpoint = new RestClient(finalUrl);
                        var finalRequest = new RestRequest();
                        finalRequest.AddHeader("Authorization", uploadToken);
                        string fileName = Uri.EscapeDataString($"{folder}/{DateTime.Now:yyyyMMddHHmmssfff}.jpg");
                        finalRequest.AddHeader("X-Bz-File-Name", fileName);
                        finalRequest.AddHeader("X-Bz-Content-Sha1", "do_not_verify");
                        // send image as binary data
                        var binary = new byte[imageFile.Length];
                        using (var stream = imageFile.OpenReadStream())
                        {
                            stream.Read(binary, 0, (int)imageFile.Length);
                        }
                        if (binary.Length > 0)
                        {
                            finalRequest.AddParameter("application/octet-stream", binary, ParameterType.RequestBody);
                        }



                        var finalResponse = await finalEndpoint.ExecutePostAsync(finalRequest);
                        var finalResult = System.Text.Json.JsonSerializer.Deserialize<JsonNode>(finalResponse.Content!)!;
                        Console.WriteLine(finalResult);
                        var imageFinalURL = $"https://evently-images.s3.eu-central-003.backblazeb2.com/{finalResult["fileName"]}";
                        return (imageFinalURL);

                    }
                    else
                    {
                        Console.WriteLine($"Request failed with status code: {response.StatusCode}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred: {ex.Message}");
                }
            }
            return null;
        }
    }
}
