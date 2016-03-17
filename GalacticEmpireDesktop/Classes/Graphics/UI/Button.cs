using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;

namespace GalacticEmpire
{
    /// <summary>
    /// Classe Button che implementa un pulsante e controlla i relativi click su di esso
    /// </summary>
    class Button
    {
        //Texture del pulsante
        Texture2D buttonTexture;
        public Texture2D ButtonTexture { get { return buttonTexture; } }
        //Font, dichiarato statico, valido per tutti. Rimuovere se si vuole usare un font diverso per i pulsanti
        static SpriteFont font;

        Rectangle position;
        // Posizione del pulsante
        public Rectangle Position { get { return position; } set { if (GraphicSettings.ScreenBounds.Contains(value)) { position = value; SetTextPosition(); } } }
        // Posizione del testo
        Vector2 textPosition;
        // Colore del testo
        Color textColor;

        string text;
        // Testo del pulsante
        public string Text { get { return text; } set { if (value != null) { text = value; SetTextPosition(); } } }

        string type;
        // Tipo del pulsante, serve per riconoscerlo tra gli altri
        public string Type { get { return type; } }
        // Se true, il pulsante ha il testo
        bool withText;
        // Se true, viene consentito il tocco continuo, altrimenti saranno considerati solo i tocchi differenti
        bool continueTouch;
        // Se true viene mostrato a schermo
        bool isVisible;
        public bool IsVisible { get { return isVisible; } set { isVisible = value; } }
        // Id dell'ultimo tocco
        int lastTouchId;
        Vector2 lastTouchPosition;
        // Posizione dell'ultimo tocco - Utile se il pulsante è un pad
        public Vector2 LastTouchPosition { get { return lastTouchPosition; } }
        // Se true, il pulsante è premuto
        bool isPressed;

        /// <summary>
        /// Costruttore di un pulsante con testo
        /// </summary>
        /// <param name="startPosition">Posizione iniziale</param>
        /// <param name="buttonText">Testo del pulsante</param>
        /// <param name="buttonType">Tipo o nome del pulsante</param>
        public Button(Rectangle startPosition, string buttonText, string buttonType)
        {
            position = startPosition;
            text = buttonText;
            type = buttonType;
            withText = true;
            isVisible = true;
            continueTouch = false;
            textColor = Color.White;
        }
        /// <summary>
        /// Costruttore di un pulsante senza testo
        /// </summary>
        /// <param name="startPosition">Posizione iniziale del pulsante</param>
        /// <param name="buttonType">Tipo o nome del pulsante</param>
        /// <param name="continueTouch">Indica se si vuole il tocco continuo, false di default (Mettere TRUE per PAD)</param>
        public Button(Rectangle startPosition, string buttonType, bool continueTouch = false)
        {
            position = startPosition;
            type = buttonType;
            isVisible = true;
            withText = false;
            this.continueTouch = continueTouch;
        }
        /// <summary>
        /// Carica la texture del pulsante
        /// </summary>
        /// <param name="txt">Texture da dare al pulsante</param>
        public void LoadTexture(Texture2D txt)
        {
            buttonTexture = txt;
        }
        /// <summary>
        /// Carica texture e font per il pulsante
        /// </summary>
        /// <param name="txt">Texture da dare al pulsante</param>
        /// <param name="spriteFont">Font da dare al testo del pulsante</param>
        public void LoadTextureAndFont(Texture2D txt, SpriteFont spriteFont)
        {
            withText = true;
            buttonTexture = txt;
            font = spriteFont;
        }
        /// <summary>
        /// Metodo che posiziona il testo al centro del pulsante
        /// </summary>
        public void SetTextPosition()
        {
            if (text == null)
                return;
            Vector2 size = font.MeasureString(text);
            size = size * 0.5f;
            textPosition.X = position.X + position.Width / 2 - size.X;
            textPosition.Y = position.Y + position.Height / 2 - size.Y;
        }
        /// <summary>
        /// Metodo che seleziona il colore del testo del pulsante
        /// </summary>
        /// <param name="textColor">Colore da dare</param>
        public void SetTextColor(Color textColor)
        {
            this.textColor = textColor;
        }
        /// <summary>
        /// Metodo che disegna il pulsante
        /// </summary>
        /// <param name="spriteBatch">Spritebatch</param>
        public void DrawButton(SpriteBatch spriteBatch)
        {
            if (!isVisible)
                return;
            spriteBatch.Draw(buttonTexture, position, Color.White);
            if (withText)
                spriteBatch.DrawString(font, text, textPosition, textColor);
        }
        /// <summary>
        /// Metodo che ritorna se il pulsante è stato premuto oppure no
        /// </summary>
        /// <returns>True se è premuto, false se no</returns>
        public bool WasTouched()
        {
            // Se il pulsante non è visibile, ritorna false
            if (!isVisible)
                return false;

            //Ottieni i touch sullo schermo
            TouchPanelCapabilities tpanel = TouchPanel.GetCapabilities();
            TouchCollection touches;
            //Se non esiste il touch screen ritorna false
            if (tpanel.IsConnected)
                touches = TouchPanel.GetState();
            else
                return false;
            //Per ogni tocco
            foreach (var touch in touches)
            {
                // Se ha lo stesso id dell'ultimo tocco e non c'è il tocco continuo non contarlo
                if (touch.Id == lastTouchId && !continueTouch)
                    continue;
                // Se è stato rilasciato non contarlo
                if (touch.State == TouchLocationState.Released)
                    continue;
                // Se ha premuto nel rettangolo della texture allora c'è il tocco
                if (position.Contains(touch.Position))
                {
                    isPressed = true;
                    lastTouchId = touch.Id;
                    lastTouchPosition = touch.Position;
                    return isPressed;
                }
            }
            //Se non è avvenuto nessun tocco ritorna false
            isPressed = false;
            return isPressed;
        }
        /// <summary>
        /// Metodo che controlla se è avvenuto il click sul pulsante
        /// </summary>
        /// <returns>True se è premuto, false se no</returns>
        public bool WasClicked()
        {
            //Ottieni lo stato del pulsante
            MouseState ms = Mouse.GetState();
            isPressed = false;
            //Se il tasto sinistro è premuto torna true
            if (position.Contains(ms.Position) && ms.LeftButton == ButtonState.Pressed)
                isPressed = true;
            return isPressed;
        }
        /// <summary>
        /// Metodo che controlla se il pulsante è stato premuto, indifferentemente dall'uso di mouse o touch
        /// </summary>
        /// <returns>True se è premuto, false se no</returns>
        public bool WasPressed()
        {
            return WasTouched() || WasClicked();
        }

    }
}