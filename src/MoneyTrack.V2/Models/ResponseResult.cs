using System.Linq;
using System.Web.Mvc;

namespace CloudyWing.MoneyTrack.Models {
    /// <summary>
    /// Represents a generic response result without data.
    /// </summary>
    public class ResponseResult {
        protected const string ExecutionFailureMessage = "執行失敗，請重新操作。";

        /// <summary>
        /// Gets a value indicating whether the operation was successful.
        /// </summary>
        public bool IsOk { get; protected set; }

        /// <summary>
        /// Gets a value indicating whether the operation encountered an error.
        /// </summary>
        public bool IsError => !IsOk;

        /// <summary>
        /// Gets or sets the message associated with the response result.
        /// </summary>
        public string Message { get; protected set; } = "";

        /// <summary>
        /// Creates a successful response result without data.
        /// </summary>
        /// <param name="message">The optional message associated with the success.</param>
        /// <returns>A successful <see cref="ResponseResult"/>.</returns>
        public static ResponseResult Succeed(string message = "") {
            return new ResponseResult {
                IsOk = true,
                Message = message
            };
        }

        /// <summary>
        /// Creates a failed response result without data.
        /// </summary>
        /// <param name="message">The optional message associated with the failure.</param>
        /// <returns>A failed <see cref="ResponseResult"/>.</returns>
        public static ResponseResult Fail(string message = "") {
            return new ResponseResult {
                IsOk = false,
                Message = message
            };
        }

        /// <summary>
        /// Creates a failed response result based on the specified <paramref name="modelState"/>.
        /// </summary>
        /// <param name="modelState">The <see cref="ModelStateDictionary"/> containing validation errors.</param>
        /// <returns>A failed <see cref="ResponseResult"/>.</returns>
        public static ResponseResult Fail(ModelStateDictionary modelState) {
            return new ResponseResult {
                IsOk = false,
                Message = modelState.First(x => x.Value.Errors.Count > 0)
                                    .Value.Errors.First().ErrorMessage
            };
        }

        /// <summary>
        /// Creates a failed response result with an execution error message.
        /// </summary>
        /// <returns>A failed <see cref="ResponseResult"/>.</returns>
        public static ResponseResult FailExecution() {
            return new ResponseResult {
                IsOk = false,
                Message = ExecutionFailureMessage
            };
        }
    }

    /// <summary>
    /// Represents a generic response result with data of type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type of the data.</typeparam>
    public class ResponseResult<T> : ResponseResult {
        /// <summary>
        /// Gets or sets the data associated with the response result.
        /// </summary>
        public T Data { get; protected set; }

        /// <summary>
        /// Creates a successful response result with data.
        /// </summary>
        /// <param name="data">The data associated with the success.</param>
        /// <param name="message">The optional message associated with the success.</param>
        /// <returns>A successful <see cref="ResponseResult{T}"/>.</returns>
        public static ResponseResult<T> Succeed(T data, string message = "") {
            return new ResponseResult<T> {
                IsOk = true,
                Message = message,
                Data = data
            };
        }

        /// <summary>
        /// Creates a failed response result with data.
        /// </summary>
        /// <param name="data">The data associated with the failure.</param>
        /// <param name="message">The optional message associated with the failure.</param>
        /// <returns>A failed <see cref="ResponseResult{T}"/>.</returns>
        public static ResponseResult<T> Fail(T data, string message = "") {
            return new ResponseResult<T> {
                IsOk = false,
                Message = message,
                Data = data
            };
        }

        /// <summary>
        /// Creates a failed response result with data based on the specified <paramref name="modelState"/>.
        /// </summary>
        /// <param name="data">The data associated with the failure.</param>
        /// <param name="modelState">The <see cref="ModelStateDictionary"/> containing validation errors.</param>
        /// <returns>A failed <see cref="ResponseResult{T}"/>.</returns>
        public static ResponseResult<T> Fail(T data, ModelStateDictionary modelState) {
            return new ResponseResult<T> {
                IsOk = false,
                Message = modelState.First(x => x.Value.Errors.Count > 0)
                                    .Value.Errors.First().ErrorMessage,
                Data = data
            };
        }

        /// <summary>
        /// Creates a failed response result with an execution error message.
        /// </summary>
        /// <param name="data">The data associated with the failure.</param>
        /// <returns>A failed <see cref="ResponseResult"/>.</returns>
        public static ResponseResult<T> FailExecution(T data) {
            return new ResponseResult<T> {
                IsOk = false,
                Message = ExecutionFailureMessage,
                Data = data
            };
        }
    }
}
