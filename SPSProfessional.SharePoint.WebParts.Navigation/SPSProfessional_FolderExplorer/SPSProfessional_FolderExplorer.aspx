<%@page masterpagefile="~/_layouts/application.master" language="C#"%>
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=12.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> <%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=12.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> <%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Import Namespace="Microsoft.SharePoint.ApplicationPages" %>
<%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=12.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="wssuc" TagName="Welcome" src="~/_controltemplates/Welcome.ascx" %>
<%@ Register TagPrefix="wssuc" TagName="DesignModeConsole" src="~/_controltemplates/DesignModeConsole.ascx" %>
<%@ Register Assembly="SPSProfessional.SharePoint.WebParts.Navigation, Version=1.0.1030.2136, Culture=neutral, PublicKeyToken=4031063ddba1c7c7"
    Namespace="SPSProfessional.SharePoint.WebParts.Navigation" TagPrefix="cc1" %>

<asp:Content ID="Content1" contentplaceholderid="PlaceHolderPageTitle" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderId="PlaceHolderPageTitleInTitleArea" runat="server">
Folder Explorer  
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderId="PlaceHolderPageDescription" runat="server">
<asp:Label ID="lblError" runat="server" CssClass="ms-formvalidation" Text="Label" Visible="False" Width="571px"></asp:Label>
</asp:Content>

<asp:Content id="Content4" runat="server" contentplaceholderid="PlaceHolderMain">
<table width="100%">
<tr><td>
<div class=ms-WPBody>
<cc1:FolderExplorerControl id="FolderExplorerControl1" runat="server">
</cc1:FolderExplorerControl>
</div>
</td></tr></table>
</asp:Content>
