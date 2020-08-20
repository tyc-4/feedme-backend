using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace feedme_backend.Models
{
    public class OrderPost
    {
        public int ResId { get; set; }
        public int UserId { get; set; }

        public List<OrderItem> OrderItems { get; set; }
    }
}