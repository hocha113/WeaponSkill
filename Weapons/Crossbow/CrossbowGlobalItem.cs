﻿using Microsoft.CodeAnalysis.FlowAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeaponSkill.Weapons.Crossbow.Parts;
using WeaponSkill.Weapons.LongSword;

namespace WeaponSkill.Weapons.Crossbow
{
    public class CrossbowGlobalItem : BasicWeaponItem<CrossbowGlobalItem>
    {
        public static Dictionary<int, List<int>> CrossbowCanAddPart;
        public static bool ShowTheUI;
        public List<Item> Crossbow_Parts;
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            WeaponID = new() { 1229,1194,578,436,481,1201,1187,435 };
            CrossbowCanAddPart = new();
            AddCrossbowAddCanPart();
        }
        public override void SetDefaults(Item entity)
        {
            Crossbow_Parts = new List<Item>();
        }
        public override void HoldItem(Item item, Player player)
        {
            //if (player.ownedProjectileCounts[ModContent.ProjectileType<LongSwordProj>()] <= 0) // 生成手持弹幕
            //{
            //    int proj = Projectile.NewProjectile(player.GetSource_ItemUse(item), player.position, Vector2.Zero, ModContent.ProjectileType<LongSwordProj>(), player.GetWeaponDamage(item), player.GetWeaponKnockback(item), player.whoAmI);
            //    Main.projectile[proj].originalDamage = Main.projectile[proj].damage;
            //}
        }
        public override void UpdateInventory(Item item, Player player)
        {
            if (!Main.playerInventory)
            {
                ShowTheUI = false;
            }
        }
        public override bool CanRightClick(Item item)
        {
            ShowTheUI = true;
            return base.CanRightClick(item);
        }
        public static void AddCrossbowAddCanPart()
        {
            CrossbowCanAddPart.Add(1229, new()
            {
            });
        }
    }
}
