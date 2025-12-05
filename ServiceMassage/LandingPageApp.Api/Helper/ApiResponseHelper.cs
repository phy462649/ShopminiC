using Microsoft.AspNetCore.Mvc;

namespace LandingPageApp.Api.Helper
{
    public static class ApiResponseHelper
    {
        public static ActionResult HandleGetAll<T>(IEnumerable<T>? data)
        {
            return (data == null || !data.Any())
                ? new NotFoundObjectResult("No data found.")
                : new OkObjectResult(data);
        }

        public static ActionResult HandleGetById<T>(T? data)
        {
            return (data == null)
               ? new NotFoundObjectResult("Item not found.")
               : new OkObjectResult(data);
        }

        public static ActionResult HandleCreate<T>(
            string actionName,
            object routeValues,
            T createdItem)
        {
            return new CreatedAtActionResult(actionName, null, routeValues, createdItem);
        }

        public static ActionResult HandleUpdate<T>(T? updated)
        {
            return (updated == null)
                ? new NotFoundObjectResult("Item not found.")
                : new OkObjectResult(updated);
        }

        public static ActionResult HandleDelete(bool isDeleted)
        {
            return (!isDeleted)
                ? new NotFoundObjectResult("Item not found.")
                : new OkObjectResult("Deleted successfully");
        }
    }

}
