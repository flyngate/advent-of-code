using System;
using System.ComponentModel;
using System.Data;
using System.IO.Compression;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Text.RegularExpressions;
using System.Xml.Schema;
using System.Xml.XPath;

namespace Day07
{

    public class Hand
    {
        public string hand;
        public int[] cards;
        public int bid;
        public int type;

        public int CompareTo(Hand other)
        {
            if (type != other.type)
            {
                return type.CompareTo(other.type);
            }

            for (int i = 0; i < cards.Length; i++)
            {
                if (cards[i] != other.cards[i])
                {
                    return cards[i].CompareTo(other.cards[i]);
                }
            }

            return 0;
        }
    }

    public abstract class HandFactory
    {
        public abstract Hand Create(string hand, int bid);
    }

    public class HandFactoryPartOne : HandFactory
    {

        public static int GetStrength(char card)
        {
            switch (card)
            {
                case 'A': return 14;
                case 'K': return 13;
                case 'Q': return 12;
                case 'J': return 11;
                case 'T': return 10;
                default: return int.Parse(card.ToString());
            }
        }

        public static int GetType(int[] _cards)
        {
            var cards = new int[_cards.Length];

            _cards.CopyTo(cards, 0);
            Array.Sort(cards);

            var first = cards[0];

            // five of a kind
            if (cards.All(card => card == first))
            {
                return 7;
            }

            // four of a kind
            if (cards[0] == cards[1] && cards[1] == cards[2] && cards[2] == cards[3])
            {
                return 6;
            }

            if (cards[1] == cards[2] && cards[2] == cards[3] && cards[3] == cards[4])
            {
                return 6;
            }

            // full house
            if (cards[0] == cards[1] && cards[2] == cards[3] && cards[3] == cards[4])
            {
                return 5;
            }

            if (cards[0] == cards[1] && cards[1] == cards[2] && cards[3] == cards[4])
            {
                return 5;
            }

            // three of a kind
            if (cards[0] == cards[1] && cards[1] == cards[2])
            {
                return 4;
            }

            if (cards[1] == cards[2] && cards[2] == cards[3])
            {
                return 4;
            }

            if (cards[2] == cards[3] && cards[3] == cards[4])
            {
                return 4;
            }

            // two pair
            if (cards[0] == cards[1] && cards[2] == cards[3])
            {
                return 3;
            }

            if (cards[0] == cards[1] && cards[3] == cards[4])
            {
                return 3;
            }

            if (cards[1] == cards[2] && cards[3] == cards[4])
            {
                return 3;
            }

            // one pair
            if (cards[0] == cards[1]
                || cards[1] == cards[2]
                || cards[2] == cards[3]
                || cards[3] == cards[4])
            {
                return 2;
            }

            // high card
            return 1;
        }

        override public Hand Create(string hand, int bid)
        {
            var cards = hand.ToList().Select(GetStrength).ToArray();

            return new Hand()
            {
                hand = hand,
                cards = cards,
                type = GetType(cards),
                bid = bid,
            };
        }
    }

    public class HandFactoryPartTwo : HandFactory
    {
        static List<int[]> GetPossibleCombinations(int[] cards, int index, List<int[]> acc)
        {
            if (index >= cards.Length)
            {
                acc.Add(cards);

                return acc;
            }

            if (cards[index] == 1)
            {

                for (int card = 2; card <= 14; card++)
                {
                    var cardsCopy = (int[])cards.Clone();

                    cardsCopy[index] = card;

                    GetPossibleCombinations(cardsCopy, index + 1, acc);
                }
            }

            return GetPossibleCombinations(cards, index + 1, acc);
        }

        static int GetType(int[] cards)
        {
            var possibleCombinations = GetPossibleCombinations(cards, 0, new List<int[]>());

            return possibleCombinations
                .Select(HandFactoryPartOne.GetType)
                .Max();
        }

        static int GetStrength(char card)
        {
            if (card == 'J')
                return 1;

            return HandFactoryPartOne.GetStrength(card);
        }

        override public Hand Create(string hand, int bid)
        {
            var cards = hand.ToList().Select(GetStrength).ToArray();

            return new Hand()
            {
                hand = hand,
                cards = cards,
                type = GetType(cards),
                bid = bid,
            };
        }
    }

    public class Parser
    {
        public HandFactory handFactory;

        public List<Hand> Parse(string[] lines, string content)
        {
            var hands = new List<Hand>();

            foreach (var line in lines)
            {
                var parts = line.Split(" ");
                var hand = parts[0];
                var bid = int.Parse(parts[1]);

                hands.Add(handFactory.Create(hand, bid));
            }

            return hands;
        }
    }

    public class PartOne
    {
        public Parser parser = new()
        {
            handFactory = new HandFactoryPartTwo(),
        };

        public int Solve(List<Hand> hands)
        {
            hands.Sort((a, b) => a.CompareTo(b));

            return hands.Select((hand, index) => (index + 1) * hand.bid).Sum();
        }
    }
}
