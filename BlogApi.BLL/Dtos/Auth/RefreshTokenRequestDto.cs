using System;
using System.Collections.Generic;
using System.Text;


    namespace BlogApi.BLL.Dtos.Auth
    {
        public class RefreshTokenRequestDto
        {
            public string RefreshToken { get; set; } = string.Empty;
        }
    }
