using System;

namespace ProjectEuler.Solutions.Currency.UK
{
    public class ChangeMakerSolutionEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the <see cref="ChangeMaker"/> Solution.
        /// </summary>
        public ChangeMaker Solution { get; }

        internal ChangeMakerSolutionEventArgs(ChangeMaker solution)
        {
            Solution = solution;
        }
    }
}
