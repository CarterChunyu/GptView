using System.Diagnostics.Contracts;

namespace GptView.ViewModels
{
    public class Server
    {
        public string Status { get; set; }
    }

    public class GoogleAuth
    {
        public string client_id { get; set; }

        public string client_secret { get; set; }

        public string grant_type { get; set; }

        public string access_type { get; set; }

        public Redirect_Uri redirect_uri { get; set; }

        public string oauth2url { get; set; }

        public string userinfourl { get; set; }
    }

    public class Redirect_Uri
    {
        public string remote { get; set; }

        public string local { get; set; }
    }

    public class GoogleTokenResponse
    {
        public string access_token { get; set; }
        public int expires_in { get; set; }
        public string refresh_token { get; set; }
        public string scope { get; set; }
        public string token_type { get; set; }
        public string id_token { get; set; }
        public string error { get; set; }
        public string error_description { get; set; }
        public bool IsSuccess => string.IsNullOrEmpty(error);
    }

    public class GoogleUserInfo
    {
        public string? id { get; set; }

        public string? email { get; set; }

        public bool verified_email { get; set; }

        public string? picture { get; set; }

        public string? hd { get; set; }
    }
}
