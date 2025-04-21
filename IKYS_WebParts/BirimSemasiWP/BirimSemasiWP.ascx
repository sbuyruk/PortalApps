<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %> 
<%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="BirimSemasiWP.ascx.cs" Inherits="IKYS_WebParts.BirimSemasiWP.BirimSemasiWP" %>

<link rel="stylesheet" href="/Style%20Library/treant/Treant.css">




<style>
    body, div, dl, dt, dd, ul, ol, li, h1, h2, h3, h4, h5, h6, pre, form, fieldset, input, textarea, p, blockquote, th, td {
        margin: 0;
        padding: 0;
    }

    table {
        border-collapse: collapse;
        border-spacing: 0;
    }

    fieldset, img {
        border: 0;
    }

    address, caption, cite, code, dfn, em, strong, th, var {
        font-style: normal;
        font-weight: normal;
    }

    caption, th {
        text-align: left;
    }

    h1, h2, h3, h4, h5, h6 {
        font-size: 100%;
        font-weight: normal;
    }

    q:before, q:after {
        content: '';
    }

    abbr, acronym {
        border: 0;
    }

    body {
        background: #fff;
    }
    /* optional Container STYLES */
    .chart {
        min-height: 600px;
        margin: 5px;
        min-width: 1000px;
    }

    .Treant > .node {
    }

    .Treant > p {
        font-family: "HelveticaNeue-Light", "Helvetica Neue Light", "Helvetica Neue", Helvetica, Arial, "Lucida Grande", sans-serif;
        font-weight: bold;
        font-size: 12px;
    }

    .node-name {
        font-weight: bold;
    }

    .nodeExample1 {
        padding: 2px;
        -webkit-border-radius: 3px;
        -moz-border-radius: 3px;
        border-radius: 3px;
        background-color: #ffffff;
        border: 1px solid #000;
        width: 200px;
        height: 70px;
        font-family: Tahoma;
        font-size: 11px;
    }
    .nodeExample2 {
        padding: 2px;
        -webkit-border-radius: 3px;
        -moz-border-radius: 3px;
        border-radius: 3px;
        background-color: #d3d3d3;

        border: 2px dashed #808080;
        width: 200px;
        height: 70px;
        font-family: Tahoma;
        font-size: 11px;
    }
    .nodeExample1 img {
        margin-right: 10px;
        max-height:65px;
        max-height:60px;
    }

</style>



<script src="/Style%20Library/tskgv/js/raphael.js"></script>
<script src="/Style%20Library/tskgv/js/Treant.js"></script>


<div class="chart" id="BirimSemasiDiv"></div>

