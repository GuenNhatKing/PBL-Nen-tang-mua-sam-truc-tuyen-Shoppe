namespace ShoppeWebApp.ViewModels.Admin
{
    public class RevenueByCustomerViewModel
    {
        public DateTime? StartDate { get; set; } // Ngày bắt đầu
        public DateTime? EndDate { get; set; } // Ngày kết thúc
        public List<CustomerRevenue> CustomerRevenues { get; set; } = new List<CustomerRevenue>(); // Danh sách doanh thu theo khách hàng

        // Tổng doanh thu của tất cả khách hàng (tùy chọn để hiển thị tổng quan)
        public decimal TotalRevenue => CustomerRevenues.Sum(c => c.Revenue);

        // Tổng số đơn hàng của tất cả khách hàng (tùy chọn để hiển thị tổng quan)
        public int TotalOrders => CustomerRevenues.Sum(c => c.OrderCount);

        // Tổng số sản phẩm đã mua của tất cả khách hàng (tùy chọn để hiển thị tổng quan)
        public int TotalProducts => CustomerRevenues.Sum(c => c.ProductCount);
        public int TotalCustomers { get; set; } // Tổng số khách hàng
    }

    public class CustomerRevenue
    {
        public string CustomerId { get; set; } = string.Empty; // ID khách hàng
        public string CustomerName { get; set; } = string.Empty; // Tên khách hàng
        public decimal Revenue { get; set; } // Doanh thu
        public int OrderCount { get; set; } // Số lượng đơn hàng
        public int ProductCount { get; set; } // Số lượng sản phẩm đã mua

        // Tùy chọn: Hiển thị trạng thái khách hàng (ví dụ: VIP, thường xuyên, mới, v.v.)
        public string CustomerStatus
        {
            get
            {
                if (Revenue > 100000000) return "VIP";
                if (Revenue > 50000000) return "Thường xuyên";
                return "Khách hàng mới";
            }
        }
    }
}