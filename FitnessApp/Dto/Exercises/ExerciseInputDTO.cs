using System.ComponentModel.DataAnnotations;

namespace FitnessApp.Dto.Exercises
{
    public class ExerciseInputDTO
    {
        [Required]
        [MaxLength(200)]
        public string Name { get; set; }
        public string MuscleGroup { get; set; }
        public string Difficulty { get; set; }
        public string Description { get; set; }
        public string PictureResourceUrl { get; set; }
        public string VideoResourceUrl { get; set; }
    }
}
