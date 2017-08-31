using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlinePizza.Models
{
    public class CheckboxItem
    {
        public int IngredientId { get; set; }
        public string IngredientName { get; set; }
        public bool IsChecked { get; set; }
    }
}
