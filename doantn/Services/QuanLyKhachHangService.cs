using Dapper;
using System.Data.SqlClient;
using System.Data;
using doantn.Models;
using Microsoft.Data.SqlClient;
using doantn.ViewModel;

namespace doantn.Services
{
    public class QuanLyKhachHangService
    {
        private readonly IConfiguration _config;
        private readonly string _connectionString;

        public QuanLyKhachHangService(IConfiguration config)
        {
            _config = config;
            _connectionString = _config.GetConnectionString("DefaultConnection");
        }

        public async Task<List<KhachHangQuanLyViewModel>> GetAllKhachHang()
        {
            using var conn = new SqlConnection(_connectionString);
            var sql = @"
                SELECT kh.MaKhachHang, kh.TaiKhoan, kh.HoTen, kh.Email, kh.SoDT, kh.GioiTinh, c.TenCap
                FROM KhachHang kh
                JOIN Cap c ON kh.MaCap = c.MaCap
                ORDER BY kh.HoTen";

            var result = await conn.QueryAsync<KhachHangQuanLyViewModel>(sql);
            return result.ToList();
        }
        public async Task<(bool isSuccess, string message)> ThemKhachHang(KhachHangThemViewModel model)
        {
            using var conn = new SqlConnection(_connectionString);

            var checkQuery = "SELECT COUNT(1) FROM KhachHang WHERE TaiKhoan = @TaiKhoan";
            var exists = await conn.ExecuteScalarAsync<int>(checkQuery, new { model.TaiKhoan });

            if (exists > 0)
                return (false, "Tài khoản đã tồn tại.");

            var insertQuery = @"
                INSERT INTO KhachHang (MaKhachHang, TaiKhoan, MatKhau, HoTen, Email, SoDT, GioiTinh, MaCap)
                VALUES (@MaKhachHang, @TaiKhoan, @MatKhau, @HoTen, @Email, @SoDT, @GioiTinh, @MaCap)";

            var result = await conn.ExecuteAsync(insertQuery, new
            {
                MaKhachHang = Guid.NewGuid().ToString(),
                model.TaiKhoan,
                model.MatKhau,
                model.HoTen,
                model.Email,
                model.SoDT,
                model.GioiTinh,
                model.MaCap
            });

            return result > 0
                ? (true, "Thêm người dùng thành công.")
                : (false, "Thêm thất bại.");
        }

        public async Task<List<CapDoViewModel>> GetAllCap()
        {
            using var conn = new SqlConnection(_connectionString);
            var sql = "SELECT MaCap, TenCap FROM Cap ORDER BY TenCap";
            var result = await conn.QueryAsync<CapDoViewModel>(sql);
            return result.ToList();
        }
        public async Task<KhachHang?> GetThongTinKhachHang(string maKhachHang)
        {
            using var conn = new SqlConnection(_connectionString);
            var sql = "SELECT * FROM KhachHang WHERE MaKhachHang = @MaKhachHang";
            return await conn.QueryFirstOrDefaultAsync<KhachHang>(sql, new { MaKhachHang = maKhachHang });
        }

        public async Task<(bool isSuccess, string message)> CapNhatKhachHang(KhachHang model)
        {
            using var conn = new SqlConnection(_connectionString);
            var sql = @"
                UPDATE KhachHang
                SET TaiKhoan = @TaiKhoan, HoTen = @HoTen, Email = @Email, SoDT = @SoDT, GioiTinh = @GioiTinh, MaCap = @MaCap
                WHERE MaKhachHang = @MaKhachHang";

            var rows = await conn.ExecuteAsync(sql, model);
            return rows > 0
                ? (true, "Cập nhật thành công.")
                : (false, "Không thể cập nhật.");
        }
        public async Task<bool> KiemTraTaiKhoanTrung(string taiKhoan, string? maKhachHang = null)
        {
            using var conn = new SqlConnection(_connectionString);
            var sql = @"
                SELECT COUNT(*) 
                FROM KhachHang 
                WHERE TaiKhoan = @TaiKhoan
                AND (@MaKhachHang IS NULL OR MaKhachHang != @MaKhachHang)";

            var count = await conn.ExecuteScalarAsync<int>(sql, new { TaiKhoan = taiKhoan, MaKhachHang = maKhachHang });
            return count > 0;
        }
        public async Task<bool> XoaKhachHang(string maKhachHang)
        {
            using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();
            using var tran = conn.BeginTransaction();
            try
            {
                await conn.ExecuteAsync("DELETE FROM DanhGia WHERE MaKhachHang = @MaKhachHang", new { MaKhachHang = maKhachHang }, tran);
                await conn.ExecuteAsync("DELETE FROM BookMark WHERE MaKhachHang = @MaKhachHang", new { MaKhachHang = maKhachHang }, tran);
                await conn.ExecuteAsync("DELETE FROM SachDaDoc WHERE MaKhachHang = @MaKhachHang", new { MaKhachHang = maKhachHang }, tran);
                await conn.ExecuteAsync("DELETE FROM TuSach WHERE MaKhachHang = @MaKhachHang", new { MaKhachHang = maKhachHang }, tran);
                await conn.ExecuteAsync("DELETE FROM BookMarkCuon WHERE MaKhachHang = @MaKhachHang", new { MaKhachHang = maKhachHang }, tran);
                await conn.ExecuteAsync("DELETE FROM LichSuMua WHERE MaKhachHang = @MaKhachHang", new { MaKhachHang = maKhachHang }, tran);
                var result = await conn.ExecuteAsync("DELETE FROM KhachHang WHERE MaKhachHang = @MaKhachHang", new { MaKhachHang = maKhachHang }, tran);
                tran.Commit();
                return result > 0;
            }
            catch
            {
                tran.Rollback();
                return false;
            }
        }

    }
}
