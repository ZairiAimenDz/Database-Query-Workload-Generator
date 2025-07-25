using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DatabaseWorkloadQueryGenerator.Application.Wrappers
{
    /// <summary>
    /// A response that will be either successful or failed, if failed the succeeded property will be set to false and the errors will contain
    /// a list of errors describing why the request failed.
    /// Otherwise the succeeded property will be set to true. And the data property will contain the result of the request.
    /// </summary>
    /// <typeparam name="T">The type of data to return</typeparam>
    public class Response<T> : Response
    {
        /// <summary>
        /// The data result from the request
        /// </summary>
        public T? Data { get; set; }

        /// <inheritdoc cref="Response.Succeeded"/>
        [MemberNotNullWhen(true, nameof(Data))]
        public new bool Succeeded { get; set; }

        /// <summary>
        /// Creates a new successful response
        /// </summary>
        /// <param name="data">The data within the response</param>
        /// <returns></returns>
        public static Response<T> Success(T data) => new() { Data = data, Errors = [], Succeeded = true };

        /// <summary>
        /// Creates a new successful response
        /// </summary>
        /// <param name="data">The data within the response</param>
        /// <param name="message">A Message To be Sent With The Response</param>
        /// <returns></returns>
        public static Response<T> Success(T data, string message) => new() { Data = data, Errors = [], Succeeded = true, Message = message };

        /// <summary>
        /// Creates a new failed response
        /// </summary>
        /// <param name="error">The error message to return</param>
        /// <returns></returns>
        public static new Response<T> Fail(string error) => new() { Errors = [error], Succeeded = false };

        /// <summary>
        /// Creates a new failed response
        /// </summary>
        /// <param name="errors">The error messages to return</param>
        /// <returns></returns>
        public static new Response<T> Fail(List<string> errors) => new() { Errors = errors, Succeeded = false };

        /// <summary>
        /// Implicitly convert the response data to a success response including the data
        /// </summary>
        /// <param name="data">The response data</param>
        public static implicit operator Response<T>(T data) => Success(data);
    }


    /// <summary>
    /// A response that will be either successful or failed, if failed the succeeded property will be set to false and the errors will contain
    /// a list of errors describing why the request failed.
    /// Otherwise the succeeded property will be set to true.
    /// </summary>
    public class Response
    {
        /// <summary>
        /// Whether the request was successful or not
        /// </summary>
        public bool Succeeded { get; set; }

        /// <summary>
        /// Return true if the operation has failed
        /// </summary>
        [JsonIgnore]
        public bool IsFailed => !Succeeded;

        /// <summary>
        /// Success message
        /// </summary>
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// The list of errors incase the request fails
        /// </summary>
        public List<string> Errors { get; set; } = [];

        /// <summary>
        /// Creates a new successful response
        /// </summary>
        /// <returns></returns>
        public static Response Success() => new() { Succeeded = true };

        /// <summary>
        /// Creates a new successful response
        /// </summary>
        /// <param name="message">a message to be sent to the front-end</param>
        /// <returns></returns>
        public static Response Success(string message) => new() { Succeeded = true, Message = message };

        /// <summary>
        /// Creates a new failed response
        /// </summary>
        /// <param name="error">The error message to return</param>
        /// <returns></returns>
        public static Response Fail(string error) => new() { Errors = [error], Succeeded = false };

        /// <summary>
        /// Creates a new failed response
        /// </summary>
        /// <param name="errors">The error messages to return</param>
        /// <returns></returns>
        public static Response Fail(List<string> errors) => new() { Errors = errors, Succeeded = false };

        /// <summary>
        /// Creates a new successful response
        /// </summary>
        /// <returns></returns>
        public static Response<TData> Success<TData>(TData data) => new() { Data = data, Errors = [], Succeeded = true };
    }
}
