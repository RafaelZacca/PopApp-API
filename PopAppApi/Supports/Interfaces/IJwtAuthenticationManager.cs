using System;

namespace API.Supports.Interfaces
{
    public interface IJwtAuthenticationManager
    {
        string Authenticate(int id, string name);
    }
}
