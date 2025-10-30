using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Wrappers
{
    public interface IResponseWrapper
    {
        List<string> Messages { get; set; }
        public bool IsSuccessful { get; set; }
    }

    public interface IResponseWrapper<out T> : IResponseWrapper
    {
        T Data { get; }
    }
}
