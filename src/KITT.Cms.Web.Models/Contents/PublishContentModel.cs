using System.ComponentModel.DataAnnotations;

namespace KITT.Cms.Web.Models.Contents;

public record PublishContentModel
{
    [Required]
    public DateTime? PublicationDate { get; set; } = DateTime.Now;
}
