using Dapper;
using System.Data.SqlClient;
using System.Data;
using doantn.Models;
using Microsoft.Data.SqlClient;
using doantn.ViewModel;

namespace doantn.Services
{
    public class KhachHangService
    {
        private readonly IConfiguration _config;
        private readonly string _connectionString;

        public KhachHangService(IConfiguration config)
        {
            _config = config;
            _connectionString = _config.GetConnectionString("DefaultConnection");
        }
        public async Task<bool> KiemTraDaDocSach(string maKhachHang, string maSach)
        {
            using var conn = new SqlConnection(_connectionString);
            var query = @"SELECT COUNT(1) FROM SachDaDoc WHERE MaKhachHang = @MaKhachHang AND MaSach = @MaSach";
            int count = await conn.ExecuteScalarAsync<int>(query, new { MaKhachHang = maKhachHang, MaSach = maSach });
            return count > 0;
        }
        public async Task LuuSachDaDoc(string maKhachHang, string maSach)
        {
            using var conn = new SqlConnection(_connectionString);
            var query = @"INSERT INTO SachDaDoc (MaKhachHang, MaSach, NgayDoc) VALUES (@MaKhachHang, @MaSach, GETDATE())";
            await conn.ExecuteAsync(query, new { MaKhachHang = maKhachHang, MaSach = maSach });
        }
        public async Task<List<Sach>> GetSachDaDocAsync(string maKhachHang)
        {
            using var conn = new SqlConnection(_connectionString);
            var query = @"
                SELECT s.MaSach, s.TenSach, s.TenTacGia, s.GioiThieu, l.TenLoai, s.LuotXem, s.LuotTai, s.Anh
                FROM SachDaDoc sd
                JOIN Sach s ON sd.MaSach = s.MaSach
                JOIN Loai l ON s.MaLoai = l.MaLoai
                WHERE sd.MaKhachHang = @MaKhachHang
                ORDER BY sd.NgayDoc DESC";
            var result = await conn.QueryAsync<Sach>(query, new { MaKhachHang = maKhachHang });
            return result.ToList();
        }

    }
}
