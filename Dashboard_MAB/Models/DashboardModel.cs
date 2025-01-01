namespace Dashboard_MAB.Models
{
    public class DashboardModel
    {
        public List<DashboardCounts> Counts { get; set; }
        public List<RecentOrder> RecentOrders { get; set; }
        public List<RecentProduct> RecentProducts { get; set; }
        public List<TopCustomer> TopCustomers { get; set; }
        public List<TopSellingProduct> TopSellingProducts { get; set; }
        public List<QuickLinks> NavigationLinks { get; set; }
    }
}
