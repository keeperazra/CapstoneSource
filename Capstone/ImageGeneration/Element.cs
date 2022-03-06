using SkiaSharp;

namespace ImageGeneration
{
    public enum ElementType
    {
        Primitive,
        Glyph,
        Text
    } // Remove?
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
        public Document Document { get; set; }
        public float FontSize { get; set; }
        public SKPaint Paint { get; set; }
        public abstract void Draw();
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
        public override void Draw()
        {
            throw new NotImplementedException();
        }
    }
    public class GlyphElement : IElement
    {
        public string Glyph { get; set; } // Should be only one character, but SymbolMapping returns a string
        public GlyphElement(string glyph, SKPoint pos, Document document, float size = 24)
        {
            ElementType = ElementType.Glyph;
            Glyph = glyph;
            Position = pos;
            FontSize = size;
            Document = document;
            GetPaintAndTextSize();
        }
        public void GetPaintAndTextSize()
        {
            Paint = Document.MusicPaint.Clone();
            Paint.TextSize = FontSize;
            Size = new(Paint.MeasureText(Glyph), FontSize);
        }
        public override void Draw()
        {
            Document.Canvas.DrawText(Glyph, Position, Paint);
        }
    }
    public class TextElement : IElement
    {
        public string Text { get; set; }
        public TextElement(string text, SKPoint pos, Document document, float size = 16)
        {
            Text = text;
            ElementType = ElementType.Text;
            Position = pos;
            FontSize = size;
            Document = document;
            GetPaintAndTextSize();
        }
        public void GetPaintAndTextSize()
        {
            Paint = Document.TextPaint.Clone();
            Paint.TextSize = FontSize;
            Size = new(Paint.MeasureText(Text), FontSize);
        }
        public override void Draw()
        {
            Document.Canvas.DrawText(Text, Position, Paint);
        }
    }
}