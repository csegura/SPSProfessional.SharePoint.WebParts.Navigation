using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using SPSProfessional.SharePoint.Framework.Tools;


namespace SPSProfessional.SharePoint.WebParts.Navigation
{
    internal class WebExplorerParamsEditor : EditorPart
    {
        private CheckBox chkShowLists;
        private CheckBox chkShowSubsites;
        private CheckBox chkShowFolders;
        private TextBox txtExpandDepth;
        private TextBox txtFilterList;
        private TextBox txtFilterWeb;
        private TextBox txtRootWeb;
        
        public WebExplorerParamsEditor()
        {
            ID = "WebExplorerParamsEditor";
            Title = "WebExplorer";
        }

        public override bool ApplyChanges()
        {
            EnsureChildControls();
            WebExplorer webpart = WebPartToEdit as WebExplorer;

            if (webpart != null)
            {
                webpart.RootWeb = txtRootWeb.Text;
                webpart.ShowSubSites = chkShowSubsites.Checked;
                webpart.ShowLists = chkShowLists.Checked;
                webpart.ShowFolders = chkShowFolders.Checked;
                webpart.FilterWeb = txtFilterWeb.Text;
                webpart.FilterList = txtFilterList.Text;

                int converted;
                if (int.TryParse(txtExpandDepth.Text, out converted))
                    webpart.ExpandDepth = converted;

                webpart.ClearControlState();
                webpart.ClearCache();

                return true;
            }
            return false;
        }

        public override void SyncChanges()
        {
            EnsureChildControls();
            WebExplorer webpart = WebPartToEdit as WebExplorer;

            if (webpart != null)
            {
                txtRootWeb.Text = webpart.RootWeb;
                chkShowSubsites.Checked = webpart.ShowSubSites;
                chkShowLists.Checked = webpart.ShowLists;
                chkShowFolders.Checked = webpart.ShowFolders;
                txtFilterWeb.Text = webpart.FilterWeb;
                txtFilterList.Text = webpart.FilterList;
                txtExpandDepth.Text = webpart.ExpandDepth.ToString();
            }
        }

        protected override void CreateChildControls()
        {
            txtRootWeb = new TextBox();
            txtRootWeb.Width = new Unit("100%");
            Controls.Add(txtRootWeb);

            chkShowSubsites = new CheckBox();
            chkShowSubsites.Text = SPSResources.GetResourceString("SPSPE_ShowSubsites");
            chkShowSubsites.Checked = false;
            Controls.Add(chkShowSubsites);

            chkShowLists = new CheckBox();
            chkShowLists.Text = SPSResources.GetResourceString("SPSPE_ShowLists");
            chkShowLists.Checked = false;
            Controls.Add(chkShowLists);

            chkShowFolders = new CheckBox();
            chkShowFolders.Text = SPSResources.GetResourceString("SPSPE_ShowFolders");
            chkShowFolders.Checked = false;
            Controls.Add(chkShowFolders);

            txtFilterWeb = new TextBox();
            txtFilterWeb.Width = new Unit("100%");
            Controls.Add(txtFilterWeb);

            txtFilterList = new TextBox();
            txtFilterList.Width = new Unit("100%");
            Controls.Add(txtFilterList);

            txtExpandDepth = new TextBox();
            txtExpandDepth.Width = new Unit("100%");
            Controls.Add(txtExpandDepth);
        }

        protected override void RenderContents(HtmlTextWriter writer)
        {
            SPSEditorPartsTools tools = new SPSEditorPartsTools(writer);

            tools.DecorateControls(Controls);

            tools.SectionBeginTag();

            tools.SectionHeaderTag(SPSResources.GetResourceString("SPSPE_TopUrl"));
            txtRootWeb.RenderControl(writer);
            tools.SectionFooterTag();

            tools.SectionHeaderTag();
            chkShowSubsites.RenderControl(writer);
            tools.SectionFooterTag();

            tools.SectionHeaderTag();
            chkShowLists.RenderControl(writer);
            tools.SectionFooterTag();

            tools.SectionHeaderTag();
            chkShowFolders.RenderControl(writer);
            tools.SectionFooterTag();

            tools.SectionHeaderTag(SPSResources.GetResourceString("SPSPE_FilterWeb"));
            txtFilterWeb.RenderControl(writer);
            tools.SectionFooterTag();

            tools.SectionHeaderTag(SPSResources.GetResourceString("SPSPE_FilterList"));
            txtFilterList.RenderControl(writer);
            tools.SectionFooterTag();

            tools.SectionHeaderTag(SPSResources.GetResourceString("SPSPE_TreeExpandDepth"));
            txtExpandDepth.RenderControl(writer);
            tools.SectionFooterTag();

            tools.SectionEndTag();
        }
    }
}