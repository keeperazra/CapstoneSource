using SkiaSharp;
using System.Reflection;

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
         * Creates empty canvas with specified (or default 256x256) dimensions.
         * Calls private rendering functions based on type. Can throw error if type
         * is somehow not recognized.
         * Handles opening and saving file.
         */
        public void Render(int width = 256, int height = 256)
        {
            SKImageInfo imageInfo = new(width, height);
            SKSurface surface = SKSurface.Create(imageInfo);
            SKCanvas canvas = surface.Canvas;
            switch (type)
            {
                case ExampleType.Squares:
                    RenderSquares(canvas, width, height);
                    break;
                case ExampleType.Notes:
                    RenderNotes(canvas, width, height);
                    break;
                case ExampleType.Scale:
                    RenderScale(canvas, width, height);
                    break;
                default:
                    throw new ArgumentException("ExampleImage created with unknown type!");
            }
            SKImage cImage = surface.Snapshot();
            SKData data = cImage.Encode(SKEncodedImageFormat.Png, 80);
            using FileStream file = File.OpenWrite(filePath);
            data.SaveTo(file);
        }

        /*
         * Draws some squares.
         */
        private static void RenderSquares(SKCanvas canvas, int width, int height)
        {
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
        }

        /*
         * Loads in a music font (SMuFL compliant) and draws some notes and text.
         */
        private void RenderNotes(SKCanvas canvas, int width, int height)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            using Stream assemblyStream = assembly.GetManifestResourceStream("Capstone.Resources.Fonts.Bravura.otf");
            if (assemblyStream == null)
            {
                throw new FileNotFoundException("Could not find Resources\\Fonts\\Bravura.otf");
            }
            SKTypeface bravura = SKTypeface.FromStream(assemblyStream);

            canvas.DrawColor(SKColors.White);

            SKPaint paint = new()
            {
                TextSize = 32f,
                IsAntialias = true,
                Color = SKColors.Black,
                Typeface = bravura,
            };

            canvas.DrawText("\uE1D7", width / 2f, height / 2f, paint); // Draw eighth note in middle
            paint.Typeface = SKTypeface.Default; // Bravura cannot be used to render text apparently, need some other font?
            canvas.DrawText("Sample text", 42f, 40f, paint);
        }

        /*
         * TODO: Render a simple scale. Requires additional framework.
         */
        private void RenderScale(SKCanvas canvas, int width, int height)
        {
            throw new NotImplementedException(); // TODO
        }
    }
}