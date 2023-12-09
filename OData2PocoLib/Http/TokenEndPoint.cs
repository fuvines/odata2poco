﻿// Copyright (c) Mohamed Hassan & Contributors. All rights reserved. See License.md in the project root for license information.

using System.Net;
using Newtonsoft.Json.Linq;
using OData2Poco.InfraStructure.Logging;

namespace OData2Poco.Http;

/// <summary>
///     Generate access_token for OAuth2
/// </summary>
internal class TokenEndpoint
{
    private static readonly ILog Logger = PocoLogger.Default;
    private string? _tokenUrl;
    public TokenEndpoint(OdataConnectionString odataConnectionString)
    {
        if (odataConnectionString.TokenUrl != null) TokenUrl = odataConnectionString.TokenUrl;
        TokenParams = SetTokenParams(odataConnectionString);
        TokenParamsCollection = TokenParamsAsDictionary();
    }

    public string TokenUrl
    {
        set => _tokenUrl = value;
        get => _tokenUrl
               ?? throw new InvalidOperationException("Uninitialized property: " + nameof(TokenUrl));
    }

    public string TokenParams { get; set; }

    public Dictionary<string, string> TokenParamsCollection { get; set; }

    //json string 
    public string? LastToken { get; set; }

    internal Dictionary<string, string> TokenParamsAsDictionary()
    {
        if (string.IsNullOrEmpty(TokenParams))
            return [];
        var args = TokenParams.Split(new[] { '&' }, StringSplitOptions.RemoveEmptyEntries);
        return args.Select(StringToKeyValue).ToDictionary(pair => pair.Key, pair => pair.Value);
    }

    internal KeyValuePair<string, string> StringToKeyValue(string arg)
    {
        if (string.IsNullOrEmpty(arg))
            return new KeyValuePair<string, string>();
        var index = arg.IndexOf('=');
        var key = arg[..index];
        var value = arg[(index + 1)..].Trim();
        return new KeyValuePair<string, string>(key, value);
    }
    private string SetTokenParams(OdataConnectionString ocs)
    {
        var clientParams =
            $"grant_type=client_credentials&client_id={ocs.UserName}&client_secret={ocs.Password.GetToken()}";
        var tokenParams = string.IsNullOrEmpty(ocs.TokenParams)
            ? clientParams
            : $"{clientParams}&{ocs.TokenParams}";
        return tokenParams;
    }

    public async Task<string?> GetAccessTokenAsync()
    {
        Logger.Normal($"Start connecting to Token endpoint: {TokenUrl}");
        var token = await GetToken(new Uri(TokenUrl), TokenParamsCollection);
        Logger.Normal("Token endpoint reply with access_token");
        return token;
    }

    /*
Note: The json token is in the form:

token_type     : Bearer
expires_in     : 3600
ext_expires_in : 3600
expires_on     : 1553441019
not_before     : 1553437119
resource       : https://resource.com
access_token   : bi05REFMcXdodUhZbkhRNjNHZUNYYyIsImtpZCI6Ik4tbEMwbi05REFMcXdod...

*/
    public async Task<string?> GetToken(Uri authenticationUrl,
        Dictionary<string, string> authenticationCredentials)
    {
        var client = new HttpClient();
        var content = new FormUrlEncodedContent(authenticationCredentials);
        var response = await client.PostAsync(authenticationUrl, content);
        if (response.StatusCode != HttpStatusCode.OK)
            throw new InvalidOperationException(
                $"Fail to get access_token, Http status code: ({(int)response.StatusCode}) {response.StatusCode}");
        var responseString = await response.Content.ReadAsStringAsync();
        LastToken = responseString;
        return ParseTokenResponse(responseString, "access_token");
    }

    public DateTime ToLocalDateTime(long unixDate)
    {
        var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        var date = epoch.AddSeconds(unixDate);
        return date.ToLocalTime();
    }

    internal DateTime ExpiresOn()
    {
        if (string.IsNullOrEmpty(LastToken))
            return DateTime.MinValue;
        var expireOn = ParseTokenResponse(LastToken, "expires_on");
        var expireOnLong = Convert.ToInt64(expireOn);
        var date = ToLocalDateTime(expireOnLong);
        return date;
    }

    internal string? ParseTokenResponse(string? content, string key)
    {
        if (string.IsNullOrEmpty(content) || string.IsNullOrEmpty(key))
            return null;
        var token = JObject.Parse(content).SelectToken(key);
        return token?.ToString();
    }
}