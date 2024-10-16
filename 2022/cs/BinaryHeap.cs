
public class BinaryHeap<T>
{
    T[] Elements;
    public int Length = 0;
    Func<T, T, bool> Comparator;

    public BinaryHeap(int capacity, Func<T, T, bool> comparator)
    {
        Elements = new T[capacity];
        Comparator = comparator;
    }

    public void Add(T elem)
    {
        var index = Length;
        Elements[index] = elem;

        while (index > 0)
        {
            var nextIndex = index / 2;

            if (Comparator(Elements[index], Elements[nextIndex]))
            {
                var t = Elements[nextIndex];
                Elements[nextIndex] = Elements[index];
                Elements[index] = t;
            }
            else
            {
                break;
            }

            index = nextIndex;
        }

        Length++;
    }

    public T Pop()
    {
        var result = Elements[0];
        var index = 0;

        Elements[0] = Elements[Length - 1];

        while (true)
        {
            if (index * 2 >= Length)
                break;

            var nextIndex = index * 2;
            
            if (index * 2 + 1 < Length)
                if (Comparator(Elements[index * 2 + 1], Elements[index * 2]))
                    nextIndex = index * 2 + 1;

            if (Comparator(Elements[index], Elements[nextIndex]))
            {
                var t = Elements[nextIndex];
                Elements[nextIndex] = Elements[index];
                Elements[index] = t;
            }
            else
            {
                break;
            }

            index = nextIndex;
        }

        Length--;

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
