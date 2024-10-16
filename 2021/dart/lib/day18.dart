import 'dart:math';
import 'dart:io';
import 'package:cli/utils.dart';

abstract class Number {
  Number? parent;
  Number(this.parent);
  int magnitude();
  Number clone();
}

class Pair extends Number {
  Number left;
  Number right;

  Pair(Number? parent, this.left, this.right) : super(parent);

  static Pair create(Number? parent, Number left, Number right) {
    final pair = Pair(parent, left, right);
    left.parent = pair;
    right.parent = pair;
    return pair;
  }

  @override
  int magnitude() {
    return left.magnitude() * 3 + right.magnitude() * 2;
  }

  @override
  Pair clone() {
    return Pair.create(parent, left.clone(), right.clone());
  }

  @override
  String toString() {
    return "[${left.toString()},${right.toString()}]";
  }

  void replace(Number leftOrRight, Number number) {
    if (leftOrRight == left) {
      left = number;
    }
    if (leftOrRight == right) {
      right = number;
    }
  }

  Pair? _findExplode(Pair current, int level) {
    if (level == 4) {
      return current;
    }

    final left = current.left;
    final right = current.right;

    if (left is Pair) {
      final result = _findExplode(left, level + 1);

      if (result != null) {
        return result;
      }
    }

    if (right is Pair) {
      final result = _findExplode(right, level + 1);

      if (result != null) {
        return result;
      }
    }
  }

  Leaf? _findSplit(Number current) {
    if (current is Pair) {
      return _findSplit(current.left) ?? _findSplit(current.right);
    } else if (current is Leaf && current.value >= 10) {
      return current;
    } else {
      return null;
    }
  }

  Leaf? _findLeftLeaf(Number current) {
    bool isLeft;

    do {
      final parent = current.parent;

      if (parent == null) {
        return null;
      }

      isLeft = (parent as Pair).left == current;
      current = parent;
    } while (isLeft);

    current = (current as Pair).left;

    while (current is Pair) {
      current = current.right;
    }

    return current as Leaf;
  }

  Leaf? _findRightLeaf(Number current) {
    bool isRight;

    do {
      final parent = current.parent;

      if (parent == null) {
        return null;
      }

      isRight = (parent as Pair).right == current;
      current = parent;
    } while (isRight);

    current = (current as Pair).right;

    while (current is Pair) {
      current = current.left;
    }

    return current as Leaf;
  }

  void _performExpload(Pair exploadNumber) {
    final left = _findLeftLeaf(exploadNumber);
    final right = _findRightLeaf(exploadNumber);

    if (left != null) {
      left.value += (exploadNumber.left as Leaf).value;
    }

    if (right != null) {
      right.value += (exploadNumber.right as Leaf).value;
    }

    (exploadNumber.parent as Pair)
        .replace(exploadNumber, Leaf(exploadNumber.parent, 0));
  }

  void _performSplit(Leaf splitNumber) {
    final pair = Pair.create(
      splitNumber.parent,
      Leaf(null, (splitNumber.value * 0.5).floor()),
      Leaf(null, (splitNumber.value * 0.5).ceil()),
    );

    (splitNumber.parent as Pair).replace(splitNumber, pair);
  }

  bool _reduceOnce() {
    final exploadNumber = _findExplode(this, 0);

    if (exploadNumber != null) {
      _performExpload(exploadNumber);

      return true;
    }

    final splitNumber = _findSplit(this);

    if (splitNumber != null) {
      _performSplit(splitNumber);

      return true;
    }

    return false;
  }

  Pair reduce() {
    final clone = this.clone();

    while (clone._reduceOnce()) {}

    return clone;
  }

  Pair add(Pair x) {
    return Pair.create(null, this, x).reduce();
  }
}

class Leaf extends Number {
  int value;
  Leaf(Number? parent, this.value) : super(parent);

  @override
  int magnitude() {
    return value;
  }

  @override
  Leaf clone() {
    return Leaf(parent, value);
  }

  @override
  String toString() {
    return value.toString();
  }
}

class Parser {
  Either<String, String> splitInput(String str) {
    str = str.substring(1, str.length - 1);
    List<String> stack = [];
    int i;

    for (i = 0; i < str.length; i++) {
      if (str[i] == '[') {
        stack.add('[');
      }
      if (str[i] == ']') {
        stack.removeLast();
      }
      if (str[i] == ',' && stack.isEmpty) {
        break;
      }
    }

    return Either(str.substring(0, i), str.substring(i + 1));
  }

  Number parse(String str) {
    if (str.contains("[")) {
      final splitResult = splitInput(str);

      return Pair(
        null,
        parse(splitResult.left),
        parse(splitResult.right),
      );
    }

    return Leaf(null, int.parse(str));
  }

  void setParents(Number number, Number? parent) {
    number.parent = parent;

    if (number is Pair) {
      setParents(number.left, number);
      setParents(number.right, number);
    }
  }

  Pair parseNumber(String str) {
    final number = parse(str) as Pair;
    setParents(number, null);
    return number;
  }

  List<Pair> parseInput(String str) {
    return str.trim().split("\n").map((line) => parseNumber(line)).toList();
  }
}

class Solution {
  int part1(List<Pair> numbers) {
    return numbers.reduce((a, b) => a.add(b)).magnitude();
  }

  int part2(List<Pair> numbers) {
    var maxMagnitude = 0;

    for (var a in numbers) {
      for (var b in numbers) {
        if (a != b) {
          maxMagnitude = max(maxMagnitude, a.add(b).magnitude());
        }
      }
    }

    return maxMagnitude;
  }
}

final input = """
[[[0,[5,8]],[[1,7],[9,6]]],[[4,[1,2]],[[1,4],2]]]
[[[5,[2,8]],4],[5,[[9,9],0]]]
[6,[[[6,2],[5,6]],[[7,6],[4,7]]]]
[[[6,[0,7]],[0,9]],[4,[9,[9,0]]]]
[[[7,[6,4]],[3,[1,3]]],[[[5,5],1],9]]
[[6,[[7,3],[3,2]]],[[[3,8],[5,7]],4]]
[[[[5,4],[7,7]],8],[[8,3],8]]
[[9,3],[[9,9],[6,[4,9]]]]
[[2,[[7,7],7]],[[5,8],[[9,3],[0,2]]]]
[[[[5,2],5],[8,[3,7]]],[[5,[7,5]],[4,4]]]
""";

void run() async {
  final input = await File("/Users/igor/Downloads/input.txt").readAsString();
  final numbers = Parser().parseInput(input);
  final result = Solution().part2(numbers);
  print(result);
}
