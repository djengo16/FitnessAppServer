using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitnessApp.Tests.Helper
{
    public static class RandomizationHelper
    {
        public static string GetRandomUserEmail()
        {
            Random rnd = new Random();
            int randomNumber = rnd.Next(1, 30);
            return $"user{randomNumber}@sample.com";
        }
    }
}
