using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs.Authentication;

namespace Application.Interfaces.Services
{
    public interface IAuthenticationService
    {
        Task<AuthenticatedUserDto?> LoginAsync(LoginRequest request);
    }
}
