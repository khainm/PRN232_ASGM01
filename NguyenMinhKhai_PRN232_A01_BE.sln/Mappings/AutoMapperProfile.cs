using AutoMapper;
using NguyenMinhKhai_PRN232_A01_BE.sln.Controllers;
using NguyenMinhKhai_PRN232_A01_BE.sln.DTOs;
using NguyenMinhKhai_PRN232_A01_BE.sln.Models;
using System.Linq;

namespace NguyenMinhKhai_PRN232_A01_BE.sln.Mappings
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            // Account mappings
            CreateMap<Account, AccountDTO>();
            CreateMap<AccountDTO, Account>();
            CreateMap<CreateAccountDTO, Account>();
            CreateMap<UpdateAccountDTO, Account>();
            CreateMap<AdminUpdateAccountDTO, Account>();
            CreateMap<RegisterDTO, CreateAccountDTO>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => 1));

            

            // Category mappings
            CreateMap<Category, CategoryDTO>();
            CreateMap<CreateCategoryDTO, Category>();
            CreateMap<UpdateCategoryDTO, Category>();

            // News mappings
            CreateMap<News, NewsDTO>()
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name))
                .ForMember(dest => dest.AccountName, opt => opt.MapFrom(src => src.Account.FullName))
                .ForMember(dest => dest.Tags, opt => opt.MapFrom(src => src.Tags.Select(t => t.Name)));
            CreateMap<CreateNewsDTO, News>();
            CreateMap<UpdateNewsDTO, News>();

            // Tag mappings
            CreateMap<Tag, TagDTO>();
            CreateMap<CreateTagDTO, Tag>();
            CreateMap<UpdateTagDTO, Tag>();
        }
    }
} 