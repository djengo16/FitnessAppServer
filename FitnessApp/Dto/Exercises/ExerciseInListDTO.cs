namespace FitnessApp.Dto.Exercises
{
    public class ExerciseInListDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string MuscleGroup { get; set; }
        public string Difficulty { get; set; }
        public string Description { get; set; }
        public string PictureResourceUrl { get; set; }
        public string VideoResourceUrl { get; set; }
    }
}
