namespace DXWebNWind.Code.NWindEF
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Products Above Average Price")]
    public partial class Products_Above_Average_Price
    {
        [Key]
        [StringLength(40)]
        public string ProductName { get; set; }

        [Column(TypeName = "smallmoney")]
        public decimal? UnitPrice { get; set; }
    }
}
