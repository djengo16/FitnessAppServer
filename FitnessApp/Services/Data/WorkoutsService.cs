namespace FitnessApp.Services.Data
{
    using AutoMapper;
    using AutoMapper.QueryableExtensions;
    using FitnessApp.Data;
    using FitnessApp.Dto.ExerciseInWorkoutDay;
    using FitnessApp.Dto.WorkoutDays;
    using FitnessApp.Dto.Workouts;
    using FitnessApp.Models;
    using FitnessApp.Models.Enums;
    using FitnessApp.Models.Repositories;
    using FitnessApp.Services.Factories;
    using FitnessApp.Services.ServiceConstants;
    using System.Linq;

    public class WorkoutsService : IWorkoutsService
    {
        private IDictionary<DayOfWeek, List<MuscleGroup>> workoutSplit;
        private IDeletableEntityRepository<Exercise> _exercises;
        private readonly IDeletableEntityRepository<WorkoutPlan> workoutPlansStorage;
        private readonly IWorkoutDaysService workoutDaysService;
        private readonly IExerciseInWorkoutDayService exerciseInWorkoutDayService;
        private readonly IMapper mapper;
        private readonly IUsersService usersService;
        private ICollection<WorkoutPlan> generatedWorkoutPlans;
        private WorkoutPlan workoutPlan;
        private WorkoutSplitFactory workoutSplitFactory = new WorkoutSplitFactory();

        private int sets;
        private int minReps;
        private int maxReps;
        public WorkoutsService(
            IDeletableEntityRepository<Exercise> exercises,
            IDeletableEntityRepository<WorkoutPlan> workoutPlansStorage,
            IWorkoutDaysService workoutDaysService,
            IExerciseInWorkoutDayService exerciseInWorkoutDayService,
            IMapper mapper,
            IUsersService usersService)
        {
            _exercises = exercises;
            this.workoutPlansStorage = workoutPlansStorage;

            this.workoutDaysService = workoutDaysService;
            this.exerciseInWorkoutDayService = exerciseInWorkoutDayService;
            this.mapper = mapper;
            this.usersService = usersService;
            this.generatedWorkoutPlans = new List<WorkoutPlan>();

            workoutSplit = new Dictionary<DayOfWeek, List<MuscleGroup>>();
            workoutPlan = new WorkoutPlan();
        }
        public ICollection<GeneratedWorkoutPlanDTO> GetGeneratedWorkoutPlans()
        {
            List<GeneratedWorkoutPlanDTO> plans = new List<GeneratedWorkoutPlanDTO>();


            foreach (var plan in generatedWorkoutPlans)
            {
                plans.Add(mapper.Map<GeneratedWorkoutPlanDTO>(plan));
            }

            return plans;
        }
        public async Task<string> SaveWorkoutPlanAsync(GeneratedWorkoutPlanDTO chosenWorkoutPlan)
        {
            var userId = chosenWorkoutPlan.UserId;
            var currWorkoutPlan = new WorkoutPlan(chosenWorkoutPlan.Id)
            {
                UserId = userId,
                Difficulty = chosenWorkoutPlan.Difficulty,
                Goal = chosenWorkoutPlan.Goal,
            };

            await workoutPlansStorage.AddAsync(currWorkoutPlan);
            await workoutPlansStorage.SaveChangesAsync();


            foreach (var workoutday in chosenWorkoutPlan.WorkoutDays)
            {
                var workoutDay = new WorkoutDayDTO()
                {
                    WorkoutPlanId = chosenWorkoutPlan.Id,
                    Day = workoutday.Day,
                };

                var workoutDayId = await workoutDaysService.AddAsync(workoutDay);

                foreach (var exerciseInWorkoutDay in workoutday.ExercisesInWorkoutDays)
                {
                    await exerciseInWorkoutDayService.AddAsync(new ExerciseInWorkoutDayDTO()
                    {
                        ExerciseId = exerciseInWorkoutDay.ExerciseId,
                        WorkoutDayId = workoutDayId,
                        MinReps = exerciseInWorkoutDay.MinReps,
                        MaxReps = exerciseInWorkoutDay.MaxReps,
                        Sets = exerciseInWorkoutDay.Sets
                    });
                }
            }

            await usersService.AssignTrainingProgramToUser(chosenWorkoutPlan.Id, userId);
            return chosenWorkoutPlan.Id;
        }
        private void LoadExerciseDefaultValues()
        {
            sets = WorkoutConstants.AvgExerciseSet;
            minReps = WorkoutConstants.AvgExerciseMinReps;
            maxReps = WorkoutConstants.AvgExerciseMaxReps;
        }



        public void GenerateWorkoutPlans(WorkoutGenerationInputModel inputModel)
        {
            for (int i = 0; i < inputModel.Count; i++)
            {
                workoutSplit = workoutSplitFactory.CreateSplits(inputModel.Days);
                workoutPlan.UserId = inputModel.UserId;
                workoutPlan.Goal = inputModel.Goal;
                workoutPlan.Difficulty = inputModel.Difficulty;

                foreach (var split in workoutSplit)
                {
                    CreateWorkoutDay(split, inputModel);
                }

                if (inputModel.Goal == Goal.LoseWeight)
                {
                    AddCardioToLeastBusyDay(workoutPlan.WorkoutDays, inputModel.Difficulty);
                }
                generatedWorkoutPlans.Add(workoutPlan);
                workoutPlan = new WorkoutPlan();
            }
        }

        private void AddCardioToLeastBusyDay(ICollection<WorkoutDay> workoutDays, Difficulty difficulty)
        {
            var cardioExercises = _exercises
                .AllAsNoTracking()
                .Where(x => x.Difficulty == difficulty && x.MuscleGroup == MuscleGroup.Cardio)
                .ToList();

            var randomCardio = cardioExercises.OrderBy(x => Guid.NewGuid()).Take(1).FirstOrDefault();

            var cardioExercise = new ExerciseInWorkoutDay()
            {
                ExerciseId = randomCardio.Id,
                Exercise = randomCardio,
                Sets = 1,
                MinReps = 20,
                MaxReps = 30,
            };

            workoutDays
                .OrderBy(x => x.ExercisesInWorkoutDays.Count)
                .FirstOrDefault()
                .ExercisesInWorkoutDays
                .Add(cardioExercise);
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
            List<ExerciseInWorkoutDay> firtMuscleGroupExercises = GetExercisesForSeparateMuscleForWorkoutDay(
                inputModel,
                muscles.Count,
                muscles[0]);
            List<ExerciseInWorkoutDay> secondMuscleGroupExercises = GetExercisesForSeparateMuscleForWorkoutDay(
                inputModel,
                muscles.Count,
                muscles[1]);
            List<ExerciseInWorkoutDay> thirdMuscleGroupExercises = GetExercisesForSeparateMuscleForWorkoutDay(
                inputModel,
                muscles.Count,
                muscles[2]);

            // Something like joining the lists
            firtMuscleGroupExercises.AddRange(secondMuscleGroupExercises);
            firtMuscleGroupExercises.AddRange(thirdMuscleGroupExercises);

            return firtMuscleGroupExercises;
        }

        private ICollection<ExerciseInWorkoutDay> CreateSplitOfTwoMusclesDay(
            WorkoutGenerationInputModel inputModel,
            List<MuscleGroup> muscles)
        {
            List<ExerciseInWorkoutDay> firtMuscleGroupExercises = GetExercisesForSeparateMuscleForWorkoutDay(
                inputModel,
                muscles.Count,
                muscles[0]);
            List<ExerciseInWorkoutDay> secondMuscleGroupExercises = GetExercisesForSeparateMuscleForWorkoutDay(
                inputModel,
                muscles.Count,
                muscles[1]);

            // Something like joining the lists
            firtMuscleGroupExercises.AddRange(secondMuscleGroupExercises);

            return firtMuscleGroupExercises;
        }

        private ICollection<ExerciseInWorkoutDay> CreateSeparateMuscleDay(
            WorkoutGenerationInputModel inputModel,
            List<MuscleGroup> muscles)
        {
            List<ExerciseInWorkoutDay> exercises = GetExercisesForSeparateMuscleForWorkoutDay(
                inputModel,
                muscles.Count,
                muscles[0]);

            return exercises;
        }


        /* We know that the minimum exercise count for separata muscle day is 5
        */
        private List<ExerciseInWorkoutDay> GetExercisesForSeparateMuscleForWorkoutDay(
            WorkoutGenerationInputModel inputModel,
            int totalMusclesForTheDay,
            MuscleGroup muscleGroup)
        {
            LoadExerciseDefaultValues();
            int offset = CalculateOffset(totalMusclesForTheDay);
            ConfigureExercise(inputModel);

            List<ExerciseInWorkoutDay> exercisesInWorkoutDay = new List<ExerciseInWorkoutDay>();

            return GenerateExercisesForCurrentWorkoutDay(muscleGroup, exercisesInWorkoutDay, inputModel, offset);
        }

        private int CalculateOffset(int totalMusclesForTheDay)
        {
            switch (totalMusclesForTheDay)
            {
                case 2:
                    return -1;
                case 3:
                    return -2;
                default:
                    return 0;
            }
        }

        private List<ExerciseInWorkoutDay> GenerateExercisesForCurrentWorkoutDay(
            MuscleGroup muscleGroup,
            List<ExerciseInWorkoutDay> exercisesInWorkoutDay,
            WorkoutGenerationInputModel inputModel,
            int offset = 0)
        {
            int exercisesCount = WorkoutConstants.AvgExercisePerMuscle + offset;

            List<Exercise> currBodyPartEasyExercises = _exercises
                .AllAsNoTracking()
                .Where(x => x.MuscleGroup == muscleGroup &&
                x.Difficulty == Difficulty.Easy)
                .ToList();

            List<Exercise> currBodyPartMediumExercises = _exercises
                .AllAsNoTracking()
                .Where(x => x.MuscleGroup == muscleGroup &&
                x.Difficulty == Difficulty.Medium)
                .ToList();

            List<Exercise> currBodyPartHardExercises = _exercises
                .AllAsNoTracking()
                .Where(x => x.MuscleGroup == muscleGroup &&
                x.Difficulty == Difficulty.Hard)
                .ToList();

            /* Easy program => 5 _exercises | Medium = 6 | Hard = 7
             * firstly create 3 easy and 2 medium for begginers then + 1 hard for intermediate & + 2 hard for advanced
             * Making medium and hard exercise repetitions less
             */
            int minRepsMediumExercise = minReps - 1;
            int maxRepsMediumExercise = maxReps - 1;
            int minRepsHardExercise = minReps - 2;
            int maxRepsHardExercise = maxReps - 2;

            // Adding the exercises from hardest to easiest
            if (inputModel.Difficulty == Difficulty.Hard)
            {
                exercisesInWorkoutDay
                    .AddRange(GetSpecificExercisesForWorkoutDay(
                        currBodyPartHardExercises, exercisesCount - 1,
                        sets, minRepsHardExercise, minRepsMediumExercise));
            }
            if (inputModel.Difficulty == Difficulty.Medium)
            {
                exercisesInWorkoutDay
                    .AddRange(GetSpecificExercisesForWorkoutDay(
                        currBodyPartMediumExercises, exercisesCount - 2,
                        sets, minRepsMediumExercise, maxRepsMediumExercise));
            }

            exercisesInWorkoutDay
                .AddRange(GetSpecificExercisesForWorkoutDay(
                    currBodyPartMediumExercises, exercisesCount - 1,
                    sets, minRepsMediumExercise, maxRepsMediumExercise));
            exercisesInWorkoutDay
                .AddRange(GetSpecificExercisesForWorkoutDay(
                    currBodyPartEasyExercises, exercisesCount,
                    sets, minReps, maxReps));

            return exercisesInWorkoutDay;
        }

        private void ConfigureExercise(WorkoutGenerationInputModel inputModel)
        {
            // More reps with lighter weights for losing weight
            // And less reps with more kgs for muscle gain

            sets = CalculateSetsByDifficulty(inputModel);

            if (inputModel.Goal == Goal.LoseWeight)
            {
                minReps = 12;
                maxReps = 16;
            }
            else if (inputModel.Goal == Goal.GainMuscle)
            {
                minReps = 6;
                maxReps = 10;
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
            List<ExerciseInWorkoutDay> exercisesResult = new List<ExerciseInWorkoutDay>();

            for (int i = 0; i < count; i++)
            {
                var current = exercises
                    .OrderBy(exercises => Guid.NewGuid())
                    .FirstOrDefault();

                // if we run out of exercises => prevent null reference error
                if (current == null)
                {
                    break;
                }

                exercisesResult.Add(new ExerciseInWorkoutDay
                {
                    ExerciseId = current.Id,
                    Exercise = current,
                    Sets = sets,
                    MinReps = minReps,
                    MaxReps = maxReps,
                });
                exercises.Remove(current);
            }

            return exercisesResult;
        }

        public GeneratedWorkoutPlanDTO GetUserWorkoutPlan(string userId, string planId)
        {
            var workoutPlan = workoutPlansStorage
                .All()
                .Where(x => x.Id == planId && x.UserId == userId)
                .ProjectTo<GeneratedWorkoutPlanDTO>(this.mapper.ConfigurationProvider)
                .FirstOrDefault();

            var activeId = usersService.GetActiveWorkoutPlanId(userId);

            if(activeId == planId)
                workoutPlan.IsActive = true;

            if(workoutPlan == null)
                throw new ArgumentException(ErrorMessages.TrainingProgramIsNotAssigned);

            return workoutPlan;
        }

        public ICollection<UserWorkoutPlanInAllUserPlansDTO> GetUserWorkoutPlans(string userId)
        {
            string activePlanId = usersService.GetActiveWorkoutPlanId(userId);

            //User don't have workout plans
            if(activePlanId == null)
            {
                return null;
            }

            var workoutPlans = workoutPlansStorage
                .All()
                .Where(x => x.UserId == userId)
                .ProjectTo<UserWorkoutPlanInAllUserPlansDTO>(this.mapper.ConfigurationProvider)
                .ToList();
             
            foreach (var plan in workoutPlans)
            {
                if(activePlanId == plan.Id)
                {
                    plan.IsActive = true;
                }
            }

            return workoutPlans;
        }
    }
}