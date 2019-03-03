using System;
using System.Collections;
using System.Collections.Generic;

namespace Universe
{
    [Flags]
    public enum OrderFlavour : byte
    {
        Forward = 0,
        Backward = 1,
        NullGoesFinally = 0,
        NullGoesEarly = 2,
        Default = Forward | NullGoesFinally,
    }

    // Emit-free, fast, strongly typed, highly-customizable yet another ComparerBuilder
    public partial class ComparerBuilder<TItem>
    {
        private readonly List<FieldMeta> _Comparisons = new List<FieldMeta>();

        public ComparerBuilder<TItem> Compare<TField>(Func<TItem, TField> expression, OrderFlavour flavour = OrderFlavour.Default)
        {
            return Compare(expression, Comparer<TField>.Default, flavour);
        }

        public ComparerBuilder<TItem> CompareString(Func<TItem, string> expression, StringComparer comparer, OrderFlavour flavour = OrderFlavour.Default)
        {
            return Compare(expression, comparer, flavour);
        }

        public IComparer<TItem> GetComparer(OrderFlavour flavour = OrderFlavour.Default)
        {
            return new ItemComparer<TItem>(_Comparisons, flavour);
        }

        public Comparison<TItem> GetComparison(OrderFlavour flavour = OrderFlavour.Default)
        {
            // ThreadSafe
            return new ItemComparer<TItem>(_Comparisons, OrderFlavour.Default).Compare;
        }

        public ComparerBuilder<TItem> Compare<TField>(Func<TItem, TField> expression, IComparer<TField> comparer, OrderFlavour flavour = OrderFlavour.Default)
        {
            if (expression == null)
                throw new ArgumentNullException("expression");

            if (comparer == null)
                throw new ArgumentNullException("comparer");

            Func<object, object, int> fieldComparison = delegate(object x, object y)
            {
                var f = flavour; // stack is faster then heap
                bool areNullsEarly = (f & OrderFlavour.NullGoesEarly) != 0;
                bool isBackward = (f & OrderFlavour.Backward) != 0;

                // 1. x & y are unconditionally NOT NULL and type of them is defenitely TItem
                // 2. The compiler replaces null comparisons with a call to HasValue for nullable types
                TField xField = expression((TItem)x);
                TField yField = expression((TItem)y);
                if (TypeInfo<TField>.IsReferenceType && ReferenceEquals(xField, yField)) return 0;
                if (xField == null && yField == null) return 0;
                if (xField == null) return areNullsEarly ? -1 : 1;
                if (yField == null) return areNullsEarly ? 1 : -1;
                int ret = comparer.Compare(xField, yField);
                return isBackward ? -ret : ret;
            };

            _Comparisons.Add(new FieldMeta()
            {
                Comparison = fieldComparison,
            });

            return this;
        }

        private sealed class FieldMeta
        {
            public Func<object, object, int> Comparison;
        }

        private sealed class TypeInfo<T>
        {
            static readonly object Sync = new object();
            private static bool? _IsReferenceType;

            public static bool IsReferenceType
            {
                get
                {
                    if (!_IsReferenceType.HasValue)
                        lock(Sync)
                            if (!_IsReferenceType.HasValue)
                                _IsReferenceType = !typeof(ValueType).IsAssignableFrom(typeof(T));

                    return _IsReferenceType.Value;
                }
            }

        }

        private sealed class ItemComparer<T> : IComparer<T>
        {
            private readonly List<FieldMeta> _Fields;
            private readonly OrderFlavour _OrderFlavour;

            public ItemComparer(List<FieldMeta> fields, OrderFlavour orderFlavour)
            {
                _Fields = fields;
                _OrderFlavour = orderFlavour;
            }

            public int Compare(T x, T y)
            {
                bool areNullsEarly = (_OrderFlavour & OrderFlavour.NullGoesEarly) != 0;
                bool isBackward = (_OrderFlavour & OrderFlavour.Backward) != 0;

                if (x == null && y == null) return 0;
                if (TypeInfo<T>.IsReferenceType && ReferenceEquals(x, y)) return 0;
                if (x == null) return areNullsEarly ? -1 : 1;
                if (y == null) return areNullsEarly ? 1 : -1;

                foreach (var fieldMeta in _Fields)
                {
                    int ret = fieldMeta.Comparison(x, y);
                    if (ret != 0) return isBackward ? -ret : ret;
                }

                return 0;
            }
        }
    }


    // Warning. This class is just a p-o-c. And it couses short-term boxing
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

#if CSHARP_OLD
    public delegate TResult Func<T1, T2, TResult>(T1 arg1, T2 arg2);
    public delegate TField Func<TItem, TField>(TItem item);
#endif
}