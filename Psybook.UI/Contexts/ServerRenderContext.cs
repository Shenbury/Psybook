using Psybook.Shared.Communication;

namespace Psybook.UI.Contexts
{
    /// <inheritdoc/>
    public sealed class ServerRenderContext(IHttpContextAccessor contextAccessor) : IRenderContext
    {
        /// <inheritdoc/>
        public bool IsClient => false;

        /// <inheritdoc/>
        public bool IsServer => true;

        /// <inheritdoc/>
        public bool IsPrerendering => !contextAccessor.HttpContext?.Response.HasStarted ?? false;
    }
}
