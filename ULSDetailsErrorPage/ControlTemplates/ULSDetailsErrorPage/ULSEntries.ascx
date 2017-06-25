<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %> 
<%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ULSEntries.ascx.cs" Inherits="ULSError.ControlTemplates.ULSEntries" %>

<style>
    #ULSEntriesTable {
        border-collapse: collapse;
        width:100%;
    }

    #ULSEntriesTable > thead {
        border-bottom: 1px solid #444;
    }

    #ULSEntries > tr:hover {
        background:#f2f2f2;
    }

    #ULSEntries > tr > td {
        vertical-align: top;
    }
</style>

<input type="hidden" runat="server" id="ulsentriescorrelationid" value="" />
<table id="ULSEntriesTable">
    <thead>
        <tr>
            <th style="text-align:left; width:10%;"><asp:Literal runat="server" Text="<%$ Resources:ulserrorpage, ULSLevel%>" /></th>
            <th style="text-align:left"><asp:Literal runat="server" Text="<%$ Resources:ulserrorpage, ULSMessage%>" /></th>
        </tr>
    </thead>
    <tbody id="ULSEntries" class="ms-metadata">

    </tbody>
</table>
<div id="ULSEntriesMore"></div>

<SharePoint:ScriptBlock runat="server" ID="_ulsentriessb">
    function _loadULSEntries() {        
        var corId = _getCorrelationId();        
        var req = new XMLHttpRequest();
        var _onload = function() {
            if(req.readyState != 4)
                return false;

            if(req.status === 200 && (req.response || req.responseText)) {
                var reqresp = req.response || req.responseText || '{}';
                var resp = JSON.parse(reqresp);
                var tbody = document.getElementById('ULSEntries');
                if(tbody) {
                    for(var x=0; x < resp.Entries.length; x++) {
                        var entry = resp.Entries[x];
                        var row = tbody.insertRow(-1);
                    
                        var levelCell = row.insertCell(0);
                        var msgCell = row.insertCell(1);

                        levelCell.innerHTML = entry.Severity;
                        msgCell.innerHTML = entry.Message;
                    }

                    if (resp.More === true) {
                        var more = document.getElementById('ULSEntriesMore');
                        if(more) more.innerText = '...';
                    }
                }
            }
            else console.log(req.statusText)
        };
        req.open('GET', '<%= SPContext.Current.Web.ServerRelativeUrl %>/_layouts/15/ULSError/ULSDetails.aspx?correlationId=' + corId);
        req.setRequestHeader('SPResponseGuid', document.getElementById('<%= ulsentriescorrelationid.ClientID %>').value);
        if(req.onload === null)
            req.onload = _onload
        else req.onreadystatechange = _onload;
        req.onerror = function(e) {
            console.log(e);
        };
        req.send();
    }
    _spBodyOnLoadFunctionNames.push("_loadULSEntries");
</SharePoint:ScriptBlock>