//===============================================================================
// Microsoft FastTrack for Azure
// Azure AD User and Group CRUD Sample
//===============================================================================
// Copyright © Microsoft Corporation.  All rights reserved.
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
// FITNESS FOR A PARTICULAR PURPOSE.
//===============================================================================
using Newtonsoft.Json.Linq;
using System;
using System.Configuration;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace RegisterEventHook
{
    /// <summary>
    /// Azure AD CRUD test harness
    /// </summary>
    class Program
    {
        private static readonly string _apiUrl = ConfigurationManager.AppSettings["APIURL"];
        static void Main(string[] args)
        {
            // Update these values for your testing purposes
            string userMailNickname = "JerleS";
            string userDisplayName = "Jerle Shannara";
            string userPrincipalName = "JerleS@nimccolladds.onmicrosoft.com";
            string userPassword = "Shannara123";
            string groupDescription = "Demonstrate group creation via API #3";
            string groupDisplayName = "Demo Group Number 3";
            bool groupMailEnabled = false;
            string groupMailNickname = "DemoGroup3";
            bool groupSecurityEnabled = true;
            string userObjectId = string.Empty;
            string groupObjectId = string.Empty;

            Console.WriteLine("*** Azure AD CRUD Test Harness Started ***");

            userObjectId = CreateUser(userMailNickname, userDisplayName, userPrincipalName, userPassword);
            groupObjectId = CreateGroup(groupDescription, groupDisplayName, groupMailEnabled, groupMailNickname, groupSecurityEnabled);
            AddMember(groupObjectId, userObjectId);

            Console.WriteLine("*** Press any key to exit ***");
            Console.Read();
        }

        private static string CreateUser(string mailNickName, string displayName, string userPrincipalName, string password)
        {
            string userObjectId = string.Empty;

            Console.WriteLine("*** Creating new user... ***");
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(_apiUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                string queryString = "api/users";
                string aadUser = $"{{ \"id\":\"\", \"displayName\":\"{displayName}\", \"mailNickname\":\"{mailNickName}\", \"userPrincipalName\":\"{userPrincipalName}\", \"password\":\"{password}\" }}";
                StringContent content = new StringContent(aadUser, Encoding.UTF8, "application/json");

                HttpResponseMessage response = client.PostAsync(queryString, content).Result;

                if (response.IsSuccessStatusCode)
                {
                    string responseContent = response.Content.ReadAsStringAsync().Result;
                    var newUser = JObject.Parse(responseContent);
                    userObjectId = newUser.Property("id").Value.ToString();
                    Console.WriteLine("*** User created successfully ***");
                }
                else
                {
                    string responseContent = response.Content.ReadAsStringAsync().Result;
                    Console.WriteLine($"*** User creation failed. HTTP request failed with a status code of {response.StatusCode} - {responseContent} ***");
                }
            }

            return userObjectId;
        }

        private static string CreateGroup(string description, string displayName, bool mailEnabled, string mailNickname, bool securityEnabled)
        {
            string groupObjectId = string.Empty;

            Console.WriteLine("*** Creating new group... ***");
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(_apiUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                string queryString = "api/groups";
                string aadGroup = $"{{ \"id\":\"\", \"description\":\"{description}\", \"displayName\":\"{displayName}\", \"mailEnabled\":\"{mailEnabled}\", \"mailNickname\":\"{mailNickname}\", \"securityEnabled\":\"{securityEnabled}\" }}";
                StringContent content = new StringContent(aadGroup, Encoding.UTF8, "application/json");

                HttpResponseMessage response = client.PostAsync(queryString, content).Result;

                if (response.IsSuccessStatusCode)
                {
                    string responseContent = response.Content.ReadAsStringAsync().Result;
                    var newGroup = JObject.Parse(responseContent);
                    groupObjectId = newGroup.Property("id").Value.ToString();
                    Console.WriteLine("*** Group created successfully ***");
                }
                else
                {
                    string responseContent = response.Content.ReadAsStringAsync().Result;
                    Console.WriteLine($"*** Group creation failed. HTTP request failed with a status code of {response.StatusCode} - {responseContent} ***");
                }
            }

            return groupObjectId;
        }

        private static void AddMember(string groupObjectId, string userObjectId)
        {
            Console.WriteLine("*** Adding user to group... ***");
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(_apiUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                string queryString = "api/groups/members";
                string aadGroupMembership = $"{{ \"groupId\":\"{groupObjectId}\", \"members\":[\"{userObjectId}\"] }}";
                StringContent content = new StringContent(aadGroupMembership, Encoding.UTF8, "application/json");

                HttpResponseMessage response = client.PostAsync(queryString, content).Result;

                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("*** User(s) successfully added to group ***");
                }
                else
                {
                    string responseContent = response.Content.ReadAsStringAsync().Result;
                    Console.WriteLine($"*** Adding user to group failed. HTTP request failed with a status code of {response.StatusCode} - {responseContent} ***");
                }
            }
        }
    }
}
