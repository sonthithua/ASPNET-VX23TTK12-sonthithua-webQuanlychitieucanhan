using System.Linq;
using System.Web.Mvc;
using QuanLyThuChi.Data;
using QuanLyThuChi.Models;
using ClosedXML.Excel;
using System.IO;
using System;

namespace QuanLyThuChi.Controllers
{
    public class TransactionController : Controller
    {
        private AppDbContext db = new AppDbContext();

        // 📋 Danh sách giao dịch
        public ActionResult Index()
        {
            if (Session["UserId"] == null)
                return RedirectToAction("Login", "Account");

            int userId = (int)Session["UserId"];
            var transactions = db.Transactions
                .Include("Category")
                .Where(t => t.UserId == userId)
                .OrderByDescending(t => t.TransactionDate)
                .ToList();

            return View(transactions);
        }

        // ➕ Hiển thị form thêm giao dịch
        public ActionResult Create()
        {
            if (Session["UserId"] == null)
                return RedirectToAction("Login", "Account");

            LoadDropdowns();
            return View(new Transaction
            {
                TransactionDate = System.DateTime.Now
            });
        }

        // 💾 Thêm giao dịch mới (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Transaction transaction)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("Login", "Account");

            if (!ModelState.IsValid)
            {
                ViewBag.Error = "Vui lòng nhập đầy đủ thông tin hợp lệ.";
                LoadDropdowns();
                return View(transaction);
            }

            transaction.UserId = (int)Session["UserId"];
            if (transaction.TransactionDate == default)
                transaction.TransactionDate = System.DateTime.Now;

            db.Transactions.Add(transaction);
            db.SaveChanges();

            TempData["Success"] = "✅ Giao dịch đã được thêm thành công!";
            return RedirectToAction("Index");
        }

        // ✏️ Hiển thị form sửa giao dịch
        public ActionResult Edit(int? id)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("Login", "Account");

            if (id == null)
                return RedirectToAction("Index");

            var transaction = db.Transactions.Find(id);
            if (transaction == null)
            {
                TempData["Error"] = "❌ Không tìm thấy giao dịch cần sửa.";
                return RedirectToAction("Index");
            }

