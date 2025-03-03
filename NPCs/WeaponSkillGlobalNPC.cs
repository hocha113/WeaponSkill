﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using WeaponSkill.Configs;
using WeaponSkill.Items.DualBlades;
using WeaponSkill.Weapons.StarBreakerWeapon.StarSpinBlade;

namespace WeaponSkill.NPCs
{
    public class WeaponSkillGlobalNPC : GlobalNPC
    {
        public override bool InstancePerEntity => true;
        public List<WeaponSkillGlobalNPCComponent> weaponSkillGlobalNPCComponents = new();
        public bool CanUpdate = true;
        /// <summary>
        /// 特殊的冻结NPC
        /// </summary>
        public int FrozenNPCTime = 0;
        public int FrostFist_Seal = 0; // 霜拳封印术

        /// <summary>
        /// 这一个是根据接触而定的允许伤害
        /// </summary>
        public float HitPlayerTime;
        public override void Load()
        {
            On_NPC.UpdateNPC += On_NPC_UpdateNPC;
        }
        public override void Unload()
        {
            On_NPC.UpdateNPC -= On_NPC_UpdateNPC;
        }
        private static void On_NPC_UpdateNPC(On_NPC.orig_UpdateNPC orig, NPC self, int i)
        {
            if (self.TryGetGlobalNPC<WeaponSkillGlobalNPC>(out var skill) && (!skill.CanUpdate || (skill.FrozenNPCTime > 0)))
            {
                bool flag = false;
                for(int j = 0; j < 3; j++)
                {
                    Tile tile = Main.tile[((self.Bottom + Vector2.UnitY * j * 16) / 16).ToPoint()];
                    if (tile.HasTile && tile.HasUnactuatedTile)
                    {
                        flag = true;
                    }
                }
                if (!flag)
                {
                    orig.Invoke(self, i);
                    return;
                }
                    
                if (skill.FrozenNPCTime > 0)
                {
                    //if(self.realLife != -1)
                    //{
                    //    int time = skill.FrozenNPCTime;
                    //    int realLife = self.realLife;
                    //    NPC owner = Main.npc[realLife];
                    //    while (realLife != -1 && realLife != owner.whoAmI)
                    //    {
                    //        if (owner.TryGetGlobalNPC<WeaponSkillGlobalNPC>(out var skill1))
                    //        {
                    //            skill1.FrozenNPCTime = time;
                    //        }
                    //        realLife = owner.realLife;
                    //        owner = Main.npc[realLife];
                    //    }
                    //    skill.FrozenNPCTime = 0;
                    //}
                    if (--skill.FrozenNPCTime <= 0)
                    {
                        for (int j = 0; j < 30; j++)
                        {
                            Dust dust = Dust.NewDustDirect(self.position, self.width, self.height, DustID.FrostStaff, 0, 0, 150, default, 1.3f);
                            dust.noGravity = true;
                        }
                        NPC n = new NPC();
                        n.SetDefaults(self.type);
                        self.noTileCollide = n.noTileCollide;
                        self.noGravity = n.noGravity;
                    }
                }
                skill.CanUpdate = true;
                return;
            }
            orig.Invoke(self, i);
        }
        public override void ModifyShop(NPCShop shop)
        {
            //if(shop.NpcType == 550) // 酒保
            //{
            //    shop.Add<StrongAle>();
            //}
        }
        public override bool PreAI(NPC npc)
        {
            if (FrozenNPCTime > 0)
            {
                npc.velocity.X *= 0.5f;
                npc.noTileCollide = false;
                npc.noGravity = false;
                return false;
            }
            bool useWindsState = Main.LocalPlayer.GetModPlayer<WindsPlayer>().UseWindsState;
            if (useWindsState && npc.Center.Distance(Main.LocalPlayer.Center) < npc.Size.Length() * 1.5f)
                npc.velocity -= (Main.LocalPlayer.position - npc.position) * 0.05f;
            return base.PreAI(npc);
        }
        public override bool CanHitPlayer(NPC npc, Player target, ref int cooldownSlot)
        {
            if (!CanUpdate || FrozenNPCTime > 0)
            {
                return false;
            }

            #region 碰撞的检测
            if (npc.Hitbox.Intersects(target.Hitbox))
            {
                hitTest:
                float v = npc.velocity.LengthSquared();
                if (v > 0 && v < 5)
                    v *= 5;
                if (npc.realLife != -1 && npc.realLife != npc.whoAmI)
                {
                    npc = Main.npc[npc.realLife];
                    goto hitTest;
                }
                HitPlayerTime += v;
                if(HitPlayerTime > npc.Size.LengthSquared() * 0.01f)
                {
                    HitPlayerTime = 0;
                    return true;
                }
                return false;
            }
            #endregion
            return base.CanHitPlayer(npc, target, ref cooldownSlot);
        }
        public override void ResetEffects(NPC npc)
        {
            weaponSkillGlobalNPCComponents.ForEach(x => x.ResetEffect(npc));
            weaponSkillGlobalNPCComponents.RemoveAll(x => x.Remove);
        }
        public override void AI(NPC npc)
        {
            weaponSkillGlobalNPCComponents.ForEach(x => x.AI(npc));

            #region 霜拳冻结特殊判定
            if(npc.realLife != -1)
            {
                Main.npc[npc.realLife].GetGlobalNPC<WeaponSkillGlobalNPC>().FrozenNPCTime = FrozenNPCTime;
            }
            if (npc.realLife != -1 && Main.npc[npc.realLife].GetGlobalNPC<WeaponSkillGlobalNPC>().FrozenNPCTime > 0) FrozenNPCTime = Main.npc[npc.realLife].GetGlobalNPC<WeaponSkillGlobalNPC>().FrozenNPCTime;
            #endregion
            #region 霜拳封印术
            if (npc.realLife != -1)
            {
                Main.npc[npc.realLife].GetGlobalNPC<WeaponSkillGlobalNPC>().FrostFist_Seal = FrostFist_Seal;
            }
            if (npc.realLife != -1 && Main.npc[npc.realLife].GetGlobalNPC<WeaponSkillGlobalNPC>().FrostFist_Seal > 0) FrostFist_Seal = Main.npc[npc.realLife].GetGlobalNPC<WeaponSkillGlobalNPC>().FrostFist_Seal;
            if (FrostFist_Seal-- >= 0)
            {
                if (npc.velocity.LengthSquared() > 100) npc.velocity = npc.velocity.SafeNormalize(default) * 10;
                npc.damage = npc.defDamage / 5;
                if(FrostFist_Seal == 0)
                {
                    npc.damage = npc.defDamage;
                }
            }
            #endregion
        }
        public override bool PreDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (BossSetting_Config.Init.WoShiHuangLeiLong)
            {
                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
                ModAsset.WoShiHuangLeiLong.Value.CurrentTechnique.Passes[0].Apply();
            }
            return base.PreDraw(npc, spriteBatch, screenPos, drawColor);
        }
        public override void PostDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (BossSetting_Config.Init.WoShiHuangLeiLong)
            {
                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            }
            weaponSkillGlobalNPCComponents.ForEach(x => x.PostDraw(npc, spriteBatch, screenPos, drawColor));

