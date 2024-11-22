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

        //Tutorial
        private Texture2D botonTuto;
        private Texture2D botonTuto2;
        private Texture2D barraTuto;
        private Texture2D pixel;

        
        private Mosquito mosquito;
        private Sapo sapo;

        private Texture2D fondoD;
        private Texture2D fondoN;

        private SpriteFont fuente;
        private SpriteFont fuenteGrande;
        private SpriteFont fuenteChica;

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
       
        private int fotogramasBoton = 4; // Cuántos fotogramas tiene el botón 
        private int frameActualBoton = 0;
        private double tiempoTranscurridoBoton = 0;
        private double tiempoPorFrameBoton = 300;

        private int fotogramasBoton2 = 4; // Cuántos fotogramas tiene el botón 
        private int frameActualBoton2 = 0;
        private double tiempoTranscurridoBoton2 = 0;
        private double tiempoPorFrameBoton2 = 300;
        

        private bool botonVisible = true;  // Visibilidad del botón 
        private int contadorTeclasArriba = 0; // Contador para desaparicion de tutorial Hoja-1
        private int contadorTeclasDerecha = 0;
        private int contadorTeclasIzquierda = 0;

        private int fotogramasBarra = 5; 
        private int frameActualBarra = 0;
        private double tiempoTranscurridoBarra = 0;
        private double tiempoPorFrameBarra = 400; 



        private int score = 0;
        private int mosquitosCapturados = 0; // Contador de mosquitos
        private double gameTimer = 60;

        private bool GameOver = false;
        private bool JuegoEmpezado = false;
        private bool pantallaCompleta = false;
        private bool teclaF11Presionada = false;

        // Pagina Tutorial
        private int paginaTutorial = 1;
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
            // Cargar boton
            botonTuto2 = Content.Load<Texture2D>("Menu/botonTuto2");

            // Cargar barra

            barraTuto = Content.Load<Texture2D>("Menu/barraTuto");


            // Cargar recursos del juego
            fuente = Content.Load<SpriteFont>("Fuente/Fuente");
            fuenteGrande = Content.Load<SpriteFont>("Fuente/FuenteGrande");
            fuenteChica = Content.Load<SpriteFont>("Fuente/FuenteChica");
            fondoD = Content.Load<Texture2D>("Fondos/FondoD");
            fondoN = Content.Load<Texture2D>("Fondos/FondoN");
         

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

            tiempoTranscurridoBoton2 += gameTime.ElapsedGameTime.TotalMilliseconds;
            if (tiempoTranscurridoBoton2 >= tiempoPorFrameBoton2)
            {
                // Cambia el fotograma actual
                frameActualBoton2 = (frameActualBoton2 + 1) % fotogramasBoton2;
                tiempoTranscurridoBoton2 = 0;
            }


            // Lógica de actualización para el botón
            tiempoTranscurridoBarra += gameTime.ElapsedGameTime.TotalMilliseconds;
            if (tiempoTranscurridoBarra >= tiempoPorFrameBarra)
            {
                // Cambia el fotograma actual
                frameActualBarra = (frameActualBarra + 1) % fotogramasBarra;
                tiempoTranscurridoBarra = 0;
            }

            // Lógica para el botón y el contador de teclas hacia arriba
            if (keyboardState.IsKeyDown(Keys.Up))
            {
                contadorTeclasArriba++;  // Aumenta el contador al presionar la tecla hacia arriba
            }

            if (keyboardState.IsKeyDown(Keys.Right) && contadorTeclasDerecha == 0)
            {
                // Cambiar a la siguiente página
                paginaTutorial = 2;
                contadorTeclasDerecha++;
                contadorTeclasIzquierda = 0;// Evita que se cambie constantemente
            }
            if (keyboardState.IsKeyDown(Keys.Left) && contadorTeclasIzquierda== 0)
            {
                paginaTutorial = 1;
                contadorTeclasIzquierda++;
                contadorTeclasDerecha = 0;
              
            }
            
            
            if (contadorTeclasArriba >= 2)
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

                // Luego dibujamos el botón si es visible
                if (botonVisible)
                {
                    // Calcula el rectángulo que representa el fotograma actual en la sprite sheet
                    int anchoBoton = 128; // Ancho del fotograma (64px)
                    int altoBoton = 128; // Alto del fotograma (64px)
                    
                    int anchoBoton2 = 128;
                    int altoBoton2 = 128;

                    int anchoBarra = 64;
                    int altoBarra = 64;

                    // Establece el rectángulo de la sprite sheet que se debe dibujar
                    Rectangle rectanguloBoton = new Rectangle(frameActualBoton * anchoBoton, 0, anchoBoton, altoBoton);
                    
                    Rectangle rectanguloBoton2 = new Rectangle(frameActualBoton2 * anchoBoton2, 0, anchoBoton2, altoBoton2);
                    
                    Rectangle rectanguloBarra = new Rectangle(frameActualBarra * anchoBarra, 0, anchoBarra, altoBarra);

                    // Establece la posición en pantalla donde se dibujará el botón
                    Rectangle posicionBoton = new Rectangle(580, 300, 100, 100);
                    
                    Rectangle posicionBoton2 = new Rectangle(650, 280, 150, 150);

                    Rectangle posicionBarra = new Rectangle(500, 310, 100, 100);

                    Color colortransparente = new Color(0, 0, 0, 128);
                    Rectangle rectanguloNegro = new Rectangle(490, 150, 310, 250);
                    _spriteBatch.Draw(pixel, rectanguloNegro, colortransparente);
                    _spriteBatch.Draw(botonTuto, posicionBoton, rectanguloBoton, Color.Yellow);
                    _spriteBatch.Draw(botonTuto2, posicionBoton2, rectanguloBoton2, Color.Yellow);

                    _spriteBatch.Draw(barraTuto, posicionBarra, rectanguloBarra, Color.WhiteSmoke);

                    if (paginaTutorial == 1)
                    {
                        // Página 1 del tutorial
                        _spriteBatch.DrawString(fuenteGrande, "TUTORIAL         1/2", new Vector2(500, 150), Color.Yellow);
                        _spriteBatch.DrawString(fuente, "Presiona la flecha hacia arriba", new Vector2(500, 200), Color.Yellow);
                        _spriteBatch.DrawString(fuente, "Para cargar la lengua", new Vector2(500, 220), Color.Yellow);
                        _spriteBatch.DrawString(fuente, "Y Sueltala para disparar", new Vector2(500, 240), Color.Yellow);
                        _spriteBatch.DrawString(fuente, "Atento a las mejillas del sapo", new Vector2(500, 260), Color.Yellow);
                        _spriteBatch.DrawString(fuente, "Indican que tan lejos puedes llegar", new Vector2(500, 280), Color.Yellow);
                        _spriteBatch.DrawString(fuenteChica, "Presiona la flecha derecha para ir a la siguiente pagina", new Vector2(500, 380), Color.Yellow);
                    }
                    else if (paginaTutorial == 2)
                    {
                        // Página 2 del tutorial
                        _spriteBatch.DrawString(fuenteGrande, "TUTORIAL         2/2", new Vector2(500, 150), Color.Yellow);
                        _spriteBatch.DrawString(fuente, "Existen 2 tipos de mosquitos", new Vector2(500, 200), Color.Yellow);
                        _spriteBatch.DrawString(fuente, "Los ", new Vector2(500, 220), Color.Yellow);
                        _spriteBatch.DrawString(fuente, "rojos", new Vector2(500 + fuente.MeasureString("Los ").X, 220), Color.Red);
                        _spriteBatch.DrawString(fuente, ", quitan puntos y tiempo", new Vector2(500 + fuente.MeasureString("Los rojos").X, 220), Color.Yellow);
                        _spriteBatch.DrawString(fuente, "Los ", new Vector2(500, 240), Color.Yellow);
                        _spriteBatch.DrawString(fuente, "verdes", new Vector2(500 + fuente.MeasureString("Los ").X, 240), Color.LightGreen);
                        _spriteBatch.DrawString(fuente, ", suman puntos y tiempo", new Vector2(500 + fuente.MeasureString("Los verdes").X, 240), Color.Yellow);
                        _spriteBatch.DrawString(fuente, "Consigue la mayor puntuacion", new Vector2(500, 280), Color.Yellow);
                        _spriteBatch.DrawString(fuenteChica, "Presiona la flecha Izquierda para ir a la pagina anterior", new Vector2(500, 380), Color.Yellow);


                    }
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
