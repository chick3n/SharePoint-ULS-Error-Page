using System;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;

namespace ULSError.Features.ULSErrorPage
{
    /// <summary>
    /// This class handles events raised during feature activation, deactivation, installation, uninstallation, and upgrade.
    /// </summary>
    /// <remarks>
    /// The GUID attached to this class may be used during packaging and should not be modified.
    /// </remarks>

    [Guid("9878571c-31ed-4d4d-b96a-d484b53b8676")]
    public class ULSErrorPageEventReceiver : SPFeatureReceiver
    {
        // Uncomment the method below to handle the event raised after a feature has been activated.
        private const string errorPagePath = "/_layouts/15/ULSError/ULSErrorPage.aspx";
        public override void FeatureActivated(SPFeatureReceiverProperties properties)
        {
            var app = properties.Feature.Parent as SPWebApplication;
            if (app != null)
            {
                var errorPage = app.GetMappedPage(SPWebApplication.SPCustomPage.Error);
                if (string.IsNullOrEmpty(errorPage) || !errorPage.Equals(errorPagePath))
                {
                    app.UpdateMappedPage(SPWebApplication.SPCustomPage.Error, errorPagePath);
                    app.Update();
                }

                var site = Core.Administration.GetRootSiteFromApplication(app);
                if (site != null)
                {
                    var admin = SPAdministrationWebApplication.Local;
                    var settings = new Core.Settings();
                    settings.MailTo = string.Empty;
                    settings.VisibleTo = string.Empty;

                    using (var caWeb = admin.Sites[0].OpenWeb())
                    {
                        if(caWeb.AllProperties.ContainsKey(Core.Constants.PROPERTY_MAILSERVER)) {
                            settings.MailTo = caWeb.AllProperties[Core.Constants.PROPERTY_MAILSERVER]?.ToString() ?? string.Empty;
                        }
                        if(caWeb.AllProperties.ContainsKey(Core.Constants.PROPERTY_VISIBLETO))
                        {
                            settings.VisibleTo = caWeb.AllProperties[Core.Constants.PROPERTY_VISIBLETO]?.ToString() ?? string.Empty;
                        }
                    }

                    using (var web = site.OpenWeb())
                    {
                        if (!web.AllProperties.ContainsKey(Core.Constants.PROPERTY_MAILSERVER))
                        {
                            web.AllProperties.Add(Core.Constants.PROPERTY_MAILSERVER, settings.MailTo);
                        }
                        else
                        {
                            web.AllProperties[Core.Constants.PROPERTY_MAILSERVER] = settings.MailTo;
                        }

                        if (!web.AllProperties.ContainsKey(Core.Constants.PROPERTY_VISIBLETO))
                        {
                            web.AllProperties.Add(Core.Constants.PROPERTY_VISIBLETO, settings.VisibleTo);
                        }
                        else
                        {
                            web.AllProperties[Core.Constants.PROPERTY_VISIBLETO] = settings.VisibleTo;
                        }

                        web.Update();
                    }
                }
            }
        }


        // Uncomment the method below to handle the event raised before a feature is deactivated.

        public override void FeatureDeactivating(SPFeatureReceiverProperties properties)
        {
            var app = properties.Feature.Parent as SPWebApplication;
            if (app != null)
            {
                var errorPage = app.GetMappedPage(SPWebApplication.SPCustomPage.Error);
                if (errorPage != null && errorPage.Equals(errorPagePath))
                {
                    app.UpdateMappedPage(Microsoft.SharePoint.Administration.SPWebApplication.SPCustomPage.Error, null);
                    app.Update();
                }

                var site = Core.Administration.GetRootSiteFromApplication(app);
                if(site != null)
                {
                    using (var web = site.OpenWeb())
                    {
                        if (web.AllProperties.ContainsKey(Core.Constants.PROPERTY_MAILSERVER))
                        {
                            web.AllProperties.Remove(Core.Constants.PROPERTY_MAILSERVER);
                        }
                        if (web.AllProperties.ContainsKey(Core.Constants.PROPERTY_VISIBLETO))
                        {
                            web.AllProperties.Remove(Core.Constants.PROPERTY_VISIBLETO);
                        }

                        web.Update();
                    }
                }
            }
        }


        // Uncomment the method below to handle the event raised after a feature has been installed.

        //public override void FeatureInstalled(SPFeatureReceiverProperties properties)
        //{
        //}


        // Uncomment the method below to handle the event raised before a feature is uninstalled.

        //public override void FeatureUninstalling(SPFeatureReceiverProperties properties)
        //{
        //}

        // Uncomment the method below to handle the event raised when a feature is upgrading.

        //public override void FeatureUpgrading(SPFeatureReceiverProperties properties, string upgradeActionName, System.Collections.Generic.IDictionary<string, string> parameters)
        //{
        //}
        
    }
}
