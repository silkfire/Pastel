[assembly: Xunit.CollectionBehavior(DisableTestParallelization = true)]

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
            [InlineData(3,  40, 255, "", "\x1b[38;2;3;40;255m\x1b[0m")]
            [InlineData(1,   1,   1, "input", "\x1b[38;2;1;1;1minput\x1b[0m")]
            [InlineData(44, 70, 125, "input", "\x1b[38;2;44;70;125minput\x1b[0m")]
            public void Given_Specified_RGB_Color_And_Input_String_Should_Return_Specified_String(int red, int green, int blue, string inputString, string expectedAnsiColorString)
            {
                var outputAnsiColorString = inputString.Pastel(Color.FromArgb(red, green, blue));

                Assert.Equal(expectedAnsiColorString, outputAnsiColorString);
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
            public void A_Foreground_And_Background_Nested_Color_String_Must_Be_Correctly_Closed()
            {
                var outputAnsiColorString = $"{$"START_{"[TEST1]".Pastel(Color.Yellow).PastelBg(Color.Crimson)}____{"[TEST2]".Pastel(Color.Yellow).PastelBg(Color.Crimson)}_END".Pastel(Color.DeepPink)}";

                Assert.Equal("\x1b[38;2;255;20;147mSTART_\x1b[48;2;220;20;60m\x1b[38;2;255;255;0m[TEST1]\x1b[0m\x1b[38;2;255;20;147m____\x1b[48;2;220;20;60m\x1b[38;2;255;255;0m[TEST2]\x1b[0m\x1b[38;2;255;20;147m_END\x1b[0m", outputAnsiColorString);
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

                Assert.Equal($"\x1b[38;2;255;20;147mSTART_\x1b[48;2;220;20;60m\x1b[38;2;255;255;0m[TEST1]\x1b[0m\x1b[38;2;255;20;147m____\x1b[48;2;220;20;60m\x1b[38;2;255;255;0m[TEST2]\x1b[0m{validEscapeSequence}_END\x1b[0m", outputAnsiColorString);
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
        }


        public class NoOutputColor
        {
            private const string _input = "input";

            private static void ColorOutputEnabledTest(string expectedAnsiColorCodePart, string outputAnsiColorString1, string outputAnsiColorString2)
            {
                Assert.Equal($"\x1b[38;2;{expectedAnsiColorCodePart}m{_input}\x1b[0m", outputAnsiColorString1);
                Assert.Equal($"\x1b[48;2;{expectedAnsiColorCodePart}m{_input}\x1b[0m", outputAnsiColorString2);
            }

            private static void ColorOutputEnabledTestColor(Color color, string expectedAnsiColorCodePart)
            {
                var outputAnsiColorString1 = _input.Pastel(  color);
                var outputAnsiColorString2 = _input.PastelBg(color);

                ColorOutputEnabledTest(expectedAnsiColorCodePart, outputAnsiColorString1, outputAnsiColorString2);
            }

            private static void ColorOutputEnabledTestConsoleColor(ConsoleColor color, string expectedAnsiColorCodePart)
            {
                var outputAnsiColorString1 = _input.Pastel(  color);
                var outputAnsiColorString2 = _input.PastelBg(color);

                ColorOutputEnabledTest(expectedAnsiColorCodePart, outputAnsiColorString1, outputAnsiColorString2);
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

            private static void ColorOutputDisabledTestColor(Color color)
            {
                var outputAnsiColorString1 = _input.Pastel(  color);
                var outputAnsiColorString2 = _input.PastelBg(color);

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
            public void Output_Should_Honor_Current_State_When_Switching_Between_States_ConsoleColor(ConsoleColor color, string expectedAnsiColorCodePart)
            {
                // Enable color output

                ConsoleExtensions.Enable();
                ColorOutputEnabledTestConsoleColor(color, expectedAnsiColorCodePart);


                // Disable color output

                ConsoleExtensions.Disable();
                ColorOutputDisabledTestConsoleColor(color);


                // Re-enable color output

                ConsoleExtensions.Enable();
                ColorOutputEnabledTestConsoleColor(color, expectedAnsiColorCodePart);
            }
        }
    }
}
