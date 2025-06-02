using Dapper;
using System.Data.SqlClient;
using System.Data;
using doantn.Models;
using Microsoft.Data.SqlClient;
using doantn.ViewModel;


namespace doantn.Services
{
    public class TuSachService
    {
        private readonly IConfiguration _config;
        private readonly string _connectionString;
        public TuSachService(IConfiguration config)
        {
            _config = config;
            _connectionString = _config.GetConnectionString("DefaultConnection");
        }
        public async Task<bool> KiemTraDaLuu(string maKhachHang, string maSach)
        {
            using var conn = new SqlConnection(_connectionString);
            var sql = "SELECT COUNT(1) FROM TuSach WHERE MaKhachHang = @MaKhachHang AND MaSach = @MaSach";
            var count = await conn.ExecuteScalarAsync<int>(sql, new { MaKhachHang = maKhachHang, MaSach = maSach });
            return count > 0;
        }

        public async Task LuuVaoTuSach(string maKhachHang, string maSach)
        {
            using var conn = new SqlConnection(_connectionString);
            var sql = "INSERT INTO TuSach (MaKhachHang, MaSach) VALUES (@MaKhachHang, @MaSach)";
            await conn.ExecuteAsync(sql, new { MaKhachHang = maKhachHang, MaSach = maSach });
        }
        public async Task XoaKhoiTuSach(string maKhachHang, string maSach)
        {
            using var conn = new SqlConnection(_connectionString);
            var sql = "DELETE FROM TuSach WHERE MaKhachHang = @MaKhachHang AND MaSach = @MaSach";
            await conn.ExecuteAsync(sql, new { MaKhachHang = maKhachHang, MaSach = maSach });
        }
        public async Task<List<Sach>> LayDanhSachTuSachAsync(string maKhachHang)
        {
            using var conn = new SqlConnection(_connectionString);
            var sql = @"
                SELECT s.MaSach, s.TenSach, s.TenTacGia, s.GioiThieu, l.TenLoai, s.LuotXem, s.LuotTai, s.Anh
                FROM TuSach ts
                JOIN Sach s ON ts.MaSach = s.MaSach
                JOIN Loai l ON s.MaLoai = l.MaLoai
                WHERE ts.MaKhachHang = @MaKhachHang
                ORDER BY ts.ThoiGianThem DESC";

            var result = await conn.QueryAsync<Sach>(sql, new { MaKhachHang = maKhachHang });
            return result.ToList();
        }
    }
}
