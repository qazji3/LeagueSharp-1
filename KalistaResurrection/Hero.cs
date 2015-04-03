﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LeagueSharp;
using LeagueSharp.Common;

namespace KalistaResurrection
{
    public static class Hero
    {
        public static ActiveModes ActiveMode { get; private set; }
        public static bool IsAfterAttack { get { return AfterAttackTarget != null; } }
        public static AttackableUnit AfterAttackTarget { get; private set; }

        public static Orbwalking.Orbwalker Orbwalker { get { return Config.Menu.Orbwalker; } }

        static Hero()
        {
            Core.OnPostTick += OnPostTick;
            Game.OnWndProc += OnWndProc;
            Orbwalking.AfterAttack += OnAfterAttack;
        }

        static void OnPostTick(EventArgs args)
        {
            AfterAttackTarget = null;
        }

        private static void OnWndProc(WndEventArgs args)
        {
            if (MenuGUI.IsChatOpen)
            {
                return;
            }

            if (Config.Keys.AllKeys.Contains(args.WParam))
            {
                var mode = ActiveModes.None;
                if (Config.Keys.Combo.Value.Key == args.WParam)
                {
                    mode = mode | ActiveModes.Combo;
                }
                if (Config.Keys.Harass.Value.Key == args.WParam)
                {
                    mode = mode | ActiveModes.Harass;
                }
                if (Config.Keys.WaveClear.Value.Key == args.WParam)
                {
                    mode = mode | ActiveModes.WaveClear;
                }
                if (Config.Keys.JungleClear.Value.Key == args.WParam)
                {
                    mode = mode | ActiveModes.JungleClear;
                }
                if (Config.Keys.Flee.Value.Key == args.WParam)
                {
                    mode = mode | ActiveModes.Flee;
                }

                switch (args.Msg)
                {
                    case (uint)WindowsMessages.WM_KEYDOWN:

                        ActiveMode = ActiveMode | mode;
                        break;

                    case (uint)WindowsMessages.WM_KEYUP:

                        ActiveMode = ActiveMode ^ mode;
                        break;
                }
            }
        }

        private static void OnAfterAttack(AttackableUnit unit, AttackableUnit target)
        {
            if (unit.IsMe)
            {
                AfterAttackTarget = target;
            }
        }
    }

    [Flags]
    public enum ActiveModes
    {
        None = 0x0,
        Combo = 0x1,
        Harass = 0x2,
        WaveClear = 0x4,
        JungleClear = 0x8,
        Flee = 0x10
    }
}
