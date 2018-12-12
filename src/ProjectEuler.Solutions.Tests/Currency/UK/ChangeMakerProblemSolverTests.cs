using System;
using System.Collections.Generic;

namespace ProjectEuler.Solutions.Currency.UK
{
    using Xunit;
    using Xunit.Abstractions;
    using static String;

    public class ChangeMakerProblemSolverTests
    {
        private ITestOutputHelper OutputHelper { get; }

        public ChangeMakerProblemSolverTests(ITestOutputHelper outputHelper)
        {
            OutputHelper = outputHelper;
        }

        [Fact]
        public void VerifySolver()
        {
            var solutions = new List<ChangeMaker>();

            IEnumerable<string> ReportTimeSpan(TimeSpan elapsed)
            {
                if (elapsed.Hours != 0)
                {
                    yield return $"{elapsed.Hours} hours";
                }

                if (elapsed.Minutes != 0)
                {
                    yield return $"{elapsed.Minutes} minutes";
                }

                if (elapsed.Seconds != 0)
                {
                    yield return $"{elapsed.Seconds} seconds";
                }

                if (elapsed.Milliseconds != 0)
                {
                    yield return $"{elapsed.Milliseconds} milliseconds";
                }
            }

            using (var ps = new ChangeMakerProblemSolver())
            {
                ps.Solved += (sender, e) =>
                {
                    solutions.Add(e.Solution);
                };

                Assert.True(ps.TrySolve());

                OutputHelper.WriteLine($"Solver ran in {Join(" ", ReportTimeSpan(ps.Elapsed))}");
            }

            const int expectedSolutionCount = 73681;

            Assert.Equal(expectedSolutionCount, solutions.Count);

            OutputHelper.WriteLine($"There were {solutions.Count} possible solutions.");

            // The lion's share of test time is spent here reporting the actual results.
            foreach (var s in solutions)
            {
                OutputHelper.WriteLine($"{s}");
            }
        }
    }
}
