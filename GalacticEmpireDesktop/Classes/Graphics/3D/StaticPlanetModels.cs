using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GalacticEmpire
{
    static class StaticPlanetModels
    {
        ///Modelli delle stelle
        public static Model Type1 { get; set; }
        public static Model Type2 { get; set; }
        public static Model Type3 { get; set; }
        public static Model Type4 { get; set; }
        public static Model Type5 { get; set; }
        public static Model Type6 { get; set; }
        public static Model Type7 { get; set; }
        public static Model Type8 { get; set; }
        public static Model Type9 { get; set; }
        public static Model Gas1 { get; set; }
        public static Model Gas2 { get; set; }
        public static Model Gas3 { get; set; }
        public static Model Gas4 { get; set; }
        public static Model Gas5 { get; set; }
        public static Model Gas6 { get; set; }
        public static Model Gas7 { get; set; }
        public static Model Gas8 { get; set; }

        public static void LoadModels(Game game)
        {
            Type1 = game.Content.Load<Model>(@"Planets\Type1");
            Type2 = game.Content.Load<Model>(@"Planets\Type2");
            Type3 = game.Content.Load<Model>(@"Planets\Type3");
            Type4 = game.Content.Load<Model>(@"Planets\Type4");
            Type5 = game.Content.Load<Model>(@"Planets\Type5");
            Type6 = game.Content.Load<Model>(@"Planets\Type6");
            Type7 = game.Content.Load<Model>(@"Planets\Type7");
            Type8 = game.Content.Load<Model>(@"Planets\Type8");
            Type9 = game.Content.Load<Model>(@"Planets\Type9");

            Gas1 = game.Content.Load<Model>(@"Planets\Gas1");
            Gas2 = game.Content.Load<Model>(@"Planets\Gas2");
            Gas3 = game.Content.Load<Model>(@"Planets\Gas3");
            Gas4 = game.Content.Load<Model>(@"Planets\Gas4");
            Gas5 = game.Content.Load<Model>(@"Planets\Gas5");
            Gas6 = game.Content.Load<Model>(@"Planets\Gas6");
            Gas7 = game.Content.Load<Model>(@"Planets\Gas7");
            Gas8 = game.Content.Load<Model>(@"Planets\Gas8");
        }

        public static Model GetModel(string name)
        {
            switch (name)
            {
                case "Type1":
                    return Type1;
                case "Type2":
                    return Type2;
                case "Type3":
                    return Type3;
                case "Type4":
                    return Type4;
                case "Type5":
                    return Type5;
                case "Type6":
                    return Type6;
                case "Type7":
                    return Type7;
                case "Type8":
                    return Type8;
                case "Type9":
                    return Type9;
                case "Gas1":
                    return Gas1;
                case "Gas2":
                    return Gas2;
                case "Gas3":
                    return Gas3;
                case "Gas4":
                    return Gas4;
                case "Gas5":
                    return Gas5;
                case "Gas6":
                    return Gas6;
                case "Gas7":
                    return Gas7;
                case "Gas8":
                default:
                    return Gas8;
            }
        }

    }
}
