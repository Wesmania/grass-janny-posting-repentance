using Vintagestory.API.Server;
using Vintagestory.API.Common;
using Vintagestory.GameContent;

using HarmonyLib;

[assembly: ModInfo("Grass Janny Posting: Repentance")]

class Core : ModSystem
{
    public static ICoreServerAPI _api;
    public override void StartServerSide(ICoreServerAPI api)
    {
        _api = api;
        base.StartServerSide(api);
	Patcher.Patch();
    }
}

class Patcher
{
    public static void Patch() {
        var harmony = new Harmony("org.4channel.vspatch");
        harmony.PatchAll();
    }
}

[HarmonyPatch(typeof(Vintagestory.Server.ServerSystemBlockSimulation))]
[HarmonyPatch("tryTickBlock")]
class GrassJannyService {
    static int FixY(int y_bug) {
	var y_mod_32 = (-y_bug - 289) % 32;
	var floor_y_div_32 = ((-y_bug - 289) - (y_mod_32)) / 608;
	return floor_y_div_32 * 32 + y_mod_32;
    }

    static bool Prefix(out int __state, ref Vintagestory.API.Common.Block block, Vintagestory.API.MathTools.BlockPos atPos) {
	if (atPos.Y < 0) {
	    __state = atPos.Y;
	    atPos.Y = GrassJannyService.FixY(atPos.Y);
	    block = Core._api.World.GetBlockAccessor(true, false, false).GetBlock(atPos);
	} else {
	    __state = 0;
	}
        return true;
    }

    static void Postfix(int __state, Vintagestory.API.Common.Block block, Vintagestory.API.MathTools.BlockPos atPos) {
	if (__state != 0) {
	    atPos.Y = __state;
	}
    }
}
