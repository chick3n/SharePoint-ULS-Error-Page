<%@ Assembly Name="Microsoft.SharePoint.ApplicationPages, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c"%> 
<%@ Page Language="C#" Inherits="Microsoft.SharePoint.ApplicationPages.ErrorPage" MasterPageFile="~/_layouts/15/errorv15.master"       %> 
<%@ Import Namespace="Microsoft.SharePoint.ApplicationPages" %> 
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Import Namespace="Microsoft.SharePoint" %> 
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="wssuc" TagName="FoldHyperLink" src="~/_controltemplates/15/FoldHyperLink.ascx" %>
<%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register TagPrefix="uls" Assembly="ULSErrorPage, Version=1.0.0.0, Culture=neutral, PublicKeyToken=1407c8269c2130be" Namespace="ULSError.ControlTemplates" %>
<%@ Register TagPrefix="uls" TagName="ULSEntries" Src="~/_controltemplates/15/ULSDetailsErrorPage/ULSEntries.ascx" %>
<%@ Register TagPrefix="uls" TagName="Notify" Src="~/_controltemplates/15/ULSDetailsErrorPage/ULSNotify.ascx" %>
<%@ Import Namespace="Microsoft.SharePoint" %> 
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<asp:Content ContentPlaceHolderId="PlaceHolderPageTitle" runat="server">
	<SharePoint:EncodedLiteral runat="server" text="<%$Resources:wss,error_pagetitle%>" EncodeMethod='HtmlEncode'/>
</asp:Content>
<asp:Content ContentPlaceHolderId="PlaceHolderPageTitleInTitleArea" runat="server">
	<asp:Panel id="ErrorPageTitlePanel" runat="server">
	</asp:Panel>
</asp:Content>
<asp:Content contentplaceholderid="PlaceHolderAdditionalPageHead" runat="server">
	<meta name="Robots" content="NOINDEX " />
	<meta name="SharePointError" content="0" />
