﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;
using ExplogineMonoGame.TextFormatting;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ExplogineMonoGame.Data;

public class FormattedText
{
    private readonly IFragment[]? _fragments;

    public FormattedText(IFontGetter indirectFont, string text, Color? color = null) : this(
        new Fragment(indirectFont, text, color))
    {
    }

    public FormattedText(params IFragment[] fragments)
    {
        _fragments = fragments;
    }

    public FormattedText(IFontGetter startingFont, Color startingColor,
        Instruction[] instructions)
    {
        var textRun = new TextRun(startingFont, startingColor);

        foreach (var instruction in instructions)
        {
            instruction.Do(textRun);
        }

        _fragments = textRun.Fragments.ToArray();
    }

    private FormattedText(IFontGetter startingFont, Color startingColor, string formatString,
        FormattedTextParser parser) : this(startingFont, startingColor,
        Format.StringToInstructions(formatString, parser))
    {
    }

    public int Length
    {
        get
        {
            if (_fragments == null || _fragments.Length == 0)
            {
                return 0;
            }

            var charCount = 0;
            foreach (var fragment in _fragments)
            {
                charCount += fragment.CharCount;
            }

            return charCount;
        }
    }

    public static FormattedText FromFormatString(IFontGetter startingFont, Color startingColor, string formatString,
        FormattedTextParser? parser = null)
    {
        return new FormattedText(startingFont, startingColor, formatString, parser ?? FormattedTextParser.Default);
    }

    /// <summary>
    ///     The size of the whole text if it had infinite width
    /// </summary>
    /// <returns></returns>
    public Vector2 MaxNeededSize(float? restrictWidth = null)
    {
        var (_, restrictedSize) = RestrictedStringBuilder.FromFragments(_fragments!, restrictWidth ?? float.MaxValue);

        // +1 on both sides to round up
        return restrictedSize + new Vector2(1);
    }

    [Obsolete("RENAMED Use MaxNeededSize instead", true)]
    public Vector2 MaxNeededWith()
    {
        return MaxNeededSize(float.MaxValue);
    }

    public IEnumerable<FormattedGlyph> GetGlyphs(RectangleF rectangle, Alignment alignment)
    {
        if (_fragments == null || _fragments.Length == 0)
        {
            yield break;
        }

        var (lines, restrictedSize) = RestrictedStringBuilder.FromFragments(_fragments, rectangle.Width);
        var restrictedBounds =
            RectangleF.FromSizeAlignedWithin(rectangle, restrictedSize, alignment.JustVertical());

        var lineNumber = 0;
        var verticalSpaceUsedByPreviousLines = 0f;
        foreach (var fragmentLine in lines)
        {
            var actualLineSize = fragmentLine.Size;
            var availableBoundForLine = new RectangleF(
                restrictedBounds.TopLeft + new Vector2(0, verticalSpaceUsedByPreviousLines),
                new Vector2(rectangle.Width, actualLineSize.Y));
            var actualLineBounds =
                RectangleF.FromSizeAlignedWithin(availableBoundForLine, actualLineSize, alignment);

            verticalSpaceUsedByPreviousLines += actualLineSize.Y;

            var letterPosition = Vector2.Zero;
            foreach (var letterFragment in fragmentLine)
            {
                var letterSize = letterFragment.Size;
                var position = actualLineBounds.TopLeft + letterPosition + fragmentLine.Size.JustY() -
                               letterSize.JustY();
                var isWhiteSpace = letterFragment is CharGlyphData fragmentChar &&
                                   char.IsWhiteSpace(fragmentChar.Text);

                var glyphData = letterFragment;
                if (isWhiteSpace)
                {
                    // White space still has a size and scale factor, but it's a different type so it's not caught like regular text
                    glyphData = new WhiteSpaceGlyphData(letterFragment.Size, letterFragment.ScaleFactor,
                        WhiteSpaceType.Space);
                }

                yield return new FormattedGlyph(position, glyphData, lineNumber);
                letterPosition += letterSize.JustX();
            }

            lineNumber++;
        }
    }

