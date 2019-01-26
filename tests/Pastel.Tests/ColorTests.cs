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
    }
}
