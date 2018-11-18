using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Web.UI;
using System.Web.UI.Adapters;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebPartPages;
using SPSProfessional.SharePoint.BuscarControlar;
using WebPart=System.Web.UI.WebControls.WebParts.WebPart;

namespace SPSProfessional.SharePoint.WebParts.Navigation
{
    public class SPSWebPartDbg : WebPart
    {
        private const string ERR_LICENSE_FILE = "Please check the license file.";
        private const string ERR_MISSING_CONF = "Check webpart properties in tool panel.";
        private const string SPS_DESIGNMODE = "In design Mode.";
        private const string SPS_MESSAGE = "<span><font color={0}>{1} - {2}<br/>{3}</font></span>";
        private const string SPSPROPARTS = "<b>spsProParts</b>";

        private readonly List<EditorPart> _editorParts;
        private string _errorMessage;
        private string _spsGuid;
        private string _spsHelpUrl = "http://www.spsprofessional.net/";
        private string _spsPartName;
        private string _spsVersion;

        #region CONSTRUCTORS

        public SPSWebPartDbg()
        {
            _editorParts = new List<EditorPart>();
            _errorMessage = string.Empty;
        }

        public void SPSInit(string spsGuid, string spsVersion, string spsName, string spsHelpUrl)
        {
            _spsGuid = spsGuid;
            _spsVersion = spsVersion;
            _spsPartName = spsName;
            _spsHelpUrl = spsHelpUrl;
        }

        #endregion

        #region SPS PROPERTIES

        public List<EditorPart> EditorParts
        {
            get { return _editorParts; }
        }

        public string ErrorMessage
        {
            get { return _errorMessage; }
            set { _errorMessage = value; }
        }

        private bool CheckLicense
        {
            get
            {
                SPSControlar controlar = new SPSControlar(_spsGuid, _spsVersion);
                return controlar.Aceptado();
            }
        }

        protected string MissingConfiguration
        {
            get
            {
                string configureLink =
                    string.Format("<a href=\"javascript:{0};\">{1}</a>",
                                  ToolPane.GetShowExtensibleToolPaneEvent(string.Format(@"'{0}'", UniqueID)),
                                  ERR_MISSING_CONF);
                return string.Format(SPS_MESSAGE,
                                     "DarkBlue",
                                     SPSPROPARTS,
                                     _spsPartName,
                                     configureLink.Replace("ExtensibleView", "Edit"));
            }
        }

        #endregion

        /// <summary>
        /// Gets or sets the URL to a Help file for a <see cref="T:System.Web.UI.WebControls.WebParts.WebPart"/> 
        /// control.
        /// </summary>
        /// <value></value>
        /// <returns>A string that represents the URL to a Help file. The default value is an empty string ("").</returns>
        /// <exception cref="T:System.ArgumentException">The internal validation system has determined that the URL might contain script attacks.</exception>
        public override string HelpUrl
        {
            get { return _spsHelpUrl; }
            set { base.HelpUrl = value; }
        }


        /// <summary>
        /// In the constructor add the custom editor Parts
        /// EditorParts.Add(new xxxEditor());
        /// </summary>
        /// <returns></returns>
        public override EditorPartCollection CreateEditorParts()
        {
            EditorParts.Add(new SPSProfessionalAboutEditor());
            return new EditorPartCollection(EditorParts);
        }

