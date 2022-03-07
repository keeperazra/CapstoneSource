using ArgHandling;
using ImageGeneration;
using SkiaSharp;

ArgumentParser parser = new();
parser.AddArgument("demo", "d", "demofile", help: "Path to demo file output");

Dictionary<string, string> kwargs;
try
{
    kwargs = parser.Parse(args);
}
catch (ArgumentException ex)
{
    Console.Error.WriteLine(ex.Message);
    parser.PrintHelp();
    return;
}

if (kwargs.ContainsKey("help"))
{
    parser.PrintHelp();
    return;
}

ExampleImage demo;

if (kwargs.ContainsKey("demofile"))
{
    demo = new(kwargs["demofile"]);
}
else
{
    demo = new();
}
/*
demo.Render(); // Default demo

demo = new("demo notes.png", ExampleType.Notes);
demo.Render(); // Notes demo
*/
Document document = new("note boxes.png", new(800, 800));
SKPaint boxp = document.MusicPaint.Clone();
boxp.Color = SKColors.Red;
document.Canvas.DrawRect(document.Margins.X, document.Margins.Y, document.Margins.X + 10, document.Margins.Y + 10, boxp);
GlyphLookup glyphLookup = new();
glyphLookup.LoadGlyphMap();
//GlyphElement whole = new(glyphLookup.GetCharacter("noteheadWhole"), document.Margins, document);
TextElement whole = new("This is some sample text", document.Margins, document);
whole.Draw();
SKPaint paint = document.MusicPaint.Clone();
paint.Style = SKPaintStyle.Stroke;
paint.StrokeWidth = 1;
SKPath box = new();
box.MoveTo(whole.SnapTo(SnapPosition.TopLeft));
box.LineTo(whole.SnapTo(SnapPosition.TopRight));
box.LineTo(whole.SnapTo(SnapPosition.BottomRight));
box.LineTo(whole.SnapTo(SnapPosition.BottomLeft));
box.LineTo(whole.SnapTo(SnapPosition.TopLeft));
document.Canvas.DrawPath(box, paint);
//GlyphElement half = new(glyphLookup.GetCharacter("noteheadHalf"), whole.SnapTo(SnapPosition.MidRight), document);
TextElement half = new("more", whole.SnapTo(SnapPosition.MidRight), document);
half.Draw();
document.SaveFile();

Console.WriteLine(whole.SnapTo(SnapPosition.TopLeft).Y);
