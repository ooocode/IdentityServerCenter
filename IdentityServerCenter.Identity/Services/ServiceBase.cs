using FluentValidation.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IdentityServerCenter.Identity.Services
{
    public abstract class ServiceBase
    {
        public virtual Result OkResult() => new Result { Succeeded = true };

        public virtual Result FailedResult(string errorMessage) => new Result { Succeeded = false, ErrorMessage = errorMessage };

        public virtual Result FailedResult(Exception ex) => new Result { Succeeded = false, ErrorMessage = ex.InnerException?.Message ?? ex.Message };

        public virtual Result FailedResult(ValidationResult v) =>
            new Result { Succeeded = false, ErrorMessage = string.Join(';', v.Errors.Select(e=>e.ErrorMessage) )};





        public virtual DataResult<T> OkDataResult<T>(T data) => new DataResult<T> { Succeeded = true, Data = data };

        public virtual DataResult<T> FailedDataResult<T>(string errorMessage) => new DataResult<T> { Succeeded = false, ErrorMessage = errorMessage };

        public virtual DataResult<T> FailedDataResult<T>(Exception ex) =>
            new DataResult<T> { Succeeded = false, ErrorMessage = ex.InnerException?.Message ?? ex.Message };

        public virtual DataResult<T> FailedDataResult<T>(ValidationResult v) =>
           new DataResult<T> { Succeeded = false, ErrorMessage = string.Join(';', v.Errors.Select(e => e.ErrorMessage)) };




        public virtual PaginationResult<T> OkPaginationResult<T>(IList<T> rows, int total, int pageSize) => new PaginationResult<T>
        {
            Succeeded = true,
            Rows = rows,
            Total = total,
            PageSize = pageSize
        };

        public virtual PaginationResult<T> FailedPaginationResult<T>(string errorMessage) => new PaginationResult<T>
        {
            Succeeded = false,
            ErrorMessage = errorMessage
        };

        public virtual PaginationResult<T> FailedPaginationResult<T>(Exception ex) => new PaginationResult<T>
        {
            Succeeded = false,
            ErrorMessage = ex.InnerException?.Message ?? ex.Message
        };


        public virtual PaginationResult<T> FailedPaginationResult<T>(ValidationResult v) => new PaginationResult<T>
        {
            Succeeded = false,
            ErrorMessage = string.Join(';', v.Errors.Select(e => e.ErrorMessage))
        };
    }
}
