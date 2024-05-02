﻿using AutoMapper;
using EventlyBackEnd.Models.DTOs;
using EventlyBackEnd.Models.Entities;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<UserDTO, User>(); // Define mapping from UserDTO to User
        CreateMap<Event, EventDTO>();
        CreateMap<EventDTO, Event>();
    }
}
