# OpenAIBatchApp

### How to run 
1. Put valid OpenAI API key into file `OpenAI.cs`, variable `openAiApiKey`
```
static readonly string openAiApiKey = "your_openai_api_key";
```
2. Compile the project (.NET 8 is required)
3. Run executable `OpenAIBatchApp` in the command line with 3 parameters
  ```
OpenAIBatchApp.exe <source-directory> <target-directory> <mode>
```
- `source-directory` - directory with batches files (`jsonl`), can be full path or relative
- `target-directory` - directory with batches results, can be full path or relative
- `mode` - possible values: `0` - upload new files and create batches for them,  `1` - download batch results, `2` - both above
Sample call 
```
OpenAIBatchApp.exe in out 2
```
