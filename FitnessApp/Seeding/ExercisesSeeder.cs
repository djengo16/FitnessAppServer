namespace FitnessApp.Seeding
{
    using FitnessApp.Data;
    using FitnessApp.Models;
    using FitnessApp.Seeding.Dto;
    using Newtonsoft.Json;

    public class ExercisesSeeder : ISeeder
    {
        public async Task SeedAsync(ApplicationDbContext dbContext, IServiceProvider serviceProvider)
        {
            if (dbContext.Exercises.Any())
            {
                return;
            }

            ImportExerciseDTO[] importedExercises = JsonConvert
                .DeserializeObject<ImportExerciseDTO[]>
                (File.ReadAllText(@"D:\Diploma work\FitnessApp\FitnessApp\Seeding\Data\Exercises.json"));

            List<Exercise> exercises = new List<Exercise>();

            foreach (var importedExercise in importedExercises)
            {
                var exercise = new Exercise()
                {
                    Name = importedExercise.Name,
                    MuscleGroup = importedExercise.MuscleGroup,
                    Difficulty = importedExercise.Difficulty,
                };

                if (importedExercise.Description != null && importedExercise.Description.Length != 0)
                {
                    exercise.Description = String.Join("\n", importedExercise.Description).ToString();
                }

                if (!String.IsNullOrEmpty(importedExercise.PictureResourceUrl))
                {
                    exercise.PictureResourceUrl = importedExercise.PictureResourceUrl;
                }

                if (!String.IsNullOrEmpty(importedExercise.VideoResourceUrl))
                {
                    exercise.VideoResourceUrl = importedExercise.VideoResourceUrl;
                }

                exercises.Add(exercise);
            }

            await dbContext.AddRangeAsync(exercises);
            await dbContext.SaveChangesAsync();
        }
    }
}