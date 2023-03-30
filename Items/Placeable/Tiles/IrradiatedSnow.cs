using Redemption.Tiles.Tiles;
using Terraria.ModLoader;
using Terraria;
using Terraria.ID;
using Terraria.GameContent;

namespace Redemption.Items.Placeable.Tiles
{
    public class IrradiatedSnow : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemTrader.ChlorophyteExtractinator.AddOption_OneWay(Type, 1, ItemID.SnowBlock, 1);
            Item.ResearchUnlockCount = 100;
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<IrradiatedSnowTile>(), 0);
            Item.width = 16;
            Item.height = 16;
            Item.maxStack = Item.CommonMaxStack;
        }
    }
}
