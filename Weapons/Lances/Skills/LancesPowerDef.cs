﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeaponSkill.Command;

namespace WeaponSkill.Weapons.Lances.Skills
{
    public class LancesPowerDef : LancesDef
    {
        public LancesPowerDef(LancesProj lancesProj) : base(lancesProj)
        {
        }

        public override void AI()
        {
            Vector2 rotVector = Vector2.UnitX.RotatedBy(-0.5);
            swingHelper.ProjFixedPlayerCenter(player, 0);
            swingHelper.Change_Lerp(rotVector, 0.2f, Vector2.One, 1f, 0f, 0.2f);
            swingHelper.ProjFixedPlayerCenter(player, 0, true);
            swingHelper.SwingAI(lancesProj.SwingLength, player.direction, 0);
            #region 盾的更新

            LancesShield lancesShield = lancesProj.shield;
            lancesShield.Update(Projectile.Center - new Vector2(-1 * player.direction, 2), player.direction);
            lancesShield.InDef = true;
            lancesShield.PowerDef = true;
            PreAttack = true;
            #endregion
            //if (!)
            //{
            //    SkillTimeOut = true;
            //}
            player.velocity.X = 0;
            //Projectile.ai[0]++;
            #region 防御成功击退代码
            player.GetModPlayer<WeaponSkillPlayer>().StatStamina -= 2;
            player.GetModPlayer<WeaponSkillPlayer>().StatStaminaAddTime = 0;
            if(player.GetModPlayer<WeaponSkillPlayer>().StatStamina < 0)
            {
                SkillTimeOut = true;
            }
            if (lancesShield.DefSucceeded)
            {
                Projectile.ai[1] = (int)lancesShield.KNLevel;
                player.GetModPlayer<WeaponSkillPlayer>().StatStamina -= 20 * ((int)lancesShield.KNLevel);
            }
            #endregion
        }
        public override bool ActivationCondition() => player.controlUseTile && player.GetModPlayer<WeaponSkillPlayer>().StatStamina > 20;

        public override bool SwitchCondition()
        {
            if (!WeaponSkill.BowSlidingStep.Current || player.GetModPlayer<WeaponSkillPlayer>().StatStamina <= 0)
            {
                return true;
            }
            return false;
        }
        public override bool CompulsionSwitchSkill(ProjSkill_Instantiation nowSkill) => Projectile.ai[0] > 40 && ActivationCondition();
    }
}
