using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System;

namespace GalacticEmpire
{
    static class CommerceUIDesigner
    {
        static SpriteFont font;
        static Texture2D inGameButton;
        static Texture2D buttonTexture;
        static Texture2D buttonClickedTexture;
        static Texture2D closeTexture;
        static Texture2D foodIcon;
        static Texture2D toolIcon;
        static Texture2D tecnoIcon;
        static Dictionary<string, Texture2D> powerUpTextures;
        static SpriteBatch spriteBatch;

        static List<Button> commerceButtons;
        static List<CommerceButton<Product>> buyButtons;
        static List<CommerceButton<Product>> sellButtons;
        static List<CommerceButton<PlayerPowerUp>> powerButtons;
        static Button closeButton;

        static QuantitySelectionItem selectionBox;
        static bool selectionItemOpened;

        static GraphicsDevice device;
        static Texture2D rectangle;
        static Texture2D mistedScreen;
        static Rectangle relativeScreen;

        static Planet planet;

        static string selectedButton;

        static public void Load(Game game)
        {
            string GUIFolder = GraphicSettings.GetGUIFolder();

            relativeScreen = new Rectangle(100, 100, GraphicSettings.ScreenBounds.Width - 100, GraphicSettings.ScreenBounds.Height - 100);
            spriteBatch = (SpriteBatch)game.Services.GetService(typeof(SpriteBatch));
            commerceButtons = new List<Button>();
            buyButtons = new List<CommerceButton<Product>>();
            sellButtons = new List<CommerceButton<Product>>();
            powerButtons = new List<CommerceButton<PlayerPowerUp>>();

            font = game.Content.Load<SpriteFont>(GUIFolder + "Consolas");
            closeTexture = game.Content.Load<Texture2D>(GUIFolder + "Close");
            foodIcon = game.Content.Load<Texture2D>(GUIFolder + "/Icons/FoodIcon");
            toolIcon = game.Content.Load<Texture2D>(GUIFolder + "/Icons/ToolIcon");
            tecnoIcon = game.Content.Load<Texture2D>(GUIFolder + "/Icons/TecnoIcon");
            inGameButton = game.Content.Load<Texture2D>(GUIFolder + "InGameButton");

            powerUpTextures = new Dictionary<string, Texture2D>();
            string[] names = { "CargoIcon", "EngineIcon", "LaserIcon", "RocketIcon", "ShieldIcon", "TerraformIcon"};
            foreach(string s in names)
                powerUpTextures.Add(s, game.Content.Load<Texture2D>(GUIFolder + "/Icons/" + s));
            

            device = game.GraphicsDevice;
            CreateDefaultQuantitySelectionItem(game.Content);

            Color[] data = new Color[(relativeScreen.Width - 100) * (relativeScreen.Height - 100)];
            Color[] mdata = new Color[(GraphicSettings.ScreenBounds.Width) * (GraphicSettings.ScreenBounds.Height)];
            rectangle = new Texture2D(device, relativeScreen.Width - 100, relativeScreen.Height - 100);
            mistedScreen = new Texture2D(device, GraphicSettings.ScreenBounds.Width, GraphicSettings.ScreenBounds.Height);

            for (int i = 0; i < data.Length; ++i)
                data[i] = new Color(200, 200, 200, 10);
            for (int i = 0; i < mdata.Length; ++i)
                mdata[i] = new Color(64, 64, 64, 64);

            rectangle.SetData(data);
            mistedScreen.SetData(mdata);
            SetButtons();
        }

