using MudBlazor;

namespace Psybook.UI.Client.Services
{
    /// <summary>
    /// Implementation of theme service for managing application appearance.
    /// </summary>
    public sealed class ThemeService : IThemeService
    {
        private bool _isDarkMode = true;
        private readonly MudTheme _theme;

        /// <summary>
        /// Initializes a new instance of the <see cref="ThemeService"/> class.
        /// </summary>
        public ThemeService()
        {
            _theme = CreateDefaultTheme();
        }

        /// <inheritdoc />
        public MudTheme CurrentTheme => _theme;

        /// <inheritdoc />
        public bool IsDarkMode
        {
            get => _isDarkMode;
            set
            {
                if (_isDarkMode != value)
                {
                    _isDarkMode = value;
                    ThemeChanged?.Invoke();
                }
            }
        }

        /// <inheritdoc />
        public event Action? ThemeChanged;

        /// <inheritdoc />
        public void ToggleDarkMode()
        {
            IsDarkMode = !IsDarkMode;
        }

        /// <summary>
        /// Creates the default theme configuration.
        /// </summary>
        private static MudTheme CreateDefaultTheme()
        {
            return new MudTheme
            {
                PaletteLight = new PaletteLight
                {
                    Black = "#110e2d",
                    AppbarText = "#424242",
                    AppbarBackground = "rgba(255,255,255,0.8)",
                    DrawerBackground = "#ffffff",
                    GrayLight = "#e8e8e8",
                    GrayLighter = "#f9f9f9",
                    Primary = "#7e6fff",
                    Secondary = "#ff6b6b",
                    Success = "#51cf66",
                    Info = "#74c0fc",
                    Warning = "#ffd43b",
                    Error = "#ff6b6b"
                },
                PaletteDark = new PaletteDark
                {
                    Primary = "#7e6fff",
                    Surface = "#1e1e2d",
                    Background = "#1a1a27",
                    BackgroundGray = "#151521",
                    AppbarText = "#92929f",
                    AppbarBackground = "rgba(26,26,39,0.8)",
                    DrawerBackground = "#18202c",
                    Secondary = "#ff6b6b",
                    Success = "#51cf66",
                    Info = "#74c0fc",
                    Warning = "#ffd43b",
                    Error = "#ff6b6b"
                },
                LayoutProperties = new LayoutProperties
                {
                    DrawerWidthLeft = "260px",
                    DrawerWidthRight = "300px"
                }
            };
        }
    }
}