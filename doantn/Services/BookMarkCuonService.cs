using Dapper;
using System.Data.SqlClient;
using System.Data;
using doantn.Models;
using Microsoft.Data.SqlClient;
using doantn.ViewModel;

namespace doantn.Services
{
    public class BookMarkCuonService
    {
        private readonly string _connectionString;
        public BookMarkCuonService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<bool> AddBookmark(BookMarkCuon model)
        {
            using var conn = new SqlConnection(_connectionString);
            var sql = @"IF NOT EXISTS (SELECT 1 FROM BookMarkCuon WHERE MaKhachHang = @MaKhachHang AND MaSach = @MaSach AND SoDong = @SoDong)
                    INSERT INTO BookMarkCuon (MaKhachHang, MaSach, TenChuong, SoDong)
                    VALUES (@MaKhachHang, @MaSach, @TenChuong, @SoDong)
                    ";
            var result = await conn.ExecuteAsync(sql, model);
            return result > 0;
        }

        public async Task<List<BookMarkCuon>> GetBookmarks(string maKhachHang, string maSach)
        {
            using var conn = new SqlConnection(_connectionString);
            var sql = "SELECT * FROM BookMarkCuon WHERE MaKhachHang = @MaKhachHang AND MaSach = @MaSach ORDER BY SoDong";
            var result = await conn.QueryAsync<BookMarkCuon>(sql, new { MaKhachHang = maKhachHang, MaSach = maSach });
            return result.ToList();
        }
        public async Task<bool> XoaBookmark(string maKhachHang, string maSach, int soDong)
        {
            using var conn = new SqlConnection(_connectionString);
            var sql = "DELETE FROM BookMarkCuon WHERE MaKhachHang = @MaKhachHang AND MaSach = @MaSach AND SoDong = @SoDong";
            var result = await conn.ExecuteAsync(sql, new { MaKhachHang = maKhachHang, MaSach = maSach, SoDong = soDong });
            return result > 0;
        }
        public async Task<bool> BookmarkDaTonTai(string maKhachHang, string maSach, int soDong)
        {
            using var conn = new SqlConnection(_connectionString);
            var sql = "SELECT 1 FROM BookMarkCuon WHERE MaKhachHang = @MaKhachHang AND MaSach = @MaSach AND SoDong = @SoDong";
            var exists = await conn.ExecuteScalarAsync<int?>(sql, new { MaKhachHang = maKhachHang, MaSach = maSach, SoDong = soDong });
            return exists.HasValue;
        }
    }
}
