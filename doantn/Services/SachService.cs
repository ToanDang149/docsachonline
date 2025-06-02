using Dapper;
using System.Data.SqlClient;
using System.Data;
using doantn.Models;
using Microsoft.Data.SqlClient;
using doantn.ViewModel;
public class SachService
{
    private readonly IConfiguration _config;
    private readonly string _connectionString;

    public SachService(IConfiguration config)
    {
        _config = config;
        _connectionString = _config.GetConnectionString("DefaultConnection");
    }

    public List<Sach> GetSachTaiNhieu()
    {
        using var conn = new SqlConnection(_connectionString);

        string sql = @"
            SELECT s.MaSach, s.TenSach, s.TenTacGia, s.GioiThieu, s.LuotTai, s.LuotXem, l.TenLoai,
                   s.Anh, s.Maloai
            FROM Sach s
            JOIN Loai l ON s.MaLoai = l.MaLoai
            ORDER BY s.LuotTai DESC";

        return conn.Query<Sach>(sql).ToList();
    }
    public List<Sach> GetSachHot()
    {
        using var conn = new SqlConnection(_connectionString);
        string sql = @"
            SELECT s.MaSach, s.TenSach, s.TenTacGia, s.GioiThieu, s.LuotTai, s.LuotXem, l.TenLoai,
                   s.Anh, s.Maloai
            FROM Sach s
            JOIN Loai l ON s.MaLoai = l.MaLoai
            ORDER BY s.LuotXem DESC";
        return conn.Query<Sach>(sql).ToList();
    }
    public List<Sach> GetSachMoi()
    {
        using var conn = new SqlConnection(_connectionString);
        string sql = @"
            SELECT s.MaSach, s.TenSach, s.TenTacGia, s.GioiThieu, s.LuotTai, s.LuotXem, l.TenLoai,
                   s.Anh, s.Maloai
            FROM Sach s
            JOIN Loai l ON s.MaLoai = l.MaLoai
            ORDER BY s.ThoiGianCatNhap DESC";
        return conn.Query<Sach>(sql).ToList();
    }
    public async Task<IEnumerable<Sach>> TimKiemSachAsync(string keyword)
    {
        using var connection = new SqlConnection(_connectionString);
        var sql = @"
        SELECT s.MaSach, s.TenSach, s.TenTacGia, s.GioiThieu,s.LuotTai, s.LuotXem, l.TenLoai, s.Anh, s.Maloai
        FROM Sach s
        LEFT JOIN Loai l ON s.maloai = l.maloai
        WHERE (@keyword IS NULL OR s.TenSach LIKE '%' + @keyword + '%' OR s.TenTacGia LIKE '%' + @keyword + '%')
        ORDER BY s.TenSach";

        return await connection.QueryAsync<Sach>(sql, new { keyword });
    }
    public async Task<Sach?> GetChiTiet(string maSach)
    {
        using var conn = new SqlConnection(_connectionString);
        var query = @"
            SELECT 
                s.MaSach,
                s.TenSach,
                s.TenTacGia,
                s.GioiThieu,
                s.ThoiGianCatNhap,
                l.TenLoai,
                s.LuotXem,
                s.LuotTai,
                s.Anh,
                s.Nguon,
                s.DanhGia,
                s.MaLoai
            FROM Sach s
            JOIN Loai l ON s.MaLoai = l.MaLoai
            WHERE s.MaSach = @MaSach
        ";

        return await conn.QueryFirstOrDefaultAsync<Sach>(query, new { MaSach = maSach });
    }
    public async Task<List<Sach>> GetCungTheLoai(string maLoai, string maSach)
    {
        using var conn = new SqlConnection(_connectionString);
        var query = @"
        SELECT TOP 6 s.*, l.TenLoai 
        FROM Sach s
        JOIN Loai l ON s.MaLoai = l.MaLoai
        WHERE s.MaLoai = @MaLoai AND s.MaSach != @MaSach";
        return (await conn.QueryAsync<Sach>(query, new { MaLoai = maLoai, MaSach = maSach })).ToList();
    }
    public async Task<SachChiTietViewModel> GetChiTietSach(string maSach)
    {
        using var conn = new SqlConnection(_connectionString);

        var sql = @"
        SELECT s.MaSach, s.TenSach, s.TenTacGia, s.GioiThieu, s.LuotXem, s.LuotTai, s.Anh,
               c.TenChuong, t.SoTrang, t.NoiDung
        FROM Sach s
        JOIN Chuong c ON s.MaSach = c.MaSach
        JOIN Trang t ON c.MaChuong = t.MaChuong
        WHERE s.MaSach = @maSach
        ORDER BY c.TenChuong, t.SoTrang";

        var lookup = new SachChiTietViewModel { DanhSachTrang = new List<TrangNoiDungVM>() };

        var result = await conn.QueryAsync(sql, new { maSach });

        foreach (var row in result)
        {
            if (lookup.MaSach == null)
            {
                lookup.MaSach = row.MaSach;
                lookup.TenSach = row.TenSach;
                lookup.TenTacGia = row.TenTacGia;
            }

            lookup.DanhSachTrang.Add(new TrangNoiDungVM
            {
                TenChuong = row.TenChuong,
                SoTrang = row.SoTrang,
                NoiDung = row.NoiDung
            });
        }

        return lookup;
    }
    public async Task<SachChiTietViewModel> GetChiTietSachAsync(string maSach)
    {
        using var conn = new SqlConnection(_connectionString);

        var sql = @"
        SELECT 
            s.MaSach, 
            s.TenSach,
            s.Anh,
            s.TenTacGia,
            c.MaChuong,
            c.TenChuong,
            ISNULL(t.NoiDung, '') AS NoiDung
        FROM Sach s
        JOIN Chuong c ON s.MaSach = c.MaSach
        LEFT JOIN (
            SELECT 
                MaChuong, 
                STRING_AGG(NoiDung, ' ') WITHIN GROUP (ORDER BY MaTrang ASC) AS NoiDung
            FROM Trang
            GROUP BY MaChuong
        ) t ON c.MaChuong = t.MaChuong
        WHERE s.MaSach = @maSach
        ORDER BY c.ThuTuChuong ASC;
    ";

        var lookup = new SachChiTietViewModel
        {
            ChuongNoiDung = new List<ChuongNoiDungVM>()
        };

        var result = await conn.QueryAsync(sql, new { maSach });

        foreach (var row in result)
        {
            if (lookup.MaSach == null)
            {
                lookup.MaSach = row.MaSach;
                lookup.TenSach = row.TenSach;
                lookup.Anh = row.Anh;
                lookup.TenTacGia = row.TenTacGia;
            }

            lookup.ChuongNoiDung.Add(new ChuongNoiDungVM
            {
                TenChuong = row.TenChuong,
                NoiDung = row.NoiDung
            });
        }

        return lookup;
    }
    public List<Sach> GetSachByLoai(string maLoai)
    {
        using var connection = new SqlConnection(_connectionString);
        var sql = @"SELECT MaSach, TenSach, TenTacGia, GioiThieu, TenLoai, LuotXem, LuotTai, Anh
                    FROM Sach
                    INNER JOIN Loai ON Sach.MaLoai = Loai.MaLoai
                    WHERE Sach.MaLoai = @MaLoai";

        var list = connection.Query<Sach>(sql, new { MaLoai = maLoai }).ToList();
        return list;
    }
    //public async Task TangLuotXem(string maSach)
    //{
    //    using var conn = new SqlConnection(_connectionString);
    //    var query = @"UPDATE Sach SET LuotXem = ISNULL(LuotXem,0) + 1 WHERE MaSach = @MaSach";

