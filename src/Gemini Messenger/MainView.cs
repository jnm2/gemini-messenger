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

    private void MainView_Shown(object sender, EventArgs e)
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

    private async void SendButton_Click(object sender, EventArgs e)
    {
        requestTextBox.Enabled = false;
        sendButton.Enabled = false;
        UseWaitCursor = true;
        try
        {
            var response = await client!.SendAsync(requestTextBox.Text);

            responseTextBox.Text = JsonNode.Parse(response)!.ToJsonString(new JsonSerializerOptions { WriteIndented = true });
        }
        finally
        {
            requestTextBox.Enabled = true;
            sendButton.Enabled = true;
            UseWaitCursor = false;
        }
    }
}
