﻿using System;
using Dalamud.Interface;
using ImGuiNET;
using NoTankYou.Components;
using NoTankYou.Data.Components;
using NoTankYou.Data.Modules;
using NoTankYou.Enums;
using NoTankYou.Interfaces;
using NoTankYou.Localization;
using NoTankYou.Utilities;

namespace NoTankYou.ModuleConfiguration
{
    internal class FoodConfiguration : ICustomConfigurable
    {
        public ModuleType ModuleType => ModuleType.Food;
        public string ConfigurationPaneLabel => Strings.Modules.Food.ConfigurationPanelLabel;
        public GenericSettings GenericSettings => Settings;
        private static FoodModuleSettings Settings => Service.Configuration.ModuleSettings.Food;

        private readonly InfoBox EarlyWarningTime = new()
        {
            Label = Strings.Modules.Food.EarlyWarningLabel,
            ContentsAction = () =>
            {
                ImGui.SetNextItemWidth(75.0f * ImGuiHelpers.GlobalScale);
                ImGui.InputInt(Strings.Common.Labels.Seconds, ref Settings.FoodEarlyWarningTime, 0, 0);
                if (ImGui.IsItemDeactivatedAfterEdit())
                {
                    Settings.FoodEarlyWarningTime = Math.Clamp(Settings.FoodEarlyWarningTime, 0, 3600);
                }
            }
        };

        private readonly InfoBox ZoneFilters = new()
        {
            Label = Strings.Modules.Food.ZoneFilters,
            ContentsAction = () =>
            {
                ImGui.Text(Strings.Modules.Food.ZoneFiltersDescription);

                ImGui.Spacing();

                if (ImGui.Checkbox(Strings.Common.Labels.Savage, ref Settings.SavageDuties))
                {
                    Service.Configuration.Save();
                }

                if (ImGui.Checkbox(Strings.Common.Labels.Ultimate, ref Settings.UltimateDuties))
                {
                    Service.Configuration.Save();
                }

                if (ImGui.Checkbox(Strings.Common.Labels.ExtremeUnreal, ref Settings.ExtremeUnreal))
                {
                    Service.Configuration.Save();
                }
            }
        };

        private readonly InfoBox AdditionalOptions = new()
        {
            Label = Strings.Modules.Food.AdditionalOptionsLabel,
            ContentsAction = () =>
            {
                if (ImGui.Checkbox(Strings.Modules.Food.SuppressInCombat, ref Settings.DisableInCombat))
                {
                    Service.Configuration.Save();
                }
            }
        };

        public void DrawTabItem()
        {
            ImGui.TextColored(Settings.Enabled ? Colors.SoftGreen : Colors.SoftRed, Strings.Modules.Food.Label);
        }

        public void DrawOptions()
        {
            ZoneFilters.DrawCentered();
            
            ImGuiHelpers.ScaledDummy(30.0f);
            AdditionalOptions.DrawCentered();
            
            ImGuiHelpers.ScaledDummy(30.0f);
            EarlyWarningTime.DrawCentered();
        }
    }
}
