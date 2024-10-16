import 'dart:io';
import 'dart:math';

class Point {
  int x;
  int y;
  int z;
  Point(this.x, this.y, this.z);
}

class Cuboid {
  Point start;
  Point end;
  int sign;

  Cuboid(this.start, this.end, [this.sign = 1]);

  BigInt get volume {
    final width = BigInt.from(end.x - start.x + 1);
    final height = BigInt.from(end.y - start.y + 1);
    final depth = BigInt.from(end.z - start.z + 1);

    return BigInt.from(sign) * width * height * depth;
  }

  Cuboid? intersection(Cuboid cuboid) {
    final intStart = Point(
      max(start.x, cuboid.start.x),
      max(start.y, cuboid.start.y),
      max(start.z, cuboid.start.z),
    );

    final intEnd = Point(
      min(end.x, cuboid.end.x),
      min(end.y, cuboid.end.y),
      min(end.z, cuboid.end.z),
    );

    final valid = intStart.x <= intEnd.x &&
        intStart.y <= intEnd.y &&
        intStart.z <= intEnd.z;

    if (valid) {
      final plus =
          (sign == -1 && cuboid.sign == -1) || (sign == -1 && cuboid.sign == 1);
      final intSign = plus ? 1 : -1;

      return Cuboid(intStart, intEnd, intSign);
    }

    return null;
  }

  @override
  String toString() {
    return "$volume "
        "(${start.x},${end.x}) (${start.y},${end.y}) (${start.z},${end.z})";
  }
}

class Step {
  int on;
  Cuboid cuboid;
  Step(this.on, this.cuboid);

  @override
  String toString() {
    return cuboid.toString();
  }
}

List<Step> parseInput(String data) {
  return data.trim().split("\n").map((line) {
    final numberGroup = r"(-?\d+)";
    final xRange = "x=$numberGroup..$numberGroup";
    final yRange = "y=$numberGroup..$numberGroup";
    final zRange = "z=$numberGroup..$numberGroup";

    final match = RegExp("(on|off) $xRange,$yRange,$zRange").firstMatch(line)!;
    final on = match.group(1) == "on" ? 1 : 0;
    final values =
        match.groups([2, 3, 4, 5, 6, 7]).map((str) => int.parse(str!)).toList();

    final cuboid = Cuboid(Point(values[0], values[2], values[4]),
        Point(values[1], values[3], values[5]), on == 0 ? -1 : 1);

    return Step(on, cuboid);
  }).toList();
}

// https://stackoverflow.com/questions/8477940/how-to-calculate-the-intersection-of-two-cuboids?rq=1
BigInt part2(List<Step> steps) {
  List<Cuboid> lst = [];

  for (var step in steps) {
    List<Cuboid> intersections = [];

    for (var cuboid in lst) {
      var intersection = cuboid.intersection(step.cuboid);

      if (intersection != null) {
        intersections.add(intersection);
      }
    }

    if (step.cuboid.sign > 0) {
      lst.add(step.cuboid);
    }
    lst.addAll(intersections);
  }

  var result = BigInt.from(0);

  for (var cuboid in lst) {
    result += cuboid.volume;
  }

  return result;
}

void run() async {
  final sampleInput = "./input.txt";
  final puzzleInput = "/Users/igor/Downloads/input.txt";
  final data = await File(puzzleInput).readAsString();
  final steps = parseInput(data);
  final result = part2(steps);
  print(result);
}
