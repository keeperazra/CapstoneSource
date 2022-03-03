using SkiaSharp;
using System.Numerics;
using System.Reflection;

namespace ImageGeneration
{
    /*
     * Controls document size and layout as well as global fonts
     */
    public class Document
    {
        public Vector2 Dimensions { get; set; } // TODO: Force int?
        public Vector2 Margins { get; set; } // TODO: Force int?
        public SKTypeface MusicFont { get; set; }
        public SKTypeface TextFont { get; set; }
        public SKSurface Surface { get; set; }
        public SKCanvas Canvas { get; set; }
        public string filePath;
        public Document(string file, Vector2 dimensions, Vector2? margins = null)
        {
            filePath = file;
            this.Dimensions = dimensions;
            if (margins != null)
            {
                this.Margins = (Vector2)margins;
            }
            else
            {
                this.Margins = new(50, 50);
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
            SKImageInfo imgInfo = new((int)Dimensions.X, (int)Dimensions.Y);
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