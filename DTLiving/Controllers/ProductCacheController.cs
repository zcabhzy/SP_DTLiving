using DTLiving.Context;
using DTLiving.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DTLiving.Controllers
{
    public class ProductCacheController : Controller
    {
        private readonly DTContext _context;

        /// <summary>
        /// 控制器的建構函式，初始化 <see cref="DTContext"/> 物件。
        /// </summary>
        /// <param name="context">資料庫上下文。</param>
        public ProductCacheController(DTContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 從資料庫中獲取所有產品的資訊,包括產品名稱 / 品牌名稱 / 圖片 / 價格
        /// </summary>
        /// <returns> 包含產品資訊的 JSON 物件 </returns>
        [HttpGet]
        [Route("/ProductCache/GetProduct")]
        public IActionResult GetProduct()
        {

            var product = _context.Product
                    .Include(p => p.Category)
                    .ToList();

            if (product == null)
            {
                return NotFound();
            }

            var result = product.Select(product => new {
                ID = product.ID,
                Name = product.NAME,
                Category = product.Category.CategoryName,
                Image = product.IMAGE,
                Price = product.PRICE
            });

            return Ok(result);
        }

        /// <summary>
        /// 根據前端發送的 productid，從資料庫中查詢是否有相同的 ID
        /// </summary>
        /// <param name="productid"> 前端傳送產品 Id </param>
        /// <returns> 查詢資料庫是否有對應產品 Id ; 有 : 回傳所有資訊 ; 否 : 回傳 404 Not Found </returns>
        [HttpGet]
        [Route("/ProductCache/GetProductDetail")]
        public IActionResult GetProductDetail(int productid)
        {
            // 查詢資料庫是否含有相對 Id 產品
            var GetProductDetail_ID = _context.Product
                .Include(p => p.Category)
                .FirstOrDefault(p => p.ID == productid);

            // 如果找到相應的產品，則返回詳細資訊
            if (GetProductDetail_ID != null)
            {
                var DetailJson = new {
                    ID = GetProductDetail_ID.ID,
                    NAME = GetProductDetail_ID.NAME,
                    PRICE = GetProductDetail_ID.PRICE,
                    STOCK = GetProductDetail_ID.STOCK,
                    IMAGE = GetProductDetail_ID.IMAGE,
                    CONTEXT = GetProductDetail_ID.CONTEXT,
                    Category = GetProductDetail_ID.Category.CategoryName
                };

                return Ok(DetailJson);
            }
            else
            {
                // 如果未找到相應的產品，則返回 404 Not Found
                return NotFound(new { error = "Product not found" });
            }
        }

    }
}
