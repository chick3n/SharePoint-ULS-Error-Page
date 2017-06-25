using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;
using Microsoft.SharePoint.Utilities;
using Microsoft.SharePoint.WebControls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using ULSError.Core;

namespace ULSError.Core
{
    internal class Administration
    {
        private string _mailTo;
        private string _visibleTo;
        private bool _fromCache;

        public SPWeb GetContext()
        {
            SPWeb context = null;
            SPSecurity.RunWithElevatedPrivileges(delegate ()
            {
                try
                {
                    var app = SPWebApplication.Context;
                    if (app != null)
                    {
                        var site = GetRootSiteFromApplication(app);
                        if (site != null)
                        {
                            context = site.OpenWeb();
                        }
                    }
                }
                catch(SPException ex)
                {
                    Logger.WriteLog(Logger.Category.High, "ULS.GetContext", ex.Message);
                    context = null;
                }

            });

            return context;
        }

        private void SetCached(Settings settings)
        {
            if (settings.MailTo == null)
            {
                HttpRuntime.Cache.Insert(Constants.PROPERTY_MAILSERVER, string.Empty);
            } 
            else
            {
                HttpRuntime.Cache.Insert(Constants.PROPERTY_MAILSERVER, settings.MailTo);
            }

            if(settings.VisibleTo == null)
            {
                HttpRuntime.Cache.Insert(Constants.PROPERTY_VISIBLETO, string.Empty);
            }
            else
            {
                HttpRuntime.Cache.Insert(Constants.PROPERTY_VISIBLETO, settings.VisibleTo);
            }            
        }

        private Settings GetCached()
        {
            _mailTo = HttpRuntime.Cache.Get(Constants.PROPERTY_MAILSERVER)?.ToString();
            _visibleTo = HttpRuntime.Cache.Get(Constants.PROPERTY_VISIBLETO)?.ToString();

            var settings = new Settings();
            settings.MailTo = _mailTo;
            settings.VisibleTo = _visibleTo;

            if(_mailTo != null && _visibleTo != null)
            {
                _fromCache = true;
            }

            return settings;
        }

        /// <summary>
        /// Get all the site properties for the setttings
        /// </summary>
        /// <param name="create">if true will create any missing properties</param>
        /// <returns></returns>
        public Settings GetSettings(SPWeb context = null, bool cached = true)
        {            

            var dispose = context == null;
            //var settings = cached ? GetCached() : new Settings();
            var settings = new Settings();
            //if (_fromCache && cached)
            //    return settings;

            if(context == null)
            {
                context = GetContext();
                if(context == null)
                {
                    return settings;
                }
            }

            settings.MailTo = GetMailTo(context);
            settings.VisibleTo = GetVisibleTo(context);

            if(dispose)
            {
                context.Dispose();
            }
            //SetCached(settings);

            return settings;
        }

        public bool Update(Settings settings)
        {
            if(settings == null)
            {
                return false;
            }


            var context = GetContext();
            if (context != null)
            {
                SetMailTo(context, settings.MailTo);
                SetVisibleTo(context, settings.VisibleTo);
                context.Update();
                context.Dispose();

                var currentSettings = GetSettings(cached: false); //new context to validate
                var results = true;
                if (currentSettings.MailTo != settings.MailTo)
                {
                    results = false;
                    Logger.WriteLog(Logger.Category.High, nameof(Administration), string.Format("Update mail to {0} to {1} failed.", currentSettings.MailTo, settings.MailTo));
                }

                if (currentSettings.VisibleTo != settings.VisibleTo)
                {
                    results = false;
                    Logger.WriteLog(Logger.Category.High, nameof(Administration), string.Format("Update visible to {0} to {1} failed.", currentSettings.VisibleTo, settings.VisibleTo));
                }

                if (results)
                {
                    UpdateWebApplications(settings);
                }


                return results;
            }

            return false;
        }

        private void UpdateWebApplications(Settings settings)
        {
            var services = SPFarm.Local.Services;
            var featureId = new Guid("acee21c4-259c-4ec9-a806-7361f762bd0d");
            foreach(var service in services)
            {
                if(service is SPWebService)
                {
                    SPWebService wService = service as SPWebService;
                    foreach(SPWebApplication app in wService.WebApplications)
                    {
                        var feature = app.Features[featureId];
                        if(feature != null)
                        {
                            var site = GetRootSiteFromApplication(app);
                            if(site != null)
                            {
                                using(var web = site.OpenWeb())
                                {
                                    SetMailTo(web, settings.MailTo);
                                    SetVisibleTo(web, settings.VisibleTo);
                                    web.Update();
                                }
                            }
                        }
                    }
                }
            }
        }

