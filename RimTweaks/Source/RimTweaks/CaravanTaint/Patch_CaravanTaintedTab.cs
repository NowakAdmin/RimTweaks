using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace RimTweaks.CaravanTaint
{
    internal sealed class TaintedTabState
    {
        public TransferableOneWayWidget? Widget;
    }

    [HarmonyPatch(typeof(Dialog_FormCaravan), "DoWindowContents")]
    public static class Patch_CaravanTaintedTab
    {
        // Value not present in the Tab enum — DoWindowContents renders nothing for this value
        private const int OurTabValue = 99;
        private const string TaintedLabel = "Tainted";

        private static readonly ConditionalWeakTable<Dialog_FormCaravan, TaintedTabState> States = new();

        private static readonly FieldInfo TabField =
            AccessTools.Field(typeof(Dialog_FormCaravan), "tab");
        private static readonly FieldInfo TransferablesField =
            AccessTools.Field(typeof(Dialog_FormCaravan), "transferables");
        private static readonly FieldInfo ItemsTransferField =
            AccessTools.Field(typeof(Dialog_FormCaravan), "itemsTransfer");
        private static readonly FieldInfo SelectedGetterField =
            AccessTools.Field(typeof(TabRecord), "selectedGetter");

        // ── Transpiler ────────────────────────────────────────────────────────

        static IEnumerable<CodeInstruction> Transpiler(
            IEnumerable<CodeInstruction> instructions, ILGenerator gen)
        {
            var drawTabsMethod = AccessTools.Method(
                typeof(TabDrawer), "DrawTabs",
                new[] { typeof(Rect), typeof(List<TabRecord>) });

            var onGuiMethod = AccessTools.Method(
                typeof(TransferableOneWayWidget), "OnGUI",
                new[] { typeof(Rect) });

            var injectTabsMethod = AccessTools.Method(
                typeof(Patch_CaravanTaintedTab), nameof(InjectTabs));

            var renderAreaMethod = AccessTools.Method(
                typeof(Patch_CaravanTaintedTab), nameof(RenderItemsArea));

            var itemsTransferField =
                AccessTools.Field(typeof(Dialog_FormCaravan), "itemsTransfer");

            bool tabInjected = false;
            bool renderInjected = false;

            var list = instructions.ToList();

            for (int i = 0; i < list.Count; i++)
            {
                var ins = list[i];

                // ── Inject tab: just before TabDrawer.DrawTabs(rect, tabsList) ──
                // Stack at that point: [rect, tabsList]
                // We dup tabsList, push 'this', then call InjectTabs(tabsList, dialog)
                if (!tabInjected && ins.Calls(drawTabsMethod))
                {
                    yield return new CodeInstruction(OpCodes.Dup);           // [rect, tabs, tabs]
                    yield return new CodeInstruction(OpCodes.Ldarg_0);        // [rect, tabs, tabs, this]
                    yield return new CodeInstruction(OpCodes.Call, injectTabsMethod); // [rect, tabs]
                    tabInjected = true;
                }

                // ── Intercept itemsTransfer.OnGUI(rect): ──
                // Original sequence:  ldarg.0 → ldfld itemsTransfer → ldloc rect → callvirt OnGUI
                // We change it to:    ldarg.0 → nop              → ldloc rect → call RenderItemsArea
                if (!renderInjected && ins.LoadsField(itemsTransferField))
                {
                    // Look ahead for callvirt OnGUI(Rect) within the next few instructions
                    for (int j = i + 1; j < list.Count && j < i + 6; j++)
                    {
                        if (list[j].Calls(onGuiMethod))
                        {
                            // Replace ldfld with nop so 'this' stays on stack
                            ins = new CodeInstruction(OpCodes.Nop);
                            // Replace callvirt OnGUI with call RenderItemsArea(Dialog_FormCaravan, Rect)
                            list[j] = new CodeInstruction(OpCodes.Call, renderAreaMethod);
                            renderInjected = true;
                            break;
                        }
                    }
                }

                yield return ins;
            }

            if (!tabInjected)
                Log.Warning("[RimTweaks] Patch_CaravanTaintedTab: DrawTabs injection point not found.");
            if (!renderInjected)
                Log.Warning("[RimTweaks] Patch_CaravanTaintedTab: itemsTransfer.OnGUI injection point not found.");
        }

        // ── Tab injection helper ──────────────────────────────────────────────

        public static void InjectTabs(List<TabRecord> tabs, Dialog_FormCaravan dialog)
        {
            // Skip if our tab is already in the list (list rebuilt each frame)
            if (tabs.Any(t => t.label == TaintedLabel)) return;

            bool isActive = Convert.ToInt32(TabField.GetValue(dialog)) == OurTabValue;
            var state = States.GetOrCreateValue(dialog);

            var taintedTab = new TabRecord(
                TaintedLabel,
                clickedAction: () =>
                {
                    TabField.SetValue(dialog, OurTabValue); // Tab enum is private — set as int
                    RebuildTaintedWidget(dialog, state);
                },
                selected: isActive);

            // selectedGetter: called each frame to determine highlight — more reliable than 'selected'
            SelectedGetterField.SetValue(taintedTab, (Func<bool>)(() =>
                Convert.ToInt32(TabField.GetValue(dialog)) == OurTabValue));

            tabs.Add(taintedTab);
        }

        // ── Render helper (replaces itemsTransfer.OnGUI call) ────────────────

        public static void RenderItemsArea(Dialog_FormCaravan dialog, Rect rect)
        {
            bool taintedActive = Convert.ToInt32(TabField.GetValue(dialog)) == OurTabValue;

            if (taintedActive)
            {
                var state = States.GetOrCreateValue(dialog);
                if (state.Widget == null)
                    RebuildTaintedWidget(dialog, state);
                state.Widget?.OnGUI(rect);
            }
            else
            {
                var itemsWidget = (TransferableOneWayWidget)ItemsTransferField.GetValue(dialog);
                itemsWidget?.OnGUI(rect);
            }
        }

        // ── Widget builder ────────────────────────────────────────────────────

        private static void RebuildTaintedWidget(Dialog_FormCaravan dialog, TaintedTabState state)
        {
            var allTransferables = (List<TransferableOneWay>?)TransferablesField.GetValue(dialog);
            if (allTransferables == null) return;

            var tainted = allTransferables
                .Where(t => t.AnyThing is Apparel app && app.WornByCorpse)
                .ToList();

            state.Widget = new TransferableOneWayWidget(
                transferables:                       tainted,
                sourceLabel:                         null,
                destinationLabel:                    null,
                sourceCountDesc:                     null,
                drawMass:                            true,
                ignorePawnInventoryMass:             IgnorePawnsInventoryMode.Ignore,
                includePawnsMassInMassUsage:         false,
                availableMassGetter:                 null,
                extraHeaderSpace:                    0f,
                ignoreSpawnedCorpseGearAndInventoryMass: false,
                tile:                                null,
                drawMarketValue:                     true,
                drawEquippedWeapon:                  false,
                drawNutritionEatenPerDay:            false,
                drawMechEnergy:                      false,
                drawItemNutrition:                   false,
                drawForagedFoodPerDay:               false,
                drawDaysUntilRot:                    false,
                playerPawnsReadOnly:                 false,
                drawIdeo:                            false,
                drawXenotype:                        false);
        }
    }
}
