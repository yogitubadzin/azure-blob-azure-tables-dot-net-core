namespace RandomMessageApp.Core.CommonServices.Interfaces;

public interface IPartitionKeyGenerator
{
    string Generate(DateTime dateTime);
}
