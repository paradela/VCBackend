using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VCBackend.Models.Dto
{
    public class TokenDto
    {
        public String AuthToken { get; set; }

        public TokenDto(String token) { AuthToken = token; }
    }
}