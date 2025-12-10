namespace JapaneseTrainer.Api.Exceptions
{
    /// <summary>
    /// Exception thrown when attempting to create an Item that already exists
    /// </summary>
    public class DuplicateItemException : Exception
    {
        public string Japanese { get; }
        public string? Reading { get; }
        public string? HashKey { get; }

        public DuplicateItemException(string japanese, string? reading = null, string? hashKey = null)
            : base($"Item with Japanese '{japanese}' and Reading '{reading ?? "N/A"}' already exists in the system.")
        {
            Japanese = japanese;
            Reading = reading;
            HashKey = hashKey;
        }
    }
}

