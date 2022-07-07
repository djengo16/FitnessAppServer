namespace FitnessApp.Models
{
    using FitnessApp.Models.Enums;
    using FitnessApp.Models.Common;
    using System.ComponentModel.DataAnnotations;

    public class Exercise : IDeletableEntity
    {
#nullable enable
        public Exercise()
        {
            ExerciseInWorkoutDay = new HashSet<ExerciseInWorkoutDay>();
        }
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = "";
        public bool IsDeleted { get; set; }
        public DateTime? DeletedOn { get; set; }
        public MuscleGroup MuscleGroup { get; set; }
        public Difficulty Difficulty { get; set; }
        public string? Description { get; set; }
        public string? PictureResourceUrl { get; set; }
        public string? VideoResourceUrl { get; set; }
        public virtual ICollection<ExerciseInWorkoutDay> ExerciseInWorkoutDay { get; set; }
        bool IDeletableEntity.IsDeleted { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        DateTime? IDeletableEntity.DeletedOn { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
         }
}