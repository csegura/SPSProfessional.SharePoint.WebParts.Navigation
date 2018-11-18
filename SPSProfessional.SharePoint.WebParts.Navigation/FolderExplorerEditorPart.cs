using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;
using SPSProfessional.SharePoint.Framework.Tools;


namespace SPSProfessional.SharePoint.WebParts.Navigation
{
    internal class FolderExplorerEditorPart : EditorPart
    {
        private CheckBox chkFollowListView;
        private CheckBox chkNavigateToList;
        private CheckBox chkShowCounter;
        private DropDownList ddlWebparts;
        private TextBox txtExpandDepth;
        private TextBox txtFilter;
        private CheckBox chkHideStartUnder;
        private DropDownList ddlListViews;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            ID = "FolderParamsEditor";
            Title = "FolderExplorer";
        }

        public override bool ApplyChanges()
        {
            EnsureChildControls();
            FolderExplorer webpart = WebPartToEdit as FolderExplorer;

            if (webpart != null)
            {
                webpart.ListGuid = SPSEditorPartsTools.GetListGuidFromListViewGuid(Context,
                                                                               ddlWebparts.SelectedValue);
                webpart.ListViewGuid = ddlWebparts.SelectedValue;
                webpart.FollowListView = chkFollowListView.Checked;
                webpart.NavigateToList = chkNavigateToList.Checked;
                webpart.NavigateToListView = ddlListViews.SelectedValue;
                webpart.ShowCounter = chkShowCounter.Checked;
                webpart.Filter = txtFilter.Text.ToUpper();
                webpart.HideStartUnder = chkHideStartUnder.Checked;

                int converted;
                if (int.TryParse(txtExpandDepth.Text, out converted))
                {
                    webpart.ExpandDepth = converted;
                }

                webpart.ClearControlState();
                webpart.ClearCache();
                return true;
            }
            return false;
        }

        public override void SyncChanges()
        {
            EnsureChildControls();
            FolderExplorer webpart = WebPartToEdit as FolderExplorer;

            if (webpart != null)
            {
               
                chkFollowListView.Checked = webpart.FollowListView;
                chkNavigateToList.Checked = webpart.NavigateToList;
                chkShowCounter.Checked = webpart.ShowCounter;
                txtFilter.Text = webpart.Filter;
                chkHideStartUnder.Checked = webpart.HideStartUnder;
                txtExpandDepth.Text = webpart.ExpandDepth.ToString();

                // Initialize
                chkNavigateToList.Enabled = !chkFollowListView.Checked;
                chkFollowListView.Enabled = !chkNavigateToList.Checked;
                ddlWebparts.Enabled = !chkNavigateToList.Checked;

                if (!string.IsNullOrEmpty(webpart.ListViewGuid))
                {
                    DropDownSelect(ddlWebparts, webpart.ListViewGuid);
                    ddlWebParts_SelectedIndexChanged(null, null);
                    DropDownSelect(ddlListViews, webpart.NavigateToListView);
                }
            }
        }

        private void DropDownSelect(ListControl ddlControl, string value)
        {
            ListItem item = ddlControl.Items.FindByValue(value);
            if (item != null)
            {
                ddlControl.SelectedIndex = ddlControl.Items.IndexOf(item);
            }
        }


