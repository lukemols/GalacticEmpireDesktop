using System;
using Microsoft.Xna.Framework;

namespace GalacticEmpire
{
    class MathOperations
    {
        /// <summary>
        /// Metodo che calcola l'angolo tra due Vector3 sul piano xz
        /// </summary>
        /// <param name="start">Vettore di partenza (o posizione attuale)</param>
        /// <param name="stop">Vettore di arrivo (o posizione bersaglio)</param>
        /// <returns>Angolo in radianti compreso tra 0 e 2pi</returns>
        public static float AngleBetweenVectors(Vector3 start, Vector3 stop)
        {
            Vector3 diff = Vector3.Subtract(stop, start);
            diff.Normalize();
            
            float asin = (float)Math.Asin(diff.Z);
            float acos = (float)Math.Acos(diff.X);

            float angle = 0.0f;

            if (diff.X >= 0 && diff.Z >= 0)
                angle = asin;
            else if (diff.X < 0 && diff.Z >= 0)
                angle = acos;
            else if (diff.X < 0 && diff.Z < 0)
                angle = -1 * acos;
            else if (diff.X >= 0 && diff.Z < 0)
                angle = asin;

            if (angle < 0)
                angle += MathHelper.Pi * 2;

            return angle;
        }
    }
}
