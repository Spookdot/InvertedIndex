using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace EntityFrameworkTest
{
    class Program
    {
        private static readonly List<string> words = new();
        private static readonly Regex rgx = new Regex(@"\W");
        
        static void Main(string[] args)
        {
            var tasks = new List<Task>();
            var fs = File.OpenText(args[0]);
            string line;
            while ((line = fs.ReadLine()) != null)
            {
                tasks.Add(ProcessLine(line));
            }
            Task.WaitAll(tasks.ToArray());
            words.ForEach(Console.WriteLine);
        }

        static async Task ProcessLine(string line)
        {
            foreach (var s in line.Split())
            {
                var word = rgx.Replace(s.ToLower(), "");
                if (!words.Contains(word))
                {
                    words.Add(word);
                }
            }
        }
    }
}