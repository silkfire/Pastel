#if NET7_0_OR_GREATER
[assembly: System.Runtime.CompilerServices.DisableRuntimeMarshalling]
#endif
namespace Pastel
{

    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Drawing;
    using System.Globalization;
#if !NET7_0_OR_GREATER
    using System.Linq;
#endif
    using System.Runtime.InteropServices;
    using System.Text.RegularExpressions;


    /// <summary>
    /// Controls colored console output by <see langword="Pastel"/>.
    /// </summary>
#if NET7_0_OR_GREATER
    public static partial class ConsoleExtensions
#else
    public static class ConsoleExtensions
#endif
    {
        private const string Kernel32DllName = "kernel32";

        private const int  STD_OUTPUT_HANDLE                     = -11;
        private const uint ENABLE_PROCESSED_OUTPUT            = 0x0001;
        private const uint ENABLE_VIRTUAL_TERMINAL_PROCESSING = 0x0004;

#if NET7_0_OR_GREATER
        [LibraryImport(Kernel32DllName, EntryPoint = "GetConsoleMode")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static partial bool GetConsoleMode(nint hConsoleHandle, out uint lpMode);

        [LibraryImport(Kernel32DllName, EntryPoint = "SetConsoleMode")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static partial bool SetConsoleMode(nint hConsoleHandle, uint dwMode);

        [LibraryImport(Kernel32DllName, EntryPoint = "GetStdHandle", SetLastError = true)]
        private static partial nint GetStdHandle(int nStdHandle);
#else
        [DllImport(Kernel32DllName)]
        private static extern bool GetConsoleMode(IntPtr hConsoleHandle, out uint lpMode);

        [DllImport(Kernel32DllName)]
        private static extern bool SetConsoleMode(IntPtr hConsoleHandle, uint dwMode);

        [DllImport(Kernel32DllName, SetLastError = true)]
        private static extern IntPtr GetStdHandle(int nStdHandle);
#endif


        private static bool _enabled;

        private static readonly ReadOnlyDictionary<ConsoleColor, Color> _consoleColorMapper = new ReadOnlyDictionary<ConsoleColor, Color> (new Dictionary<ConsoleColor, Color> {
                                                                                                                                              [ConsoleColor.Black]       = Color.FromArgb(0x000000),
                                                                                                                                              [ConsoleColor.DarkBlue]    = Color.FromArgb(0x00008B),
                                                                                                                                              [ConsoleColor.DarkGreen]   = Color.FromArgb(0x006400),
                                                                                                                                              [ConsoleColor.DarkCyan]    = Color.FromArgb(0x008B8B),
                                                                                                                                              [ConsoleColor.DarkRed]     = Color.FromArgb(0x8B0000),
                                                                                                                                              [ConsoleColor.DarkMagenta] = Color.FromArgb(0x8B008B),
                                                                                                                                              [ConsoleColor.DarkYellow]  = Color.FromArgb(0x808000),
                                                                                                                                              [ConsoleColor.Gray]        = Color.FromArgb(0x808080),
                                                                                                                                              [ConsoleColor.DarkGray]    = Color.FromArgb(0xA9A9A9),
                                                                                                                                              [ConsoleColor.Blue]        = Color.FromArgb(0x0000FF),
                                                                                                                                              [ConsoleColor.Green]       = Color.FromArgb(0x008000),
                                                                                                                                              [ConsoleColor.Cyan]        = Color.FromArgb(0x00FFFF),
                                                                                                                                              [ConsoleColor.Red]         = Color.FromArgb(0xFF0000),
                                                                                                                                              [ConsoleColor.Magenta]     = Color.FromArgb(0xFF00FF),
                                                                                                                                              [ConsoleColor.Yellow]      = Color.FromArgb(0xFFFF00),
                                                                                                                                              [ConsoleColor.White]       = Color.FromArgb(0xFFFFFF)
                                                                                                                                           });

        private delegate string ColorFormat(   string input, Color     color);
        private delegate string HexColorFormat(string input, string hexColor);

        private enum ColorPlane : byte
        {
            Foreground,
            Background
        }

        private const string _formatStringStart   = "\u001b[{0};2;";
        private const string _formatStringColor   = "{1};{2};{3}m";
        private const string _formatStringContent = "{4}";
        private const string _formatStringEnd     = "\u001b[0m";
#if NET6_0_OR_GREATER
        private const string _formatStringPartial = $"{_formatStringStart}{_formatStringColor}";
        private const string _formatStringFull    = $"{_formatStringStart}{_formatStringColor}{_formatStringContent}{_formatStringEnd}";
#else
        private static readonly string _formatStringPartial = $"{_formatStringStart}{_formatStringColor}";
        private static readonly string _formatStringFull    = $"{_formatStringStart}{_formatStringColor}{_formatStringContent}{_formatStringEnd}";
#endif
#if !NET7_0_OR_GREATER
        private static readonly string _closeNestedPastelStringRegex3FormatString = $"(?:{_formatStringEnd.Replace("[", @"\[")})(?!{_formatStringStart.Replace("[", @"\[")})(?!$)";
#endif

        private static readonly ReadOnlyDictionary<ColorPlane, string> _planeFormatModifiers = new ReadOnlyDictionary<ColorPlane, string>(new Dictionary<ColorPlane, string>
                                                                                               {
                                                                                                   [ColorPlane.Foreground] = "38",
                                                                                                   [ColorPlane.Background] = "48"
                                                                                               });
#if NET7_0_OR_GREATER
        [GeneratedRegex("(?:\u001b\\[0m)+")]
        private static partial Regex CloseNestedPastelStringRegex1();

        [GeneratedRegex("(?<!^)(?<!\u001b\\[0m)(?<!\u001b\\[(?:38|48);2;\\d{1,3};\\d{1,3};\\d{1,3}m)(?:\u001b\\[(?:38|48);2;)")]
        private static partial Regex CloseNestedPastelStringRegex2();

        [GeneratedRegex("(?:\u001b\\[0m)(?!\u001b\\[38;2;)(?!$)")]
        private static partial Regex CloseNestedPastelStringRegex3Foreground();

        [GeneratedRegex("(?:\u001b\\[0m)(?!\u001b\\[48;2;)(?!$)")]
        private static partial Regex CloseNestedPastelStringRegex3Background();

        private static readonly ReadOnlyDictionary<ColorPlane, Regex> _closeNestedPastelStringRegex3 = new ReadOnlyDictionary<ColorPlane, Regex>(new Dictionary<ColorPlane, Regex>
                                                                                                       {
                                                                                                           [ColorPlane.Foreground] = CloseNestedPastelStringRegex3Foreground(),
                                                                                                           [ColorPlane.Background] = CloseNestedPastelStringRegex3Background()
                                                                                                       });
#else
        private static readonly Regex _closeNestedPastelStringRegex1 = new Regex($"({_formatStringEnd.Replace("[", @"\[")})+", RegexOptions.Compiled);
        private static readonly Regex _closeNestedPastelStringRegex2 = new Regex($"(?<!^)(?<!{_formatStringEnd.Replace("[", @"\[")})(?<!{string.Format($"{_formatStringStart.Replace("[", @"\[")}{_formatStringColor}", new[] { $"(?:{_planeFormatModifiers[ColorPlane.Foreground]}|{_planeFormatModifiers[ColorPlane.Background]})" }.Concat(Enumerable.Repeat(@"\d{1,3}", 3)).Cast<object>().ToArray())})(?:{string.Format(_formatStringStart.Replace("[", @"\["), $"(?:{_planeFormatModifiers[ColorPlane.Foreground]}|{_planeFormatModifiers[ColorPlane.Background]})")})", RegexOptions.Compiled);

        private static Regex CloseNestedPastelStringRegex1() => _closeNestedPastelStringRegex1;

        private static Regex CloseNestedPastelStringRegex2() => _closeNestedPastelStringRegex2;

        private static readonly ReadOnlyDictionary<ColorPlane, Regex> _closeNestedPastelStringRegex3 = new ReadOnlyDictionary<ColorPlane, Regex>(new[] { ColorPlane.Foreground, ColorPlane.Background }.ToDictionary(p => p,
                                                                                                                                                                                                                     p => new Regex(string.Format(_closeNestedPastelStringRegex3FormatString, _planeFormatModifiers[p]), RegexOptions.Compiled)));
#endif



        private static readonly Func<string, int> _parseHexColor = hc => int.Parse(hc.Replace("#", ""), NumberStyles.HexNumber);

        private static readonly Func<string,  Color, ColorPlane, string> _colorFormat    = (i, c, p) => string.Format(_formatStringFull, _planeFormatModifiers[p], c.R, c.G, c.B, CloseNestedPastelStrings(i, c, p));
        private static readonly Func<string, string, ColorPlane, string> _colorHexFormat = (i, c, p) => _colorFormat(i, Color.FromArgb(_parseHexColor(c)), p);

        private static readonly ColorFormat    _noColorOutputFormat    = (i, _) => i;
        private static readonly HexColorFormat _noHexColorOutputFormat = (i, _) => i;

        private static readonly ColorFormat    _foregroundColorFormat    = (i, c) => _colorFormat(   i, c, ColorPlane.Foreground);
        private static readonly HexColorFormat _foregroundHexColorFormat = (i, c) => _colorHexFormat(i, c, ColorPlane.Foreground);

        private static readonly ColorFormat    _backgroundColorFormat    = (i, c) => _colorFormat(   i, c, ColorPlane.Background);
        private static readonly HexColorFormat _backgroundHexColorFormat = (i, c) => _colorHexFormat(i, c, ColorPlane.Background);



        private static readonly ReadOnlyDictionary<bool, ReadOnlyDictionary<ColorPlane, ColorFormat>>       _colorFormatFuncs = new ReadOnlyDictionary<bool, ReadOnlyDictionary<ColorPlane, ColorFormat>>(new Dictionary<bool, ReadOnlyDictionary<ColorPlane, ColorFormat>>
                                                                                                                                {
                                                                                                                                    [false] = new ReadOnlyDictionary<ColorPlane, ColorFormat>(new Dictionary<ColorPlane, ColorFormat>
                                                                                                                                                                                              {
                                                                                                                                                                                                  [ColorPlane.Foreground] = _noColorOutputFormat,
                                                                                                                                                                                                  [ColorPlane.Background] = _noColorOutputFormat
                                                                                                                                                                                              }),
                                                                                                                                    [true] = new ReadOnlyDictionary<ColorPlane, ColorFormat>(new Dictionary<ColorPlane, ColorFormat>
                                                                                                                                                                                             {
                                                                                                                                                                                                 [ColorPlane.Foreground] = _foregroundColorFormat,
                                                                                                                                                                                                 [ColorPlane.Background] = _backgroundColorFormat
                                                                                                                                                                                             })
                                                                                                                                });
        private static readonly ReadOnlyDictionary<bool, ReadOnlyDictionary<ColorPlane, HexColorFormat>> _hexColorFormatFuncs = new ReadOnlyDictionary<bool, ReadOnlyDictionary<ColorPlane, HexColorFormat>>(new Dictionary<bool, ReadOnlyDictionary<ColorPlane, HexColorFormat>>
                                                                                                                                {
                                                                                                                                    [false] = new ReadOnlyDictionary<ColorPlane, HexColorFormat>(new Dictionary<ColorPlane, HexColorFormat>
                                                                                                                                                                                                 {
                                                                                                                                                                                                     [ColorPlane.Foreground] = _noHexColorOutputFormat,
                                                                                                                                                                                                     [ColorPlane.Background] = _noHexColorOutputFormat
                                                                                                                                                                                                 }),
                                                                                                                                    [true] = new ReadOnlyDictionary<ColorPlane, HexColorFormat>(new Dictionary<ColorPlane, HexColorFormat>
                                                                                                                                                                                                {
                                                                                                                                                                                                    [ColorPlane.Foreground] = _foregroundHexColorFormat,
                                                                                                                                                                                                    [ColorPlane.Background] = _backgroundHexColorFormat
                                                                                                                                                                                                })
                                                                                                                                });

        


        static ConsoleExtensions()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                var iStdOut =   GetStdHandle(STD_OUTPUT_HANDLE);
                var _ =    GetConsoleMode(iStdOut, out var outConsoleMode)
                        && SetConsoleMode(iStdOut, outConsoleMode | ENABLE_PROCESSED_OUTPUT | ENABLE_VIRTUAL_TERMINAL_PROCESSING);
            }


            if (EnvironmentDetector.ColorsDisabled)
            {
                Disable();
            }
            else
            {
                Enable();
            }
        }




        /// <summary>
        /// Enables any future console color output produced by Pastel.
        /// </summary>
        public static void Enable()
        {
            _enabled = true;
        }

        /// <summary>
        /// Disables any future console color output produced by Pastel.
        /// </summary>
        public static void Disable()
        {
            _enabled = false;
        }


        /// <summary>
        /// Returns a string wrapped in an ANSI foreground color code using the specified color.
        /// </summary>
        /// <param name="input">The string to color.</param>
        /// <param name="color">The color to use on the specified string.</param>
        public static string Pastel(this string input, Color color)
        {
            return _colorFormatFuncs[_enabled][ColorPlane.Foreground](input, color);
        }
        
        /// <summary>
        /// Returns a string wrapped in an ANSI foreground color code using the specified color.
        /// </summary>
        /// <param name="input">The string to color.</param>
        /// <param name="color">The color to use on the specified string.</param>
        public static string Pastel(this string input, ConsoleColor color)
        {
            return Pastel(input, _consoleColorMapper[color]);
        }

        /// <summary>
        /// Returns a string wrapped in an ANSI foreground color code using the specified color.
        /// </summary>
        /// <param name="input">The string to color.</param>
        /// <param name="hexColor">The color to use on the specified string.<para>Supported format: [#]RRGGBB.</para></param>
        public static string Pastel(this string input, string hexColor)
        {
            return _hexColorFormatFuncs[_enabled][ColorPlane.Foreground](input, hexColor);
        }



        /// <summary>
        /// Returns a string wrapped in an ANSI background color code using the specified color.
        /// </summary>
        /// <param name="input">The string to color.</param>
        /// <param name="color">The color to use on the specified string.</param>
        public static string PastelBg(this string input, Color color)
        {
            return _colorFormatFuncs[_enabled][ColorPlane.Background](input, color);
        }

        /// <summary>
        /// Returns a string wrapped in an ANSI background color code using the specified color.
        /// </summary>
        /// <param name="input">The string to color.</param>
        /// <param name="color">The color to use on the specified string.</param>
        public static string PastelBg(this string input, ConsoleColor color)
        {
            return PastelBg(input, _consoleColorMapper[color]);
        }

        /// <summary>
        /// Returns a string wrapped in an ANSI background color code using the specified color.
        /// </summary>
        /// <param name="input">The string to color.</param>
        /// <param name="hexColor">The color to use on the specified string.<para>Supported format: [#]RRGGBB.</para></param>
        public static string PastelBg(this string input, string hexColor)
        {
            return _hexColorFormatFuncs[_enabled][ColorPlane.Background](input, hexColor);
        }



        private static string CloseNestedPastelStrings(string input, Color color, ColorPlane colorPlane)
        {
            var closedString = CloseNestedPastelStringRegex1().Replace(input, _formatStringEnd);

            closedString = CloseNestedPastelStringRegex2().Replace(closedString, $"{_formatStringEnd}$0");
            closedString = _closeNestedPastelStringRegex3[colorPlane].Replace(closedString, $"$0{string.Format(_formatStringPartial, _planeFormatModifiers[colorPlane], color.R, color.G, color.B)}");
            
            return closedString;
        }
    }
}
