using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using PackageSyncWebAPI.Models;
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

        [HttpGet]
        public async Task<IActionResult> GetAllPackages()
        {
            try
            {
                IEnumerable<Package> packages = await _packageService.GetAll();
                return Ok(packages.ToList());
            }
            catch (Exception)
            {
                return StatusCode(500, "Internal server error. Try again later.");
            }
        }

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
                return NotFound(keyNotFoundException.Message);
            }
            catch (Exception)
            {
                return StatusCode(500, "Internal server error. Try again later.");
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddPackage(Package package)
        {
            try
            {
                var validatorResult = _validator.Validate(package);
                if (!validatorResult.IsValid)
                {
                    var validationErrors = validatorResult.Errors.GroupBy(e => e.PropertyName).ToDictionary(g => g.Key,g => g.Select(e => e.ErrorMessage).ToArray());
                    return BadRequest(validationErrors);
                }

                Package addedPackage = await _packageService.Add(package);
                return Created(string.Empty, addedPackage);
            }
            catch (Exception)
            {
                return StatusCode(500, "Internal server error. Try again later.");
            }
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdatePackage(Guid id, Package package)
        {
            try
            {
                var validatorResult = _validator.Validate(package);
                if (!validatorResult.IsValid)
                {
                    var validationErrors = validatorResult.Errors.GroupBy(e => e.PropertyName).ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());
                    return BadRequest(validationErrors);
                }

                Package updatedPackage = await _packageService.Update(id, package);
                return Ok(updatedPackage);
            }
            catch (KeyNotFoundException keyNotFoundException)
            {
                return NotFound(keyNotFoundException.Message);
            }
            catch (InvalidOperationException invalidOperationException)
            {
                return BadRequest(invalidOperationException.Message);
            }
            catch (ArgumentException argumentException)
            {
                return BadRequest(argumentException.Message);
            }
            catch (Exception)
            {
                return StatusCode(500, "Internal server error. Try again later.");
            }
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeletePackage(Guid id)
        {
            try
            {
                Package deletedPackage = await _packageService.Delete(id);
                return Ok(deletedPackage);
            }
            catch (KeyNotFoundException keyNotFoundException)
            {
                return NotFound(keyNotFoundException.Message);
            }
            catch (Exception)
            {
                return StatusCode(500, "Internal server error. Try again later.");
            }
        }
    }
}
