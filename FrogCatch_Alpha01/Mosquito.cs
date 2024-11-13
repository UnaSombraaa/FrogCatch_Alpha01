using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace FrogCatch_Alpha01
{
    public class Mosquito
    {
        private Texture2D[] estadosMosquito;
        private Vector2[] posiciones;
        private bool[] atrapados; // Para saber si un mosquito fue atrapado
        private int frameActual;
        private double tiempoTranscurrido;
        private double tiempoPorFrame = 100;
        private float velocidad = 3f;
        private int cantidadMosquitos = 10; // Cantidad total de mosquitos
        private Random random = new Random();
        private int limiteY;
        private int distanciaMinimaEntreMosquitos = 200; // Nueva variable para ajustar la separación

        // Nuevas propiedades para efectos de color y tiempo
        private Color[] coloresMosquito;
        public bool[] agregarTiempo;
        public bool[] quitarTiempo;

        public Mosquito(Texture2D MosquitoArriba, Texture2D MosquitoMedio, Texture2D MosquitoBajo, int ancho, int alto, int limiteY)
        {
            estadosMosquito = new Texture2D[] { MosquitoArriba, MosquitoMedio, MosquitoBajo };
            posiciones = new Vector2[cantidadMosquitos];
            atrapados = new bool[cantidadMosquitos];
            coloresMosquito = new Color[cantidadMosquitos];
            agregarTiempo = new bool[cantidadMosquitos];
            quitarTiempo = new bool[cantidadMosquitos];
            this.limiteY = limiteY; // Guardamos el límite en Y
            SpawnMosquitos(ancho, alto);
            frameActual = 0;
            tiempoTranscurrido = 0;
        }

        public Vector2[] Posiciones => posiciones;

        // Método para verificar si un mosquito está atrapado
        public bool IsMosquitoAtrapado(int index)
        {
            return atrapados[index];
        }
        private void SpawnMosquitos(int ancho, int alto)
{
        int ultimoY = -distanciaMinimaEntreMosquitos; // Inicializa para asegurar la primera posición
            for (int i = 0; i < cantidadMosquitos; i++)
                {
        int nuevoY;
            do
                {
                    nuevoY = random.Next(0, limiteY);
                } 
             while (Math.Abs(nuevoY - ultimoY) < distanciaMinimaEntreMosquitos); // Verifica la distancia mínima

         int nuevoX = random.Next(0, ancho / 4); 

        posiciones[i] = new Vector2(nuevoX, nuevoY);
        ultimoY = nuevoY;
        atrapados[i] = false;
        MosquitosEspeciales(i);  // Asigna un color y efecto a cada mosquito
    }
}

        // Método para asignar color y efectos (agregar o quitar tiempo)
        private void MosquitosEspeciales(int index)
        {
            double probabilidadVerde = 0.20; // 20% de probabilidad
            double probabilidadRojo = 0.20;  // 20% de probabilidad

            double randVal = random.NextDouble();

            if (randVal < probabilidadVerde)
            {
                coloresMosquito[index] = Color.Green;  // Cambia el color a verde
                agregarTiempo[index] = true; // Este mosquito agrega tiempo
                quitarTiempo[index] = false;
            }
            else if (randVal < probabilidadVerde + probabilidadRojo)
            {
                coloresMosquito[index] = Color.Red;    // Cambia el color a rojo
                agregarTiempo[index] = false;
                quitarTiempo[index] = true; // Este mosquito quita tiempo
            }
            else
            {
                coloresMosquito[index] = Color.White;  // Color normal
                agregarTiempo[index] = false;
                quitarTiempo[index] = false; // No tiene efectos especiales
            }
        }

        public void Update(GameTime gameTime, Rectangle lenguaRect, ref double tiempoJuego, ref int score)
        {
            tiempoTranscurrido += gameTime.ElapsedGameTime.TotalMilliseconds;
            if (tiempoTranscurrido >= tiempoPorFrame)
            {
                frameActual = (frameActual + 1) % estadosMosquito.Length;
                tiempoTranscurrido = 0;
            }

            for (int i = 0; i < posiciones.Length; i++)
            {
                // Solo actualiza la posición si el mosquito no esta atrapado
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

                        // Verifica si el mosquito agrega o quita tiempo
                        if (agregarTiempo[i])
                        {
                            tiempoJuego += 2; // Aumenta 2 segundos al tiempo del juego
                            
                        }
                        else if (quitarTiempo[i])
                        {
                            tiempoJuego -= 3; // Disminuye 3 segundos al tiempo del juego
                            score -= 15; //resta 5 puntos (Los mosquitos por default dan 10)
                        }
                    }
                }
            }
        }

        public void SetMosquitoAtrapado(int index)
        {
            atrapados[index] = true;
        }

        public void GenerateNewMosquito(int index, int ancho, int alto)
        {
            int nuevoY;
            int ultimoY = (int)posiciones[index].Y;
            do
            {
                nuevoY = random.Next(0, limiteY);
            } while (Math.Abs(nuevoY - ultimoY) < distanciaMinimaEntreMosquitos); // Verifica la distancia mínima

            posiciones[index] = new Vector2(0, nuevoY); // Aparecen en la parte izquierda
            atrapados[index] = false; // Reinicia el estado del mosquito
            MosquitosEspeciales(index);  // Reasigna el color y efectos al nuevo mosquito
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            for (int i = 0; i < posiciones.Length; i++)
            {
                if (!atrapados[i]) // Solo dibuja los mosquitos que no estan atrapados
                {
                    spriteBatch.Draw(estadosMosquito[frameActual], new Rectangle((int)posiciones[i].X, (int)posiciones[i].Y, 50, 50), coloresMosquito[i]);
                }
            }
        }
    }
}