﻿using System;
using System.Globalization;
using System.Linq;

namespace KontrolSystem.Parsing;

public static partial class Parsers {
    /// <summary>
    ///     Parse a whitespace.
    /// </summary>
    public static readonly Parser<char> WhiteSpace = Char(char.IsWhiteSpace, "<whitespace>");

    /// <summary>
    ///     Parse zero or more whitespace.
    /// </summary>
    public static readonly Parser<bool> WhiteSpaces0 = Chars0(char.IsWhiteSpace);

    /// <summary>
    ///     Parse zero or more SpaceSeparators (including tabs)
    /// </summary>
    public static readonly Parser<bool> Spacing0 = Chars0(ch =>
        ch == '\t' || char.GetUnicodeCategory(ch) == UnicodeCategory.SpaceSeparator);

    /// <summary>
    ///     Parse one or more whitespace.
    /// </summary>
    public static readonly Parser<bool> WhiteSpaces1 = Chars1(char.IsWhiteSpace, "<whitespace>");

    /// <summary>
    ///     Parse one or more SpaceSeparators (including tabs)
    /// </summary>
    public static readonly Parser<bool> Spacing1 =
        Chars1(
            ch => ch == '\t' || char.GetUnicodeCategory(ch) ==
                UnicodeCategory.SpaceSeparator, "<space>");

    /// <summary>
    ///     Parse a letter.
    /// </summary>
    public static readonly Parser<char> Letter = Char(char.IsLetter, "<letter>");

    /// <summary>
    ///     Parse a letter or digit.
    /// </summary>
    public static readonly Parser<char> LetterOrDigit = Char(char.IsLetterOrDigit, "<letter or digit>");

    /// <summary>
    ///     Parse zero or more digits.
    /// </summary>
    public static readonly Parser<bool> Digits0 = Chars0(char.IsDigit);

    /// <summary>
    ///     Parse one or more digits.
    /// </summary>
    public static readonly Parser<bool> Digits1 = Chars1(char.IsDigit, "<digit>");

    public static readonly Parser<string> LineEnd = Opt(Char('\r'))
        .Then(r => Char('\n').Map(n => r.Map(char.ToString).GetOrElse("") + n));

    /// <summary>
    ///     Non-consuming parser that succeeds if input is at a line end or end of input.
    /// </summary>
    public static readonly Parser<bool> PeekLineEnd = input => {
        if (input.Available == 0) return Result.Success(input, false);
        if (input.Available >= 1 && input.Current == '\n') return Result.Success(input, true);
        if (input.Match("\r\n")) return Result.Success(input, true);
        return Result.Failure<bool>(input, "<end of line>");
    };

    /// <summary>
    ///     TryParse a single character matching 'predicate'
    /// </summary>
    public static Parser<char> Char(Predicate<char> predicate, string expected) {
        return input => {
            if (input.Available < 1) return Result.Failure<char>(input, expected);
            var current = input.Current;
            if (!predicate(current)) return Result.Failure<char>(input, expected);
            return Result.Success(input.Advance(1), current);
        };
    }

    /// <summary>
    ///     Parse a single character except those matching <paramref name="predicate" />.
    /// </summary>
    public static Parser<char> CharExcept(Predicate<char> predicate, string expected) {
        return Char(c => !predicate(c), "any character except " + expected);
    }

    /// <summary>
    ///     Zero or more characters matching 'predicate'
    /// </summary>
    public static Parser<bool> Chars0(Predicate<char> predicate) {
        return input => {
            var count = input.FindNext(ch => !predicate(ch));
            if (count < 0) return Result.Success(input.Advance(input.Available), true);
            return Result.Success(input.Advance(count), true);
        };
    }

    /// <summary>
    ///     One or more characters matching 'predicate'
    /// </summary>
    public static Parser<bool> Chars1(Predicate<char> predicate, string expected) {
        return input => {
            if (input.Available < 1) return Result.Failure<bool>(input, expected);
            var count = input.FindNext(ch => !predicate(ch));
            if (count < 0) return Result.Success(input.Advance(input.Available), true);
            if (count == 0) return Result.Failure<bool>(input, expected);
            return Result.Success(input.Advance(count), true);
        };
    }

    /// <summary>
    ///     Parse a single character c.
    /// </summary>
    public static Parser<char> Char(char c) {
        return Char(ch => ch == c, $"'{c}'");
    }

    /// <summary>
    ///     Parse a single character of any in c
    /// </summary>
    public static Parser<char> OneOf(string c) {
        return Char(c.Contains, string.Join("|", c.ToArray()));
    }

    /// <summary>
    ///     Zero or more characters not in list
    /// </summary>
    public static Parser<bool> CharsExcept0(string c) {
        return Chars0(ch => !c.Contains(ch));
    }

    /// <summary>
    ///     One or more characters not in list
    /// </summary>
    public static Parser<bool> CharsExcept1(string c, string expected) {
        return Chars1(ch => !c.Contains(ch), expected);
    }

    public static Parser<char> CharExcept(string c) {
        return CharExcept(c.Contains, c);
    }

    /// <summary>
    ///     Parse an exact tag (i.e. sequence of chars).
    /// </summary>
    public static Parser<bool> Tag(string tag) {
        return input => {
            if (!input.Match(tag)) return Result.Failure<bool>(input, $"'{tag}'");
            return Result.Success(input.Advance(tag.Length), true);
        };
    }
}
