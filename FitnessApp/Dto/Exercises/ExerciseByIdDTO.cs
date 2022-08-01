using FitnessApp.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace FitnessApp.Dto.Exercises
{
    public class ExerciseByIdDTO
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = "";
        public MuscleGroup MuscleGroup { get; set; }
        public Difficulty Difficulty { get; set; }
        public string Description { get; set; }
        public string PictureResourceUrl { get; set; }
        public string VideoResourceUrl { get; set; }
    }
}
