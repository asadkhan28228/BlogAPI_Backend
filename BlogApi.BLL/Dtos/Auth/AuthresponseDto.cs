using System;
using System.Collections.Generic;
using System.Text;

namespace BlogApi.BLL.Dtos.Auth
{
    public class AuthResponseDto
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public string Token { get; set; }
    }
}
