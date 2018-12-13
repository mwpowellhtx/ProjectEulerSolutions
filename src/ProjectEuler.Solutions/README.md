## Project Euler Problem 31: Coin Sums

This project provides a literal problem solving solution to the [Coin sums](http://projecteuler.net/problem=31). In this approach we will use the Google OR-tools, short for Operational Research Tools.

Instead of any brute force calculations involving permutations, combinations, or things of this nature, we actually "train" the problem solver, if you will, in what the problem looks like. Training involves basically specifying variables important to the resolution of the problem to solution, if possible, as well as constraints that inform the solver when solution is achieved, as well as any intermediate expressions that are necessary in order to guide the solver when making its solution decisions.

In terms of the problem itself, the problem lends itself quite well to specifying not only the demoninational units, i.e. in *Pence*, or *Pounds* in terms *Pence*, but also the solution involving the number of each unit required in order to achieve the total, in this case of £2, or 2 Pounds.

If I wanted to spend more time with this, I might consider an appropriate using C++ *Boost.Units* in order to better capture the **Currency**, in this case *U.K. currency*, as well as the units. However, for simplicity and early delivery, I find it easier to at least prototype the approach using the tried and true tools of .NET and C#, at least in my experience. In so many words, when the chips are down, I like to play to my strengths.

When using a solver, in most cases, one does not know the number of possible solutions. One may solve for any number of solutions, or all of them, etc, depending on the constraints and any optimization functions that were provided; let's say I wanted to maximize the responses involving *10p*, then I would receive the solution or solutions that maximized this result from the problem description.

In this case, however, I just wanted to collate all of the responses and report the total number of possible responses, as well as print those out in the report.
