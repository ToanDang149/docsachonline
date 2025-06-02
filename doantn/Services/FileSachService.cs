using doantn.Models;
using Microsoft.Data.SqlClient;
using Dapper;

namespace doantn.Services
{
    public class FileSachService
    {
        private readonly IConfiguration _config;
        private readonly string _connectionString;

        public FileSachService(IConfiguration config)
        {
            _config = config;
            _connectionString = _config.GetConnectionString("DefaultConnection");
        }

        public async Task<FileSach?> GetFile(string maSach, string dinhDang)
        {
            using var conn = new SqlConnection(_connectionString);
            var sql = @"SELECT TOP 1 * FROM FileSach WHERE MaSach = @maSach AND LoaiFile = @dinhDang";
            return await conn.QueryFirstOrDefaultAsync<FileSach>(sql, new { maSach, dinhDang });
        }
        public async Task<List<FileSach>> GetDanhSachFile(string maSach)
        {
            using var conn = new SqlConnection(_connectionString);
            var sql = "SELECT * FROM FileSach WHERE MaSach = @maSach";
            var result = await conn.QueryAsync<FileSach>(sql, new { maSach });
            return result.ToList();
        }
        public async Task<bool> GhiLichSuTaiNeuChuaCo(string maSach, string maKhachHang)
        {
            using var conn = new SqlConnection(_connectionString);
            var daTai = await conn.ExecuteScalarAsync<bool>(
                @"SELECT COUNT(1) FROM LichSuTai WHERE MaKhachHang = @MaKH AND MaSach = @MaSach",
                new { MaKH = maKhachHang, MaSach = maSach });
            if (!daTai)
            {
                await conn.ExecuteAsync(
                    @"INSERT INTO LichSuTai (MaLichSu, MaSach, ThoiGianTai, MaKhachHang)
                  VALUES (NEWID(), @MaSach, GETDATE(), @MaKH)",
                    new { MaSach = maSach, MaKH = maKhachHang });

                await conn.ExecuteAsync(
                    @"UPDATE Sach SET LuotTai = ISNULL(LuotTai, 0) + 1 WHERE MaSach = @MaSach",
                    new { MaSach = maSach });

                return true;
            }
            return false;
        }
    }
}
