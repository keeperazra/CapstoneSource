using SkiaSharp;
using System.Reflection;

namespace ImageGeneration
{
    /*
     * Controls document size and layout as well as global fonts
     */
    public class Document
    {
        public SKPointI Dimensions { get; set; }
        public SKPoint Margins { get; set; }
        public SKTypeface MusicFont { get; set; }
        public SKPaint MusicPaint { get; set; }
        public SKTypeface TextFont { get; set; }
        public SKPaint TextPaint { get; set; }
        public SKSurface Surface { get; set; }
        public SKCanvas Canvas { get; set; }
        public string filePath;
        public Document(string file, SKPointI dimensions, SKPoint? margins = null)
        {
            filePath = file;
            Dimensions = dimensions;
            if (margins != null)
            {
                Margins = (SKPoint)margins;
            }
            else
            {
                Margins = new(50, 50);
            }
            LoadFontDefaults();
            LoadCanvas();
        }
        private void LoadFontDefaults()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            using Stream assemblyStream = assembly.GetManifestResourceStream("Capstone.Resources.Fonts.Bravura.otf");
            if (assemblyStream == null)
            {
                throw new FileNotFoundException("Could not find Resources\\Fonts\\Bravura.otf");
            }
            MusicFont = SKTypeface.FromStream(assemblyStream);
            MusicPaint = new()
            {
                TextSize = 24f, // TODO: Must be dynamic
                IsAntialias = true,
                Color = SKColors.Black,
                Typeface = MusicFont
            };
            TextFont = SKTypeface.Default; // TODO Pick/package a better option?
            TextPaint = new()
            {
                TextSize = 16f, // TODO: Must be dynamic
                IsAntialias = true,
                Color = SKColors.Black,
                Typeface = TextFont
            };
        }
        private void LoadCanvas()
        {
            SKImageInfo imgInfo = new(Dimensions.X, Dimensions.Y);
            Surface = SKSurface.Create(imgInfo);
            Canvas = Surface.Canvas;
            Canvas.DrawColor(SKColors.White); // TODO: Change default color?
        }
        public void SaveFile()
        {
            SKImage cImage = Surface.Snapshot();
            SKData data = cImage.Encode(SKEncodedImageFormat.Png, 80);
            using FileStream file = File.OpenWrite(filePath);
            data.SaveTo(file);
        }
    }
}