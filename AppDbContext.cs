using Microsoft.EntityFrameworkCore;
using MyWebApp.Users;
using static MyWebApp.Users.UserEndPoint;

namespace MyWebApp.Data
{
    // 数据库上下文，负责和MySQL数据库交互
    public class AppDbContext : DbContext
    {
        // 定义DbSet，对应MySQL的Users表，后续通过这个DbSet操作数据（增删改查）
        public DbSet<UserInfo> Users { get; set; }

        // 用于读取配置文件中的连接字符串
        private readonly IConfiguration _configuration;

        // 构造函数注入IConfiguration（用于读取appsettings.json）
        public AppDbContext(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // 配置数据库驱动和连接字符串
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // 读取appsettings.json中的MySQL连接字符串
            var connectionString = _configuration.GetConnectionString("mygame");

            // 配置使用Pomelo的MySQL驱动，并指定MySQL版本（8.0）
            optionsBuilder.UseMySql(connectionString, new MySqlServerVersion(new Version(8, 0, 45)))
                .EnableSensitiveDataLogging(); // 开发环境启用敏感数据日志，方便调试（生产环境关闭）
        }
    }
}