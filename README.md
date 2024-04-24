# Pastel

![logo](https://raw.githubusercontent.com/silkfire/Pastel/master/img/logo.png)

[![NuGet](https://img.shields.io/nuget/dt/Pastel.svg)](https://www.nuget.org/packages/Pastel)
[![NuGet](https://img.shields.io/nuget/v/Pastel.svg)](https://www.nuget.org/packages/Pastel)

Give your console app a nicer look by adding some color to the output it produces. 
This is achieved by wrapping strings of the output in [ANSI escape sequences](https://en.wikipedia.org/wiki/ANSI_escape_code) that instruct the terminal to color the string based on the interpreted code. Tested on both Windows (requires at least Windows 10, v1511 [November Update]) and Linux.

## Introduction

Modern terminals have a feature that allows them to print text in different colors. To enable this, a string is wrapped with a special sequence of characters containing a directive to the terminal to color the string that follows and stop coloring when it encounters an end code. Producing these character sequences can be cumbersome, which is the reason why I decided to build this small library that turns this into a very easy task.  
Because Pastel only alters the output string, there is no need to manipulate or extend the built-in `System.Console` class.

If your terminal doesn't support 24-bit colors, it will approximate to the nearest color instead.


## How to use

The basic syntax is very simple. Use the `Pastel()` method on the string you want to colorize and supply a color argument.

```cs
"ENTER".Pastel(Color.FromArgb(165, 229, 250))

Console.WriteLine($"Press {"ENTER".Pastel(Color.FromArgb(165, 229, 250))} to continue");
```
![Example 1](https://raw.githubusercontent.com/silkfire/Pastel/master/img/example1.png)

You can either use a `System.Drawing.Color` object, a `System.ConsoleColor` enum or a hexadecimal string value.  
Both upper and lower case hex codes are supported and the leading number sign (#) is optional. 


```cs
var spectrum = new (string color, string letter)[]
{
    ("#124542", "a"),
    ("#185C58", "b"),
    ("#1E736E", "c"),
    ("#248A84", "d"),
    ("#20B2AA", "e"),
    ("#3FBDB6", "f"),
    ("#5EC8C2", "g"),
    ("#7DD3CE", "i"),
    ("#9CDEDA", "j"),
    ("#BBE9E6", "k")
};

Console.WriteLine(string.Join("", spectrum.Select(s => s.letter.Pastel(s.color))));
```
![Example 2](https://raw.githubusercontent.com/silkfire/Pastel/master/img/example2.png)  

![Example 3](https://raw.githubusercontent.com/silkfire/Pastel/master/img/example3.png)

Using a `Color`/`ConsoleColor` argument pairs very well with ReSharper as the extension automatically underlines the argument list and colors it accordingly:

![ReSharper color object underlining](https://raw.githubusercontent.com/silkfire/Pastel/master/img/resharper-coloring.png)


## Background colors

Pastel also supports background colors. The syntax is exactly the same except that the method is called `PastelBg`.  
Both foreground and background colors can be combined by chaining the methods:

```cs
"Colorize me".Pastel(Color.Black).PastelBg("FFD000");
```

![Example 4](https://raw.githubusercontent.com/silkfire/Pastel/master/img/example4.png)

## Disabling / enabling color output

If you for any reason would like to disable any future color output produced by Pastel for the duration of your app, simply call `ConsoleExtensions.Disable()`. To re-enable color output, call `ConsoleExtensions.Enable()`.

### CI/CD environments

Pastel will detect if your application is running under a common CI/CD environment and will disable all coloring if this is the case.  
If you'd like to override this check and force colors in CI/CD environments, you can set an environment variable named `PASTEL_DISABLE_ENVIRONMENT_DETECTION` (value does not matter).

### NO_COLOR

Pastel will also honor systems where console color output has explicitly been requested to be turned off. See more information about this initiative at https://no-color.org.

## Support

Has this library helped you or proven to be useful in your project?

* Leave a star!
* Consider buying me a coffee with the link below! ⭐

<a href="https://www.buymeacoffee.com/silkfire" target="_blank" style="margin-left: 10px;"><img src="https://img.buymeacoffee.com/button-api/?textBuy me a coffee&amp;emoji=☕&amp;slug=silkfire&amp;button_colour=FFDD00&amp;font_colour=000000&amp;font_family=Cookie&amp;outline_colour=000000&amp;coffee_colour=ffffff"></a>
