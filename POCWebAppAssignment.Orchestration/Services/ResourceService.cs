using Microsoft.Extensions.Logging;
using POCWebAppAssignment.Interfaces;
using POCWebAppAssignment.Model;
using POCWebAppAssignment.Model.DTOs;

namespace POCWebAppAssignment.Orchestration.Services
{
    public class ResourceService : IResourceService
    {
        private readonly IResourceRepository _resourceRepository;
        private readonly ILogger<ResourceService> _logger;

        public ResourceService(IResourceRepository resourceRepository, ILogger<ResourceService> logger)
        {
            _resourceRepository = resourceRepository;
            _logger = logger;
        }

        public async Task<IEnumerable<Resource>> GetAllResourcesAsync()
        {
            try
            {
                _logger.LogInformation("Starting to fetch all resources.");
                var result = await _resourceRepository.GetAllResourcesAsync();
                _logger.LogInformation("Successfully fetched {Count} resources.", result?.Count() ?? 0);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch all resources due to an unexpected error.");
                throw;
            }
        }

        public async Task<ResourceDetailsDto?> GetResourceByIdAsync(int empId)
        {
            try
            {
                _logger.LogInformation("Fetching resource by ID: {EmpId}", empId);
                var result = await _resourceRepository.GetResourceByIdAsync(empId);

                if (result == null)
                    _logger.LogWarning("No resource found with ID: {EmpId}", empId);
                else
                    _logger.LogInformation("Successfully fetched resource with ID: {EmpId}", empId);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching resource with ID: {EmpId}", empId);
                throw;
            }
        }

        public async Task<int> CreateResourceAsync(ResourceDto resource)
        {
            try
            {
                _logger.LogInformation("Creating a new resource. Email: {Email}", resource.EmailId);
                var newEmpId = await _resourceRepository.CreateResourceAsync(resource);
                _logger.LogInformation("Successfully created resource with ID: {EmpId}", newEmpId);
                return newEmpId;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create resource with Email: {Email}", resource.EmailId);
                throw;
            }
        }

