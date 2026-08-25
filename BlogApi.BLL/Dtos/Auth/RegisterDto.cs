using System;
using System.Collections.Generic;
using System.Text;

namespace BlogApi.BLL.Dtos.Auth
{
    namespace BlogAPI.BLL.DTOs
    {
        public class RegisterDto
        {
            public string Username { get; set; }
            public string Email { get; set; }
            public string Password { get; set; }
        }
    }
}
