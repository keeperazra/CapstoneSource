using SkiaSharp;

namespace ImageGeneration
{
    public class TimeSignature : IElement
    {
        private const string SigPrefix = "timeSig";
        private int upper;
        public int Upper
        {
            get => upper;
            set
            {
                if (value < 1 || value > 99)
                {
                    throw new ArgumentOutOfRangeException(nameof(upper), message: "Time Signature upper numeral must be between 1 and 99");
                }
                upper = value;
            }
        }
        private int lower;
        public int Lower
        {
            get => lower;
            set
            {
                if (value < 1 || value > 99)
                {
                    throw new ArgumentOutOfRangeException(nameof(lower), message: "Time Signature lower numeral must be between 1 and 99");
                }
                lower = value;
            }
        }
        public string glyphOverride;
        public TimeSignature(int upper, int lower, SKPoint pos, Document document, string glyphOverride = "", float size = 24)
        {
            Upper = upper;
            Lower = lower;
            Position = pos;
            Document = document;
            this.glyphOverride = glyphOverride;
            FontSize = size;
        }
        public override void Draw()
        {
            if (glyphOverride != "")
            {
                string uString = Upper.ToString();
                if (Upper > 9)
                {
                    GlyphElement leftNumeral = new(string.Concat(SigPrefix, uString.AsSpan(0, 1)), Position, Document, FontSize, true);
                    leftNumeral.Position = new(leftNumeral.Position.X - (leftNumeral.TextBounds.Width / 2), leftNumeral.Position.Y);
                    GlyphElement rightNumeral = new(string.Concat(SigPrefix, uString.AsSpan(1, 1)), Position, Document, FontSize, true);
                    rightNumeral.Position = new(rightNumeral.Position.X + (rightNumeral.TextBounds.Width / 2), rightNumeral.Position.Y);
                    leftNumeral.Draw();
                    rightNumeral.Draw();
                }
                else
                {
                    GlyphElement numeral = new(uString, Position, Document, FontSize, true);
                    numeral.Draw();
                }
                // TODO: Code deduplication
                string lString = Lower.ToString();
                if (Lower > 9)
                {
                    GlyphElement leftNumeral = new(string.Concat(SigPrefix, lString.AsSpan(0, 1)), Position, Document, FontSize, true);
                    leftNumeral.Position = new(leftNumeral.Position.X - (leftNumeral.TextBounds.Width / 2), leftNumeral.Position.Y);
                    GlyphElement rightNumeral = new(string.Concat(SigPrefix, lString.AsSpan(1, 1)), Position, Document, FontSize, true);
                    rightNumeral.Position = new(rightNumeral.Position.X + (rightNumeral.TextBounds.Width / 2), rightNumeral.Position.Y);
                    leftNumeral.Draw();
                    rightNumeral.Draw();
                }
                else
                {
                    GlyphElement numeral = new(lString, Position, Document, FontSize, true);
                    numeral.Draw();
                }
            }
            else
            {
                GlyphElement glyph = new(glyphOverride, Position, Document, FontSize, true);
                glyph.Position = new(glyph.Position.X, glyph.Position.Y + (glyph.TextBounds.Height / 2));
                glyph.Draw();
            }
        }
    }
}
