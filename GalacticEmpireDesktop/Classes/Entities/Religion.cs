using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GalacticEmpire
{
    static class Religion
    {
        public enum ReligionType { ATEO, ATOM, BLESS, CURSER}

        private static Random rnd = new Random();

        static int[,] ReligionArray = new int[,]
        {// ATEO    ATOM    BLESS   CURSER
            { 1,    -1,     0,      -2  },//ATEO
            {-1,     2,     1,       0  },//ATOM
            { 1,     0,     2,      -2  },//BLESS
            {-2,    -1,    -1,       2  }//CURSER
        };

        public static ReligionType GetRandomReligion()
        {
            switch(rnd.Next(0, 4))
            {
                case 0:
                    return ReligionType.ATEO;
                case 1:
                    return ReligionType.ATOM;
                case 2:
                    return ReligionType.BLESS;
                case 3:
                    return ReligionType.CURSER;
                default:
                    return ReligionType.ATEO;
            }
        }

        /// <summary>
        /// Metodo che restituisce il moltiplicatore per la religione con l'impero del giocatore.
        /// Se empireToPlayer è true o non indicato, ritorna il moltiplicatore di come viene visto il giocatore dall'impero indicato,
        /// altrimenti ritorna come il giocatore vede l'impero.
        /// </summary>
        /// <param name="religion">Religione dell'impero indicato</param>
        /// <param name="empireToPlayer">Indica se si restituisce il valore della relazione da Impero a Giocatore (true) o viceversa (false)</param>
        /// <returns>Moltiplicatore dovuto alla religione</returns>
        static public int ReligionIndex(ReligionType religion, bool empireToPlayer = true)
        {
            if(empireToPlayer)
                return ReligionIndex(religion, GameManager.PlayerEmpire.EmpireReligion);
            else
                return ReligionIndex(GameManager.PlayerEmpire.EmpireReligion, religion);
        }
        /// <summary>
        ///  Metodo che restituisce il moltiplicatore per la religione nella relazione IMPERO 1 -> IMPERO 2
        /// </summary>
        /// <param name="religion1">Religione dell'impero 1</param>
        /// <param name="religion2">Religione dell'impero 2</param>
        /// <returns>Moltiplicatore dovuto alla religione</returns>
        static public int ReligionIndex(ReligionType religion1, ReligionType religion2)
        {
            int x = (int)religion1;
            int y = (int)religion2;
            return ReligionArray[x,y];
        }
    }
}
