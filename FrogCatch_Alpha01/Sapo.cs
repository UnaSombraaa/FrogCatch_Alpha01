using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.Threading;

namespace FrogCatch_Alpha01
{
    public class Sapo
    {
 
        private Texture2D[] estadosSapo;
        private Texture2D[] lenguas;
        private SoundEffect Lenguetazo;

        private int frameActualSapo;
        private float alturaLengua;
        
        private float velocidadSubida = 1500f;
        private float velocidadBajada = 750f;
        
        private bool disparando;
        private bool cargando;
        private bool subiendo;

        
        
        private float tiempoPresionado;
        private Vector2 posicionLengua;
        private float alturaMaxima;
        private Vector2 posicionColision;

        public Sapo(Texture2D sapoBase, Texture2D sapoDisparoBajo, Texture2D sapoDisparoMedio, Texture2D sapoDisparoAlto, Texture2D[] lenguas, SoundEffect Lenguetazo)
        {
            estadosSapo = new Texture2D[] { sapoBase, sapoDisparoBajo, sapoDisparoMedio, sapoDisparoAlto};

            this.lenguas = lenguas;
            this.Lenguetazo = Lenguetazo;
            alturaLengua = 0;
            disparando = false;
            cargando = false;
            subiendo = false;
            tiempoPresionado = 0;
            posicionLengua = new Vector2(280, 380);
            posicionColision = new Vector2(360, 430);
            alturaMaxima = 0;
        }

        public void Update(GameTime gameTime, KeyboardState keyboardState)
        {
            if (keyboardState.IsKeyDown(Keys.Up))
            {
                cargando = true;
                tiempoPresionado += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            }
            else
            {
                if (cargando && tiempoPresionado > 0)
                {
                    if (tiempoPresionado >= 1000)
                        alturaMaxima = 450;
                    else if (tiempoPresionado >= 500)
                        alturaMaxima = 400;
                    else if (tiempoPresionado >= 200)
                        alturaMaxima = 200;
                    else
                        alturaMaxima = 0;

                    cargando = false;
                    subiendo = true;
                    tiempoPresionado = 0;

                    Lenguetazo.Play();
                }
            }

            if (subiendo)
            {
                alturaLengua += velocidadSubida * (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (alturaLengua >= alturaMaxima)
                {
                    alturaLengua = alturaMaxima;
                    subiendo = false;
                    disparando = true;
                }
            }

            if (disparando)
            {
                alturaLengua -= velocidadBajada * (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (alturaLengua <= 0)
                {
                    alturaLengua = 0;
                    disparando = false;
                }
            }

            if (keyboardState.IsKeyDown(Keys.Up))
            {
                if (tiempoPresionado < 200)
                    frameActualSapo = 0;
                else if (tiempoPresionado < 500)
                    frameActualSapo = 1;
                else if (tiempoPresionado < 1000)
                    frameActualSapo = 2;
                else
                    frameActualSapo = 3;
            }
            else
            {
                frameActualSapo = 0;
            }
        }

        public Rectangle AreaColisionLengua()
        {
            int lenguaWidth = 30;
            int lenguaHeight = (int)(alturaLengua * 0.86f);
            return new Rectangle((int)posicionColision.X, (int)(posicionColision.Y - lenguaHeight), lenguaWidth, lenguaHeight);
        }

        // Método para verificar si un mosquito fue atrapado
        public bool ColisionMosquito(Mosquito mosquito)
        {
            for (int i = 0; i < mosquito.Posiciones.Length; i++)
            {
                if (mosquito.IsMosquitoAtrapado(i)) continue; // Ignora si ya está atrapado
                    
                Rectangle mosquitoRect = new Rectangle((int)mosquito.Posiciones[i].X, (int)mosquito.Posiciones[i].Y, 50, 50);
                if (mosquitoRect.Intersects(AreaColisionLengua()))
                {
                    mosquito.SetMosquitoAtrapado(i); // Actualiza el estado del mosquito
                    return true; // Retorna que se atrapó un mosquito
                }
            }
            return false;
        }

        public void Draw(SpriteBatch spriteBatch, int screenWidth, int screenHeight)
        {
            int lenguaFrame = alturaLengua >= 450 ? 0 : alturaLengua >= 300 ? 1 : alturaLengua >= 150 ? 2 : 3;
            int lenguaWidth = 200;
            int lenguaHeight = (int)(alturaLengua * 1f);
            
            
            spriteBatch.Draw(lenguas[lenguaFrame], new Rectangle((int)posicionLengua.X, (int)(posicionLengua.Y - lenguaHeight), lenguaWidth, lenguaHeight), Color.White);
            spriteBatch.Draw(estadosSapo[frameActualSapo], new Rectangle(250, 230, 250, 250), Color.White);

            //Testeo de Hitbox
            //Rectangle areaColision = AreaColisionLengua();
            //Texture2D blanco = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
            //blanco.SetData(new[] { Color.Red });
            //spriteBatch.Draw(blanco, areaColision, Color.Blue);
        }
    }
}