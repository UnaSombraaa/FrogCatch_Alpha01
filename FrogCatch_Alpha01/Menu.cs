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
        private Texture2D botonTuto;
        private Rectangle botonPlayRect;
        private SpriteBatch spriteBatch;
        private float alpha; // Para la opacidad de la transición
        private bool iniciandoTransicion;
        private KeyboardState estadoTecla;
        // Para indicar si la transición está ocurriendo

        public Menu(GraphicsDevice graphicsDevice, ContentManager content)
        {
            spriteBatch = new SpriteBatch(graphicsDevice);

            fondoMenu = content.Load<Texture2D>("Fondos/FondoM");
            botonPlay = content.Load<Texture2D>("Menu/BOTONPLAY");
            titulo = content.Load<Texture2D>("Menu/FrogCATCHHHHH");



            // Definir la posición del botón
            botonPlayRect = new Rectangle(300, 200, 190, 200);

            alpha = 1.0f; // Comienza completamente opaco
            iniciandoTransicion = false; // No está en transición al inicio
        }

        public bool Update(GameTime gameTime)
        {
            KeyboardState keyboardState = Keyboard.GetState();

            // Comienza la transición si se presiona la tecla "Space"
            if (keyboardState.IsKeyDown(Keys.Space) && !estadoTecla.IsKeyDown(Keys.Space))
            {
                iniciandoTransicion = true;
            }

            // Si alpha disminuye da el efecto de desvanecimiento
            if (iniciandoTransicion)
            {
                alpha -= 0.05f;

                if (alpha <= 0)
                {
                    alpha = 0;
                    return true; // Termina la transición, inicia el juego
                }
            }

            // Guardamos el estado anterior del teclado para detectar el primer pulso
            estadoTecla = keyboardState;

            return false;
        }

        public void Draw(GameTime gameTime)
        {
            spriteBatch.Begin();
            spriteBatch.Draw(fondoMenu, new Rectangle(0, 0, 800, 600), Color.White);
            spriteBatch.Draw(titulo, new Rectangle(200, -100, 400, 400), Color.White);
            spriteBatch.Draw(botonPlay, botonPlayRect, Color.White);

            // Dibuja una superposición negra con alpha variable para crear el efecto de desvanecimiento
            spriteBatch.Draw(fondoMenu, new Rectangle(0, 0, 800, 600), Color.Black * (1 - alpha));
            spriteBatch.End();
        }
    }
}

