using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;
using Microsoft.SharePoint.WebControls;
using SPSProfessional.SharePoint.Framework.Hierarchy;
using SPSProfessional.SharePoint.Framework.Tools;
using SPSProfessional.SharePoint.Framework.WebPartCache;

namespace SPSProfessional.SharePoint.WebParts.Navigation
{
    [ToolboxData("<{0}:FolderExplorerControl runat=server></{0}:FolderExplorerControl>")]
    public class FolderExplorerControl : SPControl
    {
        private SPSHierarchyDataSource dataSource;
        private TreeView treeView;


        public string ListGuid
        {
            get { return SPHttpUtility.UrlKeyValueDecode(Page.Request.QueryString["ListId"]); }
        }

        private Guid ListID
        {
            get { return new Guid(ListGuid); }
        }


        public override void RenderControl(HtmlTextWriter writer)
        {
            try
            {        
                SPSControlar controlator = new SPSControlar("71D70B8F-1556-4d4c-925D-342EE0EE59C0",
                                                            "Navigation.2.0");
                if (controlator.Aceptado())
                {
                    if (!string.IsNullOrEmpty(ListGuid))
                    {
                        treeView.ExpandAll();
                        treeView.RenderControl(writer);
                    }
                    else
                    {
                        writer.Write("SPSProfessional - Missing configuration.<br/>");
                    }
                }                
            }
            catch (Exception e)
            {
                writer.Write(string.Format("Error: {0}<br/>Message: {1}<br/> Trace: {2}<br/>",
                                           e.GetType(),
                                           e.Message,
                                           e));
            }
        }

        /// <exception cref="SPException"><c>SPException</c>.</exception>
        protected override void CreateChildControls()
        {
            if (!string.IsNullOrEmpty(ListGuid))
            {
                try
                {
                    SPWeb web = SPContext.Current.Web;

                    using (dataSource = new SPSHierarchyDataSource(web, web.Lists[ListID]))
                    {
                        SPSHierarchyFilter dataFilter = new SPSHierarchyFilter
                                                            {
                                                                    IncludeFolders = true,
                                                                    IncludeNumberOfFiles = true,
                                                                    IncludeWebs = false,
                                                                    IncludeLists = false,
                                                                    SortHierarchy = true
                                                            };

                        dataSource.Filter = dataFilter;

                        treeView = SPSTreeViewHelper.MakeTreeView(dataSource, null);

                        Controls.Add(treeView);
                    }
                }
                catch (Exception e)
                {
                    throw new SPException(
                        string.Format("Error: {0}\nMessage:{1}\nTrace:\n{2}",
                                      e.GetType(),
                                      e.Message,
                                      e));
                }
            }
        }

    }
}