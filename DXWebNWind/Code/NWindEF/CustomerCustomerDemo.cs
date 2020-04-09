namespace DXWebNWind.Code.NWindEF
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("CustomerCustomerDemo")]
    public partial class CustomerCustomerDemo
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(5)]
        public string CustomerID { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(10)]
        public string CustomerTypeID { get; set; }
    }
}
