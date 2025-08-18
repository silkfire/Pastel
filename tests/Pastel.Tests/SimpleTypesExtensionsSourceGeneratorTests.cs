using System;
using System.Globalization;
using Xunit;

namespace Pastel.Tests
{
    public class SimpleTypesExtensionsSourceGeneratorTests
    {
        [Theory]
        [InlineData(true, ConsoleColor.Black)]
        [InlineData(true, ConsoleColor.DarkRed)]
        [InlineData(true, ConsoleColor.DarkGreen)]
        [InlineData(true, ConsoleColor.DarkYellow)]
        [InlineData(true, ConsoleColor.DarkBlue)]
        [InlineData(true, ConsoleColor.DarkMagenta)]
        [InlineData(true, ConsoleColor.DarkCyan)]
        [InlineData(true, ConsoleColor.Gray)]
        [InlineData(true, ConsoleColor.DarkGray)]
        [InlineData(true, ConsoleColor.Red)]
        [InlineData(true, ConsoleColor.Green)]
        [InlineData(true, ConsoleColor.Yellow)]
        [InlineData(true, ConsoleColor.Blue)]
        [InlineData(true, ConsoleColor.Magenta)]
        [InlineData(true, ConsoleColor.Cyan)]
        [InlineData(true, ConsoleColor.White)]
        public void BoolToStringTest(bool value, ConsoleColor consoleColor)
        {
            Assert.Equal(value.ToString().Pastel(consoleColor), value.Pastel(consoleColor));
            Assert.Equal(value.ToString(new CultureInfo("fr-FR")).Pastel(consoleColor), value.Pastel(consoleColor, new CultureInfo("fr-FR")));
        }

        [Theory]
        [InlineData(128, ConsoleColor.Black)]
        [InlineData(128, ConsoleColor.DarkRed)]
        [InlineData(128, ConsoleColor.DarkGreen)]
        [InlineData(128, ConsoleColor.DarkYellow)]
        [InlineData(128, ConsoleColor.DarkBlue)]
        [InlineData(128, ConsoleColor.DarkMagenta)]
        [InlineData(128, ConsoleColor.DarkCyan)]
        [InlineData(128, ConsoleColor.Gray)]
        [InlineData(128, ConsoleColor.DarkGray)]
        [InlineData(128, ConsoleColor.Red)]
        [InlineData(128, ConsoleColor.Green)]
        [InlineData(128, ConsoleColor.Yellow)]
        [InlineData(128, ConsoleColor.Blue)]
        [InlineData(128, ConsoleColor.Magenta)]
        [InlineData(128, ConsoleColor.Cyan)]
        [InlineData(128, ConsoleColor.White)]
        public void ByteToStringTest(byte value, ConsoleColor consoleColor)
        {
            Assert.Equal(value.ToString().Pastel(consoleColor), value.Pastel(consoleColor));
            Assert.Equal(value.ToString("E").Pastel(consoleColor), value.Pastel(consoleColor, "E"));
            Assert.Equal(value.ToString(new CultureInfo("fr-FR")).Pastel(consoleColor), value.Pastel(consoleColor, new CultureInfo("fr-FR")));
            Assert.Equal(value.ToString("E4", new CultureInfo("en-US")).Pastel(consoleColor), value.Pastel(consoleColor, "E4", new CultureInfo("en-US")));
        }

        [Theory]
        [InlineData('A', ConsoleColor.Black)]
        [InlineData('A', ConsoleColor.DarkRed)]
        [InlineData('A', ConsoleColor.DarkGreen)]
        [InlineData('A', ConsoleColor.DarkYellow)]
        [InlineData('A', ConsoleColor.DarkBlue)]
        [InlineData('A', ConsoleColor.DarkMagenta)]
        [InlineData('A', ConsoleColor.DarkCyan)]
        [InlineData('A', ConsoleColor.Gray)]
        [InlineData('A', ConsoleColor.DarkGray)]
        [InlineData('A', ConsoleColor.Red)]
        [InlineData('A', ConsoleColor.Green)]
        [InlineData('A', ConsoleColor.Yellow)]
        [InlineData('A', ConsoleColor.Blue)]
        [InlineData('A', ConsoleColor.Magenta)]
        [InlineData('A', ConsoleColor.Cyan)]
        [InlineData('A', ConsoleColor.White)]
        public void CharToStringTest(char value, ConsoleColor consoleColor)
        {
            Assert.Equal(value.ToString().Pastel(consoleColor), value.Pastel(consoleColor));
            Assert.Equal(value.ToString(new CultureInfo("fr-FR")).Pastel(consoleColor), value.Pastel(consoleColor, new CultureInfo("fr-FR")));
        }

