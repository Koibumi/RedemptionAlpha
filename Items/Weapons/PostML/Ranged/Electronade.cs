using Redemption.Projectiles.Ranged;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Weapons.PostML.Ranged
{
    public class Electronade : ModItem
	{
		public override void SetStaticDefaults()
		{
            Tooltip.SetDefault("Throw an energy-filled grenade");
		}

		public override void SetDefaults()
		{
            Item.width = 14;
            Item.height = 18;
            Item.damage = 250;
            Item.maxStack = 999;
            Item.value = 150;
            Item.rare = ItemRarityID.Purple;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useAnimation = 30;
            Item.useTime = 30;
            Item.UseSound = SoundID.Item7;
            Item.consumable = true;
            Item.noUseGraphic = true;
            Item.noMelee = true;
            Item.DamageType = DamageClass.Ranged;
            Item.autoReuse = true;
            Item.shootSpeed = 12f;
            Item.shoot = ModContent.ProjectileType<Electronade_Proj>();
        }
	}
}