            LoadDropdowns();
            return View(transaction);
        }

        // 💾 Cập nhật giao dịch (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Transaction transaction)
        {
            if (!ModelState.IsValid)
            {
                LoadDropdowns();
                return View(transaction);
            }

            db.Entry(transaction).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();

            TempData["Success"] = "✅ Cập nhật giao dịch thành công!";
            return RedirectToAction("Index");
        }

        // 🗑️ Hiển thị xác nhận xóa
        public ActionResult Delete(int? id)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("Login", "Account");

            if (id == null)
                return RedirectToAction("Index");

            var transaction = db.Transactions
                .Include("Category")
                .FirstOrDefault(t => t.Id == id);

            if (transaction == null)
            {
                TempData["Error"] = "❌ Không tìm thấy giao dịch để xóa.";
                return RedirectToAction("Index");
            }

            return View(transaction);
        }

        // 🗑️ Xác nhận xóa (POST)
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var transaction = db.Transactions.Find(id);
            if (transaction == null)
            {
                TempData["Error"] = "❌ Giao dịch không tồn tại hoặc đã bị xóa.";
                return RedirectToAction("Index");
            }

            db.Transactions.Remove(transaction);
            db.SaveChanges();

            TempData["Success"] = "🗑️ Giao dịch đã bị xóa thành công!";
            return RedirectToAction("Index");
        }

        // 📊 Báo cáo tổng hợp thu – chi
        public ActionResult Report()
        {
            if (Session["UserId"] == null)
                return RedirectToAction("Login", "Account");

            int userId = (int)Session["UserId"];
            var tongChi = db.Transactions
                .Where(t => t.UserId == userId && t.Type == "Chi")
                .Sum(t => (decimal?)t.Amount) ?? 0;

            var tongThu = db.Transactions
                .Where(t => t.UserId == userId && t.Type == "Thu")
                .Sum(t => (decimal?)t.Amount) ?? 0;

            ViewBag.TongChi = tongChi;
            ViewBag.TongThu = tongThu;
            ViewBag.CanDoi = tongThu - tongChi;

            return View();
        }

        // ⚙️ Load dropdown danh mục
        private void LoadDropdowns()
        {
            ViewBag.CategoryId = new SelectList(db.Categories.ToList(), "Id", "Name");
        }

        public ActionResult ExportExcel()
        {
            int userId = Convert.ToInt32(Session["UserId"]);

            // Lấy danh sách giao dịch của người dùng
            var transactions = db.Transactions
                .Where(t => t.UserId == userId)
                .Select(t => new
                {
                    t.Id,
                    CategoryName = t.Category.Name,
                    t.Type,
                    t.Amount,
                    t.Note,
                    t.TransactionDate
                }).ToList();

            // Tính tổng thu và tổng chi
            var totalThu = transactions.Where(t => t.Type == "Thu").Sum(t => t.Amount);
            var totalChi = transactions.Where(t => t.Type == "Chi").Sum(t => t.Amount);
            var balance = totalThu - totalChi;

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("GiaoDich");

                // Tạo tiêu đề cột và in đậm
                worksheet.Cell(1, 1).Value = "Mã giao dịch";
                worksheet.Cell(1, 2).Value = "Danh mục";
                worksheet.Cell(1, 3).Value = "Loại";
                worksheet.Cell(1, 4).Value = "Số tiền (VNĐ)";
                worksheet.Cell(1, 5).Value = "Ghi chú";
                worksheet.Cell(1, 6).Value = "Ngày giao dịch";
                worksheet.Range(1, 1, 1, 6).Style.Font.Bold = true;

                // Ghi dữ liệu
                int row = 2;
                foreach (var t in transactions)
                {
                    worksheet.Cell(row, 1).Value = t.Id;
                    worksheet.Cell(row, 2).Value = t.CategoryName;
                    worksheet.Cell(row, 3).Value = t.Type;

                    // Định dạng số tiền
                    worksheet.Cell(row, 4).Value = t.Amount;
                    worksheet.Cell(row, 4).Style.NumberFormat.Format = "#,##0 \"₫\"";

                    worksheet.Cell(row, 5).Value = t.Note;
                    worksheet.Cell(row, 6).Value = t.TransactionDate.ToString("dd/MM/yyyy");
                    row++;
                }

                // Thêm dòng tổng
                row++; // bỏ 1 dòng trống
                worksheet.Cell(row, 3).Value = "Tổng Thu:";
                worksheet.Cell(row, 3).Style.Font.Bold = true;
                worksheet.Cell(row, 4).Value = totalThu;
                worksheet.Cell(row, 4).Style.NumberFormat.Format = "#,##0 \"₫\"";
                worksheet.Cell(row, 4).Style.Font.Bold = true;

                row++;
                worksheet.Cell(row, 3).Value = "Tổng Chi:";
                worksheet.Cell(row, 3).Style.Font.Bold = true;
                worksheet.Cell(row, 4).Value = totalChi;
                worksheet.Cell(row, 4).Style.NumberFormat.Format = "#,##0 \"₫\"";
                worksheet.Cell(row, 4).Style.Font.Bold = true;

                row++;
                worksheet.Cell(row, 3).Value = "Số Dư:";
                worksheet.Cell(row, 3).Style.Font.Bold = true;
                worksheet.Cell(row, 4).Value = balance;
                worksheet.Cell(row, 4).Style.NumberFormat.Format = "#,##0 \"₫\"";
                worksheet.Cell(row, 4).Style.Font.Bold = true;

                worksheet.Columns().AdjustToContents();

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    return File(content,
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        "GiaoDich.xlsx");
                }
            }
        }



    }
}
