using System;
using System.Collections.Generic;

namespace Universe
{
    [Flags]
    public enum FieldOrder : byte
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

        public ComparerBuilder<TItem> Compare<TField>(Func<TItem, TField> expression, FieldOrder flavour = FieldOrder.Default)
        {
            return Compare(expression, Comparer<TField>.Default, flavour);
        }

        private ComparerBuilder<TItem> Compare<TField>(Func<TItem, TField> expression, IComparer<TField> comparer, FieldOrder flavour = FieldOrder.Default)
        {

            Func<object, object, int> fieldComparer = delegate(object x, object y)
            {
                var f = flavour; // stack faster then heap
                bool areNullsEarly = (f & FieldOrder.NullGoesEarly) != 0;
                bool isBackward = (f & FieldOrder.Backward) != 0;

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

        public ComparerBuilder<TItem> CompareString(Func<TItem, string> expression, StringComparer comparer, FieldOrder flavour = FieldOrder.Default)
        {
            return Compare(expression, comparer, flavour);
        }

        public IComparer<TItem> GetComparer(FieldOrder flavour = FieldOrder.Default)
        {
            return new ItemComparer<TItem>(Columns, flavour);
        }

        // ThreadSafe
        public Comparison<TItem> Comparison(FieldOrder flavour = FieldOrder.Default)
        {
            return new ItemComparer<TItem>(Columns, FieldOrder.Default).Compare;
        }

        private sealed class ItemComparer<T> : IComparer<T>
        {
            private readonly List<FieldMeta> Fields;
            private readonly FieldOrder Order;

            public ItemComparer(List<FieldMeta> fields, FieldOrder order)
            {
                Fields = fields;
                Order = order;
            }

            public int Compare(T x, T y)
            {
                bool areNullsEarly = (Order & FieldOrder.NullGoesEarly) != 0;
                bool isBackward = (Order & FieldOrder.Backward) != 0;

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