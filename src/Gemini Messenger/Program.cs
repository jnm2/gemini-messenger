namespace Gemini_Messenger;

public static class Program
{
    [STAThread]
    public static void Main()
    {
        ApplicationConfiguration.Initialize();

        new MainView().Show();
        Application.Run();
    }
}
