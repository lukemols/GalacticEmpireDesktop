using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace GalacticEmpire
{
    static class GraphicSettings
    {
        public enum Resolution { LD, MD, HD };

        static Rectangle screenBounds;
        static public Rectangle ScreenBounds { get { return screenBounds; } }

        static float aspectRatio;
        static public float AspectRatio { get { return aspectRatio; } }

        static Vector2 centerScreen;
        static public Vector2 CenterScreen { get { return centerScreen; } }

        static public Resolution WindowResolution
        {
            get
            {
                if (screenBounds.Height < 720)
                    return Resolution.LD;
                else if (screenBounds.Height < 900)
                    return Resolution.MD;
                else
                    return Resolution.HD;
            }
        }

        public static void ScreenBoundsChanged(Game game)
        {
            aspectRatio = (float)game.GraphicsDevice.Viewport.Width / game.GraphicsDevice.Viewport.Height;
            screenBounds = new Rectangle(0, 0, game.GraphicsDevice.Viewport.Width, game.GraphicsDevice.Viewport.Height);
            centerScreen = new Vector2(screenBounds.Width / 2, screenBounds.Height / 2);
        }

        public static string GetGUIFolder()
        {
            string GUIFolder = "GUI/";
            if (WindowResolution == Resolution.LD)
                GUIFolder += "LD/";
            else if (WindowResolution == Resolution.MD)
                GUIFolder += "MD/";
            else
                GUIFolder += "HD/";
            return GUIFolder;
        }

    }
}
