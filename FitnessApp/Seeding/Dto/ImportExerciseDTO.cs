namespace FitnessApp.Seeding.Dto
{
    using FitnessApp.Models.Enums;
    public class ImportExerciseDTO
    {
#nullable enable
        public string Name { get; set; } = "";
        public MuscleGroup MuscleGroup { get; set; }
        public Difficulty Difficulty { get; set; }
        public string[]? Description { get; set; }
        public string? PictureResourceUrl { get; set; }
        public string? VideoResourceUrl { get; set; }
    }
}