using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebPartPages;
using WebPart=System.Web.UI.WebControls.WebParts.WebPart;

namespace SPSProfessional.SharePoint.WebParts.Navigation.Tools
{
    internal static class NavigationTools
    {
        private static readonly char SLASH = '/';

        /// <summary>
        /// Check if we are in a ListContext
        /// </summary>
        /// <returns></returns>
        internal static bool InListContext
        {
            get { return (SPContext.Current.List != null ? true : false); }
        }

        /// <summary>
        /// Get the root folder for a list or document library
        /// The root folder is diferent if we are in a list context
        /// </summary>
        /// <param name="listGuid">List guid to get the root folder</param>
        /// <returns>The root folder</returns>
        internal static string GetRootFolder(string listGuid)
        {
            string rootFolder = string.Empty;

            if (!string.IsNullOrEmpty(listGuid))
            {
                SPWeb web = SPContext.Current.Web;
                Guid listID = new Guid(listGuid);

                if (InListContext)
                {
                    rootFolder = SLASH + web.Lists[listID].Title;
                }
                else
                {
                    rootFolder = web.ServerRelativeUrl + SLASH + web.Lists[listID].Title;
                }
            }
            //Trace.WriteLine("GetRootFolder:" + rootFolder);
            return rootFolder;
        }

        /// <summary>
        /// Used by EditorParts to fill in a dropdown control with all site lists
        /// </summary>
        /// <param name="dropDownList">Control to fill</param>
        internal static void FillLists(DropDownList dropDownList)
        {
            SPWeb web = SPContext.Current.Web;

            if (InListContext)
            {
                dropDownList.Items.Add(
                    new ListItem(
                        SPContext.Current.List.Title,
                        SPContext.Current.List.ID.ToString()
                        )
                    );
            }
            else
            {
                foreach (SPList list in web.Lists)
                {
                    if (!list.Hidden)
                    {
                        dropDownList.Items.Add(new ListItem(list.Title, list.ID.ToString()));
                    }
                }
            }
        }


        /// <summary>
        /// Get the ListGuid from a ListViewWebPart that contains a specified ViewGuid
        /// </summary>
        /// <param name="context">Current context</param>
        /// <param name="viewGuid">View Guid</param>
        /// <returns>The Guid of the List, null if not found</returns>
        internal static string GetListGuidFromListViewGuid(HttpContext context, string viewGuid)
        {
            SPWeb curWeb = SPContext.Current.Web;

            using (SPLimitedWebPartManager webpartManager =
                curWeb.GetLimitedWebPartManager(context.Request.Url.ToString(),
                                                PersonalizationScope.Shared))
            {
                foreach (WebPart webpart in webpartManager.WebParts)
                {
                    ListViewWebPart listViewWebPart = webpart as ListViewWebPart;

                    // if the list is a ListView WebPart
                    if (listViewWebPart != null)
                    {
                        if (((ListViewWebPart) webpart).ViewGuid == viewGuid)
                        {
                            return ((ListViewWebPart) webpart).ListName;
                        }
                    }
                    webpart.Dispose();
                }
            }

            return null;
        }

        /// <summary>
        /// Used by EditorParts to fill a ListBox or a DropDownList whith the names of
        /// ListViewWebParts currently loaded in the same page
        /// </summary>
        /// <param name="context">HttpContext, we need the request url</param>
        /// <param name="listControl">Control to fill</param>
        internal static void FillWebParts(HttpContext context, ListControl listControl)
        {
            try
            {
                SPLimitedWebPartManager webpartManager;

                SPWeb curWeb = SPContext.Current.Web;

                using (webpartManager =
                       curWeb.GetLimitedWebPartManager(context.Request.Url.ToString(),
                                                       PersonalizationScope.Shared))
                {
                    foreach (WebPart webpart in webpartManager.WebParts)
                    {
                        ListViewWebPart listViewWebPart = webpart as ListViewWebPart;

                        // if the list is a ListView WebPart
                        if (listViewWebPart != null)
                        {
                            listControl.Items.Add(new ListItem(webpart.Title, ((ListViewWebPart) webpart).ViewGuid));
                        }
                    }
                }
            }
            catch (Exception)
            {
                listControl.Items.Add(new ListItem("(unavailable)", ""));
            }
        }

        /// <summary>
        /// Used by EditorParts to fill a ListBox or a DropDownList whith the names of
        /// ListViewWebParts currently loaded in the same page and are using a determinated list 
        /// </summary>
        /// <param name="context">HttpContext, we need the request url</param>
        /// <param name="listID">Sharepoint list ID related to the view</param>
        /// <param name="listControl">Control to fill</param>
        internal static void FillWebPartsForList(HttpContext context, string listID, ListControl listControl)
        {
            try
            {
                SPLimitedWebPartManager webpartManager;

                listControl.Items.Clear();

                SPWeb curWeb = SPContext.Current.Web;

                using (webpartManager =
                       curWeb.GetLimitedWebPartManager(context.Request.Url.ToString(),
                                                       PersonalizationScope.Shared))
                {
                    listID = new Guid(listID).ToString("B").ToUpper();

                    foreach (WebPart webpart in webpartManager.WebParts)
                    {
                        ListViewWebPart listViewWebPart = webpart as ListViewWebPart;

                        // if the list is a ListView WebPart
                        if (listViewWebPart != null)
                        {
                            // if the view reference the list
                            if (listViewWebPart.ListName == listID)
                            {
                                listControl.Items.Add(new ListItem(webpart.Title, listViewWebPart.ViewGuid));
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                listControl.Items.Add(new ListItem("(unavailable)", ""));
            }
        }

        /// <summary>
        /// Number of paths levels in the relative url
        /// </summary>
        /// <returns>number of path levels</returns>
        internal static int GetCurrentWebLevel()
        {
            string relativeUrl = SPContext.Current.Web.ServerRelativeUrl;
            //Trace.WriteLine("GetCurrentWebLevel:" + relativeUrl.Split(SLASH).Length);
            int level = relativeUrl.Split(SLASH).Length;
            //if (InListContext)
            //    level -= 1;
            return level;
        }

        internal static string ConfigurationToolPaneLink(Control webpart)
        {
            string configureLink =
                string.Format("<a href=\"javascript:{0};\">Check webpart properties in tool panel.</a>",
                              ToolPane.GetShowExtensibleToolPaneEvent(string.Format(@"'{0}'", webpart.UniqueID)));
            return configureLink.Replace("ExtensibleView", "Edit");
        }
    }
}