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
    /// Provide an API facade for Azure AD User CRUD operations
    /// </summary>
    public class UsersController : ApiController
    {
        /// <summary>
        /// Add a new user to Azure Active Directory
        /// </summary>
        /// <returns>HttpResponseMessage containing the newly created user</returns>
        [HttpPost]
        public async Task<HttpResponseMessage> Post()
        {
            // The post body should contain a JSON representation of the AADUser object
            HttpResponseMessage response = new HttpResponseMessage();
            string content = Request.Content.ReadAsStringAsync().Result;
            AADUser model = null;

            try
            {
                // Convert the JSON payload to an AADUser
                model = JsonConvert.DeserializeObject<AADUser>(content);
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

                var user = new User
                {
                    AccountEnabled = true,
                    DisplayName = model.DisplayName,
                    MailNickname = model.MailNickname,
                    UserPrincipalName = model.UserPrincipalName,
                    PasswordProfile = new PasswordProfile
                    {
                        ForceChangePasswordNextSignIn = false,
                        Password = model.Password
                    }
                };

                try
                {
                    // Add the user to Azure AD
                    User newUser = await graphClient.Users
                        .Request()
                        .AddAsync(user);

                    // Capture the object ID of the newly created user
                    model.Id = newUser.Id;
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
    }
}
