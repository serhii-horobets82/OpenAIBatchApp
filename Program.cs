
using System.IO;

namespace OpenAIBatchApp
{

    class Program
    {
        /*
         The app shall upload all OpenAI batch files from a source directory that are newer than their OpenAI counterpart, and create OpenAI batches for them.
         download OpenAI batch results to a target directory. Download only those batch results that are newer than their local copy.
         You can choose to do either 1 or 2, or both.
         The app shall be simple, well structured, easy to understand, resilient and handle errors well.
         */


        static async Task Main(string[] args)
        {
            if (args.Length < 3)
            {
                Console.WriteLine("Usage: OpenAIBatchApp <source-directory> <target-directory> <mode>");
                Console.WriteLine("       source-directory and target-directory can be with full path or relative");
                Console.WriteLine("       mode: 0 - upload new files and create batches for them");
                Console.WriteLine("             1 - download batch results");
                Console.WriteLine("             2 - both above");
                Console.WriteLine();
                Console.WriteLine("Example: OpenAIBatchApp.exe in out 2");
                return;
            }
            string sourceDirectory = args[0];
            string targetDirectory = args[1];

            if (Path.GetDirectoryName(sourceDirectory) == string.Empty)
            {
                sourceDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, sourceDirectory);
            }
            if (Path.GetDirectoryName(targetDirectory) == string.Empty)
            {
                targetDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, targetDirectory);
            }

            if (!Directory.Exists(sourceDirectory))
            {
                throw new ArgumentException($"Source directory {sourceDirectory} doesn't exist");
            }
            if (!Directory.Exists(targetDirectory))
            {
                throw new ArgumentException($"Target directory {targetDirectory} doesn't exist");
            }

            var mode = Enum.Parse<Mode>(args[2]);
            Console.WriteLine($"Source directory: {sourceDirectory}\nTarget directory: {targetDirectory}\nMode: {mode}\n");

            var openAI = new OpenAI();
            try
            {
                switch (mode)
                {
                    case Mode.UploadFiles: await UploadFiles(openAI, sourceDirectory); break;
                    case Mode.DowloadFiles: await DowloadFiles(openAI, targetDirectory); break;
                    case Mode.UploadAndDownload: await UploadFiles(openAI, sourceDirectory); await DowloadFiles(openAI, targetDirectory); break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex}");
                Console.ReadLine();
            }

            Console.WriteLine("Press any key to exit");
            Console.ReadLine();
        }


        static async Task UploadFiles(OpenAI openAI, string sourceDirectory)
        {
            // get all files from OpenAI
            var existingAIFiles = await openAI.GetFiles();

            var files = Directory.GetFiles(sourceDirectory);
            var filesForBatch = new List<FileEntry>();

            foreach (var filePath in files)
            {
                Console.WriteLine($"\nProcessing file {filePath} ...");

                bool shouldUpload = true;
                var fileInfo = new FileInfo(filePath);
                // try to find existing file with the same filename and purpose as "batch"
                var openAIFile = existingAIFiles.Find(f => f.FileName == fileInfo.Name && f.Purpose == "batch");
                if (openAIFile != null)
                {
                    // creation date (UTC) of OpenAI file 
                    var openAiFileUtcDate = DateTimeOffset.FromUnixTimeSeconds(openAIFile.CreatedAt).UtcDateTime;
                    // should upload, if local file is newer
                    shouldUpload = fileInfo.LastWriteTime.ToUniversalTime() > openAiFileUtcDate;
                }
                if (shouldUpload)
                {
                    Console.WriteLine($"Uploading file {fileInfo.Name} to OpenAI ...");
                    openAIFile = await openAI.UploadFile(fileInfo);

                    Console.WriteLine($"File {fileInfo.Name} was uploaded, ID: {openAIFile?.Id}");

                    // create batch
                    var batch = await openAI.CreateBatch(openAIFile);
                    if (batch != null)
                        Console.WriteLine($"Batch was created, ID: {batch?.Id}");
                }
                else
                {
                    Console.WriteLine($"File {fileInfo.Name} was skipped for upload");
                }
            }
        }

        static async Task DowloadFiles(OpenAI openAI, string targetDirectory)
        {
            // get all batches from OpenAI
            var existingAIBatched = await openAI.GetBatches();

            foreach (var batch in existingAIBatched)
            {
                Console.WriteLine($"\nProcessing batch {batch.Id} ...");
                if (batch.Status == BatchStatus.Completed)
                {
                    // output file info 
                    var openAIFile = await openAI.GetFile(batch.OutputFileId);

                    bool shouldDownload = true;

                    // all local batch results files 
                    var localOutputFiles = Directory.GetFiles(targetDirectory).Select(f => new FileInfo(f));

                    var localFile = localOutputFiles.FirstOrDefault(f => f.Name == openAIFile.FileName);
                    if (localFile != null)
                    {
                        // creation date (UTC) of OpenAI file 
                        var openAiFileUtcDate = DateTimeOffset.FromUnixTimeSeconds(openAIFile.CreatedAt).UtcDateTime;
                        // should download, if local file is older
                        shouldDownload = localFile.LastWriteTime.ToUniversalTime() < openAiFileUtcDate;
                    }

                    if (shouldDownload)
                    {
                        var content = await openAI.GetFileContent(batch.OutputFileId);
                        var targetFile = Path.Combine(targetDirectory, openAIFile.FileName);
                        File.WriteAllText(targetFile, content);
                        Console.WriteLine($"Result file {targetFile} was created for batch: {batch?.Id}");
                    }
                    else
                    {
                        Console.WriteLine($"File {openAIFile.FileName} was skipped for download");
                    }
                }
                else
                {
                    Console.WriteLine($"Batch {batch.Id} cannot be dowloaded, status -  {batch.Status} ");
                }
            }
        }
    }
}