        public static SPSite GetRootSiteFromApplication(SPWebApplication app)
        {
            foreach (SPSite site in app.Sites)
            {
                if (site.ServerRelativeUrl.Equals("/"))
                {
                    return site;
                }
            }

            return null;
        }

        public string GetMailTo(SPWeb context)
        {
            if(context.AllProperties.ContainsKey(Constants.PROPERTY_MAILSERVER))
            {
                return context.AllProperties[Constants.PROPERTY_MAILSERVER].ToString();
            }

            return string.Empty;
        }

        public bool IsVisibleTo(SPUser currentUser)
        {
            var isVisible = false;

            if (currentUser.IsSiteAdmin || currentUser.UserToken.IsSystemAccount)
            {
                return true;
            }

            var context = GetContext();
            var settings = GetSettings(context);
            if (settings != null)
            {
                if (settings.VisibleTo.Length > 0)
                {
                    var visibleToUsers = settings.VisibleTo.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var user in visibleToUsers)
                    {
                        var usersplit = user.Split(new char[] { ':' }, 2, StringSplitOptions.RemoveEmptyEntries);
                        if (usersplit.Length == 2)
                        {
                            SPPrincipalInfo visiblePrincipal = null;
                            var key = usersplit[1];
                            try
                            {
                                visiblePrincipal = SPUtility.ResolvePrincipal(context, key, SPPrincipalType.All, SPPrincipalSource.All, null, false);
                            }
                            catch (Exception) { };

                            if (visiblePrincipal == null)
                            {
                                continue;
                            }
                            else if (visiblePrincipal.PrincipalType == SPPrincipalType.SecurityGroup || visiblePrincipal.PrincipalType == SPPrincipalType.SharePointGroup)
                            {
                                isVisible = isMemberOfGroup(context, visiblePrincipal.LoginName, currentUser.LoginName);
                            }
                            else if (visiblePrincipal.PrincipalType == SPPrincipalType.User)
                            {
                                if (visiblePrincipal.LoginName.ToLower().Equals(currentUser.LoginName.ToLower()))
                                {
                                    isVisible = true;
                                }
                            }

                            if (isVisible)
                            {
                                break;
                            }
                        }
                    }
                }
            }

            context.Dispose();
            return isVisible;
        }

        private bool isMemberOfGroup(SPWeb context, string groupName, string currentUserName)
        {
            bool maxed;
            var domainGroupUsers = SPUtility.GetPrincipalsInGroup(context, groupName, 100, out maxed);
            foreach(var domainGroupUser in domainGroupUsers)
            {
                if(domainGroupUser.PrincipalType == SPPrincipalType.SecurityGroup)
                {
                    if(isMemberOfGroup(context, domainGroupUser.LoginName, currentUserName))
                    {
                        return true;
                    }
                }
                else if(domainGroupUser.LoginName.ToLower().Equals(currentUserName.ToLower()))
                {
                    return true;
                }
            }

            return false;
        }

        public void SetMailTo(SPWeb context, string value)
        {
            if (context.AllProperties.ContainsKey(Constants.PROPERTY_MAILSERVER))
            {
                context.AllProperties[Constants.PROPERTY_MAILSERVER] = value.ToString();
            }
            else
            {
                context.AllProperties.Add(Constants.PROPERTY_MAILSERVER, value.ToString());
            }

            
        }

        public string GetVisibleTo(SPWeb context)
        {
            if (context.AllProperties.ContainsKey(Constants.PROPERTY_VISIBLETO))
            {
                return context.AllProperties[Constants.PROPERTY_VISIBLETO].ToString();
            }

            return string.Empty;
        }

        public void SetVisibleTo(SPWeb context, string value)
        {
            if (context.AllProperties.ContainsKey(Constants.PROPERTY_VISIBLETO))
            {
                context.AllProperties[Constants.PROPERTY_VISIBLETO] = value;
            }
            else
            {
                context.AllProperties.Add(Constants.PROPERTY_VISIBLETO, value);
            }
            
        }
    }
    

    internal class Settings
    {
        public string MailTo { get; set; }

        public string VisibleTo { get; set; }
    }
    

}
