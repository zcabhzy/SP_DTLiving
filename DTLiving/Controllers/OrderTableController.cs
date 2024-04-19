using DTLiving.Context;
using DTLiving.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;

namespace DTLiving.Controllers
{
    public class OrderTableController : Controller
    {
        private readonly DTContext _context;

        /// <summary>
        /// 控制器的建構函式，初始化 <see cref="DTContext"/> 物件。
        /// </summary>
        /// <param name="context">資料庫上下文。</param>
        public OrderTableController(DTContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        #region Function => CreateOrder()、ShowOrder()

        /// <summary>
        /// 至資料庫中查詢目前使用者是否有訂單資訊
        /// </summary>
        /// 
        /// <remarks>
        /// 檢查目前使用者是否有相關的訂單資訊，並以訂單編號分組返回相關資訊。
        /// </remarks>
        /// 
        /// <returns>
        /// 包含使用者訂單資訊的 JSON 物件，以訂單編號分組，每個訂單包含相關商品明細的清單。
        /// </returns>
        [HttpGet]
        [Route("/OrderTable/ShowOrder")]
        public IActionResult ShowOrder()
        {
            try
            {
                // 解析前端傳送的 Token
                string authHeader = Request.Headers["Authorization"];
                string jwtToken = authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase) ? authHeader.Substring("Bearer ".Length).Trim() : null;

                string userName = GetUserNameFromJwtToken(jwtToken);

                var userOrders = _context.OrderTable
                    .Where(order => order.CatchUser == userName)
                    .GroupBy(order => order.OrderNumber) // 以訂單編號分組
                    .Select(group => new {
                        Receiver = userName,
                        OrderNumber = group.Key, // 訂單編號
                        IsPad = _context.ShopOrder
                    .Where(shopOrder => shopOrder.OrderNumber == group.Key)
                    .Select(shopOrder => shopOrder.IsPad)
                    .FirstOrDefault(), // 取得訂單的付款狀態
                        ClientInfo = _context.Client
                    .Where(client => client.UserName == userName)
                    .Select(client => new {
                        UserPhone = client.UserPhone,
                        UserAddress = client.UserAddress
                    }).FirstOrDefault(),
                        CreateTime = _context.ShopOrder
                        .Where(shopOrder => shopOrder.OrderNumber == group.Key)
                        .Select(shopOrder => shopOrder.CreateTime)
                        .FirstOrDefault(),
                        Items = group.Select(order => new {
                            ProductId = order.ProductId,
                            Name = order.Name,
                            OrderCategory = order.OrderCategory,
                            Price = order.Price,
                            Stock = order.Stock,
                            Quantity = order.Quantity,
                            Subtotal = order.Subtotal
                        }).ToList(), // 同訂單編號下的商品明細
                        Total = group.Sum(order => order.Subtotal)
                    }).ToList();

                return Json(userOrders); // 返回使用者訂單資訊的 JSON 物件
            }
            catch (Exception ex)
            {
                return StatusCode(500, "獲取訂單失敗" + ex.Message); // 返回伺服器內部錯誤訊息
            }
        }

        /// <summary>
        /// 接收商店訂單並顯示
        /// </summary>
        /// <param name="shopOrder"> 商店訂單資料 </param>
        /// <returns> 視圖結果 </returns>
        [HttpPost]
        async Task<IActionResult> ShowOrder(ShopOrder shopOrder)
        {
            return View();
        }

        /// <summary>
        /// 建立新訂單的頁面
        /// </summary>
        /// <returns> 視圖結果 </returns>
        [HttpGet]
        public IActionResult CreateOrder()
        {
            return View();
        }

