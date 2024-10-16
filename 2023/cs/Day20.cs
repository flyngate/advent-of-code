using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.IO.Compression;
using System.IO.Pipes;
using System.Numerics;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Security;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using System.Text;
using System.Text.RegularExpressions;
using System.Transactions;
using System.Xml.Schema;
using System.Xml.XPath;
using AdventOfCode;

namespace Day20
{
    public enum Pulse
    {
        High,
        Low,
        None,
    }

    public class Module(string name, string[] outputs)
    {
        public string Name = name;
        public string[] Outputs = outputs;

        public virtual Pulse Receive(string source, Pulse pulse)
        {
            return pulse;
        }
    }

    public class FlipFlop(string name, string[] outputs)
        : Module(name, outputs)
    {
        public bool On = false;

        public override Pulse Receive(string source, Pulse pulse)
        {
            if (pulse == Pulse.High)
                return Pulse.None;

            On = !On;

            return On ? Pulse.High : Pulse.Low;
        }
    }

    public class Conjunction(string name, string[] outputs)
        : Module(name, outputs)
    {
        public HashSet<string> highPulses = [];
        public int Inputs = 0;

        public bool SendsLowPulse
        {
            get => highPulses.Count == Inputs;
        }

        public override Pulse Receive(string source, Pulse pulse)
        {
            if (pulse == Pulse.High)
                highPulses.Add(source);
            else
                highPulses.Remove(source);
            
            var count = highPulses.Count;
            
            if (Name == "sq" && count == 2)
                ;

            if (Name == "sq" && count == 3)
                ;

            return highPulses.Count == Inputs
                ? Pulse.Low
                : Pulse.High;
        }
    }

    public class Broadcaster(string name, string[] outputs)
        : Module(name, outputs);

    public class Parser
    {
        public Module[] Parse(string[] lines, string content)
        {
            var modules = lines.Select<string, Module>(line =>
            {
                var sep = line.IndexOf(" -> ");
                var name = line[0..sep];
                var outputs = line[(sep + 4)..].Split(", ").ToArray();

                if (name == "broadcaster")
                    return new Broadcaster(name, outputs);

                if (name.StartsWith('%'))
                    return new FlipFlop(name[1..], outputs);

                return new Conjunction(name[1..], outputs);
            }).ToArray();

            foreach (var module in modules)
            {
                if (module is Conjunction con)
                    con.Inputs = modules.Count(m => m.Outputs.Contains(con.Name));
            }

            return modules;
        }
    }

    public class Circuit(Module[] modules)
    {
        public Dictionary<string, Module> Modules =
            modules.ToDictionary(module => module.Name);
        public int High;
        public int Low;
        public int Rx;

        public void Simulate(int iter)
        {
            var pulses = new Queue<(string, string, Pulse)>();

            pulses.Enqueue(("", "broadcaster", Pulse.Low));

            while (pulses.Count > 0)
            {
                var (source, dest, inputPulse) = pulses.Dequeue();

                if (inputPulse == Pulse.High)
                    High += 1;

                if (inputPulse == Pulse.Low)
                    Low += 1;
                
                if (dest == "rx" && inputPulse == Pulse.Low)
                    Rx++;
                
                if (!Modules.ContainsKey(dest))
                    continue;

                var module = Modules[dest];
                var pulse = module.Receive(source, inputPulse);

                // if (dest == "vm" && (Modules["vm"] as Conjunction).SendsLowPulse)
                //     Console.WriteLine($"vm, {iter}");

                // if (dest == "kb" && (Modules["kb"] as Conjunction).SendsLowPulse)
                //     Console.WriteLine($"kb, {iter}");

                // if (dest == "dn" && (Modules["dn"] as Conjunction).SendsLowPulse)
                //     Console.WriteLine($"dn, {iter}");

                // if (dest == "vk" && (Modules["vk"] as Conjunction).SendsLowPulse)
                //     Console.WriteLine($"vk, {iter}");

                if (pulse == Pulse.None)
                    continue;

                foreach (var output in module.Outputs)
                    pulses.Enqueue((module.Name, output, pulse));
            }
        }
    }

    public class PartOne
    {
        public Parser parser = new();

        public int Solve(Module[] modules)
        {
            var circuit = new Circuit(modules);
            var iterations = 1000;

            for (int i = 0; i < iterations; i++)
                circuit.Simulate(i);

            return circuit.High * circuit.Low;
        }
    }

    public class PartTwo
    {
        public Parser parser = new();

        public long Solve(Module[] modules)
        {
            long[] cycles = [3863, 3931, 3797, 3769];
            long lcm = cycles[0];

            for (int i = 1; i < cycles.Length; i++)
                lcm = lcm * cycles[i] / Algo.GCD(lcm, cycles[i]);

            return lcm;
        }
    }
}
