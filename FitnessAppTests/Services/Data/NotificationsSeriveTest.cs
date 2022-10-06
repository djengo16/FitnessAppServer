namespace FitnessApp.Tests.Services.Data
{
    using AutoMapper;
    using FitnessApp.AutoMapperProfiles;
    using FitnessApp.Data;
    using FitnessApp.Models;
    using FitnessApp.Models.Enums;
    using FitnessApp.Repositories;
    using FitnessApp.Services.Data;

    using Microsoft.EntityFrameworkCore;
    using Moq;

    public class NotificationsSeriveTest
    {
        private INotificationsService notificationsService;
        private ApplicationDbContext db;
        private const string TestUserId = "123";
        private const string TestRdirectId = "456";
        private const int TestNotificationId = 1;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
                db = new ApplicationDbContext(options);

            var workoutsStorage = new EfDeletableEntityRepository<WorkoutPlan>(db);
            var exercisesStorage = new EfDeletableEntityRepository<Exercise>(db);
            var workoutDayService = new Mock<IWorkoutDaysService>();
            var exerciseInWorkoutDayService = new Mock<IExerciseInWorkoutDayService>();

            var usersService = new Mock<IUsersService>();
            var notificationStorage = new EfRepository<Notification>(db);


            var workoutPlanProfiles = new WorkoutPlanProfiles();
            var notificationProfiles = new NotificationProfiles();
            var configuration = new MapperConfiguration(cfg => 
                cfg.AddProfiles(new List<Profile>() { notificationProfiles, workoutPlanProfiles }));
                var mapper = new Mapper(configuration);

            var mockWorkoutsService = new Mock<IWorkoutsService>();
            //Change IsTrainingDay behavior, boolean instead of plan id, so we can manipulate the return
            mockWorkoutsService.Setup(x => x.IsTrainingDay(It.IsAny<string>(), "true")).Returns(true);
            mockWorkoutsService.Setup(x => x.IsTrainingDay(It.IsAny<string>(), "false")).Returns(false);


            notificationsService = new NotificationsService(
                mockWorkoutsService.Object,
                notificationStorage,
                usersService.Object,
                mapper
                );
        }

        [Test]
        public async Task CreateNotificationAsyncWillCreateTrainingNotification()
        {
           await notificationsService.CreateNotificationAsync(TestUserId, NotificationType.TrainingDay);
            var result = notificationsService.GetById(TestNotificationId);

           Assert.NotNull(result);
           Assert.That(result.RecipientId == TestUserId);
           Assert.That(result.Type == NotificationType.TrainingDay);
        }

        [Test]
        public async Task CreateNotificationAsyncWillCreateUnreadMessageNotification()
        {
            await notificationsService.CreateNotificationAsync(TestUserId, NotificationType.UnreadMessage, TestRdirectId);

            var result = notificationsService.GetById(TestNotificationId);

            Assert.NotNull(result);
            Assert.That(result.RecipientId == TestUserId);
            Assert.That(result.Type == NotificationType.UnreadMessage);
            Assert.That(result.RedirectId == TestRdirectId);
        }

        [Test]
        public async Task SetupTrainingDayNotificationWillPassTheChecksAndCreateNotification()
        {
            var result = await notificationsService.SetupTrainingDayNotification(TestUserId, "true");

            Assert.NotNull(result);
            Assert.That(result.RecipientId == TestUserId);
            Assert.That(result.Type == NotificationType.TrainingDay);
        }

        [Test]
        public async Task SetupTrainingDayNotificationWillReturnNullIfItsNotTrainingDay()
        {
            var result = await notificationsService.SetupTrainingDayNotification(TestUserId, "false");

            Assert.IsNull(result);
        }

        [Test]
        public async Task SetupTrainingDayNotificationWillReturnNullIfNotificationIsAlreadyCreated()
        {
            //First call, should create
            await notificationsService.SetupTrainingDayNotification(TestUserId, "true");

            //Second call, should not create
            var result = await notificationsService.SetupTrainingDayNotification(TestUserId, "true");

            Assert.IsNull(result);
        }
        [Test]
        public async Task ViewNotificationAsyncWillSetIsViewedPropertyToTrueIfItsOfTypeTrainingDay()
        {
            await notificationsService.CreateNotificationAsync(TestUserId, NotificationType.TrainingDay);
            await notificationsService.ViewNotificationAsync(TestNotificationId);
            var notification = notificationsService.GetById(TestNotificationId);


            Assert.That(notification.IsViewed);
        }

        [Test]
        public async Task ViewNotificationAsyncWillDeleteNotificationIfItsOfTypeUnreadMessage()
        {
            await notificationsService.CreateNotificationAsync(TestUserId, NotificationType.UnreadMessage);
            await notificationsService.ViewNotificationAsync(TestNotificationId);
            var notification = notificationsService.GetById(TestNotificationId);

            Assert.IsNull(notification);
        }

        [Test]
        public async Task CheckUnreadMessageNotificationExistenceWillReturnTrueIfNotificationExists()
        {
            await notificationsService.CreateNotificationAsync(TestUserId, NotificationType.UnreadMessage, TestRdirectId);
            var not = notificationsService.GetById(1);

            var result = notificationsService.CheckUnreadMessageNotificationExistence(TestRdirectId, TestUserId);

            Assert.True(result);
        }

        [Test]
        public void CheckUnreadMessageNotificationExistenceWillReturnFalseIfNotificationDontExist()
        {
            var result = notificationsService.CheckUnreadMessageNotificationExistence(TestRdirectId, TestUserId);

            Assert.False(result);
        }

        [Test]
        public async Task CheckUnreadMessageNotificationExistenceWillReturnFalseIfNotificationIsViewed()
        {
            await notificationsService.CreateNotificationAsync(TestUserId, NotificationType.UnreadMessage, TestRdirectId);
            await notificationsService.ViewNotificationAsync(TestNotificationId);

            var result = notificationsService.CheckUnreadMessageNotificationExistence(TestRdirectId, TestUserId);

            Assert.False(result);
        }

        [Test]
        public async Task GetAllByRecipientIdWillReturnOnlyOneUsersNotifications()
        {
            string anotherUserId = "321";

            await notificationsService.CreateNotificationAsync(TestUserId, NotificationType.UnreadMessage, TestRdirectId);
            await notificationsService.CreateNotificationAsync(anotherUserId, NotificationType.UnreadMessage, TestRdirectId);

            var result = notificationsService.GetAllByRecipientId(TestUserId);

            Assert.That(result.Count == 1);
            Assert.That(result.FirstOrDefault().RecipientId == TestUserId);
        }
    }
}
