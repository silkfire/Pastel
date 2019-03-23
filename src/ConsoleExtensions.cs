namespace Pastel
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Globalization;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Text.RegularExpressions;

    public static class ConsoleExtensions
    {
        private const int STD_OUTPUT_HANDLE = -11;
        private const uint ENABLE_VIRTUAL_TERMINAL_PROCESSING = 0x0004;
        private const uint DISABLE_NEWLINE_AUTO_RETURN = 0x0008;

        [DllImport("kernel32.dll")]
        private static extern bool GetConsoleMode(IntPtr hConsoleHandle, out uint lpMode);

        [DllImport("kernel32.dll")]
        private static extern bool SetConsoleMode(IntPtr hConsoleHandle, uint dwMode);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr GetStdHandle(int nStdHandle);

        [DllImport("kernel32.dll")]
        public static extern uint GetLastError();

        private delegate string ColorFormatOriginal(string input, Color color);
        private delegate string ColorFormatHexOriginal(string input, string hexColor);
        private delegate string ColorFormat(FormattableString input, Color color);
        private delegate string ColorFormatHex(FormattableString input, string hexColor);


        private const string _formatString = "\u001b[{0};2;{1};{2};{3}m{4}\u001b[0m";
        private const string _foregroundModifier = "38";
        private const string _backgroundModifier = "48";


        private static readonly Func<string, int> _parseHexColor = hc => int.Parse(hc.Replace("#", ""), NumberStyles.HexNumber);


        private static readonly Func<string, Color, string, string> _colorFormatOriginal = (s, c, f) =>
        {
            return string.Format(_formatString, f, c.R, c.G, c.B, s);
        };
        private static readonly Func<string, string, string, string> _colorHexFormatOriginal = (s, c, f) => _colorFormatOriginal(s, Color.FromArgb(_parseHexColor(c)), f);

        private static readonly Func<FormattableString, Color, string, string> _colorFormat = (s, c, f) =>
        {
            var ss = Split(s.Format).Select(x => x.shouldFormat ? _colorFormatOriginal(x.s, c, f) : x.s).ToList();
            return string.Format(string.Join("", ss), s.GetArguments());
        };
        private static readonly Func<FormattableString, string, string, string> _colorHexFormat = (s, c, f) => _colorFormat(s, Color.FromArgb(_parseHexColor(c)), f);

        private static readonly ColorFormat _noColorOutputFormat = (s, _) => s.ToString();
        private static readonly ColorFormatHex _noColorOutputHexFormat = (s, _) => s.ToString();

        private static readonly ColorFormat _foregroundColorFormat = (s, c) => _colorFormat(s, c, _foregroundModifier);
        private static readonly ColorFormatHex _foregroundColorHexFormat = (s, c) => _colorHexFormat(s, c, _foregroundModifier);

        private static readonly ColorFormat _backgroundColorFormat = (s, c) => _colorFormat(s, c, _backgroundModifier);
        private static readonly ColorFormatHex _backgroundColorHexFormat = (s, c) => _colorHexFormat(s, c, _backgroundModifier);


        private static readonly ColorFormatOriginal _noColorOutputFormatOriginal = (s, _) => s;
        private static readonly ColorFormatHexOriginal _noColorOutputHexFormatOriginal = (s, _) => s;

        private static readonly ColorFormatOriginal _foregroundColorFormatOriginal = (s, c) => _colorFormatOriginal(s, c, _foregroundModifier);
        private static readonly ColorFormatHexOriginal _foregroundColorHexFormatOriginal = (s, c) => _colorHexFormatOriginal(s, c, _foregroundModifier);

        private static readonly ColorFormatOriginal _backgroundColorFormatOriginal = (s, c) => _colorFormatOriginal(s, c, _backgroundModifier);
        private static readonly ColorFormatHexOriginal _backgroundColorHexFormatOriginal = (s, c) => _colorHexFormatOriginal(s, c, _backgroundModifier);

        private static ColorFormatOriginal _foregroundColorFormatFuncOriginal;
        private static ColorFormatHexOriginal _foregroundColorHexFormatFuncOriginal;

        private static ColorFormatOriginal _backgroundColorFormatFuncOriginal;
        private static ColorFormatHexOriginal _backgroundColorHexFormatFuncOriginal;

        private static ColorFormat _foregroundColorFormatFunc;
        private static ColorFormatHex _foregroundColorHexFormatFunc;

        private static ColorFormat _backgroundColorFormatFunc;
        private static ColorFormatHex _backgroundColorHexFormatFunc;


        static ConsoleExtensions()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                var iStdOut = GetStdHandle(STD_OUTPUT_HANDLE);

                var enable = GetConsoleMode(iStdOut, out var outConsoleMode)
                             && SetConsoleMode(iStdOut, outConsoleMode | ENABLE_VIRTUAL_TERMINAL_PROCESSING | DISABLE_NEWLINE_AUTO_RETURN);
            }


            if (Environment.GetEnvironmentVariable("NO_COLOR") == null)
            {
                Enable();
            }
            else
            {
                Disable();
            }
        }







        /// <summary>
        /// Enables any future console color output produced by Pastel.
        /// </summary>
        public static void Enable()
        {
            _foregroundColorFormatFunc = _foregroundColorFormat;
            _foregroundColorHexFormatFunc = _foregroundColorHexFormat;

            _backgroundColorFormatFunc = _backgroundColorFormat;
            _backgroundColorHexFormatFunc = _backgroundColorHexFormat;

            _foregroundColorFormatFuncOriginal = _foregroundColorFormatOriginal;
            _foregroundColorHexFormatFuncOriginal = _foregroundColorHexFormatOriginal;

            _backgroundColorFormatFuncOriginal = _backgroundColorFormatOriginal;
            _backgroundColorHexFormatFuncOriginal = _backgroundColorHexFormatOriginal;
        }

        /// <summary>
        /// Disables any future console color output produced by Pastel.
        /// </summary>
        public static void Disable()
        {
            _foregroundColorFormatFunc = _noColorOutputFormat;
            _foregroundColorHexFormatFunc = _noColorOutputHexFormat;

            _backgroundColorFormatFunc = _noColorOutputFormat;
            _backgroundColorHexFormatFunc = _noColorOutputHexFormat;

            _foregroundColorFormatFuncOriginal = _noColorOutputFormatOriginal;
            _foregroundColorHexFormatFuncOriginal = _noColorOutputHexFormatOriginal;

            _backgroundColorFormatFuncOriginal = _noColorOutputFormatOriginal;
            _backgroundColorHexFormatFuncOriginal = _noColorOutputHexFormatOriginal;
        }


        /// <summary>
        /// Returns a string wrapped in an ANSI foreground color code using the specified color.
        /// </summary>
        /// <param name="input">The string to color.</param>
        /// <param name="color">The color to use on the specified string.</param>
        public static string Pastel(this Color color, FormattableString input)
        {
            return _foregroundColorFormatFunc(input, color);
        }

        public static string Pastel(this string input, Color color)
        {
            return _foregroundColorFormatFuncOriginal(input, color);
        }

        /// <summary>
        /// Returns a string wrapped in an ANSI foreground color code using the specified color.
        /// </summary>
        /// <param name="input">The string to color.</param>
        /// <param name="hexColor">The color to use on the specified string.<para>Supported format: [#]RRGGBB.</para></param>
        public static string Pastel(this string hexColor, FormattableString input)
        {
            return _foregroundColorHexFormatFunc(input, hexColor);
        }

        public static string Pastel(this string input, string hexColor)
        {
            return _foregroundColorHexFormatFuncOriginal(input, hexColor);
        }



        /// <summary>
        /// Returns a string wrapped in an ANSI background color code using the specified color.
        /// </summary>
        /// <param name="input">The string to color.</param>
        /// <param name="color">The color to use on the specified string.</param>
        public static string PastelBg(this Color color, FormattableString input)
        {
            return _backgroundColorFormatFunc(input, color);
        }

        public static string PastelBg(this string input, Color color)
        {
            return _backgroundColorFormatFuncOriginal(input, color);
        }

        /// <summary>
        /// Returns a string wrapped in an ANSI background color code using the specified color.
        /// </summary>
        /// <param name="input">The string to color.</param>
        /// <param name="hexColor">The color to use on the specified string.<para>Supported format: [#]RRGGBB.</para></param>
        public static string PastelBg(this string hexColor, FormattableString input)
        {
            return _backgroundColorHexFormatFunc(input, hexColor);
        }

        public static string PastelBg(this string input, string hexColor)
        {
            return _backgroundColorHexFormatFuncOriginal(input, hexColor);
        }

        private static List<(string s, bool shouldFormat)> Split(string input)
        {

            var values = new List<(string, bool)>();
            int pos = 0;
            foreach (Match m in Regex.Matches(input, @"\{\d*\}"))
            {
                var v = input.Substring(pos, m.Index - pos);
                if (!string.IsNullOrEmpty(v))
                {
                    values.Add((input.Substring(pos, m.Index - pos), true));
                }
                values.Add((m.Value, false));
                pos = m.Index + m.Length;
            }
            var vEnd = input.Substring(pos);
            if (!string.IsNullOrEmpty(vEnd))
            {
                values.Add((vEnd, true));
            }
            return values;
        }
    }
}
