namespace ShoppeWebApp.Data
{
    public class Constants
    {
        public const int ADMIN_ROLE = 0;
        public const int CUSTOMER_ROLE = 1;
        public const int SELLER_ROLE = 2;
        public const int COOKIE_EXPIRY_DAYS = 3;
        public const int PAGINATION_SIZE = 16;
        public const int PAGINATION_DELTA = 2;
        public static string[] DANH_SACH_TINH = {
            "Tuyên Quang",
            "Lào Cai",
            "Thái Nguyên",
            "Phú Thọ",
            "Bắc Ninh",
            "Hưng Yên",
            "Hải Phòng",
            "Ninh Bình",
            "Quảng Trị",
            "Đà Nẵng",
            "Quảng Ngãi",
            "Gia Lai",
            "Khánh Hòa",
            "Lâm Đồng",
            "Đắk Lắk",
            "Thành phố Hồ Chí Minh",
            "Đồng Nai",
            "Tây Ninh",
            "Cần Thơ",
            "Vĩnh Long",
            "Đồng Tháp",
            "Cà Mau",
            "An Giang",
            "Hà Nội",
            "Huế",
            "Lai Châu",
            "Điện Biên",
            "Sơn La",
            "Lạng Sơn",
            "Quảng Ninh",
            "Thanh Hóa",
            "Nghệ An",
            "Hà Tĩnh",
            "Cao Bằng"
        };

        public const int RANDOM_SIZE = 16;
        public static DateTime DAYS_USE_FOR_RANDOM = new DateTime(2025, 1, 1);
        
        public const int HUY_DON_HANG = 0;
        public const int CHO_XAC_NHAN = 1;
        public const int DA_XAC_NHAN = 2;
        public const int DA_GIAO = 3;  

        public const int TAM_KHOA = 0; //cho sp
        public const int CON_HANG = 1; //con hang
        public const int HET_HANG = 2; //het hang
    }
}
