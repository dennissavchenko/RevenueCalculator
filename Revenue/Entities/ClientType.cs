namespace Revenue.Entities;

public class ClientType
{
    public int ClientTypeId { get; set; }
    public string Name { get; set; }
    public virtual ICollection<Client> Clients { get; set; }
}