        /// <summary>
        /// 建立新訂單
        /// </summary>
        /// <param name="orderTables"> 訂單表格列表 </param>
        /// <returns> 操作結果 </returns>
        [HttpPost]
        [Route("/OrderTable/CreateOrder")]
        public async Task<IActionResult> CreateOrder([FromBody] List<OrderTable> orderTables)
        {
            try
            {
                int CountPrice = 0;
                string orderNumber = GenerateOrderNumber(); // 生成訂單編號
                DateTime currentTimeUtc = DateTime.Now;

                string authHeader = Request.Headers["Authorization"];
                string jwtToken = authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase) ? authHeader.Substring("Bearer ".Length).Trim() : null;

                string userName = GetUserNameFromJwtToken(jwtToken); // 從 JWT Token 解析出使用者名稱

                // 檢查訂單項目是否為空
                if (orderTables == null || orderTables.Count == 0)
                {
                    return BadRequest("訂單項目不能為空");
                }

                var shopOrder = new ShopOrder
                {
                    OrderNumber = orderNumber,
                    IsPad = false
                };

                // 遍歷訂單項目，建立訂單表格
                foreach (var orderTableInput in orderTables)
                {
                    var orderTable = new OrderTable
                    {
                        ProductId = orderTableInput.ProductId,
                        Name = orderTableInput.Name,
                        OrderCategory = orderTableInput.OrderCategory,
                        Price = orderTableInput.Price,
                        Stock = orderTableInput.Stock,
                        Quantity = orderTableInput.Quantity,
                        OrderNumber = orderNumber,
                        CatchUser = userName,
                        Subtotal = orderTableInput.Price * orderTableInput.Quantity
                    };

                    CountPrice += orderTable.Subtotal;
                    shopOrder.orderTables.Add(orderTable);
                }

                // 設置訂單收件人、總價、建立時間
                shopOrder.Receiver = userName;
                shopOrder.Total = CountPrice;
                shopOrder.CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm");

                _context.ShopOrder.Add(shopOrder);
                await _context.SaveChangesAsync();
                return Ok("訂單建立成功");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "伺服器無法連接" + ex.Message);
            }

        }

        /// <summary>
        /// 付款功能
        /// </summary>
        /// <param name="orderNumber"> 訂單編號 </param>
        /// <returns>  </returns>
        [HttpPost]
        [Route("/OrderTable/IsPadCheck")]
        public IActionResult IsPadCheck([FromBody] string orderNumber)
        {
            try
            {
                var checknumber = _context.ShopOrder.FirstOrDefault
                    (order => order.OrderNumber == orderNumber);

                if (checknumber != null)
                {
                    checknumber.IsPad = true;

                    _context.SaveChanges();

                    return Ok("成功付款");
                }
                else
                {
                    return BadRequest("查無此訂單");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, "處理請求時出現錯誤：" + ex.Message);
            }
        }

        #endregion

        #region Function => GetUserNameFromJwtToken()、GenerateOrderNumber()

        /// <summary>
        /// JWT Token 解析
        /// </summary>
        /// <param name="jwtToken"> JWT Token 字串 </param>
        /// <returns> Return JWT Token 內的 UserName </returns>
        private string GetUserNameFromJwtToken(string jwtToken)
        {
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(jwtToken);

            var usernameClaim = token.Claims.FirstOrDefault(claim => claim.Type == "UserName");
            if (usernameClaim != null)
            {
                return usernameClaim.Value;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 生成訂單編號
        /// </summary>
        /// <returns> 訂單編號 : "0000xxxxxxxx"</returns>
        public string GenerateOrderNumber()
        {
            using (RandomNumberGenerator rnd = RandomNumberGenerator.Create())
            {
                // [8] byte Number for 0000xxxxxxxx
                byte[] RndNumber = new byte[8];
                rnd.GetBytes(RndNumber);

                // 將 byte[] 轉會為一個 8 位數的字串
                string RndString = string.Concat(RndNumber.Select(b => b % 10));

                // 與 0000 和隨機數字進行合併
                string orderNumber = "0000" + RndString;
                return orderNumber;
            }
        }

        #endregion

    }
}
