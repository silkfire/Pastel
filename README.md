# Pastel

[![NuGet](https://img.shields.io/nuget/dt/Pastel.svg)](https://www.nuget.org/packages/Pastel)
[![NuGet](https://img.shields.io/nuget/v/Pastel.svg)](https://www.nuget.org/packages/Pastel)

Give your console app a nicer look by adding some color to the output it produces. 
This is achieved by wrapping strings of the output in [ANSI codes](https://www.jerriepelser.com/blog/using-ansi-color-codes-in-net-console-apps/) that instruct the terminal to color the string based on the interpreted code. Tested on both Windows and Linux.

## Introduction

Modern terminals have a feature that allows them to print text in different colors. To enable this, a string is wrapped with a special sequence of characters containing a directive to the terminal to color the string that follows and stop coloring when it encounters an end code. Producing these character sequences can be cumbersome, which is the reason why I decided to build this small library that turns this into a very easy task.  
Because Pastel only alters the output string, there is no need to manipulate or extend the built-in `System.Console` class.

If your terminal doesn't support 24-bit colors, it will approximate to the nearest color instead.

This library was inspired by [Crayon](https://github.com/riezebosch/crayon), except that it has two main differences:

1. Instead of calling the coloring method by using the name of a static class, Pastel provides an extension method on the `String` object, leaving you to just type the method name and supply the color argument.
2. This library allows you to produce _any_ color (then it's up to your terminal whether it can correctly interpret the code, provided that it supports 24-bit colors), whereas Crayon only gives you a small set of predefined colors to choose from.


## How to use

The basic syntax is very simple. Use the `Pastel()` method on the string you want to colorize and supply a color argument.

```cs
"ENTER".Pastel(Color.FromArgb(165, 229, 250))

Console.WriteLine($"Press {"ENTER".Pastel(Color.FromArgb(165, 229, 250))} to continue");
```
![Example 1](https://github.com/silkfire/Pastel/blob/master/img/example1.png)

You can either use a `System.Drawing.Color` object or a hexadecimal string value.  
Both upper and lower case hex codes are supported and the pound sign (#) is optional. 


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
![Example 2](https://github.com/silkfire/Pastel/blob/master/img/example2.png)  

![Example 3](https://github.com/silkfire/Pastel/blob/master/img/example3.png)

Using a `Color` argument pairs very well with ReSharper as the extension automatically underlines the argument list and colors it accordingly:

![ReSharper color object underlining](https://github.com/silkfire/Pastel/blob/master/img/resharper-coloring.png)


## Background colors

Pastel now also supports background colors. The syntax is exactly the same except that the method is called `PastelBg`. Both foreground and background colors can be combined by chaining the methods:

```cs
"Colorize me".Pastel(Color.Black).PastelBg("FFD000");
```

![Example 4](https://github.com/silkfire/Pastel/blob/master/img/example4.png)
