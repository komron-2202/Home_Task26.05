using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;
using Domain.Constants;
using Domain.DTOs.Auth;
using Domain.DTOs.Email;
using Domain.Responses;
using Infrastructure.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace Infrastructure.Services;

public class AuthService(
    ILogger<AuthService> logger,
    UserManager<IdentityUser> userManager,
    RoleManager<IdentityRole> roleManager,
    IConfiguration config,
    IEmailService emailService) : IAuthService
{
    public async Task<Response<TokenDto>> Login(LoginDto loginDto)
    {
        try
        {
            logger.LogInformation("Start login");
            var user = await userManager.FindByEmailAsync(loginDto.Email);
            if (user == null)
            {
                return new Response<TokenDto>(HttpStatusCode.BadRequest, "Email or password is incorrect");
            }

            var username = await userManager.FindByNameAsync(loginDto.UserName);
            if (username == null)
            {
                return new Response<TokenDto>(HttpStatusCode.BadRequest, "Username or password is incorrect");
            }

            var checkPassword = await userManager.CheckPasswordAsync(user, loginDto.Password);
            if (!checkPassword)
            {
                return new Response<TokenDto>(HttpStatusCode.BadRequest, "Email or password is incorrect");
            }

            var token = await GenerateJwt(user);
            logger.LogInformation("End login");
            return new Response<TokenDto>(new TokenDto { Token = token });
        }
        catch (Exception e)
        {
            logger.LogError(e, e.Message);
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task<Response<string>> Register(RegisterDto registerDto)
    {
        try
        {
            var user = new IdentityUser
            {
                UserName = registerDto.UserName,
                Email = registerDto.Email,
                PhoneNumber = registerDto.PhoneNumber,
            };

            var result = await userManager.CreateAsync(user, registerDto.Password);

            if (!result.Succeeded)
            {
                return new Response<string>(HttpStatusCode.InternalServerError, "Failed to create user");
            }

            await userManager.AddToRoleAsync(user, Roles.User);
            return new Response<string>("User created");
        }
        catch (Exception e)
        {
            logger.LogError(e, e.Message);
            Console.WriteLine(e);
            throw;
        }
    }
    public async Task<Response<string>> RequestResetPassword(RequestResetPassword requestResetPassword)
    {
        try
        {
            var user = await userManager.FindByEmailAsync(requestResetPassword.Email);
            if (user == null)
            {
                return new Response<string>(HttpStatusCode.NotFound, "User not found");
            }

            var token = await userManager.GeneratePasswordResetTokenAsync(user);

            var emailDto = new EmailDto()
            {
                To = requestResetPassword.Email,
                Subject = "Reset Password",
                Body = $"Your token is {token}",
            };

            var result = await emailService.SendEmailAsync(emailDto);

            return !result
                ? new Response<string>(HttpStatusCode.InternalServerError, "Failed to send email")
                : new Response<string>("Token sent successfully to email");
        }
        catch (Exception e)
        {
            logger.LogError(e, e.Message);
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task<Response<string>> ResetPassword(ResetPasswordDto resetPasswordDto)
    {
        try
        {
            logger.LogInformation("Start reset password");
            var user = await userManager.FindByEmailAsync(resetPasswordDto.Email);
            if (user == null)
            {
                return new Response<string>(HttpStatusCode.NotFound, "User not found");
            }

            var resetResult =
                await userManager.ResetPasswordAsync(user, resetPasswordDto.Token, resetPasswordDto.NewPassword);

            logger.LogInformation("End reset password");
            return resetResult.Succeeded
                ? new Response<string>("Password reset successfully")
                : new Response<string>(HttpStatusCode.InternalServerError, "Failed to reset password");
        }
        catch (Exception e)
        {
            logger.LogError(e, e.Message);
            Console.WriteLine(e);
            throw;
        }
    }

    private async Task<string> GenerateJwt(IdentityUser user)
    {
        try
        {
            logger.LogInformation("Start generate jwt");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]!));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.MobilePhone, user.PhoneNumber ?? "")
            };

            var roles = await userManager.GetRolesAsync(user);
            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

            var token = new JwtSecurityToken(
                issuer: config["Jwt:Issuer"],
                audience: config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: credentials
            );

            logger.LogInformation("End generate jwt");
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        catch (Exception e)
        {
            logger.LogError(e, e.Message);
            Console.WriteLine(e);
            throw;
        }
    }
}