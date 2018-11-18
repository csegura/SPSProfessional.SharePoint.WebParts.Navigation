using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;
using SPSProfessional.SharePoint.BuscarControlar;

namespace SPSProfessional.SharePoint.WebParts.Navigation
{
    public class XFolderBreadCrumb : WebPart, IWebEditable
    {
        private const string CurrentFolderParameter = "CurrentFolder";
        private const string RootFolderParameter = "RootFolder";
        private const char SLASH = '/';
        private const string ViewParameter = "View";

        private string _html = string.Empty;

        private string _listGuid = string.Empty;
        private string _listViewGuid = string.Empty;
        private int _maxLevels;
        private bool _navigateToList = false;
        private readonly SPSControlar _controlar;
        private readonly bool _licensed;

        public XFolderBreadCrumb()
        {
            _controlar = new SPSControlar("fcbe46c1-d32a-4723-92bd-51bbc7f619d6", "1.0.0.0");
            _licensed = _controlar.Aceptado();
        }

        #region WEBPART PROPERTIES

        public string ListGuid
        {
            get { return _listGuid; }
            set { _listGuid = value; }
        }

        public string ListViewGuid
        {
            get { return _listViewGuid; }
            set { _listViewGuid = value; }
        }

        public int MaxLevels
        {
            get { return (_maxLevels < 3 ? 3 : _maxLevels); }
            set { _maxLevels = value; }
        }

        public bool NavigateToList
        {
            get { return _navigateToList; }
            set { _navigateToList = value; }
        }

        #endregion

        #region PROPERTIES

        private Guid ListID
        {
            get { return new Guid(_listGuid); }
        }

        private string CurrentFolder
        {
            get
            {
                if (ViewState[CurrentFolderParameter + UniqueID] == null)
                {
                    return NavigationTools.GetRootFolder(ListGuid);
                }
                else
                {
                    return ViewState[CurrentFolderParameter + UniqueID].ToString();
                }
            }
            set { ViewState[CurrentFolderParameter + UniqueID] = value; }
        }

        public override string HelpUrl
        {
            get { return "http://www.spsprofessional.com/webparts/FolderExplorer.aspx"; }
            set { base.HelpUrl = value; }
        }

        #endregion

        #region IWebEditable Members

        EditorPartCollection IWebEditable.CreateEditorParts()
        {
            List<EditorPart> editors = new List<EditorPart>();

            editors.Add(new FolderBreadCrumbParamsEditor());
            editors.Add(new SPSProfessionalAboutEditor());

            return new EditorPartCollection(editors);
        }

        #endregion

        #region WEBPART OVERRIDE MEMBERS

        protected override void OnLoad(EventArgs e)
        {
            if (!string.IsNullOrEmpty(ListGuid) && !string.IsNullOrEmpty(ListViewGuid))
                GetFolderFromQueryString();
        }

        protected override void Render(HtmlTextWriter writer)
        {
            if (_licensed)
            {
                try
                {
                    if (!string.IsNullOrEmpty(ListGuid) && !string.IsNullOrEmpty(ListViewGuid))
                    {
                        GetFolderFromQueryString();
                        GenerateBreadCrumb();
                        if (DesignMode)
                            writer.Write("SPSProfessional - FolderBreadCrumb in design mode.");
                        else
                        {
                            if (NavigationTools.InListContext)
                                _html = "<div class=ms-listdescription>" + _html + "</div>";
                            writer.Write(_html);
                        }
                    }
                    else
                    {
                        writer.Write("SPSProfessional - Missing configuration.<br/>");
                        writer.Write(NavigationTools.ConfigurationToolPaneLink(this));
                    }
                }
                catch (Exception e)
                {
                    throw new SPException(
                        string.Format("Error: {0}<br/>Message: {1}<br/> Trace: {2}<br/>",
                                      e.GetType(),
                                      e.Message,
                                      e));
                }
            }
        }

        #endregion

