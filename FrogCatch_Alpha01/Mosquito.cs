using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace FrogCatch_Alpha01
{
    public class Mosquito
    {
        private Texture2D[] estadosMosquito;
        private Vector2[] posiciones;
        private bool[] atrapados; // Para saber si un mosquito ha sido atrapado
        private int frameActual;
        private double tiempoTranscurrido;
        private double tiempoPorFrame = 100;
        private float velocidad = 2f;
        private int cantidadMosquitos = 5; // Cantidad total de mosquitos
        private Random random = new Random(); // Mueve la instancia de Random aquí
        private int limiteY; // Límite en el eje Y para la aparición de mosquitos

        // Añadimos un parámetro para establecer el límite en Y
        public Mosquito(Texture2D BichoAlto, Texture2D MosquitoMedio, Texture2D Bicho1Bajo, int ancho, int alto, int limiteY)
        {
            estadosMosquito = new Texture2D[] { BichoAlto, MosquitoMedio, Bicho1Bajo };
            posiciones = new Vector2[cantidadMosquitos];
            atrapados = new bool[cantidadMosquitos]; // Inicializa el array de atrapados
            this.limiteY = limiteY; // Guardamos el límite en Y
            SpawnMosquitos(ancho, alto);
            frameActual = 0;
            tiempoTranscurrido = 0;
        }

        public Vector2[] Posiciones => posiciones; // Agrega esta línea

        // Método para verificar si un mosquito está atrapado
        public bool IsMosquitoAtrapado(int index)
        {
            return atrapados[index];
        }

        private void SpawnMosquitos(int ancho, int alto)
        {
            for (int i = 0; i < cantidadMosquitos; i++)
            {
                posiciones[i] = new Vector2(0, random.Next(0, limiteY)); // Aparecen en la parte izquierda
                atrapados[i] = false; // Inicializa todos los mosquitos como no atrapados
            }
        }

        public void Update(GameTime gameTime, Rectangle lenguaRect)
        {
            tiempoTranscurrido += gameTime.ElapsedGameTime.TotalMilliseconds;
            if (tiempoTranscurrido >= tiempoPorFrame)
            {
                frameActual = (frameActual + 1) % estadosMosquito.Length;
                tiempoTranscurrido = 0;
            }

            for (int i = 0; i < posiciones.Length; i++)
            {
                // Solo actualiza la posición si el mosquito no ha sido atrapado
                if (!atrapados[i])
                {
                    posiciones[i].X += velocidad;

                    // Si el mosquito se sale de la pantalla, vuelve al inicio
                    if (posiciones[i].X > 800)
                    {
                        GenerateNewMosquito(i, 800, 600);
                    }

                    // Chequea si el mosquito colisiona con la lengua
                    Rectangle mosquitoRect = new Rectangle((int)posiciones[i].X, (int)posiciones[i].Y, 50, 50);
                    if (mosquitoRect.Intersects(lenguaRect))
                    {
                        atrapados[i] = true; // Marca el mosquito como atrapado
                                             // Genera un nuevo mosquito en una posición aleatoria
                        GenerateNewMosquito(i, 800, 600);
                        return; // Sale del ciclo una vez que captura un mosquito
                    }
                }
            }
        }

        // Cambiar el modificador de acceso a public
        public void GenerateNewMosquito(int index, int ancho, int alto)
        {
            posiciones[index] = new Vector2(0, random.Next(0, limiteY)); // Aparecen en la parte izquierda
            atrapados[index] = false; // Reinicia el estado del mosquito
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            for (int i = 0; i < posiciones.Length; i++)
            {
                if (!atrapados[i]) // Solo dibuja los mosquitos que no han sido atrapados
                {
                    spriteBatch.Draw(estadosMosquito[frameActual], new Rectangle((int)posiciones[i].X, (int)posiciones[i].Y, 50, 50), Color.White);
                }
            }
        }
    }
}
