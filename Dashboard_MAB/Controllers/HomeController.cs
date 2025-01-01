using Dashboard_MAB.Models;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Diagnostics;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace Dashboard_MAB.Controllers
{
    public class HomeController : Controller
    {

        private IConfiguration configuration;

        public HomeController(IConfiguration _configuration)
        {
            configuration = _configuration;
        }

        public async Task<IActionResult> Index()
        {
            var dashboardData = new DashboardModel
            {
                Counts = new List<DashboardCounts>(),
                RecentOrders = new List<RecentOrder>(),
                RecentProducts = new List<RecentProduct>(),
                TopCustomers = new List<TopCustomer>(),
                TopSellingProducts = new List<TopSellingProduct>(),
                NavigationLinks = new List<QuickLinks>()
            };

            using (var connection = new SqlConnection(this.configuration.GetConnectionString("ConnectionString")))
            {
                await connection.OpenAsync();

                using (var command = new SqlCommand("usp_GetDashboardData", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (reader.HasRows)
                        {
                            // Fetch counts
                            while (await reader.ReadAsync())
                            {
                                dashboardData.Counts.Add(new DashboardCounts
                                {
                                    Metric = reader["Metric"].ToString(),
                                    Value = Convert.ToInt32(reader["Value"])
                                });
                            }

                            // Fetch recent orders
                            if (await reader.NextResultAsync())
                            {
                                while (await reader.ReadAsync())
                                {
                                    dashboardData.RecentOrders.Add(new RecentOrder
                                    {
                                        OrderID = Convert.ToInt32(reader["OrderID"]),
                                        CustomerName = reader["CustomerName"].ToString(),
                                        OrderDate = Convert.ToDateTime(reader["OrderDate"]),
                                        Status = reader["Status"].ToString()
                                    });
                                }
                            }

                            // Fetch recent products
                            if (await reader.NextResultAsync())
                            {
                                while (await reader.ReadAsync())
                                {
                                    dashboardData.RecentProducts.Add(new RecentProduct
                                    {
                                        ProductID = Convert.ToInt32(reader["ProductID"]),
                                        ProductName = reader["ProductName"].ToString(),
                                        Category = reader["Category"].ToString(),
                                        AddedDate = Convert.ToDateTime(reader["AddedDate"]),
                                        StockQuantity = Convert.ToInt32(reader["StockQuantity"])
                                    });
                                }
                            }

                            // Fetch top customers
                            if (await reader.NextResultAsync())
                            {
                                while (await reader.ReadAsync())
                                {
                                    dashboardData.TopCustomers.Add(new TopCustomer
                                    {
                                        CustomerName = reader["CustomerName"].ToString(),
                                        TotalOrders = Convert.ToInt32(reader["TotalOrders"]),
                                        Email = reader["Email"].ToString()
                                    });
                                }
                            }

                            // Fetch top selling products
                            if (await reader.NextResultAsync())
                            {
                                while (await reader.ReadAsync())
                                {
                                    dashboardData.TopSellingProducts.Add(new TopSellingProduct
                                    {
                                        ProductName = reader["ProductName"].ToString(),
                                        TotalSoldQuantity = Convert.ToInt32(reader["TotalSoldQuantity"]),
                                        Category = reader["Category"].ToString()
                                    });
                                }
                            }
                        }
                    }
                }
            }

            dashboardData.NavigationLinks = new List<QuickLinks> {
        new QuickLinks {ActionMethodName = "Index", ControllerName="HomeMaster", LinkName="Dashboard" },
        new QuickLinks {ActionMethodName = "Privacy", ControllerName="HomeMaster", LinkName="Privacy" },
        new QuickLinks {ActionMethodName = "Index", ControllerName="Country", LinkName="Country" },
        new QuickLinks {ActionMethodName = "Index", ControllerName="State", LinkName="State" },
        new QuickLinks {ActionMethodName = "Index", ControllerName="City", LinkName="City" }
    };

            var model = new DashboardModel
            {
                Counts = dashboardData.Counts,
                RecentOrders = dashboardData.RecentOrders,
                RecentProducts = dashboardData.RecentProducts,
                TopCustomers = dashboardData.TopCustomers,
                TopSellingProducts = dashboardData.TopSellingProducts,
                NavigationLinks = dashboardData.NavigationLinks
            };

            return View("Dashboard", model);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}