        static void SetButtons()
        {
            int Width = GraphicSettings.ScreenBounds.Width;
            int Height = GraphicSettings.ScreenBounds.Height;
            closeButton = (new Button(new Rectangle(relativeScreen.Width, relativeScreen.Y, closeTexture.Width, closeTexture.Height), "CloseButton"));
            closeButton.LoadTexture(closeTexture);

            Vector2 buttonDimensions = new Vector2((Width - 200) / 6, (Height - 250) / 8);

            Color[] data = new Color[(int)(buttonDimensions.X * buttonDimensions.Y)];
            Color[] dataClicked = new Color[(int)(buttonDimensions.X * buttonDimensions.Y)];
            buttonTexture = new Texture2D(device, (int)(buttonDimensions.X), (int)(buttonDimensions.Y));
            buttonClickedTexture = new Texture2D(device, (int)(buttonDimensions.X), (int)(buttonDimensions.Y));

            for (int i = 0; i < data.Length; ++i)
            {
                data[i] = Color.Yellow;
                dataClicked[i] = Color.Orange;
            }
            buttonTexture.SetData(data);
            buttonClickedTexture.SetData(dataClicked);

            commerceButtons.Add(new Button(new Rectangle(relativeScreen.Width - (int)buttonDimensions.X - 10, relativeScreen.Y + 50, (int)buttonDimensions.X, (int)buttonDimensions.Y), "Compra", "Buy"));
            commerceButtons.Add(new Button(new Rectangle(relativeScreen.Width - (int)buttonDimensions.X - 10, relativeScreen.Y + 60 + (int)buttonDimensions.Y, (int)buttonDimensions.X, (int)buttonDimensions.Y), "Vendi", "Sell"));
            commerceButtons.Add(new Button(new Rectangle(relativeScreen.Width - (int)buttonDimensions.X - 10, relativeScreen.Y + 70 + 2 * (int)buttonDimensions.Y, (int)buttonDimensions.X, (int)buttonDimensions.Y), "Potenziamenti", "PowerUps"));
            
            foreach (Button b in commerceButtons)
            {
                b.LoadTextureAndFont(buttonTexture, font);
                b.SetTextPosition();
                b.SetTextColor(Color.Black);
            }
        }

        static void CreateDefaultQuantitySelectionItem(ContentManager Content)
        {
            //public QuantitySelectionItem(Texture2D objectImage, Texture2D pTexture, Texture2D mTexture, Texture2D btTexture,
            //SpriteFont font, int max, int ppu)
            string GUIFolder = GraphicSettings.GetGUIFolder();
            Texture2D pTexture = Content.Load<Texture2D>(GUIFolder + "Plus");
            Texture2D mTexture = Content.Load<Texture2D>(GUIFolder + "Minus");

            Vector2 buttonDimensions = new Vector2(pTexture.Width, pTexture.Width);

            Color[] data = new Color[(int)(buttonDimensions.X * buttonDimensions.Y)];
            Texture2D boxTexture = new Texture2D(device, (int)(buttonDimensions.X), (int)(buttonDimensions.Y));

            for (int i = 0; i < data.Length; ++i)
            {
                data[i] = Color.White;
            }
            boxTexture.SetData(data);

            selectionBox = new QuantitySelectionItem(foodIcon, pTexture, mTexture, inGameButton, boxTexture, font);
        }

        static public void LoadWindowComponents(Planet commercePlanet)
        {
            selectedButton = "Buy";
            selectionItemOpened = false;
            planet = commercePlanet;
            LoadCommerceButtons();
            LoadInstrumentsButtons();
        }

        static void LoadCommerceButtons()
        {
            foreach (Button b in commerceButtons)
            {
                if (b.Type == selectedButton)
                    b.LoadTexture(buttonClickedTexture);
                else
                    b.LoadTexture(buttonTexture);
            }
        }

        static void LoadInstrumentsButtons()
        {
            List<Product> products = new List<Product>();
            switch (selectedButton)
            {
                case "Buy":
                    products = planet.PlanetSettlement.Products;
                    buyButtons = CreateInstrumentsButtons(products);
                    break;
                case "Sell":
                    products = PlayerShip.Products;
                    sellButtons = CreateInstrumentsButtons(products);
                    break;
                case "PowerUps":
                    List<PlayerPowerUp> pups = GameCommerceManager.GetPowerUpList();
                    powerButtons = CreatePowerUpButtons(pups);
                    break;
            }
        }

