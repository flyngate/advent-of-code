import 'dart:io';
import 'dart:math';

enum Type {
  sum,
  product,
  minimum,
  maximum,
  literal,
  greaterThan,
  lessThan,
  equalTo,
}

class Packet {
  Type type;
  int version;
  int value;
  List<Packet> children;

  Packet(this.type, this.version, this.value, this.children);
}

class ParseResult<T> {
  T value;
  int length;
  String rest;

  ParseResult(this.value, this.length, this.rest);
}

String hexToBinary(String str) {
  return str
      .split("")
      .map((digit) =>
          int.parse(digit, radix: 16).toRadixString(2).padLeft(4, "0"))
      .join("");
}

int binaryToInt(String str) {
  return int.parse(str, radix: 2);
}

class Parser {
  int getVersion(String str) {
    return binaryToInt(str.substring(0, 3));
  }

  Type getType(String str) {
    final typeValue = binaryToInt(str.substring(3, 6));

    switch (typeValue) {
      case 0:
        return Type.sum;
      case 1:
        return Type.product;
      case 2:
        return Type.minimum;
      case 3:
        return Type.maximum;
      case 4:
        return Type.literal;
      case 5:
        return Type.greaterThan;
      case 6:
        return Type.lessThan;
      case 7:
        return Type.equalTo;
      default:
        return Type.literal;
    }
  }

  ParseResult<int> parseLiteral(String str) {
    String prefix;
    var i = 6;
    var valueStr = "";

    do {
      prefix = str[i];
      valueStr += str.substring(i + 1, i + 5);
      i += 5;
    } while (prefix == '1');

    return ParseResult(binaryToInt(valueStr), i, str.substring(i));
  }

  int getOperatorValue(Type type, List<Packet> packets) {
    final values = packets.map((packet) => packet.value).toList();

    switch (type) {
      case Type.sum:
        return values.reduce((a, b) => a + b);
      case Type.product:
        return values.reduce((a, b) => a * b);
      case Type.minimum:
        return values.reduce(min);
      case Type.maximum:
        return values.reduce(max);
      case Type.greaterThan:
        return values[0] > values[1] ? 1 : 0;
      case Type.lessThan:
        return values[0] < values[1] ? 1 : 0;
      case Type.equalTo:
        return values[0] == values[1] ? 1 : 0;
      default:
        return 0;
    }
  }

  ParseResult<List<Packet>> parseOperator(String str) {
    final lengthTypeId = str[6];

    if (lengthTypeId == '0') {
      final totalLength = int.parse(str.substring(7, 22), radix: 2);
      var currentStr = str.substring(22);
      var length = 0;
      List<Packet> packets = [];

      while (length < totalLength) {
        final result = parsePacket(currentStr);
        length += result.length;
        packets.add(result.value);
        currentStr = result.rest;
      }

      return ParseResult(packets, 22 + totalLength, currentStr);
    } else {
      final count = int.parse(str.substring(7, 18), radix: 2);
      var totalLength = 0;
      var currentStr = str.substring(18);
      List<Packet> packets = [];

      for (var i = 0; i < count; i++) {
        final result = parsePacket(currentStr);
        packets.add(result.value);
        totalLength += result.length;
        currentStr = result.rest;
      }

      return ParseResult(packets, 18 + totalLength, currentStr);
    }
  }

  ParseResult<Packet> parsePacket(String str) {
    final version = getVersion(str);
    final type = getType(str);

    if (type == Type.literal) {
      final result = parseLiteral(str);

      return ParseResult(
        Packet(type, version, result.value, []),
        result.length,
        result.rest,
      );
    }

    final result = parseOperator(str);
    final value = getOperatorValue(type, result.value);

    return ParseResult(
      Packet(type, version, value, result.value),
      result.length,
      result.rest,
    );
  }

  Packet parse(String str) {
    return parsePacket(str).value;
  }
}

typedef TraverseCallback = void Function(Packet p);

void traverse(Packet packet, TraverseCallback cb) {
  cb(packet);

  for (var child in packet.children) {
    traverse(child, cb);
  }
}

int part1(String str) {
  final packet = Parser().parse(hexToBinary(str));
  List<int> versions = [];

  traverse(packet, (p) => versions.add(p.version));

  return versions.reduce((a, b) => a + b);
}

int part2(String str) {
  return Parser().parse(hexToBinary(str)).value;
}

void runWithInput(path) async {
  final file = File(path);
  final input = (await file.readAsString()).trim();

  final result = part2(input);
  print(result);
}

void runTests() {
  var tests = [
    "C200B40A82",
    "04005AC33890",
    "880086C3E88112",
    "CE00C43D881120",
    "D8005AC2A8F0",
    "F600BC2D8F",
    "9C005AC2F8F0",
    "9C0141080250320F1802104A08",
  ];

  for (var test in tests) {
    print(part2(test));
  }
}

void run() {
  runWithInput("/Users/igor/Downloads/input.txt");
}
