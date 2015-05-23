using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace Fotogram.Services
{
    /// <summary>
    /// Classe responsável pelo tratamento de imagens
    /// </summary>
    public class ImageService
    {
        /// <summary>
        /// Reescala a imagem mantendo a proporção.
        /// </summary>
        /// <param name="image">Image</param>
        /// <param name="maxWidth">int</param>
        /// <param name="maxHeight">int</param>
        /// <returns>Image</returns>
        public static Image ScaleImage(Image image, int maxWidth, int maxHeight)
        {
            var ratioX = (double)maxWidth / image.Width;
            var ratioY = (double)maxHeight / image.Height;
            var ratio = Math.Min(ratioX, ratioY);

            var newWidth = (int)(image.Width * ratio);
            var newHeight = (int)(image.Height * ratio);

            var newImage = new Bitmap(newWidth, newHeight);
            Graphics.FromImage(newImage).DrawImage(image, 0, 0, newWidth, newHeight);
            return newImage;
        }

        /// <summary>
        /// Grava a imagem enviada no servidor, de acordo com o usuário
        /// </summary>
        /// <param name="nomeUsuario"></param>
        /// <param name="serverPath"></param>
        /// <param name="avatar"></param>
        /// <returns></returns>
        public static string SalvarImagemNoServidor(string nomeUsuario, string serverPath, string avatar)
        {
            try
            {
                var byteArray = Convert.FromBase64String(avatar);

                var path = $"{serverPath}{nomeUsuario}";

                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                var newFileName = Path.Combine(path, $"{Guid.NewGuid()}.jpg");

                using (Image image = Image.FromStream(new MemoryStream(byteArray)))
                {
                    var imageToSave = ScaleImage(image, 300, 300);
                    imageToSave.Save(newFileName, ImageFormat.Jpeg);
                }

                return newFileName;
            }
            catch (Exception)
            {
                // retorna a exception
                throw;
            }
        }
    }
}
