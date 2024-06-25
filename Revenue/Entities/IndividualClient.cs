namespace Revenue.Entities;

public class IndividualClient : Client
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string PESEL { get; set; }
    public bool IsDeleted { get; set; }
}