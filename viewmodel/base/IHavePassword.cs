using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace Abituria
{
    public interface IHavePassword///Interfejs dla klasu, która zapewnia chronione hasło
    {
        SecureString SecurePassword { get; }///Chronione hasło
    }
}