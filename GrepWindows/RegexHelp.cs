using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace GrepWindows
{
    public partial class RegexHelp : Form
    {
        public RegexHelp()
        {
            InitializeComponent();
            webBrowser1.Navigate("about:blank");
            HtmlDocument doc = webBrowser1.Document;
            String cheatSheat = @"<br/><h1>C# Regular Expressions Cheat Sheet</h1><br/>
<table>

  <tbody>
    <tr valign='top'>
      <th width='14%'><div align='left'>Character</div></th>

      <th width='86%'>Description</th>
    </tr>
    <tr valign='top'>
      <td width='14%'><strong>\ </strong></td>

      <td width='86%'><p>Marks the next character as either a special character
        or escapes a literal. For example, 'n' matches the character 'n'. '\n' matches
        a newline character. The sequence '\\' matches '\' and '\(' matches '('.</p>
        <p>Note: double quotes may be escaped by doubling them: '&lt;a href=''...&gt;' </p></td>
    </tr>
    <tr valign='top'>
      <td width='14%'><strong>^ </strong></td>
      <td width='86%'>Depending on whether the MultiLine option is set, matches the position before the first character in a line, or the first character in the string.</td>
    </tr>
    <tr valign='top'>
      <td width='14%'><strong>$ </strong></td>

      <td width='86%'>Depending on whether the MultiLine option is set, matches the position after the last character in a line, or the last character in the string.</td>
    </tr>
    <tr valign='top'>
      <td width='14%'><strong>* </strong></td>

      <td width='86%'>Matches the preceding character zero or more times. For
        example,   'zo*' matches either 'z' or 'zoo'.</td>
    </tr>
    <tr valign='top'>
      <td width='14%'><strong>+ </strong></td>
      <td width='86%'>Matches the preceding character one or more times. For
        example,   'zo+' matches 'zoo' but not 'z'.</td>
    </tr>
    <tr valign='top'>

      <td width='14%'><strong>? </strong></td>
      <td width='86%'>Matches the preceding character zero or one time. For
        example,   'a?ve?' matches the 've' in 'never'. </td>
    </tr>
    <tr valign='top'>
      <td width='14%'><strong>.</strong></td>
      <td width='86%'>Matches any single character except a newline character. </td>
    </tr>

    <tr valign='top'>
      <td width='14%'><strong>(pattern) </strong></td>

      <td width='86%'>Matches <em>pattern</em> and remembers the match. The
        matched substring can be retrieved from the resulting <strong>Matches</strong> collection,
        using Item <strong>[0]...[n]</strong>. To match parentheses characters
        ( ), use '\(' or   '\)'.</td>
    </tr>
	<tr valign='top'>
      <td width='14%'><strong>(?&lt;name&gt;pattern) </strong></td>

      <td width='86%'>Matches <em>pattern</em> and gives the match a name. </td>
    </tr>

	<tr valign='top'>
      <td width='14%'><strong>(?:pattern) </strong></td>

      <td width='86%'>A non-capturing group </td>
    </tr>
		<tr valign='top'>
      <td width='14%'><strong>(?=...) </strong></td>

      <td width='86%'>A positive lookahead  </td>
    </tr>
		<tr valign='top'>
      <td width='14%'><strong>(?!...) </strong></td>

      <td width='86%'>A negative lookahead </td>
    </tr>
		<tr valign='top'>

      <td width='14%'><strong>(?&lt;=...) </strong></td>

      <td width='86%'>A positive lookbehind . </td>
    </tr>
		<tr valign='top'>
      <td width='14%'><strong>(?&lt;!...) </strong></td>

      <td width='86%'>A negative lookbehind . </td>
    </tr>	
    <tr valign='top'>
      <td width='14%'><strong>x|y</strong></td>
      <td width='86%'>Matches either <em>x</em> or <em>y</em>. For example, 'z|wood'   matches 'z' or 'wood'. '(z|w)oo' matches 'zoo' or 'wood'. </td>
    </tr>
    <tr valign='top'>
      <td width='14%'><strong>{<em>n</em>}</strong></td>
      <td width='86%'><em>n</em> is a non-negative integer. Matches exactly <em>n</em> times.
        For example, 'o{2}' does not match the 'o' in 'Bob,' but matches the
        first two o's in 'foooood'.</td>
    </tr>
    <tr valign='top'>
      <td width='14%'><strong>{<em>n</em>,} </strong></td>
      <td width='86%'><em>n</em> is a non-negative integer. Matches at least <em>n</em> times.
        For example, 'o{2,}' does not match the 'o' in 'Bob' and matches all
        the o's in 'foooood.' 'o{1,}' is equivalent to 'o+'. 'o{0,}' is equivalent
        to   'o*'.</td>
    </tr>
    <tr valign='top'>
      <td width='14%'><strong>{</strong><em>n</em><strong>,</strong><em>m</em><strong>}</strong> </td>
      <td width='86%'><em>m</em> and <em>n</em> are non-negative integers. Matches
        at least <em>n</em> and at most <em>m</em> times. For example, 'o{1,3}' matches
        the first three o's in 'fooooood.' 'o{0,1}' is equivalent to 'o?'.</td>
    </tr>
    <tr valign='top'>
      <td width='14%'><strong>[</strong><em>xyz</em><strong>]</strong> </td>
      <td width='86%'>A character set. Matches any one of the enclosed characters.
        For example, '[abc]' matches the 'a' in 'plain'. </td>
    </tr>
    <tr valign='top'>

      <td width='14%'><strong>[^</strong><em>xyz</em><strong>]</strong> </td>
      <td width='86%'>A negative character set. Matches any character not enclosed.
        For example, '[^abc]' matches the 'p' in 'plain'. </td>
    </tr>
    <tr valign='top'>
      <td width='14%'><strong>[</strong><em>a-z</em><strong>]</strong> </td>

      <td width='86%'>A range of characters. Matches any character in the specified
        range. For example, '[a-z]' matches any lowercase alphabetic character
        in the range 'a' through 'z'. </td>
    </tr>
    <tr valign='top'>
      <td width='14%'><strong>[^</strong><em>m-z</em><strong>]</strong> </td>
      <td width='86%'>A negative range characters. Matches any character not
        in the specified range. For example, '[m-z]' matches any character not
        in the range 'm'   through 'z'. </td>
    </tr>

    <tr valign='top'>
      <td width='14%'><strong>\b </strong></td>
      <td width='86%'>Matches a word boundary, that is, the position between
        a word and a space. For example, 'er\b' matches the 'er' in 'never' but
        not the 'er' in   'verb'. </td>
    </tr>
    <tr valign='top'>
      <td width='14%'><strong>\B </strong></td>
      <td width='86%'>Matches a non-word boundary. 'ea*r\B' matches the 'ear' in   'never
        early'. </td>
    </tr>
    <tr valign='top'>
      <td width='14%'><strong>\d </strong></td>
      <td width='86%'>Matches a digit character. Equivalent to [0-9]. </td>
    </tr>
    <tr valign='top'>
      <td width='14%'><strong>\D </strong></td>

      <td width='86%'>Matches a non-digit character. Equivalent to [^0-9]. </td>
    </tr>
    <tr valign='top'>
      <td width='14%'><strong>\f </strong></td>
      <td width='86%'>Matches a form-feed character. </td>
    </tr>
    <tr valign='top'>
      <td><strong>\k </strong></td>
      <td>A back-reference to a named group. </td>
    </tr>    
    <tr valign='top'>
      <td><strong>\n </strong></td>
      <td>Matches a newline character. </td>
    </tr>
    <tr valign='top'>
      <td width='14%'><strong>\r </strong></td>
      <td width='86%'>Matches a carriage return character. </td>
    </tr>

    <tr valign='top'>
      <td width='14%'><strong>\s </strong></td>
      <td width='86%'>Matches any white space including space, tab, form-feed,
        etc. Equivalent to '[&nbsp;\f\n\r\t\v]'.</td>
    </tr>
    <tr valign='top'>
      <td width='14%'><strong>\S </strong></td>

      <td width='86%'>Matches any nonwhite space character. Equivalent to   '[^&nbsp;\f\n\r\t\v]'. </td>
    </tr>
    <tr valign='top'>
      <td width='14%'><strong>\t </strong></td>
      <td width='86%'>Matches a tab character. </td>
    </tr>

    <tr valign='top'>
      <td width='14%'><strong>\v </strong></td>
      <td width='86%'>Matches a vertical tab character. </td>
    </tr>
    <tr valign='top'>
      <td width='14%'><strong>\w </strong></td>
      <td width='86%'>Matches any word character including underscore. Equivalent
        to   '[A-Za-z0-9_]'. </td>
    </tr>
    <tr valign='top'>
      <td width='14%'><strong>\W </strong></td>
      <td width='86%'>Matches any non-word character. Equivalent to '[^A-Za-z0-9_]'. </td>
    </tr>
    <tr valign='top'>
      <td width='14%'><strong>\num </strong></td>

      <td width='86%'>Matches <em>num</em>, where <em>num</em> is a positive
        integer. A reference back to remembered matches. For example, '(.)\1' matches
        two consecutive identical characters. </td>
    </tr>
    <tr valign='top'>
      <td width='14%'><strong>\<em>n</em></strong></td>

      <td width='86%'>Matches <em>n</em>, where <em>n</em> is an octal escape
        value. Octal escape values must be 1, 2, or 3 digits long. For example, '\11' and '\011' both
        match a tab character. '\0011' is the equivalent of '\001' &amp; '1'.
        Octal escape values must not exceed 256. If they do, only the first
        two digits comprise the expression. Allows ASCII codes to be used in
        regular expressions.</td>
    </tr>
    <tr valign='top'>
      <td width='14%'><strong>\x<em>n</em></strong></td>

      <td width='86%'>Matches <em>n</em>, where <em>n</em> is a hexadecimal
        escape value. Hexadecimal escape values must be exactly two digits long.
        For example, '\x41'   matches 'A'. '\x041' is equivalent to '\x04' &amp; '1'.
        Allows ASCII codes to be used in regular expressions.</td>
    </tr>
    <tr valign='top'>
      <td width='14%'><strong>\u<em>n</em></strong></td>

      <td width='86%'>Matches a Unicode character expressed in hexadecimal notation with exactly four numeric digits. '\u0200' matches a space character. </td>
    </tr>	
    <tr valign='top'>
      <td><strong>\A</strong></td>
      <td>Matches the position before the first character in a string. Not affected by the MultiLine setting </td>
    </tr>
    <tr valign='top'>
      <td><strong>\Z</strong></td>

      <td>Matches the position after the last character of a string. Not affected by the MultiLine setting. </td>
    </tr>
    <tr valign='top'>
      <td><strong>\G</strong></td>
      <td>Specifies that the matches must be consecutive, without any intervening non-matching characters. </td>
    </tr>
  </tbody>
</table>";
            doc.Write("<html><head><style>table { border: 1px solid #BBBBBB; border-collapse: collapse; } th { background-color: #E5E5E5; border: 1px solid #BBBBBB; } td { border: 1px solid #BBBBBB; }</style></head><body>Regular expression searches follow C# regular expression rules.  Some changes to the regex rules can be made by specifying inline options to the regular expression search pattern.  <br/> <br/>To use the inline character start your expression with (?msx).  For example if you want to use the singleline option you would start with <b>(?s)</b> the multiline option would be <b>(?m)</b>.<br/><br><table><tbody><tr><th>Name</th><th>Inline character</th><th>Effect</th></tr><tr><td>Multiline</td><td>m</td><td>Use multiline mode, where ^ and $ match the beginning and end of each line (instead of the beginning and end of the input string).</td></tr><tr><td>Singleline</td><td>s</td><td>Use single-line mode, where the period (.) matches every character (instead of every character except \\n).</td></tr><tr><td>Ignore Pattern Whitespace</td><td>x</td><td>Exclude unescaped white space from the pattern, and enable comments after a number sign(#).</td></tr><tr><td>Explicit Capture</td><td>n</td><td>Do not capture unnamed groups. The only valid captures are explicitly named or numbered groups of the form (?<name> subexpression).</td></tbody></table>" + cheatSheat + "</body></html>");
        }
    }
}
