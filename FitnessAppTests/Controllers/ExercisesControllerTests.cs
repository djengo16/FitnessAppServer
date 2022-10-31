namespace FitnessApp.Tests.Controllers{    using System.Net;
    using FitnessApp.Services.Data;    using FitnessApp.Tests.Helper;    using FitnessApp.Tests.Helper.Enum;    using Microsoft.AspNetCore.Mvc.Testing;    using Microsoft.Extensions.DependencyInjection;    using FitnessApp.Services.ServiceConstants;
    using FitnessApp.Dto.Exercises;
    using FitnessApp.Models.Enums;
    using System.Net.Http.Json;

    public class ExercisesControllerTests : IntegrationTest    {        private WebApplicationFactory<Program> appFactory;        [SetUp]        public async Task Setup()        {            appFactory = new WebApplicationFactory<Program>();            this._TestHttpClient = appFactory.CreateClient();        }        [Test]        public async Task Get_WillRespondSuccessfully_OnValidParameters()        {
            using var scope = appFactory.Services.CreateScope();            var exerciseService = scope.ServiceProvider.GetRequiredService<IExercisesService>();
            var testId = exerciseService?                .GetExercises("", 1, 0)                .Exercises                .First().Id;            await this.AutheticateAsync(RoleType.User);            var response = await _TestHttpClient.GetAsync($"/exercises/{testId}");            Assert.True(response.IsSuccessStatusCode);            Assert.True(response.StatusCode == HttpStatusCode.OK);        }        [Test]        public async Task Get_WillNotRespondSuccessfully_OnInValidParameters()        {            var testId = -1;            await this.AutheticateAsync(RoleType.User);            var response = await _TestHttpClient.GetAsync($"/exercises/{testId}");            var responseMessage = await response.Content.ReadAsStringAsync();            Assert.False(response.IsSuccessStatusCode);            Assert.True(response.StatusCode == HttpStatusCode.BadRequest);            Assert.True(responseMessage == ErrorMessages.ExerciseNotFound);        }

        [Test]        public async Task Get_WillNotRespondSuccessfully_OnUanthorizedUser()        {            var testId = 1;            var response = await _TestHttpClient.GetAsync($"/exercises/{testId}");            Assert.False(response.IsSuccessStatusCode);            Assert.True(response.StatusCode == HttpStatusCode.Unauthorized);        }

        [Test]
        public async Task Create_WillRespondSuccessfully_WithValidData_And_Administrator_Rights()
        {
            using var scope = appFactory.Services.CreateScope();            var exerciseService = scope.ServiceProvider.GetRequiredService<IExercisesService>();

            ExerciseInputDTO exercise = new ExerciseInputDTO()
            {
                Name = "test name",
                Description = "test",
                MuscleGroup = MuscleGroup.Chest.ToString(),
                Difficulty = Difficulty.Medium.ToString(),
            };

            await this.AutheticateAsync(RoleType.Administrator);

            var response = await _TestHttpClient.PostAsJsonAsync("/exercises/create", exercise);

            //delete
            var id = exerciseService.GetIdByName("test name");
            await DeleteTestExercise(id);

            //assert
            Assert.True(response.StatusCode == HttpStatusCode.OK);
        }

        [Test]
        public async Task Create_WillNotRespondSuccessfully_WithInValidData_And_Administrator_Rights()
        {
            using var scope = appFactory.Services.CreateScope();            var exerciseService = scope.ServiceProvider.GetRequiredService<IExercisesService>();

            ExerciseInputDTO exercise = new ExerciseInputDTO()
            {
                Description = "test",
                MuscleGroup = MuscleGroup.Chest.ToString(),
                Difficulty = Difficulty.Medium.ToString(),
            };

            await this.AutheticateAsync(RoleType.Administrator);

            var response = await _TestHttpClient.PostAsJsonAsync("/exercises/create", exercise);

            //assert
            Assert.False(response.IsSuccessStatusCode);
        }

        [Test]
        public async Task Create_WillNotRespondSuccessfully_Without_Administrator_Rights()
        {
            using var scope = appFactory.Services.CreateScope();            var exerciseService = scope.ServiceProvider.GetRequiredService<IExercisesService>();

            ExerciseInputDTO exercise = new ExerciseInputDTO()
            {
                Description = "test",
                MuscleGroup = MuscleGroup.Chest.ToString(),
                Difficulty = Difficulty.Medium.ToString(),
            };

            var response = await _TestHttpClient.PostAsJsonAsync("/exercises/create", exercise);

            //assert
            Assert.False(response.IsSuccessStatusCode);
        }

        private async Task DeleteTestExercise(int id)
        {
            await _TestHttpClient.DeleteAsync($"/exercises/hardDelete/{id}");
        }
    }}