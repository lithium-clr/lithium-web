namespace Lithium.Web;

public static class Sitemap
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class PriorityAttribute(double priority) : Attribute
    {
        public double Priority { get; } = priority;
    }

    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class ChangeFreqAttribute(SitemapChangeFreq changeFreq) : Attribute
    {
        public SitemapChangeFreq ChangeFreq { get; } = changeFreq;
    }   
    
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class LastModAttribute(int year, int month, int day) : Attribute
    {
        public int Year => year;
        public int Month => month;
        public int Day => day;

        public override string ToString() => $"{Year}-{Month:00}-{Day:00}";
    }
}

public enum SitemapChangeFreq
{
    Always,
    Hourly,
    Daily,
    Weekly,
    Monthly,
    Yearly,
    Never
}
