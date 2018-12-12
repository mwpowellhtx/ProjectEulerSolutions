using System;
using System.Collections.Generic;
using System.Linq;
using Google.OrTools.ConstraintSolver;

namespace ProjectEuler.Solutions.Currency.UK
{
    using static DateTime;
    using static Math;
    using static Solver;
    using static TimeSpan;
    using static Tuple;
    using static Denomination;

    public class ChangeMakerProblemSolver : ChangeMakerBase, IDisposable
    {
        /// <summary>
        /// Gets or sets when StartedOn, the <see cref="DateTime"/> just prior to
        /// <see cref="Solver.NewSearch(DecisionBuilder)"/>.
        /// </summary>
        private DateTime? StartedOn { get; set; }

        /// <summary>
        /// Gets or sets when FinishedOn, the <see cref="DateTime"/> just after
        /// <see cref="Solver.EndSearch"/>.
        /// </summary>
        private DateTime? FinishedOn { get; set; }

        // ReSharper disable once UnusedMember.Global
        /// <summary>
        /// Gets the calculated <see cref="TimeSpan"/> difference between <see cref="FinishedOn"/>
        /// and <see cref="StartedOn"/>. Defaults to <see cref="UtcNow"/> in the event either
        /// of the components is not set.
        /// </summary>
        public TimeSpan Elapsed => FromTicks(Abs(((FinishedOn ?? UtcNow) - (StartedOn ?? UtcNow)).Ticks));

        private const string SolverName = "ProjectEuler #31 Coin Sums";

        // TODO: TBD: given the problem definition, 250 of any Denominational Unit is more than sufficient, i.e. there can only ever be 200x1p in £2, for example
        // TODO: TBD: a broader, more flexible application might calculate that maximum based on the Total Denominational Composite, however
        private const long DefaultMaximumDenominationCount = 250L;

        private long MaximumDenominationCount { get; }

        // ReSharper disable once UnusedMember.Global
        /// <summary>
        /// Default Constructor.
        /// </summary>
        /// <inheritdoc />
        public ChangeMakerProblemSolver()
            : this(DefaultMaximumDenominationCount)
        {
        }

        /// <summary>
        /// Default Constructor.
        /// </summary>
        /// <param name="maximumDenominationCount"></param>
        public ChangeMakerProblemSolver(long maximumDenominationCount)
        {
            MaximumDenominationCount = maximumDenominationCount;
        }

        /// <summary>
        /// Maintain the list of ClrObjects for integrity purposes.
        /// </summary>
        internal IList<object> ClrObjects { get; } = new List<object>();

        /// <summary>
        /// Declares the <paramref name="solver"/> Variables while also preserving the
        /// <see cref="Denomination"/> dimension.
        /// </summary>
        /// <param name="solver"></param>
        /// <returns></returns>
        private IntVar[,] DeclareVariables(Solver solver)
        {
            // ReSharper disable once IdentifierTypo
            // Solve for the Composition of all Denominations Except the TwoPounds.
            var compositionDenoms = GetValues<Denomination>().Except(new[] {TwoPounds}).ToArray();

            int i = -1, j;
            var compositionVars = new IntVar[compositionDenoms.Length + 1, 2];

            foreach (var d in compositionDenoms)
            {
                var actualWeight = d.GetPenceValue();
                var weight = solver.MakeIntVar(actualWeight, actualWeight, $"{d}Weight");
                // TODO: TBD: from 0L through maximum is completely arbitrary.
                var value = solver.MakeIntVar(0L, MaximumDenominationCount, $"{d}Value");

                compositionVars[++i, j = 0] = weight;
                compositionVars[i, ++j] = value;
            }

            var actualResultWeight = TwoPounds.GetPenceValue();
            var resultWeight = solver.MakeIntVar(actualResultWeight, actualResultWeight, $"{TwoPounds}Weight");
            // This is the key here, now many Denominations of Currency does it take to make 1x TwoPounds.
            var resultValue = solver.MakeIntVar(1L, 1L, $"{TwoPounds}Value");

            compositionVars[++i, j = 0] = resultWeight;
            compositionVars[i, ++j] = resultValue;

            compositionVars.Flatten().ToList().ForEach(x => x.Register(this));

            return compositionVars;
        }

