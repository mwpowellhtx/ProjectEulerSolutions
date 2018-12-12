using System;
using System.Collections.Generic;
using System.Linq;

namespace ProjectEuler.Solutions.Currency.UK
{
    public abstract class ChangeMakerBase
    {
        /// <summary>
        /// Returns the Enumerated Values. Assumes that <typeparamref name="T"/>
        /// is an <see cref="Enum"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        protected static IEnumerable<T> GetValues<T>()
            where T : struct
            => Enum.GetValues(typeof(T)).OfType<T>();

        protected static IEnumerable<T> GetRange<T>(params T[] values)
        {
            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (var x in values)
            {
                yield return x;
            }
        }
    }
}
