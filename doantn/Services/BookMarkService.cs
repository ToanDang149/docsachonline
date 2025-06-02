using doantn.ViewModel;
using Microsoft.Data.SqlClient;
using System.Data;
using Dapper;
namespace doantn.Services
{
    public interface IBookMarkService {
        Task AddBookmark(string MaKhachHang, string MaSach, string TenChuong, int SoTrang);
        Task<List<BookMarkIndex>> GetBookmarks(string MaKhachHang, string MaSach);
        Task DeleteBookmark(string MaKhachHang, string MaSach, int SoTrang);
    }
    public class BookMarkService : IBookMarkService
    {
        private readonly IConfiguration _config;
        private readonly string _connectionString;

        public BookMarkService(IConfiguration config)
        {
            _config = config;
            _connectionString = _config.GetConnectionString("DefaultConnection");
        }

        public async Task AddBookmark(string MaKhachHang, string MaSach, string TenChuong, int SoTrang)
        {
            using var conn = new SqlConnection(_connectionString);
            var sql = @"
            INSERT INTO BookMark (MaKhachHang, MaSach, TenChuong, SoTrang)
            VALUES (@MaKhachHang, @MaSach, @TenChuong, @SoTrang)";
            await conn.ExecuteAsync(sql, new { MaKhachHang, MaSach, TenChuong, SoTrang });
        }

        public async Task<List<BookMarkIndex>> GetBookmarks(string MaKhachHang, string MaSach)
        {
            using var conn = new SqlConnection(_connectionString);
            var sql = @"
            SELECT TenChuong, SoTrang
            FROM BookMark
            WHERE MaKhachHang = @MaKhachHang AND MaSach = @MaSach";

            return (await conn.QueryAsync<BookMarkIndex>(sql, new { MaKhachHang, MaSach })).ToList();
        }
        public async Task DeleteBookmark(string MaKhachHang, string MaSach, int SoTrang)
        {
            using var conn = new SqlConnection(_connectionString);
            var sql = @"DELETE FROM BookMark WHERE MaKhachHang = @MaKhachHang AND MaSach = @MaSach AND SoTrang = @SoTrang";
            await conn.ExecuteAsync(sql, new { MaKhachHang, MaSach, SoTrang });
        }
    }
}
