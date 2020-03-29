using System;
using System.Linq;
using AutoMapper;
using Broadcast.Core.Domain.Messages;
using Broadcast.Core.Domain.Users;
using Broadcast.Core.Infrastructure.Mapper;
using Broadcast.Dtos.Messages;
using Broadcast.Dtos.Users;

namespace Broadcast.Infrastructure.Mapper
{
    public class MapperProfile : Profile, IOrderedMapperProfile
    {
        public MapperProfile()
        {
            //message map
            CreateMessageProjectMap();
            CreateMessageAboutMap();
            CreateMessageDetailsMap();
            CreateMessageExtrasMap();
            //message related
            CreateExtrasMap();
            CreatePreferencesMap();
            CreateMessageMap();
            //user
            CreateUserMap();
        }

        private void CreateMessageProjectMap()
        {
            CreateMap<Message, ProjectDto>()
                .IgnoreAllNonExisting()
                .ForMember(x => x.Project, y => y.MapFrom(src => src.Project));
        }

        private void CreateMessageAboutMap()
        {
            CreateMap<Message, AboutDto>()
                .IgnoreAllNonExisting()
                .ForMember(x => x.Title, y => y.MapFrom(src => src.Title))
                .ForMember(x => x.Description, y => y.MapFrom(src => src.Description))
                .ForMember(x => x.StartDate, y => y.MapFrom(src => src.StartDate))
                .ForMember(x => x.EndDate, y => y.MapFrom(src => src.EndDate))
                .ForMember(x => x.Tags, y => y.MapFrom(src => src.MessageTags.Select(tag => tag.Tag.TagName)))
                .ForMember(x => x.Categories, y => y.MapFrom(src => src.MessageCategories.Select(cat => cat.Category.Type)));
        }

        private void CreateMessageDetailsMap()
        {
            CreateMap<Message, DetailsDto>()
                .IgnoreAllNonExisting()
                .ForMember(x => x.Editor, y => y.MapFrom(src => src.Body));
        }

        private void CreateMessageExtrasMap()
        {
            CreateMap<Message, ExtrasDto>()
                .IgnoreAllNonExisting()
                .ForMember(x => x.ExtraFiles, y => y.MapFrom(src => src.Files.Select(file => file.ToDto<FilesDto>())));
        }

        private void CreateExtrasMap()
        {
            CreateMap<File, FilesDto>()
                .IgnoreAllNonExisting()
                .ForMember(x => x.FileName, y => y.MapFrom(src => src.FileName))
                .ForMember(x => x.FileType, y => y.MapFrom(src => src.FileType))
                .ForMember(x => x.FileSize, y => y.MapFrom(src => src.FileSize))
                .ForMember(x => x.FileContent, y => y.MapFrom(src => Convert.ToBase64String(src.FileContent)));
        }

        private void CreatePreferencesMap()
        {
            CreateMap<Preference, PreferencesDto>()
                .IgnoreAllNonExisting()
                .ForMember(x => x.Cb1, y => y.MapFrom(src => src.Cb1))
                .ForMember(x => x.Cb2, y => y.MapFrom(src => src.Cb2));
        }

        private void CreateMessageMap()
        {
            CreateMap<Message, MessageDto>()
                .IgnoreAllNonExisting()
                .ForMember(x => x.MessageId, y => y.MapFrom(src => src.Id))
                .ForMember(x => x.Slug, y => y.MapFrom(src => src.Slug))
                .ForMember(x => x.CreatedAt, y => y.MapFrom(src => src.CreatedAt))
                .ForMember(x => x.UpdatedAt, y => y.MapFrom(src => src.UpdatedAt))
                .ForMember(x => x.ProjectDto, y => y.MapFrom(src => src))
                .ForMember(x => x.AboutDto, y => y.MapFrom(src => src))
                .ForMember(x => x.DetailsDto, y => y.MapFrom(src => src))
                .ForMember(x => x.ExtrasDto, y => y.MapFrom(src => src))
                .ForMember(x => x.AuthorDto, y => y.MapFrom(src => src.Author.ToDto<UserDto>()))
                .ForMember(x => x.PreferencesDto, y => y.MapFrom(src => src.Preference.ToDto<PreferencesDto>()));
        }

        private void CreateUserMap()
        {
            CreateMap<User, UserDto>()
                .IgnoreAllNonExisting()
                .ForMember(x => x.Username, y => y.MapFrom(src => src.Username))
                .ForMember(x => x.FirstName, y => y.MapFrom(src => src.FirstName))
                .ForMember(x => x.LastName, y => y.MapFrom(src => src.LastName))
                .ForMember(x => x.Email, y => y.MapFrom(src => src.Email))
                .ForMember(x => x.Bio, y => y.MapFrom(src => src.Bio))
                .ForMember(x => x.Image, y => y.MapFrom(src => src.Image));
            CreateMap<UserDto, User>()
                .IgnoreAllNonExisting()
                .ForMember(x => x.Username, y => y.MapFrom(src => src.Username))
                .ForMember(x => x.FirstName, y => y.MapFrom(src => src.FirstName))
                .ForMember(x => x.LastName, y => y.MapFrom(src => src.LastName))
                .ForMember(x => x.Email, y => y.MapFrom(src => src.Email))
                .ForMember(x => x.Bio, y => y.MapFrom(src => src.Bio))
                .ForMember(x => x.Image, y => y.MapFrom(src => src.Image));
        }

        public int Order => 0;
    }
}