<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Import Namespace="Microsoft.SharePoint.ApplicationPages" %>
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="settings.aspx.cs" Inherits="ULSError.Administration.Page.FeatureSettings" DynamicMasterPageFile="~masterurl/default.master" %>

<asp:Content ID="PageHead" ContentPlaceHolderID="PlaceHolderAdditionalPageHead" runat="server">

</asp:Content>

<asp:Content ID="Main" ContentPlaceHolderID="PlaceHolderMain" runat="server">
    <div runat="server" class="ms-descriptiontext">
        <asp:Label runat="server" ID="DescriptionText" Text=""></asp:Label>
        <asp:Label runat="server" ID="ErrorText" ForeColor="Red"></asp:Label>
    </div>
    <table class="ms-formtable" style="margin-top: 8px;" border="0" cellpadding="10" cellspacing="0">
        <tbody>
            <tr>
                <td valign="top" style="width:40%">
                    <asp:Label runat="server" Text="<%$ Resources:ulserrorpage, MailTo %>" />
                    <p>
                        <asp:Literal runat="server" Text="<%$ Resources:ulserrorpage, MailToHelp %>" />
                    </p>
                </td>
                <td valign="top">
                    <asp:TextBox runat="server" ID="txtMailTo" Width="400px" />
                </td>
            </tr>
            <tr>
                <td valign="top">
                    <span style="color:red">*</span> <asp:Label runat="server" Text="<%$ Resources:ulserrorpage, VisibleTo %>" />
                    <p>
                        <asp:Literal runat="server" Text="<%$ Resources:ulserrorpage, VisibleToHelp %>" />
                    </p>
                </td>
                <td valign="top">
                    <SharePoint:ClientPeoplePicker Required="true" ID="ppVisibleTo" ValidationEnabled="true" PrincipalAccountType="User,SecGroup,SPGroup" runat="server" AllowMultipleEntities="true"
                        CssClass="ms-long ms-spellcheck-true user-block" ErrorMessage="" Rows="3"  />
                </td>
            </tr>
        </tbody>
    </table>
    <div style="text-align: right">
        <asp:Button runat="server" ID="Save" Text="<%$ Resources:ulserrorpage, Save %>" OnClick="Save_Click" CssClass="ms-ButtonHeightWidth" />
        <asp:Button runat="server" ID="Cancel" Text="<%$ Resources:ulserrorpage, Cancel %>" OnClick="Cancel_Click"  CssClass="ms-ButtonHeightWidth" />
    </div>
</asp:Content>

<asp:Content ID="PageTitle" ContentPlaceHolderID="PlaceHolderPageTitle" runat="server">
    <SharePoint:EncodedLiteral runat="server" Text="<%$ Resources:ulserrorpage, SettingsTitle %>" EncodeMethod='HtmlEncode'/>
</asp:Content>

<asp:Content ID="PageTitleInTitleArea" ContentPlaceHolderID="PlaceHolderPageTitleInTitleArea" runat="server" >
    <asp:Literal runat="server" Text="<%$ Resources:ulserrorpage, SettingsTitle%>" />
</asp:Content>
