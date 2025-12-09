using EPiServer.Authorization;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Reingold.Optimizely.Reports.ContentTypeUsage;

[Authorize(Roles = $"{Roles.WebAdmins}, {Roles.WebEditors}")]
[Route(Constants.Route + "/{action=Index}")]
public class ContentTypeUsageReportController : Controller
{
    private const string VIEW = "/Reingold.Optimizely.Reports.ContentTypeUsage/Views/Index.cshtml";
    private readonly IContentTypeRepository contentTypeRepository;
    private readonly IContentModelUsage contentModelUsage;

    public ContentTypeUsageReportController(IContentTypeRepository contentTypeRepository, IContentModelUsage contentModelUsage)
    {
        this.contentTypeRepository = contentTypeRepository;
        this.contentModelUsage = contentModelUsage;
    }

    public ActionResult Index()
    {
        var model = new ContentTypeUsageReportViewModel();

        model.ContentTypes = GetContentTypes();

        return View(VIEW, model);
    }

    [HttpPost]
    public ActionResult Index(int contentTypeId)
    {
        var model = new ContentTypeUsageReportViewModel();
        model.ContentTypes = GetContentTypes();

        if (contentTypeId > 0)
        {
            model.Submitted = true;
            model.SelectedId = contentTypeId;

            var contentType = contentTypeRepository.Load(contentTypeId);
            //get content usages, grouping by content id + language to prevent duplicate entries from old content versions
            model.ContentTypeUsages = contentModelUsage.ListContentOfContentType(contentType)
                .GroupBy(usage => usage.ContentLink.ID + usage.LanguageBranch)
                .Select(group => group.First())
                .OrderBy(x => x.Name);
        }

        return View(VIEW, model);
    }

    private IEnumerable<ContentType> GetContentTypes()
    {
        var contentTypes = contentTypeRepository.List();
        return contentTypes
            .Where(x => x.ModelType != null && contentModelUsage.IsContentTypeUsed(x))
            .OrderBy(x => x.Name);
    }
}