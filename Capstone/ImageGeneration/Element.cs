using SkiaSharp;
using System.Numerics;

namespace ImageGeneration
{
    public enum ElementType
    {
        Primitive,
        Glyph,
        Text
    }
    public enum SnapPosition
    {
        TopLeft,
        TopRight,
        Top,
        MidLeft,
        MidRight,
        Center,
        BottomLeft,
        BottomRight,
        Bottom
    }
    public abstract class IElement
    {
        public ElementType ElementType { get; set; }
        public Vector2 Position { get; set; }
        public Vector2 Size { get; set; }
        public abstract void Draw(Document document);
        public virtual Vector2 SnapTo(SnapPosition snapPosition, Vector2? offset = null)
        {
            if (offset == null)
            {
                offset = Vector2.Zero;
            }
            Position += (Vector2)offset;
            return snapPosition switch
            {
                SnapPosition.TopLeft => Position,
                SnapPosition.TopRight => new Vector2(Position.X + Size.X, Position.Y),
                SnapPosition.Top => new Vector2(Position.X + (Size.X / 2), Position.Y),
                SnapPosition.MidLeft => new Vector2(Position.X, Position.Y + (Size.Y / 2)),
                SnapPosition.MidRight => new Vector2(Position.X + Size.X, Position.Y + (Size.Y / 2)),
                SnapPosition.Center => new Vector2(Position.X + (Size.X / 2), Position.Y + (Size.Y / 2)),
                SnapPosition.BottomLeft => new Vector2(Position.X, Position.Y + Size.Y),
                SnapPosition.Bottom => new Vector2(Position.X + (Size.X / 2), Position.Y + Size.Y),
                SnapPosition.BottomRight => new Vector2(Position.X + Size.X, Position.Y + Size.Y),
                _ => Vector2.Zero,
            };
        }
        public virtual Vector2 SnapTo(SnapPosition snapPosition, int offsetX, int offsetY)
        {
            Vector2 offset = new(offsetX, offsetY);
            return SnapTo(snapPosition, offset);
        }
    }
    public class PrimitiveElement : IElement
    {
        public override void Draw(Document document)
        {
            throw new NotImplementedException();
        }
    }
    public class GlyphElement : IElement
    {
        public string Glyph { get; set; } // Should be only one character, but SymbolMapping does not return a single char
        public GlyphElement(string glyph, Vector2 pos, Vector2 size)
        {
            Glyph = glyph;
            Position = pos;
            Size = size;
        }
        public override void Draw(Document document)
        {
            SKPaint paint = new()
            {
                TextSize = 24f, // TODO: Must be dynamic
                IsAntialias = true,
                Color = SKColors.Black,
                Typeface = document.MusicFont
            };

            document.Canvas.DrawText(Glyph, Position.Y, Position.Y, paint);
        }
    }
    public class TextElement : IElement
    {
        public string Text { get; set; }
        public TextElement(string text, Vector2 pos, Vector2 size)
        {
            Text = text;
            ElementType = ElementType.Text;
            Position = pos;
            Size = size;
        }
        public override void Draw(Document document)
        {
            SKPaint paint = new()
            {
                TextSize = 16f, // TODO: Must be dynamic
                IsAntialias = true,
                Color = SKColors.Black,
                Typeface = document.TextFont
            };

            document.Canvas.DrawText(Text, Position.Y, Position.Y, paint);
        }
    }
}