import Foundation

typealias Dir = (files: [String: Int], dirs: Set<String>, parent: String)
typealias FileSystem = [String: Dir]

typealias Entry = (type: String, name: String, size: Int)
typealias Command = [String]
typealias Output = [Entry]

typealias Input = [(Command, Output)]
typealias Result = Int

class Parser {
    func parseCommand(_ line: String) -> Command {
        return line.components(separatedBy: " ");
    }

    func parseOutput(_ lines: [String]) -> Output {
        return lines.filter { $0 != "" }.map {
            let parts = $0.components(separatedBy: " ");

            if (parts[0] == "dir") {
                return (type: "dir", name: parts[1], size: 0)
            }

            return (type: "file", name: parts[1], size: Int(parts[0])!)
        }
    }

    func parse(contents: String) -> Input {
        var result: Input = [];
        let blocks = contents.components(separatedBy: "$");

        for block in blocks {
            if block == "" {
                continue
            }

            let lines = block.components(separatedBy: "\n").map { $0.trimmingCharacters(in: .whitespacesAndNewlines) };
            let command = parseCommand(lines[0]);
            let output = parseOutput(Array(lines[1...]));

            result.append((command, output));
        }

        return result;
    }
}

class Solution {
    var fs: FileSystem = [
        "/": (files: [:], dirs: [], parent: "")
    ]
    var sizes: [String: Int] = [:];

    func parseFs(_ input: Input) {
        var cwd = "/";

        for (command, output) in input {
            if command[0] == "cd" {
                switch command[1] {
                case "/":
                    cwd = "/";
                case "..":
                    cwd = fs[cwd]!.parent
                default:
                    let dir = "\(cwd)\(command[1])/"
                    fs[cwd]!.dirs.insert(dir)
                    cwd = dir
                }
            }

            if command[0] == "ls" {
                for entry in output {
                    let path = "\(cwd)\(entry.name)/"

                    if entry.type == "dir" {
                        fs[path] = (files: [:], dirs: [], parent: cwd)
                    } else {
                        fs[cwd]!.files[entry.name] = entry.size
                    }
                }
            }
        }
    }

    @discardableResult func getSize(_ current: String) -> Int {
        if sizes[current] != nil {
            return sizes[current]!
        }

        let dir = fs[current]!
        let sizeArr = dir.files.values + dir.dirs.map(getSize)
        let result = sizeArr.reduce(0, +)

        sizes[current] = result

        return result
    }

    func part1(_ input: Input) -> Result {
        parseFs(input)
        getSize("/")

        return sizes.values.filter { $0 <= 100000 }.reduce(0, +)
    }

    func part2(_ input: Input) -> Result {
        parseFs(input)
        getSize("/")

        let capacity = 70000000
        let neededForUpdate = 30000000
        let available = capacity - sizes["/"]!
        let needed = neededForUpdate - available

        return sizes.values.sorted().first(where: { $0 > needed }) ?? 0
    }
}

let contents = try! String(contentsOfFile: "/Users/igor/Downloads/input.txt");
let input = Parser().parse(contents: contents);
let result = Solution().part2(input);
print(result);
