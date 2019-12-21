using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Universe
{
    public class PersonClass
    {
        public string Title;
        public string Name;
        public int? Age;

        public PersonClass(string title, string name, int? age = null)
        {
            Title = title;
            Name = name;
            Age = age;
        }

        public override string ToString()
        {
            return $"{nameof(Title)}: {Title ?? "<null>"}, {nameof(Name)}: {Name}, {nameof(Age)}: {Age}";
        }

    }
}
