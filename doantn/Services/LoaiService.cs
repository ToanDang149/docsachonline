using Dapper;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using doantn.Models;
using Microsoft.Data.SqlClient;

public interface ILoaiService
{
    Task<List<Loai>> GetLoai();
    Task<Loai> GetTenLoaiById(string maLoai);
}
public class LoaiService : ILoaiService
{
    private readonly IConfiguration _config;
    private readonly string _connectionString;

    public LoaiService(IConfiguration config)
    {
        _config = config;
        _connectionString = _config.GetConnectionString("DefaultConnection");
    }

    public async Task<List<Loai>> GetLoai()
    {
        using var conn = new SqlConnection(_connectionString);
        var sql = "SELECT maloai, tenloai FROM Loai";
        var result = await conn.QueryAsync<Loai>(sql);
        return result.AsList();
    }
    public async Task<Loai> GetTenLoaiById(string maLoai)
    {
        using var conn = new SqlConnection(_connectionString);
        var sql = "SELECT MaLoai, TenLoai FROM Loai WHERE MaLoai = @MaLoai";
        var result = await conn.QueryFirstOrDefaultAsync<Loai>(sql, new { MaLoai = maLoai });
        return result;
    }
}