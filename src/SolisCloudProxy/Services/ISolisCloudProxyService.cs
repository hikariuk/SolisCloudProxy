// ****************************************************************************
// File: ISolisCloudProxyService.cs
// Created: 2024-4-6 18:36
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

namespace SolisCloudProxy.Services;

/// <summary>
/// Interface for classes implementing a proxy service for the Solis Cloud
/// monitoring API.
/// </summary>
public interface ISolisCloudProxyService
{
    /// <summary>
    /// Proxies the supplied <see cref="HttpRequest"/> to the Solis Cloud
    /// Monitoring API.
    /// </summary>
    /// <param name="httpRequest">
    /// A <see cref="HttpRequest"/> that contains the request to be proxied.
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> that can be used to cancel the operation.
    /// </param>
    /// <returns>
    /// The response from the remote server.
    /// </returns>
    public Task<HttpResponseMessage> ProxyRequestAsync(HttpRequest httpRequest, CancellationToken cancellationToken);
}