using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using MyWebApp.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyWebApp.Users
{
    public class UserEndPoint:Endpoint<UserRequest,UserResponse>
    {
        [Table("t_user")]
        public class UserInfo 
        {
            [Key]
            [Column("id")] // 显式映射到表中的id字段（可选，若实体字段名和表字段名一致，可省略）
            [DatabaseGenerated(DatabaseGeneratedOption.None)] // 核心：告诉EF Core，主键值不由数据库自动生成，由用户手动提供
            public string ID { get; set; } = string.Empty;
            [Column("name")] // 显式映射到表中的id字段（可选，若实体字段名和表字段名一致，可省略）
            public string Name { get; set; } = string.Empty;
            [Column("age")] // 显式映射到表中的id字段（可选，若实体字段名和表字段名一致，可省略）
            public int Age { get; set; } = 0;
            [Column("creattime")] // 显式映射到表中的id字段（可选，若实体字段名和表字段名一致，可省略）
            public string CreatTime { get; set; } = string.Empty;
        }

        // 构造函数注入
        public UserEndPoint(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // 注入AppDbContext（依赖注入，由.NET容器自动提供实例）
        private readonly AppDbContext _dbContext;
        public override void Configure()
        {
            Post("/api/user/create");
            AllowAnonymous();
        }

        public override async Task HandleAsync(UserRequest req, CancellationToken ct)
        {

            // 1. 把用户请求数据转换为User实体（和数据库表对应）
            var newUser = new UserInfo()
            {
                ID = Guid.NewGuid().ToString(),
                Name = req.FirstName + req.LastName,
                Age = req.Age,
                // CreateTime默认是当前时间，无需手动赋值
                CreatTime = System.DateTime.Now.ToString()
            };

            // 2. 将实体添加到DbContext的Users DbSet中（此时还未写入数据库）
            await _dbContext.Users.AddAsync(newUser, ct);

            // 3. 提交更改到MySQL数据库（真正写入数据，这一步执行后，数据库才会新增记录）
            await _dbContext.SaveChangesAsync(ct);

            // 4. 返回响应给客户端
            await Send.OkAsync(new UserResponse()
            {
                FullName = $"{req.FirstName} {req.LastName}",
                IsOver18 = req.Age > 18
            }, ct);

        }
    }
}
