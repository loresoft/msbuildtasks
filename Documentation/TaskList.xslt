<?xml version="1.0" encoding="utf-8"?>

<xsl:stylesheet version="1.0"
    xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:xs="http://www.w3.org/2001/XMLSchema">

  <xsl:output method="html"/>
  
  <xsl:template match="/">
    <table border="0" cellpadding="3" cellspacing="0" width="90%" id="tasksTable">
      <tr>
        <th align="left" width="190">
          Task
        </th>
        <th align="left">
          Description
        </th>
      </tr>
      <xsl:for-each select="/xs:schema/xs:element[@substitutionGroup='msb:Task']">
        <xsl:sort data-type="text" select="@name"/>
        <tr>
          <td>
            <xsl:value-of select="@name"/>
          </td>
          <td>
            <xsl:value-of select="xs:annotation/xs:documentation"/>
          </td>
        </tr>
      </xsl:for-each>
    </table>
  </xsl:template>

</xsl:stylesheet>
