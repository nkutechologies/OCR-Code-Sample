namespace Octopus.Dtos.Common
{
    public class ResponseModel<T>
    {
        public string Response { get; set; }

        public System.Net.HttpStatusCode StatusCode { get; set; }

        public string Message { get; set; }

        public string RequestId { get; set; }

        public T Data { get; set; }
        public string Request { get; set; }
    }
}
