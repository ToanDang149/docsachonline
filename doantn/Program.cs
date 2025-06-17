using doantn.Services;

namespace doantn
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddScoped<SachService>();
            builder.Services.AddScoped<QuanLyService>();
            builder.Services.AddScoped<MuaService>();
            builder.Services.AddScoped<FileSachService>();
            builder.Services.AddScoped<QuanLyLoaiService>();
            builder.Services.AddScoped<BookMarkCuonService>();
            builder.Services.AddScoped<QuanLyKhachHangService>();
            builder.Services.AddScoped<KhachHangService>();
            builder.Services.AddScoped<TuSachService>();
            builder.Services.AddScoped<DanhGiaService>();
            builder.Services.AddScoped<ThongKeService>();
            builder.Services.AddScoped<ILoaiService,LoaiService>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IBookMarkService, BookMarkService>();
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddSwaggerGen();
            builder.Services.AddSession();
            var app = builder.Build();
            app.UseSession();
            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
           
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
            app.UseSwagger();
            app.UseSwaggerUI();
            app.Run();
        }
    }
}
