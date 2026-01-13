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

/// <summary>
/// Service xử lý logic nghiệp vụ cho người dùng.
/// Quản lý việc tạo, cập nhật, xóa và tìm kiếm người dùng.
/// </summary>
public class PersonService : IPersonService
{
    /// <summary>
    /// Repository quản lý người dùng.
    /// </summary>
    private readonly IPersonRepository _personRepository;

    /// <summary>
    /// Unit of Work để quản lý transaction và repositories.
    /// </summary>
    private readonly IUnitOfWork _unitOfWork;

    /// <summary>
    /// AutoMapper để chuyển đổi giữa Entity và DTO.
    /// </summary>
    private readonly IMapper _mapper;

    /// <summary>
    /// Service xử lý bảo mật (hash password).
    /// </summary>
    private readonly ISecurityService _securityService;

    /// <summary>
    /// Logger để ghi log hoạt động.
    /// </summary>
    private readonly ILogger<PersonService> _logger;

    /// <summary>
    /// Khởi tạo PersonService với dependency injection.
    /// </summary>
    /// <param name="personRepository">Repository người dùng.</param>
    /// <param name="unitOfWork">Unit of Work.</param>
    /// <param name="mapper">AutoMapper instance.</param>
    /// <param name="securityService">Service bảo mật.</param>
    /// <param name="logger">Logger instance.</param>
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

    /// <summary>
    /// Lấy danh sách tất cả người dùng.
    /// Bao gồm thông tin vai trò.
    /// </summary>
    /// <param name="ct">Token hủy bỏ thao tác.</param>
    /// <returns>Danh sách người dùng.</returns>
    public async Task<IEnumerable<PersonDto>> GetAllAsync(CancellationToken ct = default)
    {
        var persons = await _personRepository.Query()
            .Include(p => p.Role)
            .ToListAsync(ct);
        return _mapper.Map<IEnumerable<PersonDto>>(persons);
    }

    /// <summary>
    /// Lấy thông tin người dùng theo ID.
    /// Bao gồm thông tin vai trò.
    /// </summary>
    /// <param name="id">ID của người dùng.</param>
    /// <param name="ct">Token hủy bỏ thao tác.</param>
    /// <returns>Thông tin người dùng hoặc null nếu không tìm thấy.</returns>
    public async Task<PersonDto?> GetByIdAsync(long id, CancellationToken ct = default)
    {
        var person = await _personRepository.Query()
            .Include(p => p.Role)
            .FirstOrDefaultAsync(p => p.Id == id, ct);
        return person is null ? null : _mapper.Map<PersonDto>(person);
    }

    /// <summary>
    /// Lấy thông tin chi tiết người dùng theo ID.
    /// Bao gồm vai trò, danh sách booking và đơn hàng.
    /// </summary>
    /// <param name="id">ID của người dùng.</param>
    /// <param name="ct">Token hủy bỏ thao tác.</param>
    /// <returns>Thông tin chi tiết người dùng hoặc null nếu không tìm thấy.</returns>
    public async Task<PersonDetailDto?> GetDetailByIdAsync(long id, CancellationToken ct = default)
    {
        var person = await _personRepository.Query()
            .Include(p => p.Role)
            .Include(p => p.BookingCustomers)
            .Include(p => p.Orders)
            .FirstOrDefaultAsync(p => p.Id == id, ct);
        return person is null ? null : _mapper.Map<PersonDetailDto>(person);
    }

    /// <summary>
    /// Tạo người dùng mới.
    /// Kiểm tra trùng username và email trước khi tạo.
    /// Mật khẩu được hash trước khi lưu.
    /// </summary>
    /// <param name="dto">Thông tin người dùng cần tạo.</param>
    /// <param name="ct">Token hủy bỏ thao tác.</param>
    /// <returns>Thông tin người dùng vừa tạo.</returns>
    /// <exception cref="BusinessException">Khi username/email đã tồn tại hoặc role không hợp lệ.</exception>
    public async Task<PersonDto> CreateAsync(CreatePersonDto dto, CancellationToken ct = default)
    {
        // Kiểm tra trùng username
        var existingPerson = await _personRepository
            .FindAsync(p => p.Username == dto.Username, ct);
        
        if (existingPerson.Any())
        {
            throw new BusinessException($"Username '{dto.Username}' đã tồn tại.");
        }

        // Kiểm tra trùng email nếu có
        if (!string.IsNullOrEmpty(dto.Email))
        {
            var existingEmail = await _personRepository
                .FindAsync(p => p.Email == dto.Email, ct);
            
            if (existingEmail.Any())
            {
                throw new BusinessException($"Email '{dto.Email}' đã được sử dụng.");
            }
        }

        // Kiểm tra role tồn tại
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

        // Reload với role
        var createdPerson = await GetByIdAsync(person.Id, ct);
        return createdPerson!;
    }

