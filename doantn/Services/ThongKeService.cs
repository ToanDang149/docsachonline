using Microsoft.Data.SqlClient;
using Dapper;
using System.Data;
using doantn.ViewModel;

namespace doantn.Services
{
    public class ThongKeService
    {
        private readonly string _connectionString;

        public ThongKeService(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("DefaultConnection");
        }

        public async Task<List<ThongKeTheoNgayViewModel>> GetLuotXemTheoKhoangDayDu(string maSach, DateTime from, DateTime to)
        {
            using var conn = new SqlConnection(_connectionString);
            var sql = @"
                SELECT 
                    CAST(ThoiGianXem AS DATE) AS Ngay, 
                    COUNT(*) AS SoLuot
                FROM LichSuXem
                WHERE MaSach = @MaSach AND ThoiGianXem BETWEEN @From AND @To
                GROUP BY CAST(ThoiGianXem AS DATE)
            ";
            var data = (await conn.QueryAsync<ThongKeTheoNgayViewModel>(sql, new { MaSach = maSach, From = from, To = to }))
                       .ToDictionary(x => x.Ngay.Date, x => x.SoLuot);
            var result = new List<ThongKeTheoNgayViewModel>();
            for (var date = from.Date; date <= to.Date; date = date.AddDays(1))
            {
                result.Add(new ThongKeTheoNgayViewModel
                {
                    Ngay = date,
                    SoLuot = data.ContainsKey(date) ? data[date] : 0
                });
            }
            return result;
        }
        public async Task<List<ThongKeTheoNgayViewModel>> GetLuotTaiTheoKhoangDayDu(string maSach, DateTime from, DateTime to)
        {
            using var conn = new SqlConnection(_connectionString);

            var sql = @"
                SELECT 
                    CAST(ThoiGianTai AS DATE) AS Ngay, 
                    COUNT(*) AS SoLuot
                FROM LichSuTai
                WHERE MaSach = @MaSach AND ThoiGianTai BETWEEN @From AND @To
                GROUP BY CAST(ThoiGianTai AS DATE)
            ";

            var data = (await conn.QueryAsync<ThongKeTheoNgayViewModel>(sql, new { MaSach = maSach, From = from, To = to }))
                       .ToDictionary(x => x.Ngay.Date, x => x.SoLuot);

            var result = new List<ThongKeTheoNgayViewModel>();
            for (var date = from.Date; date <= to.Date; date = date.AddDays(1))
            {
                result.Add(new ThongKeTheoNgayViewModel
                {
                    Ngay = date,
                    SoLuot = data.ContainsKey(date) ? data[date] : 0
                });
            }

            return result;
        }
        public async Task<int> TinhTongLuotXem(string maSach)
        {
            var sql = "SELECT COUNT(*) FROM LichSuXem WHERE MaSach = @maSach";
            using var conn = new SqlConnection(_connectionString);
            return await conn.ExecuteScalarAsync<int>(sql, new { maSach });
        }

        public async Task<int> TinhTongLuotTai(string maSach)
        {
            var sql = "SELECT COUNT(*) FROM LichSuTai WHERE MaSach = @maSach";
            using var conn = new SqlConnection(_connectionString);
            return await conn.ExecuteScalarAsync<int>(sql, new { maSach });
        }
        public async Task<(string Thang, int TongLuotXem)> TinhLuotXemCaoNhatThang(string maSach)
        {
            var sql = @"
        SELECT TOP 1 
            FORMAT(ThoiGianXem, 'yyyy-MM') AS Thang,
            COUNT(*) AS TongLuotXem
                FROM LichSuXem
                WHERE MaSach = @maSach
                GROUP BY FORMAT(ThoiGianXem, 'yyyy-MM')
                ORDER BY TongLuotXem DESC";
            using var conn = new SqlConnection(_connectionString);
            return await conn.QueryFirstOrDefaultAsync<(string Thang, int TongLuotXem)>(sql, new { maSach });
        }
        public async Task<(string Thang, int TongLuotTai)> TinhLuotTaiCaoNhatThang(string maSach)
        {
            var sql = @"
        SELECT TOP 1 
            FORMAT(ThoiGianTai, 'yyyy-MM') AS Thang,
            COUNT(*) AS TongLuotTai
                FROM LichSuTai
                WHERE MaSach = @maSach
                GROUP BY FORMAT(ThoiGianTai, 'yyyy-MM')
                ORDER BY TongLuotTai DESC";

            using var conn = new SqlConnection(_connectionString);
            return await conn.QueryFirstOrDefaultAsync<(string Thang, int TongLuotTai)>(sql, new { maSach });
        }
        public async Task<(double DiemTrungBinh, int TongDanhGia, Dictionary<int, int> StarCounts)> ThongKeDanhGia(string maSach)
        {
            using var conn = new SqlConnection(_connectionString);

            var sqlTong = @"SELECT AVG(CAST(SoSao AS FLOAT)) AS DiemTrungBinh, COUNT(*) AS TongDanhGia
                    FROM DanhGia WHERE MaSach = @maSach";

            var sqlTheoSao = @"SELECT SoSao, COUNT(*) AS SoLuong
                       FROM DanhGia WHERE MaSach = @maSach GROUP BY SoSao";

            var thongTin = await conn.QueryFirstOrDefaultAsync(sqlTong, new { maSach });
            var sao = await conn.QueryAsync(sqlTheoSao, new { maSach });

            var starCounts = sao.ToDictionary(x => (int)x.SoSao, x => (int)x.SoLuong);

            return (
                DiemTrungBinh: thongTin?.DiemTrungBinh ?? 0,
                TongDanhGia: thongTin?.TongDanhGia ?? 0,
                StarCounts: starCounts
            );
        }

    }
}
