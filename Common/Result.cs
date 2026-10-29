namespace AsyncBatchProcessor.Common
{
    public class Result<T>
    {
        public bool IsSuccess { get;  }
        public bool IsFailure=>!IsSuccess;
        public string? Error { get;  }
        public int? ErrorStatusCode { get; }
        public T? Value { get;  }

        private Result(bool IsSuccess, string? error, T? value, int? errorStatusCode)
        {
            this.IsSuccess = IsSuccess;
            this.Error = error;
            this.Value = value;
            ErrorStatusCode = errorStatusCode;
        }
        public static Result<T> Success(T value)
        {
            return new Result<T> (true,null,value,null);
        }
        public static Result<T>Failure(string error,int statusCode)
        {
            return new Result<T>(false, error, default, statusCode);
        }

    }
}
