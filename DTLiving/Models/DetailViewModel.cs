namespace DTLiving.Models
{
    public class DetailViewModel
    {
        // 獲取 / 設定產品物件
        public Product Product { get; set; }

        // 獲取 / 設定分類物件
        public Category Category { get; set; }

        // 獲取 / 設定產品圖片的路徑
        public string imgsrc { get; set; }
    }
}
