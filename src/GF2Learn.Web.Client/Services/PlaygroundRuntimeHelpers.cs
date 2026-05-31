namespace GF2Learn.Web.Client.Services;

/// <summary>C# source injected around user playground code (runs in dynamically loaded assembly).</summary>
internal static class PlaygroundRuntimeHelpers
{
    public const string Source = """
        private struct __Date
        {
            public int Year;
            public int Month;
            public int Day;
            public string ToShortDateString() =>
                __TwoDigits(Day) + "-" + __TwoDigits(Month) + "-" + __IntStr(Year);
        }

        private static string __TwoDigits(int n) =>
            new string(new[] { (char)('0' + (n / 10)), (char)('0' + (n % 10)) });

        private static string __IntStr(int n)
        {
            if (n == 0) return "0";
            if (n < 0) return "-" + __IntStr(-n);
            var buf = new char[12];
            var len = 0;
            while (n > 0)
            {
                buf[len++] = (char)('0' + (n % 10));
                n /= 10;
            }
            Array.Reverse(buf, 0, len);
            return new string(buf, 0, len);
        }

        private static bool __TryParseInt(string? s, out int value)
        {
            value = 0;
            if (string.IsNullOrWhiteSpace(s)) return false;
            s = s.Trim();
            var start = 0;
            var sign = 1;
            if (s[0] == '-')
            {
                sign = -1;
                start = 1;
                if (s.Length == 1) return false;
            }
            else if (s[0] == '+')
            {
                start = 1;
                if (s.Length == 1) return false;
            }

            long n = 0;
            for (var i = start; i < s.Length; i++)
            {
                var c = s[i];
                if (c < '0' || c > '9') return false;
                n = n * 10 + (c - '0');
                if (n > int.MaxValue) return false;
            }

            value = (int)(n * sign);
            return true;
        }

        private static int __ParseInt(string? s)
        {
            if (__TryParseInt(s, out var n)) return n;
            throw new InvalidOperationException("Input string was not in a correct format.");
        }

        private static bool __TryParseDouble(string? s, out double value)
        {
            value = 0;
            if (string.IsNullOrWhiteSpace(s)) return false;
            s = s.Trim().Replace(',', '.');
            var start = 0;
            var sign = 1.0;
            if (s[0] == '-')
            {
                sign = -1;
                start = 1;
                if (s.Length == 1) return false;
            }
            else if (s[0] == '+')
            {
                start = 1;
                if (s.Length == 1) return false;
            }

            long whole = 0;
            var i = start;
            for (; i < s.Length; i++)
            {
                var c = s[i];
                if (c == '.') break;
                if (c < '0' || c > '9') return false;
                whole = whole * 10 + (c - '0');
            }

            var fraction = 0.0;
            var denom = 1.0;
            if (i < s.Length)
            {
                if (s[i] != '.') return false;
                i++;
                for (; i < s.Length; i++)
                {
                    var c = s[i];
                    if (c < '0' || c > '9') return false;
                    fraction = fraction * 10 + (c - '0');
                    denom *= 10;
                }
            }
            else if (i == start && i >= s.Length)
            {
                return false;
            }

            value = sign * (whole + fraction / denom);
            return true;
        }

        private static double __ParseDouble(string? s)
        {
            if (__TryParseDouble(s, out var n)) return n;
            throw new InvalidOperationException("Input string was not in a correct format.");
        }

        private static bool __TryParseBool(string? s, out bool value)
        {
            value = false;
            if (string.IsNullOrWhiteSpace(s)) return false;
            s = s.Trim();
            if (__EqIgnoreCase(s, "true"))
            {
                value = true;
                return true;
            }
            if (__EqIgnoreCase(s, "false"))
            {
                value = false;
                return true;
            }
            return false;
        }

        private static bool __ParseBool(string? s)
        {
            if (__TryParseBool(s, out var b)) return b;
            throw new InvalidOperationException("Input string was not in a correct format.");
        }

        private static bool __TryParseDate(string? s, out __Date value)
        {
            value = default;
            if (string.IsNullOrWhiteSpace(s)) return false;
            var parts = s.Trim().Split('-', '/');
            if (parts.Length != 3) return false;
            if (!__TryParseInt(parts[0], out var a) ||
                !__TryParseInt(parts[1], out var b) ||
                !__TryParseInt(parts[2], out var c))
                return false;

            if (a > 31)
            {
                value.Year = a;
                value.Month = b;
                value.Day = c;
            }
            else
            {
                value.Day = a;
                value.Month = b;
                value.Year = c;
            }

            if (value.Year < 100) value.Year += 2000;
            return value.Month is >= 1 and <= 12 && value.Day is >= 1 and <= 31 && value.Year > 0;
        }

        private static __Date __ParseDate(string? s)
        {
            if (__TryParseDate(s, out var d)) return d;
            throw new InvalidOperationException("Input string was not in a correct format.");
        }

        private static bool __EqIgnoreCase(string a, string b)
        {
            if (a.Length != b.Length) return false;
            for (var i = 0; i < a.Length; i++)
            {
                var ca = a[i];
                var cb = b[i];
                if (ca >= 'A' && ca <= 'Z') ca = (char)(ca + 32);
                if (cb >= 'A' && cb <= 'Z') cb = (char)(cb + 32);
                if (ca != cb) return false;
            }
            return true;
        }

        private static string __Str(object? value)
        {
            if (value is null) return string.Empty;
            if (value is bool b) return b ? "true" : "false";
            if (value is int i) return __IntStr(i);
            if (value is double d) return __DoubleStr(d);
            if (value is __Date dt) return dt.ToShortDateString();
            return value.ToString() ?? string.Empty;
        }

        private static string __DoubleStr(double n)
        {
            var whole = (long)n;
            var frac = n - whole;
            if (frac < 0) frac = -frac;
            var fracText = "";
            if (frac > 0.0000001)
            {
                var f = (long)(frac * 1000000 + 0.5);
                var fs = __IntStr((int)f);
                while (fs.Length > 1 && fs[0] == '0') fs = fs[1..];
                fracText = "." + fs;
            }
            return __IntStr((int)whole) + fracText;
        }

        private static string __Format(string format, object?[] args)
        {
            if (args.Length == 0) return format;
            var sb = new StringBuilder();
            for (var i = 0; i < format.Length; i++)
            {
                if (format[i] != '{' || i + 1 >= format.Length)
                {
                    sb.Append(format[i]);
                    continue;
                }

                var close = format.IndexOf('}', i + 1);
                if (close < 0)
                {
                    sb.Append(format[i]);
                    continue;
                }

                var token = format.Substring(i + 1, close - i - 1);
                var colon = token.IndexOf(':');
                if (colon >= 0) token = token[..colon];
                var idx = 0;
                if (token.Length == 0)
                {
                    sb.Append(format[i]);
                    continue;
                }
                for (var t = 0; t < token.Length; t++)
                {
                    var ch = token[t];
                    if (ch < '0' || ch > '9')
                    {
                        idx = -1;
                        break;
                    }
                    idx = idx * 10 + (ch - '0');
                }
                if (idx < 0 || idx >= args.Length)
                {
                    sb.Append(format[i]);
                    continue;
                }

                var arg = args[idx];
                sb.Append(__Str(arg));
                i = close;
            }
            return sb.ToString();
        }
        """;
}