    /// <summary>
    /// Cập nhật thông tin người dùng.
    /// Kiểm tra trùng email (loại trừ người dùng hiện tại) trước khi cập nhật.
    /// </summary>
    /// <param name="id">ID của người dùng cần cập nhật.</param>
    /// <param name="dto">Thông tin cập nhật.</param>
    /// <param name="ct">Token hủy bỏ thao tác.</param>
    /// <returns>Thông tin người dùng sau khi cập nhật.</returns>
    /// <exception cref="NotFoundException">Khi không tìm thấy người dùng.</exception>
    /// <exception cref="BusinessException">Khi email đã tồn tại hoặc role không hợp lệ.</exception>
    public async Task<PersonDto> UpdateAsync(long id, UpdatePersonDto dto, CancellationToken ct = default)
    {
        var person = await _personRepository.GetByIdAsync(id, ct)
            ?? throw new NotFoundException($"Không tìm thấy người dùng với Id: {id}");

        // Kiểm tra trùng email (loại trừ người dùng hiện tại)
        if (!string.IsNullOrEmpty(dto.Email))
        {
            var existingEmail = await _personRepository
                .FindAsync(p => p.Email == dto.Email && p.Id != id, ct);
            
            if (existingEmail.Any())
            {
                throw new BusinessException($"Email '{dto.Email}' đã được sử dụng.");
            }
        }

        // Kiểm tra role tồn tại
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

        // Reload với role
        var updatedPerson = await GetByIdAsync(person.Id, ct);
        return updatedPerson!;
    }

    /// <summary>
    /// Xóa người dùng theo ID.
    /// Không thể xóa người dùng đang có booking hoặc đơn hàng liên quan.
    /// </summary>
    /// <param name="id">ID của người dùng cần xóa.</param>
    /// <param name="ct">Token hủy bỏ thao tác.</param>
    /// <returns>True nếu xóa thành công, false nếu không tìm thấy.</returns>
    /// <exception cref="BusinessException">Khi người dùng đang có booking/đơn hàng liên quan.</exception>
    public async Task<bool> DeleteAsync(long id, CancellationToken ct = default)
    {
        var person = await _personRepository.GetByIdAsync(id, ct);

        if (person is null)
            return false;

        // Kiểm tra người dùng có booking liên quan không
        var hasBookings = await _unitOfWork.bookings
            .ExistsAsync(b => b.CustomerId == id, ct);

        if (hasBookings)
        {
            throw new BusinessException($"Không thể xóa người dùng '{person.Username}' vì đang có booking liên quan.");
        }

        // Kiểm tra người dùng có đơn hàng liên quan không
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

    /// <summary>
    /// Tìm kiếm và lọc người dùng với các tùy chọn nâng cao.
    /// Hỗ trợ tìm kiếm theo tên, email, số điện thoại, username, vai trò và ngày tạo.
    /// Hỗ trợ sắp xếp và phân trang.
    /// </summary>
    /// <param name="request">Các tiêu chí tìm kiếm.</param>
    /// <param name="ct">Token hủy bỏ thao tác.</param>
    /// <returns>Kết quả tìm kiếm với phân trang.</returns>
    public async Task<PersonSearchResponse> SearchAsync(PersonSearchRequest request, CancellationToken ct = default)
    {
        var query = _personRepository.Query().Include(p => p.Role).AsQueryable();

        // Áp dụng bộ lọc tìm kiếm chung
        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var searchTerm = request.Search.Trim().ToLower();
            query = query.Where(p =>
                p.Name.ToLower().Contains(searchTerm) ||
                (p.Email != null && p.Email.ToLower().Contains(searchTerm)) ||
                (p.Phone != null && p.Phone.Contains(searchTerm)) ||
                p.Username.ToLower().Contains(searchTerm));
        }

        // Áp dụng các bộ lọc cụ thể
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

        // Lấy tổng số bản ghi trước khi phân trang
        var totalCount = await query.CountAsync(ct);

        // Áp dụng sắp xếp
        var sortBy = GetValidSortByField(request.SortBy);
        var sortOrder = request.SortOrder?.ToLower() == "asc" ? "asc" : "desc";

        query = query.OrderBy($"{sortBy} {sortOrder}");

        // Áp dụng phân trang
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

    /// <summary>
    /// Lấy danh sách người dùng theo vai trò.
    /// </summary>
    /// <param name="roleId">ID của vai trò.</param>
    /// <param name="ct">Token hủy bỏ thao tác.</param>
    /// <returns>Danh sách người dùng có vai trò tương ứng.</returns>
    public async Task<IEnumerable<PersonDto>> GetByRoleAsync(long roleId, CancellationToken ct = default)
    {
        var persons = await _personRepository.Query()
            .Include(p => p.Role)
            .Where(p => p.RoleId == roleId)
            .ToListAsync(ct);
        return _mapper.Map<IEnumerable<PersonDto>>(persons);
    }

    /// <summary>
    /// Lấy trường sắp xếp hợp lệ.
    /// Trả về "CreatedAt" nếu trường không hợp lệ.
    /// </summary>
    /// <param name="sortBy">Tên trường cần sắp xếp.</param>
    /// <returns>Tên trường hợp lệ.</returns>
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
