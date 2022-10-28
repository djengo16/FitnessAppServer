namespace FitnessApp.Tests.Helper
{
    public static class RandomizationHelper
    {
        /// <summary>
        /// It is expected that the seeder has been executed,
        /// an data is added to the Database, so this method can work
        /// correctly.
        /// </summary>
        /// <returns></returns>
        public static string GetRandomUserEmail()
        {
            Random rnd = new Random();
            int randomNumber = rnd.Next(1, 30);
            return $"user{randomNumber}@sample.com";
        }
    }
}
