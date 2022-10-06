namespace Utility.Transaction
{
    public struct TransactionToken
    {
        internal long Generation { get; }

        internal TransactionToken(long generation)
        {
            Generation = generation;
        }
    }
}