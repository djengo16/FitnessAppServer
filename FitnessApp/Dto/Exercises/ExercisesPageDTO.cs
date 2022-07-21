namespace FitnessApp.Dto.Exercises
{
    public class ExercisesPageDTO : Pageable
    {
        public ICollection<ExerciseInListDTO> Exercises { get; set; }
    }
}