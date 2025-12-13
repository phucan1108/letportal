using System;
using System.Collections.Generic;
using System.Text;

namespace LetPortal.Identity.Models
{
    public class LogoutModel
    {
        public string Username { get; set; }

        public string Token { get; set; }

        public string UserSession { get; set; }
    }
}
