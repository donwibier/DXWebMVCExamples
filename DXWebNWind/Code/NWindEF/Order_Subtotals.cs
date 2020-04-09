namespace DXWebNWind.Code.NWindEF
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Order Subtotals")]
    public partial class Order_Subtotals
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int OrderID { get; set; }

        [Column(TypeName = "smallmoney")]
        public decimal? Subtotal { get; set; }

        public virtual Orders Orders { get; set; }
    }
}
