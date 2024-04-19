using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace DTLiving.Models
{
    /// <summary>
    /// 產品模型
    /// </summary>
    public partial class Product
    {
        // 產品 ID (主鍵)
        [Key]
        public int ID { get; set; }

        // 商品名稱 => 使用 JSON 屬性名稱 "NAME"
        [JsonPropertyName("NAME")]
        public string NAME { get; set; }

        // 商品價格 => 使用 JSON 屬性名稱 "PRICE"
        [JsonPropertyName("PRICE")]
        public int PRICE { get; set; }

        // 商品庫存
        public int STOCK { get; set; }

        // 商品圖片 => 使用 JSON 屬性名稱 "IMAGE"
        [JsonPropertyName("IMAGE")]
        public byte[]? IMAGE { get; set; }

        // 商品介紹
        public string? CONTEXT { get; set; } 

        // 商品品牌 => 外鍵關聯
        public int? CategoryId { get; set; }

        // 導覽屬性，關聯至對應的 Category
        public Category? Category { get; set; }
    }

    /// <summary>
    /// 品牌模型
    /// </summary>
    public partial class Category
    {
        // 品牌 ID
        [Key]
        public int ID { get; set; }

        // 品牌名稱
        public string CategoryName { get; set; }

        // 一個品牌包含的產品列表
        public List<Product> PRODUCT { get; set; }
    }
}
