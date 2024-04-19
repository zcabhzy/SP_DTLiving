using System.ComponentModel.DataAnnotations;

namespace DTLiving.Models
{
    /// <summary>
    /// 員工資料模型
    /// </summary>
    public partial class Employer
    {
        // 員工 ID
        [Key]
        public int Id { get; set; }

        // 員工編號
        public string StaffId { get; set; }

        // 員工性別
        public string Gender { get; set; }

        // 員工圖片
        public byte[]? ClerkImage { get; set; }

        // 員工姓名
        public string ClerkName { get; set; }

        // 員工生日 ; 格式為 yyyy/MM/dd
        [RegularExpression(@"^\d{4}/\d{2}/\d{2}$", ErrorMessage = "生日格式 : yyyy/MM/dd")]
        public string born { get; set; }

        // 員工電話號碼
        public string ClerkPhone { get; set; }

        // 員工地址
        public string ClerkAddress { get; set; }

        // 員工創建時間
        public DateTime SetupTime { get; set; }
    }
}
