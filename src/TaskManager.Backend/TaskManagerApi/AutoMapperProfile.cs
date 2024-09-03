using Authentication.Models;
using AutoMapper;
using TaskManagerApi.Domain.Dtos.Auth;
using TaskManagerApi.Domain.Dtos.Task;
using TaskManagerApi.Domain.Entities;
using TaskManagerApi.Domain.Models;
using Task = TaskManagerApi.Domain.Entities.Task;

namespace TaskManagerApi
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<User, UserRegistrationRequest>();
            CreateMap<UserRegistrationRequest, User>();
            CreateMap<AccessTokenData, AuthToken>();
            CreateMap<AuthToken, AccessTokenData>();
            CreateMap<UserUpdateDataRequest, UserUpdateData>();

            CreateMap<CreateTaskRequest, Task>();
            CreateMap<UpdateTaskRequest, Task>();
            CreateMap<Task, TaskResponse>();

        }
    }
}