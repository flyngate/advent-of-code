import 'dart:io';
import 'dart:math';

class Point {
  int x;
  int y;
  int z;
  Point(this.x, this.y, this.z);
}

class Step {
  int on;
  Point start;
  Point end;
  Step(this.on, this.start, this.end);
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

    return Step(on, Point(values[0], values[2], values[4]),
        Point(values[1], values[3], values[5]));
  }).toList();
}

List<List<List<int>>> array3d(int n, int m, int k) {
  return List.generate(n, (_) => List.generate(m, (_) => List.filled(k, 0)));
}

class Cubes {
  final cubes = array3d(101, 101, 101);

  int get(int x, int y, int z) {
    try {
      return cubes[x + 50][y + 50][z + 50];
    } on RangeError {
      return 0;
    }
  }

  void set(int x, int y, int z, int value) {
    try {
      cubes[x + 50][y + 50][z + 50] = value;
    } catch (_) {}
  }
}

int part1(List<Step> steps) {
  final cubes = Cubes();
  var result = 0;

  for (var step in steps) {
    final x = [max(step.start.x, -50), min(step.end.x, 50)];
    final y = [max(step.start.y, -50), min(step.end.y, 50)];
    final z = [max(step.start.z, -50), min(step.end.z, 50)];

    for (var i = x[0]; i <= x[1]; i++) {
      for (var j = y[0]; j <= y[1]; j++) {
        for (var k = z[0]; k <= z[1]; k++) {
          if (cubes.get(i, j, k) == 1 && step.on == 0) {
            result -= 1;
          } else if (cubes.get(i, j, k) == 0 && step.on == 1) {
            result += 1;
          }
          cubes.set(i, j, k, step.on);
        }
      }
    }
  }

  return result;
}

void run() async {
  final sampleInput = "./input.txt";
  final puzzleInput = "/Users/igor/Downloads/input.txt";
  final data = await File(puzzleInput).readAsString();
  final steps = parseInput(data);
  final result = part1(steps);
  print(result);
}
