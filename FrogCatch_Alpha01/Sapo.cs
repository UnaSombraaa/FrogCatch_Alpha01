using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace FrogCatch_Alpha01
{
    public class Sapo
    {
        private Texture2D[] estadosSapo; // sapoBase, sapoMedio, sapoDisparo, sapoDisparando
        private Texture2D[] lenguas; // lenguas para diferentes estados
        private int frameActualSapo;
        private float alturaLengua;
        private float velocidadSubida = 2500f;
        private float velocidadBajada = 1000f;
        private bool disparando;
        private bool cargando;
        private bool subiendo;
        private float tiempoPresionado;
        private Vector2 posicionLengua;
        private float alturaMaxima; // Asegúrate de que esta variable esté definida

        // Variable para la posición de colisión
        private Vector2 posicionColision;

        public Sapo(Texture2D sapoBase, Texture2D sapoMedio, Texture2D sapoDisparo, Texture2D sapoDisparando, Texture2D[] lenguas)
        {
            estadosSapo = new Texture2D[] { sapoBase, sapoMedio, sapoDisparo, sapoDisparando };
            this.lenguas = lenguas;
            alturaLengua = 0;
            disparando = false;
            cargando = false;
            subiendo = false;
            tiempoPresionado = 0;
            posicionLengua = new Vector2(275, 380);
            posicionColision = new Vector2(350, 380); // Inicializa la posición de colisión
            alturaMaxima = 0; // Inicializa altura máxima
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
                    if (tiempoPresionado >= 750)
                        alturaMaxima = 450;
                    else if (tiempoPresionado >= 500)
                        alturaMaxima = 350;
                    else if (tiempoPresionado >= 200)
                        alturaMaxima = 250;
                    else
                        alturaMaxima = 0;

                    cargando = false;
                    subiendo = true;
                    tiempoPresionado = 0;
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

        public Rectangle GetAreaColisionLengua()
        {
            int lenguaWidth = 100;
            int lenguaHeight = (int)(alturaLengua * 0.8f);
            return new Rectangle((int)posicionColision.X, (int)(posicionColision.Y - lenguaHeight), lenguaWidth, lenguaHeight);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            int lenguaFrame = alturaLengua >= 400 ? 0 : alturaLengua >= 300 ? 1 : alturaLengua >= 150 ? 2 : 3;
            int lenguaWidth = 250;
            int lenguaHeight = (int)(alturaLengua * 0.8f);

            spriteBatch.Draw(lenguas[lenguaFrame], new Rectangle((int)posicionLengua.X, (int)(posicionLengua.Y - lenguaHeight), lenguaWidth, lenguaHeight), Color.White);
            spriteBatch.Draw(estadosSapo[frameActualSapo], new Rectangle(250, 200, 300, 300), Color.White);

            // Dibuja el rectángulo de colisión de la lengua en un color semi-transparente
            Rectangle areaColision = GetAreaColisionLengua();
            Texture2D blanco = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
            blanco.SetData(new[] { Color.White });
            spriteBatch.Draw(blanco, areaColision, new Color(255, 0, 0, 100)); // Color rojo con transparencia
        }
    }
}
