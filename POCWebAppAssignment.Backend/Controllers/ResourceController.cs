using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using POCWebAppAssignment.API.Utilities;
using POCWebAppAssignment.Interfaces;
using POCWebAppAssignment.Model;
using POCWebAppAssignment.Model.DTOs;
using POCWebAppAssignment.Repository.RunStoredProcedures;

namespace POCWebAppAssignment.API.Controllers
{
    [Route("resources")]
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

        // GET: api/<ValuesController>
        [HttpGet("get-all-resources")]
        public async Task<ActionResult<ApiResponse<IEnumerable<Resource>>>> GetAllResourcesAsync()
        {
            _logger.LogInformation("Fetching all resources.");
            try
            {
                var allResources = await _resourceService.GetAllResourcesAsync();
                var response = new ApiResponse<IEnumerable<Resource>>(true, "Resource Data", allResources);

                _logger.LogInformation("Retrieved {Count} resources successfully.", allResources.Count());
                return Ok(response);
            }
            catch (Exception ex) {
                _logger.LogError(ex, "Error occurred while fetching all resources.");
                var response = new ApiResponse<string>(false, ex.Message, null);
                return StatusCode(500, response);
            }
        }

        // GET
        [HttpGet("get-resource-id/{EmpId}")]
        public async Task<ActionResult<ApiResponse<ResourceDetailsDto?>>> GetResourceByIdAsync(int EmpId)
        {
            _logger.LogInformation("Fetching resource with ID: {ResourceId}", EmpId);

            try
            {
                var resource = await _resourceService.GetResourceByIdAsync(EmpId);

                if (resource == null)
                {
                    _logger.LogWarning("Resource with ID: {ResourceId} not found.", EmpId);
                }
                else
                {
                    _logger.LogInformation("Resource with ID: {ResourceId} retrieved successfully.", EmpId);
                }

                var response = new ApiResponse<ResourceDetailsDto>(true, "Resource Data", resource);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching resource with ID: {ResourceId}", EmpId);

                var response = new ApiResponse<string>(false, ex.Message, null);
                return StatusCode(500, response);
            }
        }

        // POST
        [HttpPost("create-new-resource")]
        public async Task<ActionResult<ApiResponse<string>>> CreateResourceAsync([FromBody] ResourceDto resource)
        {
            _logger.LogInformation("Creating a new resource.");

            try
            {
                Console.WriteLine("request made");
                int EmpId = await _resourceService.CreateResourceAsync(resource);

                _logger.LogInformation("Resource created successfully with ID: {ResourceId}", EmpId);

                var response = new ApiResponse<string>(true, EmpId.ToString(), null);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating a new resource.");

                var response = new ApiResponse<string>(false, ex.Message, null);
                return StatusCode(500, response);
            }
        }

        // PUT
        [HttpPut("update-resource")]
        public async Task<ActionResult<ApiResponse<string>>> UpdateResourceAsync([FromBody] ResourceDto resource)
        {
            _logger.LogInformation("Updating resource with ID: {ResourceId}", resource?.EmpId);

            try
            {
                await _resourceService.UpdateResourceAsync(resource);
                _logger.LogInformation("Resource with ID: {ResourceId} updated successfully.", resource?.EmpId);

                var response = new ApiResponse<string>(true, "Resource Updated Successfully!", null);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating resource with ID: {ResourceId}", resource?.EmpId);

                var response = new ApiResponse<string>(false, ex.Message, null);
                return StatusCode(500, response);
            }
        }

        // DELETE
        [HttpDelete("delete-resource/{EmpId}")]
        public async Task<ActionResult<ApiResponse<string>>> DeleteResourceAsync(int EmpId)
        {
            _logger.LogInformation("Deleting resource with ID: {ResourceId}", EmpId);

            try
            {
                await _resourceService.DeleteResourceAsync(EmpId);
                _logger.LogInformation("Resource with ID: {ResourceId} deleted successfully.", EmpId);

                var response = new ApiResponse<string>(true, "Resource Deleted Successfully!", null);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting resource with ID: {ResourceId}", EmpId);

                var response = new ApiResponse<string>(false, ex.Message, null);
                return StatusCode(500, response);
            }
        }

        // Delete Multiple
        [HttpDelete("delete-multiple-resources")]
        public async Task<ActionResult<ApiResponse<string>>> DeleteResourceByEmpIdsAsync([FromQuery] List<int> EmpIds)
        {
            _logger.LogInformation("Deleting resources with IDs: {EmpIds}", EmpIds);

            try
            {
                await _resourceService.DeleteResourcesByEmpIdListAsync(EmpIds);
                _logger.LogInformation("Resource with ID: {EmpIds} deleted successfully.", EmpIds);

                var response = new ApiResponse<string>(true, "Resource Deleted Successfully!", null);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting resource with IDs: {EmpIds}", EmpIds);

                var response = new ApiResponse<string>(false, ex.Message, null);
                return StatusCode(500, response);
            }
        }

