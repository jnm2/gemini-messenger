namespace Gemini_Messenger;

public partial class KeySelectionView : Form
{
    public KeySelectionView()
    {
        InitializeComponent();
    }

    public string ApiKey { get; private set; } = string.Empty;
    public string ApiSecret { get; private set; } = string.Empty;

    private void UseButton_Click(object sender, EventArgs e)
    {
        apiKeyTextBox.Text = apiKeyTextBox.Text.Trim();
        apiSecretTextBox.Text = apiSecretTextBox.Text.Trim();

        if (apiKeyTextBox.Text.Length == 0)
        {
            MessageBox.Show(owner: this, "Please enter a Gemini API key.", caption: Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        if (!apiKeyTextBox.Text.AsSpan()[1..^1].Contains('-'))
        {
            MessageBox.Show(owner: this, "The Gemini API key is not in a recognized format.", caption: Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        if (apiSecretTextBox.Text.Length == 0)
        {
            MessageBox.Show(owner: this, "Please enter the Gemini API secret.", caption: Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        ApiKey = apiKeyTextBox.Text;
        ApiSecret = apiSecretTextBox.Text;

        DialogResult = DialogResult.OK;
        Close();
    }
}
