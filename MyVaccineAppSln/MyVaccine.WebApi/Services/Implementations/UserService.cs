﻿using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using Azure.Core;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MyVaccine.WebApi.Dtos;
using MyVaccine.WebApi.Literals;
using MyVaccine.WebApi.Models;
using MyVaccine.WebApi.Repositories.Contracts;
using MyVaccine.WebApi.Services.Contracts;

namespace MyVaccine.WebApi.Services.Implementations;

public class UserService : IUserService
{
    private readonly UserManager<User> _userManager;
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;
    public UserService(UserManager<User> userManager, IUserRepository userRepository, IMapper mapper)
    {
        _userManager = userManager;
        _userRepository = userRepository;
        _mapper = mapper;
    }
    public async Task<AuthResponseDto> AddUserAsync(RegisterRequetDto request)
    {
        var response = new AuthResponseDto();
        try
        {
            var result = await _userRepository.AddUser(request);

            if (result != null)
            {
                response.IsSuccess = result.Succeeded;
                response.Errors = result?.Errors?.Select(x => x.Description).ToArray() ?? new string[] { };
            }

        }
        catch (Exception ex)
        {
            response.IsSuccess = false;
            response.Errors = new string[] { ex.Message };
        }

        return response;
    }

    public async Task<AuthResponseDto> Login(LoginRequestDto request)
    {
        var response = new AuthResponseDto();
        try
        {
            var user = await _userManager.FindByNameAsync(request.Username);

            if (user != null && await _userManager.CheckPasswordAsync(user, request.Password))
            {
                var claims = new[]
                {
                    new Claim(ClaimTypes.Name, user.UserName)
                };

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(/*Environment.GetEnvironmentVariable(MyVaccineLiterals.JWT_KEY)*/ "A2*7gF9@D#1hJ$5mNpRtVwYzZ&k8zbx"));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var token = new JwtSecurityToken(
                    //issuer: _configuration["JwtIssuer"],
                    //audience: _configuration["JwtAudience"],
                    claims: claims,
                    expires: DateTime.Now.AddMonths(15),
                    signingCredentials: creds
                );

                var tokenresult = new JwtSecurityTokenHandler().WriteToken(token);
                response.Token = tokenresult;
                response.Expiration = token.ValidTo;
                response.IsSuccess = true;
            }
            else
            {
                response.IsSuccess = false;
            }
        }
        catch (Exception ex)
        {
            response.IsSuccess = false;
            response.Errors = new string[] { ex.Message };
        }

        return response;

    }
    public async Task<AuthResponseDto> RefreshToken(string email)
    {
        var response = new AuthResponseDto();
        try
        {
            var user = await _userManager.FindByNameAsync(email);

            if (user != null)
            {
                var claims = new[]
                {
                    new Claim(ClaimTypes.Name, user.UserName)
                };

                //var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable(MyVaccineLiterals.JWT_KEY)));
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(/*Environment.GetEnvironmentVariable(MyVaccineLiterals.JWT_KEY)*/ "A2*7gF9@D#1hJ$5mNpRtVwYzZ&k8zbx"));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var token = new JwtSecurityToken(
                    //issuer: _configuration["JwtIssuer"],
                    //audience: _configuration["JwtAudience"],
                    claims: claims,
                    expires: DateTime.Now.AddMonths(15),
                    signingCredentials: creds
                );

                var tokenresult = new JwtSecurityTokenHandler().WriteToken(token);
                response.Token = tokenresult;
                response.Expiration = token.ValidTo;
                response.IsSuccess = true;
            }
            else
            {
                response.IsSuccess = false;
            }
        }
        catch (Exception ex)
        {
            response.IsSuccess = false;
            response.Errors = new string[] { ex.Message };
        }

        return response;

    }

    public async Task<UserDto> GetUserInfo(string email)
    {
        var user = await _userManager.FindByNameAsync(email);
        var userFull = await _userRepository.FindByAsNoTracking(x => x.Id == user.Id).Include(x=>x.FamilyGroups).FirstOrDefaultAsync();
        var response = _mapper.Map<UserDto>(userFull);
        //var response = await _userManager.FindByIdAsync(x => x. == user.Id).FirstOrDefaultAsync();
        return response;
    }
}
