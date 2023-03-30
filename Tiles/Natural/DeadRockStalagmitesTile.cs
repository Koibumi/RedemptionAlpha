﻿using Microsoft.Xna.Framework.Graphics;
using Redemption.Tiles.Tiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Redemption.Tiles.Natural
{
    public class DeadRockStalagmitesTile : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileSolidTop[Type] = false;
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileTable[Type] = true;
            Main.tileLavaDeath[Type] = false;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style1x2);
            TileObjectData.newTile.DrawYOffset = 4;
            TileObjectData.newTile.AnchorValidTiles = new int[]
            {
                ModContent.TileType<IrradiatedStoneTile>(),
                ModContent.TileType<IrradiatedCrimstoneTile>(),
                ModContent.TileType<IrradiatedEbonstoneTile>()
            };
            TileObjectData.addTile(Type);
            ItemDrop = -1;
            DustType = DustID.Ash;
        }

        public override void SetSpriteEffects(int i, int j, ref SpriteEffects spriteEffects)
        {
            if ((i % 6) < 3)
            {
                spriteEffects = SpriteEffects.FlipHorizontally;
            }
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }

        public override void AnimateIndividualTile(int type, int i, int j, ref int frameXOffset, ref int frameYOffset)
        {
            frameXOffset = i % 3 * 18;
        }
    }
}