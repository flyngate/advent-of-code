
namespace AdventOfCode
{
    public enum BinaryHeapType
    {
        MinHeap,
        MaxHeap
    }

    public class BinaryHeap<T>
    {
        T[] Elements;
        public int Length = 0;
        Func<T, T, bool> IsGreater;
        BinaryHeapType Type;

        public BinaryHeap(int capacity, Func<T, T, bool> isGreater, BinaryHeapType type = BinaryHeapType.MinHeap)
        {
            Elements = new T[capacity];
            IsGreater = isGreater;
            Type = type;
        }

        bool IsInOrder(T a, T b)
        {
            var greater = IsGreater(a, b);

            return Type == BinaryHeapType.MaxHeap ? greater : !greater;
        }

        public void Add(T elem)
        {
            var index = Length;
            Elements[index] = elem;

            while (index > 0)
            {
                var nextIndex = index / 2;

                if (IsInOrder(Elements[nextIndex], Elements[index]))
                    break;

                var t = Elements[nextIndex];
                Elements[nextIndex] = Elements[index];
                Elements[index] = t;

                index = nextIndex;
            }

            Length++;
        }

        public T Pop()
        {
            var result = Elements[0];
            var index = 0;

            Elements[0] = Elements[Length - 1];

            Length--;

            while (index * 2 + 1 < Length)
            {
                var nextIndex = index * 2 + 1;

                if (nextIndex + 1 < Length)
                    if (IsInOrder(Elements[nextIndex + 1], Elements[nextIndex]))
                        nextIndex++;

                if (IsInOrder(Elements[index], Elements[nextIndex]))
                    break;

                var t = Elements[nextIndex];
                Elements[nextIndex] = Elements[index];
                Elements[index] = t;

                index = nextIndex;
            }

            return result;
        }

        public T Top()
        {
            return Elements.First();
        }

        public bool Empty()
        {
            return Length == 0;
        }
    }
}
