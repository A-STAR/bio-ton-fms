﻿using BioTonFMS.Telematica.Dtos;
using FluentValidation;

namespace BioTonFMS.Telematica.Validation
{
    public class CreateVehicleDtoValidator : AbstractValidator<CreateVehicleDto>
    {
        public CreateVehicleDtoValidator()
        {
            RuleFor(x => x.Name).NotEmpty().Length(1, 100);
            RuleFor(x => x.Type).NotEmpty();
            RuleFor(x => x.VehicleGroupId).NotEmpty();
            RuleFor(x => x.Make).NotEmpty().Length(1, 30);
            RuleFor(x => x.Model).NotEmpty().Length(1, 30);
            RuleFor(x => x.SubType).NotEmpty();
            RuleFor(x => x.FuelTypeId).NotEmpty();
            RuleFor(x => x.InventoryNumber).Length(1, 30);
            RuleFor(x => x.RegistrationNumber).Length(1, 15);
            RuleFor(x => x.SerialNumber).Length(1, 40);
            RuleFor(x => x.Description).Length(1, 500);
        }
    }
}
