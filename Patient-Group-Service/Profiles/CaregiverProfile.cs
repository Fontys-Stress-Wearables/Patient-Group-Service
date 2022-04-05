﻿using AutoMapper;
using Patient_Group_Service.Dtos;
using Patient_Group_Service.Models;

namespace Patient_Group_Service.Profiles;

public class CaregiverProfile : Profile
{
    public CaregiverProfile()
    {
        CreateMap<Caregiver, CaregiverDTO>();

    }
}