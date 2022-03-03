using SkiaSharp;
using System.Reflection;

namespace ImageGeneration
{
    /*
     * Controls document size and layout as well as global fonts
     */
    public class Document
    {
        public SKPointI Dimensions { get; set; } // TODO: Force int?
        public SKPoint Margins { get; set; } // TODO: Force int?
        public SKTypeface MusicFont { get; set; }
        public SKTypeface TextFont { get; set; }
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
            TextFont = SKTypeface.Default; // TODO Pick/package a better option?
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