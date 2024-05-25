using AutoMapper;
using EventlyBackEnd.Models.DTOs;
using EventlyBackEnd.Models.Entities;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<UserDTO, User>(); // Define mapping from UserDTO to User
        CreateMap<Event, EventDTO>().ReverseMap();
        CreateMap<EventDTO, Event>();
        CreateMap<Post, PostDTO>()
            .PreserveReferences()
            .ReverseMap()
            .PreserveReferences();
        CreateMap<Post, CreatePostDTO>().ReverseMap();
        CreateMap<Event, CreateEventDTO>().ReverseMap();
        CreateMap<EventDTO, CreateEventDTO>().ReverseMap();
    }
}
