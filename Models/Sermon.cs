using Microsoft.Azure.Search;

namespace Blackbaud.Church.PreachingCollective.Models
{
    public class Sermon
    {
        [System.ComponentModel.DataAnnotations.Key]
        public string Id { get; set; }
        [IsSearchable, IsFilterable, IsSortable, IsFacetable]
        public string Book { get; set; }
        [IsSearchable, IsFilterable, IsSortable, IsFacetable]
        public string Text { get; set; }
        [IsSearchable, IsFilterable, IsSortable, IsFacetable]
        public string Chapter { get; set; }
        [IsSearchable, IsFilterable, IsSortable, IsFacetable]
        public string VerseStart { get; set; }
        [IsSearchable, IsFilterable, IsSortable, IsFacetable]
        public string VerseEnd { get; set; }
        public string Url { get; set; }
        [IsSearchable, IsFilterable, IsSortable, IsFacetable]
        public string Source { get; set; }
    }
}