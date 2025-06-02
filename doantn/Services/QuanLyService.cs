using Dapper;
using System.Data.SqlClient;
using System.Data;
using doantn.Models;
using Microsoft.Data.SqlClient;
using doantn.ViewModel;

namespace doantn.Services
{
    public class QuanLyService
    {
        private readonly IConfiguration _config;
        private readonly string _connectionString;
        private readonly IWebHostEnvironment _env;

        public QuanLyService(IConfiguration config, IWebHostEnvironment env)
        {
            _config = config;
            _connectionString = _config.GetConnectionString("DefaultConnection");
            _env = env;
        }
        public async Task<List<SachQuanLyViewModel>> GetDanhSachSachQuanLy()
        {
            using var conn = new SqlConnection(_connectionString);

            var sql = @"
            SELECT 
            s.MaSach, 
            s.TenSach, 
            s.TenTacGia, 
            l.TenLoai, 
            s.Anh,
            COUNT(c.MaChuong) AS SoChuong
            FROM Sach s
            JOIN Loai l ON s.MaLoai = l.MaLoai
            LEFT JOIN Chuong c ON s.MaSach = c.MaSach
            GROUP BY s.MaSach, s.TenSach, s.TenTacGia, l.TenLoai, s.Anh, s.ThoiGianCatNhap
            ORDER BY MAX(s.ThoiGianCatNhap) DESC";

            var result = await conn.QueryAsync<SachQuanLyViewModel>(sql);
            return result.ToList();
        }
        public async Task<bool> ThemSach(SachThemMoiViewModel model)
        {
            using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();
            using var tran = conn.BeginTransaction();
            try
            {
                string newMaSach = Guid.NewGuid().ToString();
                string? anhPath = await SaveImage(model.AnhBia);
                var sqlInsertSach = @"INSERT INTO Sach (MaSach, TenSach, TenTacGia, GioiThieu, MaLoai, LuotXem, LuotTai, Anh, Nguon, ThoiGianCatNhap, MaCap)
                                      VALUES (@MaSach, @TenSach, @TenTacGia, @GioiThieu, @MaLoai, 0, 0, @Anh, @Nguon, GETDATE(), @MaCap)";
                await conn.ExecuteAsync(sqlInsertSach, new
                {
                    MaSach = newMaSach,
                    model.TenSach,
                    model.TenTacGia,
                    model.GioiThieu,
                    model.MaLoai,
                    Anh = anhPath,
                    model.Nguon,
                    model.MaCap
                }, transaction: tran);
                foreach (var chuong in model.Chuongs)
                {
                    var maChuong = Guid.NewGuid().ToString();
                    await conn.ExecuteAsync(@"INSERT INTO Chuong (MaChuong, MaSach, TenChuong, ThuTuChuong)
                                              VALUES (@MaChuong, @MaSach, @TenChuong, 1)", new
                    {
                        MaChuong = maChuong,
                        MaSach = newMaSach,
                        TenChuong = chuong.TenChuong
                    }, transaction: tran);
                    await conn.ExecuteAsync(@"INSERT INTO Trang (MaTrang, MaChuong, NoiDung)
                                              VALUES (NEWID(), @MaChuong, @NoiDung)", new
                    {
                        MaChuong = maChuong,
                        NoiDung = chuong.NoiDung
                    }, transaction: tran);
                }
                if (model.FilesSach != null && model.FilesSach.Count > 0)
                {
                    foreach (var file in model.FilesSach)
                    {
                        if (file == null || file.Length == 0 || string.IsNullOrWhiteSpace(file.FileName))
                            continue; 
                        var filePath = await SaveFile(file);

                        await conn.ExecuteAsync(@"INSERT INTO FileSach (MaFile, MaSach, TenFile, LoaiFile, DuongDan, DungLuong, ThoiGianUp)
                                                  VALUES (NEWID(), @MaSach, @TenFile, @LoaiFile, @DuongDan, @DungLuong, GETDATE())", new
                        {
                            MaSach = newMaSach,
                            TenFile = Path.GetFileNameWithoutExtension(file.FileName),
                            LoaiFile = Path.GetExtension(file.FileName).TrimStart('.'),
                            DuongDan = filePath,
                            DungLuong = file.Length
                        }, transaction: tran);
                    }
                }
                tran.Commit();
                return true;
            }
            catch (Exception ex)
            {
                tran.Rollback();
                Console.WriteLine("Lỗi thêm sách: " + ex.Message); 
                throw;
            }
        }

