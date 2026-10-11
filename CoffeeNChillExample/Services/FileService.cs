using CoffeeNChillExample.Models;
using System;
using System.Collections.Generic;
using System.Text;
using Azure.Storage.Files.Shares;
using Azure.Storage.Files.Shares.Models;
using System;
using System.Collections.Generic;
using System.IO;

namespace CoffeeNChillExample.Services
{
    public class FileService //(IIE,2026)
    {
       
        private readonly ShareClient _shareClient;

       
        public FileService(string connectionString, string shareName)
        {
            _shareClient = new ShareClient(connectionString, shareName);

          
            _shareClient.CreateIfNotExists();
        }

        // Upload a file
        public async Task UploadFileAsync(Stream fileStream, string fileName)
        {
            ShareDirectoryClient directoryClient =
                _shareClient.GetRootDirectoryClient();

            ShareFileClient fileClient =
                directoryClient.GetFileClient(fileName);

            await fileClient.CreateAsync(fileStream.Length);

            await fileClient.UploadAsync(fileStream);
        }

        // Get all files
        public async Task<List<StaffDocument>> GetAllFilesAsync()
        {
            var allFiles = new List<StaffDocument>();

            ShareDirectoryClient directoryClient =
                _shareClient.GetRootDirectoryClient();

            await foreach (ShareFileItem item in directoryClient.GetFilesAndDirectoriesAsync())
            {
                if (!item.IsDirectory)
                {
                    ShareFileClient fileClient =
                        directoryClient.GetFileClient(item.Name);

                    var properties = await fileClient.GetPropertiesAsync();

                    StaffDocument document = new StaffDocument();

                    document.FileName = item.Name;
                    document.Size = properties.Value.ContentLength;
                    document.LastModified = properties.Value.LastModified;

                    allFiles.Add(document);
                }
            }

            return allFiles;
        }

        // Download a file
        public async Task<Stream?> DownloadFileAsync(string fileName) //(IIE,2026)
        {
            ShareDirectoryClient directoryClient =
                _shareClient.GetRootDirectoryClient();

            ShareFileClient fileClient =
                directoryClient.GetFileClient(fileName);

            if (!await fileClient.ExistsAsync())
            {
                return null;
            }

            var download = await fileClient.DownloadAsync();

            return download.Value.Content;
        }
    }
}
//FULL REFERENCE LIST
//IIE, 2026. Cloud Development B: Lecture notes distributed in CLDV6212 Learning Unit 1. https://mystudies.iie.edu.za/d2l/le/lessons/81852/units/5622998  
//IIE, 2026. Cloud Development B: Lecture notes distributed in CLDV6212 Learning Unit 2. https://mystudies.iie.edu.za/d2l/le/lessons/81852/units/5622999 
//IIE, 2026. Cloud Development B: Lecture notes distributed in CLDV6212 Learning Unit 3. https://mystudies.iie.edu.za/d2l/le/lessons/81852/units/5623000



