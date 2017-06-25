using Microsoft.SharePoint;
using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using UCore = ULSError.Core;

namespace ULSError.ControlTemplates
{
    public partial class Notify : UserControl
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            UCore.Administration uadmin = new Core.Administration();
            var settings = uadmin.GetSettings();
            if(settings == null || string.IsNullOrEmpty(settings.MailTo))
            {
                //this.Parent.Controls.Remove(this);
                this.Visible = false;
            }

            ulsnotifycorrelationid.Value = Guid.NewGuid().ToString("D");
        }
        
    }
}
