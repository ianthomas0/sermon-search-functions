using System;
using Azure.Search.Documents.Indexes;

namespace PreachingCollective.Models
{
    public class SermonInsert
    {
        public string Id { get; set; }

        public string id { get { return Id; } }

        public string Title { get; set; }

        public string Author { get; set; }

        public string Book { get; set; }

        public string BookOrder { get; set; }

        public string Text { get; set; }

        public int? Chapter { get; set; }

        public int? VerseStart { get; set; }

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