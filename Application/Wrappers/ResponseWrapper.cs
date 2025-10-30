using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Wrappers
{
    public class ResponseWrapper : IResponseWrapper
    {
        public List<string> Messages { get; set; } = [];
        public bool IsSuccessful { get; set; }

        public ResponseWrapper() { }

        public static IResponseWrapper Fail()
        {
            return new ResponseWrapper()
            {
                IsSuccessful = false
            };
        }

        public static IResponseWrapper Fail(string message)
        {
            return new ResponseWrapper()
            {
                IsSuccessful = false,
                Messages = [message]
            };
        }

        public static IResponseWrapper Fail(List<string> message)
        {
            return new ResponseWrapper()
            {
                IsSuccessful = false,
                Messages = message
            };
        }

        public static Task<IResponseWrapper> FailAsync()
        {
            return Task.FromResult(Fail());
        }

        public static Task<IResponseWrapper> FailAsync(string message)
        {
            return Task.FromResult(Fail(message));
        }

        public static Task<IResponseWrapper> FailAsync(List<string> messages)
        {
            return Task.FromResult(Fail(messages));
        }

        public static IResponseWrapper Success ()
        {
            return new ResponseWrapper()
            {
                IsSuccessful = true
            };
        }

        public static IResponseWrapper Success(string message)
        {
            return new ResponseWrapper()
            {
                IsSuccessful = true,
                Messages = [message]
            };
        }

        public static IResponseWrapper Success(List<string> message)
        {
            return new ResponseWrapper()
            {
                IsSuccessful = true,
                Messages = message
            };
        }

        public static Task<IResponseWrapper> SuccessAsync()
        {
            return Task.FromResult(Success());
        }

        public static Task<IResponseWrapper> SuccessAsync(string message)
        {
            return Task.FromResult(Success(message));
        }

        public static Task<IResponseWrapper> SuccessAsync(List<string> messages)
        {
            return Task.FromResult(Success(messages));
        }
    }

    public class ResponseWrapper<T> : ResponseWrapper, IResponseWrapper<T>
    {
        public T Data { get; set; }
        public ResponseWrapper() { }

        public new static ResponseWrapper<T> Fail()
        {
            return new ResponseWrapper<T> { IsSuccessful = false };
        }

        public new static ResponseWrapper<T> Fail(string message)
        {
            return new ResponseWrapper<T> { IsSuccessful = false, Messages = [message] };
        }

        public new static ResponseWrapper<T> Fail(List<string> messages)
        {
            return new ResponseWrapper<T> { IsSuccessful = false, Messages = messages };
        }

        public new static Task<ResponseWrapper<T>> FailAsync()
        {
            return Task.FromResult(Fail());
        }

        public new static Task<ResponseWrapper<T>> FailAsync(string message)
        {
            return Task.FromResult(Fail(message));
        }

        public new static Task<ResponseWrapper<T>> FailAsync(List<string> messages)
        {
            return Task.FromResult(Fail(messages));
        }

        public new static ResponseWrapper<T> Success()
        {
            return new ResponseWrapper<T> { IsSuccessful = true };
        }

        public new static ResponseWrapper<T> Success(string message)
        {
            return new ResponseWrapper<T> { IsSuccessful = true, Messages = [message] };
        }

        public new static ResponseWrapper<T> Success(List<string> messages)
        {
            return new ResponseWrapper<T> { IsSuccessful = true, Messages = messages };
        }

        public new static Task<ResponseWrapper<T>> SuccessAsync()
        {
            return Task.FromResult(Success());
        }

        public new static Task<ResponseWrapper<T>> SuccessAsync(string message)
        {
            return Task.FromResult(Success(message));
        }

        public new static Task<ResponseWrapper<T>> SuccessAsync(List<string> messages)
        {
            return Task.FromResult(Success(messages));
        }
    }
}
