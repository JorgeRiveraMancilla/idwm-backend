using Mapster;

namespace Tienda_UCN_api.Src.Application.Mappers
{
    /// <summary>
    /// Clase para extensiones de mapeo.
    /// Contiene configuraciones globales de mapeo.
    /// </summary>
    public class MapperExtensions
    {
        /// <summary>
        /// Configura los mapeos globales.
        /// </summary>
        public static void ConfigureMapster(IServiceProvider serviceProvider)
        {
            var productMapper = serviceProvider.GetService<ProductMapper>();
            productMapper?.ConfigureAllMappings();

            var userMapper = serviceProvider.GetService<UserMapper>();
            userMapper?.ConfigureAllMappings();

            // Configuración global de Mapster para ignorar valores nulos
            TypeAdapterConfig.GlobalSettings.Default.IgnoreNullValues(true);
        }
    }
}