        // GET
        [HttpGet("get-resource-statistics")]
        public async Task<ActionResult<ApiResponse<string>>> GetResourceStatisticsAsync()
        {
            _logger.LogInformation("Fetching resource statistics.");

            try
            {
                Console.WriteLine("request made");
                var result = await _resourceService.GetResourceStatisticsAsync();
                _logger.LogInformation("Resource statistics fetched successfully.");

                var response = new ApiResponse<IEnumerable<int>>(true, "Resource Statistics", result);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching resource statistics.");
                var response = new ApiResponse<string>(false, ex.Message, null);
                return StatusCode(500, response);
            }
        }

        [HttpGet("check-email-exist")]
        public async Task<IActionResult> CheckIfEmailExistsAsync([FromQuery] string emailId)
        {
            _logger.LogInformation("Checking if emailId exists: {EmailId}", emailId);

            try
            {
                bool doesExist = await _resourceService.CheckEmailExistsAsync(emailId);

                if (doesExist)
                {
                    _logger.LogWarning("EmailId already exists: {EmailId}", emailId);
                }
                else
                {
                    _logger.LogInformation("EmailId does not exist: {EmailId}", emailId);
                }

                var response = new ApiResponse<bool>(true, "Exists boolean:", doesExist);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while checking if emailId exists: {EmailId}", emailId);

                var response = new ApiResponse<string>(false, ex.Message, null);
                return StatusCode(500, response);
            }
        }

        [HttpGet("dropdown-options")]
        public async Task<IActionResult> GetDropdownOptionsAsync()
        {
            _logger.LogInformation("Fetching dropdown options");

            try
            {
                var dropdownData = await _resourceService.GetDropdownDataAsync();
                var response = new ApiResponse<DropdownResponseDto>(true, "Dropdown options retrieved successfully", dropdownData);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching dropdown options");

                var response = new ApiResponse<string>(false, ex.Message, null);
                return StatusCode(500, response);
            }
        }

        [HttpPost("bulk-import-data")]
        public async Task<IActionResult> ImportExcelDataAsync([FromBody] List<ResourceDto> dataList)
        {
            if (dataList == null || !dataList.Any())
            {
                _logger.LogWarning("ImportExcelDataAsync: Received empty or null data list.");
                return BadRequest(new ApiResponse<string>(false, "No data received for import.", null));
            }

            _logger.LogInformation("ImportExcelDataAsync: Starting Excel data import. Total records: {Count}", dataList.Count);

            try
            {
                var empIdList = new List<int>();

                await _resourceService.BulkCreateResourcesAsync(dataList);

                _logger.LogInformation("ImportExcelDataAsync: Successfully imported {Count} resources.", empIdList.Count);

                var response = new ApiResponse<List<int>>(true, "Excel import successful", empIdList);
                return Ok(response);
            }
            catch (SqlException ex) when (ex.Number == 2627)
            {
                // 2627 = UNIQUE constraint violation
                return StatusCode(500, new ApiResponse<string>(false,"Duplicate emailId found.", null));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ImportExcelDataAsync: Error occurred during Excel import.");
                var response = new ApiResponse<string>(false, "An error occurred while importing data: " + ex.Message , null);
                return StatusCode(500, response);
            }
        }

        // PUT: api/resource/bulk-edit
        [HttpPut("bulk-edit")]
        public async Task<ActionResult<ApiResponse<string>>> BulkUpdateResourcesAsync([FromBody] BulkEditDto bulkEditDto)
        {
            _logger.LogInformation("Initiating bulk update for {Count} resources.", bulkEditDto?.ResourceIds?.Count ?? 0);

            try
            {
                if (bulkEditDto == null || bulkEditDto.ResourceIds == null || !bulkEditDto.ResourceIds.Any())
                {
                    var badResponse = new ApiResponse<string>(false, "Invalid payload: ResourceIds cannot be null or empty.", null);
                    return BadRequest(badResponse);
                }

                int updatedCount = await _resourceService.BulkUpdateResourcesAsync(bulkEditDto);

                _logger.LogInformation("Bulk update completed. Resources updated: {Count}", updatedCount);

                var response = new ApiResponse<string>(
                    true,
                    $"Successfully updated {updatedCount} resource(s).",
                    null
                );

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Bulk update failed for {Count} resources.", bulkEditDto?.ResourceIds?.Count ?? 0);

                var errorResponse = new ApiResponse<string>(
                    false,
                    "An error occurred during bulk update: " + ex.Message,
                    null
                );

                return StatusCode(500, errorResponse);
            }
        }

        [HttpGet("role-options")]
        public async Task<IActionResult> GetRoleOptions()
        {
            try
            {
                var result = await _resourceService.GetRoleOptionsDropDownAsync();
                if (result == null) throw new Exception();
                var response = new ApiResponse<List<OptionDto>>(true, "Fetched Role options successfull", result);
                return Ok(response);
            }
            catch (Exception ex)
            {
                var response = new ApiResponse<string>(false, "something went wrong while fetching role options", null);
                return StatusCode(500, response);
            }
        }


    }
}
