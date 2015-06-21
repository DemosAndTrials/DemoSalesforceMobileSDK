using Salesforce.SDK.Source.Settings;
using System;
using System.Collections.Generic;
using System.Text;
using Windows.UI;

namespace DemoSalesforceMobileSDK.Models
{
    class Config : SalesforceConfig
    {
        /// <summary>
        /// This should return the client id generated when you create a connected app through Salesforce.
        /// </summary>
        public override string ClientId
        {
            get { return "3MVG9ytVT1SanXDkvbx5XRMc.mVU3633YHCdPbP3DsFj53GLlB0la25M3BQjpAA1HsP3lXmjKSssihnQpKu9x"; }
        }

        /// <summary>
        /// This should return the callback url generated when you create a connected app through Salesforce.
        /// </summary>
        public override string CallbackUrl
        {
            get { return "sfdc:///axm/detect/oauth/done"; }
        }

        /// <summary>
        /// Return the scopes that you wish to use in your app. Limit to what you actually need, try to refrain from listing all scopes.
        /// </summary>
        public override string[] Scopes
        {
            get { return new string[] { "api", "web", "chatter_api", "refresh_token", "full" }; }
        }

        public override Windows.UI.Color? LoginBackgroundColor
        {
            get
            {
                string color = "#009adb";
                if (color.StartsWith("#"))
                    color = color.Remove(0, 1);
                byte r, g, b;
                if (color.Length == 3)
                {
                    r = Convert.ToByte(color[0] + "" + color[0], 16);
                    g = Convert.ToByte(color[1] + "" + color[1], 16);
                    b = Convert.ToByte(color[2] + "" + color[2], 16);
                }
                else if (color.Length == 6)
                {
                    r = Convert.ToByte(color[0] + "" + color[1], 16);
                    g = Convert.ToByte(color[2] + "" + color[3], 16);
                    b = Convert.ToByte(color[4] + "" + color[5], 16);
                }
                else
                {
                    throw new ArgumentException("Hex color " + color + " is invalid.");
                }
                return Color.FromArgb(255, r, g, b);
            }
        }


        public override string ApplicationTitle
        {
            get { return "App5.Shared"; }
        }

        public override Uri LoginBackgroundLogo
        {
            get { return null; }
        }

        public override bool IsApplicationTitleVisible
        {
            get { return true; }
        }
    }
}
