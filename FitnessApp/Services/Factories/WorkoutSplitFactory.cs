namespace FitnessApp.Services.Factories
{
    using FitnessApp.Models.Enums;
    using FitnessApp.Services.ServiceConstants;

    public class WorkoutSplitFactory
    {
        private IDictionary<DayOfWeek, List<MuscleGroup>> workoutSplit;
        public IDictionary<DayOfWeek, List<MuscleGroup>> CreateSplits(int days)
        {
            if (days > 7 || days < 1)
            {
                throw new ArgumentException(ErrorMessages.InvalidDaysOfWeek);
            }
            if (!(days >= 3 && days <= 5))
            {
                throw new ArgumentException(ErrorMessages.WorkoutPlanDaysNotInRange);
            }

            workoutSplit = new Dictionary<DayOfWeek, List<MuscleGroup>>();
            switch (days)
            {
                case 3:
                    CreateThreeDaySplit();
                    break;
                case 4:
                    CreateFourDaySplit();
                    break;
                case 5:
                    CreateFiveDaySplit();
                    break;
                default:
                    break;
            }
            return workoutSplit;
        }

        private void CreateThreeDaySplit()
        {
            workoutSplit.Add(DayOfWeek.Monday,
                new List<MuscleGroup>() { MuscleGroup.Chest, MuscleGroup.Shoulders, MuscleGroup.Triceps });

            workoutSplit.Add(DayOfWeek.Wednesday,
                new List<MuscleGroup>() { MuscleGroup.Back, MuscleGroup.Biceps });

            workoutSplit.Add(DayOfWeek.Friday,
                new List<MuscleGroup>() { MuscleGroup.Legs, MuscleGroup.Abs });
        }

        private void CreateFourDaySplit()
        {
            workoutSplit.Add(DayOfWeek.Monday,
                new List<MuscleGroup>() { MuscleGroup.Back, MuscleGroup.Biceps });

            workoutSplit.Add(DayOfWeek.Tuesday,
                new List<MuscleGroup>() { MuscleGroup.Chest, MuscleGroup.Triceps });

            workoutSplit.Add(DayOfWeek.Thursday,
                new List<MuscleGroup>() { MuscleGroup.Legs });

            workoutSplit.Add(DayOfWeek.Friday,
                new List<MuscleGroup>() { MuscleGroup.Shoulders, MuscleGroup.Abs });
        }

        private void CreateFiveDaySplit()
        {
            workoutSplit.Add(DayOfWeek.Monday,
                new List<MuscleGroup>() { MuscleGroup.Chest });

            workoutSplit.Add(DayOfWeek.Tuesday,
                new List<MuscleGroup>() { MuscleGroup.Back });

            workoutSplit.Add(DayOfWeek.Wednesday,
                new List<MuscleGroup>() { MuscleGroup.Shoulders });

            workoutSplit.Add(DayOfWeek.Thursday,
                new List<MuscleGroup>() { MuscleGroup.Biceps, MuscleGroup.Triceps });

            workoutSplit.Add(DayOfWeek.Friday,
                new List<MuscleGroup>() { MuscleGroup.Legs, MuscleGroup.Abs });
        }
    }
}