namespace TesteVeste.Domain.Shared;

public class CommandResult<T>
{
    public bool Succeeded { get; private set; }
    public T? Data { get; private set; }
    public IReadOnlyList<string> Messages { get; private set; } = new List<string>();

    private CommandResult() { }

    public static CommandResult<T> Success(T data) => new()
    {
        Succeeded = true,
        Data = data,
        Messages = new List<string>().AsReadOnly()
    };

    public static CommandResult<T> Failure(IEnumerable<string> messages) => new()
    {
        Succeeded = false,
        Data = default,
        Messages = messages.ToList().AsReadOnly()
    };
}
