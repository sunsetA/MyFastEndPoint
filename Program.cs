using FastEndpoints;
using MyWebApp.Data;

var bld = WebApplication.CreateBuilder();
bld.Services.AddFastEndpoints();
// 2. 注册AppDbContext（核心：添加数据库上下文服务）
bld.Services.AddDbContext<AppDbContext>();
var app = bld.Build();
app.UseFastEndpoints();


// 新增：手动测试数据库连接
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    try
    {
        // 尝试连接数据库，并执行一个简单的查询（不修改数据）
        var canConnect = await dbContext.Database.CanConnectAsync();
        if (canConnect)
        {
            Console.WriteLine(" 数据库连接成功！");
        }
        else
        {
            Console.WriteLine(" 数据库连接失败！");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($" 数据库连接异常：{ex.Message}");
    }
}

app.Run();
