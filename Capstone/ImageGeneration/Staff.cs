using SkiaSharp;
using System.Collections;

namespace ImageGeneration
{
    public class Staff
    {
        public TimeSignature TimeSignature { get; set; }
        public ArrayList notes;
        public ArrayList measuredNotes;
        public GlyphElement Clef { get; set; }
        public Document Document { get; set; }
        public const float lineSep = 6;
        public Staff(Document document, TimeSignature timeSignature = null, ArrayList startingNotes = null, GlyphElement clef = null)
        {
            Document = document;
            TimeSignature = timeSignature;
            notes = startingNotes;
            Clef = clef;
            MeasureNotes();
        }
        public void Draw()
        {
            // TODO: Notes may be drawn above the staff, making this finicky. Good thing we've not implemented that yet.
            if (Document.Dimensions.X - ((Document.Margins.X * 2) + 50) <= 0
                || Document.Dimensions.Y - ((Document.Margins.Y * 2) + 50) == 0)
            {
                throw new Exception("The provided document is too small to draw a staff on!");
            }
            float currentX = Document.Margins.X;
            float MAXX = Document.Dimensions.X - Document.Margins.X;
            float centerlineY = Document.Margins.Y + 1 + (lineSep * 3);
            SKPaint linePaint = Document.MusicPaint.Clone();
            linePaint.Style = SKPaintStyle.Stroke;
            linePaint.StrokeWidth = 1;
            foreach (int i in Enumerable.Range(-2, 5))
            {
                SKPath staffLine = new();
                staffLine.MoveTo(currentX, OffsetFromCenterline(centerlineY, i));
                staffLine.LineTo(MAXX, OffsetFromCenterline(centerlineY, i));
                Document.Canvas.DrawPath(staffLine, linePaint);
            }
            if (Clef != null)
            {
                currentX += 10;
                Clef.Position = new(currentX, centerlineY);
                Clef.Draw();
                currentX += 10;
            }
            if (TimeSignature != null)
            {
                currentX += 20;
                TimeSignature.Position = new(currentX, centerlineY);
                TimeSignature.Draw();
                // TODO: Draw from measuredNotes
            }
            else
            {
                currentX += 20;
                foreach (var n in notes)
                {
                    if (currentX == Document.Margins.X)
                    {
                        currentX += 20;
                    }
                    Note note = (Note)n;
                    if (note != null)
                    {
                        note.Position = new(currentX, OffsetFromCenterline(centerlineY, note.staffOffset));
                        note.Draw();
                        currentX += note.ExpectedWidth;
                    }
                    if (currentX >= MAXX)
                    {
                        centerlineY = OffsetFromCenterline(centerlineY, -7);
                        if (OffsetFromCenterline(centerlineY, -2) >= Document.Dimensions.Y - Document.Margins.Y)
                        {
                            Console.Error.WriteLine("ERROR: Ran out of room in document for all the notes!");
                            break;
                        }
                        currentX = Document.Margins.X;
                        foreach (int i in Enumerable.Range(-2, 5))
                        {
                            SKPath staffLine = new();
                            staffLine.MoveTo(currentX, OffsetFromCenterline(centerlineY, i));
                            staffLine.LineTo(MAXX, OffsetFromCenterline(centerlineY, i));
                            Document.Canvas.DrawPath(staffLine, linePaint);
                        }
                    }
                }
            }
        }
        private float OffsetFromCenterline(float centerY, float offset)
        {
            return centerY - (offset * lineSep);
        }
        private void MeasureNotes()
        {
            if (TimeSignature == null || notes == null)
            {
                return;
            }
            measuredNotes = new();
            int mcount = 0;
            int inote = 0;
            ArrayList measure = new();
            int maxDuration = TimeSignature.Upper;
            int durationType = TimeSignature.Lower;
            float currentDuration = 0;
            while (inote < notes.Count)
            {
                Note current = (Note)notes[inote];
                if (current == null)
                {
                    continue; // Shouldn't happen, but keeps the IDE happy
                }
                measure.Add(current);
                // TODO: possible floating point errors?
                currentDuration += durationType / current.Duration;
                if (current.Duration >= maxDuration)
                {
                    measuredNotes.Add(measure);
                    mcount++;
                    measure = new();
                    if (current.Duration > maxDuration)
                    {
                        Console.WriteLine("WARNING: measure number " + mcount + " is overfull. Check your note durations!");
                    }
                }
                inote++;
            }
            if (measure.Count > 0)
            {
                measuredNotes.Add(measure);
                mcount++;
                Console.WriteLine("WARNING: measure number " + mcount + " is undefull. Check your note durations!");
            }
        }
    }
}
