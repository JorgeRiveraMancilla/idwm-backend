using Mapster;
using Tienda_UCN_api.src.Domain.Models;
using Tienda_UCN_api.Src.Application.DTO.AuthDTO;

namespace Tienda_UCN_api.Src.Application.Mappers
{
    /// <summary>
    /// Clase para mapear objetos de tipo DTO a User y viceversa.
    /// </summary>
    public class UserMapper
    {

        public UserMapper() { }


        /// <summary>
        /// Configura el mapeo de RegisterDTO a User.
        /// </summary>
        public void ConfigureAllMappings()
        {
            ConfigureAuthMappings();
        }

        /// <summary>
        /// Configura el mapeo de RegisterDTO a User.
        /// </summary>
        public void ConfigureAuthMappings()
        {
            TypeAdapterConfig<RegisterDTO, User>.NewConfig()
                .Map(dest => dest.UserName, src => src.Email)
                .Map(dest => dest.Email, src => src.Email)
                .Map(dest => dest.FirstName, src => src.FirstName)
                .Map(dest => dest.LastName, src => src.LastName)
                .Map(dest => dest.Rut, src => src.Rut)
                .Map(dest => dest.BirthDate, src => DateTime.SpecifyKind(src.BirthDate, DateTimeKind.Utc))
                .Map(dest => dest.PhoneNumber, src => src.PhoneNumber)
                .Map(dest => dest.EmailConfirmed, src => false);
        }
    }
}
