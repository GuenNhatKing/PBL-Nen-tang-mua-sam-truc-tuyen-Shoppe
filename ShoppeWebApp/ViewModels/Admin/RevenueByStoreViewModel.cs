namespace ShoppeWebApp.ViewModels.Admin
{
    public class RevenueByStoreViewModel
    {
        public DateTime? StartDate { get; set; } // Ngày bắt đầu
        public DateTime? EndDate { get; set; } // Ngày kết thúc
        public List<StoreRevenue> StoreRevenues { get; set; } = new List<StoreRevenue>(); // Danh sách doanh thu theo cửa hàng
    }

    public class StoreRevenue
    {
        public string StoreName { get; set; } = string.Empty; // Tên cửa hàng
        public string StoreId { get; set; } = string.Empty; // ID cửa hàng
        public int OrderCount { get; set; } // Số lượng đơn hàng
        public int ProductCount { get; set; } // Số lượng sản phẩm bán được
        public decimal Revenue { get; set; } // Doanh thu
    }
}