    //    await conn.ExecuteAsync(query, new { MaSach = maSach });
    //}
    public async Task TangLuotXem(string maSach, string? maKhachHang)
    {
        using var conn = new SqlConnection(_connectionString);
        var query = @"UPDATE Sach SET LuotXem = ISNULL(LuotXem,0) + 1 WHERE MaSach = @MaSach";
        await conn.ExecuteAsync(query, new { MaSach = maSach });
        if (!string.IsNullOrEmpty(maKhachHang))
        {
            var sql = @"INSERT INTO LichSuXem (MaLichSu, MaSach, MaKhachHang, ThoiGianXem)
                    VALUES (NEWID(), @MaSach, @MaKhachHang, GETDATE())";

            await conn.ExecuteAsync(sql, new { MaSach = maSach, MaKhachHang = maKhachHang });
        }
    }
    public async Task<SachChiTietViewModel> GetChiTietSachCuon(string maSach)
    {
        using var conn = new SqlConnection(_connectionString);

        var sql = @"
        SELECT 
            s.MaSach, 
            s.TenSach,
            s.Anh,
            s.TenTacGia,
            c.MaChuong,
            c.TenChuong,
            ISNULL(t.NoiDung, '') AS NoiDung
        FROM Sach s
        JOIN Chuong c ON s.MaSach = c.MaSach
        LEFT JOIN (
            SELECT 
                MaChuong, 
                STRING_AGG(NoiDung, CHAR(10)) WITHIN GROUP (ORDER BY MaTrang ASC) AS NoiDung
            FROM Trang
            GROUP BY MaChuong
        ) t ON c.MaChuong = t.MaChuong
        WHERE s.MaSach = @maSach
        ORDER BY c.ThuTuChuong ASC;
        ";

        var lookup = new SachChiTietViewModel
        {
            ChuongNoiDung = new List<ChuongNoiDungVM>()
        };

        var result = await conn.QueryAsync(sql, new { maSach });

        foreach (var row in result)
        {
            if (lookup.MaSach == null)
            {
                lookup.MaSach = row.MaSach;
                lookup.TenSach = row.TenSach;
                lookup.Anh = row.Anh;
                lookup.TenTacGia = row.TenTacGia;
            }

            lookup.ChuongNoiDung.Add(new ChuongNoiDungVM
            {
                TenChuong = row.TenChuong,
                NoiDung = row.NoiDung
            });
        }

        return lookup;
    }
}