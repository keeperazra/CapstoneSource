using ArgHandling;
using ImageGeneration;
using TextParsing;

ArgumentParser argParser = new();
argParser.AddArgument("file", "f", "file", help: "Path to file to parse (required)");
argParser.AddArgument("output", "o", "output", help: "Path to output file (required)");

Dictionary<string, string> kwargs;
try
{
    kwargs = argParser.Parse(args);
}
catch (ArgumentException ex)
{
    Console.Error.WriteLine(ex.Message);
    argParser.PrintHelp();
    return;
}

if (kwargs.ContainsKey("help"))
{
    argParser.PrintHelp();
    return;
}
if (!kwargs.ContainsKey("file"))
{
    Console.Error.WriteLine("Missing required parameter --file!");
    argParser.PrintHelp();
    return;
}
if (!kwargs.ContainsKey("output"))
{
    Console.Error.WriteLine("Missing required parameter --output!");
    argParser.PrintHelp();
    return;
}

string filePath = kwargs["file"];
if (!File.Exists(filePath))
{
    Console.Error.WriteLine("File \"" + filePath + "\" does not exist!");
    return;
}
string outPath = kwargs["output"];
if (File.Exists(outPath))
{
    Console.WriteLine("Warning! Output file \"" + outPath + "\" already exists and will be overwritten!");
}

Document document = new(outPath, new(800, 800));

FileParser parser = new(filePath);
TimeSignature ts = null;
if (parser.timeUpper > 0 && parser.timeLower > 0)
{
    ts = new(parser.timeUpper, parser.timeLower, new(), document);
}
Staff staff = new(document, ts, new(), new(parser.clef, new(), document, lookupGlyph: true));
Note note;
while ((note = parser.ReadNote(document)) != null)
{
    staff.notes.Add(note);
}
staff.Draw();

document.SaveFile();