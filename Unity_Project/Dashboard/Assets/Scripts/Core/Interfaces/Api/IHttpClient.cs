using System.Collections;
using Core.Models;

namespace Core.Interfaces.Api
{
    public interface IHttpClient
    {
        IEnumerator SendRequest<T>(HttpRequest<T> request);
    }
}