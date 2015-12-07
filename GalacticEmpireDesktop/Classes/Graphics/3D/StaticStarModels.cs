using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace GalacticEmpire
{
    static class StaticStarModels
    {
        ///Modelli delle stelle
        public static Model BlueStar { get; set; }
        public static Model BlueWhiteStar { get; set; }
        public static Model OrangeStar { get; set; }
        public static Model RedStar { get; set; }
        public static Model WhiteStar { get; set; }
        public static Model YellowWhiteStar { get; set; }
        public static Model YellowStar { get; set; }

        public static void LoadModels(Game game)
        {
            BlueStar = game.Content.Load<Model>(@"Stars\Blue");
            BlueWhiteStar = game.Content.Load<Model>(@"Stars\BlueWhite");
            OrangeStar = game.Content.Load<Model>(@"Stars\Orange");
            RedStar = game.Content.Load<Model>(@"Stars\Red");
            WhiteStar = game.Content.Load<Model>(@"Stars\White");
            YellowWhiteStar = game.Content.Load<Model>(@"Stars\YellowWhite");
            YellowStar = game.Content.Load<Model>(@"Stars\Yellow");
        }

        public static Model GetModel(string name)
        {
            switch (name)
            {
                case "Blue":
                    return BlueStar;
                case "BlueWhite":
                    return BlueWhiteStar;
                case "Orange":
                    return OrangeStar;
                case "White":
                    return WhiteStar;
                case "YellowWhite":
                    return YellowWhiteStar;
                case "Yellow":
                    return YellowStar;
                case "Red":
                default:
                    return RedStar;
            }
        }

    }
}
