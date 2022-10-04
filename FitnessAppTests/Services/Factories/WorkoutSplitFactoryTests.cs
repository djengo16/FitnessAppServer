namespace FitnessApp.Tests.Services.Factories
{
    using FitnessApp.Models.Enums;
    using FitnessApp.Services.Factories;
    using FitnessApp.Services.ServiceConstants;

    public class WorkoutSplitFactoryTests
    {
        private WorkoutSplitFactory _factory;

        [SetUp]
        public void Setup()
        {
            _factory = new WorkoutSplitFactory();
        }

        [Test]
        public void WhenCreateSplitWithThreeDaysWillReturnDictionaryWithTrainingDays()
        {

            IDictionary<DayOfWeek, List<MuscleGroup>> expectedData = new Dictionary<DayOfWeek, List<MuscleGroup>>()
            {
                { DayOfWeek.Monday,
                new List<MuscleGroup>() { MuscleGroup.Chest, MuscleGroup.Shoulders, MuscleGroup.Triceps } },
                { DayOfWeek.Wednesday,
                new List<MuscleGroup>() { MuscleGroup.Back, MuscleGroup.Biceps }},
                { DayOfWeek.Friday,
                new List<MuscleGroup>() { MuscleGroup.Legs, MuscleGroup.Abs }}
            };

            var actualData = _factory.CreateSplits(3);

            Assert.That(actualData, Is.EqualTo(expectedData));
        }

        [Test]
        public void WhenCreateSplitWithFourDaysWillReturnDictionaryWithTrainingDays()
        {

            IDictionary<DayOfWeek, List<MuscleGroup>> expectedData = new Dictionary<DayOfWeek, List<MuscleGroup>>()
            {
                { DayOfWeek.Monday,
                new List<MuscleGroup>() { MuscleGroup.Back, MuscleGroup.Biceps } },
                { DayOfWeek.Tuesday,
                new List<MuscleGroup>() { MuscleGroup.Chest, MuscleGroup.Triceps }},
                { DayOfWeek.Thursday,
                new List<MuscleGroup>() { MuscleGroup.Legs }},
                { DayOfWeek.Friday,
                new List<MuscleGroup>() { MuscleGroup.Shoulders, MuscleGroup.Abs }}
            };

            var actualData = _factory.CreateSplits(4);

            Assert.That(actualData, Is.EqualTo(expectedData));
        }

        [Test]
        public void WhenCreateSplitWithFiveDaysWillReturnDictionaryWithTrainingDays()
        {

            IDictionary<DayOfWeek, List<MuscleGroup>> expectedData = new Dictionary<DayOfWeek, List<MuscleGroup>>()
            {
                { DayOfWeek.Monday,
                new List<MuscleGroup>() { MuscleGroup.Chest }},

                { DayOfWeek.Tuesday,
                new List<MuscleGroup>() { MuscleGroup.Back } },

            { DayOfWeek.Wednesday,
                new List<MuscleGroup>() { MuscleGroup.Shoulders }},

                { DayOfWeek.Thursday,
                new List<MuscleGroup>() { MuscleGroup.Biceps, MuscleGroup.Triceps } },

                { DayOfWeek.Friday,
                new List<MuscleGroup>() { MuscleGroup.Legs, MuscleGroup.Abs } },
            };

            var actualData = _factory.CreateSplits(5);

            Assert.That(actualData, Is.EqualTo(expectedData));
        }

        [TestCase(-1)]
        [TestCase(0)]
        [TestCase(8)]
        public void WhenInvalidDayIsEnteredWillThrowArgumentException(int value)
        {
            var variableException = Assert.Throws<ArgumentException>(() => _factory.CreateSplits(-1));

            Assert.That(variableException.Message.Contains(ErrorMessages.InvalidDaysOfWeek));
        }

        [Test]
        public void WhenDayIsNotInRangeWillThrowArgumentException()
        {
            var variableException = Assert.Throws<ArgumentException>(() => _factory.CreateSplits(1));

            Assert.That(variableException.Message.Contains(ErrorMessages.WorkoutPlanDaysNotInRange));
        }
    }
}