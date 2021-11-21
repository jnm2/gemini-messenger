using System.Text.Json;
using System.Text.Json.Nodes;

namespace Gemini_Messenger;

public partial class MainView : Form
{
    private GeminiClient? client;

    public MainView()
    {
        InitializeComponent();

        CodeTextBox.Apply(requestTextBox);
        CodeTextBox.Apply(responseTextBox);

        requestTextBox.Text = "{\r\n  \"request\": \"/v1/\",\r\n  \"account\": \"primary\"\r\n}";
        requestTextBox.Select(21, 0);
    }

    private MainView(GeminiClient existingClient, string initialRequest) : this()
    {
        client = existingClient;
        requestTextBox.Text = initialRequest;
    }

    private void MainView_Shown(object sender, EventArgs e)
    {
        if (client is null)
        {
            using var keySelectionView = new KeySelectionView();

            if (keySelectionView.ShowDialog(owner: this) != DialogResult.OK)
            {
                Close();
                return;
            }

            client = new(keySelectionView.ApiKey, keySelectionView.ApiSecret);

            Text = $"Gemini Messenger ({keySelectionView.ApiKey})";
        }
    }

    private async void SendButton_Click(object sender, EventArgs e)
    {
        requestTextBox.Enabled = false;
        sendButton.Enabled = false;
        UseWaitCursor = true;
        try
        {
            var response = await client!.SendAsync(requestTextBox.Text);
            if (response.IsError(out var message))
            {
                MessageBox.Show(owner: this, message, caption: Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            responseTextBox.Text = JsonNode.Parse(response.Value)!.ToJsonString(new JsonSerializerOptions { WriteIndented = true });
        }
        finally
        {
            requestTextBox.Enabled = true;
            sendButton.Enabled = true;
            UseWaitCursor = false;
        }
    }

    private void DuplicateWindowButton_Click(object sender, EventArgs e)
    {
        var newWindow = new MainView(
            existingClient: client!,
            initialRequest: requestTextBox.Text) { Text = Text };

        newWindow.Show();
    }

    private void MainView_FormClosed(object sender, FormClosedEventArgs e)
    {
        if (Application.OpenForms.Count == 0)
            Application.Exit();
    }
}
