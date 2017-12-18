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

        private readonly List<FieldMeta> Columns = new List<FieldMeta>();

        public ComparerBuilder<TItem> Compare<TField>(Func<TItem, TField> expression, OrderFlavour flavour = OrderFlavour.Default)
        {
            if (expression == null)
                throw new ArgumentNullException("expression");

            return Compare(expression, Comparer<TField>.Default, flavour);
        }

        private ComparerBuilder<TItem> Compare<TField>(Func<TItem, TField> expression, IComparer<TField> comparer, OrderFlavour flavour = OrderFlavour.Default)
        {

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

            Columns.Add(new FieldMeta()
            {
                Comparision = fieldComparer,
            });

            return this;
        }

        public ComparerBuilder<TItem> CompareString(Func<TItem, string> expression, StringComparer comparer, OrderFlavour flavour = OrderFlavour.Default)
        {
            if (expression == null)
                throw new ArgumentNullException("expression");

            if (comparer == null)
                throw new ArgumentNullException("comparer");

            return Compare(expression, comparer, flavour);
        }

        public IComparer<TItem> GetComparer(OrderFlavour flavour = OrderFlavour.Default)
        {
            return new ItemComparer<TItem>(Columns, flavour);
        }

        // ThreadSafe
        public Comparison<TItem> Comparison(OrderFlavour flavour = OrderFlavour.Default)
        {
            return new ItemComparer<TItem>(Columns, OrderFlavour.Default).Compare;
        }

        private sealed class ItemComparer<T> : IComparer<T>
        {
            private readonly List<FieldMeta> Fields;
            private readonly OrderFlavour _orderFlavour;

            public ItemComparer(List<FieldMeta> fields, OrderFlavour orderFlavour)
            {
                Fields = fields;
                _orderFlavour = orderFlavour;
            }

            public int Compare(T x, T y)
            {
                bool areNullsEarly = (_orderFlavour & OrderFlavour.NullGoesEarly) != 0;
                bool isBackward = (_orderFlavour & OrderFlavour.Backward) != 0;

                if (x == null && y == null) return 0;
                if (ReferenceEquals(x, y)) return 0;
                if (x == null) return areNullsEarly ? -1 : 1;
                if (y == null) return areNullsEarly ? 1 : -1;

                foreach (var fieldMeta in Fields)
                {
                    int ret = fieldMeta.Comparision(x, y);
                    if (ret != 0) return isBackward ? -ret : ret;
                }

                return 0;
            }
        }
    }

}