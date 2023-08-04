using Microsoft.AspNetCore.Mvc;

namespace foodremedy.api.tests.Extensions;

public static class ActionResultExtensions
{
    public static TResult? Unpack<TResult>(this ActionResult<TResult> objectResult) where TResult : class
    {
        if (objectResult.Result is not OkObjectResult okObjectResult)
            throw new ArgumentException("Not of type OK object result");

        return okObjectResult.Value as TResult;
    }
}
