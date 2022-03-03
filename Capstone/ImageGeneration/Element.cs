using SkiaSharp;

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
        public SKPoint Position { get; set; }
        public SKPoint Size { get; set; }
        public abstract void Draw(Document document);
        public virtual SKPoint SnapTo(SnapPosition snapPosition, SKPoint? offset = null)
        {
            if (offset == null)
            {
                offset = new(0, 0);
            }
            Position += (SKPoint)offset;
            return snapPosition switch
            {
                SnapPosition.TopLeft => Position,
                SnapPosition.TopRight => new SKPoint(Position.X + Size.X, Position.Y),
                SnapPosition.Top => new SKPoint(Position.X + (Size.X / 2), Position.Y),
                SnapPosition.MidLeft => new SKPoint(Position.X, Position.Y + (Size.Y / 2)),
                SnapPosition.MidRight => new SKPoint(Position.X + Size.X, Position.Y + (Size.Y / 2)),
                SnapPosition.Center => new SKPoint(Position.X + (Size.X / 2), Position.Y + (Size.Y / 2)),
                SnapPosition.BottomLeft => new SKPoint(Position.X, Position.Y + Size.Y),
                SnapPosition.Bottom => new SKPoint(Position.X + (Size.X / 2), Position.Y + Size.Y),
                SnapPosition.BottomRight => new SKPoint(Position.X + Size.X, Position.Y + Size.Y),
                _ => new SKPoint(0, 0), // Maybe point this somewhere else?
            };
        }
        public virtual SKPoint SnapTo(SnapPosition snapPosition, int offsetX, int offsetY)
        {
            SKPoint offset = new(offsetX, offsetY);
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
        public GlyphElement(string glyph, SKPoint pos, SKPoint size)
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

            document.Canvas.DrawText(Glyph, Position, paint);
        }
    }
    public class TextElement : IElement
    {
        public string Text { get; set; }
        public TextElement(string text, SKPoint pos, SKPoint size)
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

            document.Canvas.DrawText(Text, Position, paint);
        }
    }
}