<%@ Language=VBScript %>
<%Option Explicit%>
<!-- #INCLUDE FILE="../inc/format.asp" -->
<%
' =====================================================================

' Modification Log:
' Developer		Date			Comment
' =========		====			=======
'Urmi Doshi		03/2009		A0687 - HelpFIles migrated to MercPlus. redirected to a path on MercPlus server now instead to external servers.
'==============================================================================================================
dim strInPage
dim arryReferer
dim redirPage
dim strTmp
dim sServer

strInPage = ucase(Request.ServerVariables("HTTP_REFERER"))
arryReferer = split(strInPage,"/",-1,1)
strTmp = arryReferer(ubound(arryReferer))
arryReferer = split(strTmp,".",-1,1)
sServer = "merc/3_HelpScrns_DoNotChangeStructure/"
'if isAdmin or isAnyCPH or isAnyMSL or isReadOnly then
'	sServer = INTERNAL_HELP_SERVER
'else
'	sServer = EXTERNAL_HELP_SERVER
'end if
'Response.Write sServer & lcase(arryReferer(0)) & "_help.htm"
Response.Redirect sServer & lcase(arryReferer(0)) & "_help.htm"
'Response.Redirect lcase(arryReferer(0)) & "_help.htm"
%>