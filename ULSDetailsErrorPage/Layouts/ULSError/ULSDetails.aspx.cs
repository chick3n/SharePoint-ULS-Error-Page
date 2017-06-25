using System;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using ULSError.Core;
using System.Collections.Generic;
using System.Web.Script.Serialization;
using System.Linq;

namespace ULSError.Layouts.ULSError
{
    public partial class ULSDetails : LayoutsPageBase
    {
        private Guid CorrelationId { get; set; }

        private bool CanViewLogs()
        {
            return new Core.Administration().IsVisibleTo(SPContext.Current.Web.CurrentUser);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            Response.Clear();
            Response.ContentType = "application/json; charset=utf-8";
            string cId = Request.QueryString["correlationid"];

            if (String.IsNullOrEmpty(cId))
            {
                Response.Write(GetResponse());
                return;
            }

            Guid _correlationId;
            if(!Guid.TryParse(cId, out _correlationId))
            {
                Response.Write(GetResponse());
                return;
            }

            if(!CanViewLogs())
            {
                Response.Write(GetResponse());
                return;
            }

            CorrelationId = _correlationId;
            var response = GetULSEntries();
            Response.Write(GetResponse(response));
        }

        private DetailsResult GetULSEntries()
        {
            if(CorrelationId == Guid.Empty)
            {
                return DetailsResult.Empty();
            }

            var entries = new ULS().FindLogs(CorrelationId);
            if(entries == null)
            {
                return DetailsResult.Empty();
            }

            var response = new DetailsResult();
            response.Entries = entries.Take(ULS.Limit-1).ToList();
            response.More = entries.Count >= ULS.Limit;

            return response;
        }

        private string GetResponse(DetailsResult response = null)
        {
            if(response == null)
            {
                response = DetailsResult.Empty();
            }

            return new JavaScriptSerializer().Serialize(response);
        }
        
    }

    internal class DetailsResult
    {
        public static DetailsResult Empty()
        {
            return new DetailsResult
            {
                Entries = new List<Entry>(),
                More = false
            };
        }

        public List<Entry> Entries { get; set; }
        public bool More { get; set; }
    }
}
