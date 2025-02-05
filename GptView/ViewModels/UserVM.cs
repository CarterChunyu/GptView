using GptView.Models;

namespace GptView.ViewModels
{
    public enum SubmitType:int
    {
        login, regiseter
    }
    public class UserVM
    {
        public int userid { get; set; }

        public string? username { get; set; }

        public string? password { get; set; }

        public string? nickname { get; set; }

        public string? email { get; set; }

        public string? submittype { get; set; }
    }

    public class UserInfoVM
    {   
        public int userid { get; set; }

        public string? username { get; set; }

        public List<Function> functionlist { get; set; } = new List<Function>();
    }
}
