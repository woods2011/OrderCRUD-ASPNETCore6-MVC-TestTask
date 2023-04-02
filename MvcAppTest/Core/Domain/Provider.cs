namespace MvcAppTest.Core.Domain;

public class Provider
{
    public int Id { get; private set; }

    public string Name { get; set; }

    public Provider(string name) => Name = name;

    
#pragma warning disable CS8618
    private Provider() { }
#pragma warning restore CS8618
}