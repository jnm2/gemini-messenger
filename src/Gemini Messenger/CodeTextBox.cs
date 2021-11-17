namespace Gemini_Messenger
{
    public static class CodeTextBox
    {
        private readonly static Lazy<Font?> Font = new(() =>
        {
            var preferredFontNames = new[] { "Cascadia Mono", "Consolas", "Courier New", "Segoe UI" };

            var preferredFontFamily = preferredFontNames
                .Select(name => FontFamily.Families.FirstOrDefault(f => f.Name == name))
                .FirstOrDefault(family => family is not null);

            return preferredFontFamily is not null ? new Font(preferredFontFamily, 11) : null;
        });

        public static void Apply(TextBox textBox)
        {
            if (Font.Value is { } font)
                textBox.Font = font;

            textBox.AcceptsTab = true;
            textBox.KeyDown += OnKeyDown;
        }

        private static void OnKeyDown(object? sender, KeyEventArgs e)
        {
            var textBox = (TextBox)sender!;

            if (e.KeyData == Keys.Tab)
            {
                textBox.SelectedText = "  ";
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
            else if (e.KeyData == Keys.Enter)
            {
                var firstCharOfCurrentLine = textBox.GetFirstCharIndexOfCurrentLine();
                var indentationEnd = firstCharOfCurrentLine;

                while (indentationEnd < textBox.Text.Length
                    && char.IsWhiteSpace(textBox.Text[indentationEnd]))
                {
                    indentationEnd++;
                }

                var indentation = textBox.Text[firstCharOfCurrentLine..indentationEnd];

                textBox.SelectedText = "\r\n" + indentation;
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
        }
    }
}
