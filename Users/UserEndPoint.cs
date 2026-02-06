using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using MyWebApp.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Threading;

namespace MyWebApp.Users
{
    // 将实体提升为文件级可复用类型
    [Table("t_user")]
    public class UserInfo
    {
        [Key]
        [Column("id")]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string ID { get; set; } = string.Empty;

        [Column("name")]
        public string Name { get; set; } = string.Empty;

        [Column("age")]
        public int Age { get; set; } = 0;

        [Column("creattime")]
        public DateTime CreatTime { get; set; }
    }

    // 原有的创建用户 Endpoint（保持不变）
    public class UserEndPoint : Endpoint<UserRequest, UserResponse>
    {
        private readonly AppDbContext _dbContext;

        public UserEndPoint(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public override void Configure()
        {
            Post("/api/user/create");
            AllowAnonymous();
        }

        public override async Task HandleAsync(UserRequest req, CancellationToken ct)
        {
            var newUser = new UserInfo()
            {
                ID = Guid.NewGuid().ToString(),
                Name = req.FirstName + req.LastName,
                Age = req.Age,
                CreatTime = System.DateTime.Now
            };

            await _dbContext.Users.AddAsync(newUser, ct);
            await _dbContext.SaveChangesAsync(ct);

            await Send.OkAsync(new UserResponse()
            {
                FullName = $"{req.FirstName} {req.LastName}",
                IsOver18 = req.Age > 18
            }, ct);
        }
    }

    // 用于绑定路由参数 /api/user/{id}
    public class GetUserRequest
    {
        public string Name { get; set; } = string.Empty;
    }

    // 新增：查询用户 Endpoint，返回完整的 UserInfo 实体
    public class GetUserEndpoint : Endpoint<GetUserRequest, UserInfo>
    {
        private readonly AppDbContext _dbContext;

        public GetUserEndpoint(AppDbContext dbContext) => _dbContext = dbContext;

        public override void Configure()
        {
            Get("/api/user/{name}");
            AllowAnonymous();
        }

        public override async Task HandleAsync(GetUserRequest req, CancellationToken ct)
        {
            // 处理空 name（返回全部或返回 400 视业务需求）

            var users = await _dbContext.Users
                .Where(u => u.Name == req.Name)
                .ToListAsync(ct);

            await Send.OkAsync(users[0], ct);

        }
    }
}