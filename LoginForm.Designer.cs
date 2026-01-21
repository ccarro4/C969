using System.Globalization;

namespace SchedulerApp
{
    public partial class LoginForm : Form
    {
        /// <summary>
        ///  LoginForm for Scheduler.
        /// </summary>

        private readonly ILocalizer _localizer;
        private readonly AuthService _authService;
        private readonly LocationService _locationService;

        private Label labelTitle;
        private Label labelUsername;
        private Label labelPassword;
        private TextBox tbUsername;
        private TextBox tbPassword;
        private Button buttonLogin;

        private Label labelStatus;
        private Label labelLocationCapture;
        private Label labelLocationName;
        private ComboBox comboLanguage;

        public LoginForm()
        {
            InitializeComponent();
            /// Language Capture using systeminfo
            Lang initLang = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName.Equals("es", StringComparison.OrdinalIgnoreCase)
                ? Lang.Es
                : Lang.En;

            _localizer = new ILocalizer(initLang);
            _authService = new AuthService();
            _locationService = new LocationService();

            ApplyLocalization();
        }

        private async void LoginForm_Load(object? sender, EventArgs e)
        {
            /// Location Capture
            labelLocationName.Text = _localizer["LocationDetecting"];
            labelLocationName.Text = await _locationService.GetLocationAsync();
        }

        private void comboLanguage_SelectedIndexChanged(object? sender, EventArgs e)
        {
            var selected = comboLanguage.SelectedIndex.ToString();
            if (string.Equals(selected, "English", StringComparison.OrdinalIgnoreCase))
                _localizer.Current = Lang.En;
            else
                _localizer.Current = Lang.Es;

            ApplyLocalization();
        }

        private void ApplyLocalization()
        {
            /// Labels
            Text = _localizer["Title"];
            labelTitle = _localizer["WelcomeTitle"];
            labelUsername = _localizer["Username"];
            labelPassword = _localizer["Password"];
            buttonLogin.Text = _localizer["Login"];
            labelLocationCapture.Text = _localizer["YourLocation"];
            labelStatus.Text = string.Empty;

            comboLanguage.Items.Clear();
            comboLanguage.Items.Add(_localizer["English"]);
            comboLanguage.Items.Add(_localizer["Spanish"]);

            if (_localizer.Current == Lang.En)
                comboLanguage.SelectedItem = _localizer["English"];
            else
                comboLanguage.SelectedItem = _localizer["Spanish"];
        }

        private void buttonLogin_Click(object? sender, EventArgs e)
        {
            /// Validation
            if (string.IsNullOrWhiteSpace(tbUsername.Text) || string.IsNullOrWhiteSpace(tbPassword.Text))
            {
                labelStatus.ForeColor = Color.Red;
                labelStatus.Text = _localizer["MissingCredentials"];
                return;
            }

            bool okCheck = _authService.Verify(txtUsername.Text, txt.Password.Text);
            if (okCheck)
            {
                labelStatus.ForeColor = Color.PaleGreen;
                labelStatus.Text = _localizer["Login Successful"];
            }

            else
            {
                labelStatus.ForeColor = Color.Red;
                labelStatus.Text = _localizer["LoginError"];
            }
        }

        private void InitializeComponent()
        {
            this.Width = 500;
            this.Height = 400;
            this.StartPosition = FormStartPosition.CenterScreen;

            labelTitle = new Label
            {
                AutoSize = true,
                Font = new Font("Century Gothic", 14f, FontStyle.Bold),
                Location = new Point(25, 25)
            };

            tbUsername = new TextBox
            {
                Location = new Point(100, 75),
                Width = 300
            };

            labelPassword = new Label
            {
                AutoSize = true,
                Location = new Point(25, 80),
            };

            tbPassword = new TextBox
            {
                Location = new Point(100, 120),
                Width = 300,
                UseSystemPasswordChar = true
            };

            buttonLogin = new Button
            {
                Width = 100,
                Height = 30,
                Location = new Point(350, 150)
            };
            buttonLogin.Click += buttonLogin_Click;

            labelStatus = new Label
            {
                AutoSize = true,
                Location = new Point(25, 200),
                ForeColor = Color.Red
            };

            comboLanguage = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Location = new Point(25, 150),
                Width = 180
            };
            comboLanguage.SelectedIndexChanged += comboLanguage_SelectedIndexChanged;

            labelLocationCapture = new Label
            {
                AutoSize = true,
                Location = new Point(25, 250),
                Font = new Font("Gothic Century", 10f, FontStyle.Bold)
            };

            labelLocationName = new Label
            {
                AutoSize = true,
                Location = new Point(25, 275)
            };

            this.Load += LoginForm_Load;
            Controls.Add(labelTitle);
            Controls.Add(labelUsername);
            Controls.Add(labelPassword);
            Controls.Add(labelStatus);
            Controls.Add(tbUsername);
            Controls.Add(tbPassword);
            Controls.Add(labelLocationCapture);
            Controls.Add(labelLocationName);
            Controls.Add(comboLanguage);
            Controls.Add(buttonLogin);
        }
    }
    #region Localization
    public enum Lang { En, Es}
    public interface ILocalizer
    {
        string this[string key] { get; }
        Lang Current { get; set; }
    }
    public class Localizer : ILocalizer 
    {
        private static readonly Dictionary<Lang, Dictionary<string, string>> _resources = new()
        {
            [Lang.En] = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                ["Title"] = "Scheduler Login",
                ["WelcomeTitle"] = "Welcome! Please sign in.",
                ["Username"] = "Username",
                ["Password"] = "Password",
                ["Login"] = "Login",
                ["LoginError"] = "Username and password do not match.",
                ["LoginSuccess"] = "Successfully logged in, welcome.",
                ["MissingCredentials"] = "Please enter both username and password.",
                ["YourLocation"] = "Your location",
                ["LocationCapture"] = "Capturing your location...",
                ["English"] = "English",
                ["Spanish"] = "Spanish"
            },
            [Lang.Es] = new Dictionary<Lang, string>.Enumerator().GetType()
            != null ?
            new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                ["Title"] = "Inicio de sesión seguro",
                ["WelcomeTitle"] = "¡Bienvenido! Inicia sesión.",
                ["Username"] = "Usuario",
                ["Password"] = "Contraseña",
                ["Login"] = "Acceder",
                ["LoginError"] = "El nombre de usuario y la contraseña no coinciden.",
                ["LoginSuccess"] = "Inicia de sesión exitoso. ­¡Bienvenido!",
                ["MissingCredentials"] = "Por favor, ingresa el usuario y la contraseña.",
                ["YourLocation"] = "Tu ubicación",
                ["LocationCapture"] = "Detectando ubicación...",
                ["English"] = "Inglés",
                ["Spanish"] = "Español"
            } : null!
        };
    public Lang Current { get; set; }
    public Localizer(Lang lang) => Current = lang;
    public string this[string key]
        {
            get
            {
                if (_resources.TryGetValue(Current, out var dict) && dict.TryGetValue(key, out var value))
                    return value;
                return _resources[Lang.En][key];
            }
        }
    }
    #endregion
    #region Authentication (PBKDF2)
    public sealed class AuthService
    {
        private readonly Dictionary<string, Credential> _users = new(StringComparer.OrdinalIgnoreCase);
        public AuthService()
        {
            AddUser("admin", "admin");
        }
        public void AddUser(string username, string password)
        {

        }
    }
}