        public async Task UpdateResourceAsync(ResourceDto resource)
        {
            try
            {
                _logger.LogInformation("Updating resource with ID: {EmpId}", resource.EmpId);
                await _resourceRepository.UpdateResourceAsync(resource);
                _logger.LogInformation("Successfully updated resource with ID: {EmpId}", resource.EmpId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating resource with ID: {EmpId}", resource.EmpId);
                throw;
            }
        }

        public async Task DeleteResourceAsync(int empId)
        {
            try
            {
                _logger.LogInformation("Deleting resource with ID: {EmpId}", empId);
                await _resourceRepository.DeleteResourceAsync(empId);
                _logger.LogInformation("Successfully deleted resource with ID: {EmpId}", empId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete resource with ID: {EmpId}", empId);
                throw;
            }
        }

        public async Task DeleteResourcesByEmpIdListAsync(List<int> empIds)
        {
            try
            {
                _logger.LogInformation("Deleting multiple resources. IDs: {@EmpIds}", empIds);
                await _resourceRepository.DeleteResourcesByEmpIdListAsync(empIds);
                _logger.LogInformation("Successfully deleted {Count} resources.", empIds.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting resources. IDs: {@EmpIds}", empIds);
                throw;
            }
        }

        public async Task<IEnumerable<int>> GetResourceStatisticsAsync()
        {
            try
            {
                _logger.LogInformation("Fetching resource statistics.");
                var stats = await _resourceRepository.GetResourceStatisticsAsync();
                _logger.LogInformation("Fetched {Count} resource statistics.", stats?.Count() ?? 0);
                return stats;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch resource statistics.");
                throw;
            }
        }

        public async Task<bool> CheckEmailExistsAsync(string emailId)
        {
            try
            {
                _logger.LogInformation("Checking if email exists: {Email}", emailId);
                var exists = await _resourceRepository.CheckEmailExistsAsync(emailId);

                if (exists)
                    _logger.LogInformation("Email {Email} already exists.", emailId);
                else
                    _logger.LogInformation("Email {Email} is available.", emailId);

                return exists;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while checking email existence: {Email}", emailId);
                throw;
            }
        }

        public async Task<DropdownResponseDto> GetDropdownDataAsync()
        {
            try
            {
                _logger.LogInformation("Fetching dropdown data for UI.");
                var dropdownData = await _resourceRepository.GetDropdownDataAsync();
                _logger.LogInformation("Successfully fetched dropdown data.");
                return dropdownData;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch dropdown data.");
                throw;
            }
        }

        public async Task<int> BulkUpdateResourcesAsync(BulkEditDto bulkEditDetails)
        {
            try
            {
                _logger.LogInformation("Initiating bulk update for resources. IDs: {@EmpIds}", bulkEditDetails.ResourceIds);
                var updatedCount = await _resourceRepository.BulkUpdateResourcesAsync(bulkEditDetails);
                _logger.LogInformation("Successfully bulk updated {Count} resources.", updatedCount);
                return updatedCount;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during bulk update for resource IDs: {@EmpIds}", bulkEditDetails.ResourceIds);
                throw;
            }
        }

        public async Task BulkCreateResourcesAsync(List<ResourceDto> resources)
        {
            try
            {
                _logger.LogInformation("Initiating bulk creation of {Count} resources.", resources.Count);
                await _resourceRepository.BulkCreateResourcesAsync(resources);
                _logger.LogInformation("Successfully created {Count} resources in bulk.", resources.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during bulk resource creation. Count: {Count}", resources.Count);
                throw;
            }
        }

        public async Task<List<OptionDto>?> GetRoleOptionsDropDownAsync()
        {
            try
            {
                _logger.LogInformation("Fetching role options for dropdown.");
                var options = await _resourceRepository.GetRoleOptionsDropDownAsync();
                _logger.LogInformation("Successfully fetched {Count} role options.", options?.Count ?? 0);
                return options;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching role options for dropdown.");
                throw;
            }
        }

        public async Task<PagedResult<Resource>> GetResourcesAsync(GridQueryParameters query)
        {
            
            var resources = await _resourceRepository.GetAllResourcesAsync();
            var filtered = resources;

            // Filtering
            if (query.Filters != null && query.Filters.Count > 0)
            {
                foreach (var filter in query.Filters)
                {
                    if (!string.IsNullOrWhiteSpace(filter.Value))
                    {
                        filtered = filtered.Where(r =>
                        {
                            switch (filter.Field.ToLower())
                            {
                                case "empid":
                                    return r.EmpId.HasValue && r.EmpId.Value.ToString() == filter.Value;
                                case "resourcename":
                                    return r.ResourceName?.Contains(filter.Value, StringComparison.OrdinalIgnoreCase) == true;
                                case "designation":
                                    return r.Designation?.Contains(filter.Value, StringComparison.OrdinalIgnoreCase) == true;
                                case "reportingto":
                                    return r.ReportingTo?.Contains(filter.Value, StringComparison.OrdinalIgnoreCase) == true;
                                case "billable":
                                    return bool.TryParse(filter.Value, out var billableVal) && r.Billable == billableVal;
                                case "technologyskill":
                                    return r.TechnologySkill?.Contains(filter.Value, StringComparison.OrdinalIgnoreCase) == true;
                                case "projectallocation":
                                    return r.ProjectAllocation?.Contains(filter.Value, StringComparison.OrdinalIgnoreCase) == true;
                                case "location":
                                    return r.Location?.Contains(filter.Value, StringComparison.OrdinalIgnoreCase) == true;
                                case "emailid":
                                    return r.EmailId?.Contains(filter.Value, StringComparison.OrdinalIgnoreCase) == true;
                                case "ctedoj":
                                    return DateOnly.TryParse(filter.Value, out var doj) && r.CteDoj == doj;
                                case "remarks":
                                    return r.Remarks?.Contains(filter.Value, StringComparison.OrdinalIgnoreCase) == true;
                                default:
                                    return true;
                            }
                        });
                    }
                }
            }

            // Sorting
            if (query.Sorts != null && query.Sorts.Count > 0)
            {
                IOrderedEnumerable<Resource>? ordered = null;

                foreach (var sort in query.Sorts)
                {
                    Func<Resource, object?> keySelector = sort.Field.ToLower() switch
                    {
                        "empid" => r => r.EmpId,
                        "resourcename" => r => r.ResourceName,
                        "designation" => r => r.Designation,
                        "reportingto" => r => r.ReportingTo,
                        "billable" => r => r.Billable,
                        "technologyskill" => r => r.TechnologySkill,
                        "projectallocation" => r => r.ProjectAllocation,
                        "location" => r => r.Location,
                        "emailid" => r => r.EmailId,
                        "ctedoj" => r => r.CteDoj,
                        "remarks" => r => r.Remarks,
                        _ => r => null
                    };

                    if (ordered == null)
                    {
                        ordered = sort.Direction.Equals("asc", StringComparison.OrdinalIgnoreCase)
                            ? filtered.OrderBy(keySelector)
                            : filtered.OrderByDescending(keySelector);
                    }
                    else
                    {
                        ordered = sort.Direction.Equals("asc", StringComparison.OrdinalIgnoreCase)
                            ? ordered.ThenBy(keySelector)
                            : ordered.ThenByDescending(keySelector);
                    }
                }

                if (ordered != null) filtered = ordered;
            }

            // Paging
            var totalCount = filtered.Count();
            var data = filtered
                .Skip((query.PageNumber - 1) * query.PageSize)
                .Take(query.PageSize)
                .ToList();

            return new PagedResult<Resource>
            {
                Data = data,
                TotalCount = totalCount,
                PageNumber = query.PageNumber,
                PageSize = query.PageSize
            };
        }



    }
}
