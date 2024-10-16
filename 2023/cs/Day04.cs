using System;
using System.Data;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Xml.XPath;

namespace Day04
{
    public class Card
    {
        public int id;
        // public HashSet<int> winning;
        // public HashSet<int> youHave;
        public int copies;
        public int matchingCards;

        public int Worth
        {
            get => this.matchingCards > 0
                    ? (int)Math.Pow(2, this.matchingCards - 1)
                    : 0;
        }
    }


    public class Parser
    {
        public List<Card> Parse(string[] lines)
        {
            var cards = new List<Card>();
            var regex = new Regex(@"Card\s+(\d+): ([0-9 ]+) \| ([0-9 ]+)");

            foreach (var line in lines)
            {
                var matches = regex.Match(line);
                var id = int.Parse(matches.Groups[1].Value);
                var winning = matches.Groups[2].Value
                    .Split(" ")
                    .Where(s => s != "")
                    .Select(x => int.Parse(x)).ToHashSet();
                var youHave = matches.Groups[3].Value
                    .Split(" ")
                    .Where(s => s != "")
                    .Select(x => int.Parse(x)).ToHashSet();
                var matchingCards = youHave.Where(x => winning.Contains(x)).Count();

                cards.Add(new Card()
                {
                    id = id,
                    matchingCards = matchingCards,
                    copies = 1,
                });
            }

            return cards;
        }
    }

    public class PartOne
    {
        public int Solve(List<Card> cards)
        {
            return cards.Select(card => card.Worth).Sum();
        }
    }

    public class PartTwo
    {
        public int Solve(List<Card> cards)
        {
            for (int i = 0; i < cards.Count; i++) {
                var matchingCards = cards[i].matchingCards;

                for (int j = 1; i + j < cards.Count && j <= matchingCards; j++) {
                    var card = cards[i + j];

                    card.copies += cards[i].copies;
                }
            }

            return cards.Select(card => card.copies).Sum();
        }
    }
}
