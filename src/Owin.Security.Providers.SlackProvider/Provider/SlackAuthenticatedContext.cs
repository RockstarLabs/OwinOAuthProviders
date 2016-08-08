﻿using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Provider;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Security.Claims;

namespace Owin.Security.Providers.Slack.Provider
{
    public class SlackAuthenticatedContext : BaseContext
    {
        /// <summary>
        /// Initializes a <see cref="SlackAuthenticatedContext"/>
        /// </summary>
        /// <param name="context">The OWIN environment</param>
        /// <param name="user">The JSON-serialized user</param>
        /// <param name="accessToken">Slack Access token</param>
        /// <param name="scope">Indicates access level of application</param>
        /// <param name="incomingWebhook">Contains channel infomation about the choosen default channel</param>
        public SlackAuthenticatedContext(IOwinContext context, JObject user, string accessToken, string scope, JObject incomingWebhook)
            : base(context)
        {
            User = user;
            AccessToken = accessToken;
            Scope = scope.Split(',');

            UserId = TryGetValue(user, "user_id");
            UserName = TryGetValue(user, "user");
            TeamId = TryGetValue(user, "team_id");
            TeamName = TryGetValue(user, "team");
            TeamUrl = TryGetValue(user, "url");
            if (incomingWebhook != null)
            {
                ChannelId = TryGetValue(incomingWebhook, "channel_id");
                ChannelName = TryGetValue(incomingWebhook, "channel");
                ConfigurationUrl = TryGetValue(incomingWebhook, "configuration_url");
                HooksUrl = TryGetValue(incomingWebhook, "url");
            }

        }

        /// <summary>
        /// Gets the JSON-serialized user
        /// </summary>
        /// <remarks>
        /// Contains the Slack user
        /// </remarks>
        public JObject User { get; private set; }

        /// <summary>
        /// Gets the Slack access token
        /// </summary>
        public string AccessToken { get; private set; }

        /// <summary>
        /// Gets the scope of the application's access to user info
        /// </summary>
        public IList<string> Scope { get; private set; }

        /// <summary>
        /// Gets the Slack team ID
        /// </summary>
        public string TeamId { get; private set; }

        /// <summary>
        /// Gets the Slack team name
        /// </summary>
        public string TeamName { get; private set; }

        /// <summary>
        /// Gets the Slack user's team URL
        /// </summary>
        public string TeamUrl { get; private set; }

        /// <summary>
        /// Gets the Slack user ID
        /// </summary>
        public string UserId { get; private set; }

        /// <summary>
        /// Gets the Slack username
        /// </summary>
        public string UserName { get; private set; }


        /// <summary>
        /// Gets the name of the channel choose at oauth time
        /// </summary>
        public string ChannelName { get; private set; }

        /// <summary>
        /// Gets the channel ID of the channel choosen at oauth time
        /// </summary>
        public string ChannelId { get; private set; }

        /// <summary>
        /// Gets the confiquration url
        /// </summary>
        public string ConfigurationUrl { get; private set; }

        /// <summary>
        /// Gets the hook url
        /// </summary>
        public string HooksUrl { get; private set; }

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
    }
}
