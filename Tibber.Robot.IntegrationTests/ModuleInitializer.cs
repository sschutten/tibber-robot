using System.Runtime.CompilerServices;

namespace Tibber.Robot.IntegrationTests;

public static class ModuleInitializer
{
    [ModuleInitializer]
    public static void Initialize() =>
        VerifierSettings.InitializePlugins();
}