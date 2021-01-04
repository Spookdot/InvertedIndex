using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace EntityFrameworkTest
{
    public class IndexContext : DbContext
    {
        public DbSet<Word> Words { get; set; }
        public DbSet<Document> Documents { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.UseSqlite("DataSource=/home/spookdot/RiderProjects/EntityFrameworkTest/database.db");
    }

    public class Word
    {
        [Key]
        public string Keyword { get; set; }

        public List<Document> Documents { get; } = new List<Document>();
    }

    public class Document
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Url { get; set; }
        public List<Word> Words { get; } = new List<Word>();
    }
}