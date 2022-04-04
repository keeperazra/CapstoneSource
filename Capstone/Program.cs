using ArgHandling;
using ImageGeneration;

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
Document document = new("sample scales.png", new(800, 800));

Staff staff = new(document, null, new(), null);
int len = 4;
staff.notes.Add(new Note(len, -2, new(), document));
staff.notes.Add(new Note(len, -1.5f, new(), document));
staff.notes.Add(new Note(len, -1, new(), document));
staff.notes.Add(new Note(len, -0.5f, new(), document));
staff.notes.Add(new Note(len, 0, new(), document));
staff.notes.Add(new Note(len, 0.5f, new(), document));
staff.notes.Add(new Note(len, 1, new(), document));
staff.notes.Add(new Note(len, 1.5f, new(), document));
staff.notes.Add(new Note(len, 2, new(), document));

len = 8;
staff.notes.Add(new Note(len, -2, new(), document));
staff.notes.Add(new Note(len, -1.5f, new(), document));
staff.notes.Add(new Note(len, -1, new(), document));
staff.notes.Add(new Note(len, -0.5f, new(), document));
staff.notes.Add(new Note(len, 0, new(), document));
staff.notes.Add(new Note(len, 0.5f, new(), document));
staff.notes.Add(new Note(len, 1, new(), document));
staff.notes.Add(new Note(len, 1.5f, new(), document));
staff.notes.Add(new Note(len, 2, new(), document));

len = 1;
staff.notes.Add(new Note(len, -2, new(), document));
staff.notes.Add(new Note(len, -1.5f, new(), document));
staff.notes.Add(new Note(len, -1, new(), document));
staff.notes.Add(new Note(len, -0.5f, new(), document));
staff.notes.Add(new Note(len, 0, new(), document));
staff.notes.Add(new Note(len, 0.5f, new(), document));
staff.notes.Add(new Note(len, 1, new(), document));
staff.notes.Add(new Note(len, 1.5f, new(), document));
staff.notes.Add(new Note(len, 2, new(), document));

len = 2;
staff.notes.Add(new Note(len, -2, new(), document));
staff.notes.Add(new Note(len, -1.5f, new(), document));
staff.notes.Add(new Note(len, -1, new(), document));
staff.notes.Add(new Note(len, -0.5f, new(), document));
staff.notes.Add(new Note(len, 0, new(), document));
staff.notes.Add(new Note(len, 0.5f, new(), document));
staff.notes.Add(new Note(len, 1, new(), document));
staff.notes.Add(new Note(len, 1.5f, new(), document));
staff.notes.Add(new Note(len, 2, new(), document));
staff.Draw();

document.SaveFile();
