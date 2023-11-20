namespace RandomMessageApp.FunctionApp.Services.Interfaces;

public interface ITableStoragePrimaryKeyGenerator
{
    (string, string) Generate();
}
