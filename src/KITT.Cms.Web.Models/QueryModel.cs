using System.Reflection;
using System.Web;

namespace KITT.Cms.Web.Models;

public abstract record QueryModel
{
    //public string ToQueryString()
    //{
    //    var queryItems = GetType()
    //        .GetProperties(BindingFlags.Public | BindingFlags.Instance)
    //        .Select(p => (
    //            Name: p.GetCustomAttribute<FromQueryAttribute>()?.Name ?? p.Name.ToLowerInvariant(),
    //            Value: p.GetValue(this)
    //        ))
    //        .Where(x => x.Value is not null)
    //        .Select(x => $"{x.Name}={HttpUtility.UrlEncode(x.Value!.ToString())}");

    //    return string.Join("&", queryItems);
    //}
}
