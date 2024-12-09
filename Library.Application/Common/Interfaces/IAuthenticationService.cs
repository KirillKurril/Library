using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Application.Common.Interfaces
{
    public interface IAuthenticationService
    {
        Task<string> GetAccessTokenAsync();
        Task<bool> ValidateTokenAsync(string token);
    }
}
