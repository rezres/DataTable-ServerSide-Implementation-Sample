using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DataTable_ServerSide__Implementation_Sample.Data.Model
{
    public class ProductContext : DbContext
    {
       
        public ProductContext(DbContextOptions<ProductContext> options)
              : base(options)
        {
        }
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            
        }


        public async Task InitSeed()
        {
            var rand = new Random();
            var categories = new List<Category> {
                new Category{ Name = "Electronic"},
                new Category{ Name = "Clothes"},
                new Category{ Name = "Home"},
            };
            await this.Categories.AddRangeAsync(categories);
            await this.SaveChangesAsync();

        }

    }
}
