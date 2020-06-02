using System;
using System.Collections.Generic;

namespace OMSWebService.Model
{
    public partial class Shippers
    {
        public Shippers()
        {
            Orders = new HashSet<Order>();
        }

        public int ShipperId { get; set; }
        public string CompanyName { get; set; }
        public string Phone { get; set; }

        public virtual ICollection<Order> Orders { get; set; }
    }
}