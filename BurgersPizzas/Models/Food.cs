using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BurgersPizzas.Models
{
   
   
    public class BurgerDb
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Web { get; set; }
        public string Description { get; set; }

        public string Ingredients { get; set; }

        public CategoryDb Category { get; set; }

    }

    public class PizzaDb
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Web { get; set; }
        public string Description { get; set; }

        public string Ingredients { get; set; }

        public CategoryDb Category { get; set; }
    }
    public class CategoryDb
    {
        [Key]
        public int Id { get; set; }

        public string Category { get; set; }

        public List<BurgerDb>? Burgers { get; set; }

        public List<PizzaDb>? Pizzas { get; set; }
    }
}
