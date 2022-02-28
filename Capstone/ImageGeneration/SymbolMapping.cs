using System.Reflection;
using System.Text.Json;

namespace ImageGeneration
{
    public class GlyphLookup
    {
        public GlyphNames GlyphNames { get; set; }

        public void LoadGlyphMap()
        {
            JsonSerializerOptions options = new()
            {
                PropertyNameCaseInsensitive = true
            };
            Assembly assembly = Assembly.GetExecutingAssembly();
            using Stream assemblyStream = assembly.GetManifestResourceStream("Capstone.Resources.Fonts.glyphnames.json");
            if (assemblyStream == null)
            {
                throw new FileNotFoundException("Could not find Resources\\Fonts\\glyphnames.json");
            }
            using StreamReader reader = new(assemblyStream);
            string json = reader.ReadToEnd();
            GlyphNames = JsonSerializer.Deserialize<GlyphNames>(json, options);
        }

        public string GetCharacter(string glyph)
        {
            if(GlyphNames.ContainsKey(glyph))
            {
                string codepoint = GlyphNames[glyph].Codepoint;
                return char.ConvertFromUtf32(int.Parse(codepoint[(codepoint.IndexOf('+') + 1)..], style: System.Globalization.NumberStyles.HexNumber));
            }
            return "";
        }
    }
    public class GlyphNames : Dictionary<string, GlyphData> { };
    public class GlyphData
    {
        public string Codepoint { get; set; }
        public string Description { get; set; }
        public string AlternateCodepoint { get; set; }
    }
}
