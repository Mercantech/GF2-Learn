using System.Net;
using System.Text.RegularExpressions;

var code = "bool isAdult = age >= 18;\nif (isAdult) { Console.WriteLine(\"Du må starte.\"); }";
var html = $"<pre><code class=\"language-csharp\">{WebUtility.HtmlEncode(code)}</code></pre>";

// inline minimal test - call actual service via reflection is hard; paste regex logic
Console.WriteLine(html);
