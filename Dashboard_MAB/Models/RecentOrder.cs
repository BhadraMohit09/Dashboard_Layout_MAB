﻿namespace Dashboard_MAB.Models
{
    public class RecentOrder
    {
        public int OrderID { get; set; }
        public string CustomerName { get; set; }
        public DateTime OrderDate { get; set; }
        public string Status { get; set; }
    }
}