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
using Newtonsoft.Json;
using System;

namespace AADSampleAPI.Models
{
    /// <summary>
    /// Represents a list of users to be added to an Azure AD group
    /// </summary>
    [Serializable]
    public class AADGroupMembership
    {
        [JsonProperty("groupId")]
        public string GroupId { get; set; }

        [JsonProperty("members")]
        public string[] Members { get; set; }
    }
}