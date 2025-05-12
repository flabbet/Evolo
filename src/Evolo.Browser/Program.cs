using DrawiEngine;
using DrawiEngine.Browser;
using Evolo;

public static class Program
{
    public static void Main()
    {
        DrawingEngine engine = BrowserDrawingEngine.CreateDefaultBrowser();

        EvoloApp sampleApp = new EvoloApp();

        engine.RunWithApp(sampleApp);
    }
}
