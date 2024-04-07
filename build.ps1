#!/usr/bin/env pwsh
[CmdletBinding()]
param(
    [Parameter(Mandatory = $False)]
    [ValidateSet("Release", "Debug")]
    $Configuration = "Debug",

    [Parameter(Mandatory = $False)]
    $Tag = "dev"
)

& docker build --build-arg "BUILD_CONFIGURATION=${Configuration}" -t solis-cloud-proxy:${Tag} -f src/SolisCloudProxy/Dockerfile .
