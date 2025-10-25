using System;
using System.Linq;
using System.Web.Mvc;
using QuanLyThuChi.Data;
using QuanLyThuChi.Models;
using System.Data.Entity;

namespace QuanLyThuChi.Controllers
{
    public class BudgetsController : Controller
    {
        private AppDbContext db = new AppDbContext();

        // GET: Budgets
        public ActionResult Index()
        {
            var budgets = db.Budgets.ToList();

            // ✅ Lặp qua từng ngân sách để tính tổng chi tiêu thật
            foreach (var budget in budgets)
            {
                var spent = db.Transactions
    .Where(t => t.CategoryId == budget.CategoryId
             && t.TransactionDate.Month == budget.Month
             && t.TransactionDate.Year == budget.Year
             && t.UserId == budget.UserId
             && t.Type == "Chi") // ✅ chỉ tính chi tiêu
    .Sum(t => (decimal?)t.Amount) ?? 0;

                budget.Note = $"Đã chi: {spent:N0} VNĐ"; // gắn tạm vào Note để hiển thị
                                                         // Bạn có thể tạo ViewModel riêng nếu muốn chuyên nghiệp hơn
            }

            return View(budgets);
        }


        // GET: Budgets/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);

            var budget = db.Budgets.Find(id);
            if (budget == null)
                return HttpNotFound();

            return View(budget);
        }

        // GET: Budgets/Create
        public ActionResult Create()
        {
            ViewBag.CategoryId = new SelectList(db.Categories, "Id", "Name");
            return View();
        }

        // POST: Budgets/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Budgets model)
        {
            if (ModelState.IsValid)
            {
                // Gán giá trị mặc định
                model.UserId = (int)Session["UserId"];   // Nếu có quản lý user đăng nhập
                model.Month = DateTime.Now.Month;
                model.Year = DateTime.Now.Year;
                model.CreatedAt = DateTime.Now;

                db.Budgets.Add(model);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CategoryId = new SelectList(db.Categories, "Id", "Name", model.CategoryId);
            return View(model);
        }

        // GET: Budgets/Edit/5
        public ActionResult Edit(int id)
        {
            var budget = db.Budgets.Find(id);
            if (budget == null)
            {
                return HttpNotFound();
            }
            return View(budget);
        }

        // POST: Budgets/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Budgets model)
        {
            if (ModelState.IsValid)
            {
                // Giữ nguyên khóa ngoại (UserId, CategoryId)
                var existing = db.Budgets.AsNoTracking().FirstOrDefault(x => x.Id == model.Id);
                if (existing != null)
                {
                    model.UserId = existing.UserId;
                    model.CategoryId = existing.CategoryId;
                }

                db.Entry(model).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CategoryId = new SelectList(db.Categories, "Id", "Name", model.CategoryId);
            return View(model);
        }



        // GET: Budgets/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);

            var budget = db.Budgets.Find(id);
            if (budget == null)
                return HttpNotFound();

            return View(budget);
        }

        // POST: Budgets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                var budget = db.Budgets.Find(id);
                db.Budgets.Remove(budget);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch (System.Data.Entity.Infrastructure.DbUpdateException)
            {
                ViewBag.ErrorMessage = "Không thể xóa vì dữ liệu này đang được sử dụng ở nơi khác!";
                var budget = db.Budgets.Find(id);
                return View("Delete", budget);
            }
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