</asp:Content>
<asp:Content ContentPlaceHolderId="PlaceHolderMain" runat="server">
    <input type="hidden" value="" id="ulserror_correlationid" />
	<asp:PlaceHolder runat="server" id="ErrorPageInfo"></asp:PlaceHolder>
	<WebPartPages:AllowFraming runat="server" />
	<div>
		<SharePoint:FormattedString id="LabelMessage" EncodeMethod="HtmlEncodeAllowSimpleTextFormatting" runat="server">
			<asp:HyperLink id="LinkContainedInMessage" runat="server"/>
		</SharePoint:FormattedString>
	</div>
    <uls:Notify ID="ULSNotify" runat="server" />
    
	<asp:Panel id="FoldPanel" class="ms-error-detailsFold" runat="server">
		<wssuc:FoldHyperLink id="FoldLink" runat="server"
			LinkTitleWhenFoldOpened="<%$Resources:wss,error_pagetechieDetails%>" >
			<div>
				<p>
					<span class="ms-descriptiontext">
					<asp:HyperLink id="AdditionalHelpLink" Visible="false" runat="server"/>
					</span>
				</p>
				<p>
					<span class="ms-descriptiontext">
						<asp:Panel id="WSSCentralAdmin_TroubleshootPanel" runat="server">
							<SharePoint:FormattedString id="helptopic_WSSCentralAdmin_Troubleshoot" FormatText="<%$Resources:wss,helptopic_link%>" EncodeMethod="NoEncode" runat="server"> <SharePoint:EncodedLiteral runat="server" text="<%$Resources:wss,troubleshoot_issues%>" EncodeMethod='HtmlEncode'/> <SharePoint:EncodedLiteral runat="server" text='WSSCentralAdmin_Troubleshoot' EncodeMethod='EcmaScriptStringLiteralEncode'/> </SharePoint:FormattedString>
						</asp:Panel>
						<asp:Panel id="WSSEndUser_troubleshootingPanel" runat="server">
							<SharePoint:FormattedString id="helptopic_WSSEndUser_troubleshooting" FormatText="<%$Resources:wss,helptopic_link%>" EncodeMethod="NoEncode" runat="server"> <SharePoint:EncodedLiteral runat="server" text="<%$Resources:wss,troubleshoot_issues%>" EncodeMethod='HtmlEncode'/> <SharePoint:EncodedLiteral runat="server" text='WSSEndUser_troubleshooting' EncodeMethod='EcmaScriptStringLiteralEncode'/> </SharePoint:FormattedString>
						</asp:Panel>
					</span>
				</p>
				<p>
					<asp:Label CssClass="ms-metadata" ID="RequestGuidText" Runat="server" />
				</p>
				<p>
					<asp:Label CssClass="ms-metadata" ID="DateTimeText" Runat="server" />
				</p>
                <p>
                    <uls:ULSEntries ID="ULSEntries" runat="server" />
                </p>
			</div>
		</wssuc:FoldHyperLink>
        <br />

	</asp:Panel>
	<div class="ms-error-techMsg">
		<hr />
	</div>
	<SharePoint:ScriptBlock runat="server">
        function _getCorrelationId() {
            var hdn = document.getElementById('ulserror_correlationid');
            return (hdn ? hdn.value : '');
        }

		var gearPage = document.getElementById('ms-loading-box');
		if(null != gearPage)
		{
			gearPage.parentNode.removeChild(gearPage);
			document.title = "<SharePoint:EncodedLiteral runat='server' text='<%$Resources:wss,error_pagetitle%>' EncodeMethod='HtmlEncode'/>";
		}
		function _spBodyOnLoad()
		{
			var intialFocus = (document.getElementById("<%= MoreDetailsLink.ClientID %>"));
			try
			{
				intialFocus.focus();
			}
			catch(ex)
			{
			}

            var loadUls = document.getElementById('ulserror_correlationid');
            if(loadUls) {
                var requestText = '<%= RequestGuidText.Text %>';
                var matches = requestText.match(/[a-f0-9]{8}(?:-[a-f0-9]{4}){3}-[a-f0-9]{12}/i);
                if(matches) {
                    loadUls.value =matches[0];
                }
            }
		}
		function _onmessage(e)
		{
			if (e && window.JSON)
			{
				var origin = e.origin;
				var data = e.data;
				if (window.console && window.console.log)
				{
					console.log("ErrorPage.OnMessage: Origin=" + origin + ", Data=" + data);
				}
				var requestInfo = JSON.parse(data);
				if (requestInfo && (requestInfo.command == 'Ping' || requestInfo.command == 'Query'))
				{
					var requestGuidElem = (document.getElementById("<%= RequestGuidText.ClientID %>"));
					var responseInfo = {};
					responseInfo.command = requestInfo.command;
					responseInfo.postMessageId = requestInfo.postMessageId;
					responseInfo.responseAvailable = false;
					responseInfo.errorCode = -1007;
					var errorMessage;
					if (requestGuidElem)
					{
						errorMessage = requestGuidElem.textContent;
						if (typeof(errorMessage) == "undefined")
						{
							errorMessage = requestGuidElem.innerText;
						}
					}
					if (typeof(errorMessage) == "undefined")
					{
						errorMessage = "Error";
					}
					responseInfo.errorMessage = errorMessage;
					if (window.parent && window.parent.postMessage)
					{
						data = JSON.stringify(responseInfo);
						if (window.console && window.console.log)
						{
							console.log("ErrorPage.PostMessage: Origin=" + origin + ", Data=" + data);
						}
						window.parent.postMessage(data, origin);
					}
				}
			}
		}
		if (window.addEventListener) {
			window.addEventListener('message', _onmessage, false);
		}
		else if (window.attachEvent) {
			window.attachEvent('onmessage', _onmessage);
		}
	</SharePoint:ScriptBlock>
</asp:Content>
