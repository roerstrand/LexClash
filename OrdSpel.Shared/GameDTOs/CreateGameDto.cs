using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace OrdSpel.Shared.GameDTOs
{
    public class CreateGameDto
    {
        [Required]
        public int CategoryId { get; set; }
    }
}