    /// <summary>
    ///     GlyphData is the guts of the output
    /// </summary>
    public interface IGlyphData
    {
        Vector2 Size { get; }
        float ScaleFactor { get; }

        /// <summary>
        ///     We never actually call this I think?
        /// </summary>
        void OneOffDraw(Painter painter, Vector2 position, DrawSettings drawSettings);
    }

    /// <summary>
    ///     Fragments are input
    /// </summary>
    public interface IFragment
    {
        Vector2 Size { get; }
        int CharCount { get; }
        IGlyphData ToGlyphData();
    }

    /// <summary>
    ///     This is the final output of the FormattedText. These are the things you interface with.
    /// </summary>
    /// <param name="Position"></param>
    /// <param name="Data"></param>
    /// <param name="LineNumber"></param>
    public readonly record struct FormattedGlyph(Vector2 Position, IGlyphData Data, int LineNumber)
    {
        public RectangleF Rectangle => new(Position, Data.Size);

        public void Draw(Painter painter, Vector2 offset, DrawSettings settings)
        {
            Data.OneOffDraw(painter, Position + offset, settings);
        }
    }

    public readonly record struct Fragment(IFontGetter FontGetter, string Text, Color? Color = null) : IFragment
    {
        public IFont Font => FontGetter.GetFont();
        public int CharCount => Text.Length;
        public Vector2 Size => Font.MeasureString(Text);

        public IGlyphData ToGlyphData()
        {
            // Sucks that this is written this way
            throw new Exception(
                $"{nameof(Fragment)} contains more than one character so it cannot be converted to {nameof(IGlyphData)}");
        }

        public override string ToString()
        {
            return $"{Size} \"{Text}\"";
        }
    }

    public readonly record struct CharGlyphData(IFont Font, char Text, Color? Color = null) : IGlyphData
    {
        public Vector2 Size => Font.MeasureString(CacheUtil.CharToString(Text));
        public float ScaleFactor => Font.ScaleFactor;

        public void OneOffDraw(Painter painter, Vector2 position, DrawSettings drawSettings)
        {
            painter.DrawStringAtPosition(Font, CacheUtil.CharToString(Text), position,
                drawSettings with {Color = Color ?? drawSettings.Color});
        }

        public override string ToString()
        {
            return $"{Size} '{Text}'";
        }
    }

    public readonly record struct WhiteSpaceGlyphData(
        Vector2 Size,
        float ScaleFactor,
        WhiteSpaceType WhiteSpaceType) : IGlyphData
    {
        public void OneOffDraw(Painter painter, Vector2 position, DrawSettings drawSettings)
        {
            // this function is intentionally left blank
        }

        public override string ToString()
        {
            return $"{Size} (whitespace)";
        }
    }

    public readonly record struct ImageGlyphData(
        IndirectAsset<ImageAsset> Image,
        float ScaleFactor = 1f,
        Color? Color = null) : IGlyphData
    {
        public void OneOffDraw(Painter painter, Vector2 position, DrawSettings drawSettings)
        {
            painter.DrawAtPosition(Image.Get().Texture, position, new Scale2D(ScaleFactor),
                drawSettings with {SourceRectangle = Image.Get().SourceRectangle, Color = Color ?? drawSettings.Color});
        }

        public Vector2 Size => Image.Get().SourceRectangle.Size.ToVector2() * ScaleFactor;

        public override string ToString()
        {
            return $"{Size} (image)";
        }
    }

    public readonly record struct DrawableGlyphData(
        Action<Painter, Vector2, DrawSettings> DrawCallback,
        Func<Vector2> SizeCallback)
        : IGlyphData
    {
        public Vector2 Size => SizeCallback();
        public float ScaleFactor => 1f;

        public void OneOffDraw(Painter painter, Vector2 position, DrawSettings drawSettings)
        {
            DrawCallback(painter, position, drawSettings);
        }
    }

