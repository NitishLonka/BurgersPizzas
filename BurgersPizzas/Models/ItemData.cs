using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
namespace BurgersPizzas.Models
{
    public class ItemData
    {
        public int Id { get; set; }

        public string Name { get; set; }

       
        public string Web { get; set; }

        public string Description { get; set; }

        public string Ingredients { get; set; }

        public bool isDisabled { get; set; }

        public bool isBurger { get; set; }

    }
}


