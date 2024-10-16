import 'dart:io';

class Grid {
  List<List<int>> data = [];

  Grid(int n, int m) {
    data = List.generate(n, (_) => List.filled(m, 0));
  }
}

class ParseResult {
  List<int> enhanceVector;
  List<List<int>> pixels;
  ParseResult(this.enhanceVector, this.pixels);
}

List<int> parsePixels(String str) {
  return str.split("").map((char) => char == "#" ? 1 : 0).toList();
}

ParseResult parseInput(String str) {
  final lines = str.trim().split("\n");
  final enhanceVector = parsePixels(lines[0]);
  final pixels = <List<int>>[];

  for (var line in lines.sublist(2)) {
    pixels.add(parsePixels(line));
  }

  return ParseResult(
    enhanceVector,
    pixels,
  );
}

var allOtherPixels = 0;

int getEnhanceIndex(List<List<int>> pixels, int x, int y) {
  final offsets = [
    [-1, -1],
    [-1, 0],
    [-1, 1],
    [0, -1],
    [0, 0],
    [0, 1],
    [1, -1],
    [1, 0],
    [1, 1],
  ];
  var binary = "";

  for (var offset in offsets) {
    final p = x + offset[0];
    final q = y + offset[1];

    try {
      binary += pixels[p][q].toString();
    } on RangeError {
      binary += allOtherPixels.toString();
    }

    // if (p >= 0 && q >= 0 && p < pixels.length && q < pixels[0].length) {
    //   binary += pixels[p][q]
    // }
  }

  return int.parse(binary, radix: 2);
}

typedef Matrix = List<List<int>>;

Matrix newMatrix(int n, int m, int value) {
  return List.generate(n, (index) => List.filled(m, value));
}

Matrix enhance(Matrix pixels, List<int> enhanceVector) {
  final nextPixels = newMatrix(pixels.length, pixels[0].length, 0);

  for (var i = 0; i < pixels.length; i++) {
    for (var j = 0; j < pixels[0].length; j++) {
      final enhanceIndex = getEnhanceIndex(pixels, i, j);
      nextPixels[i][j] = enhanceVector[enhanceIndex];
    }
  }

  return nextPixels;
}

Matrix expand(Matrix pixels, int gap) {
  final nextPixels = newMatrix(
      pixels.length + gap * 2, pixels[0].length + gap * 2, allOtherPixels);

  for (var i = 0; i < pixels.length; i++) {
    for (var j = 0; j < pixels[0].length; j++) {
      nextPixels[i + gap][j + gap] = pixels[i][j];
    }
  }

  return nextPixels;
}

String pixelsToString(Matrix pixels) {
  return pixels
      .map((line) => line.map((bit) => bit == 1 ? '#' : '.').join(""))
      .join("\n");
}

int part1(Matrix pixels, List<int> enhanceVector) {
  final steps = 50;

  for (var i = 0; i < steps; i++) {
    pixels = enhance(expand(pixels, 1), enhanceVector);
    allOtherPixels = 1 - allOtherPixels;
  }

  return pixels
      .map((line) => line.reduce((a, b) => a + b))
      .reduce((a, b) => a + b);
}

void run() async {
  // final sampleInput = "./input.txt";
  final puzzleInput = "/Users/igor/Downloads/input.txt";
  final data = await File(puzzleInput).readAsString();
  final parseResult = parseInput(data);
  final result = part1(parseResult.pixels, parseResult.enhanceVector);
  print(result);
}
