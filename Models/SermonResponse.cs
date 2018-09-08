using System;

namespace preaching_collective_functions.Models
{
    public class SermonResponse
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string Book { get; set; }
        public string Text { get; set; }
        public int Chapter { get; set; }
        public int? VerseStart { get; set; }
        public int? VerseEnd { get; set; }
        public string Url { get; set; }
        public string Source { get; set; }

        public string Scripture { get; set; }
        public DateTimeOffset Date { get; set; }
    }
}