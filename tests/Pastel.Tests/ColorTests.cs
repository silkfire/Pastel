namespace Pastel.Tests
{
    using Xunit;

    using System;
    using System.Drawing;

    public class ColorTests
    {
        public class ForegroundColor
        {
            [Theory]
            [InlineData(3,  40, 255, "",      "\x1b[38;2;3;40;255m\x1b[0m")]
            [InlineData(1,   1,   1, "input", "\x1b[38;2;1;1;1minput\x1b[0m")]
            [InlineData(44, 70, 125, "input", "\x1b[38;2;44;70;125minput\x1b[0m")]
            public void Given_Specified_RGB_Color_And_Input_String_Should_Return_Specified_String(int red, int green, int blue, string inputString, string expectedAnsiColorString)
            {
                var outputAnsiColorString = inputString.Pastel(Color.FromArgb(red, green, blue));

                Assert.Equal(expectedAnsiColorString, outputAnsiColorString);
            }

            [Theory]
            [InlineData(ConsoleColor.Black,       "30")]
            [InlineData(ConsoleColor.DarkRed,     "31")]
            [InlineData(ConsoleColor.DarkGreen,   "32")]
            [InlineData(ConsoleColor.DarkYellow,  "33")]
            [InlineData(ConsoleColor.DarkBlue,    "34")]
            [InlineData(ConsoleColor.DarkMagenta, "35")]
            [InlineData(ConsoleColor.DarkCyan,    "36")]
            [InlineData(ConsoleColor.Gray,        "37")]
            [InlineData(ConsoleColor.DarkGray,    "90")]
            [InlineData(ConsoleColor.Red,         "91")]
            [InlineData(ConsoleColor.Green,       "92")]
            [InlineData(ConsoleColor.Yellow,      "93")]
            [InlineData(ConsoleColor.Blue,        "94")]
            [InlineData(ConsoleColor.Magenta,     "95")]
            [InlineData(ConsoleColor.Cyan,        "96")]
            [InlineData(ConsoleColor.White,       "97")]
            public void Given_Specified_ConsoleColor_And_Input_String_Should_Return_Specified_String(ConsoleColor color, string expectedConsoleColorValue)
            {
                const string inputString = "test";

                var outputAnsiColorString = inputString.Pastel(color);

                Assert.Equal($"\x1b[{expectedConsoleColorValue}m{inputString}\x1b[0m", outputAnsiColorString);
            }

            [Theory]
            [InlineData(ConsoleColor.Black,         0,   0,   0)]
            [InlineData(ConsoleColor.DarkRed,     139,   0,   0)]
            [InlineData(ConsoleColor.DarkGreen,     0, 100,   0)]
            [InlineData(ConsoleColor.DarkYellow,  128, 128,   0)]
            [InlineData(ConsoleColor.DarkBlue,      0,   0, 139)]
            [InlineData(ConsoleColor.DarkMagenta, 139,   0, 139)]
            [InlineData(ConsoleColor.DarkCyan,      0, 139, 139)]
            [InlineData(ConsoleColor.Gray,        128, 128, 128)]
            [InlineData(ConsoleColor.DarkGray,    169, 169, 169)]
            [InlineData(ConsoleColor.Red,         255,   0,   0)]
            [InlineData(ConsoleColor.Green,         0, 128,   0)]
            [InlineData(ConsoleColor.Yellow,      255, 255,   0)]
            [InlineData(ConsoleColor.Blue,          0,   0, 255)]
            [InlineData(ConsoleColor.Magenta,     255,   0, 255)]
            [InlineData(ConsoleColor.Cyan,          0, 255, 255)]
            [InlineData(ConsoleColor.White,       255, 255, 255)]
            public void Given_Specified_ConsoleColorLegacy_And_Input_String_Should_Return_Specified_String(ConsoleColor color, int expectedRValue, int expectedGValue, int expectedBValue)
            {
                const string inputString = "test";

                var outputAnsiColorString = inputString.Pastel(color, true);

                Assert.Equal($"\x1b[38;2;{expectedRValue};{expectedGValue};{expectedBValue}m{inputString}\x1b[0m", outputAnsiColorString);
            }

            [Theory]
            [InlineData("#0328ff",      "", "\x1b[38;2;3;40;255m\x1b[0m")]
            [InlineData("#010101", "input", "\x1b[38;2;1;1;1minput\x1b[0m")]
            [InlineData("#DDDDDD", "input", "\x1b[38;2;221;221;221minput\x1b[0m")]
            [InlineData("#dDdDdD", "input", "\x1b[38;2;221;221;221minput\x1b[0m")]
            [InlineData("#C2985D", "input", "\x1b[38;2;194;152;93minput\x1b[0m")]
            [InlineData("#aaaaaa", "input", "\x1b[38;2;170;170;170minput\x1b[0m")]
            public void Given_Specified_Hex_Color_String_And_Input_String_Should_Return_Specified_String(string hexColor, string inputString, string expectedAnsiColorString)
            {
                var outputAnsiColorString = inputString.Pastel(hexColor);

                Assert.Equal(expectedAnsiColorString, outputAnsiColorString);
            }


            [Theory]
            [InlineData("0328ff",      "", "\x1b[38;2;3;40;255m\x1b[0m")]
            [InlineData("010101", "input", "\x1b[38;2;1;1;1minput\x1b[0m")]
            [InlineData("DDDDDD", "input", "\x1b[38;2;221;221;221minput\x1b[0m")]
            [InlineData("dDdDdD", "input", "\x1b[38;2;221;221;221minput\x1b[0m")]
            [InlineData("C2985D", "input", "\x1b[38;2;194;152;93minput\x1b[0m")]
            [InlineData("aaaaaa", "input", "\x1b[38;2;170;170;170minput\x1b[0m")]
            public void Given_Specified_Hex_Color_String_Without_A_Leading_Number_Sign_And_Input_String_Should_Return_Specified_String(string hexColor, string inputString, string expectedAnsiColorString)
            {
                var outputAnsiColorString = inputString.Pastel(hexColor);

                Assert.Equal(expectedAnsiColorString, outputAnsiColorString);
            }

            [Fact]
            public void A_Given_Hex_Color_String_Should_Return_Same_Ansi_Output_String_Irrespective_Of_Being_Preceded_By_A_Number_Sign()
            {
                const string inputString = "input";
                const string hexColor    = "010101";
                var hexColorWithLeadingNumberSign = $"#{hexColor}";

                Assert.NotEqual(hexColor, hexColorWithLeadingNumberSign);
                Assert.Equal(hexColor, hexColorWithLeadingNumberSign.Substring(1));

                var outputAnsiColorString1 = inputString.Pastel(hexColor);
                var outputAnsiColorString2 = inputString.Pastel(hexColorWithLeadingNumberSign);

                Assert.Equal(outputAnsiColorString1, outputAnsiColorString2);
            }

            [Theory]
            [InlineData("ababab")]
            [InlineData("ABaBaB")]
            [InlineData("aBaBaB")]
            [InlineData("aBaBAB")]
            [InlineData("ABAbab")]
            [InlineData("abaBAB")]
            [InlineData("ABABAB")]
            public void A_Given_Hex_Color_String_Should_Return_Same_Ansi_Output_String_Irrespective_Of_Case(string hexColor)
            {
                const string inputString = "input";
                const string expectedAnsiColorString = "\x1b[38;2;171;171;171minput\x1b[0m";


                var outputAnsiColorString = inputString.Pastel(hexColor);


                Assert.Equal(expectedAnsiColorString, outputAnsiColorString);
            }


        }

        public class BackgroundColor
        {
            [Theory]
            [InlineData(3,  40, 255, "",      "\x1b[48;2;3;40;255m\x1b[0m")]
            [InlineData(1,   1,   1, "input", "\x1b[48;2;1;1;1minput\x1b[0m")]
            [InlineData(44, 70, 125, "input", "\x1b[48;2;44;70;125minput\x1b[0m")]
            public void Given_Specified_RGB_Color_And_Input_String_Should_Return_Specified_String(int red, int green, int blue, string inputString, string expectedAnsiColorString)
            {
                var outputAnsiColorString = inputString.PastelBg(Color.FromArgb(red, green, blue));

                Assert.Equal(expectedAnsiColorString, outputAnsiColorString);
            }

            [Theory]
            [InlineData(ConsoleColor.Black,        "40")]
            [InlineData(ConsoleColor.DarkRed,      "41")]
            [InlineData(ConsoleColor.DarkGreen,    "42")]
            [InlineData(ConsoleColor.DarkYellow,   "43")]
            [InlineData(ConsoleColor.DarkBlue,     "44")]
            [InlineData(ConsoleColor.DarkMagenta,  "45")]
            [InlineData(ConsoleColor.DarkCyan,     "46")]
            [InlineData(ConsoleColor.Gray,         "47")]
            [InlineData(ConsoleColor.DarkGray,    "100")]
            [InlineData(ConsoleColor.Red,         "101")]
            [InlineData(ConsoleColor.Green,       "102")]
            [InlineData(ConsoleColor.Yellow,      "103")]
            [InlineData(ConsoleColor.Blue,        "104")]
            [InlineData(ConsoleColor.Magenta,     "105")]
            [InlineData(ConsoleColor.Cyan,        "106")]
            [InlineData(ConsoleColor.White,       "107")]
            public void Given_Specified_ConsoleColor_And_Input_String_Should_Return_Specified_String(ConsoleColor color, string expectedConsoleColorValue)
            {
                const string inputString = "test";

                var outputAnsiColorString = inputString.PastelBg(color);

                Assert.Equal($"\x1b[{expectedConsoleColorValue}m{inputString}\x1b[0m", outputAnsiColorString);
            }

            [Theory]
            [InlineData(ConsoleColor.Black,         0,   0,   0)]
            [InlineData(ConsoleColor.DarkRed,     139,   0,   0)]
            [InlineData(ConsoleColor.DarkGreen,     0, 100,   0)]
            [InlineData(ConsoleColor.DarkYellow,  128, 128,   0)]
            [InlineData(ConsoleColor.DarkBlue,      0,   0, 139)]
            [InlineData(ConsoleColor.DarkMagenta, 139,   0, 139)]
            [InlineData(ConsoleColor.DarkCyan,      0, 139, 139)]
            [InlineData(ConsoleColor.Gray,        128, 128, 128)]
            [InlineData(ConsoleColor.DarkGray,    169, 169, 169)]
            [InlineData(ConsoleColor.Red,         255,   0,   0)]
            [InlineData(ConsoleColor.Green,         0, 128,   0)]
            [InlineData(ConsoleColor.Yellow,      255, 255,   0)]
            [InlineData(ConsoleColor.Blue,          0,   0, 255)]
            [InlineData(ConsoleColor.Magenta,     255,   0, 255)]
            [InlineData(ConsoleColor.Cyan,          0, 255, 255)]
            [InlineData(ConsoleColor.White,       255, 255, 255)]
            public void Given_Specified_ConsoleColorLegacy_And_Input_String_Should_Return_Specified_String(ConsoleColor color, int expectedRValue, int expectedGValue, int expectedBValue)
            {
                const string inputString = "test";

                var outputAnsiColorString = inputString.PastelBg(color, true);

                Assert.Equal($"\x1b[48;2;{expectedRValue};{expectedGValue};{expectedBValue}m{inputString}\x1b[0m", outputAnsiColorString);
            }

            [Theory]
            [InlineData("#0328ff",      "", "\x1b[48;2;3;40;255m\x1b[0m")]
            [InlineData("#010101", "input", "\x1b[48;2;1;1;1minput\x1b[0m")]
            [InlineData("#DDDDDD", "input", "\x1b[48;2;221;221;221minput\x1b[0m")]
            [InlineData("#dDdDdD", "input", "\x1b[48;2;221;221;221minput\x1b[0m")]
            [InlineData("#C2985D", "input", "\x1b[48;2;194;152;93minput\x1b[0m")]
            [InlineData("#aaaaaa", "input", "\x1b[48;2;170;170;170minput\x1b[0m")]
            public void Given_Specified_Hex_Color_String_And_Input_String_Should_Return_Specified_String(string hexColor, string inputString, string expectedAnsiColorString)
            {
                var outputAnsiColorString = inputString.PastelBg(hexColor);

                Assert.Equal(expectedAnsiColorString, outputAnsiColorString);
            }

            [Theory]
            [InlineData("0328ff",      "", "\x1b[48;2;3;40;255m\x1b[0m")]
            [InlineData("010101", "input", "\x1b[48;2;1;1;1minput\x1b[0m")]
            [InlineData("DDDDDD", "input", "\x1b[48;2;221;221;221minput\x1b[0m")]
            [InlineData("dDdDdD", "input", "\x1b[48;2;221;221;221minput\x1b[0m")]
            [InlineData("C2985D", "input", "\x1b[48;2;194;152;93minput\x1b[0m")]
            [InlineData("aaaaaa", "input", "\x1b[48;2;170;170;170minput\x1b[0m")]
            public void Given_Specified_Hex_Color_String_Without_A_Leading_Number_Sign_And_Input_String_Should_Return_Specified_String(string hexColor, string inputString, string expectedAnsiColorString)
            {
                var outputAnsiColorString = inputString.PastelBg(hexColor);

                Assert.Equal(expectedAnsiColorString, outputAnsiColorString);
            }

            [Fact]
            public void A_Given_Hex_Color_String_Should_Return_Same_Ansi_Output_String_Irrespective_Of_Being_Preceded_By_A_Number_Sign()
            {
                const string inputString = "input";
                const string hexColor    = "010101";

                var outputAnsiColorString1 = inputString.PastelBg(hexColor);
                var outputAnsiColorString2 = inputString.PastelBg($"#{hexColor}");

                Assert.Equal(outputAnsiColorString1, outputAnsiColorString2);
            }
        }

        public class NestedColor
        {
            [Fact]
            public void A_Nested_Color_String_That_Ends_With_Reset_Should_Not_Add_An_Extra_Reset_Escape_Sequence_1()
            {
                const int red1 = 1, green1 = 1, blue1 = 1;
                const int red2 = 2, green2 = 2, blue2 = 2;

                var output = $"START{"b".Pastel(Color.FromArgb(red2, green2, blue2))}".Pastel(Color.FromArgb(red1, green1, blue1));

                Assert.Equal("\x1b[38;2;1;1;1mSTART\x1b[38;2;2;2;2mb\x1b[0m", output);
            }

            [Fact]
            public void A_Nested_Color_String_That_Ends_With_Reset_Should_Not_Add_An_Extra_Reset_Escape_Sequence_2()
            {
                const int red1 = 1, green1 = 1, blue1 = 1;
                const int red2 = 2, green2 = 2, blue2 = 2;

                var output = $"START{"b".PastelBg(Color.FromArgb(red2, green2, blue2))}".PastelBg(Color.FromArgb(red1, green1, blue1));

                Assert.Equal("\x1b[48;2;1;1;1mSTART\x1b[48;2;2;2;2mb\x1b[0m", output);
            }

            [Fact]
            public void A_Nested_Color_String_That_Ends_With_Reset_Should_Not_Add_An_Extra_Reset_Escape_Sequence_3()
            {
                const int red1 = 1, green1 = 1, blue1 = 1;
                const int red2 = 2, green2 = 2, blue2 = 2;

                var output = $"START{"b".Pastel(Color.FromArgb(red2, green2, blue2))}".PastelBg(Color.FromArgb(red1, green1, blue1));

                Assert.Equal("\x1b[48;2;1;1;1mSTART\x1b[38;2;2;2;2mb\x1b[0m", output);
            }

            [Fact]
            public void A_Nested_Color_String_That_Ends_With_Reset_Should_Not_Add_An_Extra_Reset_Escape_Sequence_4()
            {
                const int red1 = 1, green1 = 1, blue1 = 1;
                const int red2 = 2, green2 = 2, blue2 = 2;

                var output = $"START{"b".PastelBg(Color.FromArgb(red2, green2, blue2))}".Pastel(Color.FromArgb(red1, green1, blue1));

                Assert.Equal("\x1b[38;2;1;1;1mSTART\x1b[48;2;2;2;2mb\x1b[0m", output);
            }

            [Fact]
            public void A_Nested_Color_String_Must_Be_Correctly_Closed_1()
            {
                const int red1 = 1, green1 = 1, blue1 = 1;
                const int red2 = 2, green2 = 2, blue2 = 2;

                var output = $"a{"b".Pastel(Color.FromArgb(red2, green2, blue2))}c".Pastel(Color.FromArgb(red1, green1, blue1));

                Assert.Equal("\x1b[38;2;1;1;1ma\x1b[38;2;2;2;2mb\x1b[0m\x1b[38;2;1;1;1mc\x1b[0m", output);
            }

            [Fact]
            public void A_Nested_Color_String_Must_Be_Correctly_Closed_2()
            {
                const int red1 = 1, green1 = 1, blue1 = 1;
                const int red2 = 2, green2 = 2, blue2 = 2;

                var output = $"a{"b".PastelBg(Color.FromArgb(red2, green2, blue2))}c".PastelBg(Color.FromArgb(red1, green1, blue1));

                Assert.Equal("\x1b[48;2;1;1;1ma\x1b[48;2;2;2;2mb\x1b[0m\x1b[48;2;1;1;1mc\x1b[0m", output);
            }

            [Fact]
            public void A_Nested_Color_String_Must_Be_Correctly_Closed_3()
            {
                const int red1 = 1, green1 = 1, blue1 = 1;
                const int red2 = 2, green2 = 2, blue2 = 2;

                var output = $"a{"b".Pastel(Color.FromArgb(red2, green2, blue2))}c".PastelBg(Color.FromArgb(red1, green1, blue1));

                Assert.Equal("\x1b[48;2;1;1;1ma\x1b[38;2;2;2;2mb\x1b[0m\x1b[48;2;1;1;1mc\x1b[0m", output);
            }

            [Fact]
            public void A_Nested_Color_String_Must_Be_Correctly_Closed_4()
            {
                const int red1 = 1, green1 = 1, blue1 = 1;
                const int red2 = 2, green2 = 2, blue2 = 2;

                var output = $"a{"b".PastelBg(Color.FromArgb(red2, green2, blue2))}c".Pastel(Color.FromArgb(red1, green1, blue1));

                Assert.Equal("\x1b[38;2;1;1;1ma\x1b[48;2;2;2;2mb\x1b[0m\x1b[38;2;1;1;1mc\x1b[0m", output);
            }

            [Fact]
            public void A_Foreground_And_Background_Nested_Color_String_Must_Be_Correctly_Closed_1()
            {
                var outputAnsiColorString = $"{$"START_{"[TEST1]".Pastel(Color.Yellow).PastelBg(Color.Crimson)}____{"[TEST2]".Pastel(Color.Yellow).PastelBg(Color.Crimson)}_END".Pastel(Color.DeepPink)}";

                Assert.Equal("\x1b[38;2;255;20;147mSTART_\x1b[48;2;220;20;60m\x1b[38;2;255;255;0m[TEST1]\x1b[0m\x1b[38;2;255;20;147m____\x1b[48;2;220;20;60m\x1b[38;2;255;255;0m[TEST2]\x1b[0m\x1b[38;2;255;20;147m_END\x1b[0m", outputAnsiColorString);
            }

            [Fact]
            public void A_Foreground_And_Background_Nested_Color_String_Must_Be_Correctly_Closed_2()
            {
                var outputAnsiColorString = $"{"Hello ".Pastel(Color.Yellow)}{"World".Pastel(Color.Red)}".PastelBg(Color.Blue);

                Assert.Equal("\x1b[48;2;0;0;255m\x1b[38;2;255;255;0mHello \x1b[0m\x1b[48;2;0;0;255m\x1b[38;2;255;0;0mWorld\x1b[0m", outputAnsiColorString);     // FAULTY
            }

            [InlineData("\x1b[0m")]
            [InlineData("\x1b[0m\x1b[0m")]
            [InlineData("\x1b[0m\x1b[0m\x1b[0m")]
            [Theory]
            public void A_Foreground_And_Background_Nested_Color_String_Containing_Valid_Reset_Escape_Sequences_Must_Be_Correctly_Closed(string validEscapeSequence)
            {
                var outputAnsiColorString = $"{$"START_{"[TEST1]".Pastel(Color.Yellow).PastelBg(Color.Crimson)}____{"[TEST2]".Pastel(Color.Yellow).PastelBg(Color.Crimson)}{validEscapeSequence}_END".Pastel(Color.DeepPink)}";

                Assert.Equal($"\x1b[38;2;255;20;147mSTART_\x1b[48;2;220;20;60m\x1b[38;2;255;255;0m[TEST1]\x1b[0m\x1b[38;2;255;20;147m____\x1b[48;2;220;20;60m\x1b[38;2;255;255;0m[TEST2]\x1b[0m{validEscapeSequence}\x1b[38;2;255;20;147m_END\x1b[0m", outputAnsiColorString);
            }

            [InlineData("\x1b[38")]
            [InlineData("\x1b[48")]
            [Theory]
            public void A_Foreground_And_Background_Nested_Color_String_Containing_Valid_Color_Escape_Sequences_Must_Be_Correctly_Closed(string validEscapeSequence)
            {
                var outputAnsiColorString = $"{$"START_{"[TEST1]".Pastel(Color.Yellow).PastelBg(Color.Crimson)}____{"[TEST2]".Pastel(Color.Yellow).PastelBg(Color.Crimson)}{validEscapeSequence}_END".Pastel(Color.DeepPink)}";

                Assert.Equal($"\x1b[38;2;255;20;147mSTART_\x1b[48;2;220;20;60m\x1b[38;2;255;255;0m[TEST1]\x1b[0m\x1b[38;2;255;20;147m____\x1b[48;2;220;20;60m\x1b[38;2;255;255;0m[TEST2]\x1b[0m\x1b[38;2;255;20;147m{validEscapeSequence}_END\x1b[0m", outputAnsiColorString);
            }

            [InlineData("\x1b")]
            [InlineData("\x1b[")]

            [InlineData("x[38")]
            [InlineData("\x1bx38")]
            [InlineData("\x1b[x8")]
            [InlineData("\x1b[3x")]

            [InlineData("x[48")]
            [InlineData("\x1bx48")]
            [InlineData("\x1b[4x")]

            [InlineData("x[0m")]
            [InlineData("\x1bx0m")]
            [InlineData("\x1b[xm")]
            [InlineData("\x1b[0x")]
            [Theory]
            public void A_Foreground_And_Background_Nested_Color_String_Containing_Invalid_Escape_Sequences_Must_Be_Correctly_Closed(string invalidEscapeSequence)
            {
                var outputAnsiColorString = $"{$"START_{"[TEST1]".Pastel(Color.Yellow).PastelBg(Color.Crimson)}____{"[TEST2]".Pastel(Color.Yellow).PastelBg(Color.Crimson)}{invalidEscapeSequence}_END".Pastel(Color.DeepPink)}";

                Assert.Equal($"\x1b[38;2;255;20;147mSTART_\x1b[48;2;220;20;60m\x1b[38;2;255;255;0m[TEST1]\x1b[0m\x1b[38;2;255;20;147m____\x1b[48;2;220;20;60m\x1b[38;2;255;255;0m[TEST2]\x1b[0m\x1b[38;2;255;20;147m{invalidEscapeSequence}_END\x1b[0m", outputAnsiColorString);
            }

            [Fact]
            public void A_String_Containing_Console_Escape_Sequences_Should_Be_Correctly_Nested_When_Applying_ConsoleColor()
            {
                var outputAnsiColorString = $"START_{"[TEST1]".Pastel(Color.Yellow)}_END".Pastel(ConsoleColor.Magenta).PastelBg(ConsoleColor.DarkRed);

                Assert.Equal("\x1b[41m\x1b[95mSTART_\x1b[38;2;255;255;0m[TEST1]\x1b[0m\x1b[41m\x1b[95m_END\x1b[0m", outputAnsiColorString);
            }

            [Fact]
            public void A_String_Containing_ConsoleColor_Escape_Sequences_Should_Be_Correctly_Nested_When_Applying_ConsoleColor()
            {
                var outputAnsiColorString = $"START_{"[TEST1]".Pastel(ConsoleColor.Yellow)}_END".Pastel(ConsoleColor.Magenta).PastelBg(ConsoleColor.DarkRed);

                Assert.Equal("\x1b[41m\x1b[95mSTART_\x1b[93m[TEST1]\x1b[0m\x1b[41m\x1b[95m_END\x1b[0m", outputAnsiColorString);
            }
        }

        public class NoOutputColor
        {
            private const string _input = "input";

            private static void ColorOutputEnabledTest(string expectedAnsiColorCodePart, string outputAnsiColorStringForeground, string outputAnsiColorStringBackground)
            {
                Assert.Equal($"\x1b[38;2;{expectedAnsiColorCodePart}m{_input}\x1b[0m", outputAnsiColorStringForeground);
                Assert.Equal($"\x1b[48;2;{expectedAnsiColorCodePart}m{_input}\x1b[0m", outputAnsiColorStringBackground);
            }

            private static void ColorOutputEnabledTestConsoleColor(string expectedConsoleColorValue, string outputAnsiColorString)
            {
                Assert.Equal($"\x1b[{expectedConsoleColorValue}m{_input}\x1b[0m", outputAnsiColorString);
            }

            private static void ColorOutputEnabledTestColor(Color color, string expectedAnsiColorCodePart)
            {
                var outputAnsiColorString1 = _input.Pastel(  color);
                var outputAnsiColorString2 = _input.PastelBg(color);

                ColorOutputEnabledTest(expectedAnsiColorCodePart, outputAnsiColorString1, outputAnsiColorString2);
            }

            private static void ColorOutputEnabledTestConsoleColorLegacy(ConsoleColor color, string expectedAnsiColorCodePart)
            {
                var outputAnsiColorString1 = _input.Pastel(  color, true);
                var outputAnsiColorString2 = _input.PastelBg(color, true);

                ColorOutputEnabledTest(expectedAnsiColorCodePart, outputAnsiColorString1, outputAnsiColorString2);
            }

            private static void ColorOutputEnabledTestConsoleColorForeground(ConsoleColor color, string expectedConsoleColorValue)
            {
                var outputAnsiColorString = _input.Pastel(color);

                ColorOutputEnabledTestConsoleColor(expectedConsoleColorValue, outputAnsiColorString);
            }

            private static void ColorOutputEnabledTestConsoleColorBackground(ConsoleColor color, string expectedConsoleColorValue)
            {
                var outputAnsiColorString = _input.PastelBg(color);

                ColorOutputEnabledTestConsoleColor(expectedConsoleColorValue, outputAnsiColorString);
            }

            private static void ColorOutputEnabledTestHexColor(string color, string expectedAnsiColorCodePart)
            {
                var outputAnsiColorString1 = _input.Pastel(  color);
                var outputAnsiColorString2 = _input.PastelBg(color);

                ColorOutputEnabledTest(expectedAnsiColorCodePart, outputAnsiColorString1, outputAnsiColorString2);
            }

            private static void ColorOutputDisabledTest(string outputAnsiColorString1, string outputAnsiColorString2)
            {
                Assert.Equal(_input, outputAnsiColorString1);
                Assert.Equal(_input, outputAnsiColorString2);
            }

            private static void ColorOutputDisabledTestConsoleColor(string outputAnsiColorString)
            {
                Assert.Equal(_input, outputAnsiColorString);
            }

            private static void ColorOutputDisabledTestColor(Color color)
            {
                var outputAnsiColorString1 = _input.Pastel(  color);
                var outputAnsiColorString2 = _input.PastelBg(color);

                ColorOutputDisabledTest(outputAnsiColorString1, outputAnsiColorString2);
            }

            private static void ColorOutputDisabledTestConsoleColorLegacy(ConsoleColor color)
            {
                var outputAnsiColorString1 = _input.Pastel(  color, true);
                var outputAnsiColorString2 = _input.PastelBg(color, true);

                ColorOutputDisabledTest(outputAnsiColorString1, outputAnsiColorString2);
            }

            private static void ColorOutputDisabledTestConsoleColor(ConsoleColor color)
            {
                var outputAnsiColorString1 = _input.Pastel(  color);
                var outputAnsiColorString2 = _input.PastelBg(color);

                ColorOutputDisabledTest(outputAnsiColorString1, outputAnsiColorString2);
            }

            private static void ColorOutputDisabledTestHexColor(string color)
            {
                var outputAnsiColorString1 = _input.Pastel(  color);
                var outputAnsiColorString2 = _input.PastelBg(color);

                ColorOutputDisabledTest(outputAnsiColorString1, outputAnsiColorString2);
            }

            [Fact]
            public void Output_Should_Honor_Current_State_When_Switching_Between_States()
            {
                var color = Color.FromArgb(1, 1, 1);
                const string hexColor = "#010101";
                const string expectedAnsiColorCodePart = "1;1;1";

                // Enable color output

                ConsoleExtensions.Enable();
                ColorOutputEnabledTestColor(color, expectedAnsiColorCodePart);
                ColorOutputEnabledTestHexColor(hexColor, expectedAnsiColorCodePart);


                // Disable color output

                ConsoleExtensions.Disable();
                ColorOutputDisabledTestColor(color);
                ColorOutputDisabledTestHexColor(hexColor);


                // Re-enable color output

                ConsoleExtensions.Enable();
                ColorOutputEnabledTestColor(color, expectedAnsiColorCodePart);
                ColorOutputEnabledTestHexColor(hexColor, expectedAnsiColorCodePart);
            }

            [Theory]
            [InlineData(ConsoleColor.Black,       "0;0;0")]
            [InlineData(ConsoleColor.DarkBlue,    "0;0;139")]
            [InlineData(ConsoleColor.DarkGreen,   "0;100;0")]
            [InlineData(ConsoleColor.DarkCyan,    "0;139;139")]
            [InlineData(ConsoleColor.DarkRed,     "139;0;0")]
            [InlineData(ConsoleColor.DarkMagenta, "139;0;139")]
            [InlineData(ConsoleColor.DarkYellow,  "128;128;0")]
            [InlineData(ConsoleColor.Gray,        "128;128;128")]
            [InlineData(ConsoleColor.DarkGray,    "169;169;169")]
            [InlineData(ConsoleColor.Blue,        "0;0;255")]
            [InlineData(ConsoleColor.Green,       "0;128;0")]
            [InlineData(ConsoleColor.Cyan,        "0;255;255")]
            [InlineData(ConsoleColor.Red,         "255;0;0")]
            [InlineData(ConsoleColor.Magenta,     "255;0;255")]
            [InlineData(ConsoleColor.Yellow,      "255;255;0")]
            [InlineData(ConsoleColor.White,       "255;255;255")]
            public void Output_Should_Honor_Current_State_When_Switching_Between_States_ConsoleColorLegacy(ConsoleColor color, string expectedAnsiColorCodePart)
            {
                // Enable color output

                ConsoleExtensions.Enable();
                ColorOutputEnabledTestConsoleColorLegacy(color, expectedAnsiColorCodePart);


                // Disable color output

                ConsoleExtensions.Disable();
                ColorOutputDisabledTestConsoleColorLegacy(color);


                // Re-enable color output

                ConsoleExtensions.Enable();
                ColorOutputEnabledTestConsoleColorLegacy(color, expectedAnsiColorCodePart);
            }

            [Theory]
            [InlineData(ConsoleColor.Black,       "30")]
            [InlineData(ConsoleColor.DarkRed,     "31")]
            [InlineData(ConsoleColor.DarkGreen,   "32")]
            [InlineData(ConsoleColor.DarkYellow,  "33")]
            [InlineData(ConsoleColor.DarkBlue,    "34")]
            [InlineData(ConsoleColor.DarkMagenta, "35")]
            [InlineData(ConsoleColor.DarkCyan,    "36")]
            [InlineData(ConsoleColor.Gray,        "37")]
            [InlineData(ConsoleColor.DarkGray,    "90")]
            [InlineData(ConsoleColor.Red,         "91")]
            [InlineData(ConsoleColor.Green,       "92")]
            [InlineData(ConsoleColor.Yellow,      "93")]
            [InlineData(ConsoleColor.Blue,        "94")]
            [InlineData(ConsoleColor.Magenta,     "95")]
            [InlineData(ConsoleColor.Cyan,        "96")]
            [InlineData(ConsoleColor.White,       "97")]
            public void Output_Should_Honor_Current_State_When_Switching_Between_States_ConsoleColorForeground(ConsoleColor color, string expectedConsoleColorValue)
            {
                // Enable color output

                ConsoleExtensions.Enable();
                ColorOutputEnabledTestConsoleColorForeground(color, expectedConsoleColorValue);


                // Disable color output

                ConsoleExtensions.Disable();
                ColorOutputDisabledTestConsoleColor(color);


                // Re-enable color output

                ConsoleExtensions.Enable();
                ColorOutputEnabledTestConsoleColorForeground(color, expectedConsoleColorValue);
            }

            [Theory]
            [InlineData(ConsoleColor.Black,        "40")]
            [InlineData(ConsoleColor.DarkRed,      "41")]
            [InlineData(ConsoleColor.DarkGreen,    "42")]
            [InlineData(ConsoleColor.DarkYellow,   "43")]
            [InlineData(ConsoleColor.DarkBlue,     "44")]
            [InlineData(ConsoleColor.DarkMagenta,  "45")]
            [InlineData(ConsoleColor.DarkCyan,     "46")]
            [InlineData(ConsoleColor.Gray,         "47")]
            [InlineData(ConsoleColor.DarkGray,    "100")]
            [InlineData(ConsoleColor.Red,         "101")]
            [InlineData(ConsoleColor.Green,       "102")]
            [InlineData(ConsoleColor.Yellow,      "103")]
            [InlineData(ConsoleColor.Blue,        "104")]
            [InlineData(ConsoleColor.Magenta,     "105")]
            [InlineData(ConsoleColor.Cyan,        "106")]
            [InlineData(ConsoleColor.White,       "107")]
            public void Output_Should_Honor_Current_State_When_Switching_Between_States_ConsoleColorBackground(ConsoleColor color, string expectedConsoleColorValue)
            {
                // Enable color output

                ConsoleExtensions.Enable();
                ColorOutputEnabledTestConsoleColorBackground(color, expectedConsoleColorValue);


                // Disable color output

                ConsoleExtensions.Disable();
                ColorOutputDisabledTestConsoleColor(color);


                // Re-enable color output

                ConsoleExtensions.Enable();
                ColorOutputEnabledTestConsoleColorBackground(color, expectedConsoleColorValue);
            }
        }
    }
}
