namespace GptView.ViewModels
{
    public class PointXY
    {
        public float x { get; set; }

        public float y { get; set; }
    }

    public class GraphResponse
    {
        public GraphResponse(int statusCode, string message)
        {
            StatusCode = statusCode;
            Message = message;
        }

        public int StatusCode { get; set; }

        public string Message { get; set; }
    }
}
