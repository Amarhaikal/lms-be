using LMS.Model.Common;
using Microsoft.AspNetCore.Mvc;

namespace LMS.Controllers.Common
{
    public class BaseApiController : ControllerBase
    {

        protected IActionResult CResponseRegisterSuccessful(string? message = null)
        {
            var response = new ApiResponse<string>
            {
                Status = 200,
                Message = message ?? "Registration successful",
            };
            return Ok(response);
        }

        protected IActionResult CResponseException(string message)
        {
            var response = new ApiResponse<string>
            {
                Status = 500,
                Message = $"An error occurred: {message}"
            };
            return StatusCode(500, response);
        }

        protected IActionResult CResponseGetListSuccessful<T>(List<T> data)
        {
            var response = new ApiResponse<List<T>>
            {
                Status = 200,
                Message = "Data retrieved successfully",
                Data = data
            };
            return Ok(response);
        }

        protected IActionResult CResponseGetListNotFound<T>(string message)
        {
            var response = new ApiResponse<List<T>>
            {
                Status = 404,
                Message = message,
                Data = null
            };
            return NotFound(response);
        }

        protected IActionResult CResponseInvalidDataToSave()
        {
            var response = new ApiResponse<string>
            {
                Status = 400,
                Message = "Invalid data to save"
            };
            return BadRequest(response);
        }

        protected IActionResult CResponseCreateSuccessful<T>(T data)
        {
            var response = new ApiResponse<T>
            {
                Status = 201,
                Message = "Data created successfully",
                Data = data
            };
            return Ok(response);
        }

        protected IActionResult CResponseUpdateSuccessful<T>(T data)
        {
            var response = new ApiResponse<T>
            {
                Status = 200,
                Message = "Data updated successfully",
                Data = data
            };
            return Ok(response);
        }

        protected IActionResult CResponseDeleteSuccessful()
        {
            var response = new ApiResponse<string>
            {
                Status = 200,
                Message = "Data deleted successfully"
            };
            return Ok(response);
        }

        protected IActionResult CResponseDeleteFailed()
        {
            var response = new ApiResponse<string>
            {
                Status = 400,
                Message = "Data deleted failed"
            };
            return BadRequest(response);
        }

        protected IActionResult CResponseGetSuccessful<T>(T data)
        {
            var response = new ApiResponse<T>
            {
                Status = 200,
                Message = "Data retrieved successfully",
                Data = data
            };
            return Ok(response);
        }

        protected IActionResult CResponseNotFound()
        {
            var response = new ApiResponse<string>
            {
                Status = 404,
                Message = "Data not found"
            };
            return NotFound(response);
        }

    }
}