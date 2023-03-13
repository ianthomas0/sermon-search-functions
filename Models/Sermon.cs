using System;
using Azure.Search.Documents.Indexes;
using Azure.Search.Documents.Indexes.Models;

namespace Blackbaud.Church.PreachingCollective.Models
{
    public class Sermon
    {
        [System.ComponentModel.DataAnnotations.Key]
        public string Id { get; set; }

        [SearchableField(IsFilterable = true, IsSortable = true, AnalyzerName = LexicalAnalyzerName.Values.StandardLucene)]
        public string Title { get; set; }

        [SimpleField(IsFilterable = true, IsSortable = true, IsFacetable = true)]
        public string Author { get; set; }

        [SimpleField(IsFilterable = true, IsSortable = true, IsFacetable = true)]
        public string Book { get; set; }

        [SimpleField(IsFilterable = true, IsSortable = true)]
        public int? BookOrder { get; set; }

        [SearchableField(IsFilterable = true, IsSortable = true)]
        public string Text { get; set; }

        [SimpleField(IsFilterable = true, IsSortable = true)]
        public long? Chapter { get; set; }

        [SimpleField(IsFilterable = true, IsSortable = true)]
        public long? VerseStart { get; set; }

        [SimpleField(IsFilterable = true, IsSortable = true)]
        public long? VerseEnd { get; set; }


        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1056:Uri properties should not be strings", Justification = "<Pending>")]
        public string Url { get; set; }

        [SimpleField(IsFilterable = true, IsSortable = true, IsFacetable = true)]
        public string Source { get; set; }

        public string Scripture { get; set; }

        [SimpleField(IsFilterable = true, IsSortable = true)]
        public DateTimeOffset Date { get; set; }
    }
}