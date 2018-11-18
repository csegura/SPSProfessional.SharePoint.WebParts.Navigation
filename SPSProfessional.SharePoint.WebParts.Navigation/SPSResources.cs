using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;

namespace SPSProfessional.SharePoint.WebParts.Navigation
{
    internal class SPSResources
    {
        public static string GetResourceString(string key)
        {
            const string resourceClass = "SPSProfessional.SharePoint.WebParts.Navigation";
            uint lang = SPContext.Current.Web.Language;
            string value = SPUtility.GetLocalizedString("$Resources:" + key, resourceClass, lang);
            return value;
        }
    }
}
