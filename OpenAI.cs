using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace OpenAIBatchApp
{
    internal class OpenAI
    {
        static readonly string openAiApiKey = "your_openai_api_key";
        static readonly string apiUrl = "https://api.openai.com/v1";
        private static readonly HttpClient client = new();

        public OpenAI()
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", openAiApiKey);
        }


        public async Task<List<FileEntry>> GetFiles()
        {
            var response = await client.GetAsync($"{apiUrl}/files");

            response.EnsureSuccessStatusCode();
            var responseContent = await response.Content.ReadAsStringAsync();
            var responseData = JsonConvert.DeserializeObject<BaseResponse<List<FileEntry>>>(responseContent);
            // TODO: Add pagination fetch, using has_more

            return responseData?.Data ?? [];
        }

        public async Task<FileEntry?> UploadFile(FileInfo fileInfo)
        {
            var content = new MultipartFormDataContent();
            var fileContent = new ByteArrayContent(File.ReadAllBytes(fileInfo.FullName));
            fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("application/octet-stream");
            content.Add(fileContent, "file", fileInfo.Name);
            content.Add(new StringContent("batch"), "purpose");

            var response = await client.PostAsync($"{apiUrl}/files", content);
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<FileEntry>(responseContent);
        }

        public async Task<FileEntry?> GetFile(string fileId)
        {
            var response = await client.GetAsync($"{apiUrl}/files/{fileId}");

            response.EnsureSuccessStatusCode();
            var responseContent = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<FileEntry>(responseContent);
        }


        public async Task<string> GetFileContent(string fileId)
        {
            var response = await client.GetAsync($"{apiUrl}/files/{fileId}/content");

            response.EnsureSuccessStatusCode();
            var responseContent = await response.Content.ReadAsStringAsync();

            return responseContent;
        }

        public async Task<BatchEntry?> CreateBatch(FileEntry file)
        {
            var batchData = new
            {
                input_file_id = file.Id,
                endpoint = "/v1/chat/completions",
                completion_window = "24h",
            };

            var response = await client.PostAsJsonAsync($"{apiUrl}/batches", batchData);
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            var batch = JsonConvert.DeserializeObject<BatchEntry>(responseContent);

            return batch;
        }

        public async Task<List<BatchEntry>> GetBatches()
        {
            var response = await client.GetAsync($"{apiUrl}/batches");

            response.EnsureSuccessStatusCode();
            var responseContent = await response.Content.ReadAsStringAsync();
            var responseData = JsonConvert.DeserializeObject<BaseResponse<List<BatchEntry>>>(responseContent);
            // TODO: Add pagination fetch, using has_more

            return responseData?.Data ?? [];
        }


    }




}
