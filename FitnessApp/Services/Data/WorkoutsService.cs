namespace FitnessApp.Services.Data
{
    using FitnessApp.Dto.Workouts;
    using FitnessApp.Models;
    using FitnessApp.Models.Enums;
    using FitnessApp.Models.Repositories;
    using FitnessApp.Services.Factories;
    using FitnessApp.Services.ServiceConstants;

    public class WorkoutsService : IWorkoutsService
    {
        private IDictionary<DayOfWeek, List<MuscleGroup>> workoutSplit;
        private IDeletableEntityRepository<Exercise> _exercises;
        private WorkoutPlan workoutPlan;
        private WorkoutSplitFactory workoutSplitFactory = new WorkoutSplitFactory();
        private int sets;
        private int minReps;
        private int maxReps;
        public WorkoutsService(IDeletableEntityRepository<Exercise> exercises)
        {
            _exercises = exercises;
            workoutSplit = new Dictionary<DayOfWeek, List<MuscleGroup>>();
            workoutPlan = new WorkoutPlan();
        }

        private void LoadExerciseDefaultValues()
        {
            sets = WorkoutConstants.AvgExerciseSet;
            minReps = WorkoutConstants.AvgExerciseMinReps;
            maxReps = WorkoutConstants.AvgExerciseMaxReps;
        }


        public void GenerateWorkoutPlan(WorkoutGenerationInputModel inputModel)
        {
            workoutSplit = workoutSplitFactory.CreateSplits(inputModel.Days);

            foreach (var split in workoutSplit)
            {
                CreateWorkoutDay(split, inputModel);
            }
        }

        /* Create workout day considering totalWorkoutDays, user (goal, experience)
         * Avg exercise count per muscle is 4 by my opinion (after looking at several programs)
         * we can manipulate this considering input data
         * 
        */
        private void CreateWorkoutDay(
            KeyValuePair<DayOfWeek, List<MuscleGroup>> split, WorkoutGenerationInputModel inputModel)
        {
            WorkoutDay workoutDay = new WorkoutDay();
            workoutDay.Day = split.Key;

            switch (split.Value.Count)
            {
                case 1:
                    workoutDay.ExercisesInWorkoutDays = CreateSeparateMuscleDay(inputModel, split.Value);
                    break;
                case 2:
                    workoutDay.ExercisesInWorkoutDays = CreateSplitOfTwoMusclesDay(inputModel, split.Value);
                    break;
                default:
                    workoutDay.ExercisesInWorkoutDays = CreateSplitOfThreeMusclesDay(inputModel, split.Value);
                    break;
            }

            workoutPlan.WorkoutDays.Add(workoutDay);
        }

        private ICollection<ExerciseInWorkoutDay> CreateSplitOfThreeMusclesDay(
            WorkoutGenerationInputModel inputModel, 
            List<MuscleGroup> muscles)
        {
            throw new NotImplementedException();
        }

        private ICollection<ExerciseInWorkoutDay> CreateSplitOfTwoMusclesDay(
            WorkoutGenerationInputModel inputModel, 
            List<MuscleGroup> value)
        {
            throw new NotImplementedException();
        }

        private ICollection<ExerciseInWorkoutDay> CreateSeparateMuscleDay(
            WorkoutGenerationInputModel inputModel,
            List<MuscleGroup> value)
        {
            List<ExerciseInWorkoutDay> exercises = GetExercisesForSeparateMuscleForWorkoutDay(
                inputModel,
                value[0]);

            return exercises;


        }
        /* We know that the minimum exercise count for separata muscle day is 5
         */
        private List<ExerciseInWorkoutDay> GetExercisesForSeparateMuscleForWorkoutDay(
            WorkoutGenerationInputModel inputModel,
            MuscleGroup muscleGroup)
        {
            LoadExerciseDefaultValues();
            ConfigureExercise(inputModel);

            List<ExerciseInWorkoutDay> exercisesInWorkoutDay = new List<ExerciseInWorkoutDay>();

            return GenerateExercisesForCurrentWorkoutDay(muscleGroup, exercisesInWorkoutDay, inputModel);
        }

        private List<ExerciseInWorkoutDay> GenerateExercisesForCurrentWorkoutDay(
            MuscleGroup muscleGroup,
            List<ExerciseInWorkoutDay> exercisesInWorkoutDay,
            WorkoutGenerationInputModel inputModel,
            int offset = 0)
        {
            int exercisesCount = 3 + offset;

            List<Exercise> currBodyPartEasyExercises = _exercises
                .AllAsNoTracking()
                .Where(x => x.MuscleGroup == muscleGroup ||
                x.Difficulty == Difficulty.Easy)
                .ToList();

            List<Exercise> currBodyPartMediumExercises = _exercises
                .AllAsNoTracking()
                .Where(x => x.MuscleGroup == muscleGroup ||
                x.Difficulty == Difficulty.Medium)
                .ToList();

            List<Exercise> currBodyPartHardExercises = _exercises
                .AllAsNoTracking()
                .Where(x => x.MuscleGroup == muscleGroup ||
                x.Difficulty == Difficulty.Hard)
                .ToList();

            /* Easy program => 5 _exercises | Medium = 6 | Hard = 7
             * firstly create 3 easy and 2 medium for begginers then + 1 hard for intermediate & + 2 hard for advanced
             */
            exercisesInWorkoutDay
                .AddRange(GetSpecificExercisesForWorkoutDay(currBodyPartEasyExercises, exercisesCount, sets, minReps, maxReps));
            exercisesInWorkoutDay
                .AddRange(GetSpecificExercisesForWorkoutDay(currBodyPartMediumExercises, exercisesCount - 1, sets, minReps, maxReps));

            if (inputModel.Difficulty == Difficulty.Medium)
            {
                exercisesInWorkoutDay
                    .AddRange(GetSpecificExercisesForWorkoutDay(currBodyPartHardExercises, exercisesCount - 2, sets, minReps, maxReps));
            }
            if (inputModel.Difficulty == Difficulty.Hard)
            {
                exercisesInWorkoutDay
                    .AddRange(GetSpecificExercisesForWorkoutDay(currBodyPartHardExercises, exercisesCount - 1, sets, minReps, maxReps));
            }

            return exercisesInWorkoutDay;
        }

        private void ConfigureExercise(WorkoutGenerationInputModel inputModel )
        {
            // More reps with lighter weights for losing weight

            sets = CalculateSetsByDifficulty(inputModel);

            if (inputModel.Goal == Goal.LoseWeight)
            {
                minReps = 12;
                maxReps = 16;
            }
        }
        private int CalculateSetsByDifficulty(WorkoutGenerationInputModel inputModel)
        {
            switch (inputModel.Difficulty)
            {
                case Difficulty.Medium: return sets++;
                case Difficulty.Hard: return sets += 2;
                default: return sets;
            }
        }

        private List<ExerciseInWorkoutDay> GetSpecificExercisesForWorkoutDay(
            List<Exercise> exercises, 
            int count,
            int sets,
            int minReps,
            int maxReps)
        {
            return exercises.OrderBy(x => Guid.NewGuid()).Take(count).Select(x => new ExerciseInWorkoutDay
            {
                ExerciseId = x.Id,
                Sets = sets,
                MinReps = minReps,
                MaxRepos = maxReps,
            }).ToList();
        }
    }
}