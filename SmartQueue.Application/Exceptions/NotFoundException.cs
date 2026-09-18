namespace SmartQueue.Application.Exceptions;

public class NotFoundException : Exception
{
    public NotFoundException(string name, string guid) : base($"\"{name}\" ({guid}) not found!") { }
    public NotFoundException(string name, int guid) : base($"\"{name}\" ({guid}) not found!") { }
    public NotFoundException(string message) : base(message) { }
}
