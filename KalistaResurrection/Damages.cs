﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LeagueSharp;
using LeagueSharp.Common;

namespace KalistaResurrection
{
    public static class Damages
    {
        private static readonly Obj_AI_Hero Player = ObjectManager.Player;

        private static float[] rawRendDamage = new float[] { 20, 30, 40, 50, 60 };
        private static float[] rawRendDamageMultiplier = new float[] { 0.6f, 0.6f, 0.6f, 0.6f, 0.6f };
        private static float[] rawRendDamagePerSpear = new float[] { 10, 14, 19, 25, 32 };
        private static float[] rawRendDamagePerSpearMultiplier = new float[] { 0.2f, 0.225f, 0.25f, 0.275f, 0.3f };

        public static bool IsRendKillable(this Obj_AI_Base target)
        {
            var hero = target as Obj_AI_Hero;
            return GetRendDamage(target) > target.Health && (hero == null || !hero.HasUndyingBuff());
        }

        public static float GetRendDamage(Obj_AI_Hero target)
        {
            return (float)GetRendDamage(target, -1);
        }

        public static float GetRendDamage(Obj_AI_Base target, int customStacks = -1)
        {
            // Calculate the damage and return
            return ((float)Player.CalcDamage(target, Damage.DamageType.Physical, GetRawRendDamage(target, customStacks)) - 20) * 0.98f;
        }

        public static float GetRawRendDamage(Obj_AI_Base target, int customStacks = -1)
        {
            // Get buff
            var buff = target.GetRendBuff();

            if (buff != null || customStacks > -1)
            {
                return (rawRendDamage[SpellManager.E.Level - 1] + rawRendDamageMultiplier[SpellManager.E.Level - 1] * Player.TotalAttackDamage()) + // Base damage
                       ((customStacks < 0 ? buff.Count : customStacks) - 1) * // Spear count
                       (rawRendDamagePerSpear[SpellManager.E.Level - 1] + rawRendDamagePerSpearMultiplier[SpellManager.E.Level - 1] * Player.TotalAttackDamage()); // Damage per spear
            }

            return 0;
        }

        public static float GetTotalDamage(Obj_AI_Hero target)
        {
            // Auto attack damage
            double damage = Player.GetAutoAttackDamage(target);

            // Q damage
            if (SpellManager.Q.IsReady())
                damage += Player.GetSpellDamage(target, SpellSlot.Q);

            // E stack damage
            if (SpellManager.E.IsReady())
                damage += GetRendDamage(target);

            return (float)damage;
        }

        public static float TotalAttackDamage(this Obj_AI_Base target)
        {
            return target.BaseAttackDamage + target.FlatPhysicalDamageMod;
        }
    }
}
