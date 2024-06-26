namespace Revenue.Entities;

public class SoftwareCategory
{
    public int IdSoftwareCategory { get; set; }
    public string Name { get; set; }
    public virtual ICollection<Software> Softwares { get; set; }
}