using MediaBrowser.Model.Plugins;

namespace Jellyfin.Plugin.OIDC.Configuration;

/// <summary>
/// Plugin configuration.
/// </summary>
public class PluginConfiguration : BasePluginConfiguration
{
    private string[] defaultScopes = { "openid", "profile" };

    /// <summary>
    /// Initializes a new instance of the <see cref="PluginConfiguration"/> class.
    /// </summary>
    public PluginConfiguration()
    {
        Authority = string.Empty;
        ApiClient = string.Empty;
        ClientId = string.Empty;
        RedirectUri = string.Empty;
        Scopes = this.defaultScopes;
    }

    /// <summary>
    /// Gets or sets a new instance of the <see cref="PluginConfiguration"/> class.
    /// </summary>
    public string? Authority { get; set; }

    /// <summary>
    /// Gets or sets a new instance of the <see cref="PluginConfiguration"/> class.
    /// </summary>
    public string ApiClient { get; set; }

    /// <summary>
    /// Gets or sets a new instance of the <see cref="PluginConfiguration"/> class.
    /// </summary>
    public string ClientId { get; set; }

    /// <summary>
    /// Gets or sets a new instance of the <see cref="PluginConfiguration"/> class.
    /// </summary>
    public string RedirectUri { get; set; }

    /// <summary>
    /// Gets or sets a new instance of the <see cref="PluginConfiguration"/> class.
    /// </summary>
    public string[] Scopes { get; set; }
}
