using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;
using SPSProfessional.SharePoint.Framework.Controls;
using SPSProfessional.SharePoint.Framework.Hierarchy;
using SPSProfessional.SharePoint.Framework.Tools;
using SPSProfessional.SharePoint.Framework.WebPartCache;


namespace SPSProfessional.SharePoint.WebParts.Navigation
{
    public class WebExplorer : SPSWebPart, IWebPartCache
    {
        internal const char SLASH = '/';

        private int _expandDepth = 99;
        private string _filterList = string.Empty;
        private string _filterWeb = string.Empty;
        private string _rootWeb = string.Empty;
        private bool _showLists;
        private bool _showSubSites;
        private bool _showFolders;

        private TreeView treeView;

        public WebExplorer()
        {
            SPSInit("71D70B8F-1556-4d4c-925D-342EE0EE59C0",
                    "Navigation.2.0",
                    "WebExplorer",
                    "http://www.spsprofessional.com/page/Navigation-Web-Parts.aspx");
            EditorParts.Add(new WebExplorerParamsEditor());
        }

        #region CONFIGURATION PROPERTIES

        [Personalizable(PersonalizationScope.Shared)]
        public int ExpandDepth
        {
            get { return _expandDepth; }
            set { _expandDepth = value; }
        }

        [Personalizable(PersonalizationScope.Shared)]
        public override string HelpUrl
        {
            get { return "http://www.spsprofessional.com/page/Folder-Explorer-WebPart.aspx"; }
            set { base.HelpUrl = value; }
        }

        [Personalizable(PersonalizationScope.Shared)]
        public string RootWeb
        {
            get { return _rootWeb; }
            set { _rootWeb = value; }
        }

        [Personalizable(PersonalizationScope.Shared)]
        public bool ShowSubSites
        {
            get { return _showSubSites; }
            set { _showSubSites = value; }
        }

        [Personalizable(PersonalizationScope.Shared)]
        public bool ShowLists
        {
            get { return _showLists; }
            set { _showLists = value; }
        }

        [Personalizable(PersonalizationScope.Shared)]
        public string FilterWeb
        {
            get { return _filterWeb; }
            set { _filterWeb = value; }
        }

        [Personalizable(PersonalizationScope.Shared)]
        public string FilterList
        {
            get { return _filterList; }
            set { _filterList = value; }
        }

        [Personalizable(PersonalizationScope.Shared)]
        public bool ShowFolders
        {
            get { return _showFolders; }
            set { _showFolders = value; }
        }

        #endregion

        protected override void CreateChildControls()
        {
            using(SPWeb web = GetWebToUse())
            {
                try
                {
                    SPSHierarchyFilter dataFilter = new SPSHierarchyFilter
                                                        {
                                                                SortHierarchy = true,
                                                                IncludeLists = _showLists,
                                                                IncludeWebs = _showSubSites,
                                                                IncludeFolders = _showFolders,
                                                                MaxDeepth = 9999
                                                        };

                    if (!string.IsNullOrEmpty(FilterWeb) || !string.IsNullOrEmpty(FilterList))
                    {
                        dataFilter.OnFilter += DataSourceFilter;
                    }

                    using(SPSHierarchyDataSource dataSource = new SPSHierarchyDataSource(web))
                    {
                        dataSource.CacheService = GetCacheService();
                        dataSource.Filter = dataFilter;

                        treeView = SPSTreeViewHelper.MakeTreeView(dataSource, null);
                        treeView.ExpandDepth = ExpandDepth;

                        Controls.Add(treeView);
                    }
                }
                catch (Exception ex)
                {
                    ErrorMessage += ex.Message;
                }
            }
        }

        private SPWeb GetWebToUse()
        {
            SPWeb web = null;
            if (!string.IsNullOrEmpty(_rootWeb))
            {
                try
                {
                    using(SPSite site = new SPSite(_rootWeb))
                    {
                        web = site.OpenWeb();
                    }
                }
                catch (Exception)
                {
                    ErrorMessage += string.Format("<br>" + SPSResources.GetResourceString("SPS_Err_Open_Url"), _rootWeb);
                }
            }
            else
            {
                web = SPContext.Current.Web.Site.OpenWeb();
            }

            return web;
        }


        private bool DataSourceFilter(object sender, SPSHierarchyFilterArgs args)
        {
            if (!string.IsNullOrEmpty(FilterWeb) && (args.Web != null))
            {
                return args.Web.Name.Contains(FilterWeb);
            }

            if (!string.IsNullOrEmpty(FilterList) && (args.List != null))
            {
                return args.List.Title.Contains(FilterList);
            }

            return true;
        }

        protected override void SPSRender(HtmlTextWriter writer)
        {
            treeView.RenderControl(writer);
        }

        #region Implementation of IWebPartCache

        public SPSCacheService GetCacheService()
        {
            return CacheService;
        }

        #endregion
    }
}