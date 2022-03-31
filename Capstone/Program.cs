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

///*SKPaint boxp = document.MusicPaint.Clone();
//boxp.Color = SKColors.Red;
//document.Canvas.DrawRect(document.Margins.X, document.Margins.Y, document.Margins.X + 10, document.Margins.Y + 10, boxp);*/
GlyphLookup glyphLookup = new();
//GlyphElement whole = new(glyphLookup.GetCharacter("noteheadWhole"), document.Margins, document);
////TextElement whole = new("This is some sample text", document.Margins, document);
//whole.Draw();
//SKPaint paint = document.MusicPaint.Clone();
//paint.Style = SKPaintStyle.Stroke;
//paint.StrokeWidth = 1;
//SKPath box = new();
//box.MoveTo(whole.SnapTo(SnapPosition.TopLeft));
//box.LineTo(whole.SnapTo(SnapPosition.TopRight));
//box.LineTo(whole.SnapTo(SnapPosition.BottomRight));
//box.LineTo(whole.SnapTo(SnapPosition.BottomLeft));
//box.LineTo(whole.SnapTo(SnapPosition.TopLeft));
//document.Canvas.DrawPath(box, paint);
//GlyphElement half = new(glyphLookup.GetCharacter("noteheadBlack"), new(200, 50), document);
////TextElement half = new("more", whole.SnapTo(SnapPosition.TextRight), document);
//half.Draw();
//GlyphElement tail = new(glyphLookup.GetCharacter("flag8thUp"), half.SnapTo(SnapPosition.TextRight, new(-1.75f, 0)), document);
//tail.Position = new(tail.Position.X, tail.Position.Y - tail.TextBounds.Height);
//tail.Draw();
//SKPath stem = new();
//stem.MoveTo(half.SnapTo(SnapPosition.TextRight, new(-1.5f, 0)));
//stem.LineTo(tail.SnapTo(SnapPosition.TextLeft));
//document.Canvas.DrawPath(stem, paint);

Note test1 = new("noteheadBlack", "flag8thUp", NoteDirection.Up, new(50, 50), document);
test1.Draw();
Note test2 = new("noteheadBlack", "flag8thDown", NoteDirection.Down, new(100, 50), document);
test2.Draw();
Note test3 = new("noteheadWhole", "none", NoteDirection.Up, new(150, 50), document);
test3.Draw();

GlyphElement sampleUp = new(glyphLookup.GetCharacter("note8thUp"), new(250, 50), document);
sampleUp.Draw();
GlyphElement sampleDown = new(glyphLookup.GetCharacter("note8thDown"), new(300, 50), document);
sampleDown.Draw();

document.SaveFile();

//Console.WriteLine(tail.Position.Y);
