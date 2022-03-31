using SkiaSharp;

namespace ImageGeneration
{
    public enum ExampleType
    {
        Squares,
        Notes,
        Scale
    }

    public class ExampleImage
    {
        public string filePath;
        public ExampleType type;

        public ExampleImage(string path = "demo.png", ExampleType type = ExampleType.Squares)
        {
            filePath = path;
            this.type = type;
        }

        /*
         * Public wrapper for rendering an example image.
         * Creates empty canvas with specified (or default 800x800) dimensions.
         * Calls private rendering functions based on type. Can throw error if type
         * is somehow not recognized.
         * Handles opening and saving file.
         */
        public void Render(int width = 800, int height = 800)
        {
            switch (type)
            {
                case ExampleType.Squares:
                    RenderSquares(width, height);
                    break;
                case ExampleType.Notes:
                    RenderNotes(width, height);
                    break;
                case ExampleType.Scale:
                    RenderScale(width, height);
                    break;
                default:
                    throw new ArgumentException("ExampleImage created with unknown type!");
            }
        }

        /*
         * Draws some squares.
         */
        private void RenderSquares(int width, int height)
        {
            SKImageInfo imageInfo = new(width, height);
            SKSurface surface = SKSurface.Create(imageInfo);
            SKCanvas canvas = surface.Canvas;
            float spaceX = (float)width / 16;
            float spaceY = (float)height / 16;
            float widthS = (float)width / 4;
            float heightS = (float)height / 4;
            canvas.DrawColor(SKColors.White);
            SKPaint paint = new()
            {
                TextSize = 64f,
                IsAntialias = true,
                Color = SKColors.Red,
            };
            canvas.DrawRect(0, 0, widthS, heightS, paint);
            paint.Color = SKColors.Green;
            canvas.DrawRect(spaceX, spaceY, widthS, heightS, paint);
            paint.Color = SKColors.Blue;
            canvas.DrawRect(spaceX * 2, spaceY * 2, widthS, heightS, paint);
            paint.Color = SKColors.Black;
            canvas.DrawRect(spaceX * 3, spaceY * 3, widthS, heightS, paint);
            SKImage cImage = surface.Snapshot();
            SKData data = cImage.Encode(SKEncodedImageFormat.Png, 80);
            using FileStream file = File.OpenWrite(filePath);
            data.SaveTo(file);
        }

        /*
         * Loads in a music font (SMuFL compliant) and draws some notes and text.
         */
        private void RenderNotes(int width, int height)
        {
            SKPointI dims = new(width, height);
            Document document = new(filePath, dims);
            string sampleText = "Sample text";
            TextElement sample = new(sampleText, document.Margins, document);
            sample.Draw();

            GlyphLookup glyphLookup = new();
            string sampleGlyph = glyphLookup.GetCharacter("note8thUp");
            GlyphElement glyph = new(sampleGlyph, sample.SnapTo(SnapPosition.Bottom, new SKPoint(0, 20)), document);
            glyph.Draw();

            document.SaveFile();
        }

        /*
         * TODO: Render a simple scale. Requires additional framework.
         */
        private void RenderScale(int width, int height)
        {
            throw new NotImplementedException(); // TODO
        }
    }
}