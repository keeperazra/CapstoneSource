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
        public float staffOffset;
        private int duration;
        public int Duration
        {
            get => duration;
            set
            {
                // TODO: Duration must actually be a power of two
                if (value <= 0 || (value != 1 && value % 2 != 0))
                {
                    throw new ArgumentOutOfRangeException(nameof(duration), message: "Note duration must either be 1 (whole) or a multiple of two");
                }
                duration = value;
            }
        }
        public bool dotted;
        public float ExpectedWidth
        {
            get
            {
                float size = 20;
                if (dotted)
                {
                    size += 5;
                }
                if (accidental != "") // TODO: different accidentals can be different widths
                {
                    size += 10;
                }
                return size;
            }
        }
        public string accidental; // TODO: Enforce value?
        public Note(int duration, float offset, SKPoint position, Document document, bool dot = false, string accidental = "", float fontSize = 24)
        {
            Duration = duration;
            staffOffset = offset;
            Direction = staffOffset < 0 ? NoteDirection.Up : NoteDirection.Down;
            Position = position;
            Document = document;
            dotted = dot;
            this.accidental = accidental;
            FontSize = fontSize;
        }
        public override void Draw()
        {
            SKPoint originalPosition = Position;
            if (accidental != null && accidental != "")
            {
                GlyphElement accidentalElement = new(accidental, Position, Document, FontSize, true);
                accidentalElement.Draw();
                Position = new(originalPosition.X + 10, originalPosition.Y);
            }

            if (Duration == 1)
            {
                Head = new("noteheadWhole", Position, Document, FontSize, true);
            }
            else if (Duration == 2 || Duration == 4)
            {
                Head = Duration == 2 ? new("noteheadHalf", Position, Document, FontSize, true) : new("noteheadBlack", Position, Document, FontSize, true);
                Stem = new();
                if (Direction == NoteDirection.Up)
                {
                    Stem.MoveTo(Head.SnapTo(SnapPosition.TextRight, new(-1f, 0)));
                    Stem.LineTo(Head.SnapTo(SnapPosition.TextRight, new(-1f, -20f)));
                }
                else
                {
                    Stem.MoveTo(Head.SnapTo(SnapPosition.TextLeft, new(1f, 0)));
                    Stem.LineTo(Head.SnapTo(SnapPosition.TextLeft, new(1f, 20f)));
                }
            }
            else
            {
                Head = new("noteheadBlack", Position, Document, FontSize, true);
                Stem = new();
                string tailType = string.Format("flag{0}th", Duration);
                if (Direction == NoteDirection.Up)
                {
                    tailType += "Up";
                    Tail = new(tailType, Head.SnapTo(SnapPosition.TextRight, new(-1f, 0)), Document, lookupGlyph: true);
                    Tail.Position = new(Tail.Position.X, Tail.Position.Y - Tail.TextBounds.Height);
                    Stem.MoveTo(Head.SnapTo(SnapPosition.TextRight, new(-1f, 0)));
                    Stem.LineTo(Tail.SnapTo(SnapPosition.TextLeft));
                }
                else if (Direction == NoteDirection.Down)
                {
                    tailType += "Down";
                    Tail = new(tailType, Head.SnapTo(SnapPosition.TextLeft, new(1f, 0)), Document, lookupGlyph: true);
                    Tail.Position = new(Tail.Position.X, Tail.Position.Y + Tail.TextBounds.Height);
                    Stem.MoveTo(Head.SnapTo(SnapPosition.TextLeft, new(1f, 0)));
                    Stem.LineTo(Tail.SnapTo(SnapPosition.TextLeft));
                }
            }

            Head.Draw();
            if (Stem != null)
            {
                SKPaint stemPaint = Document.MusicPaint.Clone();
                stemPaint.Style = SKPaintStyle.Stroke;
                stemPaint.StrokeWidth = 1;
                Document.Canvas.DrawPath(Stem, stemPaint);
            }
            if (Tail != null)
            {
                Tail.Draw();
            }

            if (dotted)
            {
                float targetX = Position.X;
                float targetY = Position.Y;
                targetX += 12; // wacky
                if (staffOffset == Math.Floor(staffOffset))
                {
                    if (staffOffset < 0)
                    {
                        targetY -= Staff.lineSep / 2;
                    }
                    else
                    {
                        targetY += Staff.lineSep / 2;
                    }
                }
                Document.Canvas.DrawCircle(targetX, targetY, 1.5f, Document.MusicPaint);
            }

            Position = originalPosition; // Restore position if changed
        }
    }
}