        static List<CommerceButton<Product>> CreateInstrumentsButtons(List<Product> products)
        {
            List<CommerceButton<Product>> buttons = new List<CommerceButton<Product>>();
            int x = relativeScreen.X + 10;
            int y = relativeScreen.Y + 40;
            for(int i = 0; i < products.Count; i++)
            {
                Texture2D btTexture;
                string productType;
                int prizePerUnit = 0;
                if(selectedButton =="Buy")
                    prizePerUnit = GameCommerceManager.GetProductBuyPrize(products[i]);
                else
                    prizePerUnit = GameCommerceManager.GetProductSellPrize(products[i]);
                Tuple<Texture2D, string> tuple = ReturnProductTupleFromType(products[i].Type);
                btTexture = tuple.Item1;
                productType = tuple.Item2;
                string[] strings = { productType, "Livello " + products[i].Level, prizePerUnit + " al pezzo", "Quantità: " + products[i].Quantity};
                Vector2 position = new Vector2(relativeScreen.X + relativeScreen.Width / 5 * (i % 4), relativeScreen.Y + relativeScreen.Height / 5 * (i / 4) + 60);
                buttons.Add(new CommerceButton<Product>(btTexture, inGameButton, font, strings, position, products[i]));
            }
            return buttons;
        }

        static List<CommerceButton<PlayerPowerUp>> CreatePowerUpButtons(List<PlayerPowerUp> powerUps)
        {
            List<CommerceButton<PlayerPowerUp>> buttons = new List<CommerceButton<PlayerPowerUp>>();

            int x = relativeScreen.X + 10;
            int y = relativeScreen.Y + 40;
            for (int i = 0; i < powerUps.Count; i++)
            {
                Texture2D btTexture;
                string productType;
                int prizePerUnit = GameCommerceManager.GetPowerUpPrize(powerUps[i]);
                Tuple<Texture2D, string> tuple = ReturnPowerUpTupleFromType(powerUps[i].Type);
                btTexture = tuple.Item1;
                productType = tuple.Item2;
                string[] strings = { productType, "Livello " + powerUps[i].Level, "Prezzo " + prizePerUnit };
                Vector2 position = new Vector2(relativeScreen.X + relativeScreen.Width / 5 * (i % 4), relativeScreen.Y + relativeScreen.Height / 5 * (i / 4) + 60);
                buttons.Add(new CommerceButton<PlayerPowerUp>(btTexture, inGameButton, font, strings, position, powerUps[i]));
                int relative = relativeScreen.Width / 5;
                //position.X = relativeScreen.X + relative * (i % 4 + 1) - buttons[i].Position.Width / 2;
                //buttons[i].DrawingPoint = new Vector2((int)position.X, (int)position.Y);
            }

            return buttons;
        }

        static public string ButtonClicked()
        {
            string button = "NONE";
            //Se è aperta la finestra di selezione controllane i pulsanti e torna indietro.
            if (selectionItemOpened)
            {
                string s = selectionBox.CheckButtons();
                if (s == "CancelButton")
                {
                    selectionItemOpened = false;
                    UncheckButtons();
                }
                else if(s == "OkButton")
                {
                    selectionItemOpened = false;
                    BuyOrSellItem();
                    UncheckButtons();
                }
                return button;
            }
            if (GameSystemDesigner.ActualPlanet.PlanetSettlement != null)
            {
                foreach (Button b in commerceButtons)
                {
                    if (b.WasPressed())
                    {
                        button = b.Type;
                        selectedButton = b.Type;
                        LoadInstrumentsButtons();
                    }
                }
            }
            if (closeButton.WasPressed())
                button = closeButton.Type;
            LoadCommerceButtons();
            CheckButtons();
            return button;
        }

