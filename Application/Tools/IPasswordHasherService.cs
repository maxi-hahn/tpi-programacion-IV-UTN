using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Tools
{
    public interface IPasswordHasherService
    {
        string Hash(string password);
        bool Verify(string hash, string password);
    }
}
