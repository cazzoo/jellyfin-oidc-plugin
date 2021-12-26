using Microsoft.Extensions.Logging;

namespace Jellyfin.Plugin.OIDC;

internal sealed class OIDCAuthHandler
{
    private readonly ILogger _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="OIDCAuthHandler" /> class.
    /// </summary>
    /// <param name="logger">Instance of the <see cref="ILogger"/> interface.</param>
    public OIDCAuthHandler(ILogger logger)
    {
        _logger = logger;
    }
}
