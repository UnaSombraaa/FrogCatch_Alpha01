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

        private Texture2D botonTuto;
        private Texture2D pixel;

        private Texture2D Marco;

        private Mosquito mosquito;
        private Sapo sapo;

        private Texture2D fondoD;
        private Texture2D fondoN;

        private SpriteFont fuente;
        private SpriteFont fuenteGrande;

        // Variables para movimiento de texto
        private Vector2 posicionTexto;
        private float velocidadTexto = 3f;
        private bool moverDerecha = true;
        // Curva del texto del restart
        private float tiempoOnda = 0f;
        private float amplitudOnda = 40f; 
        private float frecuenciaOnda = 1f;


        private int totalFramesFondo = 4;
        private int frameActualFondo = 0;
        private double tiempoTranscurridoFondo = 0;
        private double tiempoPorFrameFondo = 100;
        private float alphaTransicion = 0.1f;
        private bool esDeDia = true;
        private double temporizadorFondo = 0;
        private double tiempoCambioFondo = 30;
       
        private int fotogramasBoton = 4; // Cuántos fotogramas tiene el botón (puedes ajustarlo)
        private int frameActualBoton = 0;
        private double tiempoTranscurridoBoton = 0;
        private double tiempoPorFrameBoton = 200; // Tiempo entre cada fotograma del botón
        private bool botonVisible = true;  // Visibilidad del botón (por defecto visible)
        private int contadorTeclasArriba = 0; // Contador de teclas hacia arriba


        

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

            // Cargar boton
            botonTuto = Content.Load<Texture2D>("Menu/botonTuto");

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
            pixel = new Texture2D(GraphicsDevice, 1, 1);
            pixel.SetData(new Color[] { Color.Black });

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
                // Incrementa el tiempo para la onda
                tiempoOnda += (float)gameTime.ElapsedGameTime.TotalSeconds * frecuenciaOnda;

                // Movimiento de texto de vaivén cuando el juego termina
                if (moverDerecha)
                {
                    posicionTexto.X += velocidadTexto;
                    float maxPosX = 600 - fuenteGrande.MeasureString("Presiona Enter para Reiniciar").X;
                    if (posicionTexto.X >= maxPosX)
                    {
                        posicionTexto.X = maxPosX; // Ajuste para no pasarse
                        moverDerecha = false;
                    }
                }
                else
                {
                    posicionTexto.X -= velocidadTexto;
                    if (posicionTexto.X <= 10) // Ajuste para no pegarse demasiado a la izquierda
                    {
                        posicionTexto.X = 10; // Asegura que no se vaya fuera del borde
                        moverDerecha = true;
                    }
                }

                // Aplicar el efecto de onda en la posición Y
                posicionTexto.Y = 200 + (float)Math.Sin(tiempoOnda) * amplitudOnda;

                // Limitar la posición del texto en X
                float maxPosXLimitado = 600 - fuenteGrande.MeasureString("Presiona Enter para Reiniciar").X;
                posicionTexto.X = Math.Clamp(posicionTexto.X, 10, maxPosXLimitado); 

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

            // Lógica de actualización para el botón
            tiempoTranscurridoBoton += gameTime.ElapsedGameTime.TotalMilliseconds;
            if (tiempoTranscurridoBoton >= tiempoPorFrameBoton)
            {
                // Cambia el fotograma actual
                frameActualBoton = (frameActualBoton + 1) % fotogramasBoton;
                tiempoTranscurridoBoton = 0;
            }


            // Lógica para el botón y el contador de teclas hacia arriba
            if (keyboardState.IsKeyDown(Keys.Up))
            {
                contadorTeclasArriba++;  // Aumenta el contador al presionar la tecla hacia arriba
            }

            if (contadorTeclasArriba >= 3)
            {
                botonVisible = false;  // Oculta el botón cuando el contador llega a 3
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
                // Primero dibujamos los fondos
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

                // Luego dibujamos el botón si es visible
                if (botonVisible)
                {
                    // Calcula el rectángulo que representa el fotograma actual en la sprite sheet
                    int frameWidth = 64; // Ancho del fotograma (64px)
                    int frameHeight = 64; // Alto del fotograma (64px)

                    // Establece el rectángulo de la sprite sheet que se debe dibujar
                    Rectangle sourceRectangle = new Rectangle(frameActualBoton * frameWidth, 0, frameWidth, frameHeight);

                    // Establece la posición en pantalla donde se dibujará el botón
                    Rectangle destinationRectangle = new Rectangle(500, 250, 200, 200);
                   
                    Color colortransparente = new Color(0, 0, 0, 128);
                
                    Rectangle rectanguloNegro = new Rectangle(490, 150, 310, 250);
                    _spriteBatch.Draw(pixel, rectanguloNegro, colortransparente);
                    _spriteBatch.Draw(botonTuto, destinationRectangle, sourceRectangle, Color.Yellow);
                    _spriteBatch.DrawString(fuenteGrande, "TUTORIAL", new Vector2(500, 150), Color.Yellow);
                    _spriteBatch.DrawString(fuente, "Presiona la flecha hacia arriba", new Vector2(500, 200), Color.Yellow);
                    _spriteBatch.DrawString(fuente, "Para cargar la lengua", new Vector2(500, 220), Color.Yellow);
                    _spriteBatch.DrawString(fuente, "Y Sueltala para disparar", new Vector2(500, 240), Color.Yellow);
                    _spriteBatch.DrawString(fuente, "Atento a las mejillas del sapo", new Vector2(500, 260), Color.Yellow);
                    _spriteBatch.DrawString(fuente, "Indican que tan lejos puedes llegar", new Vector2(500, 280), Color.Yellow);

                }

                // Después, dibujamos la información del juego (puntuación, tiempo, etc.)
                if (JuegoEmpezado && !GameOver)
                {
                    _spriteBatch.DrawString(fuente, $"Puntuacion: {score}", new Vector2(10, 10), Color.Orange);
                    _spriteBatch.DrawString(fuente, $"Tiempo: {Math.Max(0, (int)gameTimer)}s", new Vector2(10, 30), Color.Orange);
                    _spriteBatch.DrawString(fuente, $"Mosquitos Capturados: {mosquitosCapturados}", new Vector2(580, 10), Color.Orange);

                    mosquito.Draw(_spriteBatch);
                    sapo.Draw(_spriteBatch, _graphics.PreferredBackBufferWidth / 2, _graphics.PreferredBackBufferHeight);
                }
                else
                {
                    string message = GameOver ? "Se termino el tiempo! Enter para Reiniciar" : "null";
                    _spriteBatch.DrawString(fuenteGrande, message, posicionTexto, Color.Orange);
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
                _spriteBatch.DrawString(fuente, $"Puntuacion Dev: 4500", new Vector2(600, 0), Color.Orange);
                _spriteBatch.DrawString(fuente, $"Tu puntuacion: {score}", new Vector2(0, y), Color.Orange);
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
