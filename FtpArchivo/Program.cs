using FluentFTP;
using Renci.SshNet;
using System.Net;

namespace FtpArchivo
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            //Application.Run(new Form1());

            // Ruta del archivo local que quieres subir
            string localFilePath = @"C:\Users\Exequiel\Desktop\archivo\prueba.txt";

            // Datos del servidor FTP
            string ftpServerUrl = "ftp://127.0.0.1";  // Dirección del servidor FTP (localhost)
            string ftpFilePath = "/ficheroftp";      // Ruta en el servidor FTP
            string ftpUsername = "admin";        // Nombre de usuario FTP
            string ftpPassword = "12345";     // Contraseña FTP

            string ftpUrl = ftpServerUrl + ftpFilePath;

            // Subir archivo al servidor FTP
            //UploadFileToFtp(localFilePath,ftpUrl,ftpUsername, ftpPassword);
            UploadFileWithFluentFtp(localFilePath, ftpServerUrl, ftpUsername, ftpPassword);
        }

        static void UploadFileToFtp(string localFilePath, string ftpUrl, string ftpUsername, string ftpPassword)
        {
            FileInfo fileInfo = new FileInfo(localFilePath);

            // Crear la solicitud FTP
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(ftpUrl);
            request.Method = WebRequestMethods.Ftp.UploadFile;

            // Credenciales del servidor FTP
            request.Credentials = new NetworkCredential(ftpUsername, ftpPassword);

            // Copiar el contenido del archivo a la solicitud
            byte[] fileContents;
            using (FileStream fileStream = fileInfo.OpenRead())
            {
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    fileStream.CopyTo(memoryStream);
                    fileContents = memoryStream.ToArray();
                }
            }

            // Establecer la longitud del contenido a enviar
            request.ContentLength = fileContents.Length;

            // Enviar el archivo al servidor
            using (Stream requestStream = request.GetRequestStream())
            {
                requestStream.Write(fileContents, 0, fileContents.Length);
            }

            // Obtener la respuesta del servidor
            using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
            {
                Console.WriteLine($"Subida completada, estado: {response.StatusDescription}");
            }
        }

        static void UploadFileWithFluentFtp(string localFilePath, string ftpServerUrl, string ftpUsername, string ftpPassword)
        {
            try
            {
                using (FtpClient client = new FtpClient(ftpServerUrl, ftpUsername, ftpPassword))
                {
                    client.Connect();
                    Console.WriteLine(localFilePath);
                    client.UploadFile("ftp://127.0.0.1/ficheroftp", "prueba.txt");
                    Console.WriteLine("Subida completada");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Error interno: {ex.InnerException.Message}");
                }
            }
        }
    }
}