        /// <summary>
        /// Internally we must use SPSRender method
        /// </summary>
        /// <param name="writer"></param>
        protected override void Render(HtmlTextWriter writer)
        {
            Debug.WriteLine("Render");

            if (Page.IsPostBack)
                Debug.WriteLine("Postback");
            else
                Debug.WriteLine("No Postback");

            // Check Design Mode
            if (DesignMode)
            {
                writer.Write(SPS_MESSAGE, "blue", SPSPROPARTS, _spsPartName, SPS_DESIGNMODE);
            }
            else
            {
                // Check SPS License
                if (CheckLicense)
                {
                    try
                    {
                        EnsureChildControls();
                        SPSRender(writer);

                        // Display errors if any
                        if (!string.IsNullOrEmpty(ErrorMessage))
                        {
                            writer.Write(SPS_MESSAGE, "red", SPSPROPARTS, _spsPartName, ErrorMessage);
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new SPException(
                            string.Format("Error: {0}\nMessage: {1}\n Trace: {2}\n",
                                          ex.GetType(),
                                          ex.Message,
                                          ex));
                    }
                }
                else
                {
                    writer.Write(SPS_MESSAGE, "red", SPSPROPARTS, _spsPartName, ERR_LICENSE_FILE);
                }
            }
            //base.Render(writer);
        }

        /// <summary>
        /// Our render, override in child classes
        /// </summary>
        /// <param name="writer">The writer.</param>
        protected virtual void SPSRender(HtmlTextWriter writer)
        {
        }

        /// <summary>
        /// Clears the viewstate of child controls.
        /// Call here from EditorPart ApplyChanges 
        /// </summary>
        public void ClearControlState()
        {
            Debug.WriteLine("ClearViewState");
            // Clear the control state
            ClearChildControlState();
            // Save the empty state 
            SaveControlState();
            // Discard controls (force creation)
            ChildControlsCreated = false;
        }

        #region Wrapper for Debug

        protected override void OnConnectModeChanged(EventArgs e)
        {
            Debug.WriteLine("OnConnectModeChanged");
            try
            {
                base.OnConnectModeChanged(e);
            }
            catch (Exception ex)
            {
                ErrorMessage += ex.Message;
            }
        }

        protected override void OnClosing(EventArgs e)
        {
            Debug.WriteLine("OnClosing");
            try
            {
                base.OnClosing(e);
            }
            catch (Exception ex)
            {
                ErrorMessage += ex.Message;
            }
        }

        protected override void OnDeleting(EventArgs e)
        {
            Debug.WriteLine("OnDeleting");
            try
            {
                base.OnDeleting(e);
            }
            catch (Exception ex)
            {
                ErrorMessage += ex.Message;
            }
        }

        protected override void OnEditModeChanged(EventArgs e)
        {
            Debug.WriteLine("OnEditModeChanged");
            try
            {
                base.OnEditModeChanged(e);
            }
            catch (Exception ex)
            {
                ErrorMessage += ex.Message;
            }
        }

        protected override void TrackViewState()
        {
            Debug.WriteLine("TrackViewState");
            try
            {
                base.TrackViewState();
            }
            catch (Exception ex)
            {
                ErrorMessage += ex.Message;
            }
        }

        public override void DataBind()
        {
            Debug.WriteLine("DataBind");
            try
            {
                base.DataBind();
            }
            catch (Exception ex)
            {
                ErrorMessage += ex.Message;
            }
        }

        protected override void AddAttributesToRender(HtmlTextWriter writer)
        {
            Debug.WriteLine("AddAttributesToRender");
            try
            {
                base.AddAttributesToRender(writer);
            }
            catch (Exception ex)
            {
                ErrorMessage += ex.Message;
            }
        }

        protected override Style CreateControlStyle()
        {
            Debug.WriteLine("CreateControlStyle");
            Style style = null;
            try
            {
                style = base.CreateControlStyle();
                
            }
            catch (Exception ex)
            {
                ErrorMessage += ex.Message;
            }

            return style;
        }

        public override void RenderBeginTag(HtmlTextWriter writer)
        {
            Debug.WriteLine("RenderBeginTag");
            try
            {
                base.RenderBeginTag(writer);
            }
            catch (Exception ex)
            {
                ErrorMessage += ex.Message;
            }
        }

        public override void RenderEndTag(HtmlTextWriter writer)
        {
            Debug.WriteLine("RenderEndTag");
            try
            {
                base.RenderEndTag(writer);
            }
            catch (Exception ex)
            {
                ErrorMessage += ex.Message;
            }
        }

        protected override void LoadViewState(object savedState)
        {
            Debug.WriteLine("LoadViewState");
            try
            {
                base.LoadViewState(savedState);
            }
            catch (Exception ex)
            {
                ErrorMessage += ex.Message;
            }
        }

        protected override void RenderContents(HtmlTextWriter writer)
        {
            Debug.WriteLine("RenderContents");
            try
            {
                base.RenderContents(writer);
            }
            catch (Exception ex)
            {
                ErrorMessage += ex.Message;
            }
        }

        protected override object SaveViewState()
        {
            Debug.WriteLine("SaveViewState");
            Object o =
            null;
            try
            {
                o = base.SaveViewState();
            }
            catch (Exception ex)
            {
                ErrorMessage += ex.Message;
            }
            return o;
        }

        protected override ControlAdapter ResolveAdapter()
        {
            Debug.WriteLine("ResolveAdapter");
            ControlAdapter controlAdapter = null;
            try
            {
                controlAdapter = base.ResolveAdapter();
            }
            catch (Exception ex)
            {
                ErrorMessage += ex.Message;
            }
            return controlAdapter;
        }

        public override void ApplyStyleSheetSkin(Page page)
        {
            Debug.WriteLine("ApplyStyleSheetSkin");
            try
            {
                base.ApplyStyleSheetSkin(page);
            }
            catch (Exception ex)
            {
                ErrorMessage += ex.Message;
            }
        }

        protected override void OnDataBinding(EventArgs e)
        {
            Debug.WriteLine("OnDataBinding");
            try
            {
                base.OnDataBinding(e);
            }
            catch (Exception ex)
            {
                ErrorMessage += ex.Message;
            }
        }

        protected override void DataBind(bool raiseOnDataBinding)
        {
            Debug.WriteLine("DataBind");
            try
            {
                base.DataBind(raiseOnDataBinding);
            }
            catch (Exception ex)
            {
                ErrorMessage += ex.Message;
            }
        }

        protected override void DataBindChildren()
        {
            Debug.WriteLine("DataBindChildren");
            try
            {
                base.DataBindChildren();
            }
            catch (Exception ex)
            {
                ErrorMessage += ex.Message;
            }
        }

        protected override void AddParsedSubObject(object obj)
        {
            Debug.WriteLine("AddParsedSubObject");
            try
            {
                base.AddParsedSubObject(obj);
            }
            catch (Exception ex)
            {
                ErrorMessage += ex.Message;
            }
        }

        protected override void OnInit(EventArgs e)
        {
            Debug.WriteLine("OnInit");
            if (Page.IsPostBack)
                Debug.WriteLine("Postback");
            else
                Debug.WriteLine("No Postback");
            try
            {
                base.OnInit(e);
            }
            catch (Exception ex)
            {
                ErrorMessage += ex.Message;
            }
        }

        public override void Focus()
        {
            Debug.WriteLine("Focus");
            try
            {
                base.Focus();
            }
            catch (Exception ex)
            {
                ErrorMessage += ex.Message;
            }
        }

        protected override void LoadControlState(object savedState)
        {
            Debug.WriteLine("LoadControlState");
            try
            {
                base.LoadControlState(savedState);
            }
            catch (Exception ex)
            {
                ErrorMessage += ex.Message;
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            Debug.WriteLine("OnLoad");
            try
            {
                base.OnLoad(e);
            }
            catch (Exception ex)
            {
                ErrorMessage += ex.Message;
            }
        }

        protected override void OnPreRender(EventArgs e)
        {
            Debug.WriteLine("OnPreRender");
            try
            {
                base.OnPreRender(e);
            }
            catch (Exception ex)
            {
                ErrorMessage += ex.Message;
            }
        }

        protected override object SaveControlState()
        {
            Debug.WriteLine("SaveControlState");
            Object o = null;
            try
            {
                o = base.SaveControlState();
            }
            catch (Exception ex)
            {
                ErrorMessage += ex.Message;
            }
            return o;
        }

        protected override void RenderChildren(HtmlTextWriter writer)
        {
            Debug.WriteLine("RenderChildren");
            try
            {
                base.RenderChildren(writer);
            }
            catch (Exception ex)
            {
                ErrorMessage += ex.Message;
            }
        }

        public override void RenderControl(HtmlTextWriter writer)
        {
            Debug.WriteLine("RenderControl");
            try
            {
                base.RenderControl(writer);
            }
            catch (Exception ex)
            {
                ErrorMessage += ex.Message;
            }
        }

        protected override void OnUnload(EventArgs e)
        {
            Debug.WriteLine("OnUnload");
            try
            {
                base.OnUnload(e);
            }
            catch (Exception ex)
            {
                ErrorMessage += ex.Message;
            }
        }

        public override void Dispose()
        {
            Debug.WriteLine("Dispose");
            try
            {
                base.Dispose();
            }
            catch (Exception ex)
            {
                ErrorMessage += ex.Message;
            }
        }

        protected override bool OnBubbleEvent(object source, EventArgs args)
        {
            Debug.WriteLine("OnBubbleEvent");
            bool b = false;
            try
            {
                b = base.OnBubbleEvent(source, args);
            }
            catch (Exception ex)
            {
                ErrorMessage += ex.Message;
            }
            return b;
        }

        protected override void AddedControl(Control control, int index)
        {
            Debug.WriteLine("AddedControl");
            try
            {
                base.AddedControl(control, index);
            }
            catch (Exception ex)
            {
                ErrorMessage += ex.Message;
            }
        }

        protected override ControlCollection CreateControlCollection()
        {
            Debug.WriteLine("CreateControlCollection");
            ControlCollection controlCollection = null;
            try
            {
                controlCollection = base.CreateControlCollection();
            }
            catch (Exception ex)
            {
                ErrorMessage += ex.Message;
            }
            return controlCollection;
        }

        protected override void CreateChildControls()
        {
            Debug.WriteLine("CreateChildControls");
            try
            {
                base.CreateChildControls();
            }
            catch (Exception ex)
            {
                ErrorMessage += ex.Message;
            }
        }

        public override Control FindControl(string id)
        {
            Debug.WriteLine("FindControl");
            Control control = null;
            try
            {
                control = base.FindControl(id);
            }
            catch (Exception ex)
            {
                ErrorMessage += ex.Message;
            }
            return control;
        }

        protected override Control FindControl(string id, int pathOffset)
        {
            Debug.WriteLine("FindControl");
            Control control = null;
            try
            {
                control = base.FindControl(id, pathOffset);
            }
            catch (Exception ex)
            {
                ErrorMessage += ex.Message;
            }
            return control;
        }

        protected override IDictionary GetDesignModeState()
        {
            Debug.WriteLine("GetDesignModeState");
            IDictionary dictionary = null;
            try
            {
                dictionary =  base.GetDesignModeState();
            }
            catch (Exception ex)
            {
                ErrorMessage += ex.Message;
            }
            return dictionary;
        }

        public override bool HasControls()
        {
            Debug.WriteLine("HasControls");
            bool b = false;
            try
            {
                b = base.HasControls();
            }
            catch (Exception ex)
            {
                ErrorMessage += ex.Message;
            }
            return b;
        }

        protected override void EnsureChildControls()
        {
            Debug.WriteLine("EnsureChildControls");
            try
            {
                base.EnsureChildControls();
            }
            catch (Exception ex)
            {
                ErrorMessage += ex.Message;
            }
        }

        protected override void RemovedControl(Control control)
        {
            Debug.WriteLine("RemovedControl");
            try
            {
                base.RemovedControl(control);
            }
            catch (Exception ex)
            {
                ErrorMessage += ex.Message;
            }
        }

        protected override void SetDesignModeState(IDictionary data)
        {
            Debug.WriteLine("SetDesignModeState");
            try
            {
                base.SetDesignModeState(data);
            }
            catch (Exception ex)
            {
                ErrorMessage += ex.Message;
            }
        }

        #endregion
    }
}