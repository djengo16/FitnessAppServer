﻿namespace FitnessApp.Tests.Controllers

    using FitnessApp.Dto.Exercises;
    using FitnessApp.Models.Enums;
    using System.Net.Http.Json;

    public class ExercisesControllerTests : IntegrationTest
            using var scope = appFactory.Services.CreateScope();


        [Test]

        [Test]
        public async Task Create_WillRespondSuccessfully_WithValidData_And_Administrator_Rights()
        {
            using var scope = appFactory.Services.CreateScope();

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
            using var scope = appFactory.Services.CreateScope();

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
            using var scope = appFactory.Services.CreateScope();

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
    }