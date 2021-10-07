using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.GameContent.Creative;
using Redemption.Items.Materials.PreHM;

namespace Redemption.Items.Armor.PreHM
{
    [AutoloadEquip(EquipType.Legs)]
    public class LivingWoodLeggings : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Living Wood Leggings");

            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 22;
            Item.height = 16;
            Item.sellPrice(copper: 40);
            Item.rare = ItemRarityID.White;
            Item.defense = 2;
        }
     
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<LivingTwig>(), 30)
                .AddTile(TileID.WorkBenches)
                .Register();
        }     
    }
}