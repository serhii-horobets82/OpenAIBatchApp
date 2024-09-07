using Newtonsoft.Json;

namespace OpenAIBatchApp
{
    public class BaseResponse<T>
    {
        [JsonProperty("data")]
        public virtual T Data { get; set; }
    }

    public class BaseAIObject
    {
        public required string Id { get; set; }

        [JsonProperty("object")]
        public ObjectType Type { get; set; }

        [JsonProperty("created_at")]
        public long CreatedAt { get; set; }
    }

    public class FileEntry : BaseAIObject
    {
        public int Bytes { get; set; }

        public string? FileName { get; set; }

        public string? Purpose { get; set; }
    }

    public class BatchEntry : BaseAIObject
    {
        public string Endpoint { get; set; }

        public string Errors { get; set; }

        [JsonProperty("completion_window")]
        public string CompletionWindow { get; set; }

        public string Status { get; set; }

        [JsonProperty("output_file_id")]
        public string OutputFileId { get; set; }

        [JsonProperty("input_file_id")]
        public string InputFileId { get; set; }

        [JsonProperty("error_file_id")]
        public string ErrorFileId { get; set; }

        [JsonProperty("in_progress_at")]
        public long? InprogressAt { get; set; }

        [JsonProperty("expires_at")]
        public long? ExpiresAt { get; set; }

        [JsonProperty("finalizing_at")]
        public long? FinalizingAt { get; set; }

        [JsonProperty("completed_at")]
        public long? CompletedAt { get; set; }

        [JsonProperty("failed_at")]
        public long? FailedAt { get; set; }

        [JsonProperty("expired_at")]
        public long? ExpiredAt { get; set; }

        [JsonProperty("cancelling_at")]
        public long? CancellingAt { get; set; }
        [JsonProperty("cancelled_at")]
        public long? CancelledAt { get; set; }
    }


    public static class BatchStatus
    {
        // the input file is being validated before the batch can begin
        public static readonly string Validating = "validating";
        //the input file has failed the validation process
        public static readonly string Failed = "failed";
        // the input file was successfully validated and the batch is currently being run
        public static readonly string InProgress = "in_progress";
        // finalizing the batch has completed and the results are being prepared
        public static readonly string Finalizing = "finalizing";
        // completed the batch has been completed and the results are ready
        public static readonly string Completed = "completed";
        //expired the batch was not able to be completed within the 24-hour time window
        public static readonly string Expired = "expired";
        // cancelling  the batch is being cancelled(may take up to 10 minutes)
        public static readonly string Cancelling = "cancelling";
        // cancelled the batch was cancelled
        public static readonly string Cancelled = "cancelled";
    }

    
}
