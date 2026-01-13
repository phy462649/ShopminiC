using AutoMapper;
using LandingPageApp.Application.Dtos;
using LandingPageApp.Application.Exceptions;
using LandingPageApp.Application.Interfaces;
using LandingPageApp.Domain.Entities;
using LandingPageApp.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace LandingPageApp.Application.Services;

/// <summary>
/// Service xử lý logic nghiệp vụ cho lịch làm việc của nhân viên.
/// Quản lý việc tạo, cập nhật, xóa lịch làm việc theo ngày và theo tuần.
/// </summary>
public class StaffScheduleService : IStaffScheduleService
{
    /// <summary>
    /// Unit of Work để quản lý transaction và repositories.
    /// </summary>
    private readonly IUnitOfWork _uow;

    /// <summary>
    /// AutoMapper để chuyển đổi giữa Entity và DTO.
    /// </summary>
    private readonly IMapper _mapper;

    /// <summary>
    /// Logger để ghi log hoạt động.
    /// </summary>
    private readonly ILogger<StaffScheduleService> _logger;

    /// <summary>
    /// Khởi tạo StaffScheduleService với dependency injection.
    /// </summary>
    /// <param name="uow">Unit of Work.</param>
    /// <param name="mapper">AutoMapper instance.</param>
    /// <param name="logger">Logger instance.</param>
    public StaffScheduleService(
        IUnitOfWork uow,
        IMapper mapper,
        ILogger<StaffScheduleService> logger)
    {
        _uow = uow;
        _mapper = mapper;
        _logger = logger;
    }

    /// <summary>
    /// Lấy danh sách tất cả lịch làm việc.
    /// Bao gồm thông tin nhân viên.
    /// </summary>
    /// <param name="ct">Token hủy bỏ thao tác.</param>
    /// <returns>Danh sách lịch làm việc.</returns>
    public async Task<IEnumerable<StaffScheduleDto>> GetAllAsync(CancellationToken ct = default)
    {
        var schedules = await _uow.staffSchedules.Query()
            .Include(s => s.Staff)
            .AsNoTracking()
            .ToListAsync(ct);

        return _mapper.Map<IEnumerable<StaffScheduleDto>>(schedules);
    }

    /// <summary>
    /// Lấy thông tin lịch làm việc theo ID.
    /// </summary>
    /// <param name="id">ID của lịch làm việc.</param>
    /// <param name="ct">Token hủy bỏ thao tác.</param>
    /// <returns>Thông tin lịch làm việc hoặc null nếu không tìm thấy.</returns>
    public async Task<StaffScheduleDto?> GetByIdAsync(long id, CancellationToken ct = default)
    {
        var schedule = await _uow.staffSchedules.Query()
            .Include(s => s.Staff)
            .FirstOrDefaultAsync(s => s.Id == id, ct);

        return schedule is null ? null : _mapper.Map<StaffScheduleDto>(schedule);
    }

    /// <summary>
    /// Lấy danh sách lịch làm việc theo ID nhân viên.
    /// Sắp xếp theo ngày trong tuần và thời gian bắt đầu.
    /// </summary>
    /// <param name="staffId">ID của nhân viên.</param>
    /// <param name="ct">Token hủy bỏ thao tác.</param>
    /// <returns>Danh sách lịch làm việc của nhân viên.</returns>
    public async Task<IEnumerable<StaffScheduleDto>> GetByStaffIdAsync(long staffId, CancellationToken ct = default)
    {
        var schedules = await _uow.staffSchedules.Query()
            .Include(s => s.Staff)
            .Where(s => s.StaffId == staffId)
            .OrderBy(s => s.DayOfWeek)
            .ThenBy(s => s.StartTime)
            .AsNoTracking()
            .ToListAsync(ct);

        return _mapper.Map<IEnumerable<StaffScheduleDto>>(schedules);
    }

    /// <summary>
    /// Lấy lịch làm việc theo tuần của nhân viên.
    /// Trả về lịch làm việc được nhóm theo các ngày trong tuần.
    /// </summary>
    /// <param name="staffId">ID của nhân viên.</param>
    /// <param name="ct">Token hủy bỏ thao tác.</param>
    /// <returns>Lịch làm việc theo tuần.</returns>
    public async Task<StaffWeeklyScheduleDto> GetWeeklyScheduleAsync(long staffId, CancellationToken ct = default)
    {
        var schedules = await _uow.staffSchedules.Query()
            .Include(s => s.Staff)
            .Where(s => s.StaffId == staffId)
            .OrderBy(s => s.DayOfWeek)
            .ThenBy(s => s.StartTime)
            .AsNoTracking()
            .ToListAsync(ct);

        var staff = schedules.FirstOrDefault()?.Staff;

        return new StaffWeeklyScheduleDto
        {
            StaffId = staffId,
            StaffName = staff?.Name,
            Schedules = _mapper.Map<List<StaffScheduleDto>>(schedules)
        };
    }