            #region 霜拳的冻结
            if (FrozenNPCTime > 0)
            {
                Color color = drawColor;
                color.A = 0;
                Texture2D tex = TextureAssets.Frozen.Value;
                Vector2 origin = tex.Size() * 0.5f;
                for (int i = 0; i < 2; i++)
                {
                    spriteBatch.Draw(tex, npc.Center - screenPos, null, color, 0, origin, new Vector2((float)npc.width / tex.Width,(float)npc.height / tex.Height) * 1.5f, SpriteEffects.None, 0f);
                }
            }
            #endregion
        }
        /// <summary>
        /// 添加npc组件
        /// </summary>
        /// <param name="npc"></param>
        /// <param name="weaponSkillGlobalNPCComponent"></param>
        /// <param name="cover">覆盖</param>
        public static void AddComponent(NPC npc, WeaponSkillGlobalNPCComponent weaponSkillGlobalNPCComponent,bool cover = false)
        {
            if (!cover)
            {
                npc.GetGlobalNPC<WeaponSkillGlobalNPC>().weaponSkillGlobalNPCComponents.Add(weaponSkillGlobalNPCComponent);
            }
            else
            {
                var find = npc.GetGlobalNPC<WeaponSkillGlobalNPC>().weaponSkillGlobalNPCComponents.Find(x => x.GetType().Equals(weaponSkillGlobalNPCComponent.GetType()));
                if(find != null)
                {
                    find.OnCover(weaponSkillGlobalNPCComponent);
                }
                else
                {
                    npc.GetGlobalNPC<WeaponSkillGlobalNPC>().weaponSkillGlobalNPCComponents.Add(weaponSkillGlobalNPCComponent);
                }
            }
        }
    }
}
