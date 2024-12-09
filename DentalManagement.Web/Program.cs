using DentalManagement.DomainModels;
using DentalManagement.Web.AppCodes;
using DentalManagement.Web.Data;
using DentalManagement.Web.Hubs;
using DentalManagement.Web.Interfaces;
using DentalManagement.Web.Repository;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.General;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddHttpContextAccessor();
builder.Services.AddControllersWithViews()
    .AddMvcOptions(option =>
    {
        option.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true;
    });
builder.Services.AddScoped<IRepository<Employee>, EmployeeRepository>();
builder.Services.AddScoped<IRepository<Dentist>, DentistRepository>();
builder.Services.AddScoped<IRepository<Medicine>, MedicineRepository>();
builder.Services.AddScoped<IRepository<Patient>, PatientRepository>();
builder.Services.AddScoped<IRepository<Service>, ServiceRepository>();
builder.Services.AddScoped<IInvoiceRepository, InvoiceRepository>();
builder.Services.AddScoped<IAppointmentRepository, AppointmentRepository>();
builder.Services.AddScoped<IInvoiceDetails, InvoiceDetailsRepository>();
builder.Services.AddTransient<InvoiceRepository>();
builder.Services.AddScoped<InvoiceDetailsRepository>();
builder.Services.AddScoped<PatientRepository>();
builder.Services.AddScoped<MedicineRepository>();
builder.Services.AddScoped<ServiceRepository>();
builder.Services.AddScoped<AppointmentRepository>();
builder.Services.AddScoped<AppointmentStatus>();
builder.Services.AddScoped<PaymentRepository>();
builder.Services.AddScoped<IPayment,PaymentRepository>();
builder.Services.AddScoped<IUserAccount, UserAccountRepository>();
builder.Services.AddTransient<IInvoiceRepository,InvoiceRepository>();
builder.Services.AddScoped<EmailSerivce>();
builder.Services.AddScoped<StatisticsService>();
builder.Services.AddScoped<MessageRepository>();
builder.Services.AddScoped<MedicalRecordRepository>();
builder.Services.AddScoped<PrescriptionRepository>();
builder.Services.AddMvc();
builder.Services.AddSignalR();
builder.Services.AddHttpClient();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader());
});
// Cấu hình DbContext trước khi build app
builder.Services.AddDbContext<DentalManagementDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DentalManagement"));
});

builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 10485760; // 10 MB
});
builder.Services.Configure<CookiePolicyOptions>(options =>
{
    options.CheckConsentNeeded = context => true;
    options.MinimumSameSitePolicy = SameSiteMode.None;
});
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(option =>
                {
                    option.Cookie.Name = "AuthenticationCookie";
                    option.LoginPath = "/Account/Login";
                    option.AccessDeniedPath = "/Account/AccessDenined";
                    option.ExpireTimeSpan = TimeSpan.FromDays(360);
                });
builder.Services.AddSession(option =>
{
    option.IdleTimeout = TimeSpan.FromMinutes(200);
    option.Cookie.HttpOnly = true;
    option.Cookie.IsEssential = true;
});
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp",
        builder => builder
            .WithOrigins("http://localhost:3007")
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials());
});
builder.Services.AddControllersWithViews();
builder.Logging.AddConsole();
var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
else
{
    app.UseCors("AllowAll");
}

app.UseSession();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseCookiePolicy();
app.UseAuthentication(); // Đảm bảo middleware xác thực được thêm vào
app.UseAuthorization();
app.UseCors("AllowAll");
app.MapControllers();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
ApplicationContext.Configure
(
    httpContextAccessor: app.Services.GetRequiredService<IHttpContextAccessor>(),
    hostEnvironment: app.Services.GetRequiredService<IWebHostEnvironment>()
);
app.MapHub<ChatHub>("/chatHub");
app.Run();