    /// <summary>
    /// Tạo lịch làm việc mới.
    /// Kiểm tra ngày trong tuần hợp lệ (0-6) và không trùng lịch.
    /// </summary>
    /// <param name="dto">Thông tin lịch làm việc cần tạo.</param>
    /// <param name="ct">Token hủy bỏ thao tác.</param>
    /// <returns>Thông tin lịch làm việc vừa tạo.</returns>
    /// <exception cref="BusinessException">Khi dữ liệu không hợp lệ hoặc lịch bị trùng.</exception>
    public async Task<StaffScheduleDto> CreateAsync(CreateStaffScheduleDto dto, CancellationToken ct = default)
    {
        // Kiểm tra ngày trong tuần hợp lệ (0 = Chủ nhật, 6 = Thứ bảy)
        if (dto.DayOfWeek < 0 || dto.DayOfWeek > 6)
        {
            throw new BusinessException("DayOfWeek phải từ 0 (Chủ nhật) đến 6 (Thứ bảy).");
        }

        // Kiểm tra thời gian hợp lệ
        if (dto.StartTime >= dto.EndTime)
        {
            throw new BusinessException("Thời gian bắt đầu phải trước thời gian kết thúc.");
        }

        // Kiểm tra lịch trùng
        var hasOverlap = await _uow.staffSchedules.Query()
            .Where(s => s.StaffId == dto.StaffId)
            .Where(s => s.DayOfWeek == dto.DayOfWeek)
            .Where(s => s.StartTime < dto.EndTime && s.EndTime > dto.StartTime)
            .AnyAsync(ct);

        if (hasOverlap)
        {
            throw new BusinessException("Lịch làm việc bị trùng với lịch hiện có.");
        }

        var schedule = _mapper.Map<StaffSchedule>(dto);
        schedule.CreatedAt = DateTime.UtcNow;

        await _uow.staffSchedules.AddAsync(schedule, ct);
        await _uow.SaveChangesAsync(ct);

        _logger.LogInformation("Created staff schedule: {Id} for staff: {StaffId}", schedule.Id, dto.StaffId);

        return await GetByIdAsync(schedule.Id, ct) ?? throw new BusinessException("Lỗi khi tạo lịch làm việc.");
    }

    /// <summary>
    /// Tạo nhiều lịch làm việc cùng lúc cho một nhân viên.
    /// Dùng để thiết lập lịch làm việc cho cả tuần.
    /// </summary>
    /// <param name="dto">Thông tin các lịch làm việc cần tạo.</param>
    /// <param name="ct">Token hủy bỏ thao tác.</param>
    /// <returns>Danh sách lịch làm việc vừa tạo.</returns>
    /// <exception cref="BusinessException">Khi danh sách rỗng hoặc có lịch bị trùng.</exception>
    public async Task<IEnumerable<StaffScheduleDto>> CreateBulkAsync(CreateBulkStaffScheduleDto dto, CancellationToken ct = default)
    {
        if (!dto.Schedules.Any())
        {
            throw new BusinessException("Danh sách lịch làm việc không được rỗng.");
        }

        // Kiểm tra tất cả lịch hợp lệ
        foreach (var item in dto.Schedules)
        {
            if (item.DayOfWeek < 0 || item.DayOfWeek > 6)
            {
                throw new BusinessException("DayOfWeek phải từ 0 (Chủ nhật) đến 6 (Thứ bảy).");
            }

            if (item.StartTime >= item.EndTime)
            {
                throw new BusinessException("Thời gian bắt đầu phải trước thời gian kết thúc.");
            }
        }

        await _uow.BeginTransactionAsync(ct);

        try
        {
            var createdIds = new List<long>();

            foreach (var item in dto.Schedules)
            {
                // Kiểm tra lịch trùng
                var hasOverlap = await _uow.staffSchedules.Query()
                    .Where(s => s.StaffId == dto.StaffId)
                    .Where(s => s.DayOfWeek == item.DayOfWeek)
                    .Where(s => s.StartTime < item.EndTime && s.EndTime > item.StartTime)
                    .AnyAsync(ct);

                if (hasOverlap)
                {
                    throw new BusinessException($"Lịch làm việc ngày {item.DayOfWeek} bị trùng với lịch hiện có.");
                }

                var schedule = _mapper.Map<StaffSchedule>(item);
                schedule.StaffId = dto.StaffId;
                schedule.CreatedAt = DateTime.UtcNow;

                await _uow.staffSchedules.AddAsync(schedule, ct);
                await _uow.SaveChangesAsync(ct);
                createdIds.Add(schedule.Id);
            }

            await _uow.CommitTransactionAsync(ct);

            _logger.LogInformation("Created {Count} staff schedules for staff: {StaffId}", dto.Schedules.Count, dto.StaffId);

            var result = new List<StaffScheduleDto>();
            foreach (var id in createdIds)
            {
                var scheduleDto = await GetByIdAsync(id, ct);
                if (scheduleDto != null)
                    result.Add(scheduleDto);
            }

            return result;
        }
        catch
        {
            await _uow.RollbackTransactionAsync(ct);
            throw;
        }
    }

