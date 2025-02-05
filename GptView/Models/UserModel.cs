using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace GptView.Models
{
    public class User
    {
        public int userid { get; set; }

        public string? username { get; set; }

        public string? password { get; set; }

        public string? nickname { get; set; }

        public string? email { get; set; }
    }

    public class Function
    {
        public int functionid { get; set; }

        public int parentid { get; set; }
        public string? parentname { get; set; }

        public string? childname { get; set; }

        public string? functionpath { get; set; }

        public bool isapi { get; set; }
    }

    [PrimaryKey(nameof(userid), nameof(functionid))]
    public class AccessPermission
    {
        public int userid { get; set; }

        public int functionid { get; set; }
       
    }
}

