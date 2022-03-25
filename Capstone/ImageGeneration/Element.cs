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
        Bottom,
        TextLeft,
        TextRight,
        TextCenter
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
            return snapPosition switch
            {
                SnapPosition.TopLeft => (SKPoint)offset + Position,
                SnapPosition.TopRight => (SKPoint)offset + new SKPoint(Position.X + Size.X, Position.Y),
                SnapPosition.Top => (SKPoint)offset + new SKPoint(Position.X + (Size.X / 2), Position.Y),
                SnapPosition.MidLeft or SnapPosition.TextLeft => (SKPoint)offset + new SKPoint(Position.X, Position.Y + (Size.Y / 2)),
                SnapPosition.MidRight or SnapPosition.TextRight => (SKPoint)offset + new SKPoint(Position.X + Size.X, Position.Y + (Size.Y / 2)),
                SnapPosition.Center or SnapPosition.TextCenter => (SKPoint)offset + new SKPoint(Position.X + (Size.X / 2), Position.Y + (Size.Y / 2)),
                SnapPosition.BottomLeft => (SKPoint)offset + new SKPoint(Position.X, Position.Y + Size.Y),
                SnapPosition.Bottom => (SKPoint)offset + new SKPoint(Position.X + (Size.X / 2), Position.Y + Size.Y),
                SnapPosition.BottomRight => (SKPoint)offset + new SKPoint(Position.X + Size.X, Position.Y + Size.Y),
                _ => throw new ArgumentOutOfRangeException(paramName: nameof(snapPosition), message: "Unexpected offset given!") // Provide more information?
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
    // TODO: Builds unevenly around TextBounds, which defies the document margins, can this be fixed?
    public class TextElement : IElement
    {
        public string Text { get; set; }
        public SKRect TextBounds { get; set; }
        public TextElement(string text, SKPoint pos, Document document, float size = 16)
        {
            Text = text;
            ElementType = ElementType.Text;
            Position = pos;
            FontSize = size;
            Document = document;
            GetPaintAndTextSize();
        }
        public virtual void GetPaintAndTextSize()
        {
            Paint = Document.TextPaint.Clone();
            Paint.TextSize = FontSize;
            SKRect textBounds = new();
            Paint.MeasureText(Text, ref textBounds);
            TextBounds = textBounds;
            Size = new(Paint.MeasureText(Text), FontSize);
        }
        public override void Draw()
        {
            Document.Canvas.DrawText(Text, Position, Paint);
        }
        public override SKPoint SnapTo(SnapPosition snapPosition, SKPoint? offset = null)
        {
            if (offset == null)
            {
                offset = new(0, 0);
            }
            return snapPosition switch
            {
                SnapPosition.TopLeft => (SKPoint)offset + new SKPoint(Position.X + TextBounds.Left, Position.Y + TextBounds.Top),
                SnapPosition.TopRight => (SKPoint)offset + new SKPoint(Position.X + TextBounds.Right, Position.Y + TextBounds.Top),
                SnapPosition.Top => (SKPoint)offset + new SKPoint(Position.X + TextBounds.MidX, Position.Y + TextBounds.Top),
                SnapPosition.MidLeft => (SKPoint)offset + new SKPoint(Position.X + TextBounds.Left, Position.Y + TextBounds.MidY),
                SnapPosition.MidRight => (SKPoint)offset + new SKPoint(Position.X + TextBounds.Right, Position.Y + TextBounds.MidY),
                SnapPosition.Center => (SKPoint)offset + new SKPoint(Position.X + TextBounds.MidX, Position.Y + TextBounds.MidY),
                SnapPosition.BottomLeft => (SKPoint)offset + new SKPoint(Position.X + TextBounds.Left, Position.Y + TextBounds.Bottom),
                SnapPosition.Bottom => (SKPoint)offset + new SKPoint(Position.X + TextBounds.MidX, Position.Y + TextBounds.Bottom),
                SnapPosition.BottomRight => (SKPoint)offset + new SKPoint(Position.X + TextBounds.Right, Position.Y + TextBounds.Bottom),
                SnapPosition.TextLeft => (SKPoint)offset + new SKPoint(Position.X + TextBounds.Left, Position.Y),
                SnapPosition.TextRight => (SKPoint)offset + new SKPoint(Position.X + TextBounds.Right, Position.Y),
                SnapPosition.TextCenter => (SKPoint)offset + new SKPoint(Position.X + TextBounds.MidX, Position.Y),
                _ => throw new ArgumentOutOfRangeException(paramName: nameof(snapPosition), message: "Unexpected offset given!") // Provide more information?
            };
        }
    }
    public class GlyphElement : TextElement
    {
        public GlyphElement(string text, SKPoint pos, Document document, float size = 24) : base(text, pos, document, size)
        {
            // Should be handled in base constructor
        }
        public override void GetPaintAndTextSize()
        {
            Paint = Document.MusicPaint.Clone();
            Paint.TextSize = FontSize;
            SKRect textBounds = new();
            Paint.MeasureText(Text, ref textBounds);
            TextBounds = textBounds;
            Size = new(Paint.MeasureText(Text), FontSize);
        }
    }
}