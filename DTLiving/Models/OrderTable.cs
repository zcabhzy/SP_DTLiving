using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace DTLiving.Models
{
    /// <summary>
    /// 訂單內容模型
    /// </summary>
    public partial class OrderTable
    {
        [Key]
        public int Id { get; set; }

        // 使用者名稱 => 使用 JSON 屬性名稱 "CatchUser"
        [JsonProperty("CatchUser")]
        public string CatchUser { get; set; }

        // 產品 ID => 使用 JSON 屬性名稱 "ProductId"
        [JsonProperty("ProductId")]
        public int ProductId { get; set; }

        // 產品名稱 => 使用 JSON 屬性名稱 "Name"
        [JsonProperty("Name")]
        public string Name { get; set; }

        // 產品品牌 => 使用 JSON 屬性名稱 "OrderCategory"
        [JsonProperty("OrderCategory")]
        public string OrderCategory { get; set; }

        // 產品價格 => 使用 JSON 屬性名稱 "Price"
        [JsonProperty("Price")]
        public int Price { get; set; }

        // 產品庫存 => 使用 JSON 屬性名稱 "Stock"
        [JsonProperty("Stock")]
        public int Stock { get; set; }

        // 購買數量 => 使用 JSON 屬性名稱 "Quantity"
        [JsonProperty("Quantity")]
        public int Quantity { get; set; }

        // 訂單小計
        public int Subtotal { get; set; }

        // 訂單編號
        public string OrderNumber { get; set; }

        // 導覽屬性，關聯對應 ShopOrder
        public ShopOrder ShopOrder { get; set; }
    }

    /// <summary>
    /// 訂單模型
    /// </summary>
    public partial class ShopOrder
    {
        [Key]
        public int Id { get; set; }

        // 訂單建立時間
        public string CreateTime { get; set; }
        
        // 訂單編號
        public string OrderNumber { get; set; }

        // 收件人
        public string Receiver { get; set; }

        // 付款狀態
        public bool IsPad { get; set; }

        // 總額
        public int Total { get; set; }

        // ICollection<OrderTable>，訂單所包含的產品列表
        // 1.先行初始化屬性
        // 2.方便添加訂單明細
        public List<OrderTable> orderTables { get; set; } = new List<OrderTable>();
    }
}
