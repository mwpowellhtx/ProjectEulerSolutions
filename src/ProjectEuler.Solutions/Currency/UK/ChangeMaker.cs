using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace ProjectEuler.Solutions.Currency.UK
{
    using static String;
    using static Tuple;
    using static Denomination;

    [DebuggerDisplay("{" + nameof(DebuggerDisplayText) + "}")]
    public class ChangeMaker : ChangeMakerBase
    {
        internal string DebuggerDisplayText => ToString();

        public IDictionary<Denomination, decimal> Composition { get; }

        public Tuple<Denomination, decimal> Total { get; }

        public ChangeMaker(Tuple<Denomination, decimal> total, params Tuple<Denomination, decimal>[] composition)
        {
            Total = total;

            // ReSharper disable once IdentifierTypo
            TComposition EmplaceComposite<TComposition>(TComposition given, Denomination key, decimal value)
                where TComposition : IDictionary<Denomination, decimal>
            {
                given[key] = value;
                return given;
            }

            // Only record the Components that actually sustain a Value.
            Composition = composition.Where(x => x.Item2 != 0m)
                .Aggregate(new Dictionary<Denomination, decimal>()
                    , (g, x) => EmplaceComposite(g, x.Item1, x.Item2)
                );
        }

        // ReSharper disable once UnusedMember.Global
        public ChangeMaker(decimal totalValue, Denomination totalDenomination,
            params Tuple<Denomination, decimal>[] composition)
            : this(Create(totalDenomination, totalValue), composition)
        {
        }

        // TODO: TBD: add a "report" method of some sort ... for now, ToString will suffice...
        public override string ToString()
        {
            // ReSharper disable once IdentifierTypo
            string GetDenomPrefix(Denomination d) => new[] {OnePound, TwoPounds}.Contains(d) ? "£" : "";
            // ReSharper disable once IdentifierTypo
            string GetDenomSuffix(Denomination d) => new[] {OnePound, TwoPounds}.Contains(d) ? "" : "p";
            string GetExchangeValueString(Denomination d, decimal exchange) => $"{exchange}×{GetDenomPrefix(d)}{d.GetUnitValue()}{GetDenomSuffix(d)}";
            string GetValueString(Denomination d) => GetExchangeValueString(d, Composition[d]);
            string GetTotalString() => GetExchangeValueString(Total.Item1, Total.Item2);
            var compositeBits = GetValues<Denomination>().Where(Composition.ContainsKey).Except(GetRange(Total.Item1)).Select(GetValueString);
            return $"There are {Join(" + ", compositeBits)} in {GetTotalString()}";
        }
    }
}
