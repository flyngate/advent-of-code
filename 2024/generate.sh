#!/bin/bash

if [ -z "$1" ]; then
  echo "usage: generate.sh 1"
  exit
fi

day=$(printf %02d $1)
path="Day$day.cs"

if [ -f "$path" ]; then
  echo "$path already exists"
  exit
fi

cat <<EOF > Day$day.cs
namespace AdventOfCode.Day$day;

public class Solution
{
    public object PartOne(string input)
    {
        return 0;
    }

    public object PartTwo(string input)
    {
        return 0;
    }
}
EOF
