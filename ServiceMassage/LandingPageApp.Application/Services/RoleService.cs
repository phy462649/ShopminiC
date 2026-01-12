
using AutoMapper;
using LandingPageApp.Application.Dtos;
using LandingPageApp.Application.Exceptions;
using LandingPageApp.Application.Interfaces;
using LandingPageApp.Domain.Entities;
using LandingPageApp.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace LandingPageApp.Application.Services;

public class RoleService : IRoleService
{
    private readonly IRoleRepository _roleRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<RoleService> _logger;

    public RoleService(
        IRoleRepository roleRepository,
        IMapper mapper,
        ILogger<RoleService> logger)
    {
        _roleRepository = roleRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<IEnumerable<RoleDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var roles = await _roleRepository.GetAllAsync(cancellationToken);
        return _mapper.Map<IEnumerable<RoleDto>>(roles);
    }

    public async Task<RoleDto?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        var role = await _roleRepository.GetByIdAsync(id, cancellationToken);
        return role is null ? null : _mapper.Map<RoleDto>(role);
    }

    public async Task<RoleDto> CreateAsync(CreateRoleDto dto, CancellationToken cancellationToken = default)
    {
        if (await ExistsByNameAsync(dto.Name, cancellationToken))
        {
            throw new BusinessException($"Role với tên '{dto.Name}' đã tồn tại.");
        }

        var role = _mapper.Map<Role>(dto);
        role.Name = role.Name.Trim();
        role.Description = role.Description?.Trim();
        role.CreatedAt = DateTime.UtcNow;

        await _roleRepository.AddAsync(role, cancellationToken);
        await _roleRepository.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Created new role: {RoleName} with Id: {RoleId}", role.Name, role.Id);

        return _mapper.Map<RoleDto>(role);
    }

    public async Task<RoleDto> UpdateAsync(long id, UpdateRoleDto dto, CancellationToken cancellationToken = default)
    {
        var role = await _roleRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException($"Không tìm thấy Role với Id: {id}");

        var existingRole = await _roleRepository.ExistsAsync(
                        r => r.Name == dto.Name && r.Id != id,
                        cancellationToken);

        if (existingRole)
        {
            throw new BusinessException($"Role với tên '{dto.Name}' đã tồn tại.");
        }

        _mapper.Map(dto, role);
        role.Name = role.Name.Trim();
        role.Description = role.Description?.Trim();
        role.UpdatedAt = DateTime.UtcNow;

        _roleRepository.Update(role);
        await _roleRepository.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Updated role: {RoleId}", role.Id);

        return _mapper.Map<RoleDto>(role);
    }

    public async Task<bool> DeleteAsync(long id, CancellationToken cancellationToken = default)
    {
        var role = await _roleRepository.GetByIdAsync(id, cancellationToken) ?? throw new NotFoundException($" Role with Id '{id}' does not exist.");


        if ( await _roleRepository.HasAnyPersonAsync(id,cancellationToken))
        {
            throw new BusinessException($"Không thể xóa Role '{role.Name}' vì đang được sử dụng.");
        }

        _roleRepository.Delete(role);
        await _roleRepository.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Deleted role: {RoleId} - {RoleName}", role.Id, role.Name);

        return true;
    }

    public async Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await _roleRepository.ExistsAsync(r => r.Name == name, cancellationToken);
    }
}
