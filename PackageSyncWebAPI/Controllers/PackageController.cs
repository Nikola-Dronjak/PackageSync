using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PackageSync.Domain;
using PackageSyncWebAPI.Services;

namespace PackageSyncWebAPI.Controllers
{
    [ApiController]
    [Route("/api/packages/")]
    public class PackageController : ControllerBase
    {
        private readonly IValidator<Package> _validator;
        private readonly IPackageService _packageService;

        public PackageController(IValidator<Package> validator, IPackageService packageService)
        {
            _validator = validator;
            _packageService = packageService;
        }

        /// <summary>
        /// Fetches all the packages.
        /// </summary>
        /// <remarks>
        /// This endpoint allows clients to retrieve all the packages.
        /// </remarks>
        /// <response code="200">Packages retrieved successfully. Returns a list of all the retrieved packages.</response>
        /// <response code="500">An unexpected error occurred on the server.</response>
        [HttpGet]
        public async Task<IActionResult> GetAllPackages()
        {
            try
            {
                IEnumerable<Package> packages = await _packageService.GetAll();
                return Ok(packages.ToList());
            }
            catch (Exception exception)
            {
                return StatusCode(500, new
                {
                    title = "Internal server error",
                    details = exception.Message
                });
            }
        }

        /// <summary>
        /// Fetches a specific package.
        /// </summary>
        /// <remarks>
        /// This endpoint allows clients to retrieve a specific package by providing its id.
        /// </remarks>
        /// <param name="id">The id of the package that should be retrieved.</param>
        /// <response code="200">Package retrieved successfully. Returns the retrieved package.</response>
        /// <response code="404">The package with the given id was not found.</response>
        /// <response code="500">An unexpected error occurred on the server.</response>
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetPackageById(Guid id)
        {
            try
            {
                Package package = await _packageService.GetById(id);
                return Ok(package);
            }
            catch (KeyNotFoundException keyNotFoundException)
            {
                return NotFound(new
                {
                    title = "Item not found",
                    details = keyNotFoundException.Message
                });
            }
            catch (Exception exception)
            {
                return StatusCode(500, new
                {
                    title = "Internal server error",
                    details = exception.Message
                });
            }
        }

        /// <summary>
        /// Creates a new package.
        /// </summary>
        /// <remarks>
        /// This endpoint allows clients to create a new package by providing the necessary details in the request body.
        /// </remarks>
        /// <param name="package">The details of the package to create.</param>
        /// <response code="201">Package created successfully. Returns the newly created package.</response>
        /// <response code="400">Validation failed. Returns a list of validation errors.</response>
        /// <response code="500">An unexpected error occurred on the server.</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<IActionResult> AddPackage(Package package)
        {
            try
            {
                var validatorResult = _validator.Validate(package);
                if (!validatorResult.IsValid)
                {
                    var validationErrors = validatorResult.Errors.ToDictionary(e => e.PropertyName, e => e.ErrorMessage);
                    return BadRequest(new
                    {
                        title = "Validation error",
                        details = validationErrors
                    });
                }

                Package addedPackage = await _packageService.Add(package);
                return Created(string.Empty, addedPackage);
            }
            catch (Exception exception)
            {
                return StatusCode(500, new
                {
                    title = "Internal server error",
                    details = exception.Message
                });
            }
        }

        /// <summary>
        /// Updates an existing package.
        /// </summary>
        /// <remarks>
        /// This endpoint allows clients to update the information of an existing package by providing the necessary details in the request body.
        /// </remarks>
        /// <param name="id">The id of the package that should be updated.</param>
        /// <param name="package">The information that should be updated for the given package.</param>
        /// <response code="200">Package updated successfully. Returns the updated package.</response>
        /// <response code="400">Bad request (package not found, attempting to update a delivered package or validation errors). Returns the appropriate error message.</response>
        /// <response code="404">The package with the given id was not found.</response>
        /// <response code="500">An unexpected error occurred on the server.</response>
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdatePackage(Guid id, Package package)
        {
            try
            {
                var validatorResult = _validator.Validate(package);
                if (!validatorResult.IsValid)
                {
                    var validationErrors = validatorResult.Errors.ToDictionary(e => e.PropertyName, e => e.ErrorMessage);
                    return BadRequest(new
                    {
                        title = "Validation error",
                        details = validationErrors
                    });
                }

                Package updatedPackage = await _packageService.Update(id, package);
                return Ok(updatedPackage);
            }
            catch (KeyNotFoundException keyNotFoundException)
            {
                return NotFound(new
                {
                    title = "Item not found",
                    details = keyNotFoundException.Message
                });
            }
            catch (InvalidOperationException invalidOperationException)
            {
                return BadRequest(new
                {
                    title = "Invalid operation",
                    details = invalidOperationException.Message
                });
            }
            catch (ArgumentException argumentException)
            {
                return BadRequest(new
                {
                    title = "Invalid request",
                    details = argumentException.Message
                });
            }
            catch (Exception exception)
            {
                return StatusCode(500, new
                {
                    title = "Internal server error",
                    details = exception.Message
                });
            }
        }

        /// <summary>
        /// Removes an existing package.
        /// </summary>
        /// <remarks>
        /// This endpoint allows clients to remove an existing package by providing its id.
        /// In order to delete packages the user must be signed in.
        /// </remarks>
        /// <param name="id">The id of the package that should be removed.</param>
        /// <response code="200">Package removed successfully. Returns the removed package.</response>
        /// <response code="404">The package with the given id was not found.</response>
        /// <response code="500">An unexpected error occurred on the server.</response>
        [HttpDelete("{id:guid}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> DeletePackage(Guid id)
        {
            try
            {
                Package deletedPackage = await _packageService.Delete(id);
                return Ok(deletedPackage);
            }
            catch (KeyNotFoundException keyNotFoundException)
            {
                return NotFound(new
                {
                    title = "Item not found",
                    details = keyNotFoundException.Message
                });
            }
            catch (Exception exception)
            {
                return StatusCode(500, new
                {
                    title = "Internal server error",
                    details = exception.Message
                });
            }
        }
    }
}