        public async Task<string?> SaveImage(IFormFile file)
        {
            if (file == null) return null;
            var uploads = Path.Combine(_env.WebRootPath, "images");
            if (!Directory.Exists(uploads))
                Directory.CreateDirectory(uploads);
            var fileName = Path.GetFileName(file.FileName);
            var filePath = Path.Combine(uploads, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
            return "/images/" + fileName;
        }
        public async Task<string> SaveFile(IFormFile file)
        {
            var uploads = Path.Combine(_env.WebRootPath, "tep");
            if (!Directory.Exists(uploads))
                Directory.CreateDirectory(uploads);

            var fileName = Path.GetFileName(file.FileName);
            var filePath = Path.Combine(uploads, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
            return "/tep/" + fileName;
        }
        public async Task<SachThemMoiViewModel?> GetThongTinSach(string maSach)
        {
            using var conn = new SqlConnection(_connectionString);

            var sqlSach = @"
                SELECT MaSach, TenSach, TenTacGia, GioiThieu, MaLoai, Anh, Nguon, MaCap
                FROM Sach
                WHERE MaSach = @MaSach";

            var sach = await conn.QueryFirstOrDefaultAsync<SachThemMoiViewModel>(sqlSach, new { MaSach = maSach });
            if (sach == null) return null;

            var sqlChuong = @"
                SELECT c.MaChuong, c.TenChuong, t.NoiDung
                FROM Chuong c
                JOIN Trang t ON c.MaChuong = t.MaChuong
                WHERE c.MaSach = @MaSach
                ORDER BY c.ThuTuChuong ASC";

            var chuongs = await conn.QueryAsync<ChuongViewModel>(sqlChuong, new { MaSach = maSach });
            sach.Chuongs = chuongs.ToList();

            var sqlFiles = @"
                SELECT MaFile, TenFile, LoaiFile, DuongDan
                FROM FileSach
                WHERE MaSach = @MaSach";

            var files = await conn.QueryAsync<FileSachViewModel>(sqlFiles, new { MaSach = maSach });
            sach.FilesDaTai = files.ToList();


            return sach;
        }

        public async Task<bool> CapNhatSach(SachThemMoiViewModel model, string maSach)
        {
            using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();
            using var tran = conn.BeginTransaction();

            try
            {
                var fileCuCanXoa = model.DanhSachFileXoa?
                    .Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .ToList() ?? new List<string>();

                foreach (var maFile in fileCuCanXoa)
                {
                    var duongDan = await conn.ExecuteScalarAsync<string>(
                        "SELECT DuongDan FROM FileSach WHERE MaFile = @MaFile", new { MaFile = maFile }, tran);

                    if (!string.IsNullOrEmpty(duongDan))
                    {
                        var fullPath = Path.Combine(_env.WebRootPath, duongDan.TrimStart('/'));
                        if (File.Exists(fullPath)) File.Delete(fullPath);
                    }

                    await conn.ExecuteAsync("DELETE FROM FileSach WHERE MaFile = @MaFile", new { MaFile = maFile }, tran);
                }

                string? anhPath;

                if (model.AnhBia != null && model.AnhBia.Length > 0)
                {
                    anhPath = await SaveImage(model.AnhBia); 
                }
                else
                {
                    anhPath = model.Anh; 
                }
                var sqlUpdateSach = @"
                    UPDATE Sach 
                    SET 
                        TenSach = @TenSach,
                        TenTacGia = @TenTacGia,
                        GioiThieu = @GioiThieu,
                        MaLoai = @MaLoai,
                        Nguon = @Nguon,
                        Anh = @Anh,
                        MaCap = @MaCap
                    WHERE MaSach = @MaSach";

                await conn.ExecuteAsync(sqlUpdateSach, new
                {
                    TenSach = model.TenSach,
                    TenTacGia = model.TenTacGia,
                    GioiThieu = model.GioiThieu,
                    MaLoai = model.MaLoai,
                    Nguon = model.Nguon,
                    Anh = anhPath,
                    MaSach = maSach,
                    MaCap = model.MaCap,
                }, transaction: tran);
                await conn.ExecuteAsync(
                    "DELETE FROM Trang WHERE MaChuong IN (SELECT MaChuong FROM Chuong WHERE MaSach = @MaSach)",
                    new { MaSach = maSach }, transaction: tran);

                await conn.ExecuteAsync(
                    "DELETE FROM Chuong WHERE MaSach = @MaSach",
                    new { MaSach = maSach }, transaction: tran);
                if (model.Chuongs != null && model.Chuongs.Count > 0)
                {
                    int thuTu = 1;
                    foreach (var chuong in model.Chuongs)
                    {
                        var maChuong = Guid.NewGuid().ToString();

                        await conn.ExecuteAsync(@"
                    INSERT INTO Chuong (MaChuong, MaSach, TenChuong, ThuTuChuong)
                    VALUES (@MaChuong, @MaSach, @TenChuong, @ThuTuChuong)", new
                        {
                            MaChuong = maChuong,
                            MaSach = maSach,
                            TenChuong = chuong.TenChuong,
                            ThuTuChuong = thuTu++
                        }, transaction: tran);

                        await conn.ExecuteAsync(@"
                    INSERT INTO Trang (MaTrang, MaChuong, NoiDung)
                    VALUES (NEWID(), @MaChuong, @NoiDung)", new
                        {
                            MaChuong = maChuong,
                            NoiDung = chuong.NoiDung
                        }, transaction: tran);
                    }
                }

                if (model.FilesSach != null && model.FilesSach.Count > 0)
                {
                    foreach (var file in model.FilesSach)
                    {
                        var filePath = await SaveFile(file);

                        await conn.ExecuteAsync(@"
                    INSERT INTO FileSach (MaFile, MaSach, TenFile, LoaiFile, DuongDan, DungLuong, ThoiGianUp)
                    VALUES (NEWID(), @MaSach, @TenFile, @LoaiFile, @DuongDan, @DungLuong, GETDATE())", new
                        {
                            MaSach = maSach,
                            TenFile = Path.GetFileNameWithoutExtension(file.FileName),
                            LoaiFile = Path.GetExtension(file.FileName).TrimStart('.'),
                            DuongDan = filePath,
                            DungLuong = file.Length
                        }, tran);
                    }
                }

                tran.Commit();
                return true;
            }
            catch (Exception ex)
            {
                tran.Rollback();
                Console.WriteLine("Lỗi cập nhật sách: " + ex.Message);
                throw;
            }
        }

        public async Task XoaFileCu(IEnumerable<string> maFiles)
        {
            using var conn = new SqlConnection(_connectionString);
            foreach (var maFile in maFiles)
            {
                var duongDan = await conn.ExecuteScalarAsync<string>(
                    "SELECT DuongDan FROM FileSach WHERE MaFile = @MaFile", new { MaFile = maFile });

                if (!string.IsNullOrEmpty(duongDan))
                {
                    var fullPath = Path.Combine(_env.WebRootPath, duongDan.TrimStart('/'));
                    if (File.Exists(fullPath)) File.Delete(fullPath);
                }

                await conn.ExecuteAsync("DELETE FROM FileSach WHERE MaFile = @MaFile", new { MaFile = maFile });
            }
        }
        public async Task<bool> KiemTraTenSachTrung(string tenSach, string? maSach = null)
        {
            using var conn = new SqlConnection(_connectionString);
            var sql = @"
                SELECT COUNT(*) 
                FROM Sach 
                WHERE TenSach = @TenSach AND (@MaSach IS NULL OR MaSach != @MaSach)";

            var count = await conn.ExecuteScalarAsync<int>(sql, new { TenSach = tenSach, MaSach = maSach });
            return count > 0;
        }
        public async Task<bool> XoaSachAsync(string maSach)
        {
            using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();
            using var tran = conn.BeginTransaction();
            try
            {
                var filePaths = await conn.QueryAsync<string>(
                    "SELECT DuongDan FROM FileSach WHERE MaSach = @MaSach", new { MaSach = maSach }, tran);
                foreach (var path in filePaths)
                {
                    var fullPath = Path.Combine(_env.WebRootPath, path.TrimStart('/'));
                    if (File.Exists(fullPath)) File.Delete(fullPath);
                }
                await conn.ExecuteAsync("DELETE FROM FileSach WHERE MaSach = @MaSach", new { MaSach = maSach }, tran);
                await conn.ExecuteAsync("DELETE FROM Trang WHERE MaChuong IN (SELECT MaChuong FROM Chuong WHERE MaSach = @MaSach)", new { MaSach = maSach }, tran);
                await conn.ExecuteAsync("DELETE FROM Chuong WHERE MaSach = @MaSach", new { MaSach = maSach }, tran);
                await conn.ExecuteAsync("DELETE FROM Sach WHERE MaSach = @MaSach", new { MaSach = maSach }, tran);
                await conn.ExecuteAsync("DELETE FROM DanhGia WHERE MaSach = @MaSach", new { MaSach = maSach }, tran);
                await conn.ExecuteAsync("DELETE FROM BookMark WHERE MaSach = @MaSach", new { MaSach = maSach }, tran);
                await conn.ExecuteAsync("DELETE FROM BookMarkCuon WHERE MaSach = @MaSach", new { MaSach = maSach }, tran);
                await conn.ExecuteAsync("DELETE FROM SachDaDoc WHERE MaSach = @MaSach", new { MaSach = maSach }, tran);
                await conn.ExecuteAsync("DELETE FROM TuSach WHERE MaSach = @MaSach", new { MaSach = maSach }, tran);
                await conn.ExecuteAsync("DELETE FROM LichSuXem WHERE MaSach = @MaSach", new { MaSach = maSach }, tran);
                await conn.ExecuteAsync("DELETE FROM LichSuTai WHERE MaSach = @MaSach", new { MaSach = maSach }, tran);
                tran.Commit();
                return true;
            }
            catch (Exception ex)
            {
                tran.Rollback();
                Console.WriteLine("Lỗi khi xóa sách: " + ex.Message);
                return false;
            }
        }
        public async Task<List<CapDoViewModel>> GetAllCap()
        {
            using var conn = new SqlConnection(_connectionString);
            var sql = "SELECT MaCap, TenCap FROM Cap ORDER BY TenCap";
            var result = await conn.QueryAsync<CapDoViewModel>(sql);
            return result.AsList();
        }
    }
}
