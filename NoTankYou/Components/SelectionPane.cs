﻿using System.Collections.Generic;
using System.Numerics;
using System.Reflection;
using Dalamud.Interface;
using ImGuiNET;
using NoTankYou.Interfaces;
using NoTankYou.Localization;
using NoTankYou.Tabs;
using NoTankYou.Utilities;

namespace NoTankYou.Components
{
    internal class SelectionPane
    {
        private Vector2 AvailableArea => ImGui.GetContentRegionAvail();
        public float Padding { get; set; }
        public float SelectionPaneWidth { get; set; }

        private ITab? SelectedTab;

        private readonly List<ITab> Tabs = new()
        {
            new DisplayTab(),
            new ModulesTab(),
            new SettingsTab(),
        };

        public void Draw()
        {
            var scaledPadding = Padding * ImGuiHelpers.GlobalScale;
            var moduleSelectionWidth = SelectionPaneWidth *ImGuiHelpers.GlobalScale + Padding;
            var remainderWidth = AvailableArea.X - moduleSelectionWidth;

            if (ImGui.BeginChild("SelectionPane", new Vector2(moduleSelectionWidth, 0), false))
            {
                DrawSelectionPane();
            }
            ImGui.EndChild();

            ImGui.SameLine();

            DrawUtilities.VerticalLine();

            ImGuiHelpers.ScaledDummy(0.0f);

            ImGui.SameLine();

            if (ImGui.BeginChild("ConfigurationPane", new Vector2(remainderWidth - scaledPadding - (10.0f * ImGuiHelpers.GlobalScale), 0), false, ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse))
            {
                if (SelectedTab?.SelectedTabItem != null)
                {
                    SelectedTab.SelectedTabItem.DrawConfigurationPane();
                }
                else
                {
                    var available = ImGui.GetContentRegionAvail() / 2.0f;
                    var textSize = ImGui.CalcTextSize(Strings.Configuration.NoSelection) / 2.0f;
                    var center = new Vector2(available.X - textSize.X, available.Y - textSize.Y);

                    ImGui.SetCursorPos(center);
                    ImGui.Text(Strings.Configuration.NoSelection);
                }
            }
            ImGui.EndChild();
        }

        private void DrawSelectionPane()
        {
            ImGui.PushID("SelectionPane");

            if (ImGui.BeginTabBar("SelectionPaneTabBar", ImGuiTabBarFlags.Reorderable))
            {
                foreach (var tab in Tabs)
                {
                    if (ImGui.BeginTabItem(tab.TabName))
                    {
                        ImGui.PushID(tab.TabName);

                        SelectedTab = tab;

                        var fadedTextColor = GetFadedTextColor();
                        ImGui.TextColored(fadedTextColor, tab.Description);

                        ImGui.Spacing();

                        if (ImGui.BeginChild("SelectionPaneTabBarChild", new Vector2(0,-25), false))
                        {
                            tab.DrawTabContents();
                        }
                        ImGui.EndChild();

                        DrawVersionText();

                        ImGui.PopID();

                        ImGui.EndTabItem();
                    }
                }

                ImGui.EndTabBar();
            }

            ImGui.PopID();
        }

        private Vector4 GetFadedTextColor()
        {
            var userColor = ImGui.GetStyle().Colors[(int) ImGuiCol.Text];

            return userColor with {W = 0.5f};
        }

        private string GetVersionText()
        {
            var assemblyInformation = Assembly.GetExecutingAssembly().FullName!.Split(',');

            return assemblyInformation[1].Replace('=', ' ');
        }

        private void DrawVersionText()
        {
            var region = ImGui.GetContentRegionAvail();
            var versionText = GetVersionText();

            if (Service.Configuration.DeveloperMode)
            {
                var commitInfo = Assembly.GetExecutingAssembly().GetCustomAttribute< AssemblyInformationalVersionAttribute >()?.InformationalVersion ?? "Unknown";
                versionText += " - " + commitInfo;
            }

            var versionTextSize = ImGui.CalcTextSize(versionText) / 2.0f;
            var cursorStart = ImGui.GetCursorPos();
            cursorStart.X += region.X / 2.0f - versionTextSize.X;
            ImGui.SetCursorPos(cursorStart);
            ImGui.TextColored(Colors.Grey, versionText);
        }
    }
}
