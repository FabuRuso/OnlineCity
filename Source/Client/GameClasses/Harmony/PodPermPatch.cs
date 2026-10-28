using HarmonyLib;
using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace RimWorldOnlineCity.GameClasses.Harmony
{
    /// <summary>
    /// Just a Container of the exchenge Flags
    /// </summary>
    public static class PodPermPatchFlags__
    {
        public static bool IsDangerous = false;
    }


    /// <summary>
    /// It patches launchable objects at the moment of starting of targeting
    /// </summary>
    [
        HarmonyPatch(typeof(CompLaunchable),
        nameof(CompLaunchable.StartChoosingDestination))
        ]
    public static class CompLaunchable_StartChoosingDestination_SetFlagPatch
    {
        [HarmonyPrefix]
        public static void Prefix(CompLaunchable __instance)
        {
            PodPermPatchFlags__.IsDangerous = UpdateWorldController.ExistsEnemyPawns;
        }
    }


    /// <summary>
    /// It patches launchable objects at the moment of pointing of the target
    /// </summary>
    [HarmonyPatch(
        typeof(CompLaunchable),
        nameof(CompLaunchable.ChoseWorldTarget),
        new Type[]
        {
            typeof(GlobalTargetInfo),
            typeof(Action<PlanetTile, TransportersArrivalAction>),
            typeof(float?)
        }
    )]
    public static class CompLaunchable_ChoseWorldTarget_GetFlagPatch
    {
        /// <summary>
        /// It canceles an original calling of <see cref="CompLaunchable.ChoseWorldTarget"/>, checks the conditions and so recalls with the sames args
        /// </summary>
        /// <param name="__instance"></param>
        /// <param name="target"></param>
        /// <param name="launchAction"></param>
        /// <param name="overrideFuelLevel"></param>
        /// <returns></returns>
        [HarmonyPrefix]
        public static bool Prefix(
            CompLaunchable __instance,
            GlobalTargetInfo target,
            Action<PlanetTile, TransportersArrivalAction> launchAction,
            float? overrideFuelLevel
            )
        {

            if (PodPermPatchFlags__.IsDangerous)
            {
                var transporter = __instance.parent.GetComp<CompTransporter>();
                var pods = transporter.TransportersInGroup(__instance.parent.Map);

                Dialog_MessageBox dialog = Dialog_MessageBox.CreateConfirmation(
                    text: "OCity_PodPermPatch_EnemyCaution".Translate(),
                    confirmedAct: () => {
                        bool isLaunched = CompLaunchable.ChoseWorldTarget(
                            target,
                            __instance.parent.Tile,
                            pods,
                            __instance.MaxLaunchDistanceEver(target.Tile.Layer),
                            launchAction,
                            __instance,
                            overrideFuelLevel
                            );

                        if (isLaunched)
                            PodPermPatchFlags__.IsDangerous = false;
                    },
                    destructive: true
                    );

                Find.WindowStack.Add(dialog);
                return false;
            }

            return true;
        }
    }
}
