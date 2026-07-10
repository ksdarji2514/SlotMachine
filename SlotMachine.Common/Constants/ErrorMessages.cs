namespace SlotMachine.Common.Constants;

public static class ErrorMessages
{
    public const string MissingConnectionString = "MongoDb:ConnectionString is missing.";
    public const string MissingDatabase = "MongoDb:Database is missing.";
    public const string ValidationFailMessage = "Validation failed.";
    public const string PlayerNotFoundMessage = "Player not found.";
    public const string InsufficientBalanceMessage = "Insufficient balance for this bet.";
    public const string SlotConfigurationNotFoundMessage = "Slot configuration is missing. Seed the 'config' collection.";
}
