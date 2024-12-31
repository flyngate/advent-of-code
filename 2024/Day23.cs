namespace AdventOfCode.Day23;

public class Solution
{
    public object PartOne(string input)
    {
        var map = Parse(input);
        var hosts = map.Keys.ToArray();
        var parties = new HashSet<string>(hosts);

        parties = Grow(map, parties);
        parties = Grow(map, parties);

        return parties.Count(p => p.Split(",").Any(m => m.StartsWith('t')));
    }

    public object PartTwo(string input)
    {
        var map = Parse(input);
        var hosts = map.Keys.ToArray();
        var parties = new HashSet<string>(hosts);

        while (parties.Count > 1)
            parties = Grow(map, parties);

        return parties.Single();
    }

    HashSet<string> Grow(Dictionary<string, List<string>> map, HashSet<string> parties)
    {
        return (
            from party in parties.AsParallel()
            let members = party.Split(",")
            let candidates = members.SelectMany(host => map[host]).Distinct()
            from host in candidates
            where CanJoin(map, members, host)
            select GetPassword(members.Append(host))
        ).ToHashSet();
    }

    string GetPassword(IEnumerable<string> party) =>
        string.Join(',', party.Order());

    bool CanJoin(Dictionary<string, List<string>> map, string[] members, string host)
    {
        if (members.Contains(host))
            return false;

        foreach (var member in members)
            if (!map[member].Contains(host) || !map[host].Contains(member))
                return false;

        return true;
    }

    Dictionary<string, List<string>> Parse(string input)
    {
        var connections = input
            .Split("\n")
            .Select(line => (line[0..2], line[3..]));

        Dictionary<string, List<string>> map = [];

        foreach (var (a, b) in connections)
        {
            map.TryAdd(a, []);
            map.TryAdd(b, []);
            map[a].Add(b);
            map[b].Add(a);
        }

        return map;
    }
}
