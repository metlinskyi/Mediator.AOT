using Microsoft.AspNetCore.Http;

namespace Happy.Endpoint.Interfaces;

public partial interface IHappy 
{
   internal interface MiddlewareService 
   {
      Task Send((HttpMethod, string) key, HttpContext ctx, CancellationToken ct);
   }
}