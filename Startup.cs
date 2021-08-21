using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using App.Security.Requirements;
using App.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using App.Models;

namespace App
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddOptions();
            var mailsetting = Configuration.GetSection("MailSettings");
            services.Configure<MailSettings>(mailsetting);
            services.AddSingleton<IEmailSender, SendMailService>();


            services.AddRazorPages(options => {
                
            });

            services.AddDbContext<AppDbContext>(options => {
                string connectString = Configuration.GetConnectionString("MyBlogContext");
                options.UseSqlServer(connectString);
            });

            // Dang ky Identity
            services.AddIdentity<AppUser, IdentityRole>()
                    .AddEntityFrameworkStores<AppDbContext>()
                    .AddDefaultTokenProviders();

            // services.AddDefaultIdentity<AppUser>()
            //         .AddEntityFrameworkStores<MyBlogContext>()
            //         .AddDefaultTokenProviders();
                    

            // Truy cập IdentityOptions
            services.Configure<IdentityOptions> (options => {
                // Thiết lập về Password
                options.Password.RequireDigit = false; // Không bắt phải có số
                options.Password.RequireLowercase = false; // Không bắt phải có chữ thường
                options.Password.RequireNonAlphanumeric = false; // Không bắt ký tự đặc biệt
                options.Password.RequireUppercase = false; // Không bắt buộc chữ in
                options.Password.RequiredLength = 3; // Số ký tự tối thiểu của password
                options.Password.RequiredUniqueChars = 1; // Số ký tự riêng biệt

                // Cấu hình Lockout - khóa user
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes (5); // Khóa 5 phút
                options.Lockout.MaxFailedAccessAttempts = 3; // Thất bại 3 lầ thì khóa
                options.Lockout.AllowedForNewUsers = true;

                // Cấu hình về User.
                options.User.AllowedUserNameCharacters = // các ký tự đặt tên user
                    "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
                options.User.RequireUniqueEmail = true;  // Email là duy nhất
            

                // Cấu hình đăng nhập.
                options.SignIn.RequireConfirmedEmail = true;            // Cấu hình xác thực địa chỉ email (email phải tồn tại)
                options.SignIn.RequireConfirmedPhoneNumber = false;     // Xác thực số điện thoại
                options.SignIn.RequireConfirmedAccount = true; 
                
            });      

            services.ConfigureApplicationCookie(options => {
                options.LoginPath = "/login/";
                options.LogoutPath = "/logout/";
                options.AccessDeniedPath = "/khongduoctruycap.html";
            });  

            services.AddAuthentication()
                    .AddGoogle(options => {
                        var gconfig = Configuration.GetSection("Authentication:Google");
                        options.ClientId = gconfig["ClientId"];
                        options.ClientSecret = gconfig["ClientSecret"];
                        // https://localhost:5001/signin-google
                        options.CallbackPath =  "/dang-nhap-tu-google";
                    })
                    .AddFacebook(options => {
                        var fconfig = Configuration.GetSection("Authentication:Facebook");
                        options.AppId  = fconfig["AppId"];
                        options.AppSecret = fconfig["AppSecret"];
                        options.CallbackPath =  "/dang-nhap-tu-facebook";
                    })
                    // .AddTwitter()
                    // .AddMicrosoftAccount()
                    ;

                services.AddSingleton<IdentityErrorDescriber, AppIdentityErrorDescriber>();

                services.AddAuthorization(options => {

                    options.AddPolicy("AllowEditRole", policyBuilder => {
                        policyBuilder.RequireAuthenticatedUser();
                        policyBuilder.RequireClaim("canedit", "user");
                    });

                    options.AddPolicy("InGenZ", policyBuilder => {
                        policyBuilder.RequireAuthenticatedUser();
                        // policyBuilder.RequireClaim("canedit", "user");
                        policyBuilder.Requirements.Add(new GenZRequirement()); // GenZRequirement

                        // new GenZRequirement() -> Authorization handler

                    });   

                    options.AddPolicy("ShowAdminMenu", pb => {
                        pb.RequireRole("Admin");
                    });      

                    options.AddPolicy("CanUpdateArticle", builder => {
                        builder.Requirements.Add(new ArticleUpdateRequirement()); 
                    });      

                });

                services.AddTransient<IAuthorizationHandler, AppAuthorizationHandler>();

            
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
            });

           

        }
    }
}

/*
CREATE, READ, UPDATE, DELETE (CRUD)

dotnet aspnet-codegenerator razorpage -m razorweb.models.Article -dc razorweb.models.MyBlogContext -outDir Pages/Blog -udl --referenceScriptLibraries


Identity:
    - Athentication: Xác định danh tính  -> Login, Logout ...
    
    - Authorization: Xác thực quyền truy cập

      * Role-based authorization - xác thực quyền theo vai trò
      - Role (vai tro): (Admin, Editor, Manager, Member ...)

      * Policy-based authorization
      * Claims-based authorization:
            Claims -> Đặc tính, tính chất của đối tượng (User)

            Bằng lái B2 (Role) -> được lái 4 chỗ
            - Ngày sinh -> claim
            - Nơi sinh -> claim

            Mua rượu ( > 18 tuổi)
             - Kiểm tra ngày sinh: Claims-based authorization
        

            


      
      Areas/Admin/Pages/Role
        Index
        Create
        Edit
        Delete

        dotnet new page -n Index -o Areas/Admin/Pages/Role -na App.Admin.Role
        dotnet new page -n Create -o Areas/Admin/Pages/Role -na App.Admin.Role
        dotnet new page -n EditUserRoleClaim -o Areas/Admin/Pages/User -na App.Admin.User



        [Authorize] - Controller, Action, PageModel -> Dang nhap




    - Quản lý user: Sign Up, User, Role  ...




 /Identity/Account/Login
 /Identity/Account/Manage

 dotnet aspnet-codegenerator identity -dc razorweb.models.MyBlogContext

CallbackPath:
 https://localhost:5001/dang-nhap-tu-google
 

*/