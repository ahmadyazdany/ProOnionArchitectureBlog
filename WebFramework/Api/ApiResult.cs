﻿using Common.ApiResult;
using Common.Utilities;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace WebFramework.Api;

public class ApiResult
{
    public bool IsSuccess { get; set; }
    public ApiResultStatusCode StatusCode { get; set; }

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public string? Message { get; set; }

    public ApiResult(bool isSuccess, ApiResultStatusCode statusCode, string? message = null)
    {
        IsSuccess = isSuccess;
        StatusCode = statusCode;
        Message = message ?? statusCode.ToDisplay(DisplayProperty.Name);
    }

    #region Implicit Operator

    public static implicit operator ApiResult(OkResult result)
        => new ApiResult(true, ApiResultStatusCode.Success);

    public static implicit operator ApiResult(BadRequestResult result)
        => new ApiResult(false, ApiResultStatusCode.BadRequest);

    public static implicit operator ApiResult(BadRequestObjectResult result)
    {
        var message = result.Value.ToString();
        if (result.Value is SerializableError errors)
        {
            var errorMessages = errors.SelectMany(p => (string[])p.Value).Distinct();
            message = string.Join(" | ", errorMessages);
        }

        return new ApiResult(false, ApiResultStatusCode.BadRequest, message);
    }

    public static implicit operator ApiResult(ContentResult result)
        => new ApiResult(true, ApiResultStatusCode.Success, result.Content);

    public static implicit operator ApiResult(NotFoundResult result)
        => new ApiResult(false, ApiResultStatusCode.NotFound);

    #endregion Implicit Operator
}

public class ApiResult<TData> : ApiResult where TData : class
{
    public ApiResult(bool isSuccess, ApiResultStatusCode statusCode, TData data, string? message = null)
        : base(isSuccess, statusCode, message)
    {
        Data = data;
    }

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public TData Data { get; set; }


    #region Implicit Operator

    public static implicit operator ApiResult<TData>(TData data)
        => new ApiResult<TData>(true, ApiResultStatusCode.Success, data);

    public static implicit operator ApiResult<TData>(OkResult result)
        => new ApiResult<TData>(true, ApiResultStatusCode.Success, null);

    public static implicit operator ApiResult<TData>(OkObjectResult result)
        => new ApiResult<TData>(true, ApiResultStatusCode.Success, (TData)result.Value);

    public static implicit operator ApiResult<TData>(BadRequestResult result)
        => new ApiResult<TData>(false, ApiResultStatusCode.BadRequest, null);

    public static implicit operator ApiResult<TData>(BadRequestObjectResult result)
    {
        var message = result.Value.ToString();
        if (result.Value is SerializableError errors)
        {
            var errorMessages = errors.SelectMany(p => (string[])p.Value).Distinct();
            message = string.Join(" | ", errorMessages);
        }

        return new ApiResult<TData>(false, ApiResultStatusCode.BadRequest, null, message);
    }

    public static implicit operator ApiResult<TData>(ContentResult result)
        => new ApiResult<TData>(true, ApiResultStatusCode.Success, null, result.Content);

    public static implicit operator ApiResult<TData>(NotFoundResult result)
        => new ApiResult<TData>(false, ApiResultStatusCode.NotFound, null);

    public static implicit operator ApiResult<TData>(NotFoundObjectResult result)
    => new ApiResult<TData>(false, ApiResultStatusCode.NotFound, (TData)result.Value);

    #endregion Implicit Operator
}
