using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using POCWebAppAssignment.API.Utilities;
using POCWebAppAssignment.Interfaces;
using POCWebAppAssignment.Model;
using POCWebAppAssignment.Model.DTOs;

namespace POCWebAppAssignment.API.Controllers
{
    [Route("api/resources")]
    [ApiController]
    public class ResourceController : ControllerBase
    {
        private readonly IResourceService _resourceService;
        private readonly ILogger<ResourceController> _logger;

        // Constructor Injection
        public ResourceController(IResourceService resourceService, ILogger<ResourceController> logger)
        {
            _resourceService = resourceService;
            _logger = logger;
        }

        // GET
        [HttpGet("get-all-resources")]
        public async Task<ActionResult<ApiResponse<IEnumerable<Resource>>>> GetAllResourcesAsync()
        {
            _logger.LogInformation("Request received: Fetch all resources.");

            try
            {
                var allResources = await _resourceService.GetAllResourcesAsync();

                //if (allResources == null || !allResources.Any())
                //{
                //    _logger.LogWarning("No resources found in the system.");
                //    return NotFound(new ApiResponse<string>(false, "No resources found.", null));
                //}

                _logger.LogInformation("Successfully fetched {Count} resources.", allResources.Count());
                var response = new ApiResponse<IEnumerable<Resource>>(true, "Resources retrieved successfully.", allResources);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred while retrieving resources.");
                var response = new ApiResponse<string>(false, "An internal error occurred while fetching resources.", null);
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
        }



        // GET
        [HttpGet("get-resource-id/{EmpId}")]
        public async Task<ActionResult<ApiResponse<ResourceDetailsDto?>>> GetResourceByIdAsync(int EmpId)
        {
            _logger.LogInformation("Request received: Fetch resource by ID: {EmpId}", EmpId);

            if (EmpId <= 0)
            {
                _logger.LogWarning("Invalid EmpId provided: {EmpId}", EmpId);
                return BadRequest(new ApiResponse<string>(false, "Invalid resource ID provided.", null));
            }

            try
            {
                var resource = await _resourceService.GetResourceByIdAsync(EmpId);

                if (resource == null)
                {
                    _logger.LogWarning("Resource not found for ID: {EmpId}", EmpId);
                    return NotFound(new ApiResponse<string>(false, $"No resource found with ID {EmpId}.", null));
                }

                _logger.LogInformation("Successfully retrieved resource for ID: {EmpId}", EmpId);
                var response = new ApiResponse<ResourceDetailsDto>(true, "Resource retrieved successfully.", resource);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception occurred while fetching resource for ID: {EmpId}", EmpId);
                var response = new ApiResponse<string>(false, "An internal server error occurred while retrieving the resource.", null);
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
        }


        // POST
        [HttpPost("create-new-resource")]
        public async Task<ActionResult<ApiResponse<string>>> CreateResourceAsync([FromBody] ResourceDto resource)
        {
            _logger.LogInformation("Request received: Create new resource.");

            if (resource == null)
            {
                _logger.LogWarning("CreateResourceAsync: Received null resource payload.");
                return BadRequest(new ApiResponse<string>(false, "Invalid request: Resource data is required.", null));
            }

            try
            {
                int empId = await _resourceService.CreateResourceAsync(resource);

                _logger.LogInformation("Resource created successfully. Assigned EmpId: {EmpId}", empId);

                var response = new ApiResponse<string>(true, "Resource created successfully.", empId.ToString());
                return StatusCode(StatusCodes.Status201Created, response);
            }
            catch (SqlException ex) when (ex.Number == 2627) // Unique constraint violation
            {
                _logger.LogWarning("CreateResourceAsync: Duplicate entry violation for email ID: {EmailId}", resource.EmailId);
                return Conflict(new ApiResponse<string>(false, "A resource with this email ID already exists.", null));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CreateResourceAsync: An unexpected error occurred.");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new ApiResponse<string>(false, "An error occurred while creating the resource.", null));
            }
        }


        // PUT
        [HttpPut("update-resource")]
        public async Task<ActionResult<ApiResponse<string>>> UpdateResourceAsync([FromBody] ResourceDto resource)
        {
            _logger.LogInformation("Request received: Update resource. EmpId: {EmpId}", resource?.EmpId);

            if (resource == null || resource.EmpId <= 0)
            {
                _logger.LogWarning("UpdateResourceAsync: Invalid resource data provided.");
                return BadRequest(new ApiResponse<string>(false, "Invalid request: Resource data is missing or invalid.", null));
            }

            try
            {
                await _resourceService.UpdateResourceAsync(resource);

                _logger.LogInformation("Resource updated successfully. EmpId: {EmpId}", resource.EmpId);

                var response = new ApiResponse<string>(true, "Resource updated successfully.", null);
                return Ok(response);
            }
            catch (KeyNotFoundException)
            {
                _logger.LogWarning("UpdateResourceAsync: Resource not found. EmpId: {EmpId}", resource.EmpId);
                return NotFound(new ApiResponse<string>(false, $"No resource found with ID {resource.EmpId}.", null));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "UpdateResourceAsync: Error while updating resource. EmpId: {EmpId}", resource.EmpId);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new ApiResponse<string>(false, "An error occurred while updating the resource.", null));
            }
        }


