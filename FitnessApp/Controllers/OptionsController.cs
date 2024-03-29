﻿namespace FitnessApp.Controllers
{
    using FitnessApp.Models.Enums;
    using Microsoft.AspNetCore.Mvc;
    using System.Text;

    [ApiController]
    [Route("[controller]")]
    public class OptionsController : ControllerBase
    {
        [HttpGet("difficulty")]
        public List<KeyValuePair<int, string>> Difficulty()
        {
            var result = EnumToKeyValuePair<Difficulty>();
            return result;
        }

        [HttpGet("goal")]
        public List<KeyValuePair<int, string>> Goal()
        {
            var result = EnumToKeyValuePair<Goal>();
            return result;
        }

        [HttpGet("muscleGroup")]
        public List<KeyValuePair<int, string>> MuscleGroup()
        {
            var result = EnumToKeyValuePair<MuscleGroup>();
            return result;
        }
        
        /// <summary>
        /// We use this generic method to convert enum type to key value pair collection
        /// </summary>
        /// <typeparam name="T">Some of the enum types</typeparam>
        /// <returns>list of enum keyvaluepairs</returns>
        private List<KeyValuePair<int, string>> EnumToKeyValuePair<T>()
        {
            var enumType = typeof(T);

            return Enum.GetValues(enumType)
                                   .Cast<int>()
                                   .Where(x => x != 0)
                                   .Select(x => new KeyValuePair<int, string>
                                   (key: x, value: AddSpaceBeforeUpperCase(Enum.GetName(enumType, x))))
                                   .ToList();
        }

        //Example: LooseWeight -> return Loose Weight
        private string AddSpaceBeforeUpperCase(string word)
        {
            
            for (int i = 1; i < word.Length; i++)
            {
                if (Char.IsUpper(word[i]))
                {
                    word = word.Insert(i, " ");
                    i++;
                }
            }
            return word;
        }
    }
}
