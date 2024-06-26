﻿using MyVaccine.WebApi.Models;
using MyVaccine.WebApi.Repositories.Contracts;
using MyVaccine.WebApi.Repositories.Implementations;
using MyVaccine.WebApi.Services;
using MyVaccine.WebApi.Services.Contracts;
using MyVaccine.WebApi.Services.Implementations;

namespace MyVaccine.WebApi.Configurations.Injections;

public static class DependencyInjections
{
    public static IServiceCollection SetDependencyInjection(this IServiceCollection services)
    {
        #region Repositories Injection
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IBaseRepository<Dependent>, BaseRepository<Dependent>>();
        #endregion

        #region Services Injection

        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IDependentService, DependentService>();
        services.AddScoped<SaveProfileImageUserService, SaveProfileImageUserService>();
        #endregion

        #region Only for  testing propourses
        services.AddScoped<IGuidGeneratorScope, GuidServiceScope>();
        services.AddTransient<IGuidGeneratorTrasient, GuidServiceTransient>();
        services.AddSingleton<IGuidGeneratorSingleton, GuidServiceSingleton>();
        services.AddScoped<IGuidGeneratorDeep, GuidGeneratorDeep>();
        #endregion
        services.SetAllergyInjection();
        services.SetFamilyGroupInjection();
        services.SetUsersAllergyInjection();
        services.SetVaccineCategoryInjection();
        services.SetVaccineRecordInjection();
        services.SetVaccineInjection();
        return services;
    }
}
