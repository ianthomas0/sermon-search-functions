using System;
using Azure.Search.Documents.Indexes;

namespace Blackbaud.Church.PreachingCollective.Models
{
    public class Sermon
    {
        [System.ComponentModel.DataAnnotations.Key]
        public string Id { get; set; }

        [SearchableField(IsFilterable = true, IsSortable = true)]
        public string Title { get; set; }

        [SearchableField(IsFilterable = true, IsSortable = true)]
        public string Author { get; set; }

        [SearchableField(IsFilterable = true, IsSortable = true)]
        public string Book { get; set; }

        [SearchableField(IsFilterable = true, IsSortable = true)]
        public string Text { get; set; }

        [SimpleField(IsFilterable = true, IsSortable = true)]
        public int? Chapter { get; set; }

        [SimpleField(IsFilterable = true, IsSortable = true)]
        public int? VerseStart { get; set; }

        [SimpleField(IsFilterable = true, IsSortable = true)]
        public int? VerseEnd { get; set; }


        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1056:Uri properties should not be strings", Justification = "<Pending>")]
        public string Url { get; set; }

        [SearchableField(IsFilterable = true, IsSortable = true)]
        public string Source { get; set; }

        public string Scripture { get; set; }

        [SimpleField(IsFilterable = true, IsSortable = true)]
        public DateTimeOffset Date { get; set; }
    }
}