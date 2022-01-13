using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace WeyneLoggingWithNLog.Interfaces
{
    public interface IProcessService
    {
        Task<string> Invoke(string input);
    }
}
