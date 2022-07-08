using System;
using System.Reflection;
using Xunit.Sdk;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xunit.Sdk;

namespace FitnessApp.Tests.BeforeAfter
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class TestBeforeAfter : BeforeAfterTestAttribute
    {

        public override void Before(MethodInfo methodUnderTest)
        {
            Console.WriteLine("Green");
        }

        public override void After(MethodInfo methodUnderTest)
        {
            Console.WriteLine("Blue");
        }
    }
}
