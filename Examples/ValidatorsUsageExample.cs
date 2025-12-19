using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using JapaneseTrainer.Api.Attributes;
using JapaneseTrainer.Api.Validators;

namespace JapaneseTrainer.Api.Examples
{
    /// <summary>
    /// Example DTOs showing how to use custom validators
    /// Similar to Python Pydantic validators
    /// </summary>
    public class ExampleRequestDto
    {
        /// <summary>
        /// Using custom ObjectId attribute (validates GUID format)
        /// </summary>
        [Required]
        [ObjectId]
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// Using custom EmailFormat attribute
        /// </summary>
        [Required]
        [EmailFormat]
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Using custom PhoneFormat attribute
        /// </summary>
        [PhoneFormat]
        public string? Phone { get; set; }

        /// <summary>
        /// Using custom DateFormat attribute
        /// </summary>
        [DateFormat]
        public string? BirthDate { get; set; }

        /// <summary>
        /// Using custom UrlFormat attribute
        /// </summary>
        [UrlFormat]
        public string? Website { get; set; }
    }

    /// <summary>
    /// Example controller showing manual validation
    /// </summary>
    [ApiController]
    [Route("v1/[controller]")]
    public class ValidatorExampleController : ControllerBase
    {
        [HttpPost("validate")]
        public IActionResult Validate([FromBody] ExampleRequestDto request)
        {
            // Manual validation using validators (similar to Python)
            try
            {
                var validatedId = CommonValidators.CheckObjectId(request.Id);
                var validatedEmail = CommonValidators.CheckEmail(request.Email);
                
                if (!string.IsNullOrWhiteSpace(request.Phone))
                {
                    var validatedPhone = CommonValidators.CheckPhone(request.Phone);
                }

                if (!string.IsNullOrWhiteSpace(request.BirthDate))
                {
                    var validatedDate = CommonValidators.CheckDateFormat(request.BirthDate);
                }

                if (!string.IsNullOrWhiteSpace(request.Website))
                {
                    var validatedUrl = CommonValidators.CheckUrl(request.Website);
                }

                return Ok(new { message = "All validations passed" });
            }
            catch (Exceptions.ErrorCodeException ex)
            {
                return BadRequest(new
                {
                    errorCode = ex.ErrorCode,
                    message = ex.Message,
                    details = ex.Details
                });
            }
        }
    }
}

