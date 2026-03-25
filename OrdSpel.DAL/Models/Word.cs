using System;
using System.Collections.Generic;
using System.Text;

namespace OrdSpel.DAL.Models
{
    public class Word
    {
        public int Id { get; set; }
        public int CategoryId { get; set; }
        public string Text { get; set; } = string.Empty;
        public bool IsHard { get; set; }

    }
}
