<?xml version="1.0" encoding="iso-8859-1"?>
<!DOCTYPE xsl:stylesheet  [
  <!ENTITY nbsp   "&#160;">
  <!ENTITY copy   "&#169;">
  <!ENTITY reg    "&#174;">
  <!ENTITY trade  "&#8482;">
  <!ENTITY mdash  "&#8212;">
  <!ENTITY ldquo  "&#8220;">
  <!ENTITY rdquo  "&#8221;">
  <!ENTITY pound  "&#163;">
  <!ENTITY yen    "&#165;">
  <!ENTITY euro   "&#8364;">
]>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0">
  
  <xsl:output method="html"></xsl:output>
  <xsl:template match="/">
    <html>
      <head>
        <script type="text/javascript">
          function ifill(iframe,page) {
          eval("parent."+iframe+".location='"+page+"'");
          }
        </script>
        <title>XML Tree Control</title>
        <link rel="stylesheet" type="text/css" href="xmlTree.css"/>
        <script type="text/javascript" src="xmlTree.js"></script>
      </head>
        <body bgcolor="#EEEEEE">
          <table class="mainTable">
            <tr>
              <td nowrap="nowrap" width="100%">
                <xsl:apply-templates/>
              </td>
            </tr>
          </table>
        </body>
    </html>
  </xsl:template>


  <xsl:template match="assembly">
    <center>
        <img class="cAssemblyName">
          <xsl:attribute name="src">
            <xsl:value-of select="@name"/>.jpg
          </xsl:attribute>
          <xsl:attribute name="alt">
            <xsl:value-of select="@name"/>
          </xsl:attribute>
        </img>
      <div>
        <span class="cAssemblyVersionTitle">Version: </span>
        <span class="cAssemblyVersionText">
          <xsl:value-of select="@version"/>
        </span>
      </div>
    </center>
    <br/>
    <xsl:apply-templates/>
  </xsl:template>
  <xsl:template match="branch">
    <table class="treeTable">
      <tr>
          <td>
            <!--  Caution must be taken here to make sure the "id" attribute does NOT contain whitespaces.
                VS.NET 2005 will occasionally re-format this and at add line breaks/indents before <xsl:value-of select="@id"/> -->
            <xsl:attribute name="id">I<xsl:value-of select="@id"/>
            </xsl:attribute>
            <xsl:attribute name="onclick">
              showBranch('<xsl:value-of select="@id"/>');
            </xsl:attribute>
            <xsl:attribute name="onmouseover">onMouseOverImg('<xsl:value-of select="@id"/>');
            </xsl:attribute>
            <xsl:attribute name="onmouseout">onMouseOutImg('<xsl:value-of select="@id"/>');
            </xsl:attribute>
            <img class="trigger">
              <xsl:if test="@children = '1'">
                <xsl:attribute name="src">treeNode_Plus.gif</xsl:attribute>
              </xsl:if>
              <xsl:if test="@children = '0'">
                <xsl:attribute name="src">treeNode_Empty.gif</xsl:attribute>
              </xsl:if>
            </img>
          </td>
          <td width="100%">
              <xsl:if test="@children = '1'">
                <xsl:attribute name="class">triggerBranchText</xsl:attribute>
              </xsl:if>
              <xsl:if test="@children = '0'">
                <xsl:attribute name="class">triggerText</xsl:attribute>
              </xsl:if>
              <xsl:attribute name="onmouseover">
                onMouseOverText('<xsl:value-of select="@id"/>');
              </xsl:attribute>
              <xsl:attribute name="onmouseout">
                onMouseOutText('<xsl:value-of select="@id"/>');
              </xsl:attribute>
              <xsl:attribute name="onclick">
                showLink('<xsl:value-of select="@id"/>');
              </xsl:attribute>

              <!--  Caution must be taken here to make sure the "id" attribute does NOT contain whitespaces.
                VS.NET 2005 will occasionally re-format this and at add line breaks/indents before <xsl:value-of select="@id"/> -->
              <xsl:attribute name="id">T<xsl:value-of select="@id"/>
              </xsl:attribute>
              <xsl:if test="branchText/@ObjectType != ''">
                <span class="cObjectType">
                  <xsl:value-of select="branchText/@ObjectType"/>
                </span>&nbsp;
              </xsl:if>
              <xsl:copy-of select="branchText"/>
          </td>
        </tr>
      </table>
        <span class="branch">
          <xsl:attribute name="id"><xsl:value-of select="@id"/></xsl:attribute>
          <xsl:apply-templates/>
        </span>
  </xsl:template>

  <!-- avoid output of text node with default template -->
  <xsl:template match="branchText"/>

</xsl:stylesheet>
