using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace CloudyWing.MoneyTrack.Models.Application {
    /// <summary>
    /// Represents the result of an application operation.
    /// </summary>
    public class ApplicationResult {
        protected const string ExecutionFailureMessage = "執行失敗，請重新操作。";

        /// <summary>
        /// Gets or sets a value indicating whether the operation is successful.
        /// </summary>
        public bool IsOk { get; set; }

        /// <summary>
        /// Gets a value indicating whether the operation encountered an error.
        /// </summary>
        public bool IsError => !IsOk;

        /// <summary>
        /// Gets or sets a message associated with the result.
        /// </summary>
        public string? Message { get; set; }

        /// <summary>
        /// Creates a successful application result without data.
        /// </summary>
        /// <param name="message">The optional message associated with the success.</param>
        /// <returns>A successful <see cref="ResponseResult"/>.</returns>
        public static ApplicationResult Succeed(string message = "") {
            return new ApplicationResult {
                IsOk = true,
                Message = message
            };
        }

        /// <summary>
        /// Creates a failed application result without data.
        /// </summary>
        /// <param name="message">The optional message associated with the failure.</param>
        /// <returns>A failed <see cref="ResponseResult"/>.</returns>
        public static ApplicationResult Fail(string message = "") {
            return new ApplicationResult {
                IsOk = false,
                Message = message
            };
        }

        /// <summary>
        /// Creates a failed application result based on the specified <paramref name="modelState"/>.
        /// </summary>
        /// <param name="modelState">The <see cref="ModelStateDictionary"/> containing validation errors.</param>
        /// <returns>A failed <see cref="ResponseResult"/>.</returns>
        public static ApplicationResult Fail(ModelStateDictionary modelState) {
            return new ApplicationResult {
                IsOk = false,
                Message = modelState.First(x => x.Value!.Errors.Count > 0)
                                    .Value!.Errors.First().ErrorMessage
            };
        }

        /// <summary>
        /// Creates a failed application result with an execution error message.
        /// </summary>
        /// <returns>A failed <see cref="ResponseResult"/>.</returns>
        public static ApplicationResult FailExecution() {
            return new ApplicationResult {
                IsOk = false,
                Message = ExecutionFailureMessage
            };
        }
    }

    /// <summary>
    /// Represents the result of an application operation with additional data.
    /// </summary>
    /// <typeparam name="T">The type of the additional data.</typeparam>
    public class ApplicationResult<T> : ApplicationResult {
        /// <summary>
        /// Gets or sets the additional data.
        /// </summary>
        public T? Data { get; set; }

        /// <summary>
        /// Creates a successful application result with data.
        /// </summary>
        /// <param name="data">The data associated with the success.</param>
        /// <param name="message">The optional message associated with the success.</param>
        /// <returns>A successful <see cref="ResponseResult{T}"/>.</returns>
        public static ApplicationResult<T> Succeed(T data, string message = "") {
            return new ApplicationResult<T> {
                IsOk = true,
                Message = message,
                Data = data
            };
        }

        /// <summary>
        /// Creates a failed application result with data.
        /// </summary>
        /// <param name="data">The data associated with the failure.</param>
        /// <param name="message">The optional message associated with the failure.</param>
        /// <returns>A failed <see cref="ResponseResult{T}"/>.</returns>
        public static ApplicationResult<T> Fail(T data, string message = "") {
            return new ApplicationResult<T> {
                IsOk = false,
                Message = message,
                Data = data
            };
        }

        /// <summary>
        /// Creates a failed application result with data based on the specified <paramref name="modelState"/>.
        /// </summary>
        /// <param name="data">The data associated with the failure.</param>
        /// <param name="modelState">The <see cref="ModelStateDictionary"/> containing validation errors.</param>
        /// <returns>A failed <see cref="ResponseResult{T}"/>.</returns>
        public static ApplicationResult<T> Fail(T data, ModelStateDictionary modelState) {
            return new ApplicationResult<T> {
                IsOk = false,
                Message = modelState.First(x => x.Value!.Errors.Count > 0)
                                    .Value!.Errors.First().ErrorMessage,
                Data = data
            };
        }

        /// <summary>
        /// Creates a failed application result with an execution error message.
        /// </summary>
        /// <param name="data">The data associated with the failure.</param>
        /// <returns>A failed <see cref="ResponseResult"/>.</returns>
        public static ApplicationResult<T> FailExecution(T data) {
            return new ApplicationResult<T> {
                IsOk = false,
                Message = ExecutionFailureMessage,
                Data = data
            };
        }
    }
}
