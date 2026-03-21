using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BMS.Application.Models;

public class ApiResponse<T>
{
    public bool Success { get; set; }

    public string Message { get; set; }

    public T Data { get; set; }

    public List<string> Errors { get; set; } = new();

    public int StatusCode { get; set; }

    public static ApiResponse<T> SuccessResponse(T data, string message = "")
    {
        return new ApiResponse<T>
        {
            Success = true,
            Message = message,
            Data = data,
            StatusCode = 200
        };
    }

    public static ApiResponse<T> FailResponse(List<string> errors, string message = "", int statusCode = 400)
    {
        return new ApiResponse<T>
        {
            Success = false,
            Message = message,
            Errors = errors,
            StatusCode = statusCode
        };
    }
}
