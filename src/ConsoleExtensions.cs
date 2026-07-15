namespace Pastel
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Drawing;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    /// <summary>
    /// Controls colored console output by <see langword="Pastel"/>.
    /// </summary>
#if NET8_0_OR_GREATER
    public static partial class ConsoleExtensions
#else
    public static class ConsoleExtensions
#endif
    {
        private const string Kernel32DllName = "kernel32";

        private const int  STD_OUTPUT_HANDLE                     = -11;
        private const uint ENABLE_PROCESSED_OUTPUT            = 0x0001;
        private const uint ENABLE_VIRTUAL_TERMINAL_PROCESSING = 0x0004;

#if NET8_0_OR_GREATER
        [LibraryImport(Kernel32DllName, EntryPoint = "GetConsoleMode")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static partial bool GetConsoleMode(nint hConsoleHandle, out uint lpMode);

        [LibraryImport(Kernel32DllName, EntryPoint = "SetConsoleMode")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static partial bool SetConsoleMode(nint hConsoleHandle, uint dwMode);

        [LibraryImport(Kernel32DllName, EntryPoint = "GetStdHandle")]
        private static partial nint GetStdHandle(int nStdHandle);
#else
        [DllImport(Kernel32DllName)]
        private static extern bool GetConsoleMode(IntPtr hConsoleHandle, out uint lpMode);

        [DllImport(Kernel32DllName)]
        private static extern bool SetConsoleMode(IntPtr hConsoleHandle, uint dwMode);

        [DllImport(Kernel32DllName)]
        private static extern IntPtr GetStdHandle(int nStdHandle);
#endif

        private static bool _enabled;

        // All three mappers are indexed by the numeric value of the ConsoleColor enum, which is contiguous in the range 0-15

        // The combined CSS3 list: the Web value is used for every name where Web and X11 disagree (Gray, Green), the X11 value where they agree (the Dark* shades).
        // This mixed heritage is why DarkGray (#A9A9A9, from X11) is lighter than Gray (#808080, from the web). Note that this is not the Windows console palette.
        private static readonly Color[] s_consoleColorWebMapper = {
                                                                      /* Black       */ Color.FromArgb(0x000000),
                                                                      /* DarkBlue    */ Color.FromArgb(0x00008B),
                                                                      /* DarkGreen   */ Color.FromArgb(0x006400),
                                                                      /* DarkCyan    */ Color.FromArgb(0x008B8B),
                                                                      /* DarkRed     */ Color.FromArgb(0x8B0000),
                                                                      /* DarkMagenta */ Color.FromArgb(0x8B008B),
                                                                      /* DarkYellow  */ Color.FromArgb(0x808000),
                                                                      /* Gray        */ Color.FromArgb(0x808080),   // Web Gray; X11 Gray is #BEBEBE
                                                                      /* DarkGray    */ Color.FromArgb(0xA9A9A9),   // Lighter than Gray above, by virtue of descending from X11
                                                                      /* Blue        */ Color.FromArgb(0x0000FF),
                                                                      /* Green       */ Color.FromArgb(0x008000),   // Web Green; X11 Green is #00FF00 (the web's Lime)
                                                                      /* Cyan        */ Color.FromArgb(0x00FFFF),
                                                                      /* Red         */ Color.FromArgb(0xFF0000),
                                                                      /* Magenta     */ Color.FromArgb(0xFF00FF),
                                                                      /* Yellow      */ Color.FromArgb(0xFFFF00),
                                                                      /* White       */ Color.FromArgb(0xFFFFFF)
                                                                  };

        private static readonly char[][] s_consoleColorMapperFg = {
                                                                      /* Black       */ new[] { '3', '0' },
                                                                      /* DarkBlue    */ new[] { '3', '4' },
                                                                      /* DarkGreen   */ new[] { '3', '2' },
                                                                      /* DarkCyan    */ new[] { '3', '6' },
                                                                      /* DarkRed     */ new[] { '3', '1' },
                                                                      /* DarkMagenta */ new[] { '3', '5' },
                                                                      /* DarkYellow  */ new[] { '3', '3' },
                                                                      /* Gray        */ new[] { '3', '7' },
                                                                      /* DarkGray    */ new[] { '9', '0' },
                                                                      /* Blue        */ new[] { '9', '4' },
                                                                      /* Green       */ new[] { '9', '2' },
                                                                      /* Cyan        */ new[] { '9', '6' },
                                                                      /* Red         */ new[] { '9', '1' },
                                                                      /* Magenta     */ new[] { '9', '5' },
                                                                      /* Yellow      */ new[] { '9', '3' },
                                                                      /* White       */ new[] { '9', '7' }
                                                                  };

        private static readonly char[][] s_consoleColorMapperBg = {
                                                                      /* Black       */ new[] { '4', '0' },
                                                                      /* DarkBlue    */ new[] { '4', '4' },
                                                                      /* DarkGreen   */ new[] { '4', '2' },
                                                                      /* DarkCyan    */ new[] { '4', '6' },
                                                                      /* DarkRed     */ new[] { '4', '1' },
                                                                      /* DarkMagenta */ new[] { '4', '5' },
                                                                      /* DarkYellow  */ new[] { '4', '3' },
                                                                      /* Gray        */ new[] { '4', '7' },
                                                                      /* DarkGray    */ new[] { '1', '0', '0' },
                                                                      /* Blue        */ new[] { '1', '0', '4' },
                                                                      /* Green       */ new[] { '1', '0', '2' },
                                                                      /* Cyan        */ new[] { '1', '0', '6' },
                                                                      /* Red         */ new[] { '1', '0', '1' },
                                                                      /* Magenta     */ new[] { '1', '0', '5' },
                                                                      /* Yellow      */ new[] { '1', '0', '3' },
                                                                      /* White       */ new[] { '1', '0', '7' }
                                                                  };

        private const char _fgColorPlaneFormatModifierInitialPart = '3';
        private const char _bgColorPlaneFormatModifierInitialPart = '4';

        private static readonly char[] s_singleDigitCharCache = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };

        // Values for '\0' to 'f' where 255 indicates invalid input character
        // Starting from '\0' and not from '0' costs 48 bytes but results in zero subtractions and less if statements
        private static readonly byte[] s_fromHexTable = {
                                                            255, 255, 255, 255, 255, 255, 255, 255, 255, 255,
                                                            255, 255, 255, 255, 255, 255, 255, 255, 255, 255,
                                                            255, 255, 255, 255, 255, 255, 255, 255, 255, 255,
                                                            255, 255, 255, 255, 255, 255, 255, 255, 255, 255,
                                                            255, 255, 255, 255, 255, 255, 255, 255,   0,   1,
                                                              2,   3,   4,   5,   6,   7,   8,   9, 255, 255,
                                                            255, 255, 255, 255, 255,  10,  11,  12,  13,  14,
                                                             15, 255, 255, 255, 255, 255, 255, 255, 255, 255,
                                                            255, 255, 255, 255, 255, 255, 255, 255, 255, 255,
                                                            255, 255, 255, 255, 255, 255, 255,  10,  11,  12,
                                                             13,  14,  15
                                                        };

        // Same as above but valid values are multiplied by 16
        private static readonly byte[] s_fromHexTable16 = {
                                                              255, 255, 255, 255, 255, 255, 255, 255, 255, 255,
                                                              255, 255, 255, 255, 255, 255, 255, 255, 255, 255,
                                                              255, 255, 255, 255, 255, 255, 255, 255, 255, 255,
                                                              255, 255, 255, 255, 255, 255, 255, 255, 255, 255,
                                                              255, 255, 255, 255, 255, 255, 255, 255,   0,  16,
                                                               32,  48,  64,  80,  96, 112, 128, 144, 255, 255,
                                                              255, 255, 255, 255, 255, 160, 176, 192, 208, 224,
                                                              240, 255, 255, 255, 255, 255, 255, 255, 255, 255,
                                                              255, 255, 255, 255, 255, 255, 255, 255, 255, 255,
                                                              255, 255, 255, 255, 255, 255, 255, 160, 176, 192,
                                                              208, 224, 240
                                                          };

        static ConsoleExtensions()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                var iStdOut =   GetStdHandle(STD_OUTPUT_HANDLE);
                var _ =    GetConsoleMode(iStdOut, out var outConsoleMode)
                        && SetConsoleMode(iStdOut, outConsoleMode | ENABLE_PROCESSED_OUTPUT | ENABLE_VIRTUAL_TERMINAL_PROCESSING);
            }


            if (EnvironmentDetector.ColorsEnabled())
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
        public static string Pastel(this string input, Color color) => Pastel(input.AsSpan(), color);

        /// <summary>
        /// Returns a string wrapped in an ANSI foreground color code using the specified color.
        /// </summary>
        /// <param name="input">The string to color.</param>
        /// <param name="color">The color to use on the specified string.</param>
        public static string Pastel(this in ReadOnlySpan<char> input, Color color)
        {
            if (_enabled)
            {
                return PastelInternal(in input, color.R, color.G, color.B, _fgColorPlaneFormatModifierInitialPart);
            }

            return input.ToString();
        }

        /// <summary>
        /// Returns a string wrapped in an ANSI foreground color code using the specified color.
        /// </summary>
        /// <param name="input">The string to color.</param>
        /// <param name="color">The color to use on the specified string.</param>
        /// <param name="useWebColors">If <see langword="true"/>, indicates to use the fixed web colour that the <see cref="ConsoleColor"/> value represents instead of its theme-defined value configured in the terminal.
        /// <para>Note that <see cref="ConsoleColor.DarkGray"/> (#A9A9A9) is lighter than <see cref="ConsoleColor.Gray"/> (#808080), as the former descends from X11 and the latter from the web.</para></param>
        public static string Pastel(this string input, ConsoleColor color, bool useWebColors = false) => Pastel(input.AsSpan(), color, useWebColors);

        /// <summary>
        /// Returns a string wrapped in an ANSI foreground color code using the specified color.
        /// </summary>
        /// <param name="input">The string to color.</param>
        /// <param name="color">The color to use on the specified string.</param>
        /// <param name="useWebColors">If <see langword="true"/>, indicates to use the fixed web colour that the <see cref="ConsoleColor"/> value represents instead of its theme-defined value configured in the terminal.
        /// <para>Note that <see cref="ConsoleColor.DarkGray"/> (#A9A9A9) is lighter than <see cref="ConsoleColor.Gray"/> (#808080), as the former descends from X11 and the latter from the web.</para></param>
        public static string Pastel(this in ReadOnlySpan<char> input, ConsoleColor color, bool useWebColors = false)
        {
            ValidateConsoleColor(color);

            if (useWebColors)
            {
                return Pastel(input, s_consoleColorWebMapper[(int)color]);
            }

            return PastelConsole(input, color);
        }

        /// <summary>
        /// Returns a string wrapped in an ANSI foreground color code using the specified console color.
        /// </summary>
        /// <param name="input">The string to color.</param>
        /// <param name="color">The console color to use on the specified string.</param>
        private static string PastelConsole(this in ReadOnlySpan<char> input, ConsoleColor color)
        {
            if (_enabled)
            {
                return PastelConsoleColorInternal(in input, s_consoleColorMapperFg[(int)color]);
            }

            return input.ToString();
        }

        /// <summary>
        /// Returns a string wrapped in an ANSI background color code using the specified console color.
        /// </summary>
        /// <param name="input">The string to color.</param>
        /// <param name="color">The console color to use on the specified string.</param>
        private static string PastelConsoleBg(this in ReadOnlySpan<char> input, ConsoleColor color)
        {
            if (_enabled)
            {
                return PastelConsoleColorInternal(in input, s_consoleColorMapperBg[(int)color]);
            }

            return input.ToString();
        }

        /// <summary>
        /// Returns a string wrapped in an ANSI foreground color code using the specified color.
        /// </summary>
        /// <param name="input">The string to color.</param>
        /// <param name="hexColor">The color to use on the specified string.<para>Supported formats: [#]RRGGBB and [#]RGB (case-insensitive).</para></param>
        public static string Pastel(this string input, in string hexColor) => Pastel(input.AsSpan(), hexColor.AsSpan());

        /// <summary>
        /// Returns a string wrapped in an ANSI foreground color code using the specified color.
        /// </summary>
        /// <param name="input">The string to color.</param>
        /// <param name="hexColor">The color to use on the specified string.<para>Supported formats: [#]RRGGBB and [#]RGB (case-insensitive).</para></param>
        public static string Pastel(this string input, in ReadOnlySpan<char> hexColor) => Pastel(input.AsSpan(), hexColor);

        /// <summary>
        /// Returns a string wrapped in an ANSI foreground color code using the specified color.
        /// </summary>
        /// <param name="input">The string to color.</param>
        /// <param name="hexColor">The color to use on the specified string.<para>Supported formats: [#]RRGGBB and [#]RGB (case-insensitive).</para></param>
        public static string Pastel(this in ReadOnlySpan<char> input, in string hexColor) => Pastel(input, hexColor.AsSpan());

        /// <summary>
        /// Returns a string wrapped in an ANSI foreground color code using the specified color.
        /// </summary>
        /// <param name="input">The string to color.</param>
        /// <param name="hexColor">The color to use on the specified string.<para>Supported formats: [#]RRGGBB and [#]RGB (case-insensitive).</para></param>
        public static string Pastel(this in ReadOnlySpan<char> input, in ReadOnlySpan<char> hexColor)
        {
            if (_enabled)
            {
                HexToRgb(hexColor, out var r, out var g, out var b);

                return PastelInternal(in input, r, g, b, _fgColorPlaneFormatModifierInitialPart);
            }

            return input.ToString();
        }

        /// <summary>
        /// Returns a string wrapped in an ANSI background color code using the specified color.
        /// </summary>
        /// <param name="input">The string to color.</param>
        /// <param name="color">The color to use on the specified string.</param>
        public static string PastelBg(this string input, Color color) => PastelBg(input.AsSpan(), color);

        /// <summary>
        /// Returns a string wrapped in an ANSI background color code using the specified color.
        /// </summary>
        /// <param name="input">The string to color.</param>
        /// <param name="color">The color to use on the specified string.</param>
        public static string PastelBg(this in ReadOnlySpan<char> input, Color color)
        {
            if (_enabled)
            {
                return PastelInternal(in input, color.R, color.G, color.B, _bgColorPlaneFormatModifierInitialPart);
            }

            return input.ToString();
        }

        /// <summary>
        /// Returns a string wrapped in an ANSI background color code using the specified color.
        /// </summary>
        /// <param name="input">The string to color.</param>
        /// <param name="color">The color to use on the specified string.</param>
        /// <param name="useWebColors">If <see langword="true"/>, indicates to use the fixed web colour that the <see cref="ConsoleColor"/> value represents instead of its theme-defined value configured in the terminal.
        /// <para>Note that <see cref="ConsoleColor.DarkGray"/> (#A9A9A9) is lighter than <see cref="ConsoleColor.Gray"/> (#808080), as the former descends from X11 and the latter from the web.</para></param>
        public static string PastelBg(this string input, ConsoleColor color, bool useWebColors = false) => PastelBg(input.AsSpan(), color, useWebColors);

        /// <summary>
        /// Returns a string wrapped in an ANSI background color code using the specified color.
        /// </summary>
        /// <param name="input">The string to color.</param>
        /// <param name="color">The color to use on the specified string.</param>
        /// <param name="useWebColors">If <see langword="true"/>, indicates to use the fixed web colour that the <see cref="ConsoleColor"/> value represents instead of its theme-defined value configured in the terminal.
        /// <para>Note that <see cref="ConsoleColor.DarkGray"/> (#A9A9A9) is lighter than <see cref="ConsoleColor.Gray"/> (#808080), as the former descends from X11 and the latter from the web.</para></param>
        public static string PastelBg(this in ReadOnlySpan<char> input, ConsoleColor color, bool useWebColors = false)
        {
            ValidateConsoleColor(color);

            if (useWebColors)
            {
                return PastelBg(input, s_consoleColorWebMapper[(int)color]);
            }

            return PastelConsoleBg(input, color);
        }

        /// <summary>
        /// Returns a string wrapped in an ANSI background color code using the specified color.
        /// </summary>
        /// <param name="input">The string to color.</param>
        /// <param name="hexColor">The color to use on the specified string.<para>Supported formats: [#]RRGGBB and [#]RGB (case-insensitive).</para></param>
        public static string PastelBg(this string input, string hexColor) => PastelBg(input.AsSpan(), hexColor.AsSpan());

        /// <summary>
        /// Returns a string wrapped in an ANSI background color code using the specified color.
        /// </summary>
        /// <param name="input">The string to color.</param>
        /// <param name="hexColor">The color to use on the specified string.<para>Supported formats: [#]RRGGBB and [#]RGB (case-insensitive).</para></param>
        public static string PastelBg(this string input, in ReadOnlySpan<char> hexColor) => PastelBg(input.AsSpan(), hexColor);

        /// <summary>
        /// Returns a string wrapped in an ANSI background color code using the specified color.
        /// </summary>
        /// <param name="input">The string to color.</param>
        /// <param name="hexColor">The color to use on the specified string.<para>Supported formats: [#]RRGGBB and [#]RGB (case-insensitive).</para></param>
        public static string PastelBg(this in ReadOnlySpan<char> input, string hexColor) => PastelBg(input, hexColor.AsSpan());

        /// <summary>
        /// Returns a string wrapped in an ANSI background color code using the specified color.
        /// </summary>
        /// <param name="input">The string to color.</param>
        /// <param name="hexColor">The color to use on the specified string.<para>Supported formats: [#]RRGGBB and [#]RGB (case-insensitive).</para></param>
        public static string PastelBg(this in ReadOnlySpan<char> input, in ReadOnlySpan<char> hexColor)
        {
            if (_enabled)
            {
                HexToRgb(hexColor, out var r, out var g, out var b);

                return PastelInternal(in input, r, g, b, _bgColorPlaneFormatModifierInitialPart);
            }

            return input.ToString();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static string PastelInternal(in ReadOnlySpan<char> input, byte r, byte g, byte b, char colorPlaneFormatModifierInitialPart)
        {
            var rDigitCount = CountDigits(r);
            var gDigitCount = CountDigits(g);
            var bDigitCount = CountDigits(b);

            var colorFormatStringInsertPositions = ScanColorFormatStringInsertPositions(in input);

#if NET8_0_OR_GREATER
            var addResetSuffix = input is not [.., '\x1b', '[', '0', 'm'];
#else
            var addResetSuffix = !input.EndsWith("\x1b[0m".AsSpan());
#endif
            var bufferLength = (7 + rDigitCount + 1 + gDigitCount + 1 + bDigitCount + 1) * (1 + colorFormatStringInsertPositions.Length) + input.Length + (addResetSuffix ? 4 : 0);

#if NET8_0_OR_GREATER
            var stringCreateData = new StringCreateData
                                   {
                                       ColorPlaneFormatModifierInitialPart = colorPlaneFormatModifierInitialPart,
                                       R = r,
                                       RDigitCount = rDigitCount,
                                       G = g,
                                       GDigitCount = gDigitCount,
                                       B = b,
                                       BDigitCount = bDigitCount,
                                       AddResetSuffix = addResetSuffix
                                   };
#else
            return PastelInternal(bufferLength,
                                  input,
                                  colorPlaneFormatModifierInitialPart,
                                  r,
                                  rDigitCount,
                                  g,
                                  gDigitCount,
                                  b,
                                  bDigitCount,
                                  colorFormatStringInsertPositions,
                                  addResetSuffix);
#endif

#if NET8_0_OR_GREATER
            return PastelInternal(bufferLength, input, in stringCreateData, colorFormatStringInsertPositions);
#endif
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#if NET8_0_OR_GREATER
        private static unsafe string PastelInternal(int bufferLength, in ReadOnlySpan<char> input, in StringCreateData stringCreateData, int[] colorFormatStringInsertPositions)
        {
            string pastelString;

            fixed (char* inputPtr = input)
            {
                fixed (StringCreateData* stringCreateDataPtr = &stringCreateData)
                {
                    pastelString = string.Create(bufferLength,
                                                 (
                                                  Ptr: (nint)stringCreateDataPtr,
                                                  Input: (nint)inputPtr,
                                                  InputLength: input.Length,
                                                  ColorFormatStringInsertPositions: colorFormatStringInsertPositions
                                                 ),
                                                 static (buf, ctx) =>
                                                 {
                                                     var ctxPtr = *(StringCreateData*)ctx.Ptr;

                                                     int i = 0;

                                                     buf[i++] = '\x1b';
                                                     buf[i++] = '[';
                                                     buf[i++] = ctxPtr.ColorPlaneFormatModifierInitialPart;
                                                     buf[i++] = '8';
                                                     buf[i++] = ';';
                                                     buf[i++] = '2';
                                                     buf[i++] = ';';

                                                     ByteToString(buf.Slice(i, ctxPtr.RDigitCount), ctxPtr.R);
                                                     i += ctxPtr.RDigitCount;

                                                     buf[i++] = ';';

                                                     ByteToString(buf.Slice(i, ctxPtr.GDigitCount), ctxPtr.G);
                                                     i += ctxPtr.GDigitCount;

                                                     buf[i++] = ';';

                                                     ByteToString(buf.Slice(i, ctxPtr.BDigitCount), ctxPtr.B);
                                                     i += ctxPtr.BDigitCount;

                                                     buf[i++] = 'm';


                                                     var colorFormatStringBuf = buf.Slice(0, i);
                                                     var textSpan = new ReadOnlySpan<char>((char*)ctx.Input, ctx.InputLength);
                                                     var currentIndex = i;
                                                     if (ctx.ColorFormatStringInsertPositions.Length > 0)
                                                     {
                                                         int previousInsertPos = 0;

                                                         for (var colorFormatStringIndex = 0; colorFormatStringIndex < ctx.ColorFormatStringInsertPositions.Length; colorFormatStringIndex++)
                                                         {
                                                             var currentInsertPos = ctx.ColorFormatStringInsertPositions[colorFormatStringIndex];
                                                             var segmentLength = currentInsertPos - previousInsertPos;

                                                             CopySegmentToBuffer(textSpan.Slice(previousInsertPos, segmentLength), buf, ref currentIndex);
                                                             CopySegmentToBuffer(colorFormatStringBuf, buf, ref currentIndex);

                                                             previousInsertPos = currentInsertPos;
                                                         }

                                                         CopySegmentToBuffer(textSpan.Slice(previousInsertPos), buf, ref currentIndex);
                                                     }
                                                     else
                                                     {
                                                         CopySegmentToBuffer(textSpan, buf, ref currentIndex);
                                                     }

                                                     if (ctxPtr.AddResetSuffix)
                                                     {
                                                         buf[currentIndex++] = '\x1b';
                                                         buf[currentIndex++] = '[';
                                                         buf[currentIndex++] = '0';
                                                         buf[currentIndex] = 'm';
                                                     }
                                                 });
                }
            }

            return pastelString;
        }
#else
        private static string PastelInternal(int bufferLength, in ReadOnlySpan<char> input, char colorPlaneFormatModifierInitialPart, byte r, byte rDigitCount, byte g, byte gDigitCount, byte b, byte bDigitCount, int[] colorFormatStringInsertPositions, bool addResetSuffix)
        {
            var buf = new char[bufferLength];
            var bufSpan = buf.AsSpan();

            int i = 0;

            bufSpan[i++] = '\x1b';
            bufSpan[i++] = '[';
            bufSpan[i++] = colorPlaneFormatModifierInitialPart;
            bufSpan[i++] = '8';
            bufSpan[i++] = ';';
            bufSpan[i++] = '2';
            bufSpan[i++] = ';';

            IntToString(bufSpan.Slice(i, rDigitCount), r);
            i += rDigitCount;

            bufSpan[i++] = ';';

            IntToString(bufSpan.Slice(i, gDigitCount), g);
            i += gDigitCount;

            bufSpan[i++] = ';';

            IntToString(bufSpan.Slice(i, bDigitCount), b);
            i += bDigitCount;

            bufSpan[i++] = 'm';


            var colorFormatStringBuf = bufSpan.Slice(0, i);
            var currentIndex = i;
            if (colorFormatStringInsertPositions.Length > 0)
            {
                int previousInsertPos = 0;

                for (var colorFormatStringIndex = 0; colorFormatStringIndex < colorFormatStringInsertPositions.Length; colorFormatStringIndex++)
                {
                    var currentInsertPos = colorFormatStringInsertPositions[colorFormatStringIndex];
                    var segmentLength = currentInsertPos - previousInsertPos;

                    CopySegmentToBuffer(input.Slice(previousInsertPos, segmentLength), buf, ref currentIndex);
                    CopySegmentToBuffer(colorFormatStringBuf, buf, ref currentIndex);

                    previousInsertPos = currentInsertPos;
                }

                CopySegmentToBuffer(input.Slice(previousInsertPos), buf, ref currentIndex);
            }
            else
            {
                CopySegmentToBuffer(input, buf, ref currentIndex);
            }

            if (addResetSuffix)
            {
                bufSpan[currentIndex++] = '\x1b';
                bufSpan[currentIndex++] = '[';
                bufSpan[currentIndex++] = '0';
                bufSpan[currentIndex] = 'm';
            }

            return new string(buf);
        }
#endif

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static string PastelConsoleColorInternal(in ReadOnlySpan<char> input, char[] consoleColorValue)
        {
            var colorFormatStringInsertPositions = ScanColorFormatStringInsertPositions(in input);

#if NET8_0_OR_GREATER
            var addResetSuffix = input is not [.., '\x1b', '[', '0', 'm'];
#else
            var addResetSuffix = !input.EndsWith("\x1b[0m".AsSpan());
#endif
            var bufferLength = (2 + consoleColorValue.Length + 1) * (1 + colorFormatStringInsertPositions.Length) + input.Length + (addResetSuffix ? 4 : 0); // \x1b[ + consoleColorValue + m + input (+ \x1b[0m)

#if NET8_0_OR_GREATER
            string pastelString;

            unsafe
            {
                fixed (char* inputPtr = input)
                {
                    pastelString = string.Create(bufferLength,
                                                 (
                                                  Input: (nint)inputPtr,
                                                  InputLength: input.Length,
                                                  ConsoleColorValue: consoleColorValue,
                                                  AddResetSuffix: addResetSuffix,
                                                  ColorFormatStringInsertPositions: colorFormatStringInsertPositions
                                                 ),
                                                 static (buf, ctx) =>
                                                 {
                                                     int i = 0;

                                                     buf[i++] = '\x1b';
                                                     buf[i++] = '[';

                                                     ctx.ConsoleColorValue.CopyTo(buf.Slice(i));
                                                     i += ctx.ConsoleColorValue.Length;

                                                     buf[i++] = 'm';

                                                     var colorFormatStringBuf = buf.Slice(0, i);
                                                     var textSpan = new ReadOnlySpan<char>((char*)ctx.Input, ctx.InputLength);
                                                     var currentIndex = i;
                                                     if (ctx.ColorFormatStringInsertPositions.Length > 0)
                                                     {
                                                         int previousInsertPos = 0;

                                                         for (var colorFormatStringIndex = 0; colorFormatStringIndex < ctx.ColorFormatStringInsertPositions.Length; colorFormatStringIndex++)
                                                         {
                                                             var currentInsertPos = ctx.ColorFormatStringInsertPositions[colorFormatStringIndex];
                                                             var segmentLength = currentInsertPos - previousInsertPos;

                                                             CopySegmentToBuffer(textSpan.Slice(previousInsertPos, segmentLength), buf, ref currentIndex);
                                                             CopySegmentToBuffer(colorFormatStringBuf, buf, ref currentIndex);

                                                             previousInsertPos = currentInsertPos;
                                                         }

                                                         CopySegmentToBuffer(textSpan.Slice(previousInsertPos), buf, ref currentIndex);
                                                     }
                                                     else
                                                     {
                                                         CopySegmentToBuffer(textSpan, buf, ref currentIndex);
                                                     }

                                                     if (ctx.AddResetSuffix)
                                                     {
                                                         buf[currentIndex++] = '\x1b';
                                                         buf[currentIndex++] = '[';
                                                         buf[currentIndex++] = '0';
                                                         buf[currentIndex] = 'm';
                                                     }
                                                 });
                }
            }

            return pastelString;
#else
            var buf = new char[bufferLength];
            var bufSpan = buf.AsSpan();

            int i = 0;

            bufSpan[i++] = '\x1b';
            bufSpan[i++] = '[';

            consoleColorValue.CopyTo(bufSpan.Slice(i));
            i += consoleColorValue.Length;

            bufSpan[i++] = 'm';

            var colorFormatStringBuf = bufSpan.Slice(0, i);
            var currentIndex = i;
            if (colorFormatStringInsertPositions.Length > 0)
            {
                int previousInsertPos = 0;

                for (var colorFormatStringIndex = 0; colorFormatStringIndex < colorFormatStringInsertPositions.Length; colorFormatStringIndex++)
                {
                    var currentInsertPos = colorFormatStringInsertPositions[colorFormatStringIndex];
                    var segmentLength = currentInsertPos - previousInsertPos;

                    CopySegmentToBuffer(input.Slice(previousInsertPos, segmentLength), buf, ref currentIndex);
                    CopySegmentToBuffer(colorFormatStringBuf, buf, ref currentIndex);

                    previousInsertPos = currentInsertPos;
                }

                CopySegmentToBuffer(input.Slice(previousInsertPos), buf, ref currentIndex);
            }
            else
            {
                CopySegmentToBuffer(input, buf, ref currentIndex);
            }

            if (addResetSuffix)
            {
                bufSpan[currentIndex++] = '\x1b';
                bufSpan[currentIndex++] = '[';
                bufSpan[currentIndex++] = '0';
                bufSpan[currentIndex] = 'm';
            }

            return new string(buf);
#endif
        }

        /// <summary>
        /// Locates every escape sequence reset (<c>\x1b[0m</c>) that is neither at the very end of the input nor immediately followed by another reset,
        /// i.e. all solitary resets between the start and the end of the input string. The color format string has to be re-inserted at each of these
        /// positions, otherwise the remainder of the input would render uncolored.
        /// </summary>
        /// <param name="input">The string to scan.</param>
        /// <returns>The positions at which the color format string has to be re-inserted.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int[] ScanColorFormatStringInsertPositions(in ReadOnlySpan<char> input)
        {
            var formatStringStartColorInsertCount = CountColorFormatStringInsertPositions(in input, null);

            if (formatStringStartColorInsertCount == 0)
            {
                return Array.Empty<int>();
            }

            var colorFormatStringInsertPositions = new int[formatStringStartColorInsertCount];

            CountColorFormatStringInsertPositions(in input, colorFormatStringInsertPositions);

            return colorFormatStringInsertPositions;
        }

        /// <summary>
        /// Counts the positions at which the color format string has to be re-inserted, optionally recording them.
        /// </summary>
        /// <param name="input">The string to scan.</param>
        /// <param name="colorFormatStringInsertPositions">If not <see langword="null"/>, receives the positions found. Must be able to hold as many positions as a preceding counting pass reported.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int CountColorFormatStringInsertPositions(in ReadOnlySpan<char> input, int[] colorFormatStringInsertPositions)
        {
            var formatStringStartColorInsertCount = 0;

            var haystack  = input;
            var offsetPos = 0;

            int pos;
#if NET8_0_OR_GREATER
            while ((pos = haystack.IndexOf("\x1b[0m")) >= 0)
#else
            while ((pos = haystack.IndexOf("\x1b[0m".AsSpan())) >= 0)
#endif
            {
                haystack   = haystack.Slice(pos + 4);
                offsetPos += pos + 4;

                // A trailing partial escape sequence (e.g. "\x1b[" or "\x1b[0") is not a reset, so it counts as an insert position
#if NET8_0_OR_GREATER
                if (haystack is not [] and not ['\x1b', '[', '0', 'm', ..])
#else
                if (!haystack.IsEmpty && !haystack.StartsWith("\x1b[0m".AsSpan()))
#endif
                {
                    if (colorFormatStringInsertPositions != null)
                    {
                        colorFormatStringInsertPositions[formatStringStartColorInsertCount] = offsetPos;
                    }

                    formatStringStartColorInsertCount++;
                }
            }

            return formatStringStartColorInsertCount;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void CopySegmentToBuffer(in ReadOnlySpan<char> segment, Span<char> destination, ref int currentIndex)
        {
            segment.CopyTo(destination.Slice(currentIndex));
            currentIndex += segment.Length;
        }

        /// <summary>
        /// Throws if the specified value is not one of the defined <see cref="ConsoleColor"/> members, as all mappers are indexed by its numeric value.
        /// </summary>
        /// <param name="color">The console color to validate.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void ValidateConsoleColor(ConsoleColor color)
        {
            // A single unsigned comparison covers both negative values and values greater than White (15)
            if ((uint)color > (uint)ConsoleColor.White)
            {
                throw new ArgumentOutOfRangeException(nameof(color), color, $"Undefined {nameof(ConsoleColor)} value.");
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static byte CountDigits(byte value)
        {
            byte digits = 1;

            if (value < 10)
            {
                // no-op
            }
            else if (value < 100)
            {
                digits++;
            }
            else
            {
                digits += 2;
            }

            return digits;
        }

#if NET8_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void ByteToString(Span<char> buffer, byte value)
        {
            if (buffer.Length == 1)
            {
                var singleDigitCharCache = s_singleDigitCharCache;
                buffer[0] = singleDigitCharCache[value];

                return;
            }

            var i = buffer.Length;
            do
            {
                (value, var remainder) = Math.DivRem(value, (byte)10);
                buffer[--i] = (char)(remainder + '0');
            }
            while (value != 0);
        }
#else
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void IntToString(Span<char> buffer, int value)
        {
            if (buffer.Length == 1)
            {
                var singleDigitCharCache = s_singleDigitCharCache;
                buffer[0] = singleDigitCharCache[value];

                return;
            }

            var i = buffer.Length;
            do
            {
                value = Math.DivRem(value, 10, out var remainder);
                buffer[--i] = (char)(remainder + '0');
            }
            while (value != 0);
        }
#endif

        private static void HexToRgb(ReadOnlySpan<char> hexString, out byte r, out byte g, out byte b)
        {
#if NET8_0_OR_GREATER
            if (hexString is ['#', ..])
#else
            if (hexString.Length > 1 && hexString[0] == '#')
#endif
            {
                hexString = hexString.Slice(1);
            }

            switch (hexString.Length)
            {
                case 3:
                    r = FromChar(in hexString[0], in hexString[0], in hexString);
                    g = FromChar(in hexString[1], in hexString[1], in hexString);
                    b = FromChar(in hexString[2], in hexString[2], in hexString);

                    return;
                case 6:
                    r = FromChar(in hexString[0], in hexString[1], in hexString);
                    g = FromChar(in hexString[2], in hexString[3], in hexString);
                    b = FromChar(in hexString[4], in hexString[5], in hexString);

                    return;
                default:
                    throw InvalidHexadecimalColorValueException(hexString);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static byte FromChar(in char byteHi, in char byteLo, in ReadOnlySpan<char> hexString)
        {
            byte result, add;

            if (    byteHi > 102
                || (result = s_fromHexTable16[byteHi]) == 255
                ||  byteLo > 102
                || (add = s_fromHexTable[byteLo]) == 255
               )
            {
                throw InvalidHexadecimalColorValueException(hexString);
            }

            return (byte)(result + add);
        }

        // The parameter is named hexColor on every public overload, so that's the name callers have to be given
        private const string HexColorParameterName = "hexColor";

#if NET8_0_OR_GREATER
        private static ArgumentException InvalidHexadecimalColorValueException(in ReadOnlySpan<char> hexString) => new($"Invalid hexadecimal color value encountered: {hexString}", HexColorParameterName);
#else
        private static ArgumentException InvalidHexadecimalColorValueException(in ReadOnlySpan<char> hexString) => new ArgumentException($"Invalid hexadecimal color value encountered: {hexString.ToString()}", HexColorParameterName);
#endif

#if NET8_0_OR_GREATER
        private readonly struct StringCreateData
        {
            public required char ColorPlaneFormatModifierInitialPart { get; init; }

            public required byte R { get; init; }

            public required byte RDigitCount { get; init; }

            public required byte G { get; init; }

            public required byte GDigitCount { get; init; }

            public required byte B { get; init; }

            public required byte BDigitCount { get; init; }

            public required bool AddResetSuffix { get; init; }
        }
#endif
    }
}
