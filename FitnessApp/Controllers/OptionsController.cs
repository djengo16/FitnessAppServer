namespace FitnessApp.Controllers
{
    using FitnessApp.Models.Enums;
    using Microsoft.AspNetCore.Mvc;

    [ApiController]
    [Route("[controller]")]
    public class OptionsController : ControllerBase
    {
        [HttpGet("difficulty")]
        public List<KeyValuePair<int, string>> Difficulty()
        {
            var result = enumToKeyValuePair<Difficulty>();
            return result;
        }

        [HttpGet("goal")]
        public List<KeyValuePair<int, string>> Goal()
        {
            var result = enumToKeyValuePair<Goal>();
            return result;
        }

        [HttpGet("muscleGroup")]
        public List<KeyValuePair<int, string>> MuscleGroup()
        {
            var result = enumToKeyValuePair<MuscleGroup>();
            return result;
        }
        
        /// <summary>
        /// We use this generic method to convert enum type to key value pair collection
        /// </summary>
        /// <typeparam name="T">Some of the enum types</typeparam>
        /// <returns>list of enum keyvaluepairs</returns>
        private List<KeyValuePair<int, string>> enumToKeyValuePair<T>()
        {
            var enumType = typeof(T);

            return Enum.GetValues(enumType)
                                   .Cast<int>()
                                   .Select(x => new KeyValuePair<int, string>(key: x, value: Enum.GetName(enumType, x)))
                                   .ToList();
        }
    }
}