        [Theory]
        [InlineData(615.239182, ConsoleColor.Black)]
        [InlineData(615.239182, ConsoleColor.DarkRed)]
        [InlineData(615.239182, ConsoleColor.DarkGreen)]
        [InlineData(615.239182, ConsoleColor.DarkYellow)]
        [InlineData(615.239182, ConsoleColor.DarkBlue)]
        [InlineData(615.239182, ConsoleColor.DarkMagenta)]
        [InlineData(615.239182, ConsoleColor.DarkCyan)]
        [InlineData(615.239182, ConsoleColor.Gray)]
        [InlineData(615.239182, ConsoleColor.DarkGray)]
        [InlineData(615.239182, ConsoleColor.Red)]
        [InlineData(615.239182, ConsoleColor.Green)]
        [InlineData(615.239182, ConsoleColor.Yellow)]
        [InlineData(615.239182, ConsoleColor.Blue)]
        [InlineData(615.239182, ConsoleColor.Magenta)]
        [InlineData(615.239182, ConsoleColor.Cyan)]
        [InlineData(615.239182, ConsoleColor.White)]
        public void DecimalToStringTest(decimal value, ConsoleColor consoleColor)
        {
            Assert.Equal(value.ToString().Pastel(consoleColor), value.Pastel(consoleColor));
            Assert.Equal(value.ToString("E").Pastel(consoleColor), value.Pastel(consoleColor, "E"));
            Assert.Equal(value.ToString(new CultureInfo("fr-FR")).Pastel(consoleColor), value.Pastel(consoleColor, new CultureInfo("fr-FR")));
            Assert.Equal(value.ToString("E4", new CultureInfo("en-US")).Pastel(consoleColor), value.Pastel(consoleColor, "E4", new CultureInfo("en-US")));
        }

        [Theory]
        [InlineData(615.239182, ConsoleColor.Black)]
        [InlineData(615.239182, ConsoleColor.DarkRed)]
        [InlineData(615.239182, ConsoleColor.DarkGreen)]
        [InlineData(615.239182, ConsoleColor.DarkYellow)]
        [InlineData(615.239182, ConsoleColor.DarkBlue)]
        [InlineData(615.239182, ConsoleColor.DarkMagenta)]
        [InlineData(615.239182, ConsoleColor.DarkCyan)]
        [InlineData(615.239182, ConsoleColor.Gray)]
        [InlineData(615.239182, ConsoleColor.DarkGray)]
        [InlineData(615.239182, ConsoleColor.Red)]
        [InlineData(615.239182, ConsoleColor.Green)]
        [InlineData(615.239182, ConsoleColor.Yellow)]
        [InlineData(615.239182, ConsoleColor.Blue)]
        [InlineData(615.239182, ConsoleColor.Magenta)]
        [InlineData(615.239182, ConsoleColor.Cyan)]
        [InlineData(615.239182, ConsoleColor.White)]
        public void DoubleToStringTest(double value, ConsoleColor consoleColor)
        {
            Assert.Equal(value.ToString().Pastel(consoleColor), value.Pastel(consoleColor));
            Assert.Equal(value.ToString("E").Pastel(consoleColor), value.Pastel(consoleColor, "E"));
            Assert.Equal(value.ToString(new CultureInfo("fr-FR")).Pastel(consoleColor), value.Pastel(consoleColor, new CultureInfo("fr-FR")));
            Assert.Equal(value.ToString("E4", new CultureInfo("en-US")).Pastel(consoleColor), value.Pastel(consoleColor, "E4", new CultureInfo("en-US")));
        }

