using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;
using SPSProfessional.SharePoint.Framework.Tools;


namespace SPSProfessional.SharePoint.WebParts.Navigation
{
    internal class FolderBreadCrumbEditorPart : EditorPart
    {
        private DropDownList ddlWebparts;
        private TextBox txtMaxLevels;
        private CheckBox chkNavigateToList;
        private DropDownList ddlListViews;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            ID = "FolderBreadCrumbEditorPart";
            Title = "FolderBreadCrumb";
        }

        public override bool ApplyChanges()
        {
            EnsureChildControls();
            FolderBreadCrumb webpart = WebPartToEdit as FolderBreadCrumb;

            if (webpart != null)
            {
                webpart.ListGuid = SPSEditorPartsTools.GetListGuidFromListViewGuid(Context, ddlWebparts.SelectedValue);
                webpart.ListViewGuid = ddlWebparts.SelectedValue;

                int converted;
                if (int.TryParse(txtMaxLevels.Text, out converted))
                {
                    webpart.MaxLevels = converted;
                }

                webpart.NavigateToList = chkNavigateToList.Checked;
                webpart.NavigateToListView = ddlListViews.SelectedValue;
                return true;
            }
            return false;
        }

        public override void SyncChanges()
        {
            EnsureChildControls();
            FolderBreadCrumb webpart = WebPartToEdit as FolderBreadCrumb;

            if (webpart != null)
            {

                txtMaxLevels.Text = webpart.MaxLevels.ToString();
                chkNavigateToList.Checked = webpart.NavigateToList;

                if (!string.IsNullOrEmpty(webpart.ListViewGuid) && ddlWebparts.Items.Count > 1)
                {
                    DropDownSelect(ddlWebparts,webpart.ListViewGuid);
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

            txtMaxLevels = new TextBox();
            txtMaxLevels.Width = new Unit("100%");
            txtMaxLevels.Text = string.Empty;
            Controls.Add(txtMaxLevels);

            chkNavigateToList = new CheckBox();
            chkNavigateToList.Text = SPSResources.GetResourceString("SPSPE_NavigateToList");
            chkNavigateToList.Checked = false;
            chkNavigateToList.CheckedChanged += chkNavigateToList_CheckedChanged;
            chkNavigateToList.AutoPostBack = true;
            Controls.Add(chkNavigateToList);

            ddlListViews = new DropDownList();
            ddlListViews.Width = new Unit("100%");
            Controls.Add(ddlListViews);
        }

        private void chkNavigateToList_CheckedChanged(object sender, EventArgs e)
        {
            ddlListViews.Enabled = chkNavigateToList.Checked;
            ddlWebParts_SelectedIndexChanged(null, null);
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

            tools.SectionHeaderTag(SPSResources.GetResourceString("SPSPE_MaxLevels"));
            txtMaxLevels.RenderControl(writer);
            tools.SectionFooterTag();

            tools.SectionHeaderTag();
            chkNavigateToList.RenderControl(writer);
            tools.SectionFooterTag();

            tools.SectionHeaderTag(SPSResources.GetResourceString("SPSPE_NavigateToListView"));
            ddlListViews.RenderControl(writer);
            tools.SectionFooterTag();

            tools.SectionEndTag();
        }
    }
}