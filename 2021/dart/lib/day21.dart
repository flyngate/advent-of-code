import 'dart:math';

class Die {
  int counter = 0;
  int get rolledTimes => counter;

  int roll() {
    final a = (counter) % 100 + 1;
    final b = (counter + 1) % 100 + 1;
    final c = (counter + 2) % 100 + 1;

    counter += 3;

    return a + b + c;
  }
}

int getPosition(int position, int moves) {
  return (position - 1 + moves) % 10 + 1;
}

class Game {
  final die = Die();
  int current = 0;
  List<int> player = [0, 0];
  List<int> score = [0, 0];

  Game(int player1, int player2) {
    player = [player1, player2];
  }

  bool step() {
    player[current] = getPosition(player[current], die.roll());
    score[current] += player[current];
    current = 1 - current;
    return max(score[0], score[1]) >= 1000;
  }

  int get loosingScore => min(score[0], score[1]);
  int get score1 => score[0];
  int get score2 => score[1];
}

int part1(int player1, int player2) {
  final game = Game(player1, player2);

  while (!game.step()) {
    print("${game.score1} ${game.score2}");
  }

  print("${game.score1} ${game.score2}");

  print(game.loosingScore);
  print(game.die.rolledTimes);

  return game.loosingScore * game.die.rolledTimes;
}

class QuantumGame {
  int turn;
  List<int> position;
  List<int> score;

  QuantumGame(this.position, this.score, [this.turn = 0]);

  QuantumGame play(int die) {
    List<int> nextPosition, nextScore;

    if (turn == 0) {
      nextPosition = [getPosition(position[0], die), position[1]];
      nextScore = [score[0] + nextPosition[0], score[1]];
    } else {
      nextPosition = [position[0], getPosition(position[1], die)];
      nextScore = [score[0], score[1] + nextPosition[1]];
    }

    return QuantumGame(nextPosition, nextScore, 1 - turn);
  }

  int get score1 => score[0];
  int get score2 => score[1];
  int get maxScore => max(score1, score2);
}

int part2(int player1, int player2) {
  final x = [0, 0, 0, 1, 3, 6, 7, 6, 3, 1].map(BigInt.from).toList();
  var result = BigInt.zero;

  void f(QuantumGame game, BigInt value) {
    if (game.maxScore >= 21) {
      if (game.score1 >= 21) {
        result += value;
      }
      return;
    }

    for (var die = 3; die < 10; die++) {
      final nextValue = value * x[die];

      f(game.play(die), nextValue);
    }
  }

  f(QuantumGame([player1, player2], [0, 0]), BigInt.one);

  print(result);

  return 0;
}

void run() async {
  final player1 = 8;
  final player2 = 2;
  final result = part2(player1, player2);
  print(result);
}
