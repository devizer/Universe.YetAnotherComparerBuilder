using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Universe
{
    public class PersonClass
    {
        public string Title { get; set; }
        public string Name { get; set; }
        public int? Age { get; set; }

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
