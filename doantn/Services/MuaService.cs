using Microsoft.Data.SqlClient;
using Dapper;
using System.Data.SqlClient;
using System.Data;
using doantn.Models;
using Microsoft.Data.SqlClient;
using doantn.ViewModel;

namespace doantn.Services
{
    public class MuaService
    {
        private readonly string _connectionString;
        public MuaService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }
        public async Task<bool> CapNhatCapNguoiDung(string maKhachHang, string maCap)
        {
            using var conn = new SqlConnection(_connectionString);
            var sql = "UPDATE KhachHang SET MaCap = @MaCap WHERE MaKhachHang = @MaKhachHang";
            var result = await conn.ExecuteAsync(sql, new { MaKhachHang = maKhachHang, MaCap = maCap });
            return result > 0;
        }
        public async Task<bool> GhiLichSuMua(string maKhachHang, string maCap, float gia)
        {
            using var conn = new SqlConnection(_connectionString);
            var sql = @"
            INSERT INTO LichSuMua (MaKhachHang, MaCap, Gia, NgayMua)
            VALUES (@MaKhachHang, @MaCap, @Gia, GETDATE())";
            var result = await conn.ExecuteAsync(sql, new { MaKhachHang = maKhachHang, MaCap = maCap, Gia = gia });
            return result > 0;
        }
        public async Task<CapViewModel?> GetCapByMaCap(string maCap)
        {
            using var conn = new SqlConnection(_connectionString);
            var sql = "SELECT MaCap, TenCap, Gia FROM Cap WHERE MaCap = @MaCap";
            return await conn.QueryFirstOrDefaultAsync<CapViewModel>(sql, new { MaCap = maCap });
        }
    }
}
