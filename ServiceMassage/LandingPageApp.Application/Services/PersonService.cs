using LandingPageApp.Application.Dtos;
using LandingPageApp.Application.Interfaces;
using LandingPageApp.Domain.Entities;
using LandingPageApp.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace LandingPageApp.Application.Services
{
    public class PersonService : IPersonService
    {
        private readonly IPersonRepository _repository;

        public PersonService(IPersonRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<object>> GetAllAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<object> GetByIdAsync(int id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task<object> CreateAsync(object createDto)
        {
            // Implementation will depend on the actual DTO structure
            return await Task.FromResult(createDto);
        }

        public async Task<object> UpdateAsync(long id, object updateDto)
        {
            // Implementation will depend on the actual DTO structure
            return await Task.FromResult(updateDto);
        }

        public async Task<bool> DeleteAsync(long id)
        {
            // Implementation will depend on the repository
            return await Task.FromResult(true);
        }

        public async Task<PersonSearchResponse> SearchAsync(PersonSearchRequest request)
        {
            var query = _repository.Query();

            // Apply search filter
            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var searchTerm = request.Search.Trim().ToLower();
                query = query.Where(p =>
                    p.Name.ToLower().Contains(searchTerm) ||
                    (p.Email != null && p.Email.ToLower().Contains(searchTerm)) ||
                    (p.Phone != null && p.Phone.Contains(searchTerm)) ||
                    p.Username.ToLower().Contains(searchTerm));
            }

            // Apply specific filters
            if (!string.IsNullOrWhiteSpace(request.Name))
            {
                query = query.Where(p => p.Name.ToLower().Contains(request.Name.ToLower().Trim()));
            }

            if (!string.IsNullOrWhiteSpace(request.Email))
            {
                query = query.Where(p => p.Email != null && p.Email.ToLower().Contains(request.Email.ToLower().Trim()));
            }

            if (!string.IsNullOrWhiteSpace(request.Phone))
            {
                query = query.Where(p => p.Phone != null && p.Phone.Contains(request.Phone.Trim()));
            }

            if (!string.IsNullOrWhiteSpace(request.Username))
            {
                query = query.Where(p => p.Username.ToLower().Contains(request.Username.ToLower().Trim()));
            }

            if (request.RoleId.HasValue)
            {
                query = query.Where(p => p.RoleId == request.RoleId.Value);
            }

            if (request.CreatedFrom.HasValue)
            {
                query = query.Where(p => p.CreatedAt >= request.CreatedFrom.Value.Date);
            }

            if (request.CreatedTo.HasValue)
            {
                query = query.Where(p => p.CreatedAt <= request.CreatedTo.Value.Date.AddDays(1).AddTicks(-1));
            }

            // Get total count before pagination
            var totalCount = await Task.FromResult(query.Count());

            // Apply sorting
            var sortBy = GetValidSortByField(request.SortBy);
            var sortOrder = request.SortOrder?.ToLower() == "asc" ? "asc" : "desc";

            query = query.OrderBy($"{sortBy} {sortOrder}");

            // Apply pagination
            var skip = (request.Page - 1) * request.PageSize;
            query = query.Skip(skip).Take(request.PageSize);

            // Execute query
            var items = await Task.FromResult(query.ToList());

            return new PersonSearchResponse
            {
                Items = items,
                TotalCount = totalCount,
                Page = request.Page,
                PageSize = request.PageSize,
                SearchCriteria = request
            };
        }

        private string GetValidSortByField(string? sortBy)
        {
            if (string.IsNullOrWhiteSpace(sortBy))
                return "CreatedAt";

            var validFields = new[] { "Id", "Name", "Email", "Phone", "Username", "RoleId", "CreatedAt", "UpdatedAt" };

            return validFields.Contains(sortBy, StringComparer.OrdinalIgnoreCase)
                ? sortBy
                : "CreatedAt";
        }
    }
}


