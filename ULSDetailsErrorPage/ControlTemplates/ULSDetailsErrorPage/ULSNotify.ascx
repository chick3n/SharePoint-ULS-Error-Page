<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %> 
<%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ULSNotify.ascx.cs" Inherits="ULSError.ControlTemplates.Notify" %>

<div class="ms-error-detailsFold">
    <input type="hidden" runat="server" id="ulsnotifycorrelationid" value="" />
    <a href="Javascript:;" ID="ulsnotifylink" class="ms-commandLink" onclick="_notify();">
        <asp:Literal runat="server" Text="<%$ Resources:ulserrorpage, SendNotification%>" />
    </a>
    

    <SharePoint:ScriptBlock runat="server">
        function _notify() {
            var corId = _getCorrelationId();
            var href = document.getElementById('ulsnotifylink');
            if(href) {
                href.innerText = '<%= ULSError.Core.Resource.Read("WaitNotification") %>';
                href.onclick = function () { return false; };
                href.style.cursor = 'pointer';

                var req = new XMLHttpRequest();
                var _onload = function() {
                    if(req.readyState != 4)
                        return false;
                    if(req.status === 200 && (req.response || req.responseText)) {
                            var resp = req.response || req.responseText || '{}';
                            var result = JSON.parse(resp);
                            if(result.sent === true) {
                                href.innerText = '<%= ULSError.Core.Resource.Read("DoneNotification") %>';
                            } else {
                                href.innerText = '<%= ULSError.Core.Resource.Read("FailedNotification") %>';
                            }
                        }
                        else {
                            console.log(req.statusText)
                            href.innerText = '<%= ULSError.Core.Resource.Read("FailedNotification") %>';
                        }
                    };
                req.open('GET', '<%= SPContext.Current.Web.ServerRelativeUrl %>/_layouts/15/ULSError/Notify.aspx?correlationId=' + corId);
                req.setRequestHeader('SPResponseGuid', document.getElementById('<%= ulsnotifycorrelationid.ClientID %>').value);
                if(req.onload === null)
                    req.onload = _onload;
                else req.onreadystatechange = _onload;
                    
                req.onerror = function(e) {
                    console.log(e);
                    href.innerText = '<%= ULSError.Core.Resource.Read("FailedNotification") %>';
                };
                req.send();
            }
        }
    </SharePoint:ScriptBlock>
</div>