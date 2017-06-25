using System;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;

namespace ULSDetailsErrorPage.Features.ULSAdmin
{
    /// <summary>
    /// This class handles events raised during feature activation, deactivation, installation, uninstallation, and upgrade.
    /// </summary>
    /// <remarks>
    /// The GUID attached to this class may be used during packaging and should not be modified.
    /// </remarks>

    [Guid("d4f268ce-3000-4557-bc66-8108ab4ed468")]
    public class ULSAdminEventReceiver : SPFeatureReceiver
    {
        // Uncomment the method below to handle the event raised after a feature has been activated.

        //public override void FeatureActivated(SPFeatureReceiverProperties properties)
        //{
        //}


        // Uncomment the method below to handle the event raised before a feature is deactivated.

        //public override void FeatureDeactivating(SPFeatureReceiverProperties properties)
        //{
        //}


        // Uncomment the method below to handle the event raised after a feature has been installed.

        public override void FeatureInstalled(SPFeatureReceiverProperties properties)
        {
            var cas = SPAdministrationWebApplication.Local;
            var ca = cas.Sites[0] as SPSite;
            if(ca != null)
            {
                using(var web = ca.OpenWeb())
                {
                    if (!web.AllProperties.ContainsKey(ULSError.Core.Constants.PROPERTY_MAILSERVER))
                    {
                        web.AllProperties.Add(ULSError.Core.Constants.PROPERTY_MAILSERVER, "");
                    }
                    if (!web.AllProperties.ContainsKey(ULSError.Core.Constants.PROPERTY_VISIBLETO))
                    {
                        web.AllProperties.Add(ULSError.Core.Constants.PROPERTY_VISIBLETO, "");
                    }

                    web.Update();
                }
            }
        }


        // Uncomment the method below to handle the event raised before a feature is uninstalled.

        public override void FeatureUninstalling(SPFeatureReceiverProperties properties)
        {
            //var cas = SPAdministrationWebApplication.Local;
            //var ca = cas.Sites[0] as SPSite;
            //if (ca != null)
            //{
            //    using (var web = ca.OpenWeb())
            //    {
            //        if (web.AllProperties.ContainsKey(ULSError.Core.Constants.PROPERTY_MAILSERVER))
            //        {
            //            web.AllProperties.Remove(ULSError.Core.Constants.PROPERTY_MAILSERVER);
            //        }
            //        if (web.AllProperties.ContainsKey(ULSError.Core.Constants.PROPERTY_VISIBLETO))
            //        {
            //            web.AllProperties.Remove(ULSError.Core.Constants.PROPERTY_VISIBLETO);
            //        }

            //        web.Update();
            //    }
            //}
        }

        // Uncomment the method below to handle the event raised when a feature is upgrading.

        //public override void FeatureUpgrading(SPFeatureReceiverProperties properties, string upgradeActionName, System.Collections.Generic.IDictionary<string, string> parameters)
        //{
        //}
    }
}
