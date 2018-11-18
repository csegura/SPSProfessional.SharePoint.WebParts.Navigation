using System;
using System.Web.UI;
using System.Web.UI.WebControls.WebParts;
using SPSProfessional.SharePoint.Framework.Controls;
using SPSProfessional.SharePoint.Framework.WebPartCache;

namespace SPSProfessional.SharePoint.WebParts.Navigation
{
    public class FolderExplorer : SPSWebPart, IWebPartCache
    {
        private bool _autoCollapse;

        private bool _expandAll;
        private int _expandDepth = 99;
        private string _filter;
        private bool _followListView;
        private string _listGuid = string.Empty;
        private string _listViewGuid = string.Empty;
        private bool _navigateToList;
        private bool _showCounter;
        private bool _hideStartUnder;
        private string _navigateToListView;

        private SPSExplorer.FolderExplorerControl _folderExplorerControl;

        public FolderExplorer()
        {
            SPSInit("71D70B8F-1556-4d4c-925D-342EE0EE59C0",
                    "Navigation.2.0",
                    "FolderExplorer",
                    "http://www.spsprofessional.com/page/Navigation-Web-Parts.aspx");

            EditorParts.Add(new FolderExplorerEditorPart());
        }

        #region CONFIGURATION PROPERTIES

        [Personalizable(PersonalizationScope.Shared)]
        public string ListGuid
        {
            get { return _listGuid; }
            set { _listGuid = value; }
        }

        [Personalizable(PersonalizationScope.Shared)]
        public string ListViewGuid
        {
            get { return _listViewGuid; }
            set { _listViewGuid = value; }
        }

        [Personalizable(PersonalizationScope.Shared)]
        public int ExpandDepth
        {
            get { return _expandDepth; }
            set { _expandDepth = value; }
        }

        [Personalizable(PersonalizationScope.Shared)]
        public bool ExpandAll
        {
            get { return _expandAll; }
            set { _expandAll = value; }
        }

        [Personalizable(PersonalizationScope.Shared)]
        public bool FollowListView
        {
            get { return _followListView; }
            set { _followListView = value; }
        }

        [Personalizable(PersonalizationScope.Shared)]
        public bool NavigateToList
        {
            get { return _navigateToList; }
            set { _navigateToList = value; }
        }

        [Personalizable(PersonalizationScope.Shared)]
        public string NavigateToListView
        {
            get { return _navigateToListView; }
            set { _navigateToListView = value; }
        }

        [Personalizable(PersonalizationScope.Shared)]
        public bool ShowCounter
        {
            get { return _showCounter; }
            set { _showCounter = value; }
        }

        [Personalizable(PersonalizationScope.Shared)]
        public bool AutoCollapse
        {
            get { return _autoCollapse; }
            set { _autoCollapse = value; }
        }

        [Personalizable(PersonalizationScope.Shared)]
        public string Filter
        {
            get { return _filter; }
            set { _filter = value; }
        }

        [Personalizable(PersonalizationScope.Shared)]
        public bool HideStartUnder
        {
            get { return _hideStartUnder; }
            set { _hideStartUnder = value; }
        }

        #endregion

        protected override void CreateChildControls()
        {
            if (CheckParameters())
            {
                try
                {
                    _folderExplorerControl = new SPSExplorer.FolderExplorerControl(ListGuid, ListViewGuid)
                                                 {
                                                         ShowCounter = ShowCounter,
                                                         HideUnderscoreFolders = HideStartUnder,
                                                         Filter = Filter,
                                                         FollowListView = FollowListView,
                                                         NavigateToList = NavigateToList,
                                                         NavigateToListView = NavigateToListView,
                                                         ExpandDepth = ExpandDepth,
                                                         CacheService = GetCacheService()
                                                 };

                    Controls.Add(_folderExplorerControl);
                }
                catch (Exception ex)
                {
                    ErrorMessage += ex.Message;
                }
            }
        }

        private bool CheckParameters()
        {
            return !string.IsNullOrEmpty(ListGuid);
        }


        protected override void SPSRender(HtmlTextWriter writer)
        {
            if (CheckParameters())
            {
                _folderExplorerControl.RenderControl(writer);
            }
            else
            {
                writer.Write(MissingConfiguration);
            }
        }

        #region Implementation of IWebPartCache

        public SPSCacheService GetCacheService()
        {
            return base.CacheService;
        }

        #endregion
    }
}