        // DELETE
        [HttpDelete("delete-resource/{EmpId}")]
        public async Task<ActionResult<ApiResponse<string>>> DeleteResourceAsync(int EmpId)
        {
            _logger.LogInformation("Request received: Delete resource by ID: {EmpId}", EmpId);

            if (EmpId <= 0)
            {
                _logger.LogWarning("DeleteResourceAsync: Invalid EmpId provided: {EmpId}", EmpId);
                return BadRequest(new ApiResponse<string>(false, "Invalid resource ID.", null));
            }

            try
            {
                await _resourceService.DeleteResourceAsync(EmpId);

                _logger.LogInformation("Resource deleted successfully. EmpId: {EmpId}", EmpId);
                return Ok(new ApiResponse<string>(true, "Resource deleted successfully.", null));
            }
            catch (KeyNotFoundException)
            {
                _logger.LogWarning("DeleteResourceAsync: Resource not found. EmpId: {EmpId}", EmpId);
                return NotFound(new ApiResponse<string>(false, $"No resource found with ID {EmpId}.", null));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "DeleteResourceAsync: Unexpected error occurred. EmpId: {EmpId}", EmpId);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new ApiResponse<string>(false, "An error occurred while deleting the resource.", null));
            }
        }


        // Delete Multiple
        [HttpDelete("delete-multiple-resources")]
        public async Task<ActionResult<ApiResponse<string>>> DeleteResourceByEmpIdsAsync([FromQuery] List<int> EmpIds)
        {
            _logger.LogInformation("Request received: Bulk delete for EmpIds: {EmpIds}", string.Join(", ", EmpIds));

            if (EmpIds == null || !EmpIds.Any())
            {
                _logger.LogWarning("DeleteResourceByEmpIdsAsync: No EmpIds provided for bulk deletion.");
                return BadRequest(new ApiResponse<string>(false, "At least one EmpId is required for deletion.", null));
            }

            try
            {
                await _resourceService.DeleteResourcesByEmpIdListAsync(EmpIds);

                _logger.LogInformation("Successfully deleted {Count} resources.", EmpIds.Count);
                return Ok(new ApiResponse<string>(true, "Selected resources deleted successfully.", null));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "DeleteResourceByEmpIdsAsync: Error occurred during bulk deletion.");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new ApiResponse<string>(false, "An error occurred while deleting multiple resources.", null));
            }
        }


        // GET
        [HttpGet("get-resource-statistics")]
        public async Task<ActionResult<ApiResponse<IEnumerable<int>>>> GetResourceStatisticsAsync()
        {
            _logger.LogInformation("Request received: Fetch resource statistics.");

            try
            {
                var result = await _resourceService.GetResourceStatisticsAsync();

                if (result == null || !result.Any())
                {
                    _logger.LogWarning("No resource statistics available.");
                    return NotFound(new ApiResponse<string>(false, "No resource statistics found.", null));
                }

                _logger.LogInformation("Resource statistics retrieved successfully. Count: {Count}", result.Count());
                return Ok(new ApiResponse<IEnumerable<int>>(true, "Resource statistics retrieved successfully.", result));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching resource statistics.");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new ApiResponse<string>(false, "An internal error occurred while retrieving statistics.", null));
            }
        }


        [HttpGet("check-email-exist")]
        public async Task<IActionResult> CheckIfEmailExistsAsync([FromQuery] string emailId)
        {
            _logger.LogInformation("Request received: Check if email exists.");

            if (string.IsNullOrWhiteSpace(emailId))
            {
                _logger.LogWarning("CheckIfEmailExistsAsync: Email ID is null or empty.");
                return BadRequest(new ApiResponse<string>(false, "Email ID is required.", null));
            }

            try
            {
                bool doesExist = await _resourceService.CheckEmailExistsAsync(emailId);

                _logger.LogInformation("Email existence check completed for '{EmailId}': Exists = {Exists}", emailId, doesExist);

                return Ok(new ApiResponse<bool>(true, "Email existence check completed.", doesExist));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while checking email existence for: {EmailId}", emailId);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new ApiResponse<string>(false, "An error occurred while checking the email address.", null));
            }
        }


        [HttpGet("dropdown-options")]
        public async Task<IActionResult> GetDropdownOptionsAsync()
        {
            _logger.LogInformation("Request received: Fetch dropdown options.");

            try
            {
                var dropdownData = await _resourceService.GetDropdownDataAsync();

                if (dropdownData == null)
                {
                    _logger.LogWarning("Dropdown data not found or empty.");
                    return NotFound(new ApiResponse<string>(false, "No dropdown data available.", null));
                }

                _logger.LogInformation("Dropdown options retrieved successfully.");
                return Ok(new ApiResponse<DropdownResponseDto>(true, "Dropdown options retrieved successfully.", dropdownData));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving dropdown options.");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new ApiResponse<string>(false, "An error occurred while fetching dropdown options.", null));
            }
        }


        [HttpPost("bulk-import-data")]
        public async Task<IActionResult> ImportExcelDataAsync([FromBody] List<ResourceDto> dataList)
        {
            _logger.LogInformation("Request received: Bulk import of resource data. Total records: {Count}", dataList?.Count ?? 0);

            if (dataList == null || !dataList.Any())
            {
                _logger.LogWarning("ImportExcelDataAsync: No data received for import.");
                return BadRequest(new ApiResponse<string>(false, "No data received for import.", null));
            }

            try
            {
                await _resourceService.BulkCreateResourcesAsync(dataList);

                _logger.LogInformation("ImportExcelDataAsync: Successfully imported {Count} resources.", dataList.Count);

                return Ok(new ApiResponse<string>(true, "Bulk import completed successfully.", null));
            }
            catch (SqlException ex) when (ex.Number == 2627)
            {
                _logger.LogWarning("ImportExcelDataAsync: Duplicate email detected during bulk import.");
                return Conflict(new ApiResponse<string>(false, "Duplicate email detected in import data.", null));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ImportExcelDataAsync: Unexpected error occurred during bulk import.");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new ApiResponse<string>(false, "An internal error occurred during bulk import.", null));
            }
        }


        // PUT: api/resource/bulk-edit
        [HttpPut("bulk-edit")]
        public async Task<ActionResult<ApiResponse<string>>> BulkUpdateResourcesAsync([FromBody] BulkEditDto bulkEditDto)
        {
            _logger.LogInformation("Request received: Bulk update for {Count} resources.", bulkEditDto?.ResourceIds?.Count ?? 0);

            if (bulkEditDto == null || bulkEditDto.ResourceIds == null || !bulkEditDto.ResourceIds.Any())
            {
                _logger.LogWarning("BulkUpdateResourcesAsync: Invalid request. ResourceIds list is null or empty.");
                return BadRequest(new ApiResponse<string>(false, "Invalid request: ResourceIds are required.", null));
            }

            try
            {
                int updatedCount = await _resourceService.BulkUpdateResourcesAsync(bulkEditDto);

                _logger.LogInformation("BulkUpdateResourcesAsync: Successfully updated {Count} resources.", updatedCount);

                return Ok(new ApiResponse<string>(true, $"Successfully updated {updatedCount} resource(s).", null));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "BulkUpdateResourcesAsync: Error occurred while updating resources.");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new ApiResponse<string>(false, "An error occurred during bulk update.", null));
            }
        }


        [HttpGet("role-options")]
        public async Task<IActionResult> GetRoleOptions()
        {
            _logger.LogInformation("Request received: Fetch role dropdown options.");

            try
            {
                var result = await _resourceService.GetRoleOptionsDropDownAsync();

                if (result == null || !result.Any())
                {
                    _logger.LogWarning("GetRoleOptions: No role options found.");
                    return NotFound(new ApiResponse<string>(false, "No role options available.", null));
                }

                _logger.LogInformation("GetRoleOptions: Successfully retrieved {Count} role options.", result.Count);
                return Ok(new ApiResponse<List<OptionDto>>(true, "Role options retrieved successfully.", result));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetRoleOptions: Error occurred while retrieving role options.");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new ApiResponse<string>(false, "An error occurred while fetching role options.", null));
            }
        }



    }
}
