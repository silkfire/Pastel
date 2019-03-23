namespace Pastel.Tests
{
    using Xunit;

    using System.Drawing;


    public class ColorTests
    {
        public class ForegroundColor
        {
            [Theory]
            [InlineData(1,   1,   1, "input", "\u001b[38;2;1;1;1minput\u001b[0m")]
            [InlineData(44, 70, 125, "input", "\u001b[38;2;44;70;125minput\u001b[0m")]
            public void Given_Specified_RGB_Color_And_Input_String_Should_Return_Specified_String(int red, int green, int blue, string inputString, string expectedAnsiColorString)
            {
                /////////////////
                // ACT
                /////////

                var outputAnsiColorString = inputString.Pastel(Color.FromArgb(red, green, blue));


                /////////////////
                // ASSERT
                /////////

                Assert.Equal(expectedAnsiColorString, outputAnsiColorString);
            }


            [Theory]
            [InlineData("#010101", "input", "\u001b[38;2;1;1;1minput\u001b[0m")]
            [InlineData("#DDDDDD", "input", "\u001b[38;2;221;221;221minput\u001b[0m")]
            [InlineData("#dDdDdD", "input", "\u001b[38;2;221;221;221minput\u001b[0m")]
            [InlineData("#C2985D", "input", "\u001b[38;2;194;152;93minput\u001b[0m")]
            [InlineData("#aaaaaa", "input", "\u001b[38;2;170;170;170minput\u001b[0m")]
            public void Given_Specified_Hex_Color_String_And_Input_String_Should_Return_Specified_String(string hexColor, string inputString, string expectedAnsiColorString)
            {
                /////////////////
                // ACT
                /////////

                var outputAnsiColorString = inputString.Pastel(hexColor);


                /////////////////
                // ASSERT
                /////////

                Assert.Equal(expectedAnsiColorString, outputAnsiColorString);
            }


            [Theory]
            [InlineData("010101", "input", "\u001b[38;2;1;1;1minput\u001b[0m")]
            [InlineData("DDDDDD", "input", "\u001b[38;2;221;221;221minput\u001b[0m")]
            [InlineData("dDdDdD", "input", "\u001b[38;2;221;221;221minput\u001b[0m")]
            [InlineData("C2985D", "input", "\u001b[38;2;194;152;93minput\u001b[0m")]
            [InlineData("aaaaaa", "input", "\u001b[38;2;170;170;170minput\u001b[0m")]
            public void Given_Specified_Hex_Color_String_Without_Hash_Sign_And_Input_String_Should_Return_Specified_String(string hexColor, string inputString, string expectedAnsiColorString)
            {
                /////////////////
                // ACT
                /////////

                var outputAnsiColorString = inputString.Pastel(hexColor);


                /////////////////
                // ASSERT
                /////////

                Assert.Equal(expectedAnsiColorString, outputAnsiColorString);
            }



            [Fact]
            public void A_Given_Hex_Color_String_Should_Return_Same_Ansi_Output_String_Regardless_Whether_Prefixed_By_Hash_Sign()
            {
                /////////////////
                // ARRANGE
                /////////

                const string inputString = "input";
                const string hexColor    = "010101";


                /////////////////
                // ACT
                /////////

                var outputAnsiColorString1 = inputString.Pastel(hexColor);
                var outputAnsiColorString2 = inputString.Pastel($"#{hexColor}");


                /////////////////
                // ASSERT
                /////////

                Assert.Equal(outputAnsiColorString1, outputAnsiColorString2);
            }
        }



        public class BackgroundColor
        {
            [Theory]
            [InlineData(1,   1,   1, "input", "\u001b[48;2;1;1;1minput\u001b[0m")]
            [InlineData(44, 70, 125, "input", "\u001b[48;2;44;70;125minput\u001b[0m")]
            public void Given_Specified_RGB_Color_And_Input_String_Should_Return_Specified_String(int red, int green, int blue, string inputString, string expectedAnsiColorString)
            {
                /////////////////
                // ACT
                /////////

                var outputAnsiColorString = inputString.PastelBg(Color.FromArgb(red, green, blue));


                /////////////////
                // ASSERT
                /////////

                Assert.Equal(expectedAnsiColorString, outputAnsiColorString);
            }


            [Theory]
            [InlineData("#010101", "input", "\u001b[48;2;1;1;1minput\u001b[0m")]
            [InlineData("#DDDDDD", "input", "\u001b[48;2;221;221;221minput\u001b[0m")]
            [InlineData("#dDdDdD", "input", "\u001b[48;2;221;221;221minput\u001b[0m")]
            [InlineData("#C2985D", "input", "\u001b[48;2;194;152;93minput\u001b[0m")]
            [InlineData("#aaaaaa", "input", "\u001b[48;2;170;170;170minput\u001b[0m")]
            public void Given_Specified_Hex_Color_String_And_Input_String_Should_Return_Specified_String(string hexColor, string inputString, string expectedAnsiColorString)
            {
                /////////////////
                // ACT
                /////////

                var outputAnsiColorString = inputString.PastelBg(hexColor);


                /////////////////
                // ASSERT
                /////////

                Assert.Equal(expectedAnsiColorString, outputAnsiColorString);
            }


            [Theory]
            [InlineData("010101", "input", "\u001b[48;2;1;1;1minput\u001b[0m")]
            [InlineData("DDDDDD", "input", "\u001b[48;2;221;221;221minput\u001b[0m")]
            [InlineData("dDdDdD", "input", "\u001b[48;2;221;221;221minput\u001b[0m")]
            [InlineData("C2985D", "input", "\u001b[48;2;194;152;93minput\u001b[0m")]
            [InlineData("aaaaaa", "input", "\u001b[48;2;170;170;170minput\u001b[0m")]
            public void Given_Specified_Hex_Color_String_Without_Hash_Sign_And_Input_String_Should_Return_Specified_String(string hexColor, string inputString, string expectedAnsiColorString)
            {
                /////////////////
                // ACT
                /////////

                var outputAnsiColorString = inputString.PastelBg(hexColor);


                /////////////////
                // ASSERT
                /////////

                Assert.Equal(expectedAnsiColorString, outputAnsiColorString);
            }



            [Fact]
            public void A_Given_Hex_Color_String_Should_Return_Same_Ansi_Output_String_Regardless_Whether_Prefixed_By_Hash_Sign()
            {
                /////////////////
                // ARRANGE
                /////////

                const string inputString = "input";
                const string hexColor    = "010101";


                /////////////////
                // ACT
                /////////

                var outputAnsiColorString1 = inputString.PastelBg(hexColor);
                var outputAnsiColorString2 = inputString.PastelBg($"#{hexColor}");


                /////////////////
                // ASSERT
                /////////

                Assert.Equal(outputAnsiColorString1, outputAnsiColorString2);
            }
        }


        public class NoOutputColor
        {
            private const string _input = "input";


            private void ColorOutputEnabledTest()
            {
                /////////////////
                // ARRANGE
                /////////

                ConsoleExtensions.Enable();


                /////////////////
                // ACT
                /////////

                var outputAnsiColorString1 = _input.Pastel(  Color.FromArgb(1, 1, 1));
                var outputAnsiColorString2 = _input.Pastel(  "#010101");
                var outputAnsiColorString3 = _input.PastelBg(Color.FromArgb(1, 1, 1));
                var outputAnsiColorString4 = _input.PastelBg("#010101");


                /////////////////
                // ASSERT
                /////////

                Assert.Equal($"\u001b[38;2;1;1;1m{_input}\u001b[0m", outputAnsiColorString1);
                Assert.Equal($"\u001b[38;2;1;1;1m{_input}\u001b[0m", outputAnsiColorString2);
                Assert.Equal($"\u001b[48;2;1;1;1m{_input}\u001b[0m", outputAnsiColorString3);
                Assert.Equal($"\u001b[48;2;1;1;1m{_input}\u001b[0m", outputAnsiColorString4);
            }


            [Fact]
            public void Output_Should_Honor_Current_State_When_Switching_Between_States()
            {
                // Enable color output

                ColorOutputEnabledTest();


                // Disable color output

                /////////////////
                // ARRANGE
                /////////

                ConsoleExtensions.Disable();


                /////////////////
                // ACT
                /////////

                var outputAnsiColorString1 = _input.Pastel(  Color.FromArgb(1, 1, 1));
                var outputAnsiColorString2 = _input.Pastel(  "#010101");
                var outputAnsiColorString3 = _input.PastelBg(Color.FromArgb(1, 1, 1));
                var outputAnsiColorString4 = _input.PastelBg("#010101");


                /////////////////
                // ASSERT
                /////////

                Assert.Equal(_input, outputAnsiColorString1);
                Assert.Equal(_input, outputAnsiColorString2);
                Assert.Equal(_input, outputAnsiColorString3);
                Assert.Equal(_input, outputAnsiColorString4);


                // Re-enable color output

                ColorOutputEnabledTest();
            }
        }

        [Fact]
        public void NestedColors()
        {
            int red1 = 1, green1 = 1, blue1 = 1;
            int red2 = 2, green2 = 2, blue2 = 2;
            string expected = $"{"a".Pastel(Color.FromArgb(red1, green1, blue1))}{"b".Pastel(Color.FromArgb(red2, green2, blue2))}{"c".Pastel(Color.FromArgb(red1, green1, blue1))}";
            string actual = Color.FromArgb(red1, green1, blue1).Pastel($"a{"b".Pastel(Color.FromArgb(red2, green2, blue2))}c");


            Assert.Equal(expected, actual); 
        }
    }
}
