import 'dart:io';
import 'dart:math';
import 'package:quiver/core.dart';
import 'package:collection/collection.dart';

final roomColumnByPod = {
  "A": 3,
  "B": 5,
  "C": 7,
  "D": 9,
};

final hallwayPoints = [
  Point(1, 1),
  Point(1, 2),
  Point(1, 4),
  Point(1, 6),
  Point(1, 8),
  Point(1, 10),
  Point(1, 11),
];

final pods = ["A", "B", "C", "D"];

final costPerPod = {
  "A": 1,
  "B": 10,
  "C": 100,
  "D": 1000,
};

final finalState = parseState("""
#############
#...........#
###A#B#C#D###
  #A#B#C#D#
  #A#B#C#D#
  #A#B#C#D#
  #########
""");

class Move {
  Point point;
  int distance;
  Move(this.point, this.distance);
}

class Point {
  int x;
  int y;
  late int _hash;

  Point(this.x, this.y) {
    _hash = hash2(x, y);
  }

  @override
  bool operator ==(other) => other is Point && other.x == x && other.y == y;

  @override
  int get hashCode => _hash;
}

class NextState {
  State state;
  int cost;
  NextState(this.state, this.cost);
}

State parseState(String input) => State(input
    .trim()
    .split("\n")
    .map((line) => line.trimRight().padRight(13))
    .toList());

class State {
  List<String> state;
  late int _hash;

  State(this.state) {
    _hash = toString().hashCode;
  }

  String get(Point p) => state[p.x][p.y];

  State set(Point p, String value) {
    final clone = state.toList();
    clone[p.x] = clone[p.x].replaceRange(p.y, p.y + 1, value);
    return State(clone);
  }

  Set<NextState> getNextStates() {
    return getPods().map(getNextStatesForPod).expand((state) => state).toSet();
  }

  List<NextState> getNextStatesForPod(Point podPoint) {
    final up = Point(podPoint.x - 1, podPoint.y);

    final pod = get(podPoint);
    var points = <Point>[];

    if (pods.contains(get(up))) {
      return <NextState>[];
    }

    // Rooms
    final roomColumn = roomColumnByPod[pod]!;

    for (var roomRow = 5; roomRow > 1; roomRow--) {
      final roomPoint = Point(roomRow, roomColumn);
      final roomValue = get(roomPoint);

      if (roomValue == '.') {
        points.add(roomPoint);
        break;
      }

      if (roomValue != pod) {
        break;
      }
    }

    // Hallways
    if (podPoint.x > 1) {
      points.addAll(hallwayPoints);
    }

    // Filtering
    points = points.where((point) {
      if (point.y == podPoint.y) {
        return false;
      }

      if (get(point) != '.') {
        return false;
      }

      final isBlocked = hallwayPoints.firstWhereOrNull((hallwayPoint) {
            if (get(hallwayPoint) != '.') {
              if (podPoint.y < point.y) {
                return podPoint.y < hallwayPoint.y && hallwayPoint.y < point.y;
              } else {
                return point.y < hallwayPoint.y && hallwayPoint.y < podPoint.y;
              }
            }
            return false;
          }) !=
          null;

      if (isBlocked) {
        return false;
      }

      return true;
    }).toList();

    return points.map((point) {
      final distance =
          (podPoint.y - point.y).abs() + (podPoint.x - 1) + (point.x - 1);
      final state = set(podPoint, ".").set(point, pod);
      final cost = distance * costPerPod[pod]!;

      return NextState(state, cost);
    }).toList();
  }

  List<Point> getPods() {
    final result = <Point>[];

    for (var i = 0; i < state.length; i++) {
      for (var j = 0; j < state[0].length; j++) {
        if (pods.contains(state[i][j])) {
          result.add(Point(i, j));
        }
      }
    }

    return result;
  }

  @override
  String toString() => state.join("\n");

  @override
  bool operator ==(other) => other is State && other._hash == _hash;

  // @override
  // bool operator ==(other) => other is State && other.toString() == toString();

  @override
  int get hashCode => _hash;
}

int dijkstra(initialState, finalState) {
  final costs = <State, int>{};
  final visited = <State>[];
  final queue = PriorityQueue<State>((a, b) => costs[a]!.compareTo(costs[b]!));

  costs[initialState] = 0;
  queue.add(initialState);

  while (queue.isNotEmpty) {
    final state = queue.removeFirst();
    final nextStates = state.getNextStates();

    for (var nextState in nextStates) {
      if (!costs.containsKey(nextState.state)) {
        costs[nextState.state] = 1000000000;
      }

      if (costs[nextState.state]! > costs[state]! + nextState.cost) {
        costs[nextState.state] = costs[state]! + nextState.cost;
      }

      if (nextState.state == finalState) {
        print("New min = ${costs[nextState.state]!}");
      }

      if (!visited.contains(nextState.state)) {
        queue.add(nextState.state);
      }
    }

    visited.add(state);

    while (queue.remove(state)) {}
  }

  return costs[finalState]!;
}

int part1(State initialState) {
  return dijkstra(initialState, finalState);
}

final sampleInput = parseState("""
#############
#...........#
###B#C#B#D###
  #D#C#B#A#
  #D#B#A#C#
  #A#D#C#A#
  #########
""");

final puzzleInput = parseState("""
#############
#...........#
###A#C#B#A###
  #D#C#B#A#
  #D#B#A#C#
  #D#D#B#C#
  #########
""");

void run() async {
  // final nextStates = test.getNextStates();
  // nextStates.forEach((nextState) {
  //   print(nextState.state);
  //   print(nextState.cost);
  //   print("\n");
  // });
  final result = part1(puzzleInput);
  print(result);
}