        [Theory]
        [InlineData(615.239182f, ConsoleColor.Black)]
        [InlineData(615.239182f, ConsoleColor.DarkRed)]
        [InlineData(615.239182f, ConsoleColor.DarkGreen)]
        [InlineData(615.239182f, ConsoleColor.DarkYellow)]
        [InlineData(615.239182f, ConsoleColor.DarkBlue)]
        [InlineData(615.239182f, ConsoleColor.DarkMagenta)]
        [InlineData(615.239182f, ConsoleColor.DarkCyan)]
        [InlineData(615.239182f, ConsoleColor.Gray)]
        [InlineData(615.239182f, ConsoleColor.DarkGray)]
        [InlineData(615.239182f, ConsoleColor.Red)]
        [InlineData(615.239182f, ConsoleColor.Green)]
        [InlineData(615.239182f, ConsoleColor.Yellow)]
        [InlineData(615.239182f, ConsoleColor.Blue)]
        [InlineData(615.239182f, ConsoleColor.Magenta)]
        [InlineData(615.239182f, ConsoleColor.Cyan)]
        [InlineData(615.239182f, ConsoleColor.White)]
        public void FloatToStringTest(float value, ConsoleColor consoleColor)
        {
            Assert.Equal(value.ToString().Pastel(consoleColor), value.Pastel(consoleColor));
            Assert.Equal(value.ToString("E").Pastel(consoleColor), value.Pastel(consoleColor, "E"));
            Assert.Equal(value.ToString(new CultureInfo("fr-FR")).Pastel(consoleColor), value.Pastel(consoleColor, new CultureInfo("fr-FR")));
            Assert.Equal(value.ToString("E4", new CultureInfo("en-US")).Pastel(consoleColor), value.Pastel(consoleColor, "E4", new CultureInfo("en-US")));
        }

        [Theory]
        [InlineData(615239, ConsoleColor.Black)]
        [InlineData(615239, ConsoleColor.DarkRed)]
        [InlineData(615239, ConsoleColor.DarkGreen)]
        [InlineData(615239, ConsoleColor.DarkYellow)]
        [InlineData(615239, ConsoleColor.DarkBlue)]
        [InlineData(615239, ConsoleColor.DarkMagenta)]
        [InlineData(615239, ConsoleColor.DarkCyan)]
        [InlineData(615239, ConsoleColor.Gray)]
        [InlineData(615239, ConsoleColor.DarkGray)]
        [InlineData(615239, ConsoleColor.Red)]
        [InlineData(615239, ConsoleColor.Green)]
        [InlineData(615239, ConsoleColor.Yellow)]
        [InlineData(615239, ConsoleColor.Blue)]
        [InlineData(615239, ConsoleColor.Magenta)]
        [InlineData(615239, ConsoleColor.Cyan)]
        [InlineData(615239, ConsoleColor.White)]
        public void IntToStringTest(int value, ConsoleColor consoleColor)
        {
            Assert.Equal(value.ToString().Pastel(consoleColor), value.Pastel(consoleColor));
            Assert.Equal(value.ToString("E").Pastel(consoleColor), value.Pastel(consoleColor, "E"));
            Assert.Equal(value.ToString(new CultureInfo("fr-FR")).Pastel(consoleColor), value.Pastel(consoleColor, new CultureInfo("fr-FR")));
            Assert.Equal(value.ToString("E4", new CultureInfo("en-US")).Pastel(consoleColor), value.Pastel(consoleColor, "E4", new CultureInfo("en-US")));
        }

        [Theory]
        [InlineData(615239, ConsoleColor.Black)]
        [InlineData(615239, ConsoleColor.DarkRed)]
        [InlineData(615239, ConsoleColor.DarkGreen)]
        [InlineData(615239, ConsoleColor.DarkYellow)]
        [InlineData(615239, ConsoleColor.DarkBlue)]
        [InlineData(615239, ConsoleColor.DarkMagenta)]
        [InlineData(615239, ConsoleColor.DarkCyan)]
        [InlineData(615239, ConsoleColor.Gray)]
        [InlineData(615239, ConsoleColor.DarkGray)]
        [InlineData(615239, ConsoleColor.Red)]
        [InlineData(615239, ConsoleColor.Green)]
        [InlineData(615239, ConsoleColor.Yellow)]
        [InlineData(615239, ConsoleColor.Blue)]
        [InlineData(615239, ConsoleColor.Magenta)]
        [InlineData(615239, ConsoleColor.Cyan)]
        [InlineData(615239, ConsoleColor.White)]
        public void LongToStringTest(long value, ConsoleColor consoleColor)
        {
            Assert.Equal(value.ToString().Pastel(consoleColor), value.Pastel(consoleColor));
            Assert.Equal(value.ToString("E").Pastel(consoleColor), value.Pastel(consoleColor, "E"));
            Assert.Equal(value.ToString(new CultureInfo("fr-FR")).Pastel(consoleColor), value.Pastel(consoleColor, new CultureInfo("fr-FR")));
            Assert.Equal(value.ToString("E4", new CultureInfo("en-US")).Pastel(consoleColor), value.Pastel(consoleColor, "E4", new CultureInfo("en-US")));
        }

