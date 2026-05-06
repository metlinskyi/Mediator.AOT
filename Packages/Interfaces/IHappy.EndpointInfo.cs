using Microsoft.AspNetCore.Authorization;

namespace Happy.Endpoint.Interfaces;

public partial interface IHappy
{
   public interface EndpointInfo 
   {
      HttpMethod Method { get; }
      Type RequestType { get; }
      Type? ResponseType { get; }
      IAuthorizeData[] AuthorizeData { get; }
   }
}