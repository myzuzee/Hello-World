using System;
using System.Drawing;
using System.Windows.Forms;

namespace TrayHelloApp;

public sealed class MainForm : Form
{
    private readonly TextBox _firstNameTextBox;
    private readonly TextBox _lastNameTextBox;
    private readonly Button _greetButton;
    private readonly NotifyIcon _notifyIcon;
    private readonly ContextMenuStrip _trayMenu;
    private bool _allowClose;

    public MainForm()
    {
        Text = "Hello Tray App";
        ClientSize = new Size(360, 180);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        StartPosition = FormStartPosition.CenterScreen;

        var firstNameLabel = new Label
        {
            Text = "First name:",
            AutoSize = true,
            Location = new Point(20, 20)
        };

        _firstNameTextBox = new TextBox
        {
            Location = new Point(120, 16),
            Width = 200
        };

        var lastNameLabel = new Label
        {
            Text = "Last name:",
            AutoSize = true,
            Location = new Point(20, 60)
        };

        _lastNameTextBox = new TextBox
        {
            Location = new Point(120, 56),
            Width = 200
        };

        _greetButton = new Button
        {
            Text = "Say hello",
            Location = new Point(120, 100),
            Width = 120
        };
        _greetButton.Click += GreetButtonOnClick;

        Controls.Add(firstNameLabel);
        Controls.Add(_firstNameTextBox);
        Controls.Add(lastNameLabel);
        Controls.Add(_lastNameTextBox);
        Controls.Add(_greetButton);

        _trayMenu = new ContextMenuStrip();
        var openItem = new ToolStripMenuItem("Open", null, (_, _) => ShowFromTray());
        var exitItem = new ToolStripMenuItem("Exit", null, (_, _) => ExitFromTray());
        _trayMenu.Items.Add(openItem);
        _trayMenu.Items.Add(exitItem);

        _notifyIcon = new NotifyIcon
        {
            Text = "Hello Tray App",
            Icon = SystemIcons.Application,
            Visible = true,
            ContextMenuStrip = _trayMenu
        };
        _notifyIcon.DoubleClick += (_, _) => ShowFromTray();

        Resize += OnResize;
        FormClosing += OnFormClosing;
    }

    private void GreetButtonOnClick(object? sender, EventArgs e)
    {
        var first = _firstNameTextBox.Text.Trim();
        var last = _lastNameTextBox.Text.Trim();

        if (string.IsNullOrWhiteSpace(first) || string.IsNullOrWhiteSpace(last))
        {
            MessageBox.Show("Please enter both first and last name.", "Missing information",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        MessageBox.Show($"Hello, {first} {last}!", "Greeting", MessageBoxButtons.OK, MessageBoxIcon.Information);
        Hide();
    }

    private void OnResize(object? sender, EventArgs e)
    {
        if (WindowState == FormWindowState.Minimized)
        {
            Hide();
        }
    }

    private void OnFormClosing(object? sender, FormClosingEventArgs e)
    {
        if (_allowClose)
        {
            return;
        }

        e.Cancel = true;
        Hide();
    }

    private void ShowFromTray()
    {
        Show();
        WindowState = FormWindowState.Normal;
        BringToFront();
        Activate();
    }

    private void ExitFromTray()
    {
        _allowClose = true;
        _notifyIcon.Visible = false;
        Close();
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _notifyIcon.Dispose();
            _trayMenu.Dispose();
        }

        base.Dispose(disposing);
    }
}
