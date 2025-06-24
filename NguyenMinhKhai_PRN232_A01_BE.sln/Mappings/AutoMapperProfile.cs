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
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category != null ? src.Category.Name : string.Empty))
                .ForMember(dest => dest.AccountName, opt => opt.MapFrom(src => src.Account != null ? src.Account.FullName : string.Empty))
                .ForMember(dest => dest.Tags, opt => opt.MapFrom(src => src.Tags.Select(t => t.Name)))
                .ForMember(dest => dest.TagIds, opt => opt.MapFrom(src => src.Tags.Select(t => t.TagId)))
                .ForMember(dest => dest.UpdatedDate, opt => opt.MapFrom(src => src.UpdatedDate))
                .ForMember(dest => dest.ViewCount, opt => opt.MapFrom(src => src.ViewCount))
                .ForMember(dest => dest.Thumbnail, opt => opt.MapFrom(src => (string?)null)); // No thumbnail field in current model
            CreateMap<CreateNewsDTO, News>();
            CreateMap<UpdateNewsDTO, News>();

            // Tag mappings
            CreateMap<Tag, TagDTO>();
            CreateMap<CreateTagDTO, Tag>();
            CreateMap<UpdateTagDTO, Tag>();
        }
    }
} 