using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace DTLiving.Models
{
    /// <summary>
    /// 會員資料模型
    /// </summary>
    public partial class Client
    {
        // 會員 Id
        [Key]
        public int Id { get; set; }

        // 會員帳號
        [JsonProperty("UserID")]
        public string UserID { get; set; }

        // 會員密碼
        [JsonProperty("UserPassword")]
        public string UserPassword { get; set; }

        // 會員名稱
        [JsonProperty("UserName")]
        public string UserName { get; set; }

        // 會員電話
        [JsonProperty("UserPhone")]
        public string UserPhone { get; set; }

        // 會員信箱
        [JsonProperty("UserMail")]
        public string UserMail { get; set; }

        // 會員地址
        [JsonProperty("UserAddress")]
        public string UserAddress { get; set; }
    }
}
