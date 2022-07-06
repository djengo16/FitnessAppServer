using FitnessApp.Models.Enums;

namespace FitnessApp.Seeding.Dto
{
    public class UserTrainingProgramCombinationDTO
    {
        public Goal Goal { get; set; }
        public int Days { get; set; }
        public Difficulty Difficulty { get; set; }
    }
}
