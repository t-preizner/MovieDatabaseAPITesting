using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.Configuration.Attributes;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieDatabaseAPITesting
{
    public class Search
    {
        [Name("title")]
        public string Title { get; set; }
        [Name("year")]
        public string Year { get; set; }
        [Name("imdbId")]
        public string imdbID { get; set; }
        [Name("type")]
        public string Type { get; set; }
        [Name("poster")]
        public string Poster { get; set; }
        
        public Search(string title, string year, string imdbId, string type, string poster) 
        {
            Title = title;
            Year = year;
            imdbID = imdbId;
            Type = type;
            Poster = poster;
        }
    }
 /*
    public sealed class SearchMap : ClassMap<Search>
    {
        public SearchMap()
        {
            Map(m => m.Title).Name("title");
            Map(m => m.Year).Name("year");
            Map(m => m.imdbID).Name("imdbId");
            Map(m => m.Type).Name("type");
            Map(m => m.Poster).Name("poster");
        }
    }
 */
}
