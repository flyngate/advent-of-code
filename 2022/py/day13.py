import json
import functools

def compare(a, b):
    for (left, right) in zip(a, b):
        if type(left) == int and type(right) == int:
            if left > right: return 1
            if left < right: return -1
        elif type(left) == list and type(right) == int:
            r = compare(left, [right])
            if r != 0: return r
        elif type(left) == int and type(right) == list:
            r = compare([left], right)
            if r != 0: return r
        else:
            r = compare(left, right)
            if r != 0: return r

    if len(a) > len(b): return 1
    if len(a) < len(b): return -1

    return 0
            

def part1(pairs):
    result = 0

    for index, (a, b) in enumerate(pairs):
        if compare(a, b) < 0:
            result += index + 1

    return result

def part2(pairs):
    divider1 = [[2]]
    divider2 = [[6]]

    all = [divider1, divider2]

    for (a, b) in pairs:
        all.append(a)
        all.append(b)

    s = sorted(all, key=functools.cmp_to_key(compare))

    i = s.index(divider1)
    j = s.index(divider2)

    return (i + 1) * (j + 1)

with open("/Users/igor/Downloads/input.txt") as f:
    input = f.read()
    parsed = []
    
    for block in input.split("\n\n"):
        lines = block.split("\n")
        parsed.append((
            json.loads(lines[0]),
            json.loads(lines[1])
        ))

    result = part2(parsed)
    print(result)
