using System;
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
    public class ComparerBuilder<TItem>
    {
        private sealed class FieldMeta
        {
            public Func<object, object, int> Comparision;
        }

        private readonly List<FieldMeta> _Comparers = new List<FieldMeta>();

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
            return new ItemComparer<TItem>(_Comparers, flavour);
        }

        public Comparison<TItem> GetComparison(OrderFlavour flavour = OrderFlavour.Default)
        {
            // ThreadSafe
            return new ItemComparer<TItem>(_Comparers, OrderFlavour.Default).Compare;
        }

        public ComparerBuilder<TItem> Compare<TField>(Func<TItem, TField> expression, IComparer<TField> comparer, OrderFlavour flavour = OrderFlavour.Default)
        {
            if (expression == null)
                throw new ArgumentNullException("expression");

            if (expression == null)
                throw new ArgumentNullException("expression");

            Func<object, object, int> fieldComparer = delegate(object x, object y)
            {
                var f = flavour; // stack faster then heap
                bool areNullsEarly = (f & OrderFlavour.NullGoesEarly) != 0;
                bool isBackward = (f & OrderFlavour.Backward) != 0;

                // 1. x & y are unconditionally NOT NULL and type of them is defenitely TItem
                // 2. The compiler replaces null comparisons with a call to HasValue for nullable types
                TField xField = expression((TItem)x);
                TField yField = expression((TItem)y);
                if (ReferenceEquals(xField, yField)) return 0;
                if (xField == null && yField == null) return 0;
                if (xField == null) return areNullsEarly ? -1 : 1;
                if (yField == null) return areNullsEarly ? 1 : -1;
                int ret = comparer.Compare(xField, yField);
                return isBackward ? -ret : ret;
            };

            _Comparers.Add(new FieldMeta()
            {
                Comparision = fieldComparer,
            });

            return this;
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
                if (ReferenceEquals(x, y)) return 0;
                if (x == null) return areNullsEarly ? -1 : 1;
                if (y == null) return areNullsEarly ? 1 : -1;

                foreach (var fieldMeta in _Fields)
                {
                    int ret = fieldMeta.Comparision(x, y);
                    if (ret != 0) return isBackward ? -ret : ret;
                }

                return 0;
            }
        }
    }

}