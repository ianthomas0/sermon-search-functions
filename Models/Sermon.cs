using System;
using Microsoft.Azure.Search;

namespace Blackbaud.Church.PreachingCollective.Models
{
    public class Sermon
    {
        [System.ComponentModel.DataAnnotations.Key]
        public string Id { get; set; }
        [IsSearchable, IsFilterable, IsSortable]
        public string Title { get; set; }
        [IsSearchable, IsFilterable, IsSortable]
        public string Author { get; set; }
        [IsSearchable, IsFilterable, IsSortable]
        public string Book { get; set; }
        [IsSearchable, IsFilterable, IsSortable]
        public string Text { get; set; }
        [IsSearchable, IsFilterable, IsSortable]
        public int? Chapter { get; set; }
        [IsSearchable, IsFilterable, IsSortable]
        public int? VerseStart { get; set; }
        [IsSearchable, IsFilterable, IsSortable]
        public int? VerseEnd { get; set; }
        public string Url { get; set; }
        [IsSearchable, IsFilterable, IsSortable]
        public string Source { get; set; }

        public string Scripture { get; set; }
        [IsFilterable, IsSortable]
        public DateTimeOffset Date { get; set; }
    }
}