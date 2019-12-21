using System;
using System.Collections;
using System.Collections.Generic;

namespace Universe
{
    public class EqualityComparerBuilder<T>
    {
        private sealed class EqualityParts
        {
            public Func<T, object> Function;
            public IEqualityComparer EqualityComparer;
        }

        private sealed class AnEqualityComparer<TItem> : IEqualityComparer
        {
            private IEqualityComparer<TItem> EqualityComparer;

            public AnEqualityComparer(IEqualityComparer<TItem> equalityComparer)
            {
                EqualityComparer = equalityComparer;
            }

            public bool Equals(object x, object y)
            {
                return this.EqualityComparer.Equals((TItem)x, (TItem)y);
            }

            public int GetHashCode(object obj)
            {
                return this.EqualityComparer.GetHashCode((TItem)obj);
            }
        }

        private List<EqualityParts> Parts = new List<EqualityParts>();

        public EqualityComparerBuilder<T> Use<TField>(Func<T, TField> expression, IEqualityComparer<TField> equalityComparer)
        {
            Parts.Add(new EqualityParts()
            {
                Function = delegate(T arg) { return expression(arg); },
                EqualityComparer = new AnEqualityComparer<TField>(equalityComparer)
            });

            return this;
        }

        public IEqualityComparer<T> GetEqualityComparer()
        {
            return new GenericEqualityComparer(Parts);
        }

        private class GenericEqualityComparer : IEqualityComparer<T>
        {
            private List<EqualityParts> Parts;

            public GenericEqualityComparer(List<EqualityParts> parts)
            {
                Parts = parts;
            }

            public bool Equals(T x, T y)
            {
                foreach (var part in Parts)
                {
                    if (!part.EqualityComparer.Equals(part.Function(x), part.Function(y)))
                        return false;
                }

                return true;
            }

            public int GetHashCode(T arg)
            {
                int ret = 31;
                foreach (var part in Parts)
                {
                    ret = (ret * -1521134295) ^ part.EqualityComparer.GetHashCode(part.Function(arg));
                }

                return ret;
            }
        }
    }
}