using FitnessApp.Models.Enums;
using FitnessApp.Services.Factories;
using FitnessApp.Tests.BeforeAfter;

namespace FitnessApp.Tests
{
    public class WorkoutSlpitFactory
    {
        WorkoutSplitFactory _workoutSplitFactory = new WorkoutSplitFactory();


        [Fact]
        public void GenerateWorkoutSplitsBy3Days()
        {
            IDictionary<DayOfWeek, List<MuscleGroup>> expectedWorkout = GetExpectedFor3Days();
            IDictionary <DayOfWeek, List <MuscleGroup>> workout  = _workoutSplitFactory.CreateSplits(3);
            Assert.Equal(expectedWorkout, workout);

        }


        [Fact]
        public void GenerateWorkoutSplitsBy4Days()
        {
            IDictionary<DayOfWeek, List<MuscleGroup>> expectedWorkout = GetExpectedFor4Days();
            IDictionary<DayOfWeek, List<MuscleGroup>> workout = _workoutSplitFactory.CreateSplits(4);
            Assert.Equal(expectedWorkout, workout);

        }


        private IDictionary<DayOfWeek, List<MuscleGroup>> GetExpectedFor3Days()
        {
            IDictionary<DayOfWeek, List<MuscleGroup>> expectedWorkout = new Dictionary<DayOfWeek, List<MuscleGroup>>();
            expectedWorkout.Add(DayOfWeek.Monday,
        new List<MuscleGroup>() { MuscleGroup.Chest, MuscleGroup.Shoulders, MuscleGroup.Triceps });

            expectedWorkout.Add(DayOfWeek.Wednesday,
                new List<MuscleGroup>() { MuscleGroup.Back, MuscleGroup.Biceps });

            expectedWorkout.Add(DayOfWeek.Friday,
                new List<MuscleGroup>() { MuscleGroup.Legs, MuscleGroup.Abs });
            return expectedWorkout;
        }

        private IDictionary<DayOfWeek, List<MuscleGroup>> GetExpectedFor4Days()
        {
            IDictionary<DayOfWeek, List<MuscleGroup>> expectedWorkout = new Dictionary<DayOfWeek, List<MuscleGroup>>();
    
            expectedWorkout.Add(DayOfWeek.Monday,
                new List<MuscleGroup>() { MuscleGroup.Back, MuscleGroup.Biceps });

            expectedWorkout.Add(DayOfWeek.Tuesday,
                new List<MuscleGroup>() { MuscleGroup.Chest, MuscleGroup.Triceps });

            expectedWorkout.Add(DayOfWeek.Thursday,
                new List<MuscleGroup>() { MuscleGroup.Legs });

            expectedWorkout.Add(DayOfWeek.Friday,
                new List<MuscleGroup>() { MuscleGroup.Shoulders, MuscleGroup.Abs });
            return expectedWorkout;
        }
    }
}