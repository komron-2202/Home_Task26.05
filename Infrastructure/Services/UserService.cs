using System.Net;
using AutoMapper;
using Domain.Responses;
using Microsoft.AspNetCore.Identity;
using Domain.Constants;
using Domain.DTOs.Email;
using Domain.DTOs.User;
using Domain.Filters;
using Infrastructure.Data;
using Infrastructure.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services;

public class UserService(
    IEmailService service,
    DataContext context,
    UserManager<IdentityUser> userManager,
    RoleManager<IdentityRole> roleManager,
    IMapper mapper,
    IConfiguration configuration,
    ILogger<UserService> logger) : IUserService
{
    public async Task<Response<string>> Add(CreateUserDto input)
    {
        try
        {
            logger.LogInformation($"Adding new user {input.Email}");
            var user = new IdentityUser
            {
                UserName = input.UserName,
                Email = input.Email,
                PhoneNumber = input.Phone
            };

            var result = await userManager.CreateAsync(user, input.Password);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return new Response<string>(HttpStatusCode.BadRequest, $"User not created: {errors}");
            }

            await userManager.AddToRoleAsync(user, input.Role);

            var emailDto = new EmailDto
            {
                To = input.Email,
                Subject = "Your registration details",
                Body =
                    $"Welcome {input.UserName}! Your password is: {input.Password} , You have Strong side, you are {input.Role}'"
            };

            var res = await service.SendEmailAsync(emailDto);

            logger.LogInformation($"User with this email {input.Email} created");
            return !res
                ? new Response<string>(HttpStatusCode.BadRequest, "User created, but email failed to send.")
                : new Response<string>(HttpStatusCode.OK, "User created and email sent successfully.");
        }
        catch (Exception e)
        {
            logger.LogError(e, e.Message);
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task<Response<GetUserDto>> Update(string id, UpdateUserDto request)
    {
        try
        {
            logger.LogInformation($"Updating user {id}");
            var Users = await userManager.FindByIdAsync(id);
            if (Users == null)
            {
                return new Response<GetUserDto>(HttpStatusCode.NotFound, "Id in not found");
            }

            Users.Email = request.Email;
            Users.UserName = request.Email;
            Users.PhoneNumber = request.Phone;

            var res = await userManager.UpdateAsync(Users);

            var data = mapper.Map<GetUserDto>(Users);

            logger.LogInformation($"User with this email {request.Email} updated");
            return res == null
                ? new Response<GetUserDto>(HttpStatusCode.BadRequest, "User not Updated!")
                : new Response<GetUserDto>(data);
        }
        catch (Exception e)
        {
            logger.LogError(e, e.Message);
            Console.WriteLine(e);
            throw;
        }
    }

    [Authorize]
    public async Task<Response<string>> Delete(string id)
    {
        try
        {
            logger.LogInformation($"Deleting user {id}");
        var User = await userManager.FindByIdAsync(id);
        if (User == null)
        {
            return new Response<string>(HttpStatusCode.NotFound, "Not found");
        }

        var res = await userManager.DeleteAsync(User);

        logger.LogInformation($"User with this email {id} deleted");
        return res == null
            ? new Response<string>(HttpStatusCode.BadRequest, "Not found ")
            : new Response<string>("User deleted !");

        }
        catch (Exception e)
        {
            logger.LogError(e, e.Message);
            Console.WriteLine(e);
            throw;
        }
    }

    [Authorize]
    public async Task<PagedResponse<List<GetUserDto>>> GetAll(UserFilter filter)
    {
        try
        {
        logger.LogInformation($"Getting all users");
        var validFilter = new ValidFilter(filter.PagesNumber, filter.PageSize);

        var users = context.Users.AsQueryable();

        if (filter.Name != null)
        {
            users = users.Where(u => u.UserName.Contains(filter.Name));
        }

        var mapped = mapper.Map<List<GetUserDto>>(users);

        var totalRecords = mapped.Count();

        var data = mapped
            .Skip((validFilter.PageNumber - 1) * validFilter.PageSize)
            .Take(validFilter.PageSize)
            .ToList();

        logger.LogInformation($"Total records: {totalRecords}");
        return new PagedResponse<List<GetUserDto>>(data, validFilter.PageNumber, validFilter.PageSize,
            totalRecords);
        }
        catch (Exception e)
        {
            logger.LogError(e, e.Message);
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task<Response<GetUserDto>> Get(string id)
    {
        try
        {
            logger.LogInformation("Getting user");
        var Users = await userManager.FindByIdAsync(id);
        if (Users == null)
        {
            return new Response<GetUserDto>(HttpStatusCode.NotFound, "Id in not found");
        }

        var data = mapper.Map<GetUserDto>(Users);
        logger.LogInformation($"User with this email {id} retrieved");
        return new Response<GetUserDto>(data);

        }
        catch (Exception e)
        {
            logger.LogError(e, e.Message);
            Console.WriteLine(e);
            throw;
        }
    }
}