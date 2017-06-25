using Microsoft.SharePoint;
using Microsoft.SharePoint.Diagnostics;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;


namespace ULSError.Core
{
    internal class ULS
    {
        public static readonly int Limit = 100;
        public List<Entry> FindLogs(Guid correlationId)
        {
            if(correlationId == Guid.Empty)
            {
                return new List<Entry>();
            }

            IList<LogFileEntry> logentries = null;

            SPSecurity.RunWithElevatedPrivileges(delegate ()
            {
                var ulsadmin = new SPULSRetriever(180, Limit, DateTime.Now.AddMinutes(-180));
                logentries = ulsadmin.GetULSEntries(correlationId);
            });

            var entries = logentries.Select(x => new Entry()
            {
                Message = x.Message,
                Severity = x.Level,
                Time = x.Timestamp
            }).ToList();

            return entries;
        }
        
    }
    

    internal class Entry
    {
        public string Message { get; set; }
        public string Severity { get; set; }
        public DateTime Time { get; set; }
    }
}