    /// <summary>
    /// Cập nhật thông tin lịch làm việc.
    /// Kiểm tra không trùng lịch (loại trừ lịch hiện tại).
    /// </summary>
    /// <param name="id">ID của lịch làm việc cần cập nhật.</param>
    /// <param name="dto">Thông tin cập nhật.</param>
    /// <param name="ct">Token hủy bỏ thao tác.</param>
    /// <returns>Thông tin lịch làm việc sau khi cập nhật.</returns>
    /// <exception cref="NotFoundException">Khi không tìm thấy lịch làm việc.</exception>
    /// <exception cref="BusinessException">Khi dữ liệu không hợp lệ hoặc lịch bị trùng.</exception>
    public async Task<StaffScheduleDto> UpdateAsync(long id, UpdateStaffScheduleDto dto, CancellationToken ct = default)
    {
        var schedule = await _uow.staffSchedules.GetByIdAsync(id, ct)
            ?? throw new NotFoundException($"Không tìm thấy StaffSchedule với Id: {id}");

        // Kiểm tra ngày trong tuần hợp lệ
        if (dto.DayOfWeek < 0 || dto.DayOfWeek > 6)
        {
            throw new BusinessException("DayOfWeek phải từ 0 (Chủ nhật) đến 6 (Thứ bảy).");
        }

        // Kiểm tra thời gian hợp lệ
        if (dto.StartTime >= dto.EndTime)
        {
            throw new BusinessException("Thời gian bắt đầu phải trước thời gian kết thúc.");
        }

        // Kiểm tra lịch trùng (loại trừ lịch hiện tại)
        var hasOverlap = await _uow.staffSchedules.Query()
            .Where(s => s.StaffId == schedule.StaffId)
            .Where(s => s.Id != id)
            .Where(s => s.DayOfWeek == dto.DayOfWeek)
            .Where(s => s.StartTime < dto.EndTime && s.EndTime > dto.StartTime)
            .AnyAsync(ct);

        if (hasOverlap)
        {
            throw new BusinessException("Lịch làm việc bị trùng với lịch hiện có.");
        }

        _mapper.Map(dto, schedule);
        schedule.UpdatedAt = DateTime.UtcNow;

        _uow.staffSchedules.Update(schedule);
        await _uow.SaveChangesAsync(ct);

        _logger.LogInformation("Updated staff schedule: {Id}", schedule.Id);

        return await GetByIdAsync(schedule.Id, ct) ?? throw new BusinessException("Lỗi khi cập nhật lịch làm việc.");
    }

    /// <summary>
    /// Xóa lịch làm việc theo ID.
    /// </summary>
    /// <param name="id">ID của lịch làm việc cần xóa.</param>
    /// <param name="ct">Token hủy bỏ thao tác.</param>
    /// <returns>True nếu xóa thành công, false nếu không tìm thấy.</returns>
    public async Task<bool> DeleteAsync(long id, CancellationToken ct = default)
    {
        var schedule = await _uow.staffSchedules.GetByIdAsync(id, ct);

        if (schedule is null)
            return false;

        _uow.staffSchedules.Delete(schedule);
        await _uow.SaveChangesAsync(ct);

        _logger.LogInformation("Deleted staff schedule: {Id}", id);

        return true;
    }

    /// <summary>
    /// Xóa tất cả lịch làm việc của một nhân viên.
    /// </summary>
    /// <param name="staffId">ID của nhân viên.</param>
    /// <param name="ct">Token hủy bỏ thao tác.</param>
    /// <returns>True nếu xóa thành công, false nếu không tìm thấy lịch nào.</returns>
    public async Task<bool> DeleteByStaffIdAsync(long staffId, CancellationToken ct = default)
    {
        var schedules = await _uow.staffSchedules.Query()
            .Where(s => s.StaffId == staffId)
            .ToListAsync(ct);

        if (!schedules.Any())
            return false;

        foreach (var schedule in schedules)
        {
            _uow.staffSchedules.Delete(schedule);
        }

        await _uow.SaveChangesAsync(ct);

        _logger.LogInformation("Deleted all schedules for staff: {StaffId}", staffId);

        return true;
    }
}
