using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Serilog;
using SkiaSharp;
using Tienda_UCN_api.src.Domain.Models;
using Tienda_UCN_api.Src.Application.Services.Interfaces;
using Tienda_UCN_api.Src.Infrastructure.Repositories.Interfaces;

namespace Tienda_UCN_api.Src.Application.Services.Implements
{
    /// <summary>
    /// Servicio para manejar archivos en Cloudinary.
    /// </summary>
    public class FileService : IFileService
    {
        private readonly IConfiguration _configuration;
        private readonly Cloudinary _cloudinary;
        private readonly string[] _allowedExtensions;
        private readonly int _maxFileSizeInBytes;
        private readonly IFileRepository _fileRepository;
        private readonly string _cloudName;
        private readonly string _cloudApiKey;
        private readonly string _cloudApiSecret;
        public FileService(IConfiguration configuration, IFileRepository fileRepository)
        {
            _configuration = configuration;
            _fileRepository = fileRepository;
            _cloudName = _configuration["Cloudinary:CloudName"] ?? throw new InvalidOperationException("La configuración de CloudName es obligatoria");
            _cloudApiKey = _configuration["Cloudinary:ApiKey"] ?? throw new InvalidOperationException("La configuración de ApiKey es obligatoria");
            _cloudApiSecret = _configuration["Cloudinary:ApiSecret"] ?? throw new InvalidOperationException("La configuración de ApiSecret es obligatoria");
            Account account = new Account(_cloudName, _cloudApiKey, _cloudApiSecret);
            _cloudinary = new Cloudinary(account);
            _cloudinary.Api.Secure = true; // Aseguramos  que las URLs sean seguras con HTTPS
            _allowedExtensions = _configuration.GetSection("Products:AllowedExtensions").Get<string[]>() ?? throw new InvalidOperationException("La configuración de las extensiones de las imágenes es obligatoria");
            if (!int.TryParse(_configuration["Products:ImageMaxSizeInBytes"], out _maxFileSizeInBytes)) { throw new InvalidOperationException("La configuración del tamaño de la imagen es obligatoria"); }
        }

        /// <summary>
        /// Sube un archivo a Cloudinary.
        /// </summary>
        /// <param name="file">El archivo a subir.</param>
        /// <param name="productId">El ID del producto al que pertenece la imagen.</param>
        /// <returns>URL del archivo subido.</returns>
        public async Task<string> UploadAsync(IFormFile file, int productId)
        {
            if (productId <= 0)
            {
                Log.Error($"ProductId inválido: {productId}");
                throw new ArgumentException("ProductId debe ser mayor a 0");
            }

            if (file == null || file.Length == 0)
            {
                Log.Error("Intento de subir un archivo nulo o vacío");
                throw new ArgumentException("Archivo inválido");
            }

            if (file.Length > _maxFileSizeInBytes)
            {
                Log.Error($"El archivo {file.FileName} excede el tamaño máximo permitido de {_maxFileSizeInBytes / 1024 / 1024} MB");
                throw new ArgumentException($"El archivo excede el tamaño máximo permitido de {_maxFileSizeInBytes / 1024 / 1024} MB");
            }

            var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();

            if (!_allowedExtensions.Contains(fileExtension))
            {
                Log.Error($"Extensión de archivo no permitida: {fileExtension}");
                throw new ArgumentException($"Extensión de archivo no permitida. Permitir: {string.Join(", ", _allowedExtensions)}");
            }

            if (!IsValidImageFile(file))
            {
                Log.Error($"El archivo {file.FileName} no es una imagen válida");
                throw new ArgumentException("El archivo no es una imagen válida");
            }
            var folder = $"product/{productId}/images";
            using var stream = file.OpenReadStream();

            var uploadParams = new ImageUploadParams()
            {
                Folder = folder,
                File = new FileDescription(file.FileName, stream),
                UseFilename = true,
                UniqueFilename = true
            };

            Log.Information($"Optimizando imagen: {file.FileName} antes de subir a la nube");
            uploadParams.Transformation = new Transformation()
                .Width(1000).Crop("scale").Chain()
                .Quality("auto:best").Chain()
                .FetchFormat("auto");

            Log.Information($"Subiendo imagen: {file.FileName} a Cloudinary");
            var uploadResult = await _cloudinary.UploadAsync(uploadParams);

            if (uploadResult.Error != null)
            {
                Log.Error($"Hubo un error al subir la imagen: {uploadResult.Error.Message}");
                throw new Exception($"Error al subir la imagen: {uploadResult.Error.Message}");
            }

            var image = new Image()
            {
                PublicId = uploadResult.PublicId,
                ImageUrl = uploadResult.SecureUrl.ToString(),
                ProductId = productId
            };

            var result = await _fileRepository.CreateAsync(image);
            if (!result)
            {
                Log.Error($"Error al guardar la imagen en la base de datos: {file.FileName}");
                await DeleteAsync(uploadResult.PublicId); // Eliminamos la imagen de Cloudinary si falla la creación de la imagen en la bdd
                throw new Exception("Error al guardar la imagen en la base de datos");
            }
            Log.Information($"Imagen subida exitosamente: {uploadResult.SecureUrl}");
            return uploadResult.SecureUrl.ToString();
        }

        /// <summary>
        /// Elimina un archivo de Cloudinary.
        /// </summary>
        /// <param name="publicId">El ID público del archivo a eliminar.</param>
        /// <returns>True si la eliminación fue exitosa, de lo contrario false.</returns>
        public async Task<bool> DeleteAsync(string publicId)
        {
            var deletionParams = new DeletionParams(publicId);
            Log.Information($"Eliminando imagen con PublicId: {publicId} de Cloudinary");
            var deleteResult = await _cloudinary.DestroyAsync(deletionParams);
            if (deleteResult.Error != null)
            {
                Log.Error($"Error al eliminar la imagen con PublicId: {publicId} de Cloudinary: {deleteResult.Error.Message}");
                throw new Exception($"Error al eliminar la imagen: {deleteResult.Error.Message}");
            }
            Log.Information($"Imagen con PublicId: {publicId} eliminada exitosamente de Cloudinary");
            return await _fileRepository.DeleteAsync(publicId);
        }

        private bool IsValidImageFile(IFormFile file)
        {
            try
            {
                using var stream = file.OpenReadStream();
                using var skiaStream = new SKManagedStream(stream); // SkiaSharp es una librería de procesamiento de imágenes (dotnet add package SkiaSharp)
                using var codec = SKCodec.Create(skiaStream); // Crea un codec para leer la imagen (codificador/decodificador)

                return codec != null && codec.Info.Width > 0 && codec.Info.Height > 0;
            }
            catch (Exception ex)
            {
                Log.Warning($"Error validando imagen {file.FileName}: {ex.Message}");
                return false;
            }
        }
    }
}
