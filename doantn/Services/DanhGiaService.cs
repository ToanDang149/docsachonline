using Dapper;
using System.Data.SqlClient;
using System.Data;
using doantn.Models;
using Microsoft.Data.SqlClient;
using doantn.ViewModel;

namespace doantn.Services
{
    public class DanhGiaService
    {
        private readonly IConfiguration _config;
        private readonly string _connectionString;

        public DanhGiaService(IConfiguration config)
        {
            _config = config;
            _connectionString = _config.GetConnectionString("DefaultConnection");
        }

        public async Task AddDanhGia(DanhGia danhGia)
        {
            using var conn = new SqlConnection(_connectionString);
            var sql = @"
                INSERT INTO DanhGia (MaSach, MaKhachHang, SoSao, NoiDung, ThoiGianDanhGia)
                VALUES (@MaSach, @MaKhachHang, @SoSao, @NoiDung, GETDATE())
                ";
            await conn.ExecuteAsync(sql, danhGia);
        }

        public async Task<IEnumerable<DanhGia>> GetDanhGiaBySach(string maSach)
        {
            using var conn = new SqlConnection(_connectionString);
            var sql = @"
                SELECT dg.MaDanhGia, dg.MaSach, dg.MaKhachHang, dg.SoSao, dg.NoiDung, dg.ThoiGianDanhGia,
                        kh.HoTen, kh.MaCap As Cap
                FROM DanhGia dg
                JOIN KhachHang kh ON dg.MaKhachHang = kh.MaKhachHang
                WHERE dg.MaSach = @MaSach
                ORDER BY dg.ThoiGianDanhGia DESC
                ";
            return await conn.QueryAsync<DanhGia>(sql, new { MaSach = maSach });
        }

        public async Task<DanhGia> GetDanhGia(string maSach, string maKhachHang)
        {
            using var conn = new SqlConnection(_connectionString);
            var sql = "SELECT * FROM DanhGia WHERE MaSach = @MaSach AND MaKhachHang = @MaKhachHang";
            return await conn.QueryFirstOrDefaultAsync<DanhGia>(sql, new { MaSach = maSach, MaKhachHang = maKhachHang });
        }

        public async Task UpdateDanhGia(DanhGia danhGia)
        {
            using var conn = new SqlConnection(_connectionString);
            var sql = @"UPDATE DanhGia 
                SET SoSao = @SoSao, NoiDung = @NoiDung, ThoiGianDanhGia = @ThoiGianDanhGia
                WHERE MaSach = @MaSach AND MaKhachHang = @MaKhachHang";
            await conn.ExecuteAsync(sql, danhGia);
        }

    }
}
