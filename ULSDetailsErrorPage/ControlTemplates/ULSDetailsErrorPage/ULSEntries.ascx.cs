using Microsoft.SharePoint;
using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using UCore = ULSError.Core;

namespace ULSError.ControlTemplates
{
    public partial class ULSEntries : UserControl
    {
        
        protected void Page_Load(object sender, EventArgs e)
        {
            UCore.Administration uadmin = new UCore.Administration();
            if (!uadmin.IsVisibleTo(SPContext.Current.Web.CurrentUser))
            {
                //this.Controls.Remove(_ulsentriessb); //dont run call
                //this.Parent.Controls.Remove(this);
                this.Visible = false;
            }

            ulsentriescorrelationid.Value = Guid.NewGuid().ToString("D");
        }
    }
}
