using System;
using System.Data;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Xml.Schema;
using System.Xml.XPath;

namespace Day05
{
    public class MapEntry
    {
        public long srcRangeStart;
        public long destRangeStart;
        public long length;

        public bool IsMapped(long value)
        {
            return value >= srcRangeStart && value <= srcRangeStart + length;
        }

        public long MapValue(long value)
        {
            if (IsMapped(value))
            {
                return value - srcRangeStart + destRangeStart;
            }

            return value;
        }
    }

    public class Map : List<MapEntry>
    {
        public long MapValue(long value)
        {
            foreach (var entry in this)
            {
                if (entry.IsMapped(value))
                {
                    return entry.MapValue(value);
                }
            }

            return value;
        }
    }

    public class Almanac
    {
        public long[] seeds;
        public Map seedToSoil;
        public Map soilToFertilizer;
        public Map fertilizerToWater;
        public Map waterToLight;
        public Map lightToTemperature;
        public Map temperatureToHumidity;
        public Map humidityToLocation;
    }

    public class Parser
    {
        public Map ParseMap(string[] lines)
        {
            var result = new Map();

            foreach (var line in lines.Skip(1))
            {
                var values = line
                    .Split(" ")
                    .Select(long.Parse)
                    .ToList();

                result.Add(new MapEntry()
                {
                    destRangeStart = values[0],
                    srcRangeStart = values[1],
                    length = values[2],
                });
            }

            return result;
        }

        public Almanac Parse(string[] lines, string content)
        {
            var blocks = content
                .Split("\n\n")
                .Select((block) => block.Split("\n").ToArray())
                .ToArray();

            var seeds = new Regex(@"seeds: (.*)")
                .Match(blocks[0][0])
                .Groups[1]
                .Value
                .Split(" ")
                .Select(value => long.Parse(value))
                .ToArray();

            return new Almanac()
            {
                seeds = seeds,
                seedToSoil = ParseMap(blocks[1]),
                soilToFertilizer = ParseMap(blocks[2]),
                fertilizerToWater = ParseMap(blocks[3]),
                waterToLight = ParseMap(blocks[4]),
                lightToTemperature = ParseMap(blocks[5]),
                temperatureToHumidity = ParseMap(blocks[6]),
                humidityToLocation = ParseMap(blocks[7]),
            };
        }
    }

    public class MapSeedToLocation
    {
        public static long Run(Almanac almanac, long seed)
        {
            var soil = almanac.seedToSoil.MapValue(seed);
            var fertilizer = almanac.soilToFertilizer.MapValue(soil);
            var water = almanac.fertilizerToWater.MapValue(fertilizer);
            var light = almanac.waterToLight.MapValue(water);
            var temperature = almanac.lightToTemperature.MapValue(light);
            var humidity = almanac.temperatureToHumidity.MapValue(temperature);
            var location = almanac.humidityToLocation.MapValue(humidity);

            return location;
        }
    }

    public class PartOne
    {
        public long Solve(Almanac almanac)
        {
            var locations = almanac.seeds
                .Select(seed => MapSeedToLocation.Run(almanac, seed))
                .ToArray();

            return locations.Min();
        }
    }

    public class PartTwo
    {
        public long Solve(Almanac almanac)
        {
            long result = long.MaxValue;
            long total = 0;

            for (var i = 0; i < almanac.seeds.Length; i += 2) {
                var start = almanac.seeds[i];
                var size = almanac.seeds[i + 1];

                for (var seed = start; seed < start + size; seed++) {
                    var location = MapSeedToLocation.Run(almanac, seed);

                    result = Math.Min(result, location);
                }

                total += size;

                Console.WriteLine($"  total: {total}, result: {result}, {i} of {almanac.seeds.Length / 2}");
            }

            return result;
        }
    }
}
