using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace ULSError.Core
{
    public class Resource
    {
        public static string Read(string key, string _default = null)
        {
            var value = SPUtility.GetLocalizedString("$Resources:ulserrorpage, " + key, "ulserrorpage", (uint)SPContext.Current.Web.Locale.LCID);
            if (value == null && _default != null)
                return _default;
            return value != null ? value.ToString() : string.Empty;
        }
    }
}
