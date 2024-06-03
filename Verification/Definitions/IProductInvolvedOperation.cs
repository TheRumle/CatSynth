namespace Cat.Verify.Definitions;

public interface IProductInvolvedOperation : ICatOperation
{
    public IEnumerable<string> ProductIds { get; }
}