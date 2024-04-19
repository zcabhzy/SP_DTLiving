using DTLiving.Context;
using DTLiving.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IO;

namespace DTLiving.Controllers
{

    public class EmployerController : Controller
    {
        #region Function => EmployerController()

        private readonly DTContext _context;

        /// <summary>
        /// 控制器的建構函式，初始化 <see cref="DTContext"/> 物件。
        /// </summary>
        /// <param name="context">資料庫上下文。</param>
        public EmployerController(DTContext context)
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
        /// 載入員工表格的資料
        /// </summary>
        /// <returns> 包含員工表格資料的 JSON 物件 </returns>
        [Route("/Employer/LoadData")]
        public IActionResult LoadData()
        {
            try
            {
                var draw = HttpContext.Request.Form["draw"].FirstOrDefault();
                var start = Request.Form["start"].FirstOrDefault();
                var length = Request.Form["length"].FirstOrDefault();
                var searchString = Request.Form["search[value]"].FirstOrDefault();
                var sortColumn = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();
                var sortColumnDirection = Request.Form["order[0][dir]"].FirstOrDefault();

                int pageSize = length != null ? Convert.ToInt32(length) : 0;
                int skip = start != null ? Convert.ToInt32(start) : 0;
                int recordsTotal = 0;

                var employerdata = from m in _context.Employer
                                   select new {
                                       m.Id,
                                       m.StaffId,
                                       m.Gender,
                                       m.ClerkName,
                                       m.ClerkPhone,
                                       m.ClerkAddress
                                   };

                if (!string.IsNullOrEmpty(searchString))
                {
                    employerdata = employerdata.Where(m => m.ClerkName.Contains(searchString));
                }

                recordsTotal = employerdata.Count();

                var data = employerdata.Skip(skip).Take(pageSize).ToList();

                return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });
            }
            catch (Exception)
            {
                return Json(new { error = "An error occurred while processing the request." });
            }
        }

        #endregion

        #region Function => Create()

        /// <summary>
        /// 員工註冊功能
        /// </summary>
        /// <returns> 包含用於創建員工的視圖 </returns>
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// 員工註冊功能 : 將員工資料新增至資料庫。如果提供了圖片,則將其轉換為位元組並存儲在雇主模型中。
        /// </summary>
        /// <param name="employer"> 員工模型資料 </param>
        /// <param name="employerimage"> 員工圖片 </param>
        /// <returns> 成功註冊員工，重新導向至員工列表 ; 否則,返回創建頁面以重新填寫表單 </returns>
        [HttpPost]
        public async Task<IActionResult> Create(Employer employer, IFormFile employerimage)
        {
            if (ModelState.IsValid)
            {
                if (employerimage != null)
                {
                    var ms = new MemoryStream();
                    employerimage.CopyTo(ms);
                    employer.ClerkImage = ms.ToArray();
                }
                _context.Add(employer);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View();
        }

        #endregion

        #region Function => EmployerLogin()

        [HttpGet]
        [Route("/Employer/EmployerLogin")]
        public IActionResult EmployerLogin(string staffId)
        {
            var StaffIDCheck = _context.Employer.FirstOrDefault
                (e => e.StaffId == staffId);

            if (StaffIDCheck != null)
            {
                return Ok(new { message = "登入成功" });
            }
            else
            {
                return NotFound(new { message = "員工編號不存在" });
            }
        }

        #endregion
    }
}