        // ReSharper disable once UnusedParameter.Local
        private IEnumerable<Constraint> DefineConstraints(Solver solver, IntVar[,] compositionVars)
        {
            // ReSharper disable once IdentifierTypo
            IntExpr MakeDenomComposite(Denomination d)
                => compositionVars[(int) d, 0] * compositionVars[(int) d, 1];

            var onePenceComposite = MakeDenomComposite(OnePence).Register(this);
            var twoPenceComposite = MakeDenomComposite(TwoPence).Register(this);
            var fivePenceComposite = MakeDenomComposite(FivePence).Register(this);
            var tenPenceComposite = MakeDenomComposite(TenPence).Register(this);
            var twentyPenceComposite = MakeDenomComposite(TwentyPence).Register(this);
            var fiftyPenceComposite = MakeDenomComposite(FiftyPence).Register(this);
            var onePoundComposite = MakeDenomComposite(OnePound).Register(this);
            var twoPoundsComposite = MakeDenomComposite(TwoPounds).Register(this);

            var compositeSum =
                    onePenceComposite
                    + twoPenceComposite
                    + fivePenceComposite
                    + tenPenceComposite
                    + twentyPenceComposite
                    + fiftyPenceComposite
                    + onePoundComposite
                ;

            compositeSum.Register(this);

            var theMoneyConstraint = compositeSum == twoPoundsComposite;

            yield return theMoneyConstraint;
        }

        public event EventHandler<ChangeMakerSolutionEventArgs> Solved;

        private void OnSolved(ChangeMaker solution)
            => Solved?.Invoke(this, new ChangeMakerSolutionEventArgs(solution));

        // ReSharper disable once UnusedMember.Global
        public bool TrySolve()
        {
            var count = 0;

            void CountSolutions(object sender, ChangeMakerSolutionEventArgs e) => ++count;

            Solved += CountSolutions;

            using (var solver = new Solver(SolverName))
            {
                var compositionVars = DeclareVariables(solver);

                // ReSharper disable once IdentifierTypo
                decimal GetActualDenomCompositeValue(Denomination d)
                    => compositionVars[(int) d, 1].Value();

                // ReSharper disable once IdentifierTypo
                Tuple<Denomination, decimal> GetActualDenomComposite(Denomination d)
                    => Create(d, GetActualDenomCompositeValue(d));

                DefineConstraints(solver, compositionVars).ToList().ForEach(solver.Add);

                using (var sc = solver.MakeAllSolutionCollector())
                {
                    using (var db = solver.MakePhase(compositionVars.Flatten().ToArray()
                        , INT_VAR_DEFAULT, INT_VALUE_DEFAULT))
                    {
                        StartedOn = UtcNow;
                        solver.NewSearch(db, sc);

                        while (solver.NextSolution())
                        {
                            var solution = new ChangeMaker(
                                GetActualDenomComposite(TwoPounds)
                                , GetActualDenomComposite(OnePence)
                                , GetActualDenomComposite(TwoPence)
                                , GetActualDenomComposite(FivePence)
                                , GetActualDenomComposite(TenPence)
                                , GetActualDenomComposite(TwentyPence)
                                , GetActualDenomComposite(FiftyPence)
                                , GetActualDenomComposite(OnePound)
                            );
                            OnSolved(solution);
                        }

                        solver.EndSearch();
                        FinishedOn = UtcNow;
                    }
                }
            }

            Solved -= CountSolutions;
            return count > 0;
        }

        public void Dispose()
        {
            //foreach (var x in ClrObjects.OfType<IDisposable>())
            //{
            //    x.Dispose();
            //}
        }
    }

    internal static class ClrExtensionMethods
    {
        /// <summary>
        /// We need to do this to prevent premature Garbage Collection.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="x"></param>
        /// <param name="problemSolver"></param>
        /// <returns></returns>
        public static T Register<T>(this T x, ChangeMakerProblemSolver problemSolver)
        {
            problemSolver.ClrObjects.Add(x);
            return x;
        }
    }
}
