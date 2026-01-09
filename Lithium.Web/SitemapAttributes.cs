namespace Lithium.Web;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class SitemapPriorityAttribute(double priority) : Attribute
{
    public double Priority { get; } = priority;
}