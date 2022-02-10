using System.Collections;

namespace ArgHandling
{
    public class ArgumentParser
    {
        public ArrayList Arguments
        {
            get; set;
        }
        public Dictionary<string, Argument> argMap;
        public bool defaults;

        public ArgumentParser(bool defaults = true)
        {
            Arguments = new ArrayList();
            if (defaults)
            {
                Arguments.Add(new Argument("help", "h", "help", true, "default", help: "Print this help message and exit"));
            }
            this.defaults = defaults;
            argMap = new Dictionary<string, Argument>();
            UpdateArgMap();
        }

        public void UpdateArgMap()
        {
            argMap = new Dictionary<string, Argument>();
            foreach (Argument argument in Arguments)
            {
                argMap["--" + argument.longName] = argument;
                argMap["-" + argument.shortName] = argument;
            }
        }

        public void AddArgument(Argument argument, bool update = true)
        {
            if (argument == null)
            {
                throw new ArgumentNullException(nameof(argument));
            }
            Arguments.Add(argument);
            if (update)
            {
                UpdateArgMap();
            }
        }

        public void AddArgument(string longName, string shortName, string key, bool flag = false, string defaultValue = "", string help = "", bool update = true)
        {
            AddArgument(new Argument(longName, shortName, key, flag, defaultValue, help), update);
        }

        public void AddFlag(string longName, string shortName, string key, string defaultValue, string help = "", bool update = true)
        {
            AddArgument(new Argument(longName, shortName, key, true, defaultValue, help), update);
        }

        public Dictionary<string, string> Parse(string[] args)
        {
            Dictionary<string, string> kwargs = new();
            for (int i = 0; i < args.Length; i++)
            {
                string arg = args[i];
                string value = null;
                if (arg.Contains('='))
                {
                    arg = arg.Split('=')[0];
                    value = arg.Split('=')[1];
                }
                else if (i + 1 < args.Length && args[i + 1][0] != '-')
                {
                    i++;
                    value = args[i];
                }
                if (argMap.ContainsKey(arg))
                {
                    Argument argument = argMap[arg];
                    if (argument.flag)
                    {
                        kwargs[argument.key] = argument.defaultValue;
                    }
                    else
                    {
                        kwargs[argument.key] = value ?? throw new ArgumentException($"Detected \"{arg}\", but no value was provided!");
                    }
                }
                else
                {
                    Console.WriteLine($"NOTICE: Ignoring unrecognized parameter \"{arg}\"");
                }
            }
            return kwargs;
        }

        public void PrintHelp()
        {
            string syntaxString = "program";
            foreach (Argument argument in Arguments)
            {
                syntaxString += " -" + argument.shortName;
                if (!argument.flag)
                {
                    syntaxString += " <" + argument.key + ">";
                }
            }
            Console.WriteLine("Syntax: " + syntaxString);
            foreach (Argument argument in Arguments)
            {
                Console.WriteLine($"\t-{argument.shortName}/--{argument.longName}\t{argument.help}");
            }
        }
    }

    public class Argument
    {
        public string longName;
        public string shortName;
        public string key;
        public bool flag;
        public string defaultValue;
        public string help;

        public Argument(string longName, string shortName, string key, bool flag = false, string defaultValue = "", string help = "")
        {
            this.longName = longName;
            this.shortName = shortName;
            this.key = key;
            if (flag && defaultValue == "")
            {
                throw new ArgumentException("All \"flag\" arguments MUST define a non-empty \"defaultValue\"!");
            }
            this.flag = flag;
            this.defaultValue = defaultValue;
            this.help = help;
        }
    }
}