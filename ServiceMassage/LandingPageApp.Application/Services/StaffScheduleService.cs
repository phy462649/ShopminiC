using AutoMapper;
using LandingPageApp.Application.Dtos;
using LandingPageApp.Application.Exceptions;
using LandingPageApp.Application.Interfaces;
using LandingPageApp.Domain.Entities;
using LandingPageApp.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace LandingPageApp.Application.Services;

public class StaffScheduleService : IStaffScheduleService
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;
    private readonly ILogger<StaffScheduleService> _logger;

    public StaffScheduleService(
        IUnitOfWork uow,
        IMapper mapper,
        ILogger<StaffScheduleService> logger)
    {
        _uow = uow;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<IEnumerable<StaffScheduleDto>> GetAllAsync(CancellationToken ct = default)
    {
        var schedules = await _uow.staffSchedules.Query()
            .Include(s => s.Staff)
            .AsNoTracking()
            .ToListAsync(ct);

        return _mapper.Map<IEnumerable<StaffScheduleDto>>(schedules);
    }

    public async Task<StaffScheduleDto?> GetByIdAsync(long id, CancellationToken ct = default)
    {
        var schedule = await _uow.staffSchedules.Query()
            .Include(s => s.Staff)
            .FirstOrDefaultAsync(s => s.Id == id, ct);

        return schedule is null ? null : _mapper.Map<StaffScheduleDto>(schedule);
    }

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

    public async Task<StaffScheduleDto> CreateAsync(CreateStaffScheduleDto dto, CancellationToken ct = default)
    {
        // Validate day of week
        if (dto.DayOfWeek < 0 || dto.DayOfWeek > 6)
        {
            throw new BusinessException("DayOfWeek phải từ 0 (Chủ nhật) đến 6 (Thứ bảy).");
        }

        // Validate time range
        if (dto.StartTime >= dto.EndTime)
        {
            throw new BusinessException("Thời gian bắt đầu phải trước thời gian kết thúc.");
        }

        // Check for overlapping schedule
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

    public async Task<IEnumerable<StaffScheduleDto>> CreateBulkAsync(CreateBulkStaffScheduleDto dto, CancellationToken ct = default)
    {
        if (!dto.Schedules.Any())
        {
            throw new BusinessException("Danh sách lịch làm việc không được rỗng.");
        }

        // Validate all schedules
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
                // Check for overlapping schedule
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

    public async Task<StaffScheduleDto> UpdateAsync(long id, UpdateStaffScheduleDto dto, CancellationToken ct = default)
    {
        var schedule = await _uow.staffSchedules.GetByIdAsync(id, ct)
            ?? throw new NotFoundException($"Không tìm thấy StaffSchedule với Id: {id}");

        // Validate day of week
        if (dto.DayOfWeek < 0 || dto.DayOfWeek > 6)
        {
            throw new BusinessException("DayOfWeek phải từ 0 (Chủ nhật) đến 6 (Thứ bảy).");
        }

        // Validate time range
        if (dto.StartTime >= dto.EndTime)
        {
            throw new BusinessException("Thời gian bắt đầu phải trước thời gian kết thúc.");
        }

        // Check for overlapping schedule (exclude current)
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
