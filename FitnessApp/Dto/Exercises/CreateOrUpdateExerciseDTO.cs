namespace FitnessApp.Dto.Exercises
{
    using FitnessApp.Models.Enums;
    using System.ComponentModel.DataAnnotations;

    public class CreateOrUpdateExerciseDTO
    {
#nullable enable
        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = "";
        [Required]
        public MuscleGroup MuscleGroup { get; set; }
        [Required]
        public Difficulty Difficulty { get; set; }
        public string? Description { get; set; }
        public string? PictureResourceUrl { get; set; }
        public string? VideoResourceUrl { get; set; }
    }
}
