using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
namespace BurgersPizzas.Models
{
    public class ItemData
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string Restaurant { get; set; }

        public string Web { get; set; }

        public string Description { get; set; }

        public List<string> Ingredients { get; set; }
    }
}


