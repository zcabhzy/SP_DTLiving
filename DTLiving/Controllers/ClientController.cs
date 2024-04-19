using DTLiving.Context;
using DTLiving.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Newtonsoft.Json;
using System.ComponentModel;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using DTLiving.JWT;

namespace DTLiving.Controllers
{
    public class ClientController : Controller
    {
        #region Function => ClientController()

        private readonly DTContext _context;

        /// <summary>
        /// 控制器的建構函式，初始化 <see cref="DTContext"/> 物件。
        /// </summary>
        /// <param name="context">資料庫上下文。</param>
        public ClientController(DTContext context)
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
        /// 載入會員表格的資料。
        /// </summary>
        /// <returns> 包含會員表格資料的 JSON 物件。 </returns>
        [Route("/Client/LoadData")]
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

                // 查詢以取得會員資料
                var productData = from m in _context.Client
                                  select new {
                                      m.Id,
                                      m.UserID,
                                      m.UserName,
                                      m.UserPhone,
                                      m.UserMail
                                  };

                // 如果提供了搜尋字串，則應用搜尋篩選
                if (!string.IsNullOrEmpty(searchString))
                {
                    productData = productData.Where(m => m.UserName.Contains(searchString));
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

        #region Function => ClientCheck()、ClientLogin()

        [HttpGet]
        public IActionResult ClientCheck()
        {
            return View();
        }

        /// <summary>
        /// 註冊會員功能 : 接收的會員資料與 Client 模型相同,存入資料庫
        /// </summary>
        /// <param name="client"> 會員物件 </param>
        /// <returns> 若註冊成功,回傳狀態碼 200 OK 並顯示註冊成功訊息；若發生錯誤,回傳狀態碼 500 Internal Server Error 並顯示錯誤訊息 </returns>
        [HttpPost]
        [Route("/Client/ClientCheck")]
        public async Task<IActionResult> ClientCheck([FromBody] Client client)
        {
            try
            {
                _context.Client.Add(client);
                await _context.SaveChangesAsync();
                return Ok("註冊成功");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"發生錯誤：{ex.Message}");
                return StatusCode(500, "伺服器內部錯誤：" + ex.Message);
            }
        }

        [HttpGet]
        public IActionResult ClientLogin()
        {
            return View();
        }

        /// <summary>
        /// 1. 會員登入功能 : 客戶端發送的會員資料,驗證會員帳號和密碼是否正確。
        /// 2. 如果驗證通過,生成 JWT 令牌並返回至客戶端 ; 否則回傳 BadRequest 並提示帳號或密碼錯誤。
        /// </summary>
        /// <param name="client"> 包含會員帳號和密碼的會員物件 </param>
        /// <returns>
        /// 1. 若驗證通過,回傳狀態碼 200 OK 並返回包含 JWT 令牌和會員資訊的 JSON 物件。
        /// 2. 若帳號或密碼錯誤,回傳狀態碼 400 Bad Request 並提示錯誤訊息。
        /// 3. 若發生其他錯誤,回傳狀態碼 500 Internal Server Error 並顯示錯誤訊息。
        /// </returns>
        [HttpPost]
        [Route("/Client/ClientLogin")]
        public async Task<IActionResult> ClientLogin([FromBody] Client client)
        {
            try
            {
                // 根據客戶端發送的會員帳號和密碼，從資料庫中查詢會員資料
                var user = await _context.Client
                    .FirstOrDefaultAsync(c => c.UserID == client.UserID && c.UserPassword == client.UserPassword);

                // 如果找到符合條件的會員
                if (user != null)
                {
                    // JWT 生成
                    var token = JwtHandler.GenerateJwtToken(user.UserID, user.UserName, "UserRole");

                    // 回應物件 : JWT 和會員資訊
                    var response = new {
                        Token = token,
                        UserName = user.UserName,
                        UserMail = user.UserMail,
                        UserPhone = user.UserPhone,
                        UserAddress = user.UserAddress
                    };

                    // 返回成功訊息和會員資訊
                    return Ok(response);
                }
                else
                {
                    // 返回帳號或密碼錯誤訊息
                    return BadRequest("帳號或密碼錯誤");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"發生錯誤：{ex.Message}");
                // 返回伺服器內部錯誤訊息
                return StatusCode(500, "伺服器內部錯誤：" + ex.Message);
            }
        }

        /// <summary>
        /// 1. 刪除客戶端資料功能：從資料庫中查詢指定 ID 的客戶端資料，如果找到對應的客戶端，則從資料庫中刪除它並儲存變更。
        /// 2. 如果客戶端不存在，則返回 JSON 物件表示刪除失敗並包含錯誤訊息。
        /// </summary>
        /// <param name="id"> 客戶ID </param>
        /// <returns>
        /// 1. 如果成功刪除客戶端,則返回 JSON 物件表示刪除成功 ;
        /// 2. 如果未找到對應的客戶端,則返回 JSON 物件表示刪除失敗並包含錯誤訊息。
        /// </returns>
        [Route("/Client/Delete")]
        public async Task<IActionResult> Delete(int? id)
        {
            // 從資料庫中查詢指定 ID 的客戶端資料
            var client = await _context.Client.FirstOrDefaultAsync(p => p.Id == id);

            // 如果找到指定的客戶端
            if (client != null)
            {
                // 從資料庫中刪除客戶端
                _context.Client.Remove(client);
                await _context.SaveChangesAsync();
                return Json(new { success = true });
            }

            return Json(new { success = false, errorMessage = "Client not found." });
        }

        #endregion

        #region Function => Detail()、

        

        #endregion
    }
}
