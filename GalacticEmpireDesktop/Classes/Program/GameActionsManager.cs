using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GalacticEmpire
{
    static class GameActionsManager
    {

        public static int TerraformPlanet(Planet planet, int money, int scienceLevel)
        {
            //Prendiamo il livello di scienza e troviamo il costo per alzare lo score come 512 / log del livello(+1 x evitare problemi)
            double lg = Math.Log(scienceLevel + 1);
            int costPerUnit = (int)(512 / lg);
            int units = money / costPerUnit; //trova di quanto si può alzare il terrascore al max a seconda dei soldi disponibili
            int ts = 32 - planet.Terrascore; //punteggio terrascore mancante al massimo
            if (units > ts) //Se è possibile superare il max x la disponibilità economica, allora raggiungi il max
                units = ts;
            planet.Terrascore += units; //Aggiungi le unità di terrascore

            return units * costPerUnit; //ritorna il costo totale
        }
    }
}
