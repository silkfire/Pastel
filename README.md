# Pastel

Give your flat console app a refreshing look by adding some colour to the output it produces! 
This is achieved by wrapping strings of the output in [ANSI codes](https://www.jerriepelser.com/blog/using-ansi-color-codes-in-net-console-apps/) that instruct the terminal to colour the string based on the code provided.

## Introduction

Modern terminals have a feature that allows them to print text in different colours. To support this, a string is wrapped with a special sequence of characters containing a directive to the terminal to colour the string that follows and stop colouring when it encounters an end code. Producing these character sequences can be cumbersome, which is the reason why I decided to build this small library that turns this into a very easy task.  
Because **Pastel** only alters the output string, there is no need to manipulate or extend the built-in `System.Console` class.

If your terminal doesn't support 24-bit colours, it will approximate to the nearest colour instead.

This library was inspired by [Crayon](https://github.com/riezebosch/crayon), except that it has two main differences:

1. Instead of calling the colouring method by using the name of a static class, **Pastel** extends the `String` object, leaving you to just type the method name and supply the colour argument.
2. This library allows you to produce _any_ colour (then it's up to your terminal whether it can correctly interpret the code, depending on whether it supports 24-bit colours), whereas Crayon only gives you a small set of predefined colours to choose from.


## How to use

```cs
Console.WriteLine($"Press {"ENTER".Pastel(Color.FromArgb(165, 229, 250))} to continue";)
```

**IMPORTANT!** _Note that ANSI codes cannot be produced inside verbatim strings (`@"string"`), as these will interpret the escape sequences literally rather than transforming them into the intended codes._

