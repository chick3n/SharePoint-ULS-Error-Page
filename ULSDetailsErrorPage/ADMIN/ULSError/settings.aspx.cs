using System;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using UCore = ULSError.Core;
using System.Linq;
using System.Collections.Generic;

namespace ULSError.Administration.Page
{
    public partial class FeatureSettings : LayoutsPageBase
    {
        
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                UCore.Administration admin = new Core.Administration();
                var settings = admin.GetSettings();
                if (settings != null)
                {
                    txtMailTo.Text = settings.MailTo;
                    var visibleToUsers = settings.VisibleTo?.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries) 
                        ?? new string[0];
                    foreach (var user in visibleToUsers)
                    {
                        var usersplit = user.Split(new char[] { ':' }, 2, StringSplitOptions.RemoveEmptyEntries);
                        if (usersplit.Length == 2)
                        {
                            PickerEntity entity = new PickerEntity();
                            entity.IsResolved = false;
                            entity.DisplayText = usersplit[0];
                            entity.Key = usersplit[1];
                            ppVisibleTo.AllEntities.Add(entity);
                        }
                    }

                    ppVisibleTo.Validate();
                    
                }
            }
        }

        /// <summary>
        /// Cancel button is clicked, return user back to central admin
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Cancel_Click(object sender, EventArgs e)
        {
            Response.Redirect(SPContext.Current.Web.Url);

        }

        /// <summary>
        /// Save button is clicked, save current settings to property bag
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Save_Click(object sender, EventArgs e)
        {
            UCore.Administration admin = new Core.Administration();
            UCore.Settings settings = new Core.Settings();
            settings.MailTo = txtMailTo.Text;

            var users = string.Join(";", ppVisibleTo.AllEntities.Where(x => x.IsResolved).Select(x => x.DisplayText + ":" + x.Key));
            settings.VisibleTo = users;

            var results = admin.Update(settings);
            if(results)
            {
                Response.Redirect(SPContext.Current.Web.Url);
                return;
            }

            ErrorText.Visible = true;
        }
    }
}
