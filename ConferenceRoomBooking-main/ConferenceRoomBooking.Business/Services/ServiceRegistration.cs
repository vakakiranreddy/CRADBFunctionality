using ConferenceRoomBooking.DataAccess.Data;
using ConferenceRoomBooking.DataAccess.Interfaces.IRepositories;
using ConferenceRoomBooking.Business.Interfaces.IServices;
using ConferenceRoomBooking.DataAccess.Repositories;
using ConferenceRoomBooking.Business.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

namespace ConferenceRoomBooking.Business.Services
{
    public static class ServiceRegistration
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Database Context
            services.AddDbContext<ConferenceRoomBookingDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            // Email Settings
            services.Configure<EmailSettings>(configuration.GetSection("EmailSettings"));

            // Repositories
            services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<ILocationRepository, LocationRepository>();
            services.AddScoped<IBuildingRepository, BuildingRepository>();
            services.AddScoped<IFloorRepository, FloorRepository>();
            services.AddScoped<IResourceRepository, ResourceRepository>();
            services.AddScoped<IRoomRepository, RoomRepository>();
            services.AddScoped<IDeskRepository, DeskRepository>();
            services.AddScoped<IBookingRepository, BookingRepository>();
            services.AddScoped<IEventRepository, EventRepository>();
            services.AddScoped<IEventRSVPRepository, EventRSVPRepository>();
            services.AddScoped<IDepartmentRepository, DepartmentRepository>();
            services.AddScoped<IBookingCheckInRepository, BookingCheckInRepository>();
            services.AddScoped<IUserNotificationRepository, UserNotificationRepository>();
            services.AddScoped<IUserOtpVerificationRepository, UserOtpVerificationRepository>();
            services.AddScoped<IBroadcastNotificationRepository, BroadcastNotificationRepository>();

            // Services
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ILocationService, LocationService>();
            services.AddScoped<IBuildingService, BuildingService>();
            services.AddScoped<IFloorService, FloorService>();
            services.AddScoped<IResourceService, ResourceService>();
            services.AddScoped<IRoomService, RoomService>();           // ADD THIS
            services.AddScoped<IDeskService, DeskService>();           // ADD THIS
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IOtpService, OtpService>();
            services.AddScoped<IUserNotificationService, UserNotificationService>();
            services.AddScoped<IBookingService, BookingService>();
            services.AddScoped<IBookingCheckInService, BookingCheckInService>();
            services.AddScoped<IEventService, EventService>();
            services.AddScoped<IEventRSVPService, EventRSVPService>();
            services.AddScoped<IBroadcastNotificationService, BroadcastNotificationService>();
            services.AddScoped<IDepartmentService, DepartmentService>();

            // Background Services
            services.AddHostedService<NotificationBackgroundService>();

            return services;
        }
    }
}