    public readonly record struct FragmentDrawable(
        Action<Painter, Vector2, DrawSettings> DrawCallback,
        Func<Vector2> SizeCallback) : IFragment
    {
        public Vector2 Size => SizeCallback();

        public IGlyphData ToGlyphData()
        {
            return new DrawableGlyphData(DrawCallback, SizeCallback);
        }

        /// <summary>
        ///     This represents one drawable "unit" so it's just 1 character
        /// </summary>
        public int CharCount => 1;
    }

    public readonly record struct FragmentImage(
        IndirectAsset<ImageAsset> Image,
        float ScaleFactor = 1f,
        Color? Color = null) : IFragment
    {
        public FragmentImage(Texture2D texture, float scaleFactor = 1f, Color? color = null) : this(
            new ImageAsset(texture, texture.Bounds), scaleFactor, color)
        {
        }

        public Vector2 Size => Image.Get().SourceRectangle.Size.ToVector2() * ScaleFactor;

        public IGlyphData ToGlyphData()
        {
            return new ImageGlyphData(Image, ScaleFactor, Color);
        }

        /// <summary>
        ///     An image is always just one character
        /// </summary>
        public int CharCount => 1;

        public override string ToString()
        {
            return $"{Size} (image)";
        }
    }

    public readonly record struct GlyphDataLine(ImmutableArray<IGlyphData> Fragments) : IEnumerable<IGlyphData>
    {
        public Vector2 Size
        {
            get
            {
                var size = new Vector2();
                foreach (var fragment in Fragments)
                {
                    size.X += fragment.Size.X;
                    size.Y = MathF.Max(size.Y, fragment.Size.Y);
                }

                return size;
            }
        }

        public IEnumerator<IGlyphData> GetEnumerator()
        {
            foreach (var item in Fragments)
            {
                yield return item;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public override string ToString()
        {
            var result = new StringBuilder();
            foreach (var fragment in Fragments)
            {
                if (fragment is CharGlyphData fragmentChar)
                {
                    result.Append(fragmentChar.Text);
                }

                if (fragment is ImageGlyphData)
                {
                    result.Append("(image)");
                }
            }

            return $"{Size} {result}";
        }
    }
}

public class TextRun
{
    private readonly Stack<Color> _colors = new();
    private readonly Stack<IFontGetter> _fonts = new();
    private readonly Stack<float> _scales = new();
    private readonly Color _startingColor;
    private readonly IFontGetter _startingFont;

    public TextRun(IFontGetter startingFont, Color startingColor)
    {
        _startingFont = startingFont;
        _startingColor = startingColor;
        _fonts.Push(startingFont);
        _colors.Push(startingColor);
    }

    public List<FormattedText.IFragment> Fragments { get; } = new();

    public IFontGetter PeekFont()
    {
        if (!_fonts.TryPeek(out var font))
        {
            font = _startingFont.GetFont();
        }

        var realFont = font.GetFont();
        font = realFont.WithHeight((int) (realFont.Height * PeekScale()));

        return font;
    }

    public float PeekScale()
    {
        if (!_scales.TryPeek(out var scale))
        {
            scale = 1f;
        }

        return scale;
    }

    public Color PeekColor()
    {
        if (!_colors.TryPeek(out var color))
        {
            color = _startingColor;
        }

        return color;
    }

    public void PushColor(Color color)
    {
        _colors.Push(color);
    }

    public void PushFont(IFontGetter font)
    {
        _fonts.Push(font);
    }

    public void PopColor()
    {
        _colors.TryPop(out _);
    }

    public void PopFont()
    {
        _fonts.TryPop(out _);
    }

    public void PopScale()
    {
        _scales.TryPop(out _);
    }

    public void PushScale(float scale)
    {
        _scales.Push(scale);
    }
}
