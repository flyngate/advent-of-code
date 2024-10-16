import Foundation

typealias Input = [[Int]]
typealias Result = Int

class Parser {
    func parse(contents: String) -> Input {
        var result: Input = [];
        let lines = contents.trimmingCharacters(in: .whitespacesAndNewlines).components(separatedBy: "\n");

        for line in lines {
            result.append(Array(line).map { Int(String($0))! })
        }

        return result
    }
}

class Solution {
    func walk(_ input: Input, _ visible: inout [[Bool]], _ start: (Int, Int), _ step: (Int, Int)) {
        let n = input[0].count;
        var (x, y) = start;
        var max = -1;

        while x >= 0 && y >= 0 && x < n && y < n {
            if input[x][y] > max {
                max = input[x][y]
                visible[x][y] = true
            }

            x += step.0;
            y += step.1;
        }
    }

    func part1(_ input: Input) -> Result {
        let n = input[0].count;
        var visible = Array(repeating: Array(repeating: false, count: n), count: n)

        for i in 0..<n {
            walk(input, &visible, (0, i), (1, 0))
            walk(input, &visible, (i, 0), (0, 1))
            walk(input, &visible, (n - 1, i), (-1, 0))
            walk(input, &visible, (i, n - 1), (0, -1))
        }

        var result = 0;

        for i in 0..<n {
            for j in 0..<n {
                if visible[i][j] {
                    result += 1
                }
            }
        }

        return result;
    }

    func viewingDistance(_ input: Input, _ height: Int, _ start: (Int, Int), _ step: (Int, Int)) -> Int {
        let n = input[0].count;
        var result = 0
        var (x, y) = start

        x += step.0
        y += step.1

        while x >= 0 && y >= 0 && x < n && y < n {
            result += 1

            if input[x][y] >= height {
                break
            }

            x += step.0
            y += step.1
        }

        return result
    }

    func scenicScore(_ input: Input, _ x: Int, _ y: Int) -> Int {
        let height = input[x][y];

        return viewingDistance(input, height, (x, y), (1, 0)) *
            viewingDistance(input, height, (x, y), (-1, 0)) *
            viewingDistance(input, height, (x, y), (0, 1)) *
            viewingDistance(input, height, (x, y), (0, -1))
    }

    func part2(_ input: Input) -> Result {
        let n = input[0].count;
        var result = 0;

        for i in 0..<n {
            for j in 0..<n {
                let score = scenicScore(input, i, j)
                result = max(result, score)
            }
        }

        return result
    }
}

let contents = try! String(contentsOfFile: "/Users/igor/Downloads/input.txt");
let input = Parser().parse(contents: contents);
let result = Solution().part2(input);
print(result);
