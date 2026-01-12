using AutoMapper;
using LandingPageApp.Application.Dtos;
using LandingPageApp.Application.Exceptions;
using LandingPageApp.Application.Interfaces;
using LandingPageApp.Domain.Entities;
using LandingPageApp.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq.Dynamic.Core;

namespace LandingPageApp.Application.Services;

public class PersonService : IPersonService
{
    private readonly IPersonRepository _personRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ISecurityService _securityService;
    private readonly ILogger<PersonService> _logger;

    public PersonService(
        IPersonRepository personRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ISecurityService securityService,
        ILogger<PersonService> logger)
    {
        _personRepository = personRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _securityService = securityService;
        _logger = logger;
    }

    public async Task<IEnumerable<PersonDto>> GetAllAsync(CancellationToken ct = default)
    {
        var persons = await _personRepository.Query()
            .Include(p => p.Role)
            .ToListAsync(ct);
        return _mapper.Map<IEnumerable<PersonDto>>(persons);
    }

    public async Task<PersonDto?> GetByIdAsync(long id, CancellationToken ct = default)
    {
        var person = await _personRepository.Query()
            .Include(p => p.Role)
            .FirstOrDefaultAsync(p => p.Id == id, ct);
        return person is null ? null : _mapper.Map<PersonDto>(person);
    }

    public async Task<PersonDetailDto?> GetDetailByIdAsync(long id, CancellationToken ct = default)
    {
        var person = await _personRepository.Query()
            .Include(p => p.Role)
            .Include(p => p.BookingCustomers)
            .Include(p => p.Orders)
            .FirstOrDefaultAsync(p => p.Id == id, ct);
        return person is null ? null : _mapper.Map<PersonDetailDto>(person);
    }

    public async Task<PersonDto> CreateAsync(CreatePersonDto dto, CancellationToken ct = default)
    {
        // Check duplicate username
        var existingPerson = await _personRepository
            .FindAsync(p => p.Username == dto.Username, ct);
        
        if (existingPerson.Any())
        {
            throw new BusinessException($"Username '{dto.Username}' đã tồn tại.");
        }

        // Check duplicate email if provided
        if (!string.IsNullOrEmpty(dto.Email))
        {
            var existingEmail = await _personRepository
                .FindAsync(p => p.Email == dto.Email, ct);
            
            if (existingEmail.Any())
            {
                throw new BusinessException($"Email '{dto.Email}' đã được sử dụng.");
            }
        }

        // Validate role exists
        var roleExists = await _unitOfWork.roles.ExistsAsync(r => r.Id == dto.RoleId, ct);
        if (!roleExists)
        {
            throw new BusinessException($"Role với Id: {dto.RoleId} không tồn tại.");
        }

        var person = _mapper.Map<Person>(dto);
        person.PasswordHash = _securityService.HashPassword(dto.Password);
        person.StatusVerify = false;
        person.CreatedAt = DateTime.UtcNow;

        await _personRepository.AddAsync(person, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        _logger.LogInformation("Created person: {Username} with Id: {Id}", person.Username, person.Id);

        // Reload with role
        var createdPerson = await GetByIdAsync(person.Id, ct);
        return createdPerson!;
    }

    public async Task<PersonDto> UpdateAsync(long id, UpdatePersonDto dto, CancellationToken ct = default)
    {
        var person = await _personRepository.GetByIdAsync(id, ct)
            ?? throw new NotFoundException($"Không tìm thấy người dùng với Id: {id}");

        // Check duplicate email if provided (exclude current person)
        if (!string.IsNullOrEmpty(dto.Email))
        {
            var existingEmail = await _personRepository
                .FindAsync(p => p.Email == dto.Email && p.Id != id, ct);
            
            if (existingEmail.Any())
            {
                throw new BusinessException($"Email '{dto.Email}' đã được sử dụng.");
            }
        }

        // Validate role exists
        var roleExists = await _unitOfWork.roles.ExistsAsync(r => r.Id == dto.RoleId, ct);
        if (!roleExists)
        {
            throw new BusinessException($"Role với Id: {dto.RoleId} không tồn tại.");
        }

        _mapper.Map(dto, person);
        person.UpdatedAt = DateTime.UtcNow;

        _personRepository.Update(person);
        await _unitOfWork.SaveChangesAsync(ct);

        _logger.LogInformation("Updated person: {Id}", person.Id);

        // Reload with role
        var updatedPerson = await GetByIdAsync(person.Id, ct);
        return updatedPerson!;
    }

    public async Task<bool> DeleteAsync(long id, CancellationToken ct = default)
    {
        var person = await _personRepository.GetByIdAsync(id, ct);

        if (person is null)
            return false;

        // Check if person has any bookings as customer
        var hasBookings = await _unitOfWork.bookings
            .ExistsAsync(b => b.CustomerId == id, ct);

        if (hasBookings)
        {
            throw new BusinessException($"Không thể xóa người dùng '{person.Username}' vì đang có booking liên quan.");
        }

        // Check if person has any orders
        var hasOrders = await _unitOfWork.orders
            .ExistsAsync(o => o.CustomerId == id, ct);

        if (hasOrders)
        {
            throw new BusinessException($"Không thể xóa người dùng '{person.Username}' vì đang có đơn hàng liên quan.");
        }

        _personRepository.Delete(person);
        await _unitOfWork.SaveChangesAsync(ct);

        _logger.LogInformation("Deleted person: {Id} - {Username}", person.Id, person.Username);

        return true;
    }

    public async Task<PersonSearchResponse> SearchAsync(PersonSearchRequest request, CancellationToken ct = default)
    {
        var query = _personRepository.Query().Include(p => p.Role).AsQueryable();

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
        var totalCount = await query.CountAsync(ct);

        // Apply sorting
        var sortBy = GetValidSortByField(request.SortBy);
        var sortOrder = request.SortOrder?.ToLower() == "asc" ? "asc" : "desc";

        query = query.OrderBy($"{sortBy} {sortOrder}");

        // Apply pagination
        var skip = (request.Page - 1) * request.PageSize;
        var items = await query.Skip(skip).Take(request.PageSize).ToListAsync(ct);

        return new PersonSearchResponse
        {
            Items = items,
            TotalCount = totalCount,
            Page = request.Page,
            PageSize = request.PageSize,
            SearchCriteria = request
        };
    }

    public async Task<IEnumerable<PersonDto>> GetByRoleAsync(long roleId, CancellationToken ct = default)
    {
        var persons = await _personRepository.Query()
            .Include(p => p.Role)
            .Where(p => p.RoleId == roleId)
            .ToListAsync(ct);
        return _mapper.Map<IEnumerable<PersonDto>>(persons);
    }

    private static string GetValidSortByField(string? sortBy)
    {
        if (string.IsNullOrWhiteSpace(sortBy))
            return "CreatedAt";

        var validFields = new[] { "Id", "Name", "Email", "Phone", "Username", "RoleId", "CreatedAt", "UpdatedAt" };

        return validFields.Contains(sortBy, StringComparer.OrdinalIgnoreCase)
            ? sortBy
            : "CreatedAt";
    }
}
