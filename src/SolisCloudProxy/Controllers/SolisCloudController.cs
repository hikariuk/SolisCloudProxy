// ****************************************************************************
// File: SolisCloudController.cs
// Created: 2024-4-6 16:11
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

using Microsoft.AspNetCore.Mvc;

using SolisCloudProxy.Services;

namespace SolisCloudProxy.Controllers;

/// <summary>
/// Controller providing proxied access to the Solis Cloud Monitoring API.
/// </summary>
[Controller]
[Route("")]
public class SolisCloudController : Controller
{
    private readonly ISolisCloudProxyService _proxyService;

    /// <summary>
    /// Initializes a new instance of the <see cref="SolisCloudController"/>
    /// class.
    /// </summary>
    /// <param name="proxyService">
    /// The proxy service.
    /// </param>
    public SolisCloudController(ISolisCloudProxyService proxyService)
    {
        this._proxyService = proxyService;
    }

    /// <summary>
    /// Retrieves details of the specified inverter.
    /// </summary>
    [HttpPost]
    [Route("{*queryvalues}")]
    public async Task ProxyRequestAsync()
    {
        using HttpResponseMessage response = await this._proxyService.ProxyRequestAsync(this.Request, CancellationToken.None);

        this.Response.StatusCode = (int)response.StatusCode;
        this.Response.ContentType = response.Content.Headers.ContentType != null
            ? response.Content.Headers.ContentType.ToString()
            : "application/json";

        await response.Content.CopyToAsync(this.Response.Body);
    }
}