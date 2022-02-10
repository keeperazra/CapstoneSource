using SkiaSharp;
using System.Numerics;

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
                    RenderNotes();
                    break;
                case ExampleType.Scale:
                    RenderScale();
                    break;
                default:
                    throw new ArgumentException("ExampleImage created with unknown type!");
            }
            SKImage cImage = surface.Snapshot();
            SKData data = cImage.Encode(SKEncodedImageFormat.Png, 80);
            using FileStream file = File.OpenWrite(filePath);
            data.SaveTo(file);
        }

        private static void RenderSquares(SKCanvas canvas, int width, int height)
        {
            float spaceX = (float)width / 16;
            float spaceY = (float)height / 16;
            float widthS = (float)width / 4;
            float heightS = (float)height / 4;
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

        private void RenderNotes()
        {
            throw new NotImplementedException(); // TODO
        }

        private void RenderScale()
        {
            throw new NotImplementedException(); // TODO
        }
    }
}