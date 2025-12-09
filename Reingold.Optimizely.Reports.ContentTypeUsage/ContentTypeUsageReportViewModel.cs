using EPiServer.DataAbstraction;

namespace Reingold.Optimizely.Reports.ContentTypeUsage;

public class ContentTypeUsageReportViewModel
{
    public bool Submitted { get; set; }
    public int SelectedId { get; set; }
    public IEnumerable<ContentType> ContentTypes { get; set; }
    public IEnumerable<ContentUsage> ContentTypeUsages { get; set; }
}