        /// <summary>
        /// Generates the bread crumb.
        /// </summary>
        private void GenerateBreadCrumb()
        {
            string[] paths = CurrentFolder.Split('/');
            string fullPath = string.Empty;
            int contextStart = NavigationTools.GetCurrentWebLevel(); 
            int start = contextStart;

            if (paths.Length > MaxLevels + contextStart)
            {
                start = paths.Length - MaxLevels;
            }            

            for (int i = 1; i < start && i < paths.Length; i++)
            {
                fullPath += SLASH + paths[i];
            }

            if (paths.Length > MaxLevels + contextStart)
            {
                _html = GenerateLink("... > ", fullPath);
            }
            
            for (int i = start; i < paths.Length; i++)
            {
                string path = paths[i];

                fullPath += SLASH + path;

                if (!path.Equals("Lists"))
                {
                    _html += GenerateLink(path, fullPath);

                    if (i != paths.Length - 1)
                        _html += " > ";
                }
            }

            if (_html.Length == 0)
            {
                _html = GenerateLink(paths[paths.Length-1], fullPath);
            }
        }

        /// <summary>
        /// Gets the folder from query string.
        /// </summary>
        private void GetFolderFromQueryString()
        {
            string folder = Page.Request.QueryString[RootFolderParameter];
            string view = Page.Request.QueryString[ViewParameter];

            if (folder != null && view != null)
            {
                if (SPHttpUtility.UrlKeyValueDecode(view) == ListViewGuid)
                {
                    CurrentFolder = SPHttpUtility.UrlKeyValueDecode(folder);
                }
            }
        }

        /// <summary>
        /// Generates the link.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="fullPath">The full path.</param>
        /// <returns></returns>
        private string GenerateLink(string path, string fullPath)
        {
            string href;
            if (NavigateToList)
                href = GenerateLinkToList(path, fullPath);
            else
                href = GenerateLinkToView(path, fullPath);

            return href;
        }

        /// <summary>
        /// Generates the link to view.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="fullPath">The full path.</param>
        /// <returns></returns>
        private string GenerateLinkToView(string path, string fullPath)
        {
            string hrefArgs = string.Format("{0}?RootFolder={1}&View={2}",
                                            GetCurrentPageUrl(),
                                            SPHttpUtility.UrlKeyValueEncode(fullPath),
                                            SPHttpUtility.UrlKeyValueEncode(ListViewGuid));

            string onclick = string.Format("javascript:EnterFolder('{0}');javascript: return false;", hrefArgs);

            string href;

            if (NavigationTools.InListContext)
                href = string.Format("<a href=\"{0}\">{1}</a>", hrefArgs, path);
            else
                href = string.Format("<a href=\"{0}\" onclick=\"{1}\">{2}</a>", hrefArgs, onclick, path);

            return href;
        }

        /// <summary>
        /// Generates the link to list.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="fullPath">The full path.</param>
        /// <returns></returns>
        private string GenerateLinkToList(string path, string fullPath)
        {
            string listViewUrl = SPContext.Current.Web.Lists[ListID].DefaultViewUrl;
            string hrefArgs = string.Format("{0}?RootFolder={1}",
                                            listViewUrl,
                                            SPHttpUtility.UrlKeyValueEncode(fullPath));
            string href = string.Format("<a href=\"{0}\">{1}</a>", hrefArgs, path);
            return href;
        }

        /// <summary>
        /// Gets the current page URL.
        /// </summary>
        /// <returns></returns>
        private string GetCurrentPageUrl()
        {
            string currentPageUrl = Page.Request.Url.ToString();
            if (currentPageUrl.IndexOf('?') > 0)
            {
                currentPageUrl = currentPageUrl.Substring(0, currentPageUrl.IndexOf('?'));
            }
            return currentPageUrl;
        }

        private bool ListIsDocumentLibrary()
        {
            return SPContext.Current.Web.Lists[ListID].BaseType == SPBaseType.DocumentLibrary ? true : false;                
        }


    }
}