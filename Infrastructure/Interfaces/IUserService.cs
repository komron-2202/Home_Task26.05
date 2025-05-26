using Domain.DTOs.User;
using Domain.Filters;
using Domain.Responses;

namespace Infrastructure.Services;

public interface IUserService
{
    Task<Response<GetUserDto>> Get(string id);
    Task<PagedResponse<List<GetUserDto>>> GetAll(UserFilter filter);
    Task<Response<string>> Add(CreateUserDto input);
    Task<Response<GetUserDto>> Update(string id, UpdateUserDto input);
    Task<Response<string>> Delete(string id);

}