        static public void CheckButtons()
        {
            if (selectedButton == "Buy")
                foreach (CommerceButton<Product> b in buyButtons)
                {
                    if (b.WasPressed())
                    {
                        Tuple<Texture2D, string> tuple = ReturnProductTupleFromType(b.ButtonComponent.Type);
                        Texture2D txt = tuple.Item1;
                        int prize = GameCommerceManager.GetProductBuyPrize(b.ButtonComponent);
                        int maxBuy = PlayerShip.Money / prize;
                        int max = MathHelper.Min(b.ButtonComponent.Quantity, maxBuy);
                        selectionBox.SetValues(max, prize, txt);
                        selectionItemOpened = true;
                    }
                }
            else if (selectedButton == "Sell")
                foreach (CommerceButton<Product> b in sellButtons)
                {
                    if (b.WasPressed())
                    {
                        Tuple<Texture2D, string> tuple = ReturnProductTupleFromType(b.ButtonComponent.Type);
                        Texture2D txt = tuple.Item1;
                        int prize = GameCommerceManager.GetProductSellPrize(b.ButtonComponent);
                        int maxBuy = planet.PlanetSettlement.Money / prize;
                        int max = MathHelper.Min(b.ButtonComponent.Quantity, maxBuy);
                        selectionBox.SetValues(max, prize, txt);
                        selectionItemOpened = true;
                    }
                }
            else if(selectedButton == "PowerUps")
                foreach (CommerceButton<PlayerPowerUp> b in powerButtons)
                {
                    if (b.WasPressed())
                    {
                        Tuple<Texture2D, string> tuple = ReturnPowerUpTupleFromType(b.ButtonComponent.Type);
                        Texture2D txt = tuple.Item1;
                        int prize = GameCommerceManager.GetPowerUpPrize(b.ButtonComponent);
                        int maxBuy = 1;
                        selectionBox.SetValues(maxBuy, prize, txt);
                        selectionItemOpened = true;
                    }
                }
        }

        static void BuyOrSellItem()
        {
            int prize = 0;
            if (selectedButton == "Buy")
            {
                foreach (CommerceButton<Product> b in buyButtons)
                    if (b.IsSelected)
                    {
                        prize = selectionBox.TotalPrice;
                        int quantity = selectionBox.Value;
                        PlayerShip.Money -= prize;
                        PlayerShip.Products.Add(new Product(b.ButtonComponent.Type, quantity, b.ButtonComponent.Level));
                        foreach (Product p in planet.PlanetSettlement.Products)
                        {
                            if (p == b.ButtonComponent)
                                p.Quantity -= quantity;
                        }
                        LoadInstrumentsButtons();
                        break;
                    }
            }
            else if (selectedButton == "Sell")
            {
                foreach (CommerceButton<Product> b in sellButtons)
                    if (b.IsSelected)
                    {
                        prize = selectionBox.TotalPrice;
                        int quantity = selectionBox.Value;
                        PlayerShip.Money += prize;
                        planet.PlanetSettlement.Products.Add(new Product(b.ButtonComponent.Type, quantity, b.ButtonComponent.Level));
                        foreach (Product p in PlayerShip.Products)
                        {
                            if (p == b.ButtonComponent)
                                p.Quantity -= quantity;
                        }
                        LoadInstrumentsButtons();
                        break;
                    }
            }
            else if (selectedButton == "PowerUps")
            {
                foreach (CommerceButton<PlayerPowerUp> b in powerButtons)
                    if (b.IsSelected)
                    {
                        prize = selectionBox.TotalPrice;
                        if (PlayerShip.Money >= prize && PlayerShip.AddNewPowerUp(b.ButtonComponent))
                        {
                            PlayerShip.Money -= prize;
                            PlayerShip.PowerUps.Add(new PlayerPowerUp(b.ButtonComponent.Type, b.ButtonComponent.Level));
                            LoadInstrumentsButtons();
                        }
                        break;
                    }
            }
            CleanLists(PlayerShip.Products);
            CleanLists(planet.PlanetSettlement.Products);

            bool playerSell = false;
            if (selectedButton == "Sell")
                playerSell = true;

            if (GameManager.PlayerEmpire != GameManager.ActualSystemOwner)
                GameRelationManager.CreateCommerceEvent(GameManager.PlayerEmpire, GameManager.ActualSystemOwner, prize, playerSell);
        }

        static void UncheckButtons()
        {
            foreach (CommerceButton<Product> b in buyButtons)
                b.IsSelected = false;
            foreach (CommerceButton<Product> b in sellButtons)
                b.IsSelected = false;
            foreach (CommerceButton<PlayerPowerUp> b in powerButtons)
                b.IsSelected = false;
        }

