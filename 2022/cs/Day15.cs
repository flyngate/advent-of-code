using System.Text.RegularExpressions;
using System.Linq;

namespace Day15
{
    public struct Sensor
    {
        public int X;
        public int Y;
        public int BeaconX;
        public int BeaconY;
        public int Distance;
    }

    class Helpers
    {
        public static int Distance(int x0, int y0, int x1, int y1)
        {
            return Math.Abs(x0 - x1) + Math.Abs(y0 - y1);
        }
    }

    class Parser
    {
        public List<Sensor> Parse(string[] lines)
        {
            var result = new List<Sensor>();

            var regex = new Regex(
                @"Sensor at x=([-0-9]+), y=([-0-9]+): closest beacon is at x=([-0-9]+), y=([-0-9]+)"
            );

            foreach (var line in lines)
            {
                var matches = regex.Matches(line);
                var groups = matches.First().Groups;
                var sensorX = int.Parse(groups[1].Value);
                var sensorY = int.Parse(groups[2].Value);
                var beaconX = int.Parse(groups[3].Value);
                var beaconY = int.Parse(groups[4].Value);
                var distance = Helpers.Distance(sensorX, sensorY, beaconX, beaconY);

                result.Add(new Sensor()
                {
                    X = sensorX,
                    Y = sensorY,
                    BeaconX = beaconX,
                    BeaconY = beaconY,
                    Distance = distance
                });
            }

            return result;
        }
    }

    class PartOne
    {
        bool isBeacon(List<Sensor> sensors, int x, int y)
        {
            foreach (var sensor in sensors)
            {
                if (sensor.BeaconX == x && sensor.BeaconY == y)
                    return true;
            }

            return false;
        }

        bool isCovered(List<Sensor> sensors, int x, int y)
        {

            foreach (var sensor in sensors)
            {
                var isCovered = Helpers.Distance(
                    sensor.X,
                    sensor.Y,
                    x,
                    y
                ) <= sensor.Distance;

                if (isCovered)
                    return true;
            }

            return false;
        }

        public int Solve(List<Sensor> sensors)
        {
            var result = 0;
            // var y = 10;
            var y = 2000000;
            var startX = sensors
                .Select(sensor => Math.Min(sensor.X, sensor.BeaconX))
                .Min() * 2;
            var endX = sensors
                .Select(sensor => Math.Max(sensor.X, sensor.BeaconX))
                .Max() * 2;

            for (var x = startX; x <= endX; x++)
                if (!isBeacon(sensors, x, y) && isCovered(sensors, x, y))
                    result += 1;

            return result;
        }
    }

    public class PartTwo
    {
        Sensor? FindFirstSensor(List<Sensor> sensors, int x, int y)
        {
            foreach (var sensor in sensors)
                if (Helpers.Distance(sensor.X, sensor.Y, x, y) <= sensor.Distance)
                    return sensor;

            return null;
        }

        (int, int) FindLostBeacon(List<Sensor> sensors)
        {
            var maxCoordinateValue = 4000000;
            // var maxCoordinateValue = 20;

            for (var x = 0; x <= maxCoordinateValue; x++)
            {
                if (x % 100000 == 0)
                    Console.WriteLine(x);

                for (var y = 0; y <= maxCoordinateValue; y++)
                {
                    var sensor = FindFirstSensor(sensors, x, y);

                    if (sensor == null)
                        return (x, y);
                    else
                        y = sensor.Value.Y +
                            sensor.Value.Distance -
                            Math.Abs(sensor.Value.X - x);
                }
            }

            throw new Exception("beacon not found");
        }

        public long Solve(List<Sensor> sensors)
        {
            var (x, y) = FindLostBeacon(sensors);

            return x * 4000000L + y;
        }

    }
}
