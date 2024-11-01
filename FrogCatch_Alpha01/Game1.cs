using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace FrogCatch_Alpha01
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private Mosquito mosquito;
        private Sapo sapo;
        private Texture2D fondoAnimado;
        private SpriteFont fuente;

        private int totalFramesFondo = 4;
        private int frameActualFondo = 0;
        private double tiempoTranscurridoFondo = 0;
        private double tiempoPorFrameFondo = 100;

        private int score = 0; // Agrega un campo para el score

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            fuente = Content.Load<SpriteFont>("PixelFont"); // Asegúrate de que la fuente esté cargada
            fondoAnimado = Content.Load<Texture2D>("FONDOREAL");

            Texture2D bichoAlto = Content.Load<Texture2D>("bichos1Alto");
            Texture2D mosquitoMedio = Content.Load<Texture2D>("MosquitoMedio");
            Texture2D bichoBajo = Content.Load<Texture2D>("bichos1Bajo");

            mosquito = new Mosquito(bichoAlto, mosquitoMedio, bichoBajo, _graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight, 600);

            Texture2D sapoBase = Content.Load<Texture2D>("RANA MODIF");
            Texture2D sapoMedio = Content.Load<Texture2D>("RANA MODIF2");
            Texture2D sapoDisparo = Content.Load<Texture2D>("RANA MODIFDisparo");
            Texture2D sapoDisparando = Content.Load<Texture2D>("RANA MODIFAA");

            Texture2D[] lenguas = new Texture2D[4];
            lenguas[0] = Content.Load<Texture2D>("Lengua sapo");
            lenguas[1] = Content.Load<Texture2D>("Lengua sapo2");
            lenguas[2] = Content.Load<Texture2D>("Lengua sapo3");
            lenguas[3] = Content.Load<Texture2D>("Lengua sapo4");

            sapo = new Sapo(sapoBase, sapoMedio, sapoDisparo, sapoDisparando, lenguas);


        }

        protected override void Update(GameTime gameTime)
        {
            mosquito.Update(gameTime, sapo.GetAreaColisionLengua());

            KeyboardState keyboardState = Keyboard.GetState();
            sapo.Update(gameTime, keyboardState);

            tiempoTranscurridoFondo += gameTime.ElapsedGameTime.TotalMilliseconds;
            if (tiempoTranscurridoFondo >= tiempoPorFrameFondo)
            {
                frameActualFondo = (frameActualFondo + 1) % totalFramesFondo;
                tiempoTranscurridoFondo = 0;
            }

            // Incrementa el score si se atrapa un mosquito
            for (int i = 0; i < mosquito.Posiciones.Length; i++)
            {
                if (mosquito.IsMosquitoAtrapado(i))
                {
                    score++; // Incrementa el score
                    Console.WriteLine($"Score: {score}"); // Imprime el score para verificar
                    mosquito.GenerateNewMosquito(i, _graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight);
                }
            }

            if (keyboardState.IsKeyDown(Keys.Escape))
                Exit();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            _spriteBatch.Begin();

            Rectangle rectOrigenFondo = new Rectangle(frameActualFondo * fondoAnimado.Width / totalFramesFondo, 0, fondoAnimado.Width / totalFramesFondo, fondoAnimado.Height);
            _spriteBatch.Draw(fondoAnimado, new Vector2(0, 0), rectOrigenFondo, Color.White);
            _spriteBatch.DrawString(fuente, $"Score: {score}", new Vector2(10, 10), Color.Orange); // Muestra el score
            mosquito.Draw(_spriteBatch);
            sapo.Draw(_spriteBatch);

            _spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
