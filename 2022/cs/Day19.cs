using System.Text.RegularExpressions;
using System.Numerics;

namespace Day19
{
    public struct OreRobotCost { public int ore; }
    public struct ClayRobotCost { public int ore; }
    public struct ObsidianRobotCost { public int ore, clay; }
    public struct GeodeRobotCost { public int ore, obsidian; }

    public class Blueprint
    {
        public OreRobotCost oreRobotCost;
        public ClayRobotCost clayRobotCost;
        public ObsidianRobotCost obsidianRobotCost;
        public GeodeRobotCost geodeRobotCost;
    }

    public class State
    {
        public int oreRobots, clayRobots, obsidianRobots, geodeRobots;
        public int ore, clay, obsidian, geode;
        public int minute;

        public State Clone()
        {
            return (State)MemberwiseClone();
        }

        public void ProduceResources()
        {
            ore += oreRobots;
            clay += clayRobots;
            obsidian += obsidianRobots;
            geode += geodeRobots;
        }

        public void AdvanceTime(int minutes)
        {
            minute += minutes;
            ore += oreRobots * minutes;
            clay += clayRobots * minutes;
            obsidian += obsidianRobots * minutes;
            geode += geodeRobots * minutes;
        }
    }

    class Parser
    {
        public Blueprint[] Parse(string[] lines)
        {
            var result = new List<Blueprint>();
            var regex = new Regex(@"Blueprint ([0-9]+):\s*" +
                @"Each ore robot costs ([0-9]+) ore.\s*" +
                @"Each clay robot costs ([0-9]+) ore.\s*" +
                @"Each obsidian robot costs ([0-9]+) ore and ([0-9]+) clay.\s*" +
                @"Each geode robot costs ([0-9]+) ore and ([0-9]+) obsidian.");

            var content = string.Concat(lines);
            var matches = regex.Matches(content);

            foreach (Match match in matches)
            {
                var groups = match.Groups;

                result.Add(new Blueprint()
                {
                    oreRobotCost = new OreRobotCost()
                    {
                        ore = int.Parse(groups[2].Value)
                    },
                    clayRobotCost = new ClayRobotCost()
                    {
                        ore = int.Parse(groups[3].Value)
                    },
                    obsidianRobotCost = new ObsidianRobotCost()
                    {
                        ore = int.Parse(groups[4].Value),
                        clay = int.Parse(groups[5].Value)
                    },
                    geodeRobotCost = new GeodeRobotCost()
                    {
                        ore = int.Parse(groups[6].Value),
                        obsidian = int.Parse(groups[7].Value)
                    },
                });
            }

            return result.ToArray();
        }
    }

    public class PartTwo
    {
        int Result = 0;
        const int MinutesAmount = 32;

        int GetTimeToBuild(int cost, int currentAmount, int robotsAmount)
        {
            return (int)Math.Ceiling(Math.Max(cost - currentAmount, 0) * 1.0 / robotsAmount + 1);
        }

        State[] GetNextStates(Blueprint blueprint, State state)
        {
            var result = new List<State>();

            {
                var nextState = state.Clone();
                var minutes = MinutesAmount - state.minute;
                nextState.AdvanceTime(minutes);
                result.Add(nextState);
            }

            if (state.obsidianRobots > 0 && state.geodeRobots < 15)
            {
                var nextState = state.Clone();
                var minutes = Math.Max(
                    GetTimeToBuild(blueprint.geodeRobotCost.ore, state.ore, state.oreRobots),
                    GetTimeToBuild(blueprint.geodeRobotCost.obsidian, state.obsidian, state.obsidianRobots)
                );
                nextState.AdvanceTime(minutes);
                nextState.geodeRobots += 1;
                nextState.ore -= blueprint.geodeRobotCost.ore;
                nextState.obsidian -= blueprint.geodeRobotCost.obsidian;
                result.Add(nextState);
            }

            if (state.clayRobots > 0 && state.obsidianRobots < 15)
            {
                var nextState = state.Clone();
                var minutes = Math.Max(
                    GetTimeToBuild(blueprint.obsidianRobotCost.ore, state.ore, state.oreRobots),
                    GetTimeToBuild(blueprint.obsidianRobotCost.clay, state.clay, state.clayRobots)
                );
                nextState.AdvanceTime(minutes);
                nextState.obsidianRobots += 1;
                nextState.ore -= blueprint.obsidianRobotCost.ore;
                nextState.clay -= blueprint.obsidianRobotCost.clay;
                result.Add(nextState);
            }

            if (state.clayRobots < 15)
            {
                var nextState = state.Clone();
                var minutes = GetTimeToBuild(blueprint.clayRobotCost.ore, state.ore, state.oreRobots);
                nextState.AdvanceTime(minutes);
                nextState.clayRobots += 1;
                nextState.ore -= blueprint.clayRobotCost.ore;
                result.Add(nextState);
            }

            if (state.oreRobots < 15)
            {
                var nextState = state.Clone();
                var minutes = GetTimeToBuild(blueprint.oreRobotCost.ore, state.ore, state.oreRobots);
                nextState.AdvanceTime(minutes);
                nextState.oreRobots += 1;
                nextState.ore -= blueprint.oreRobotCost.ore;
                result.Add(nextState);
            }

            return result.ToArray();
        }

        void Rec(Blueprint blueprint, State state)
        {
            if ((MinutesAmount - state.minute) * (MinutesAmount - state.minute + state.geodeRobots) + state.geode < Result)
                return;

            if (state.minute > MinutesAmount)
                return;

            if (state.minute == MinutesAmount)
            {
                if (Result < state.geode)
                    Result = state.geode;

                return;
            }

            var nextStates = GetNextStates(blueprint, state);

            foreach (var nextState in nextStates)
                Rec(blueprint, nextState);
        }

        int GetMaxGeode(Blueprint blueprint)
        {
            Result = 0;
            var state = new State() { oreRobots = 1 };

            Rec(blueprint, state);

            return Result;
        }

        public int Solve(Blueprint[] blueprints)
        {
            var result = 1;

            foreach (var blueprint in blueprints.Take(3))
            {
                var maxGeode = GetMaxGeode(blueprint);
                result *= maxGeode;
            }

            return result;
        }
    }
}
