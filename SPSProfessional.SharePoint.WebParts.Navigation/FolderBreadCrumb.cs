using System.Web.UI;
using System.Web.UI.WebControls.WebParts;
using SPSProfessional.SharePoint.Framework.Controls;
using SPSProfessional.SharePoint.WebParts.SPSExplorer;

namespace SPSProfessional.SharePoint.WebParts.Navigation
{
    public class FolderBreadCrumb : SPSWebPart
    {
        private string _listGuid = string.Empty;
        private string _listViewGuid = string.Empty;
        private int _maxLevels;
        private bool _navigateToList;
        private string _navigateToListView = string.Empty;

        private BreadCrumbControl _breadCrumbControl;

        public FolderBreadCrumb()
        {
            SPSInit("71D70B8F-1556-4d4c-925D-342EE0EE59C0",
                    "Navigation.2.0",
                    "FolderBreadCrumb",
                    "http://www.spsprofessional.com/page/Navigation-Web-Parts.aspx");

            EditorParts.Add(new FolderBreadCrumbEditorPart());
        }

        #region WebPart Properties

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
        public int MaxLevels
        {
            get { return (_maxLevels < 3 ? 3 : _maxLevels); }
            set { _maxLevels = value; }
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

        #endregion

        #region WebPart Overrides      

        protected override void CreateChildControls()
        {
            if (CheckParameters())
            {
                _breadCrumbControl = new BreadCrumbControl(ListGuid, ListViewGuid)
                                         {
                                                 MaxLevels = MaxLevels,
                                                 NavigateToList = NavigateToList,
                                                 NavigateToListView = NavigateToListView
                                         };

                Controls.Add(_breadCrumbControl);
            }
            base.CreateChildControls();
        }

        protected override void SPSRender(HtmlTextWriter writer)
        {
            if (CheckParameters())
            {
                _breadCrumbControl.RenderControl(writer);
            }
            else
            {
                writer.Write(MissingConfiguration);
            }
        }

        #endregion

        #region Private Methods

        private bool CheckParameters()
        {
            return !string.IsNullOrEmpty(ListGuid) && !string.IsNullOrEmpty(ListViewGuid);
        }

        #endregion
    }
}