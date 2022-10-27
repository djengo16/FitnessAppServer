namespace FitnessApp.Tests.Helper
{
    using FitnessApp.Data;
    using FitnessApp.Models;
    using FitnessApp.Models.Repositories;
    using FitnessApp.Repositories;
    using FitnessApp.Services.Data;
    using FitnessApp.Services.Security;
    using FitnessApp.Services.SocketService;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Microsoft.IdentityModel.Tokens;

    public static class ServiceHelper
    {
        private const string dbContextName  = "MyInMemoryDatabase";
        public static T GetRequiredService<T>()
        {
            var provider = Provider();

            return provider.GetRequiredService<T>();
        }

        private static IServiceProvider Provider()
        {
            var services = new ServiceCollection();

            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            // Data repositories
            services.AddScoped(typeof(IDeletableEntityRepository<>), typeof(EfDeletableEntityRepository<>));
            services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));

            services.AddSingleton<IWebSocketService, WebSocketService>();

            services
                .AddLogging(logging => logging.AddConsole())
                .AddScoped<IUsersService, UsersService>()
                .AddTransient<IWorkoutsService, WorkoutsService>()
                .AddTransient<IWorkoutDaysService, WorkoutDaysService>()
                .AddTransient<IExerciseInWorkoutDayService, ExerciseInWorkoutDayService>()
                .AddTransient<INotificationsService, NotificationsService>()
                .AddTransient<IExercisesService, ExercisesService>()
                .AddTransient<IConversationService, ConversationService>()
                .AddTransient<IMessagesService, MessagesService>()
                .AddTransient<IJwtService, JwtService>()
                .AddTransient<IWebSocketService, WebSocketService>();


            services.AddDbContext<ApplicationDbContext>(options => options.UseInMemoryDatabase(dbContextName));
            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.SignIn.RequireConfirmedAccount = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 6;
            })
            .AddEntityFrameworkStores<ApplicationDbContext>();
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            });

            return services.BuildServiceProvider();
        }
    }
}
