using DTLiving.Models;
using Microsoft.EntityFrameworkCore;

namespace DTLiving.Context
{
    public class DTContext : DbContext
    {
        // 定義 DTContext 類別,繼承至 DbContext。
        // 每個 DbSet<> 屬性對應資料庫中的一張資料表,屬性的名稱是資料表名稱。
        public virtual DbSet<Product> Product { get; set; } // 產品資料表

        public virtual DbSet<Category> Category { get; set; } // 品牌資料表

        public virtual DbSet<Employer> Employer { get; set; } // 員工資料表

        public virtual DbSet<OrderTable> OrderTable { get; set; } // 訂單明細資料表

        public virtual DbSet<ShopOrder> ShopOrder { get; set; } // 訂單資料表

        public virtual DbSet<Client> Client { get; set; } // 客戶資料表

        public DTContext()
        {
            
        }

        public DTContext(DbContextOptions<DTContext> option) : base(option)
        {

        }

        // 覆寫 OnConfiguring 方法,設定資料庫連線字串以連接資料庫。
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // 使用 SQL Server 資料庫,設定連線字串。
            optionsBuilder.UseSqlServer("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=Local;Integrated Security=True");
        }
    }
}
