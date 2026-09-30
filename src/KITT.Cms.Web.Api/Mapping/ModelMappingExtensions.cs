using KITT.Cms.Web.Models;
using KITT.Core.Models;

namespace KITT.Cms.Web.Api.Mapping;

public static class ModelMappingExtensions
{
    extension(SeoData seo)
    {
        public Content.SeoData ToEntity()
            => new() 
            { 
                Title = seo.Title,
                Description = seo.Description,
                Keywords = seo.Keywords
            };
    }

    extension(Content.SeoData seo)
    {
        public SeoData ToModel()
            => new()
            {
                Title = seo.Title ?? string.Empty,
                Description = seo.Description ?? string.Empty,
                Keywords = seo.Keywords ?? string.Empty
            };
    }
}
