using System;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;

namespace ULSError.Layouts.ULSError
{
    public partial class Test : LayoutsPageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            throw new SPException("ULS Error Details Page test error.");
        }
    }
}
