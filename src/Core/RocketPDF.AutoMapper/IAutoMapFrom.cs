using AutoMapper;

namespace RocketPDF.AutoMapper
{
    public interface IAutoMapFrom<T>
        where T : new()
    {
        void Mapping(IMapperConfigurationExpression configuration);
    }
}