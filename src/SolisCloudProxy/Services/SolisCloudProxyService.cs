// ****************************************************************************
// File: SolisCloudProxyService.cs
// Created: 2024-4-6 16:29
// Created By: Chris Crowther
// 
// Copyright © 2024 Chris Crowther
// 
// MIT License
// ===========
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of
// this software and associated documentation files (the "Software"), to deal in
// the Software without restriction, including without limitation the rights to use,
// copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the
// Software, and to permit persons to whom the Software is furnished to do so,
// subject to the following conditions:
// 
// The above copyright notice and this permission notice (including the next paragraph)
// shall be included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.
// 
// LicenseRef-MIT
// ****************************************************************************

using System.Diagnostics;
using System.Net.Http.Headers;
using System.Text;
using System.Security.Cryptography;

namespace SolisCloudProxy.Services;

/// <summary>
/// Provides a proxy for accessing the Solis Cloud monitoring API.
/// </summary>
public class SolisCloudProxyService : ISolisCloudProxyService
{
    private readonly IConfiguration _configuration;

    /// <summary>
    /// Initializes a new instance of the <see cref="SolisCloudProxyService"/> class.
    /// </summary>
    /// <param name="configuration"></param>
    public SolisCloudProxyService(IConfiguration configuration)
    {
        this._configuration = configuration;
    }

    /// <inheritdoc />
    public async Task<HttpResponseMessage> ProxyRequestAsync(HttpRequest httpRequest, CancellationToken cancellationToken)
    {
        string baseUrl = this._configuration["SolisCloud:BaseUrl"];

        if (string.IsNullOrEmpty(baseUrl))
        {
            throw new InvalidOperationException("Missing Solis Cloud base URL.");
        }

        Debug.WriteLine($"Solis Cloud Base URL: {baseUrl}");

        UriBuilder uriBuilder = new(baseUrl)
        {
            Path = httpRequest.Path
        };

        Debug.WriteLine($"Request URL: {uriBuilder}");

        using StreamContent content = await this.CreateRequestStreamContent(
            httpRequest,
            cancellationToken);

        HttpResponseMessage httpResponseMessage = 
            await this.SubmitRequestToSolisCloudAsync(
                uriBuilder.Uri, 
                content, 
                cancellationToken);

        return httpResponseMessage;
    }

    protected async Task<HttpResponseMessage> SubmitRequestToSolisCloudAsync(
        Uri requestUri,
        StreamContent content, 
        CancellationToken cancellationToken)
    {
        HttpClientHandler handler = new()
        {
            ServerCertificateCustomValidationCallback = (_, _, _, _) => true
        };

        HttpClient httpClient = new(handler);

        httpClient.DefaultRequestHeaders.Date = DateTimeOffset.UtcNow;
        httpClient.DefaultRequestHeaders.Authorization = 
            await this.GenerateAuthorizationHeaderAsync(
                "POST",
                content.Headers.ContentMD5,
                (DateTimeOffset)httpClient.DefaultRequestHeaders.Date,
                requestUri.PathAndQuery, 
                cancellationToken);

        HttpResponseMessage httpResponseMessage = 
            await httpClient.PostAsync(requestUri, content, cancellationToken);

        return httpResponseMessage;
    }

    protected async Task<StreamContent> CreateRequestStreamContent(
        HttpRequest httpRequest,
        CancellationToken cancellationToken)
    {
        MemoryStream body = new();
        await httpRequest.Body.CopyToAsync(body, cancellationToken);
        body.Position = 0;

        byte[] calculateMd5 = await this.CalculateMd5Async(body, cancellationToken);

        StreamContent content = new(body);
        
        content.Headers.ContentMD5 = calculateMd5;
        content.Headers.ContentType = httpRequest.ContentType != null ? 
            MediaTypeHeaderValue.Parse(httpRequest.ContentType) : 
            MediaTypeHeaderValue.Parse("application/json");

        return content;
    }

    protected async Task<byte[]> CalculateMd5Async(Stream stream, CancellationToken cancellationToken)
    {
        MD5 md5 = MD5.Create();
        byte[] computedHash = await md5.ComputeHashAsync(stream, cancellationToken);
        
        if (stream.CanSeek)
        {
            stream.Position = 0;
        }

        return computedHash;
    }

    protected async Task<AuthenticationHeaderValue> GenerateAuthorizationHeaderAsync(
        string method, 
        byte[] contentMd5, 
        DateTimeOffset dateTimeOffset, 
        string requestPath, 
        CancellationToken cancellationToken)
    {
        string appId = this._configuration["SolisCloud:AppId"];
        string appSecret = this._configuration["SolisCloud:AppSecret"];

        if (string.IsNullOrEmpty(appId))
        {
            throw new InvalidOperationException("Missing Solis Cloud AppId");
        }

        if (string.IsNullOrEmpty(appSecret))
        {
            throw new InvalidOperationException("Missing Solis Cloud AppSecret");
        }

        string signBody = $"{method}\n{Convert.ToBase64String(contentMd5)}\napplication/json\n{dateTimeOffset.UtcDateTime:R}\n{requestPath}";

        using MemoryStream ms = new(Encoding.UTF8.GetBytes(signBody));

        string authHmac = Convert.ToBase64String(await HMACSHA1.HashDataAsync(Encoding.UTF8.GetBytes(appSecret), ms, cancellationToken));

        AuthenticationHeaderValue ahv = new("API", $"{appId}:{authHmac}");

        return ahv;
    }
}