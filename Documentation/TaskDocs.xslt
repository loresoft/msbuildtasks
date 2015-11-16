<?xml version="1.0" encoding="utf-8"?>

<xsl:stylesheet version="1.0"
    xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:xs="http://www.w3.org/2001/XMLSchema"
    xmlns:msxsl="urn:schemas-microsoft-com:xslt"
    xmlns:user="http://mycompany.com/mynamespace">

  <msxsl:script language="JScript" implements-prefix="user">
    <![CDATA[
  function replace(str_text,str_replace,str_by)
  {
       return str_text.replace(str_replace,str_by);
  }
  function trim(str_text)
  {
       return str_text.replace(/^\s*/g,'').replace(/\s*$/g, '');
  }
  function prepExample(str_text)
  {
      while (/ {8}\</.test(str_text)) {
       str_text = str_text.replace(/ {8}\</g,'      \<');
      }
      str_text = str_text
        .replace(/^\s*(\w)/,'$1')
        .replace(/\>\n(\s*)(\w)/g, '\>\n$2');
      return str_text;
  }
  function lastWordInName(str_text) {
      if (str_text.indexOf('.') > -1) return str_text.substring(str_text.lastIndexOf('.') + 1);
      return str_text;
  }
]]>
  </msxsl:script>

  <xsl:output method="text"/>
  
  <xsl:template match="/doc/members/member"><xsl:if test="self::node()[starts-with(@name,'T:')]">
## &lt;a id=&quot;<xsl:value-of select="user:lastWordInName(string(@name))"/>&quot;&gt;<xsl:value-of select="user:lastWordInName(string(@name))"/>&lt;/a&gt;<xsl:if test="not(user:lastWordInName(string(@name)) = user:replace(string(@name), 'T:MSBuild.Community.Tasks.', ''))"> (&lt;a id=&quot;<xsl:value-of select="user:replace(string(@name), 'T:MSBuild.Community.Tasks.', '')"/>&quot;&gt;<xsl:value-of select="user:replace(string(@name), 'T:MSBuild.Community.Tasks.', '')"/>&lt;/a&gt;)</xsl:if>
### Description
<xsl:value-of select="user:trim(string(summary))"/><xsl:if test="example">
### Example
<xsl:value-of select="user:prepExample(user:replace(user:replace(string(example), '&lt;code>&lt;![CDATA[', ''), ']]>&lt;/code>', ''))"/>
</xsl:if><xsl:if test="not(example)">
### No example given
The developer of this task did not add an example in the summary documentation.
</xsl:if>
* * *
</xsl:if></xsl:template>

</xsl:stylesheet>
