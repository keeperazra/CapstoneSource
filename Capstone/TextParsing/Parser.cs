using ImageGeneration;
using System.Collections;
using System.Text.RegularExpressions;

namespace TextParsing
{
    public enum NoteMode
    {
        Absolute,
        Relative,
        Fixed
    }
    public class FileParser
    {
        private readonly StreamReader _stream;
        public readonly string filePath;
        public string clef;
        public string clefName;
        public int timeUpper;
        public int timeLower;
        private NoteMode mode;
        private Dictionary<string, object> settings;
        private ArrayList oldModes;

        public FileParser(string path)
        {
            filePath = path;
            LoadGlobals();
            _stream = new(filePath);
            mode = NoteMode.Absolute;
            settings = new();
            oldModes = new();
        }
        private void LoadGlobals()
        {
            using StreamReader stream = new(filePath); // The _stream field is used exclusively in ReadNote()
            while (stream.Peek() >= 0)
            {
                string line = stream.ReadLine();
                if (line == null)
                {
                    break;
                }

                if (clef == null)
                {
                    int index = line.IndexOf("\\clef");
                    if (index >= 0)
                    {
                        string temp = line[(index + 6)..].Split(' ')[0].Trim('"');
                        clef = CleffToGlyphName(temp);
                        clefName = temp;
                    }
                }

                if (timeUpper == 0 || timeLower == 0)
                {
                    int index = line.IndexOf("\\time");
                    if (index >= 0)
                    {
                        string temp = line[(index + 6)..].Split(' ')[0].Trim('"');
                        string[] parts = temp.Split('/');
                        if (parts.Length != 2)
                        {
                            Console.WriteLine("Warning! Bad time signature '" + temp + "'");
                            continue;
                        }
                        timeUpper = int.Parse(parts[0]);
                        timeLower = int.Parse(parts[1]);
                    }
                }
            }
        }
        public Note ReadNote(Document document)
        {
        retry:
            if (_stream.EndOfStream)
            {
                return null;
            }
            string readStr = "";
            bool controlSequence = false;
            while (true)
            {
                char next = (char)_stream.Read();
                if (char.IsLetterOrDigit(next) || next is '\'' or ',' or '.')
                {
                    readStr += next;
                }
                else if (readStr == "" && next == '\\')
                {
                    readStr += next;
                    controlSequence = true;
                }
                else if (next == '}')
                {
                    if (oldModes.Count > 0)
                    {
#pragma warning disable CS8605 // Unboxing a possibly null value.
                        mode = (NoteMode)oldModes[^1];
#pragma warning restore CS8605 // Unboxing a possibly null value.
                        oldModes.RemoveAt(oldModes.Count - 1);
                    }
                    else
                    {
                        mode = NoteMode.Absolute; // Default
                    }
                }
                else
                {
                    break; // Break also happens at end of stream
                }
            }
            if (controlSequence && readStr is "\\relative" or "\\fixed")
            {
                int next;
                if (readStr == "\\fixed")
                {
                    next = ReadIfChar("abcdefg");
                    if (next == -1)
                    {
                        Console.WriteLine("Bad control sequence \"" + readStr + "\"");
                        goto retry; // TODO: soo many of these checks. Are they necessary?
                    }

                    string fixedOctave = "";
                    while ((next = ReadIfChar("',")) >= 0)
                    {
                        fixedOctave += (char)next;
                    }

                    next = ReadIfChar(' ');
                    if (next == -1)
                    {
                        Console.WriteLine("Bad control sequence \"" + readStr + "\"");
                        goto retry; // TODO: soo many of these checks. Are they necessary?
                    }

                    settings["fixedOctave"] = fixedOctave;
                    oldModes.Add(mode);
                    mode = NoteMode.Fixed;
                }
                else if (readStr == "\\relative")
                {
                    oldModes.Add(mode);
                    mode = NoteMode.Relative;
                }

                next = ReadIfChar('{');
                if (next == -1)
                {
                    Console.WriteLine("Bad control sequence \"" + readStr + "\"");
                    goto retry; // TODO: soo many of these checks. Are they necessary?
                }
                goto retry; // Control sequence has set settings, now go back and look for notes!
            }
            Note n = NoteStringToNoteObj(readStr, document);
            if (n != null)
            {
                return n;
            }
            goto retry; // Unknown string, read for next note
        }
        private int ReadIfChar(char c)
        {
            return (char)_stream.Peek() != c ? -1 : _stream.Read();
        }
        private int ReadIfChar(string cs)
        {
            return !cs.Contains((char)_stream.Peek()) ? -1 : _stream.Read();
        }
        private static string CleffToGlyphName(string clef)
        {
            return clef switch
            {
                "treble" => "gClef",
                "alto" or "tenor" => "cClef",
                "bass" => "fClef",
                _ => throw new ArgumentOutOfRangeException(nameof(clef), "Clef '" + clef + "' is not in expected values!")
            };
        }
        private Note NoteStringToNoteObj(string noteStr, Document document)
        {
            Regex rx = new(@"^(?<pitch>[a-g])(?<accidental>(is|es){1,2})?(?<octave>[',]{1,2})?(?<duration>\d+)?(?<dot>\.)?$");
            Match m = rx.Match(noteStr);
            if (!m.Success)
            {
                return null;
            }
            GroupCollection group = m.Groups;

            string pitch = group["pitch"].Value;

            string accidental = "";
            if (group.ContainsKey("accidental") && group["accidental"].Success)
            {
                accidental = group["accidental"].Value;
            }

            string octave = null;
            if (group.ContainsKey("octave") && group["octave"].Success)
            {
                octave = group["octave"].Value;
                settings["octave"] = octave;
            }
            else if (mode != NoteMode.Relative && settings.ContainsKey("octave"))
            {
                octave = (string)settings["octave"];
            }

            string duration;
            if (group.ContainsKey("duration") && group["duration"].Success)
            {
                duration = group["duration"].Value;
                settings["duration"] = duration;
            }
            else if (settings.ContainsKey("duration"))
            {
                duration = (string)settings["duration"];
            }
            else
            {
                throw new FormatException("Note duration must be specified for at least the first note.");
            }

            bool dot = false;
            if (group.ContainsKey("dot") && group["dot"].Success)
            {
                dot = true;
            }

            float offset = mode switch
            {
                NoteMode.Absolute => CalculateOffsetAbsolute(pitch, octave),
                NoteMode.Relative => CalculateOffsetRelative(pitch, octave),
                NoteMode.Fixed => CalculateOffsetFixed(pitch, octave),
                _ => CalculateOffsetAbsolute(pitch, octave)
            };

            settings["previousPitch"] = pitch;
            settings["lastOffset"] = offset;

            string accidentalGlyph = AccidentalToGlyphName(accidental);
            return new(int.Parse(duration), offset, new(), document, dot, accidentalGlyph);
        }
        private static string AccidentalToGlyphName(string accidental)
        {
            return accidental switch
            {
                "is" => "accidentalSharp",
                "isis" => "accidentalDoubleSharp",
                "es" => "accidentalFlat",
                "eses" => "accidentalDoubleFlat",
                _ => null // TODO: Is this good?
            };
        }
        private float ClefOffset()
        {
            return clefName switch
            {
                "treble" => -3,
                "alto" => 0,
                "tenor" => 1,
                "bass" => 3,
                _ => 0,
            };
        }
        private float CalculateOffsetAbsolute(string pitch, string octave)
        {
            float offsetC = ClefOffset();
            float pitchOffset = pitch switch
            {
                "a" => -1,
                "b" => -0.5f,
                "c" => 0,
                "d" => 0.5f,
                "e" => 1,
                "f" => 1.5f,
                "g" => 2,
                _ => throw new ArgumentException("\"" + pitch + "\" is not a valid pitch name!")
            };
            float octaveOffset = octave switch
            {
                "'" => 0,
                "''" => 4,
                "'''" => 8,
                "," => -4,
                ",," => -8,
                _ => 0
            };

            return offsetC + pitchOffset + octaveOffset;
        }
        private float CalculateOffsetRelative(string pitch, string octave)
        {
            if (!settings.ContainsKey("previousPitch") || (octave != null && octave != ""))
            {
                return CalculateOffsetAbsolute(pitch, octave);
            }

            float lastOffset = (float)settings["lastOffset"];
            string previousPitch = (string)settings["previousPitch"];
            float pitchOffset = pitch switch
            {
                "a" when previousPitch == "a" => 0,
                "b" when previousPitch == "b" => 0,
                "c" when previousPitch == "c" => 0,
                "d" when previousPitch == "d" => 0,
                "e" when previousPitch == "e" => 0,
                "f" when previousPitch == "f" => 0,
                "g" when previousPitch == "g" => 0,
                "a" when previousPitch == "b" => -0.5f,
                "b" when previousPitch == "c" => -0.5f,
                "c" when previousPitch == "d" => -0.5f,
                "d" when previousPitch == "e" => -0.5f,
                "e" when previousPitch == "f" => -0.5f,
                "f" when previousPitch == "g" => -0.5f,
                "g" when previousPitch == "a" => -0.5f,
                "a" when previousPitch == "c" => -1,
                "b" when previousPitch == "d" => -1,
                "c" when previousPitch == "e" => -1,
                "d" when previousPitch == "f" => -1,
                "e" when previousPitch == "g" => -1,
                "f" when previousPitch == "a" => -1,
                "g" when previousPitch == "b" => -1,
                "a" when previousPitch == "d" => -1.5f,
                "b" when previousPitch == "e" => -1.5f,
                "c" when previousPitch == "f" => -1.5f,
                "d" when previousPitch == "g" => -1.5f,
                "e" when previousPitch == "a" => -1.5f,
                "f" when previousPitch == "b" => -1.5f,
                "g" when previousPitch == "c" => -1.5f,
                "a" when previousPitch == "e" => 1.5f,
                "b" when previousPitch == "f" => 1.5f,
                "c" when previousPitch == "g" => 1.5f,
                "d" when previousPitch == "a" => 1.5f,
                "e" when previousPitch == "b" => 1.5f,
                "f" when previousPitch == "c" => 1.5f,
                "g" when previousPitch == "d" => 1.5f,
                "a" when previousPitch == "f" => 1,
                "b" when previousPitch == "g" => 1,
                "c" when previousPitch == "a" => 1,
                "d" when previousPitch == "b" => 1,
                "e" when previousPitch == "c" => 1,
                "f" when previousPitch == "d" => 1,
                "g" when previousPitch == "e" => 1,
                "a" when previousPitch == "g" => 0.5f,
                "b" when previousPitch == "a" => 0.5f,
                "c" when previousPitch == "b" => 0.5f,
                "d" when previousPitch == "c" => 0.5f,
                "e" when previousPitch == "d" => 0.5f,
                "f" when previousPitch == "e" => 0.5f,
                "g" when previousPitch == "f" => 0.5f,
                _ => throw new ArgumentException("\"" + pitch + "\" is not a valid pitch name!")
            };

            return lastOffset + pitchOffset;
        }
        private float CalculateOffsetFixed(string pitch, string octave)
        {
            if (octave is null or "")
            {
                return CalculateOffsetAbsolute(pitch, (string)settings["fixedOctave"]);
            }
            return CalculateOffsetAbsolute(pitch, octave);
        }
    }
}