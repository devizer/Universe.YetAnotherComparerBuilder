using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace Universe.YetAnotherComparerBuilder.Tests
{
    public class SmokeTests
    {

        [Test]
        public void Test_YAC()
        {
            var persons = GetSourcePersons();
            var demoComparer = PersonComparers.GetDemoComparer("Dr", 42);
            Array.Sort(persons, demoComparer);
            TestContext.WriteLine(string.Join(Environment.NewLine, (IEnumerable<PersonClass>)persons));
            
            Assert.AreEqual(42, persons[0].Age.Value);
            Assert.AreEqual("Dr", persons[1].Title);
        }

        [Test]
        public void Test_ManualComparer()
        {

            var persons = GetSourcePersons();
            Array.Sort(persons, PersonComparers.ManualPersonClassComparer);
            TestContext.WriteLine(string.Join(Environment.NewLine, (IEnumerable<PersonClass>)persons));
            
            Assert.AreEqual(42, persons[0].Age.Value);
            Assert.AreEqual("Dr", persons[1].Title);
        }

        [Test]
        public void Test_Linq()
        {

            var persons = GetSourcePersons();
            persons = persons.Order().ToArray();
            TestContext.WriteLine(string.Join(Environment.NewLine, (IEnumerable<PersonClass>)persons));
            
            Assert.AreEqual(42, persons[0].Age.Value);
            Assert.AreEqual("Dr", persons[1].Title);
        }

        private static PersonClass[] GetSourcePersons()
        {
            PersonClass
                p1 = new PersonClass("Mr", "Adam", 1),
                p2 = new PersonClass(null, "Ginger", 1),
                p3 = new PersonClass("Ms", "Lama", 1),
                p4 = new PersonClass("Dr", "Don", null),
                p5 = new PersonClass("Dr", "Zed", 42);

            PersonClass[] persons = new[] {p1, p2, p3, p4, p5};
            return persons;
        }
    }
}