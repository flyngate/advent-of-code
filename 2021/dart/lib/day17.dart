import 'dart:math';

class Vec2<T> {
  T x;
  T y;
  Vec2(this.x, this.y);
}

class TargetArea {
  int x1;
  int y1;
  int x2;
  int y2;
  TargetArea(this.x1, this.y1, this.x2, this.y2);
}

TargetArea parseInput(String str) {
  final regexp = RegExp(r"x=([-0-9]+)..([-0-9]+), y=([-0-9]+)..([-0-9]+)");
  final match = regexp.firstMatch(str)!;
  final values = [
    match.group(1)!,
    match.group(2)!,
    match.group(3)!,
    match.group(4)!,
  ].map((value) => int.parse(value)).toList();

  final x1 = values[0];
  final x2 = values[1];
  final y1 = values[2];
  final y2 = values[3];

  return TargetArea(x1, y2, x2, y1);
}

class SimulateResult {
  bool value;
  int maxHeight;
  SimulateResult(this.value, this.maxHeight);

  @override
  String toString() {
    return "($value, $maxHeight)";
  }
}

SimulateResult simulate(Vec2<int> vel, TargetArea targetArea) {
  int x = 0;
  int y = 0;
  int maxHeight = 0;

  while (y > targetArea.y2) {
    x += vel.x;
    y += vel.y;

    if (vel.x > 0) {
      vel.x -= 1;
    } else if (vel.x < 0) {
      vel.x += 1;
    }
    vel.y -= 1;

    if (y > maxHeight) {
      maxHeight = y;
    }

    final inTargetArea = targetArea.x1 <= x &&
        x <= targetArea.x2 &&
        targetArea.y2 <= y &&
        y <= targetArea.y1;

    if (inTargetArea) {
      return SimulateResult(true, maxHeight);
    }
  }

  return SimulateResult(false, maxHeight);
}

List<SimulateResult> findTrajectories(TargetArea targetArea) {
  final threshold = 1000;
  List<SimulateResult> results = [];

  for (var x = -threshold; x <= threshold; x++) {
    for (var y = -threshold; y <= threshold; y++) {
      final result = simulate(Vec2(x, y), targetArea);

      if (result.value) {
        results.add(result);
      }
    }
  }

  return results;
}

int part1(TargetArea targetArea) {
  return findTrajectories(targetArea)
      .map((result) => result.maxHeight)
      .reduce(max);
}

int part2(TargetArea targetArea) {
  return findTrajectories(targetArea).length;
}

void run() {
  // final targetArea = parseInput("target area: x=20..30, y=-10..-5");
  final targetArea = parseInput("target area: x=236..262, y=-78..-58");
  final result = part2(targetArea);
  print(result);
}
