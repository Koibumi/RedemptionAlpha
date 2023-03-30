using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Terraria.DataStructures;
using Terraria.ID;
using Redemption.Items.Placeable.Furniture.Misc;
using Terraria.Localization;

namespace Redemption.Tiles.Furniture.Misc
{
    public class AkkaPaintingTile : ModTile
	{
        public override void SetStaticDefaults()
		{
			Main.tileFrameImportant[Type] = true;
			Main.tileLavaDeath[Type] = false;
            Main.tileNoAttach[Type] = true;
            TileID.Sets.FramesOnKillWall[Type] = true;
            TileObjectData.newTile.Width = 6;
            TileObjectData.newTile.Height = 4;
            TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16, 16, 16 };
            TileObjectData.newTile.UsesCustomCanPlace = true;
            TileObjectData.newTile.CoordinateWidth = 16;
            TileObjectData.newTile.CoordinatePadding = 2;
            TileObjectData.newTile.Origin = new Point16(2, 1);
            TileObjectData.newTile.AnchorWall = true;
            TileObjectData.addTile(Type);
            DustType = DustID.WoodFurniture;
			AddMapEntry(new Color(151, 107, 75), Language.GetText("MapObject.Painting"));
        }
    }
}