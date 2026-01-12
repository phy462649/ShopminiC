using LandingPageApp.Application.Common;
using Microsoft.AspNetCore.Mvc;

namespace LandingPageApp.Api.Controllers;

/// <summary>
/// Base controller with common response methods
/// </summary>
[ApiController]
public abstract class BaseApiController : ControllerBase
{
    /// <summary>
    /// Returns success response with data
    /// </summary>
    protected ActionResult<ApiResponseWrapper<T>> OkResponse<T>(T data, string message = "Success")
        => Ok(ApiResponseWrapper<T>.Success(data, message));

    /// <summary>
    /// Returns success response with data and pagination
    /// </summary>
    protected ActionResult<ApiResponseWrapper<IReadOnlyList<T>>> OkPagedResponse<T>(PagedResult<T> result, string message = "Success")
        => Ok(ApiResponseWrapper<IReadOnlyList<T>>.Success(
            result.Items,
            PaginationMeta.FromPagedResult(result),
            message));

    /// <summary>
    /// Returns created response
    /// </summary>
    protected ActionResult<ApiResponseWrapper<T>> CreatedResponse<T>(T data, string actionName, object routeValues, string message = "Created successfully")
        => CreatedAtAction(actionName, routeValues, ApiResponseWrapper<T>.Success(data, message));

    /// <summary>
    /// Returns error response
    /// </summary>
    protected ActionResult<ApiResponseWrapper<T>> ErrorResponse<T>(string message, int statusCode = 400)
    {
        Response.StatusCode = statusCode;
        return new ObjectResult(ApiResponseWrapper<T>.Fail(message)) { StatusCode = statusCode };
    }

    /// <summary>
    /// Returns not found response
    /// </summary>
    protected ActionResult<ApiResponseWrapper<T>> NotFoundResponse<T>(string message)
        => NotFound(ApiResponseWrapper<T>.Fail(message));

    /// <summary>
    /// Handle Result pattern
    /// </summary>
    protected ActionResult<ApiResponseWrapper<T>> FromResult<T>(Result<T> result, string successMessage = "Success")
    {
        if (result.IsSuccess)
            return OkResponse(result.Value!, successMessage);

        return result.ErrorCode switch
        {
            "NOT_FOUND" => NotFoundResponse<T>(result.Error),
            "VALIDATION_ERROR" => ErrorResponse<T>(result.Error, 400),
            "CONFLICT" => ErrorResponse<T>(result.Error, 409),
            "FORBIDDEN" => ErrorResponse<T>(result.Error, 403),
            _ => ErrorResponse<T>(result.Error, 400)
        };
    }
}
