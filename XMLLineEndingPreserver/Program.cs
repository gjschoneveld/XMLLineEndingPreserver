using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml;

namespace XMLLineEndingPreserver
{
    enum LineEnding
    {
        Unix,
        Windows
    }

    class Program
    {
        static LineEnding DetermineLineEnding(string path)
        {
            var content = File.ReadAllText(path);

            if (content.Any(b => b == '\r'))
            {
                return LineEnding.Windows;
            }
            else
            {
                return LineEnding.Unix;
            }
        }

        static string LineEndingValue(LineEnding ending)
        {
            switch (ending)
            {
                case LineEnding.Windows:
                    return "\r\n";
                case LineEnding.Unix:
                default:
                    return "\n";
            }
        }

        static void ForceLineEnding(string path, LineEnding ending)
        {
            var lines = File.ReadAllLines(path);

            var formatted = string.Join(LineEndingValue(ending), lines);

            File.WriteAllText(path, formatted);
        }

        static void Main(string[] args)
        {
            var files = new string[] { "Windows.xml", "Unix.xml" };

            foreach (var filename in files)
            {
                Console.WriteLine("Processing {0}...", filename);
                var ending = DetermineLineEnding(filename);
                Console.WriteLine("Before: {0}", ending);

                var doc = XDocument.Load(filename);
                var elem = doc.Root.Element("element");
                var attr = elem.Attribute("attr");

                attr.SetValue((int.Parse(attr.Value.Trim()) + 1));

                doc.Save(filename);

                ForceLineEnding(filename, ending);

                Console.WriteLine("After: {0}", DetermineLineEnding(filename));
                Console.WriteLine();
            }

            Console.ReadKey();
        }
    }
}
