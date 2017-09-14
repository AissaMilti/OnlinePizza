namespace OnlinePizza.Models
{
    public class SelectedCartItemIngredient
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool Enabled { get; set; }
        public int Price { get; set; }
    }
}