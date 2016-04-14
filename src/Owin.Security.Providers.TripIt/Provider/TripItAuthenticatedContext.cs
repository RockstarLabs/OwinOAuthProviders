﻿// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved. See License.txt in the project root for license information.

using System.Linq;
using System.Security.Claims;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Provider;
using Newtonsoft.Json.Linq;

namespace Owin.Security.Providers.TripIt.Provider
{
    /// <summary>
    /// Contains information about the login session as well as the user <see cref="System.Security.Claims.ClaimsIdentity"/>.
    /// </summary>
    public class TripItAuthenticatedContext : BaseContext
    {
        /// <summary>
        /// Initializes a <see cref="TripItAuthenticatedContext"/>
        /// </summary>
        /// <param name="context">The OWIN environment</param>
        /// <param name="profile">The JSON serialized user</param>
        /// <param name="accessToken">TripIt access token</param>
        /// <param name="accessTokenSecret">TripIt access token secret</param>
        public TripItAuthenticatedContext(
            IOwinContext context,
            JObject profile,
            string accessToken,
            string accessTokenSecret)
            : base(context)
        {
            Profile = profile;

            ScreenName = TryGetValue(profile, "screen_name");
            DisplayName = TryGetValue(profile, "public_display_name");
            UserId = GetUserId(profile);
            Email = GetEmail(profile);
            AccessToken = accessToken;
            AccessTokenSecret = accessTokenSecret;
        }

        private static string GetUserId(JObject profile)
        {
            return profile["@attributes"]?["ref"]?.ToString();
        }

        /// <summary>
        /// Gets the JSON-serialized User Profile
        /// </summary>
        /// <remarks>
        /// Contains the profile object obtained from TripIt
        /// </remarks>
        public JObject Profile { get; private set; }

        /// <summary>
        /// Gets the public display name for the user
        /// </summary>
        public string DisplayName { get; private set; }

        /// <summary>
        /// Gets the primary email address for the account
        /// </summary>
        public string Email { get; private set; }

        /// <summary>
        /// Gets the TripIt user ID
        /// </summary>
        public string UserId { get; private set; }

        /// <summary>
        /// Gets the TripIt screen name
        /// </summary>
        public string ScreenName { get; private set; }

        /// <summary>
        /// Gets the TripIt access token
        /// </summary>
        public string AccessToken { get; private set; }

        /// <summary>
        /// Gets the TripIt access token secret
        /// </summary>
        public string AccessTokenSecret { get; private set; }

        /// <summary>
        /// Gets the <see cref="ClaimsIdentity"/> representing the user
        /// </summary>
        public ClaimsIdentity Identity { get; set; }

        /// <summary>
        /// Gets or sets a property bag for common authentication properties
        /// </summary>
        public AuthenticationProperties Properties { get; set; }

        private static string TryGetValue(JObject user, string propertyName)
        {
            JToken value;
            return user.TryGetValue(propertyName, out value) ? value.ToString() : null;
        }

        private static string GetEmail(JObject user)
        {
            if (user == null) return null;
            try
            {
                JToken email = null;
                var profileEmailAddresses = user["ProfileEmailAddresses"];

                if (profileEmailAddresses != null)
                {
                    var profileEmailAddress = profileEmailAddresses["ProfileEmailAddress"];

                    if (profileEmailAddress != null)
                    {
                        if (profileEmailAddress.Type == JTokenType.Array)
                        {
                            // Try and get the primary email address
                            email = profileEmailAddress.FirstOrDefault(e => e["is_primary"].ToString() == "true") ??
                                    profileEmailAddress.FirstOrDefault();

                            // If no primary email was located, select the first email we can find
                        }
                        else if (profileEmailAddress.Type == JTokenType.Object)
                        {
                            // If the emails element is a single object and not an array, then take that object as the email object
                            email = profileEmailAddress;
                        }
                    }

                    // If we managed to find an email (primary or otherwise), then return the email
                    if (email?["address"] != null) return email["address"].ToString();
                }
            }
            catch
            {
                // Suppress any exception here
            }

            return null;
        }
    }
}
