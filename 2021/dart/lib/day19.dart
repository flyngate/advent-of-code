import 'dart:io';
import 'dart:math';
import 'package:quiver/core.dart';

typedef Scanners = List<List<Vec>>;

class Vec {
  int x;
  int y;
  int z;
  bool scanner;

  Vec(this.x, this.y, this.z, {this.scanner = false});

  static Vec from(List<int> coords) => Vec(coords[0], coords[1], coords[2]);

  static Vec fromPoints(Vec start, Vec end) =>
      Vec(end.x - start.x, end.y - start.y, end.z - start.z);

  Vec rotate(int rotationIndex) {
    Vec _rotate() {
      switch (rotationIndex) {
        case 0:
          return Vec(x, y, z);
        case 1:
          return Vec(x, -z, y);
        case 2:
          return Vec(x, -y, -z);
        case 3:
          return Vec(x, z, -y);
        case 4:
          return Vec(-x, z, y);
        case 5:
          return Vec(-x, -y, z);
        case 6:
          return Vec(-x, -z, -y);
        case 7:
          return Vec(-x, y, -z);
        case 8:
          return Vec(y, z, x);
        case 9:
          return Vec(y, -x, z);
        case 10:
          return Vec(y, -z, -x);
        case 11:
          return Vec(y, x, -z);
        case 12:
          return Vec(-y, x, z);
        case 13:
          return Vec(-y, -z, x);
        case 14:
          return Vec(-y, -x, -z);
        case 15:
          return Vec(-y, z, -x);
        case 16:
          return Vec(z, x, y);
        case 17:
          return Vec(z, -y, x);
        case 18:
          return Vec(z, -x, -y);
        case 19:
          return Vec(z, y, -x);
        case 20:
          return Vec(-z, y, x);
        case 21:
          return Vec(-z, -x, y);
        case 22:
          return Vec(-z, -y, -x);
        case 23:
          return Vec(-z, x, -y);
        default:
          throw Error();
      }
    }

    var result = _rotate();
    result.scanner = scanner;

    return result;
  }

  @override
  bool operator ==(other) =>
      other is Vec && other.x == x && other.y == y && other.z == z;

  Vec operator -(other) => other is Vec
      ? Vec(x - other.x, y - other.y, z - other.z, scanner: scanner)
      : throw Error();

  @override
  int get hashCode => hash3(x, y, z);

  @override
  String toString() => "($x, $y, $z)";
}

class Parser {
  Scanners parse(String str) {
    final result = <List<Vec>>[];
    final lines = str.trim().split("\n").where((line) => line.isNotEmpty);
    var currentList = <Vec>[];

    for (var line in lines) {
      if (line.startsWith('---')) {
        if (currentList.isNotEmpty) {
          result.add(currentList);
          currentList = [];
        }
      } else {
        final point =
            Vec.from(line.split(",").map((coord) => int.parse(coord)).toList());
        currentList.add(point);
      }
    }

    result.add(currentList);

    return result;
  }
}

class Solution {
  List<Vec> rotateAll(Vec v) {
    final baseRotations = [
      Vec(v.x, v.y, v.z),
      Vec(v.y, v.z, v.x),
      Vec(v.z, v.x, v.y),
      Vec(-v.x, v.z, v.y),
      Vec(-v.y, v.x, v.z),
      Vec(-v.z, v.y, v.x),
    ];
    final rotations = <Vec>[];

    for (var rotation in baseRotations) {
      rotations.add(Vec(rotation.x, rotation.y, rotation.z));
      rotations.add(Vec(rotation.x, -rotation.z, rotation.y));
      rotations.add(Vec(rotation.x, -rotation.y, -rotation.z));
      rotations.add(Vec(rotation.x, rotation.z, -rotation.y));
    }

    return rotations;
  }

  List<Vec> rotate(List<Vec> lst, int rotationIndex) {
    return lst.map((v) => v.rotate(rotationIndex)).toList();
  }

  // List<Vec> rotate(List<Vec> lst, int rotationIndex) {
  //   return lst.map((v) => rotateAll(v)[rotationIndex]).toList();
  // }

  Set<Vec> shift(List<Vec> lst, Vec diff) {
    return lst.map((v) => v - diff).toSet();
  }

  Vec findMinVec(List<Vec> lst) {
    var result = lst[0];

    for (var v in lst) {
      final isLess = v.x < result.x ||
          (v.x == result.x && v.y < result.x) ||
          (v.x == result.x && v.y == result.y && v.z < result.z);

      if (isLess) {
        result = v;
      }
    }

    return result;
  }

  Vec lastBeacon = Vec(0, 0, 0);

  List<Vec>? tryMerge(List<Vec> a, List<Vec> b) {
    final aSet = a.toSet();

    for (var rotationIndex = 1; rotationIndex < 24; rotationIndex++) {
      final rotated = rotate(b, rotationIndex);

      for (var p1 in a) {
        for (var p2 in rotated) {
          var shifted = shift(rotated, p2 - p1);
          var intersection = aSet.intersection(shifted);

          if (intersection.length >= 12) {
            return aSet.union(shifted).toList();
          }
        }
      }
    }

    return null;
  }

  List<Vec> merge(List<Vec> a, List<Vec> b) {
    return (a + b).toSet().toList();
  }

  Scanners reduce(Scanners scanners) {
    scanners = scanners.toList();
    print(scanners.length);

    for (var i = 0; i < scanners.length; i++) {
      for (var j = i + 1; j < scanners.length; j++) {
        var merged = tryMerge(scanners[i], scanners[j]);

        if (merged != null) {
          scanners[i] = merged;
          scanners.removeAt(j);

          return scanners;
        }
      }
    }

    throw Exception("reduce failed.");
  }

  List<Vec> reduceToOne(Scanners scanners) {
    while (scanners.length > 1) {
      scanners = reduce(scanners);
    }

    return scanners[0];
  }

  int part1(Scanners scanners) {
    return reduceToOne(scanners).length;
  }

  int getDistance(Vec a, Vec b) {
    return (a.x - b.x).abs() + (a.y - b.y).abs() + (a.z - b.z).abs();
  }

  int part2(Scanners beaconsByScanner) {
    for (var beacons in beaconsByScanner) {
      beacons.add(Vec(0, 0, 0, scanner: true));
    }

    final beacons = reduceToOne(beaconsByScanner);
    final scannerBeacons = beacons.where((element) => element.scanner).toList();

    var maxDistance = 0;

    for (var i = 0; i < scannerBeacons.length; i++) {
      for (var j = i + 1; j < scannerBeacons.length; j++) {
        final distance = getDistance(scannerBeacons[i], scannerBeacons[j]);
        maxDistance = max(distance, maxDistance);
      }
    }

    return maxDistance;
  }
}

void run() async {
  // final sampleInput = "./input.txt";
  final puzzleInput = "/Users/igor/Downloads/input.txt";
  final data = await File(puzzleInput).readAsString();
  final scanners = Parser().parse(data);
  final result = Solution().part2(scanners);
  print(result);
}
