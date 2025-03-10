/*
 * Immutable zkEVM API
 *
 * Immutable Multi Rollup API
 *
 * The version of the OpenAPI document: 1.0.0
 * Contact: support@immutable.com
 * Generated by: https://github.com/openapitools/openapi-generator.git
 */


using System;
using UnityEngine.Networking;

namespace Immutable.Api.ZkEvm.Client
{
    // Thrown when a backend doesn't return an expected response based on the expected type
    // of the response data.
    public class UnexpectedResponseException : Exception
    {
        public int ErrorCode { get; private set; }

        // NOTE: Cannot keep reference to the request since it will be disposed.
        public UnexpectedResponseException(UnityWebRequest request, System.Type type, string extra = "")
            : base(CreateMessage(request, type, extra))
        {
            ErrorCode = (int)request.responseCode;
        }

        private static string CreateMessage(UnityWebRequest request, System.Type type, string extra)
        {
            return $"httpcode={request.responseCode}, expected {type.Name} but got data: {extra}";
        }
    }
}
