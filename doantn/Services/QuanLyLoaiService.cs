using Dapper;
using System.Data.SqlClient;
using System.Data;
using doantn.Models;
using Microsoft.Data.SqlClient;
using doantn.ViewModel;

namespace doantn.Services
{
    public class QuanLyLoaiService
    {
        private readonly IConfiguration _config;
        private readonly string _connectionString;

        public QuanLyLoaiService(IConfiguration config)
        {
            _config = config;
            _connectionString = _config.GetConnectionString("DefaultConnection");
        }

        public async Task<List<LoaiQuanLyViewModel>> GetAllLoaiSach()
        {
            using var conn = new SqlConnection(_connectionString);
            var sql = @"SELECT maloai, tenloai FROM Loai ORDER BY tenloai";
            var result = await conn.QueryAsync<LoaiQuanLyViewModel>(sql);
            return result.ToList();
        }

        public async Task ThemLoaiSach(string tenLoai)
        {
            using var conn = new SqlConnection(_connectionString);
            var sql = @"INSERT INTO Loai (maloai, tenloai) VALUES (NEWID(), @tenloai)";
            await conn.ExecuteAsync(sql, new { tenloai = tenLoai });
        }

        public async Task XoaLoaiSach(string maLoai)
        {
            using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();
            using var tran = conn.BeginTransaction();

            try
            {
                var sqlDeleteSach = "DELETE FROM Sach WHERE MaLoai = @maloai";
                await conn.ExecuteAsync(sqlDeleteSach, new { maloai = maLoai }, tran);

                var sqlDeleteLoai = "DELETE FROM Loai WHERE MaLoai = @maloai";
                await conn.ExecuteAsync(sqlDeleteLoai, new { maloai = maLoai }, tran);

                await tran.CommitAsync();
            }
            catch
            {
                await tran.RollbackAsync();
                throw;
            }
        }

        public async Task CapNhatLoai(string maLoai, string tenLoai)
        {
            using var conn = new SqlConnection(_connectionString);
            var sql = @"UPDATE Loai SET tenloai = @tenloai WHERE maloai = @maloai";
            await conn.ExecuteAsync(sql, new { maloai = maLoai, tenloai = tenLoai });
        }
    }
}
