using Terraria.Audio;
using Microsoft.Xna.Framework;
using Redemption.Globals;
using Redemption.Projectiles.Magic.Noita;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Weapons.HM.Magic
{
    public class Taikasauva : ModItem
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("A witch's staff that shoots a chaotic assortment of spells\n" +
                "'From Noita, with love'");
            Item.staff[Item.type] = true;
            SacrificeTotal = 1;
        }
        private int spellType;
        private int spellCount;
        public override void SetDefaults()
        {
            Item.damage = 38;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 4;
            Item.width = 32;
            Item.height = 32;
            Item.useTime = 12;
            Item.useAnimation = 12;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 4;
            Item.value = Item.buyPrice(0, 6, 75, 0);
            Item.rare = ItemRarityID.Orange;
            Item.UseSound = SoundID.Item20;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<BubbleSparkSpell>();
            Item.ExtraItemShoot(ModContent.ProjectileType<EnergySphereSpell>(), ModContent.ProjectileType<BlackHoleSpell>());
            Item.shootSpeed = 11f;
        }
        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            Vector2 Offset = Vector2.Normalize(velocity) * 36f;

            if (Collision.CanHit(position, 0, 0, position + Offset, 0, 0))
                position += Offset;

            switch (spellType)
            {
                default:
                    type = Item.shoot;
                    velocity = velocity.RotatedByRandom(MathHelper.ToRadians(15));
                    break;
                case 1:
                    type = ModContent.ProjectileType<EnergySphereSpell>();
                    velocity *= 1.35f;
                    break;
                case 2:
                    type = ModContent.ProjectileType<BouncingBurstSpell>();
                    velocity = velocity.RotatedByRandom(MathHelper.ToRadians(5));
                    velocity *= 0.7f;
                    break;
                case 3:
                    type = ModContent.ProjectileType<ConcentratedLightSpell>();
                    damage = (int)(damage * 2.5f);
                    velocity *= 0.1f;
                    break;
                case 4:
                    type = ModContent.ProjectileType<MagicArrowSpell>();
                    velocity = velocity.RotatedByRandom(MathHelper.ToRadians(5));
                    velocity *= 0.4f;
                    break;
                case 5:
                    type = ModContent.ProjectileType<GlowingLanceSpell>();
                    damage = (int)(damage * 1.25f);
                    velocity = velocity.RotatedByRandom(MathHelper.ToRadians(5));
                    velocity *= 0.4f;
                    break;
                case 6:
                    type = ModContent.ProjectileType<LuminousDrillSpell>();
                    knockback *= .5f;
                    break;
                case 7:
                    type = ModContent.ProjectileType<BlackHoleSpell>();
                    damage = (int)(damage * 2.5f);
                    velocity *= .4f;
                    break;
                case 8:
                    type = ModContent.ProjectileType<DiscSpell>();
                    velocity = velocity.RotatedByRandom(MathHelper.ToRadians(6));
                    velocity *= 1.4f;
                    break;
                case 9:
                    type = ModContent.ProjectileType<GigaDiscSpell>();
                    velocity = velocity.RotatedByRandom(MathHelper.ToRadians(6));
                    damage = (int)(damage * 1.5f);
                    velocity *= 1.4f;
                    break;
                case 10:
                    type = ModContent.ProjectileType<OmegaDiscSpell>();
                    damage = (int)(damage * 2.5f);
                    velocity *= 1.4f;
                    break;
            }
        }
        private int spellCountMax;
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            switch (spellType)
            {
                default:
                    SoundEngine.PlaySound(SoundID.Item21, player.position);
                    spellCountMax = 8;
                    break;
                case 1:
                    player.itemAnimationMax = Item.useTime * 2;
                    player.itemTime = Item.useTime * 2;
                    player.itemAnimation = Item.useTime * 2;
                    spellCountMax = 6;
                    break;
                case 2:
                    SoundEngine.PlaySound(SoundID.Item42, player.position);
                    player.itemAnimationMax = (int)(Item.useTime * .75f);
                    player.itemTime = (int)(Item.useTime * .75f);
                    player.itemAnimation = (int)(Item.useTime * .75f);
                    spellCountMax = 12;
                    break;
                case 3:
                    SoundEngine.PlaySound(SoundID.Item125, player.position);
                    player.itemAnimationMax = Item.useTime * 3;
                    player.itemTime = Item.useTime * 3;
                    player.itemAnimation = Item.useTime * 3;
                    spellCountMax = 4;
                    break;
                case 4:
                    spellCountMax = 8;
                    break;
                case 5:
                    spellCountMax = 6;
                    break;
                case 6:
                    SoundEngine.PlaySound(SoundID.Item117, player.position);
                    player.itemAnimationMax = 4;
                    player.itemTime = 4;
                    player.itemAnimation = 4;
                    spellCountMax = 20;
                    break;
                case 7:
                    SoundEngine.PlaySound(SoundID.NPCDeath6, player.position);
                    player.itemAnimationMax = Item.useTime * 5;
                    player.itemTime = Item.useTime * 5;
                    player.itemAnimation = Item.useTime * 5;
                    spellCountMax = 3;
                    break;
                case 8:
                    spellCountMax = 7;
                    SoundEngine.PlaySound(SoundID.Item23 with { Pitch = .5f }, player.position);
                    break;
                case 9:
                    SoundEngine.PlaySound(SoundID.Item23, player.position);
                    player.itemAnimationMax = Item.useTime * 2;
                    player.itemTime = Item.useTime * 2;
                    player.itemAnimation = Item.useTime * 2;
                    spellCountMax = 5;
                    break;
                case 10:
                    SoundEngine.PlaySound(SoundID.Item23 with { Pitch = -.7f }, player.position);
                    player.itemAnimationMax = Item.useTime * 6;
                    player.itemTime = Item.useTime * 6;
                    player.itemAnimation = Item.useTime * 6;
                    spellCountMax = 1;
                    break;
            }
            if (spellCount++ >= spellCountMax - 1)
            {
                spellType = Main.rand.Next(11);
                spellCount = 0;
            }
            return true;
        }
    }
}