        [Theory]
        [InlineData(127, ConsoleColor.Black)]
        [InlineData(127, ConsoleColor.DarkRed)]
        [InlineData(127, ConsoleColor.DarkGreen)]
        [InlineData(127, ConsoleColor.DarkYellow)]
        [InlineData(127, ConsoleColor.DarkBlue)]
        [InlineData(127, ConsoleColor.DarkMagenta)]
        [InlineData(127, ConsoleColor.DarkCyan)]
        [InlineData(127, ConsoleColor.Gray)]
        [InlineData(127, ConsoleColor.DarkGray)]
        [InlineData(127, ConsoleColor.Red)]
        [InlineData(127, ConsoleColor.Green)]
        [InlineData(127, ConsoleColor.Yellow)]
        [InlineData(127, ConsoleColor.Blue)]
        [InlineData(127, ConsoleColor.Magenta)]
        [InlineData(127, ConsoleColor.Cyan)]
        [InlineData(127, ConsoleColor.White)]
        public void SbyteToStringTest(sbyte value, ConsoleColor consoleColor)
        {
            Assert.Equal(value.ToString().Pastel(consoleColor), value.Pastel(consoleColor));
            Assert.Equal(value.ToString("E").Pastel(consoleColor), value.Pastel(consoleColor, "E"));
            Assert.Equal(value.ToString(new CultureInfo("fr-FR")).Pastel(consoleColor), value.Pastel(consoleColor, new CultureInfo("fr-FR")));
            Assert.Equal(value.ToString("E4", new CultureInfo("en-US")).Pastel(consoleColor), value.Pastel(consoleColor, "E4", new CultureInfo("en-US")));
        }

        [Theory]
        [InlineData(3127, ConsoleColor.Black)]
        [InlineData(3127, ConsoleColor.DarkRed)]
        [InlineData(3127, ConsoleColor.DarkGreen)]
        [InlineData(3127, ConsoleColor.DarkYellow)]
        [InlineData(3127, ConsoleColor.DarkBlue)]
        [InlineData(3127, ConsoleColor.DarkMagenta)]
        [InlineData(3127, ConsoleColor.DarkCyan)]
        [InlineData(3127, ConsoleColor.Gray)]
        [InlineData(3127, ConsoleColor.DarkGray)]
        [InlineData(3127, ConsoleColor.Red)]
        [InlineData(3127, ConsoleColor.Green)]
        [InlineData(3127, ConsoleColor.Yellow)]
        [InlineData(3127, ConsoleColor.Blue)]
        [InlineData(3127, ConsoleColor.Magenta)]
        [InlineData(3127, ConsoleColor.Cyan)]
        [InlineData(3127, ConsoleColor.White)]
        public void ShortToStringTest(short value, ConsoleColor consoleColor)
        {
            Assert.Equal(value.ToString().Pastel(consoleColor), value.Pastel(consoleColor));
            Assert.Equal(value.ToString("E").Pastel(consoleColor), value.Pastel(consoleColor, "E"));
            Assert.Equal(value.ToString(new CultureInfo("fr-FR")).Pastel(consoleColor), value.Pastel(consoleColor, new CultureInfo("fr-FR")));
            Assert.Equal(value.ToString("E4", new CultureInfo("en-US")).Pastel(consoleColor), value.Pastel(consoleColor, "E4", new CultureInfo("en-US")));
        }

