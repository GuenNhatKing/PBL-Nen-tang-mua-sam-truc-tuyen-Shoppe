namespace ShoppeWebApp.ViewModels.Admin
{
    public class TotalRevenueViewModel
    {
        public DateTime? StartDate { get; set; } // Ngày bắt đầu
        public DateTime? EndDate { get; set; }   // Ngày kết thúc
        public decimal TotalRevenue { get; set; } // Tổng doanh thu

        // Thống kê theo trạng thái đơn hàng
        public int TotalOrders { get; set; } // Tổng số đơn hàng
        public int PendingOrders { get; set; } // Số đơn hàng chờ xác nhận
        public int ConfirmedOrders { get; set; } // Số đơn hàng đã xác nhận
        public int DeliveredOrders { get; set; } // Số đơn hàng đã giao
        public int CancelledOrders { get; set; } // Số đơn hàng đã hủy

        // Thống kê trạng thái sản phẩm
        public int ActiveProducts { get; set; } // Số sản phẩm đang hoạt động
        public int TemporarilyLockedProducts { get; set; } // Số sản phẩm tạm khóa

        // Dữ liệu biểu đồ
        public List<DateTime> RevenueDates { get; set; } // Danh sách ngày
        public List<decimal> RevenueValues { get; set; } // Doanh thu theo ngày
        public List<int> OrderFrequencies { get; set; } // Tần suất đặt đơn hàng theo ngày
        public List<int> CancelledFrequencies { get; set; } // Tần suất hủy đơn theo ngày
        public List<int> DeliveredFrequencies { get; set; } // Tần suất giao đơn thành công theo ngày
    }
}