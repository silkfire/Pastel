using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;

namespace Pastel
{
    public static class ConsoleExtensions
    {

        static ConsoleExtensions()
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) return;
            var iStdOut = GetStdHandle(StdOutputHandle);

            var enable = GetConsoleMode(iStdOut, out var outConsoleMode) 
                         && SetConsoleMode(iStdOut, 
                             outConsoleMode | EnableVirtualTerminalProcessing | DisableNewlineAutoReturn);
        }


        private const int StdOutputHandle = -11;
        private const uint EnableVirtualTerminalProcessing = 0x0004;
        private const uint DisableNewlineAutoReturn = 0x0008;

        [DllImport("kernel32.dll")]
        private static extern bool GetConsoleMode(IntPtr hConsoleHandle, out uint lpMode);

        [DllImport("kernel32.dll")]
        private static extern bool SetConsoleMode(IntPtr hConsoleHandle, uint dwMode);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr GetStdHandle(int nStdHandle);

        [DllImport("kernel32.dll")]
        public static extern uint GetLastError();

        /// <summary>
        /// Returns a string wrapped in an ANSI foreground color code using the specified color.
        /// </summary>
        /// <param name="input">The string to color.</param>
        /// <param name="color">The color to use on the specified string.</param>
        public static string Pastel(this string input, Color color)
        {
            return $"\u001b[38;2;{color.R};{color.G};{color.B}m{input}\u001b[0m";
        }

        /// <summary>
        /// Returns a string wrapped in an ANSI foreground color code using the specified color.
        /// </summary>
        /// <param name="input">The string to color.</param>
        /// <param name="hexColor">The color to use on the specified string.<para>Supported format: [#]RRGGBB.</para></param>
        public static string Pastel(this string input, string hexColor)
        {
            var color = Color.FromArgb(int.Parse(hexColor.Replace("#", ""), NumberStyles.HexNumber));

            return Pastel(input, color);
        }

        /// <summary>
        /// Returns a string wrapped in an ANSI foreground color code using the specified color with BOLD decoration.
        /// </summary>
        /// <param name="input">The string to color.</param>
        /// <param name="color">The color to use on the specified string.</param>
        public static string PastelBold(this string input, Color color)
        {
            return $"\u001b[38;2;{color.R};{color.G};{color.B};1m{input}\u001b[0m";
        }

        /// <summary>
        /// Returns a string wrapped in an ANSI foreground color code using the specified color with BOLD decoration.
        /// </summary>
        /// <param name="input">The string to color.</param>
        /// <param name="hexColor">The color to use on the specified string.<para>Supported format: [#]RRGGBB.</para></param>
        public static string PastelBold(this string input, string hexColor)
        {
            var color = Color.FromArgb(int.Parse(hexColor.Replace("#", ""), NumberStyles.HexNumber));

            return PastelBold(input, color);
        }

        /// <summary>
        /// Returns a string wrapped in an ANSI foreground color code using the specified color with UnderLine decoration.
        /// </summary>
        /// <param name="input">The string to color.</param>
        /// <param name="color">The color to use on the specified string.</param>
        public static string PastelUnderLine(this string input, Color color)
        {
            return $"\u001b[38;2;{color.R};{color.G};{color.B};4m{input}\u001b[0m";
        }

        /// <summary>
        /// Returns a string wrapped in an ANSI foreground color code using the specified color with UnderLine decoration.
        /// </summary>
        /// <param name="input">The string to color.</param>
        /// <param name="hexColor">The color to use on the specified string.<para>Supported format: [#]RRGGBB.</para></param>
        public static string PastelUnderLine(this string input, string hexColor)
        {
            var color = Color.FromArgb(int.Parse(hexColor.Replace("#", ""), NumberStyles.HexNumber));

            return PastelUnderLine(input, color);
        }

        /// <summary>
        /// Returns a string wrapped in an ANSI foreground color code using the specified color with Inverse decoration.
        /// </summary>
        /// <param name="input">The string to color.</param>
        /// <param name="color">The color to use on the specified string.</param>
        public static string PastelInverse(this string input, Color color)
        {
            return $"\u001b[38;2;{color.R};{color.G};{color.B};7m{input}\u001b[0m";
        }

        /// <summary>
        /// Returns a string wrapped in an ANSI foreground color code using the specified color with Inverse decoration.
        /// </summary>
        /// <param name="input">The string to color.</param>
        /// <param name="hexColor">The color to use on the specified string.<para>Supported format: [#]RRGGBB.</para></param>
        public static string PastelInverse(this string input, string hexColor)
        {
            var color = Color.FromArgb(int.Parse(hexColor.Replace("#", ""), NumberStyles.HexNumber));

            return PastelInverse(input, color);
        }

        /// <summary>
        /// Returns a string wrapped in an ANSI foreground color code using the specified color.
        /// </summary>
        /// <param name="input">The string to color.</param>
        /// <param name="startColor">The color to use at the start of the gradient.</param>
        /// <param name="endColor">The color to use to end the gradient.</param>
        public static string PastelWithGradient(this string input, Color startColor, Color endColor)
        {
            var gradient = GenerateGradient(input, startColor, endColor, input.Length);

            return string.Concat(gradient.Select(x => Pastel(x.Target.ToString(), x.Color)));
        }

        /// <summary>
        /// Returns a string wrapped in an ANSI foreground color code using the specified color.
        /// </summary>
        /// <param name="input">The string to color.</param>
        /// <param name="startHexColor">The color to use at the start of the gradient.<para>Supported format: [#]RRGGBB.</para></param>
        /// <param name="endHexColor">The color to use to end the gradient.<para>Supported format: [#]RRGGBB.</para></param>
        public static string PastelWithGradient(this string input, string startHexColor, string endHexColor)
        {
            var startColor = Color.FromArgb(int.Parse(startHexColor.Replace("#", ""), NumberStyles.HexNumber));
            var endColor = Color.FromArgb(int.Parse(endHexColor.Replace("#", ""), NumberStyles.HexNumber));

            return PastelWithGradient(input, startColor, endColor);
        }

        /// <summary>
        /// Returns a string wrapped in an ANSI background color code using the specified color.
        /// </summary>
        /// <param name="input">The string to color.</param>
        /// <param name="color">The color to use on the specified string.</param>
        public static string PastelBg(this string input, Color color)
        {
            return $"\u001b[48;2;{color.R};{color.G};{color.B}m{input}\u001b[0m";
        }

        /// <summary>
        /// Returns a string wrapped in an ANSI background color code using the specified color.
        /// </summary>
        /// <param name="input">The string to color.</param>
        /// <param name="hexColor">The color to use on the specified string.<para>Supported format: [#]RRGGBB.</para></param>
        public static string PastelBg(this string input, string hexColor)
        {
            var color = Color.FromArgb(int.Parse(hexColor.Replace("#", ""), NumberStyles.HexNumber));

            return PastelBg(input, color);
        }

        public static List<StyleClass<T>> GenerateGradient<T>(IEnumerable<T> input, Color startColor, Color endColor, int maxColorsInGradient)
        {
            var inputAsList = input.ToList();
            var numberOfGrades = inputAsList.Count / maxColorsInGradient;
            var numberOfGradesRemainder = inputAsList.Count % maxColorsInGradient;

            var gradients = new List<StyleClass<T>>();
            var previousColor = Color.Empty;
            var previousItem = default(T);
            int SetProgressSymmetrically(int remainder) => remainder > 1 ? -1 : 0; // An attempt to make the gradient symmetric in the event that maxColorsInGradient does not divide input.Count evenly.
            int ResetProgressSymmetrically(int progress) => progress == 0 ? -1 : 0; // An attempt to make the gradient symmetric in the event that maxColorsInGradient does not divide input.Count evenly.
            var colorChangeProgress = SetProgressSymmetrically(numberOfGradesRemainder);
            var colorChangeCount = 0;

            bool IsFirstRun(int index) => index == 0;
            bool ShouldChangeColor(int index, int progress, T current, T previous) => (progress > numberOfGrades - 1 && !current.Equals(previous) || IsFirstRun(index));
            bool CanChangeColor(int changeCount) => changeCount < maxColorsInGradient;

            for (var i = 0; i < inputAsList.Count; i++)
            {
                var currentItem = inputAsList[i];
                colorChangeProgress++;

                if (ShouldChangeColor(i, colorChangeProgress, currentItem, previousItem) && CanChangeColor(colorChangeCount))
                {
                    previousColor = GetGradientColor(i, startColor, endColor, inputAsList.Count);
                    previousItem = currentItem;
                    colorChangeProgress = ResetProgressSymmetrically(colorChangeProgress);
                    colorChangeCount++;
                }

                gradients.Add(new StyleClass<T>(currentItem, previousColor));
            }

            return gradients;
        }

        private static Color GetGradientColor(int index, Color startColor, Color endColor, int numberOfGrades)
        {
            var numberOfGradesAdjusted = numberOfGrades - 1;

            var rDistance = startColor.R - endColor.R;
            var gDistance = startColor.G - endColor.G;
            var bDistance = startColor.B - endColor.B;

            var r = startColor.R + (-rDistance * ((double)index / numberOfGradesAdjusted));
            var g = startColor.G + (-gDistance * ((double)index / numberOfGradesAdjusted));
            var b = startColor.B + (-bDistance * ((double)index / numberOfGradesAdjusted));

            var graded = Color.FromArgb((int)r, (int)g, (int)b);
            return graded;
        }
    }

    /// <inheritdoc />
    /// <summary>
    /// Exposes methods and properties that represent a style classification.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class StyleClass<T> : IEquatable<StyleClass<T>>
    {
        /// <summary>
        /// The object to be styled.
        /// </summary>
        public T Target { get; set; }

        /// <summary>
        /// The color to be applied to the target.
        /// </summary>
        public Color Color { get; set; }

        /// <summary>
        /// Exposes methods and properties that represent a style classification.
        /// </summary>
        public StyleClass()
        {
        }

        /// <summary>
        /// Exposes methods and properties that represent a style classification.
        /// </summary>
        /// <param name="target">The object to be styled.</param>
        /// <param name="color">The color to be applied to the target.</param>
        public StyleClass(T target, Color color)
        {
            Target = target;
            Color = color;
        }

        public bool Equals(StyleClass<T> other)
        {
            if (other == null)
            {
                return false;
            }

            return Target.Equals(other.Target)
                   && Color == other.Color;
        }

        public override bool Equals(object obj) => Equals(obj as StyleClass<T>);

        public override int GetHashCode()
        {
            int hash = 163;

            hash *= 79 + Target.GetHashCode();
            hash *= 79 + Color.GetHashCode();

            return hash;
        }
    }
}
