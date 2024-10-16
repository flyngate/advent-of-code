def f(z, w, a, b, c):
    x = 1 if (z % 26 + 11) != w else 0
    return z // b * (25 * x + 1) + (w + 8) * x

params = [
    [11, 1, 8],
    [14, 1, 13],
    [10, 1, 2],
    [0, 26, 7],
    [12, 1, 11],
    [12, 1, 4],
    [12, 1, 13],
    [-8, 26, 13],
    [-9, 26, 10],
    [11, 1, 1],
    [0, 26, 2],
    [-5, 26, 14],
    [-6, 26, 6],
    [-12, 26, 14],
];

number_arr = list("00000000000000")
min_number = 100000000000000000000

def solve(n, z):
    global min_number

    if n == 14:
        number = int("".join(number_arr))

        if number < min_number:
            print(number, "/", z)
            min_number = number

        return

    [a, b, c] = params[n]

    w = z % 26 + a

    if w >= 1 and w <= 9:
        solve(n + 1, f(z, w, a, b, c))
    else:
        for w in range(1, 10):
            number_arr[n] = str(w)
            solve(n + 1, f(z, w, a, b, c))

def calc_z(number):
    digits = str(number)
    z = 0

    for (i, digit) in enumerate(digits):
        [a, b, c] = params[i]
        z = f(z, int(digit), a, b, c)

    return z

print(calc_z("13799949489194"))
