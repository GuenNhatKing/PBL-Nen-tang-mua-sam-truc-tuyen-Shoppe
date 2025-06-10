namespace ShoppeWebApp.ViewModels.Admin
{
    public class TopSellingProductsViewModel
    {
        // Thông tin sản phẩm bán chạy

        // Doanh thu theo danh mục sản phẩm
        public List<CategoryRevenue> CategoryRevenues { get; set; } = new List<CategoryRevenue>(); // Danh sách doanh thu theo danh mục
        public string TopCategoryName { get; set; } = string.Empty; // Tên danh mục có doanh thu cao nhất
        public decimal TopCategoryRevenue { get; set; } // Doanh thu cao nhất của danh mục

        // Bộ lọc
        public DateTime? StartDate { get; set; } // Ngày bắt đầu
        public DateTime? EndDate { get; set; } // Ngày kết thúc
        public string? SelectedCategory { get; set; } // Danh mục được chọn để lọc
        public List<string> AvailableCategories { get; set; } = new List<string>(); // Danh sách các danh mục có sẵn
        
        // Add the missing property
        public List<Product> TopSellingProducts { get; set; } = new List<Product>();
    }

    public class Product
    {
        public string ProductId { get; set; } = string.Empty; 
        public string? ProductName { get; set; }
        public string? ImageUrl { get; set; }
        public int QuantitySold { get; set; }
        public decimal TotalRevenue { get; set; }
        
        public int OrderCount { get; set; } 
    }

    public class CategoryRevenue
    {
        public string CategoryName { get; set; } = string.Empty; // Tên danh mục sản phẩm
        public decimal Revenue { get; set; } // Tổng doanh thu của danh mục
    }
}