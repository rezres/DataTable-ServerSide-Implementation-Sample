using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DataTable_ServerSide__Implementation_Sample.Data.Model
{
    public class Product
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int MainCatId { get; set; }
        public int SubCatId { get; set; }
        [ForeignKey("MainCatId")]
        public Category MainCategory { get; set; }
        [ForeignKey("SubCatId")]
        public Category SubCategory { get; set; }
    }
}
