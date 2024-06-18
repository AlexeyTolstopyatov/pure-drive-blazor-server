using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing.Constraints;
using Microsoft.Identity.Client.Extensions.Msal;
using puredrive.Models;
using puredrive.Services.Base;
using System.IO;
using System.Security.Cryptography;
using System.Xml;
using System.Xml.Xsl;
using server = puredrive.Data;

using fs = System.IO;

namespace puredrive.Services
{

    public class DriveObjectApi : IDriveObject
    {
        // о файловой системе....
        // 
        // Необходимо собрать слой взаимодействия с файловой системой по указанному пути
        //  * собрать данные о XSL XSD разметках, существующих в системе.
        //  * получить данные одного документа на основании мета-данных из БД
        //  * создать/загрузить файл на основе зашифрованных мета-данных, добавленных в БД
        //  * загрузить файл на диск с расшифрованным названием, на основе мета-данных из БД

        
        /// <summary>
        /// Скачивает указанный файл
        /// </summary>
        /// <param name="path">путь до файла</param>
        /// <returns></returns>
        public static Task<TaskReport> Download(string path) 
        {

            return Task.FromResult(TaskReport.CompletedTask);
        }

        /// <summary>
        /// Создает копию загруженного файла на сервере. (Загружает файл в webroot)
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public static async Task<TaskReport> Upload(IBrowserFile file, string path)
        {
            // save copy to webroot
            // 1) стереть все метаданные. 
            int result =
                await DataObjectApi.Count("Documents") + 1;

            path = $"D:\\FILES\\{result}.{result}";

            string check = 
                await GetSha1Checksum(file);

            await DataObjectApi.ExecuteAsync(
                "INSERT INTO [Documents] (ID, [Name], [DType], [Check], [Size])" + 
                $"VALUES ({result}, '{file.Name}', '{file.ContentType}', '{check}', {file.Size})"
            );

            // 2) записать контент
            using (FileStream fstream = new FileStream(path, FileMode.Create))
            {
                await file.OpenReadStream().CopyToAsync(fstream);
            }

            return TaskReport.CompletedTask;
        }

        /// <summary>
        /// Загружает на сервер специальные файлы представления
        /// </summary>
        /// <param name="file">IBrowserFile, полученный из компьютера</param>
        /// <param name="view">XSLT?</param>
        /// <returns>Нифига не возвращает. Если view = false, функция определит его как XSD. Потому что есть ТОЛЬКО ДВА СТУЛА...</returns>
        public static async Task<TaskReport> UploadSchema(IBrowserFile file, bool view)
        {
            int next = 
                await DataObjectApi.Count("styles") + 1;

            if (view == true)
            await DataObjectApi.ExecuteAsync(
                "INSERT INTO [styles] (ID, [file], [ext])" + 
                $"VALUES ({next}, '{file.Name}', '{file.ContentType}')"
            );
            else
                await DataObjectApi.ExecuteAsync(
                "INSERT INTO [schemas] (ID, [file], [ext])" +
                $"VALUES ({next}, '{file.Name}', '{file.ContentType}')"
            );


            string path = $"D:\\FILES\\{next}";

            if (view == true)
                path += ".v"; // Представление XML документа (XSLT)
            else
                path += ".d"; // Определение XML документа (XSD)

            using (FileStream fstream = new FileStream(path, FileMode.Create))
            {
                await file.OpenReadStream().CopyToAsync(fstream);
            }

            return TaskReport.CompletedTask;
        }


        /// <summary>
        /// Переобразует XML документ на основе XSLT в HTML документ
        /// </summary>
        /// <param name="styleIndex"></param>
        /// <returns>HTML документ</returns>
        public static Task<MarkupString> GetPreview(int styleIndex)
        {
            MarkupString document;
            if (styleIndex == 0) styleIndex = 1;

            
                XslCompiledTransform transform = new XslCompiledTransform();
                XmlDocument xml = new XmlDocument();

                // ад начинается прямо сейсас...

                xml.Load($"D:\\FILES\\{server.Storage.FileID}.{server.Storage.FileID}"); 
                transform.Load($"D:\\FILES\\{styleIndex}.v");


                StringWriter results = new StringWriter();
                using (XmlReader reader = XmlReader.Create($"D:\\FILES\\{server.Storage.FileID}.{server.Storage.FileID}"))
                {
                    transform.Transform(reader, null, results);
                }

                document = new MarkupString(results.ToString());
            
            
            return Task.FromResult(document);
        }

        /// <summary>
        /// Создает контрольную сумму файла
        /// </summary>
        /// <param name="file">путь файла</param>
        /// <returns></returns>
        public static async Task<string> GetSha1Checksum(IBrowserFile file)
        {
            byte[] bytes = new byte[file.Size];

            using (var fileContent = new StreamContent(file.OpenReadStream(file.Size)))
            {
                bytes = await fileContent.ReadAsByteArrayAsync();
            }

            return BitConverter.ToString(SHA1.Create().ComputeHash(bytes));
        }

        /// <summary>
        /// Читает файл ассоциаций
        /// </summary>
        /// <param name="path">Путь до файла</param>
        /// <returns>Массив прочтенных ассоциаций</returns>
        public static Task<string[]> GetAssociations(string path)
        {
            List<string> assocs = new List<string>();
            
            foreach (string assoc in fs.File.ReadLines(path))
                assocs.Add(assoc);
            

            return Task.FromResult(assocs.ToArray());
        }
    }
}
