using CarbonAware.Exceptions;
using CarbonAware.DataSources.ElectricityMaps.Constants;
using CarbonAware.DataSources.ElectricityMaps.Model;

namespace CarbonAware.DataSources.ElectricityMaps.Configuration;

/// <summary>
/// A configuration class for holding ElectricityMaps client config values.
/// </summary>
public class ElectricityMapsClientConfiguration
{
    /// <summary>
    /// API Token Header (i.e 'auth-token')
    /// </summary>
    public string APITokenHeader { get; set; } = string.Empty;

    /// <summary>
    /// Token value to be used with API Token Header
    /// </summary>
    public string APIToken { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the base url to use when connecting to ElectricityMaps
    /// </summary>
    public string BaseUrl { get; set; } = BaseUrls.TokenBaseUrl;

    /// <summary>
    /// Gets or sets the optional emissionFactorType parameter used in API requests  
    /// </summary>
    /// <remarks>
    /// See https://static.electricitymaps.com/api/docs/index.html#emission-factors for valid types
    /// </remarks>
    public string? EmissionFactorType  { get; set; }

    /// <summary>
    /// Gets or sets the optional disableEstimations parameter used in API requests  
    /// </summary>
    /// <remarks>
    /// See https://static.electricitymaps.com/api/docs/index.html#estimations for details
    /// </remarks>
    public bool? DisableEstimations  { get; set; }

    /// <summary>
    /// Validate that this object is properly configured.
    /// </summary>
    public void Validate()
    {
        if (!Uri.IsWellFormedUriString(this.BaseUrl, UriKind.Absolute))
        {
            throw new ConfigurationException($"{nameof(this.BaseUrl)} is not a valid absolute url.");
        }

        // Required to provide full auth config
        if (string.IsNullOrWhiteSpace(this.APITokenHeader) || string.IsNullOrWhiteSpace(this.APIToken)){
            throw new ConfigurationException($"Incomplete auth config: must set both {nameof(this.APITokenHeader)} and {nameof(this.APIToken)}.");
        }

        // Required to have correct token header given base url
        if (BaseUrl.Equals(BaseUrls.TokenBaseUrl) && !APITokenHeader.Equals(Headers.TokenAuthHeader)){
            throw new ConfigurationException($"Invalid configuration. {nameof(this.BaseUrl)} '{this.BaseUrl}' requires '{Headers.TokenAuthHeader}' as the {nameof(this.APITokenHeader)}.");
        }

        if (BaseUrl.Equals(BaseUrls.TrialBaseUrl) && !APITokenHeader.Equals(Headers.TrialAuthHeader)){
            throw new ConfigurationException($"Invalid configuration. {nameof(this.BaseUrl)} '{this.BaseUrl}' requires '{Headers.TrialAuthHeader}' as the {nameof(this.APITokenHeader)}.");
        }
    }
}
