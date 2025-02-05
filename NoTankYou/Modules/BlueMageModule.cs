﻿using System.Collections.Generic;
using System.Linq;
using Dalamud.Game.ClientState.Objects.SubKinds;
using Lumina.Excel.GeneratedSheets;
using NoTankYou.Components;
using NoTankYou.Data.Components;
using NoTankYou.Data.Modules;
using NoTankYou.Enums;
using NoTankYou.Interfaces;
using NoTankYou.Localization;

namespace NoTankYou.Modules
{
    internal class BlueMageModule : IModule
    {
        public List<uint> ClassJobs { get; }
        public GenericSettings GenericSettings => Settings;
        public string MessageLong => Strings.Modules.BlueMage.GenericWarning;
        public string MessageShort => Strings.Modules.BlueMage.GenericWarning;
        public string ModuleCommand => "blu";
        private static BlueMageModuleSettings Settings => Service.Configuration.ModuleSettings.BlueMage;

        private readonly List<uint> MimicryStatusEffects;
        private readonly uint MightyGuardStatusEffect = 1719;
        private readonly uint BasicInstinct = 2498;

        private readonly Action MimicryAction;
        private readonly Action MightyGuardAction;
        private readonly Action BasicInstinctAction;

        public BlueMageModule()
        {
            ClassJobs = new List<uint> { 36 };

            MimicryStatusEffects = new List<uint>{ 2124, 2125, 2126 };

            MimicryAction = Service.DataManager.GetExcelSheet<Action>()!.GetRow(18322)!;
            MightyGuardAction = Service.DataManager.GetExcelSheet<Action>()!.GetRow(11417)!;
            BasicInstinctAction = Service.DataManager.GetExcelSheet<Action>()!.GetRow(23276)!;
        }

        public WarningState? EvaluateWarning(PlayerCharacter character)
        {
            if (Settings.Mimicry && !Service.EventManager.DutyStarted && !character.StatusList.Any(status => MimicryStatusEffects.Contains(status.StatusId)))
            {
                return new WarningState
                {
                    MessageShort = Strings.Modules.BlueMage.MimicryLabel,
                    MessageLong = Strings.Modules.BlueMage.Mimicry,
                    IconID = MimicryAction.Icon,
                    IconLabel = MimicryAction.Name.ToString(),
                    Priority = GenericSettings.Priority,
                    Sender = ModuleType.BlueMage,
                    SenderSubtype = SenderSubtype.AetherialMimicry,
                };
            }

            if (Settings.TankStance)
            {
                if (Service.PartyList.Length == 0)
                {
                    if (!character.StatusList.Any(status => status.StatusId == MightyGuardStatusEffect))
                    {
                        return new WarningState
                        {
                            MessageShort = Strings.Modules.BlueMage.MightyGuardLabel,
                            MessageLong = Strings.Modules.BlueMage.MightyGuard,
                            IconID = MightyGuardAction.Icon,
                            IconLabel = MightyGuardAction.Name.ToString(),
                            Priority = GenericSettings.Priority,
                            Sender = ModuleType.BlueMage,
                            SenderSubtype = SenderSubtype.MightyGuard,
                        };
                    }
                }
                else if(Service.PartyList.Length > 1)
                {
                    var partyBlueMages = Service.PartyList.Where(partyMember => partyMember.CurrentHP > 0 && ClassJobs.Contains(partyMember.ClassJob.Id));
                    var anyMightyGuard = partyBlueMages.Any(tanks => tanks.Statuses.Any(status => status.StatusId == MightyGuardStatusEffect));

                    if (!anyMightyGuard)
                    {
                        return new WarningState
                        {
                            MessageShort = Strings.Modules.BlueMage.MightyGuardLabel,
                            MessageLong = Strings.Modules.BlueMage.MightyGuard,
                            IconID = MightyGuardAction.Icon,
                            IconLabel = MightyGuardAction.Name.ToString(),
                            Priority = GenericSettings.Priority,
                            Sender = ModuleType.BlueMage,
                            SenderSubtype = SenderSubtype.MightyGuard,

                        };
                    }
                }
            }

            if (Settings.BasicInstinct && !character.StatusList.Any(status => status.StatusId == BasicInstinct))
            {
                if (Service.PartyList.Length == 0)
                {
                    return new WarningState
                    {
                        MessageShort = Strings.Modules.BlueMage.BasicInstinctLabel,
                        MessageLong = Strings.Modules.BlueMage.BasicInstinct,
                        IconID = BasicInstinctAction.Icon,
                        IconLabel = BasicInstinctAction.Name.ToString(),
                        Priority = GenericSettings.Priority,
                        Sender = ModuleType.BlueMage,
                        SenderSubtype = SenderSubtype.BasicInstinct,
                    };
                }
            }

            return null;
        }
    }
}
