using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace FrogCatch_Alpha01
{
    public class Menu
    {
        private Texture2D fondoMenu;
        private Texture2D botonPlay;
        private Texture2D titulo;
        private Rectangle botonPlayRect;
        private SpriteBatch spriteBatch;

        public Menu(GraphicsDevice graphicsDevice, ContentManager content)
        {
            spriteBatch = new SpriteBatch(graphicsDevice);

            fondoMenu = content.Load<Texture2D>("Fondos/FondoM");
            botonPlay = content.Load<Texture2D>("Menu/BOTONPLAY");
            titulo = content.Load<Texture2D>("Menu/FrogCATCHHHHH");

            // Definir la posición del botón
            botonPlayRect = new Rectangle(300, 200, 200, 200);
        }

        public bool Update(GameTime gameTime)
        {
            MouseState mouseState = Mouse.GetState();
            if (mouseState.LeftButton == ButtonState.Pressed && botonPlayRect.Contains(mouseState.Position))
            {
                return true; // Indica que el botón de "Play" fue presionado
            }

            return false;
        }

        public void Draw(GameTime gameTime)
        {
            spriteBatch.Begin();
            spriteBatch.Draw(fondoMenu, new Rectangle(0, 0, 800, 600), Color.White); // Ajusta según tu resolución
            spriteBatch.Draw(titulo, new Rectangle(200,-50,400, 400), Color.White);
            spriteBatch.Draw(botonPlay, botonPlayRect, Color.White);
            spriteBatch.End();
        }
    }
}
