using Entry = (int Id, int Span);

namespace AdventOfCode.Day09;

public class Solution
{
    public object PartOne(string input)
    {
        var disk = new LinkedList<Entry>(
            from i in Enumerable.Range(0, input.Length)
            let id = i % 2 == 0 ? i / 2 : -1
            let span = int.Parse($"{input[i]}")
            where i % 2 == 0 || span != 0
            from j in Enumerable.Range(0, span)
            select (Id: id, Span: 1)
        );

        Defrag(disk);

        return Checksum(disk);
    }

    public object PartTwo(string input) {
        var disk = new LinkedList<Entry>(
            from i in Enumerable.Range(0, input.Length)
            let id = i % 2 == 0 ? i / 2 : -1
            let span = int.Parse($"{input[i]}")
            where i % 2 == 0 || span != 0
            select (Id: id, Span: span)
        );

        Defrag(disk);

        return Checksum(disk);
    }

    void Defrag(LinkedList<Entry> disk)
    {
        for (var back = disk.Last; back != null; back = back.Previous)
        {
            if (back.Value.Id == -1)
                continue;

            for (var front = disk.First; front != null; front = front.Next)
            {
                if (back == front)
                    break;

                if (front.Value.Id == -1 && front.Value.Span >= back.Value.Span)
                {
                    var backValue = back.Value;
                    var extraSpan = front.Value.Span - back.Value.Span;

                    back.Value = (Id: -1, backValue.Span);
                    front.Value = backValue;

                    if (back.Next != null && back.Next.Value.Id == -1)
                    {
                        back.Value = (
                            Id: -1,
                            Span: back.Value.Span + back.Next.Value.Span
                        );
                        disk.Remove(back.Next);
                    }

                    if (back.Previous != null && back.Previous.Value.Id == -1)
                    {
                        var prev = back.Previous;
                        prev.Value = (
                            Id: -1,
                            Span: back.Value.Span + back.Previous.Value.Span
                        );
                        disk.Remove(back);
                        back = prev;
                    }

                    if (extraSpan > 0)
                        disk.AddAfter(front, (-1, extraSpan));
                    
                    break;
                }
            }
        }
    }

    long Checksum(IEnumerable<Entry> disk)
    {
        long r = 0;
        int index = 0;

        foreach (var entry in disk)
        {
            if (entry.Id == -1)
                index += entry.Span;
            else
                for (int k = 0; k < entry.Span; k++, index++)
                    r += entry.Id * index;
        }

        return r;
    }
}