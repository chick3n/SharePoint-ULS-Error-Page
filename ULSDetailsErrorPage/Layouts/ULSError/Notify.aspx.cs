using System;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using UCore = ULSError.Core;
using System.Collections.Generic;
using Microsoft.SharePoint.Utilities;
using System.Text;
using System.Web;
using System.Linq;

namespace ULSError.Layouts.ULSError
{
    public partial class Notify : LayoutsPageBase
    {
        private Guid CorrelationId { get; set; }

        private bool CanMailLogs(UCore.Settings settings)
        {
            if (settings == null)
                return false;
            return !string.IsNullOrEmpty(settings.MailTo);
        }

        private string GenerateResponse(bool sent)
        {
            return "{ \"sent\": " + sent.ToString().ToLower() + " }";
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            Response.Clear();
            Response.ContentType = "application/json; charset=utf-8";
            bool sent = false;
            string cId = Request.QueryString["correlationid"];

            if (String.IsNullOrEmpty(cId))
            {
                Response.Write(GenerateResponse(sent));
                return;
            }

            Guid _correlationId;
            if (!Guid.TryParse(cId, out _correlationId))
            {
                Response.Write(GenerateResponse(sent));
                return;
            }

            UCore.Administration admin = new Core.Administration();
            var settings = admin.GetSettings();
            if (!CanMailLogs(settings))
            {
                Response.Write(GenerateResponse(sent));
                return;
            }

            CorrelationId = _correlationId;
            var entries = GetULSEntries();

            if(entries.Count == 0)
            {
                sent = true;
            }
            else
            {
                sent = SendMail(settings, entries);
            }

            Response.Write(GenerateResponse(sent));
        }

        /// <summary>
        /// Builds real URL considering layouts pages.
        /// </summary>
        private Uri CurrentUrl
        {
            get
            {
                return Request.Url.ToString().ToLower().Contains("_layouts")
                    ? new Uri(
                        SPContext.Current.Site.WebApplication.GetResponseUri(
                            SPContext.Current.Site.Zone).ToString().TrimEnd('/')
                        + Request.RawUrl)
                    : Request.Url;
            }
        }

        private bool SendMail(UCore.Settings settings, List<UCore.Entry> entries)
        {
            if(entries == null || settings == null)
            {
                return false;
            }

            string subject = UCore.Resource.Read("MailSubject") + SPContext.Current.Web.Title;
            StringBuilder message = new StringBuilder();
            //message.AppendLine();
            message.AppendFormat("<b>{3}</b> {0}<br><b>{4}</b> {1}<br><b>{5}</b> {2}<br><br>{6}<br>",
                SPContext.Current.Web.CurrentUser.Email,
                CurrentUrl.ToString(),
                CorrelationId.ToString("D"),
                UCore.Resource.Read("MailFrom"),
                UCore.Resource.Read("MailSite"),
                UCore.Resource.Read("MailId"),
                UCore.Resource.Read("MailTitle"));
            message.AppendFormat("<table border=\"1\" cellpadding=\"4\" style=\"border-collapse:collapse\"><thead><tr><th width=\"10%\">{0}</th><th>{1}</th></tr></thead><tbody>",
                UCore.Resource.Read("ULSLevel"),
                UCore.Resource.Read("ULSMessage"));
            for(int x=0; x<entries.Count && x < UCore.ULS.Limit-1; x++) 
            {
                var entry = entries[x];
                message.AppendFormat("<tr><td valign=\"top\">{0}</td><td valign=\"top\">{1}</td></tr>", entry.Severity, entry.Message);
            }
            message.Append("</tbody></table>");
            if(entries.Count >= UCore.ULS.Limit)
            {
                message.Append("<div>...</div>");
            }

            try
            {
                return SPUtility.SendEmail(SPContext.Current.Web, true, false, settings.MailTo, subject, message.ToString());
            }
            catch (SPException ex)
            {
                UCore.Logger.WriteLog(Core.Logger.Category.High, "ULSErrorNotification", ex.Message);
                return false;
            }
        }

        private List<UCore.Entry> GetULSEntries()
        {
            if (CorrelationId == Guid.Empty)
            {
                return new List<UCore.Entry>();
            }

            return new UCore.ULS().FindLogs(CorrelationId);
        }
    }
}
