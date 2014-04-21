using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TranspilerUtils.Utils
{
    public class MapperHelper
    {
        public static void CreateMappings()
        {
            Mapper.CreateMap<Mordritch.Transpiler.Contracts.JavaClass, TranspilerUtils.JavaClass.JavaClassModel>();
            Mapper.CreateMap<Mordritch.Transpiler.Contracts.FieldDetail, TranspilerUtils.JavaClass.FieldDetailModel>();
            Mapper.CreateMap<string, TranspilerUtils.JavaClass.MethodDependancyModel>().ConvertUsing(s => new TranspilerUtils.JavaClass.MethodDependancyModel() { Name = s });
            Mapper.CreateMap<Mordritch.Transpiler.Contracts.MethodDetail, TranspilerUtils.JavaClass.MethodDetailModel>()
                .ForMember(dest => dest.DependantOn, opt => opt.MapFrom(src => src.DependantOn));

            Mapper.CreateMap<TranspilerUtils.JavaClass.JavaClassModel, Mordritch.Transpiler.Contracts.JavaClass>();
            Mapper.CreateMap<TranspilerUtils.JavaClass.FieldDetailModel, Mordritch.Transpiler.Contracts.FieldDetail>();
            Mapper.CreateMap<TranspilerUtils.JavaClass.MethodDependancyModel, string>().ConvertUsing(s => s.Name);
            Mapper.CreateMap<TranspilerUtils.JavaClass.MethodDetailModel, Mordritch.Transpiler.Contracts.MethodDetail>()
                .ForMember(dest => dest.DependantOn, opt => opt.MapFrom(src => src.DependantOn));


        }
    }
}
