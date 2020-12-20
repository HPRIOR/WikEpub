using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WikEpub.Models
{
    public class EpubFile
    {
        [Required]
        public string BookTitle { get; set; }

        [Required]
        public IEnumerable<string> WikiPages { get; set; }

        public Guid guid { get; set; }
        public string FilePath { get; set; }
    }
}