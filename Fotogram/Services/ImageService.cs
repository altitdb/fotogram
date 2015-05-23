using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Security.Policy;
using System.Web.Http.Routing;
using System.Web.UI.WebControls;
using Image = System.Drawing.Image;

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

                var path = string.Format("{0}{1}", serverPath, nomeUsuario);

                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                var imgName = string.Format("{0}.jpg", Guid.NewGuid());

                var newFileNameOnServer = Path.Combine(path, imgName);

                using (Image image = Image.FromStream(new MemoryStream(byteArray)))
                {
                    var imageToSave = ScaleImage(image, 300, 300);
                    imageToSave.Save(newFileNameOnServer, ImageFormat.Jpeg);
                }

                var newFileNameOnDb = string.Format("/Images/Uploads/{0}/{1}", nomeUsuario, imgName);

                return newFileNameOnDb;
            }
            catch (Exception)
            {
                // retorna a exception
                throw;
            }
        }
    }
}
