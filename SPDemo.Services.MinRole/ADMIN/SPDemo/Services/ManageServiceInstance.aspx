<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.SharePoint.ApplicationPages.Administration, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c"%>
<%@ Register Tagprefix="wssawc" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="wssuc" TagName="InputFormSection" src="~/_controltemplates/InputFormSection.ascx" %>
<%@ Register TagPrefix="wssuc" TagName="InputFormControl" src="~/_controltemplates/InputFormControl.ascx" %>
<%@ Register TagPrefix="wssuc" TagName="IisWebServiceApplicationPoolSection" src="~/_admin/IisWebServiceApplicationPoolSection.ascx" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Import Namespace="Microsoft.SharePoint.ApplicationPages" %>
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ManageServiceInstance.aspx.cs" Inherits="SPDemo.Services.MinRole.Admin.ManageServiceInstance" DynamicMasterPageFile="~/_layouts/15/dialog.master" %>

<asp:Content ContentPlaceHolderId="PlaceHolderAdditionalPageHead" runat="server">
</asp:Content>
   
<asp:Content ContentPlaceHolderId="PlaceHolderDialogHeaderPageTitle" runat="server">
    <asp:Literal ID='PageTitle' Text="Demo Custom Service" runat="server" />
</asp:Content>
 
<asp:Content contentplaceholderid="PlaceHolderDialogDescription" runat="server">
    <asp:Literal ID='PageDescription' Text="Edit the settings for this service application." runat="server" />
</asp:Content>
 
<asp:content contentplaceholderid="PlaceHolderDialogBodyMainSection" runat="server">
<wssawc:FormDigest ID="FormDigest" runat="server" />
    <table border="0" cellspacing="0" cellpadding="0" width="100%" class="ms-propertysheet">
        <wssuc:InputFormSection
            Title="Name"
            runat="server">
            <Template_InputFormControls>
                <wssuc:InputFormControl LabelText="Service Application Name" runat="server" >
                    <Template_Control>
                        <wssawc:InputFormTextBox
                            id="ServiceApplicationNameTextBox"
                            title="Name"
                            maxlength="256"
                            columns="35"
                            class="ms-input"
                            runat="server" />
                        <wssawc:InputFormRequiredFieldValidator
                            ID="ServiceApplicationNameValidator"
                            ControlToValidate="ServiceApplicationNameTextBox"
                            ErrorMessage="You must specify a value for this required field."
                            runat="server" />
                    </Template_Control>
                </wssuc:InputFormControl>
            </Template_InputFormControls>
        </wssuc:InputFormSection>
 
        <wssuc:IisWebServiceApplicationPoolSection
            id="ServiceApplicationPoolSection"
            runat="server" />
    </table>
</asp:content>

