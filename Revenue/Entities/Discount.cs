namespace Revenue.Entities;

public class Discount
{
    public int IdDiscount { get; set; }
    public string Name { get; set; }
    public double Percentage { get; set; }
    public DateTime DateFrom { get; set; }
    public DateTime DateTo { get; set; }
    public virtual ICollection<Software> Softwares { get; set; }
}