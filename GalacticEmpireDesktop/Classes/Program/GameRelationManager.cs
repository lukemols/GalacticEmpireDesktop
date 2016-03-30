using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GalacticEmpire
{
    static class GameRelationManager
    {
        static public void CreateGiftEvent(Empire eGifter, Empire eReceiver, int gift)
        {
            Relation relation = null;
            foreach(Relation r in eReceiver.EmpireRelations)
            {
                if (r.EmpireName == eGifter.EmpireName)
                    relation = r;
            }
            if (relation != null)
            {
                int i = gift / 512;
                if (relation.RelationPoints < 0)
                    i = (int)(i * 1.5);
                else
                    i = (int)(i * 2.5);

                if (i > 50)
                    i = 50;
                else if (i < 1)
                    i = 1;
                //Cerca se nella relazione c'è già un evento sul regalo e aggiungi punti
                foreach (Event e in relation.RelationEvents)
                {
                    if (e.Motivation == Event.EventMotivation.GIFT && e.Type == Event.EventType.POSITIVE)
                    {
                        e.AddPointsToEvent(i);
                        return;
                    }
                }
                //Altrimenti creane uno nuovo
                relation.RelationEvents.Add(new Event(Event.EventType.POSITIVE, Event.EventMotivation.GIFT, i));
            }
        }

        static public void CreateCommerceEvent(Empire player, Empire commerced, int prize, bool playerSell)
        {
            Relation relation = null;
            foreach (Relation r in commerced.EmpireRelations)
            {
                if (r.EmpireName == player.EmpireName)
                    relation = r;
            }
            if (relation != null)
            {
                int i = prize / 1024;
                if (relation.RelationPoints < 0)
                    i = (int)(i * 1.5);
                else
                    i = (int)(i * 2.5);

                if (playerSell)
                    i = (int)(i * 0.88);
                else
                    i = (int)(i * 1.5);

                if (i > 50)
                    i = 50;
                else if (i < 1)
                    i = 1;
                //Cerca se nella relazione c'è già un evento sul commercio e aggiungi punti
                foreach(Event e in relation.RelationEvents)
                {
                    if (e.Motivation == Event.EventMotivation.COMMERCE && e.Type == Event.EventType.POSITIVE)
                    {
                        e.AddPointsToEvent(i);
                        return;
                    }
                }
                //Altrimenti creane uno nuovo
                relation.RelationEvents.Add(new Event(Event.EventType.POSITIVE, Event.EventMotivation.COMMERCE, i));
            }
        }
    }
}
