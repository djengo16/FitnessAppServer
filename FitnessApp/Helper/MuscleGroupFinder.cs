namespace FitnessApp.Helper
{
    using FitnessApp.Models.Enums;

    public static class MuscleGroupFinder
    {
        public static int FindMuscleGroup(string keywords)
        {
            if (keywords == null)
                return -1;
            foreach (string name in Enum.GetNames(typeof(MuscleGroup)))
            {
                if (name.ToUpper().Contains(keywords.ToUpper()))
                {
                    return (int)Enum.Parse(typeof(MuscleGroup), name);
                }
            }
            return -1;
        }
    }
}
