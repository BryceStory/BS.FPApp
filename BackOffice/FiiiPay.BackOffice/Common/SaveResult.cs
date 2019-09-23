using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FiiiPay.BackOffice.Common
{
    public class SaveResult
    {
        public SaveResult()
        {
            Result = true;
        }
        public SaveResult(SqlSugar.DbResult<bool> r)
        {
            Result = r.IsSuccess;
            Message = r.IsSuccess ? "Save Success" : r.ErrorMessage;
        }
        public SaveResult(bool result)
        {
            Result = result;
        }
        public SaveResult(bool result, string msg)
        {
            Result = result;
            Message = msg;
        }
        public bool Result { get; set; }
        public string Message { get; set; }

        public object toJson()
        {
            return new { Status = this.Result ? 1 : 0, Message = this.Message };
        }
    }
    public class SaveResult<T>
    {
        public SaveResult()
        {
            Result = true;
        }
        public SaveResult(SqlSugar.DbResult<bool> r)
        {
            Result = r.IsSuccess;
            Message = r.IsSuccess ? "Save Success" : r.ErrorMessage;
        }
        public SaveResult(bool result)
        {
            Result = result;
        }
        public SaveResult(bool result, string msg)
        {
            Result = result;
            Message = msg;
        }
        public SaveResult(bool result, T t)
        {
            Result = result;
            Data = t;
        }
        public SaveResult(bool result, string msg, T t)
        {
            Result = result;
            Message = msg;
            Data = t;
        }
        public bool Result { get; set; }
        public string Message { get; set; }

        public T Data { get; set; }

        public object toJson()
        {
            return new { Status = this.Result ? 1 : 0, Message = this.Message };
        }

        public object toJsonWithData()
        {
            return new { Status = this.Result ? 1 : 0, Message = this.Message, Data = this.Data };
        }
    }
}