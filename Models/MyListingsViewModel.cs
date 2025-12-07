using SwipSwapMVC.Models;
using System.Collections.Generic;

namespace SwipSwapMVC.ViewModels
{
    public class MyListingsViewModel
    {
        public List<Product> Products { get; set; } = new();
        public Product NewProduct { get; set; } = new();
        public string Search { get; set; } = string.Empty;
    }
}
