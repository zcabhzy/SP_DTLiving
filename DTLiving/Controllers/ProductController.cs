using DTLiving.Context;
using DTLiving.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace DTLiving.Controllers
{

    public class ProductController : Controller
    {
        #region Function => ProductController()

        private readonly DTContext _context;

        /// <summary>
        /// 控制器的建構函式，初始化 <see cref="DTContext"/> 物件。
        /// </summary>
        /// <param name="context">資料庫上下文。</param>
        public ProductController(DTContext context)
        {
            _context = context;
        }

        #endregion

        #region Function => Index()、LoadData()

        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 載入產品表格的資料
        /// </summary>
        /// <returns> 包含產品表格資料的 JSON 物件 </returns>
        [Route("/Product/LoadData")]
        public IActionResult LoadData()
        {
            try
            {
                // 取得分頁和搜尋的參數
                var draw = HttpContext.Request.Form["draw"].FirstOrDefault();
                var start = Request.Form["start"].FirstOrDefault();
                var length = Request.Form["length"].FirstOrDefault();
                var searchString = Request.Form["search[value]"].FirstOrDefault();
                var sortColumn = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();
                var sortColumnDirection = Request.Form["order[0][dir]"].FirstOrDefault();

                // 初始化分頁和總紀錄數變數
                int pageSize = length != null ? Convert.ToInt32(length) : 0;
                int skip = start != null ? Convert.ToInt32(start) : 0;
                int recordsTotal = 0;

                // 查詢以取得包含分類資訊的產品資料
                var productData = from m in _context.Product.Include(p => p.Category)
                                  select new {
                                      m.ID,
                                      m.NAME,
                                      Category = m.Category.CategoryName,
                                      m.PRICE,
                                      m.STOCK
                                  };

                // 如果提供了搜尋字串，則應用搜尋篩選
                if (!string.IsNullOrEmpty(searchString))
                {
                    productData = productData.Where(m => m.NAME.Contains(searchString) || m.Category.Contains(searchString));
                }

                // 計算篩選後的總紀錄數
                recordsTotal = productData.Count();

                // 分頁資料並轉換為清單
                var data = productData.Skip(skip).Take(pageSize).ToList();

                // 回傳包含分頁和資料的 JSON 回應
                return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });
            }
            catch (Exception)
            {
                return Json(new { error = "An error occurred while processing the request." });
            }
        }

        #endregion

        #region Function => Create()、Edit()、Delete()、DeleteConfirmed()

        /// <summary>
        /// 發送 GET 請求時,顯示用於創建商品的表單。
        /// </summary>
        /// <returns>
        /// 包含商品品牌資料選項的 View 物件。
        /// </returns>
        [HttpGet]
        public IActionResult Create()
        {
            ViewData["Categories"] = new SelectList(
               _context.Set<Category>(), "ID", "CategoryName");

            return View();
        }

        /// <summary>
        /// 發送 POST 請求時,處理提交的商品表單,將圖片轉換為2進位的方式儲存至 IMAGE 屬性中
        /// 商品新增至資料庫,重新導向至商品列表頁面。將商品品牌的資料選項加入商品模型中
        /// </summary>
        /// <param name="product"> 商品模型 </param>
        /// <param name="myimg"> 商品圖片 </param>
        /// <returns> 網頁視圖或重新導向至商品列表頁面 </returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product, IFormFile myimg)
        {
            if (ModelState.IsValid)
            {

                if (myimg != null)
                {
                    using (var ms = new MemoryStream())
                    {
                        myimg.CopyTo(ms);
                        product.IMAGE = ms.ToArray();
                    }
                }
                _context.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["Categories"] = new SelectList(
               _context.Set<Category>(), "ID", "CategoryName");

            return View(product);
        }

        /// <summary>
        /// 從資料庫中查詢要刪除的商品,如果找到對應的商品,將其從資料庫中移除並儲存變更。否則回傳刪除失敗訊息。
        /// </summary>
        /// <param name="id"> 商品 ID </param>
        /// <returns> JSON 物件表示刪除成功或失敗 </returns>
        [Route("/Product/Delete")]
        public async Task<IActionResult> Delete(int? id)
        {

            var product = await _context.Product.FirstOrDefaultAsync(p => p.ID == id);

            if (product != null)
            {
                _context.Product.Remove(product);
                await _context.SaveChangesAsync();
                return Json(new { success = true });
            }

            return Json(new { success = false, errorMessage = "Product not found." });
        }

        /// <summary>
        /// 
        /// 從資料庫中查詢要編輯的商品,如果找不到商品則回傳 404 Not Found。如果商品有圖片,將其轉換為 Base64 字串並傳遞到視圖中
        /// 將商品分類的選項資料傳遞到視圖中,並顯示編輯表單
        /// 
        /// </summary>
        /// <param name="id"> 商品 ID </param>
        /// <returns> 商品編輯視圖或 404 Not Found 錯誤 </returns>
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {

            var EditId = await _context.Product.FindAsync(id);

            if (EditId == null)
            {
                return NotFound();
            }

            if (EditId.IMAGE != null)
            {
                ViewBag.Base64Image = Convert.ToBase64String(EditId.IMAGE);
            }

            ViewData["Categories"] = new SelectList(_context.Set<Category>(),
                "ID", "CategoryName", EditId.CategoryId);

            return View(EditId);
        }

        /// <summary>
        /// 編輯產品的詳細資訊。
        /// </summary>
        /// <param name="id"> 產品ID </param>
        /// <param name="product"> 需要更新資訊的 Product 物件 </param>
        /// <param name="myimg"> 新圖片 </param>
        /// <returns> 如果編輯成功,則重新導向至產品列表頁面；否則,返回編輯頁面以顯示錯誤信息 </returns>
        [HttpPost]
        public async Task<IActionResult> Edit(int? id, Product product, IFormFile myimg)
        {

            if (id != product.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {

                    if (myimg != null)
                    {
                        using (var ms = new MemoryStream())
                        {
                            myimg.CopyTo(ms);
                            product.IMAGE = ms.ToArray();
                        }
                    }

                    _context.Update(product);
                    await _context.SaveChangesAsync();

                }
                catch (DbUpdateConcurrencyException)
                {

                    if (!ProductExists(product.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                return RedirectToAction(nameof(Index));
            }

            return View(product);
        }


        /// <summary>
        /// 顯示指定產品的詳細資訊
        /// </summary>
        /// <param name="id"> 顯示詳細資訊的產品 ID </param>
        /// <returns> 如果找到指定 ID 的產品,則返回該產品的詳細資訊；否則返回 404 Not Found 頁面 </returns>
        [HttpGet]
        public async Task<IActionResult> Detail(int? id)
        {
            // 若查詢無產品 Id,回傳 404 Not Found
            if (id == null)
            {
                return NotFound();
            }

            DetailViewModel dvm = new DetailViewModel();

            var product = await _context.Product
                        .Include(p => p.Category)
                        .FirstOrDefaultAsync(m => m.ID == id);

            if (product == null)
            {
                return NotFound();
            }
            else
            {
                dvm.Product = product;
                dvm.Category = product.Category;
                if (product.IMAGE != null)
                {
                    dvm.imgsrc = ViewImage(product.IMAGE);
                }
            }
            return View(dvm);
        }

        #endregion

        #region Function => CreateCategory()

        /// <summary>
        /// 顯示商品品牌畫面
        /// </summary>
        /// <returns> 包含用於創建產品類別的表單的視圖 </returns>
        [HttpGet]
        public IActionResult CreateCategory()
        {
            return View();
        }

        /// <summary>
        /// 處理提交的商品品牌表單,將新建的品牌存入資料庫
        /// </summary>
        /// <param name="category"> 要新增的產品類別 </param>
        /// <returns> 重定向到創建產品類別的頁面 </returns>
        [HttpPost]
        public async Task<IActionResult> CreateCategory(Category category)
        {
            _context.Category.Add(category);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(CreateCategory));

        }

        #endregion

        #region Function => ProductExists()

        /// <summary>
        /// 確認指定 ID 的產品是否存在於資料庫中
        /// </summary>
        /// <param name="id"> 產品ID </param>
        /// <returns> 如果資料庫中存在具有指定 ID 的產品,則為 true；否則為 false </returns>
        private bool ProductExists(int id)
        {
            return _context.Product.Any(e => e.ID == id);
        }

        #endregion

        #region Function => ViewImage()

        /// <summary>
        /// 將 byte 陣列轉換為 Base64 字串,以供在網頁上顯示圖片
        /// </summary>
        /// <param name="arrayImage"> 轉換的 byte 陣列 </param>
        /// <returns> 圖片的 Base64 字串，用於在網頁上顯示圖片 </returns>
        public string ViewImage(byte[] arrayImage)
        {
            string base64String = Convert.ToBase64String(arrayImage, 0, arrayImage.Length);

            return "data:image/png;base64," + base64String;
        }

        #endregion

    }
}
