namespace ShoppeWebApp.ViewModels.Seller
{
    public class DashboardViewModel
    {
        public int DonChoXacNhan { get; set; }
        public int DonDaXacNhan { get; set; }
        public int DonDaGiao { get; set; }
        public int DonHuy { get; set; }
        public int SanPhamTamKhoa { get; set; }
        public int SanPhamHetHang { get; set; }

        public List<RevenueByDay> RevenueByDay { get; set; } = new List<RevenueByDay>();
        public List<RevenueByProduct> RevenueByProduct { get; set; } = new List<RevenueByProduct>();
        public List<OrderFrequency> OrderFrequency { get; set; } = new List<OrderFrequency>();

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }

    public class RevenueByDay
    {
        public DateTime Date { get; set; }
        public decimal TotalRevenue { get; set; }
    }

    public class RevenueByProduct
    {
        public string ProductName { get; set; }
        public decimal TotalRevenue { get; set; }
    }

    public class OrderFrequency
    {
        public DateTime Date { get; set; }
        public int OrderCount { get; set; }
    }
}