using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace EntityFrameworkTest
{
    class Program
    {
        private static readonly Regex rgx = new(@"\W");
        private static readonly IndexContext Context = new();
        private static Document Document;

        static void Main(string[] args)
        {
            var tasks = new List<Task>();
            if ((Document = Context.Documents.Single(d => d.Url == args[0])) == null)
            {
                Document = new Document() {Url = args[0]};
                Context.Documents.Add(Document);
            }
            var fs = File.OpenText(args[0]);
            string line;
            while ((line = fs.ReadLine()) != null)
            {
                tasks.Add(ProcessLine(line));
            }

            Task.WaitAll(tasks.ToArray());
            foreach (var word in Context.Words.ToList())
            {
                Console.WriteLine(word.Keyword);
            }
        }

        static async Task ProcessLine(string line)
        {
            foreach (var s in line.Split())
            {
                var word = rgx.Replace(s.ToLower(), "");
                Word wordEntry;
                if((wordEntry = await Context.Words.SingleAsync(w => w.Keyword == word)) == null)
                {
                    await Context.Words.AddAsync(new Word() {Keyword = word, Documents = { Document }});
                    await Context.SaveChangesAsync();
                }
                else if(wordEntry.Documents.Exists(x => x.Url == Document.Url))
                {
                    wordEntry.Documents.Add(Document);
                    await Context.SaveChangesAsync();
                }
            }
        }
    }
}