using Domain.Constants;
using Domain.DTOs;
using Domain.DTOs.User;
using Domain.Filters;
using Domain.Responses;
using Infrastructure.Interfaces;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController(IUserService service)
{
    [HttpGet("int:id")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<Response<GetUserDto>> Get(string id)
    {
        return await service.Get(id);
    }

    [HttpGet]
    [Authorize(Roles = Roles.Admin)]
    public async Task<PagedResponse<List<GetUserDto>>> GetAll(UserFilter filter)
    {
        return await service.GetAll(filter);
    }

    [HttpPut("int:id")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<Response<GetUserDto>> Update(string id, UpdateUserDto update)
    {
        return await service.Update(id, update);
    }

    [HttpDelete("int:id")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<Response<string>> Delete(string id)
    {
        return await service.Delete(id);
    }

    [HttpPost]
    [Authorize(Roles = Roles.Admin)]
    public async Task<Response<string>> Create(CreateUserDto create)
    {
        return await service.Add(create);
    }
}