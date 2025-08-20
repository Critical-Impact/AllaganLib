using Autofac;
using Lumina;
using Lumina.Excel;

namespace AllaganLib.GameSheets.Modules;

public class GameDataModule : Module
{
    public bool IncludeDebugWindows { get; set; }

    protected override void Load(ContainerBuilder containerBuilder)
    {
        containerBuilder.RegisterGeneric((context, parameters) =>
            {
                var gameData = context.Resolve<GameData>();
                var method = typeof(GameData).GetMethod(nameof(GameData.GetExcelSheet))
                    ?.MakeGenericMethod(parameters);
                var sheet = method!.Invoke(gameData, [null, null])!;
                return sheet;
            })
            .As(typeof(ExcelSheet<>));
        containerBuilder.RegisterGeneric((context, parameters) =>
            {
                var gameData = context.Resolve<GameData>();
                var method = typeof(GameData).GetMethod(nameof(GameData.GetSubrowExcelSheet))
                    ?.MakeGenericMethod(parameters);
                var sheet = method!.Invoke(gameData, [null, null])!;
                return sheet;
            })
            .As(typeof(SubrowExcelSheet<>));
    }
}