        [Theory]
        [InlineData(651321, ConsoleColor.Black)]
        [InlineData(651321, ConsoleColor.DarkRed)]
        [InlineData(651321, ConsoleColor.DarkGreen)]
        [InlineData(651321, ConsoleColor.DarkYellow)]
        [InlineData(651321, ConsoleColor.DarkBlue)]
        [InlineData(651321, ConsoleColor.DarkMagenta)]
        [InlineData(651321, ConsoleColor.DarkCyan)]
        [InlineData(651321, ConsoleColor.Gray)]
        [InlineData(651321, ConsoleColor.DarkGray)]
        [InlineData(651321, ConsoleColor.Red)]
        [InlineData(651321, ConsoleColor.Green)]
        [InlineData(651321, ConsoleColor.Yellow)]
        [InlineData(651321, ConsoleColor.Blue)]
        [InlineData(651321, ConsoleColor.Magenta)]
        [InlineData(651321, ConsoleColor.Cyan)]
        [InlineData(651321, ConsoleColor.White)]
        public void UintToStringTest(uint value, ConsoleColor consoleColor)
        {
            Assert.Equal(value.ToString().Pastel(consoleColor), value.Pastel(consoleColor));
            Assert.Equal(value.ToString("E").Pastel(consoleColor), value.Pastel(consoleColor, "E"));
            Assert.Equal(value.ToString(new CultureInfo("fr-FR")).Pastel(consoleColor), value.Pastel(consoleColor, new CultureInfo("fr-FR")));
            Assert.Equal(value.ToString("E4", new CultureInfo("en-US")).Pastel(consoleColor), value.Pastel(consoleColor, "E4", new CultureInfo("en-US")));
        }

        [Theory]
        [InlineData(95222651321, ConsoleColor.Black)]
        [InlineData(95222651321, ConsoleColor.DarkRed)]
        [InlineData(95222651321, ConsoleColor.DarkGreen)]
        [InlineData(95222651321, ConsoleColor.DarkYellow)]
        [InlineData(95222651321, ConsoleColor.DarkBlue)]
        [InlineData(95222651321, ConsoleColor.DarkMagenta)]
        [InlineData(95222651321, ConsoleColor.DarkCyan)]
        [InlineData(95222651321, ConsoleColor.Gray)]
        [InlineData(95222651321, ConsoleColor.DarkGray)]
        [InlineData(95222651321, ConsoleColor.Red)]
        [InlineData(95222651321, ConsoleColor.Green)]
        [InlineData(95222651321, ConsoleColor.Yellow)]
        [InlineData(95222651321, ConsoleColor.Blue)]
        [InlineData(95222651321, ConsoleColor.Magenta)]
        [InlineData(95222651321, ConsoleColor.Cyan)]
        [InlineData(95222651321, ConsoleColor.White)]
        public void UlongToStringTest(ulong value, ConsoleColor consoleColor)
        {
            Assert.Equal(value.ToString().Pastel(consoleColor), value.Pastel(consoleColor));
            Assert.Equal(value.ToString("E").Pastel(consoleColor), value.Pastel(consoleColor, "E"));
            Assert.Equal(value.ToString(new CultureInfo("fr-FR")).Pastel(consoleColor), value.Pastel(consoleColor, new CultureInfo("fr-FR")));
            Assert.Equal(value.ToString("E4", new CultureInfo("en-US")).Pastel(consoleColor), value.Pastel(consoleColor, "E4", new CultureInfo("en-US")));
        }

        [Theory]
        [InlineData(6582, ConsoleColor.Black)]
        [InlineData(6582, ConsoleColor.DarkRed)]
        [InlineData(6582, ConsoleColor.DarkGreen)]
        [InlineData(6582, ConsoleColor.DarkYellow)]
        [InlineData(6582, ConsoleColor.DarkBlue)]
        [InlineData(6582, ConsoleColor.DarkMagenta)]
        [InlineData(6582, ConsoleColor.DarkCyan)]
        [InlineData(6582, ConsoleColor.Gray)]
        [InlineData(6582, ConsoleColor.DarkGray)]
        [InlineData(6582, ConsoleColor.Red)]
        [InlineData(6582, ConsoleColor.Green)]
        [InlineData(6582, ConsoleColor.Yellow)]
        [InlineData(6582, ConsoleColor.Blue)]
        [InlineData(6582, ConsoleColor.Magenta)]
        [InlineData(6582, ConsoleColor.Cyan)]
        [InlineData(6582, ConsoleColor.White)]
        public void UshortToStringTest(ushort value, ConsoleColor consoleColor)
        {
            Assert.Equal(value.ToString().Pastel(consoleColor), value.Pastel(consoleColor));
            Assert.Equal(value.ToString("E").Pastel(consoleColor), value.Pastel(consoleColor, "E"));
            Assert.Equal(value.ToString(new CultureInfo("fr-FR")).Pastel(consoleColor), value.Pastel(consoleColor, new CultureInfo("fr-FR")));
            Assert.Equal(value.ToString("E4", new CultureInfo("en-US")).Pastel(consoleColor), value.Pastel(consoleColor, "E4", new CultureInfo("en-US")));
        }
    }
}