        static void CleanLists(List<Product> products)
        {
            if(products.Count == 1 && products[0].Quantity == 0)
            {
                products.RemoveAt(0);
                return;
            }
            for(int i = products.Count - 1; i > 0; i--)
            {
                if(products[i].Quantity == 0)
                {
                    products.RemoveAt(i);
                    continue;
                }
                for (int j = i - 1; j >= 0; j--)
                    if (products[i].Type == products[j].Type && products[i].Level == products[j].Level)
                    {
                        products[j].Quantity += products[i].Quantity;
                        products.RemoveAt(i);
                        break;
                    }
            }
        }
        
        static Tuple<Texture2D, string> ReturnProductTupleFromType(Product.ProductType type)
        {
            string productType = "";
            Texture2D txt = foodIcon;
            switch (type)
            {
                case Product.ProductType.FOOD:
                    productType = "Cibo";
                    txt = foodIcon;
                    break;
                case Product.ProductType.TECNO:
                    productType = "Tecnologia";
                    txt = tecnoIcon;
                    break;
                case Product.ProductType.TOOL:
                default:
                    productType = "Strumenti";
                    txt = toolIcon;
                    break;
            }
            return new Tuple<Texture2D, string>(txt, productType);
        }

        static Tuple<Texture2D, string> ReturnPowerUpTupleFromType(PlayerPowerUp.PowerUpType type)
        {
            Texture2D txt = foodIcon;
            string key;
            string name;
            switch (type)
            {
                case PlayerPowerUp.PowerUpType.CARGO:
                    key = "CargoIcon";
                    name = "Aumenta Carico";
                    break;
                case PlayerPowerUp.PowerUpType.ENGINE:
                    key = "EngineIcon";
                    name = "Motore Interstellare";
                    break;
                case PlayerPowerUp.PowerUpType.LASER:
                    key = "LaserIcon";
                    name = "Raggio Laser";
                    break;
                case PlayerPowerUp.PowerUpType.ROCKET:
                    key = "RocketIcon";
                    name = "Razzi spaziali";
                    break;
                case PlayerPowerUp.PowerUpType.SHIELD:
                    key = "ShieldIcon";
                    name = "Scudo";
                    break;
                case PlayerPowerUp.PowerUpType.TERRAFORMER:
                default:
                    key = "TerraformIcon";
                    name = "Terraformatore";
                    break;
            }
            powerUpTextures.TryGetValue(key, out txt);
            return new Tuple<Texture2D, string>(txt, name);
        }

        static public void Draw()
        {
            //spriteBatch.Begin();

            Vector2 position = new Vector2(100, 100);
            spriteBatch.Draw(rectangle, position, Color.White);

            spriteBatch.DrawString(font, planet.Name, new Vector2(GraphicSettings.CenterScreen.X - font.MeasureString(planet.Name).X / 2, 110), Color.Red);
            spriteBatch.DrawString(font, "Impero " + GameManager.ActualSystemOwner.EmpireName, new Vector2(GraphicSettings.CenterScreen.X - font.MeasureString("Impero " + GameManager.ActualSystemOwner.EmpireName).X / 2, 140), Color.Black);
            closeButton.DrawButton(spriteBatch);

            foreach (Button b in commerceButtons)
                b.DrawButton(spriteBatch);

            switch(selectedButton)
            {
                case "Buy":
                    foreach (CommerceButton<Product> b in buyButtons)
                        b.DrawButton(spriteBatch);
                    break;
                case "Sell":
                    foreach (CommerceButton<Product> b in sellButtons)
                        b.DrawButton(spriteBatch);
                    break;
                case "PowerUps":
                    foreach (CommerceButton<PlayerPowerUp> b in powerButtons)
                        b.DrawButton(spriteBatch);
                    break;
            }

            if (selectionItemOpened)
            {
                spriteBatch.Draw(mistedScreen, new Vector2(0,0), Color.White);
                selectionBox.DrawItem(spriteBatch);
            }
            //spriteBatch.End();
        }
    }
}
