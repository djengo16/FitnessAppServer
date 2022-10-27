namespace FitnessApp.Tests.Services.Data
{
    using AutoMapper;

    using FitnessApp.AutoMapperProfiles;
    using FitnessApp.Dto.Exercises;
    using FitnessApp.Models;
    using FitnessApp.Models.Enums;
    using FitnessApp.Models.Repositories;
    using FitnessApp.Services.Data;
    using Moq;

    public class ExerciseServiceTests
    {
        private Mock<IDeletableEntityRepository<Exercise>> _mockExercisesStorage;
        private IMapper _mapper;
        private Mock<IExerciseInWorkoutDayService> _mockExerciseInWorkoutDayService;

        private IExercisesService _mockExerciseService;

        // Instead of real database
        private List<Exercise> exercises;

        [SetUp]
        public void Setup()
        {
            exercises = new List<Exercise>();
            //mock exercise repo
            _mockExercisesStorage = new Mock<IDeletableEntityRepository<Exercise>>();
            _mockExercisesStorage.Setup(x => x.All()).Returns(exercises.AsQueryable());
            _mockExercisesStorage.Setup(x => x.AddAsync(It.IsAny<Exercise>())).Callback(
                (Exercise exercise) => exercises.Add(exercise));

            //configure mapper
            var exerciseProfile = new ExerciseProfiles();
            var configuration = new MapperConfiguration(cfg => cfg.AddProfile(exerciseProfile));
            _mapper = new Mapper(configuration);


            //mock exercise in workout day service
            _mockExerciseInWorkoutDayService = new Mock<IExerciseInWorkoutDayService>();


            _mockExerciseService = new ExercisesService(_mockExercisesStorage.Object, _mapper, _mockExerciseInWorkoutDayService.Object);
        }

        [Test]
        public void CreateAsyncMethodWillCreateExercise()
        {
            ExerciseInputDTO exerciseInputDTO = new ExerciseInputDTO()
            {
                Name = "Test Exercise",
                PictureResourceUrl = "Test picture",
                VideoResourceUrl = "Test video",
                Difficulty = "Easy",
                MuscleGroup = "Chest",
                Description = "Some description..",
            };

            _mockExerciseService.CreateAsync(exerciseInputDTO);

            var expected = new Exercise()
            {
                Name = "Test Exercise",
                PictureResourceUrl = "Test picture",
                VideoResourceUrl = "Test video",
                Difficulty = Difficulty.Easy,
                MuscleGroup = MuscleGroup.Chest,
                Description = "Some description..",
            };

            var actual = exercises[0];

            Assert.True(exercises.Any());
            Assert.That(actual.Name, Is.EqualTo(expected.Name));
            Assert.That(actual.PictureResourceUrl, Is.EqualTo(expected.PictureResourceUrl));
            Assert.That(actual.VideoResourceUrl, Is.EqualTo(expected.VideoResourceUrl));
            Assert.That(actual.Difficulty, Is.EqualTo(expected.Difficulty));
            Assert.That(actual.MuscleGroup, Is.EqualTo(expected.MuscleGroup));
            Assert.That(actual.Description, Is.EqualTo(expected.Description));
        }

        [Test]
        public void UpdateAsyncMethodWillUpdateExercise()
        {
            ExerciseInputDTO exerciseInputDTO = new ExerciseInputDTO()
            {
                Name = "Test Exercise",
                PictureResourceUrl = "Test picture",
                VideoResourceUrl = "Test video",
                Difficulty = "Easy",
                MuscleGroup = "Chest",
                Description = "Some description..",
            };

            _mockExerciseService.CreateAsync(exerciseInputDTO);

            ExerciseUpdateDTO updateExerciseDTO = new ExerciseUpdateDTO()
            {
                Id = 0,
                Name = "Updated Exercise",
                PictureResourceUrl = "Updated picture",
                VideoResourceUrl = "Updated video",
                Difficulty = "Medium",
                MuscleGroup = "Back",
                Description = "Updated description..",
            };

            var actual = exercises[0];

            _mockExerciseService.UpdateAsync(updateExerciseDTO);


            Assert.That(actual.Name, Is.EqualTo(updateExerciseDTO.Name));
            Assert.That(actual.PictureResourceUrl, Is.EqualTo(updateExerciseDTO.PictureResourceUrl));
            Assert.That(actual.VideoResourceUrl, Is.EqualTo(updateExerciseDTO.VideoResourceUrl));
            Assert.That(actual.Difficulty, Is.EqualTo(Difficulty.Medium));
            Assert.That(actual.MuscleGroup, Is.EqualTo(MuscleGroup.Back));
            Assert.That(actual.Description, Is.EqualTo(updateExerciseDTO.Description));

        }

        [TestCase(5)]
        [TestCase(10)]
        [TestCase(3)]
        public void GetExercisesMethodWillReturnRequestedExercisesByCount(
           int count)
        {
            LoadExercisesToTest();

            var firstCountExercises = _mockExerciseService
                .GetExercises("", count, 0, Difficulty.Default, MuscleGroup.Default);
            var nextCountExercises = _mockExerciseService
                .GetExercises("", count, count, Difficulty.Default, MuscleGroup.Default);

            // Last exercise on the first page should be equal to..
            Assert.That(firstCountExercises.Exercises.Last().Name == $"Exercise {count}");
            Assert.That(firstCountExercises.Exercises.Count(), Is.EqualTo(count));

            // First exercise on the second page should be equal to..
            Assert.That(nextCountExercises.Exercises.First().Name == $"Exercise {count + 1}");
            Assert.That(nextCountExercises.Exercises.Count(), Is.EqualTo(count));

            //Total
            Assert.That(exercises.Count == 40);
        }

        [TestCase("New", MuscleGroup.Chest, Difficulty.Medium)]
        public void GetExercisesMethodWillReturnCorrectDataWhenFitlerAndSearchParamsAreGiven(
            string searchParams, MuscleGroup muscleGroup, Difficulty difficulty)
        {
            LoadExercisesToTest();

            int newEntriesCount = 5;
            int dataPerPage = 10;

            //add new entries, so we can filter them
            for (int i = 0; i < newEntriesCount; i++)
            {
                _mockExerciseService.CreateAsync(new ExerciseInputDTO()
                {
                    Name = $"New exercise {i}",
                    PictureResourceUrl = "Test picture",
                    VideoResourceUrl = "Test video",
                    Difficulty = "Medium",
                    MuscleGroup = "Chest",
                    Description = "Some description..",
                });
            }

            var filteredExercises = _mockExerciseService.GetExercises(searchParams, dataPerPage, 0, difficulty, muscleGroup);


            //Should have 5 entries wich match the search params and filter, no matter dataPerPage count
            Assert.IsTrue(filteredExercises.Exercises.Count() == newEntriesCount);

            //check if any of them are not matching the filtered musclegroup
            Assert.IsFalse(filteredExercises.Exercises.Any(x => x.MuscleGroup != muscleGroup.ToString()));

            //check if any of them are not matching the filtered difficulty
            Assert.IsFalse(filteredExercises.Exercises.Any(x => x.Difficulty != difficulty.ToString()));

            //check if any them are not matching the searchParams
            Assert.IsTrue(filteredExercises.Exercises.All(x => x.Name.Contains(searchParams)));
        }


        /// <summary>
        /// Creates 40 different exercises,combination of muscle groups and difficuties.
        /// Four types of difficulty per muscle group.
        /// </summary>
        private void LoadExercisesToTest()
        {
            int counter = 1;

            foreach (var currentMuscleGroup in Enum.GetValues(typeof(MuscleGroup)))
            {
                foreach (var currDifficulty in Enum.GetValues(typeof(Difficulty)))
                {
                    _mockExerciseService.CreateAsync(new ExerciseInputDTO()
                    {
                        Name = $"Exercise {counter++}",
                        PictureResourceUrl = "Test picture",
                        VideoResourceUrl = "Test video",
                        Difficulty = currDifficulty.ToString(),
                        MuscleGroup = currentMuscleGroup.ToString(),
                        Description = "Some description..",
                    });
                }
            }
        }
    }
}
