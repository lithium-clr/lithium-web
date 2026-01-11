namespace Lithium.Web.Core.Utilities;

public static class Sitemap
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class PriorityAttribute(double priority) : Attribute
    {
        public double Priority { get; } = priority;
    }

    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class ChangeFrequencyAttribute(SitemapChangeFrequency changeFreq) : Attribute
    {
        public SitemapChangeFrequency ChangeFrequency { get; } = changeFreq;
    }

    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class LastModificationAttribute(int year, int month, int day) : Attribute
    {
        public int Year => year;
        public int Month => month;
        public int Day => day;

        public override string ToString() => $"{Year}-{Month:00}-{Day:00}";
    }
}