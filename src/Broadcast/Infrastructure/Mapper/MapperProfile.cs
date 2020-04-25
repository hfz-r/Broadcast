﻿using System;
using System.DirectoryServices.AccountManagement;
using System.Globalization;
using System.Linq;
using AutoMapper;
using Broadcast.Core.Domain.Messages;
using Broadcast.Core.Domain.Users;
using Broadcast.Core.Infrastructure.Mapper;
using Broadcast.Dtos.Messages;
using Broadcast.Dtos.Users;
using Broadcast.Services.Auth;

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
            CreateUserPrincipalMap();
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
                .ForMember(x => x.FileId, y => y.MapFrom(src => src.Id))
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
                .ForMember(x => x.AuthorDto, y => y.MapFrom(src => src.Author.ToDto<User>()))
                .ForMember(x => x.PreferencesDto, y => y.MapFrom(src => src.Preference.ToDto<Preference>()));
        }

        private void CreateUserMap()
        {
            CreateMap<User, UserDto>()
                .IgnoreAllNonExisting()
                .ForMember(x => x.Guid, y => y.MapFrom(src => src.Guid))
                .ForMember(x => x.Username, y => y.MapFrom(src => src.AccountName))
                .ForMember(x => x.FullName, y => y.MapFrom(src => src.Name))
                .ForMember(x => x.GivenName, y => y.MapFrom(src => src.GivenName))
                .ForMember(x => x.Email, y => y.MapFrom(src => src.Email))
                .ForMember(x => x.Phone, y => y.MapFrom(src => src.PhoneNumber))
                .ForMember(x => x.Image, y => y.MapFrom(src => Convert.ToBase64String(src.Photo)))
                .ForMember(x => x.Department, y => y.MapFrom(src => src.Department))
                .ForMember(x => x.Designation, y => y.MapFrom(src => src.Title))
                .ForMember(x => x.Roles, y => y.MapFrom(src => src.UserRoles.Select(r => r.Role.Name)));
        }

        private void CreateUserPrincipalMap()
        {
            CreateMap<UserPrincipal, User>()
                .IgnoreAllNonExisting()
                .ForMember(x => x.Guid, y => y.MapFrom(src => src.Guid))
                .ForMember(x => x.AccountName, y => y.MapFrom(src => src.SamAccountName))
                .ForMember(x => x.PrincipalName, y => y.MapFrom(src => src.UserPrincipalName))
                .ForMember(x => x.Name, y => y.MapFrom(src => src.Name))
                .ForMember(x => x.GivenName, y => y.MapFrom(src => src.GivenName))
                .ForMember(x => x.Email, y => y.MapFrom(src => src.EmailAddress))
                .ForMember(x => x.PhoneNumber, y => y.MapFrom(src => src.VoiceTelephoneNumber))
                .ForMember(x => x.Photo, y => y.MapFrom(src => Convert.FromBase64String(src.GetProperty("thumbnailPhoto"))))
                .ForMember(x => x.Company, y => y.MapFrom(src => src.GetProperty("company")))
                .ForMember(x => x.Department, y => y.MapFrom(src => src.GetProperty("department")))
                .ForMember(x => x.Title, y => y.MapFrom(src => src.GetProperty("title").ToTitleCase()));
        }

        public int Order => 0;
    }

    internal static class StringExtensions
    {
        public static string ToTitleCase(this string str)
        {
            if (string.IsNullOrEmpty(str)) throw new ArgumentNullException(nameof(str));

            var textInfo = new CultureInfo("en-US", false).TextInfo;
            return textInfo.ToTitleCase(str.ToLower());
        }
    }
}