using Psybook.Shared.Communication;

namespace Psybook.UI.Client.Renderers
{
    /// <inheritdoc/>
    public sealed class ClientRenderContext : IRenderContext
    {
        /// <inheritdoc/>
        public bool IsClient => true;

        /// <inheritdoc/>
        public bool IsServer => false;

        /// <inheritdoc/>
        public bool IsPrerendering => false;
    }
}
