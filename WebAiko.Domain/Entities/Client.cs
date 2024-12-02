namespace WebAiko.Domain.Entities;
    
public class Client
{
    public long ClientId { get; set; }
    public string Username { get; set; }
    public Guid SystemId { get; set; }
}
