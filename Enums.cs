using System.ComponentModel;

namespace OpenAIBatchApp
{
    public enum ObjectType
    {
        [Description("file")]
        File = 1,

        [Description("batch")]
        Batch = 2,
    }

    public enum Mode
    {
        UploadFiles,
        DowloadFiles,
        UploadAndDownload
    }
}
