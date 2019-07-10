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
using AADSampleAPI.Models;
using Microsoft.Graph;
using Microsoft.Graph.Auth;
using Microsoft.Identity.Client;
using Newtonsoft.Json;
using System;
using System.Configuration;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace AADSampleAPI.Controllers
{
    /// <summary>
    /// Provide an API facade for Azure AD Group CRUD operations
    /// </summary>
    public class GroupsController : ApiController
    {
        /// <summary>
        /// Add a new group to Azure Active Directory
        /// </summary>
        /// <returns>HttpResponseMessage containing the newly created group</returns>
        [HttpPost]
        public async Task<HttpResponseMessage> Post()
        {
            // The post body should contain a JSON representation of the AADGroup object
            HttpResponseMessage response = new HttpResponseMessage();
            string content = Request.Content.ReadAsStringAsync().Result;
            AADGroup model = null;

            try
            {
                // Convert the JSON payload to an AADGroup
                model = JsonConvert.DeserializeObject<AADGroup>(content);
            }
            catch (Exception ex)
            {
                response.StatusCode = HttpStatusCode.InternalServerError;
                response.Content = new StringContent(ex.Message);
            }

            if (model != null)
            {
                // Authenticate to the Microsoft Graph and create a graph client
                string clientId = ConfigurationManager.AppSettings["ClientId"];
                string clientSecret = ConfigurationManager.AppSettings["ClientSecret"];
                string tenant = ConfigurationManager.AppSettings["Tenant"];
                ClientCredential clientCredential = new ClientCredential(clientSecret);
                IConfidentialClientApplication clientApplication = ClientCredentialProvider.CreateClientApplication(clientId, clientCredential, null, tenant);
                ClientCredentialProvider authProvider = new ClientCredentialProvider(clientApplication);
                GraphServiceClient graphClient = new GraphServiceClient(authProvider);

                var group = new Group
                {
                    Description = model.Description,
                    DisplayName = model.DisplayName,
                    MailEnabled = model.MailEnabled,
                    MailNickname = model.MailNickname,
                    SecurityEnabled = model.SecurityEnabled
                };

                try
                {
                    // Add the group to Azure AD
                    var newGroup = await graphClient.Groups
                        .Request()
                        .AddAsync(group);

                    // Capture the object ID of the newly created group
                    model.Id = newGroup.Id;
                    response.StatusCode = HttpStatusCode.OK;
                    response.Content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
                }
                catch (Exception ex)
                {
                    response.StatusCode = HttpStatusCode.InternalServerError;
                    response.Content = new StringContent(ex.Message);
                }
            }

            return response;
        }

        /// <summary>
        /// Add one or more users to an Azure AD group
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("api/groups/members")]
        public async Task<HttpResponseMessage> AddMember()
        {
            // The post body should contain a JSON representation of the AADGroupMembership object
            HttpResponseMessage response = new HttpResponseMessage();
            string content = Request.Content.ReadAsStringAsync().Result;
            AADGroupMembership model = null;

            try
            {
                // Convert the JSON payload to an AADGroupMembership
                model = JsonConvert.DeserializeObject<AADGroupMembership>(content);
            }
            catch (Exception ex)
            {
                response.StatusCode = HttpStatusCode.InternalServerError;
                response.Content = new StringContent(ex.Message);
            }

            if (model != null)
            {
                // Authenticate to the Microsoft Graph and create a graph client
                string clientId = ConfigurationManager.AppSettings["ClientId"];
                string clientSecret = ConfigurationManager.AppSettings["ClientSecret"];
                string tenant = ConfigurationManager.AppSettings["Tenant"];
                ClientCredential clientCredential = new ClientCredential(clientSecret);
                IConfidentialClientApplication clientApplication = ClientCredentialProvider.CreateClientApplication(clientId, clientCredential, null, tenant);
                ClientCredentialProvider authProvider = new ClientCredentialProvider(clientApplication);
                GraphServiceClient graphClient = new GraphServiceClient(authProvider);

                try
                {
                    // Add each user to the Azure AD group
                    foreach (string userId in model.Members)
                    {
                        var user = await graphClient.Users[userId].Request().GetAsync();
                        await graphClient.Groups[model.GroupId]
                            .Members
                            .References
                            .Request()
                            .AddAsync(user);
                    }
                    response.StatusCode = HttpStatusCode.OK;
                }
                catch (Exception ex)
                {
                    response.StatusCode = HttpStatusCode.InternalServerError;
                    response.Content = new StringContent(ex.Message);
                }
            }

            return response;
        }
    }
}
