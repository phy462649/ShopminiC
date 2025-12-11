using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LandingPageApp.Application.Dtos
{
    public class ApiResponse
    {
        public bool Status{ set; get; }
        public string Message { set; get; }
        public object Data { set; get; }

    }
}
