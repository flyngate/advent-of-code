import 'dart:io';
import 'dart:math';

import 'package:cli/day23.dart';

final sampleInput = """v...>>.vv>
.vv>>.vv..
>>.>v>...v
>>v>>.>.v.
v>v.vv.v..
>.>>..v...
.vv..>.>v.
v.v..>>v.v
....v..v.>""";

Floor parseInput(String input) {
  return Floor(input.trim().split("\n").map((line) => line.split("")).toList());
}

List<List<String>> array2d(int n, int m, String fill) {
  return List.generate(n, (_) => List.filled(m, fill));
}

class Floor {
  List<List<String>> data;
  late int n;
  late int m;

  Floor(this.data) {
    n = data.length;
    m = data[0].length;
  }

  Floor moveCucumbers(
      String type, int Function(int) nextX, int Function(int) nextY) {
    final next = array2d(n, m, ".");

    for (var i = 0; i < n; i++) {
      for (var j = 0; j < m; j++) {
        if (data[i][j] == type) {
          final x = nextX(i);
          final y = nextY(j);

          if (data[x][y] == ".") {
            next[x][y] = type;
          } else {
            next[i][j] = type;
          }
        } else if (data[i][j] != '.') {
          next[i][j] = data[i][j];
        }
      }
    }

    return Floor(next);
  }

  Floor moveRight() {
    return moveCucumbers(">", (x) => x, (y) => (y + 1) % m);
  }

  Floor moveDown() {
    return moveCucumbers("v", (x) => (x + 1) % n, (y) => y);
  }

  Floor move() {
    var floor = moveRight();
    floor = floor.moveDown();
    return floor;
  }

  @override
  String toString() {
    return data.map((line) => line.join("")).join("\n");
  }

  @override
  bool operator ==(other) => other is Floor && other.toString() == toString();
}

void run() async {
  final puzzleInput =
      await File("/Users/igor/Downloads/input.txt").readAsString();

  Floor? prev;
  var floor = parseInput(puzzleInput);
  var result = 0;

  do {
    prev = floor;
    floor = floor.move();
    result++;
  } while (prev != floor);

  print(result);
}
