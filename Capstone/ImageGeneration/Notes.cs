using SkiaSharp;

namespace ImageGeneration
{
    public enum NoteDirection
    {
        Up,
        Down,
    }
    // TODO: Implement size and snap positions
    public class Note : IElement
    {
        public GlyphElement Head { get; set; }
        public GlyphElement Tail { get; set; }
        public SKPath Stem { get; set; }
        public NoteDirection Direction { get; set; }
        public Note(string headType, string tailType, NoteDirection direction, SKPoint position, Document document)
        {
            Document = document;
            Position = position;
            Head = new(headType, position, document, lookupGlyph: true);
            // TODO: These snap positions (and the thickness of the stem) will have to depend on the font size the note is drawn with
            if(tailType != "none")
            {
                Stem = new();
                if(direction == NoteDirection.Up)
                {
                    Tail = new(tailType, Head.SnapTo(SnapPosition.TextRight, new(-1f, 0)), document, lookupGlyph: true);
                    Tail.Position = new(Tail.Position.X, Tail.Position.Y - Tail.TextBounds.Height);
                    Stem.MoveTo(Head.SnapTo(SnapPosition.TextRight, new(-1f, 0)));
                    Stem.LineTo(Tail.SnapTo(SnapPosition.TextLeft));
                } else if(direction == NoteDirection.Down)
                {
                    Tail = new(tailType, Head.SnapTo(SnapPosition.TextLeft, new(1f, 0)), document, lookupGlyph: true);
                    Tail.Position = new(Tail.Position.X, Tail.Position.Y + Tail.TextBounds.Height);
                    Stem.MoveTo(Head.SnapTo(SnapPosition.TextLeft, new(1f, 0)));
                    Stem.LineTo(Tail.SnapTo(SnapPosition.TextLeft));
                }
            }
        }
        public override void Draw()
        {
            Head.Draw();
            if(Tail != null)
            {
                SKPaint stemPaint = Document.MusicPaint.Clone();
                stemPaint.Style = SKPaintStyle.Stroke;
                stemPaint.StrokeWidth = 1;
                Document.Canvas.DrawPath(Stem, stemPaint);
                Tail.Draw();
            }
        }
    }
}