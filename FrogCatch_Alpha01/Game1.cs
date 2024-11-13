using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using Microsoft.Xna.Framework.Media;

using System;

using System.IO;

namespace FrogCatch_Alpha01
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private Menu _menu;
        private bool _menuActivo = true;
        private Song cancion;
        private SoundEffect Lenguetazo;

        private Texture2D Marco;

        private Mosquito mosquito;
        private Sapo sapo;

        private Texture2D fondoD;
        private Texture2D fondoN;

        private SpriteFont fuente;
        private SpriteFont fuenteGrande;

        // Variables para movimiento de texto
        private Vector2 posicionTexto;
        private float velocidadTexto = 4f;
        private bool moverDerecha = true;


        private int totalFramesFondo = 4;
        private int frameActualFondo = 0;
        private double tiempoTranscurridoFondo = 0;
        private double tiempoPorFrameFondo = 100;
        private float alphaTransicion = 0.1f;
        private bool esDeDia = true;
        private double temporizadorFondo = 0;
        private double tiempoCambioFondo = 10;

        private int score = 0;
        private int mosquitosCapturados = 0; // Contador de mosquitos
        private double gameTimer = 60;

        private bool GameOver = false;
        private bool JuegoEmpezado = false;
        private bool pantallaCompleta = false;
        private bool teclaF11Presionada = false;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
           
        
        }
    
        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            cancion = Content.Load<Song>("Musica_Efectos/Fondo");
            MediaPlayer.IsRepeating = true;
            MediaPlayer.Volume = 0.4f;
            MediaPlayer.Play(cancion);

            
            Lenguetazo = Content.Load<SoundEffect>("Musica_Efectos/lenguetazo");
            
            // Cargar el menú
            _menu = new Menu(GraphicsDevice, Content);

            // Cargar recursos del juego
            fuente = Content.Load<SpriteFont>("Fuente/Fuente");
            fuenteGrande = Content.Load<SpriteFont>("Fuente/FuenteGrande");
            fondoD = Content.Load<Texture2D>("Fondos/FondoD");
            fondoN = Content.Load<Texture2D>("Fondos/FondoN");
            Marco = Content.Load<Texture2D>("Sprites/Marco");

            Texture2D mosquitoArriba = Content.Load<Texture2D>("Sprites/SpriteM1");
            Texture2D mosquitoMedio = Content.Load<Texture2D>("Sprites/SpriteM2");
            Texture2D mosquitoBajo = Content.Load<Texture2D>("Sprites/SpriteM3");

            mosquito = new Mosquito(mosquitoArriba, mosquitoMedio, mosquitoBajo, _graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight, 300);

            Texture2D sapoBase = Content.Load<Texture2D>("Sprites/SpriteS1");
            Texture2D sapoDisparoBajo = Content.Load<Texture2D>("Sprites/SpriteS2");
            Texture2D sapoDisparoMedio = Content.Load<Texture2D>("Sprites/SpriteS4");
            Texture2D sapoDisparoAlto = Content.Load<Texture2D>("Sprites/SpriteS3");

            Texture2D[] lenguas = new Texture2D[4];
            lenguas[0] = Content.Load<Texture2D>("Sprites/Lengua sapo");
            lenguas[1] = Content.Load<Texture2D>("Sprites/Lengua sapo2");
            lenguas[2] = Content.Load<Texture2D>("Sprites/Lengua sapo3");
            lenguas[3] = Content.Load<Texture2D>("Sprites/Lengua sapo4");

            sapo = new Sapo(sapoBase, sapoDisparoBajo, sapoDisparoMedio, sapoDisparoAlto, lenguas, Lenguetazo);

            posicionTexto = new Vector2(100, 200);

        }

        protected override void Update(GameTime gameTime)
        {
            if (_menuActivo)
            {
                if (_menu.Update(gameTime))
                {
                    _menuActivo = false;
                    JuegoEmpezado = true;
                }
                return; // Evita actualizar la lógica del juego mientras el menú está activo
            }

            if (GameOver)
            {
                // Movimiento de texto de vaivén cuando el juego termina
                if (moverDerecha)
                {
                    posicionTexto.X += velocidadTexto;
                    if (posicionTexto.X >= _graphics.PreferredBackBufferWidth - fuenteGrande.MeasureString("Presiona Enter para Reiniciar").X)
                    {
                        moverDerecha = false;
                    }
                }
                else
                {
                    posicionTexto.X -= velocidadTexto;
                    if (posicionTexto.X <= 10) // Ajuste para que no se pegue a la izquierda
                    {
                        moverDerecha = true;
                    }

                }

                if (Keyboard.GetState().IsKeyDown(Keys.Enter))
                {
                    ResetGame();
                }

                return; // Termina la actualización si el juego ha terminado
            }

            // Lógica del juego si no es Game Over
            gameTimer -= gameTime.ElapsedGameTime.TotalSeconds;
            if (gameTimer <= 0)
            {
                GameOver = true;
                SaveScore();
                return; 
            }

            mosquito.Update(gameTime, sapo.AreaColisionLengua(), ref gameTimer, ref score);

            KeyboardState keyboardState = Keyboard.GetState();
            sapo.Update(gameTime, keyboardState);

            tiempoTranscurridoFondo += gameTime.ElapsedGameTime.TotalMilliseconds;
            if (tiempoTranscurridoFondo >= tiempoPorFrameFondo)
            {
                frameActualFondo = (frameActualFondo + 1) % totalFramesFondo;
                tiempoTranscurridoFondo = 0;

                alphaTransicion -= 0.01f;
                if (alphaTransicion <= 0)
                {
                    alphaTransicion = 1f;
                    esDeDia = !esDeDia;
                }
            }

            for (int i = 0; i < mosquito.Posiciones.Length; i++)
            {
                if (mosquito.IsMosquitoAtrapado(i))
                {
                    mosquito.SetMosquitoAtrapado(i);
                    score += 10;
                    mosquitosCapturados++;
                    mosquito.GenerateNewMosquito(i, _graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight);
                }
            }
        }


        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            _spriteBatch.Begin();

            if (_menuActivo)
            {
                _menu.Draw(gameTime);
            }
            else
            {
                if (esDeDia)
                {
                    _spriteBatch.Draw(fondoD, new Rectangle(0, 0, _graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight),
                        new Rectangle(frameActualFondo * (fondoD.Width / totalFramesFondo), 0, fondoD.Width / totalFramesFondo, fondoD.Height),
                        Color.White * (1 - alphaTransicion));

                    _spriteBatch.Draw(fondoN, new Rectangle(0, 0, _graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight),
                        new Rectangle(frameActualFondo * (fondoN.Width / totalFramesFondo), 0, fondoN.Width / totalFramesFondo, fondoN.Height),
                        Color.White * alphaTransicion);
                }
                else
                {
                    _spriteBatch.Draw(fondoN, new Rectangle(0, 0, _graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight),
                        new Rectangle(frameActualFondo * (fondoN.Width / totalFramesFondo), 0, fondoN.Width / totalFramesFondo, fondoN.Height),
                        Color.White * (1 - alphaTransicion));

                    _spriteBatch.Draw(fondoD, new Rectangle(0, 0, _graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight),
                        new Rectangle(frameActualFondo * (fondoD.Width / totalFramesFondo), 0, fondoD.Width / totalFramesFondo, fondoD.Height),
                        Color.White * alphaTransicion);
                }

                if (JuegoEmpezado && !GameOver)
                {
                    _spriteBatch.DrawString(fuente, $"Puntuacion: {score}", new Vector2(10, 10), Color.Yellow);
                    _spriteBatch.DrawString(fuente, $"Tiempo: {Math.Max(0, (int)gameTimer)}s", new Vector2(10, 30), Color.Yellow);
                    _spriteBatch.DrawString(fuente, $"Mosquitos Capturados: {mosquitosCapturados}", new Vector2(580, 10), Color.Yellow);

                    mosquito.Draw(_spriteBatch);
                    sapo.Draw(_spriteBatch, _graphics.PreferredBackBufferWidth / 2, _graphics.PreferredBackBufferHeight);
                }
                else
                {
                    string message = GameOver ? "Se termino el tiempo! Enter para Reiniciar" : "Presiona Enter para Empezar";
                    _spriteBatch.DrawString(fuenteGrande, message, posicionTexto, Color.Yellow);
                    if (GameOver) DrawScores();
                }
            }

            _spriteBatch.End();
            base.Draw(gameTime);
        }

        private void SaveScore()
        {
            File.AppendAllText("scores.txt", $"{score}\n");
        }

        private void DrawScores()
        {
            string[] scores = File.ReadAllLines("scores.txt");
            int y = 0;
            foreach (string score in scores)
            {
                _spriteBatch.DrawString(fuente, $"Tu puntuacion: {score}", new Vector2(0, y), Color.Yellow);
                y += 20;
            }
        }
        
        //resetea el juego (Su nombre lo indica)
        private void ResetGame()
        {
            score = 0;
            mosquitosCapturados = 0;
            gameTimer = 60;
            GameOver = false;
            
        }
        
        //borra los puntajes al cerrar la aplicacion
        protected override void OnExiting(object sender, EventArgs args)
        {
            base.OnExiting(sender, args);
            if (File.Exists("scores.txt"))
            {
                File.Delete("scores.txt");
            }
        }
    }
}
