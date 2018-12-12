namespace ProjectEuler.Solutions.Currency.UK
{
    using System;
    using static Denomination;

    /// <summary>
    /// Specified Denominational units for purposes of correct Variable Index resolution.
    /// </summary>
    public enum Denomination
    {
        /// <summary>
        /// 1p
        /// </summary>
        OnePence,

        /// <summary>
        /// 2p
        /// </summary>
        TwoPence,

        /// <summary>
        /// 5p
        /// </summary>
        FivePence,

        /// <summary>
        /// 10p
        /// </summary>
        TenPence,

        /// <summary>
        /// 20p
        /// </summary>
        TwentyPence,

        /// <summary>
        /// 50p
        /// </summary>
        FiftyPence,

        /// <summary>
        /// £1 or 100p
        /// </summary>
        OnePound,

        /// <summary>
        /// £2 or 200p
        /// </summary>
        TwoPounds,
    }

    public static class DenominationExtensionMethods
    {
        // ReSharper disable once IdentifierTypo
        public static long GetPenceValue(this Denomination value)
        {
            // ReSharper disable once SwitchStatementMissingSomeCases
            switch (value)
            {
                case OnePence: return 1L;
                case TwoPence: return 2L;
                case FivePence: return 5L;
                case TenPence: return 10L;
                case TwentyPence: return 20L;
                case FiftyPence: return 50L;
                case OnePound: return 100L;
                case TwoPounds: return 200L;
            }

            throw new ArgumentException($"{typeof(Denomination).FullName} '{value}' unsupported", nameof(value));
        }

        /// <summary>
        /// Returns the <paramref name="value"/> in terms of its respective Currency Unit.
        /// That is, in either Pence (p) our Pounds (£).
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static decimal GetUnitValue(this Denomination value)
        {
            // ReSharper disable once SwitchStatementMissingSomeCases
            switch (value)
            {
                case OnePence: return 1m;
                case TwoPence: return 2m;
                case FivePence: return 5m;
                case TenPence: return 10m;
                case TwentyPence: return 20m;
                case FiftyPence: return 50m;
                case OnePound: return 1m;
                case TwoPounds: return 2m;
            }

            throw new ArgumentException($"{typeof(Denomination).FullName} '{value}' unsupported", nameof(value));
        }
    }
}
