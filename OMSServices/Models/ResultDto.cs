namespace OMSServices.Models
{
    public class ResultDto
    {
        public bool Status { get; set; }
        public string Message { get; set; }

        public ResultDto(bool status, string message)
        {
            Status = status;
            Message = message;
        }
    }
}
