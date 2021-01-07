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
        private static readonly Regex rgx = new(@"[^\w ]");
        private static readonly IndexContext Context = new();
        private static Document Document;

        static void Main(string[] args)
        {
            var tasks = new List<Task>();
            if((Document = Context.Documents.SingleOrDefault(d => d.Url == args[0])) == null)
            {
                Document = new Document() {Url = args[0]};
                Context.Documents.Add(Document);
                Context.SaveChanges();
            }
            var fs = File.OpenText(args[0]);
            string line;
            while ((line = fs.ReadLine()) != null)
            {
                tasks.Add(ProcessLine(line));
            }

            Task.WaitAll(tasks.ToArray());
            Context.SaveChanges();
            foreach (var word in Context.Words.ToList())
            {
                Console.WriteLine(word.Keyword);
            }
        }

        static async Task ProcessLine(string line)
        {
            foreach (var s in rgx.Replace(line.ToLower(), "").Split())
            {
                Word wordEntry;
                if ((wordEntry = await Context.Words.SingleOrDefaultAsync(w => w.Keyword == s)) == null)
                {
                    await Context.Words.AddAsync(new Word() {Keyword = s, Documents = { Document }});
                    await Context.SaveChangesAsync();
                }
                else
                {
                    var documents = Context.Entry(wordEntry).Collection(b => b.Documents)
                        .Query().Where(x => x.Url == Document.Url).ToList();
                    if(documents.Count == 0) { wordEntry.Documents.Add(Document); }
                }
            }
        }
    }
}