        protected override void CreateChildControls()
        {
            ddlWebparts = new DropDownList();
            ddlWebparts.Width = new Unit("100%");
            SPSEditorPartsTools.FillWebParts(Context, ddlWebparts);
            ddlWebparts.SelectedIndexChanged += ddlWebParts_SelectedIndexChanged;
            ddlWebparts.AutoPostBack = true;
            Controls.Add(ddlWebparts);

            txtFilter = new TextBox();
            txtFilter.Width = new Unit("100%");
            Controls.Add(txtFilter);

            chkFollowListView = new CheckBox();
            chkFollowListView.Text = SPSResources.GetResourceString("SPSPE_FollowListView");
            chkFollowListView.Checked = false;
            chkFollowListView.AutoPostBack = true;
            chkFollowListView.CheckedChanged += chkFollowListView_CheckedChanged;
            chkFollowListView.ToolTip = SPSResources.GetResourceString("SPSPE_FollowListViewTip");
            Controls.Add(chkFollowListView);

            chkNavigateToList = new CheckBox();
            chkNavigateToList.Text = SPSResources.GetResourceString("SPSPE_NavigateToList");
            chkNavigateToList.Checked = false;
            chkNavigateToList.AutoPostBack = true;
            chkNavigateToList.CheckedChanged += chkNavigateToList_CheckedChanged;
            chkNavigateToList.ToolTip = SPSResources.GetResourceString("SPSPE_NavigateToListTip");
            Controls.Add(chkNavigateToList);

            ddlListViews = new DropDownList();
            ddlListViews.Width = new Unit("100%");
            Controls.Add(ddlListViews);

            chkShowCounter = new CheckBox();
            chkShowCounter.Text = SPSResources.GetResourceString("SPSPE_ShowFileCounter");
            chkShowCounter.Checked = false;
            chkShowCounter.ToolTip = SPSResources.GetResourceString("SPSPR_ShowFileCounterTip");
            Controls.Add(chkShowCounter);

            txtExpandDepth = new TextBox();
            txtExpandDepth.Width = new Unit("100%");
            Controls.Add(txtExpandDepth);

            chkHideStartUnder = new CheckBox();
            chkHideStartUnder.Text = SPSResources.GetResourceString("SPSPE_HideUnderScoreFolders");
            chkHideStartUnder.Checked = false;
            chkHideStartUnder.ToolTip = SPSResources.GetResourceString("SPSPE_HideUnderScoreFoldersTip");
            Controls.Add(chkHideStartUnder);
        }

        private void chkNavigateToList_CheckedChanged(object sender, EventArgs e)
        {
            chkFollowListView.Enabled = !((CheckBox) sender).Checked;
            ddlWebparts.Enabled = chkFollowListView.Enabled;
            ddlListViews.Enabled = chkNavigateToList.Checked;
            ddlWebParts_SelectedIndexChanged(null, null);
        }

        private void chkFollowListView_CheckedChanged(object sender, EventArgs e)
        {
            chkNavigateToList.Enabled = !((CheckBox) sender).Checked;
        }

        private void ddlWebParts_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (chkNavigateToList.Checked)
            {
                string listName = ddlWebparts.SelectedItem.Text;
                string guid = SPContext.Current.Web.Lists[listName].ID.ToString("B");
                SPSEditorPartsTools.FillListViews(ddlListViews, guid);
            }
        }

        protected override void RenderContents(HtmlTextWriter writer)
        {
            SPSEditorPartsTools tools = new SPSEditorPartsTools(writer);

            tools.DecorateControls(Controls);

            tools.SectionBeginTag();

            tools.SectionHeaderTag(SPSResources.GetResourceString("SPSPE_LinkedTo"));
            ddlWebparts.RenderControl(writer);

            tools.SectionFooterTag();

            tools.SectionHeaderTag(SPSResources.GetResourceString("SPSPE_Filter"));
            txtFilter.RenderControl(writer);
            tools.SectionFooterTag();

            tools.SectionHeaderTag(SPSResources.GetResourceString("SPSPE_Behaviour"));
            chkFollowListView.RenderControl(writer);
            tools.SectionFooterTag();

            tools.SectionHeaderTag();
            chkNavigateToList.RenderControl(writer);
            tools.SectionFooterTag();

            tools.SectionHeaderTag();
            ddlListViews.RenderControl(writer);
            tools.SectionFooterTag();

            tools.SectionHeaderTag();
            chkShowCounter.RenderControl(writer);
            tools.SectionFooterTag();

            tools.SectionHeaderTag(SPSResources.GetResourceString("SPSPE_TreeExpandDepth"));
            txtExpandDepth.RenderControl(writer);
            tools.SectionFooterTag();

            tools.SectionHeaderTag();
            chkHideStartUnder.RenderControl(writer);
            tools.SectionFooterTag();

            tools.SectionEndTag();
        }
    }
}