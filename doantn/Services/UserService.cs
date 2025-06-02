using Dapper;
using System.Data.SqlClient;
using System.Data;
using doantn.Models;
using doantn.ViewModel;
using Microsoft.Data.SqlClient;
namespace doantn.Services
{
    public interface IUserService
    {
        Task<(bool isSuccess, string message)> DangKy(DangKyViewModel model);
        Task<(bool isSuccess, string message)> DangNhap(string taiKhoan, string matKhau);
        Task<KhachHang?> GetByLogin(string taiKhoan, string matKhau);
        Task<bool> CheckTaiKhoanTonTai(string taiKhoan);
        Task<bool> CheckEmailTonTai(string email);
    }
    public class UserService : IUserService
    {
        private readonly IConfiguration _config;
        private readonly string _connectionString;

        public UserService(IConfiguration config)
        {
            _config = config;
            _connectionString = _config.GetConnectionString("DefaultConnection");
        }

        public async Task<(bool isSuccess, string message)> DangKy(DangKyViewModel model)
        {
            using var conn = new SqlConnection(_connectionString);

            var checkQuery = "SELECT COUNT(1) FROM KhachHang WHERE TaiKhoan = @TaiKhoan";
            var exists = await conn.ExecuteScalarAsync<int>(checkQuery, new { model.TaiKhoan });

            if (exists > 0)
                return (false, "Tài khoản đã tồn tại.");

            var insertQuery = @"
            INSERT INTO KhachHang (MaKhachHang, TaiKhoan, MatKhau, HoTen, Email, SoDT, GioiTinh, MaCap)
            VALUES (@MaKhachHang, @TaiKhoan, @MatKhau, @HoTen, @Email, @SoDT, @GioiTinh, 'c1')";

            var result = await conn.ExecuteAsync(insertQuery, new
            {
                MaKhachHang = Guid.NewGuid().ToString(),
                model.TaiKhoan,
                model.MatKhau,
                model.HoTen,
                model.Email,
                model.SoDT,
                model.GioiTinh
            });

            return result > 0
                ? (true, "Đăng ký thành công.")
                : (false, "Đăng ký thất bại.");
        }
        public async Task<(bool isSuccess, string message)> DangNhap(string taiKhoan, string matKhau)
        {
            using var conn = new SqlConnection(_connectionString);

            var query = @"SELECT COUNT(1) FROM KhachHang 
                  WHERE TaiKhoan = @TaiKhoan AND MatKhau = @MatKhau";

            var isValid = await conn.ExecuteScalarAsync<int>(query, new { TaiKhoan = taiKhoan, MatKhau = matKhau });

            if (isValid == 1)
                return (true, "Đăng nhập thành công.");

            return (false, "Sai tài khoản hoặc mật khẩu.");
        }
        public async Task<KhachHang?> GetByLogin(string taiKhoan, string matKhau)
        {
            using var conn = new SqlConnection(_connectionString);

            var query = @"SELECT * FROM KhachHang 
                  WHERE TaiKhoan = @TaiKhoan AND MatKhau = @MatKhau";

            return await conn.QueryFirstOrDefaultAsync<KhachHang>(query, new { TaiKhoan = taiKhoan, MatKhau = matKhau });
        }
        public async Task<bool> CheckTaiKhoanTonTai(string taiKhoan)
        {
            using var conn = new SqlConnection(_connectionString);
            var sql = "SELECT COUNT(1) FROM KhachHang WHERE TaiKhoan = @TaiKhoan";
            var count = await conn.ExecuteScalarAsync<int>(sql, new { TaiKhoan = taiKhoan });
            return count > 0;
        }

        public async Task<bool> CheckEmailTonTai(string email)
        {
            using var conn = new SqlConnection(_connectionString);
            var sql = "SELECT COUNT(1) FROM KhachHang WHERE Email = @Email";
            var count = await conn.ExecuteScalarAsync<int>(sql, new { Email = email });
            return count > 0;
        }

    }
}
