using Shared.Kernel.Exceptions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Kernel.BaseReturnTypes
{
    public class ReturnBase<TResult> : ReturnBase
    {
        public ReturnBase(TResult? result, bool succeeded = true) : base(succeeded)
        {
            Result = result;
        }
        public TResult? Result { get; }
        public static ReturnBase<TResult> Success(TResult? result)
        {
            return new ReturnBase<TResult>(result, true);
        }
        public new static ReturnBase<TResult> Fail()
        {
            var result = new ReturnBase<TResult>(default, false);
            return result;
        }
        public new static ReturnBase<TResult> Fail(Exception exception, IExceptionManager exceptionManager)
        {
            var result = new ReturnBase<TResult>(default, false);
            result.AddError(exceptionManager.GetErrorFromException(exception));
            return result;
        }
        public new static ReturnBase<TResult> Fail(IEnumerable<ReturnBaseError> errors)
        {
            var result = new ReturnBase<TResult>(default, false);
            result.AddErrors(errors);
            return result;
        }
    }
    public class ReturnBase
    {
        private readonly List<ReturnBaseError> _errors;

        public ReturnBase(bool succeeded = true)
        {
            _errors = new List<ReturnBaseError>();
            Succeeded = succeeded;
        }

        public bool Succeeded { get; } = true;
        public ReadOnlyCollection<ReturnBaseError> Errors { get { return _errors.AsReadOnly(); } }

        public void AddError(ReturnBaseError error)
        {
            _errors.Add(error);
        }
        public void AddErrors(IEnumerable<ReturnBaseError> errors)
        {
            _errors.AddRange(errors);
        }


        public static ReturnBase Success()
        {
            return new ReturnBase(true);
        }
        public static ReturnBase Fail()
        {
            var result = new ReturnBase(false);
            return result;
        }
        public static ReturnBase Fail(Exception exception, IExceptionManager exceptionManager)
        {
            var result = new ReturnBase(false);
            result.AddError(exceptionManager.GetErrorFromException(exception));
            return result;
        }
        public static ReturnBase Fail(IEnumerable<ReturnBaseError> errors)
        {
            var result = new ReturnBase(false);
            result.AddErrors(errors);
            return result;
        }
    }

    public class ReturnBaseError
    {
        public string? Source { get; init; }
        public string? ErrorCode { get; init; }
        public string? ErrorMessage { get; init; }
        public string? SystemErrorMessage { get; init; }
        public Exception? Exception { get; init; }
    }

}