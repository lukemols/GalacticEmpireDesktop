using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GalacticEmpire
{
    class Relation
    {
        public enum RelationStatus { WAR, ANGRY, NEUTRAL, HAPPY, ALLY }

        string empireName;
        public string EmpireName { get { return empireName; } }

        int relationPoints;
        public int RelationPoints { get { return relationPoints; } }

        List<Event> relationEvents;
        public List<Event> RelationEvents { get { return relationEvents; } }

        public Relation(string empire, List<Event> firstEvents)
        {
            relationPoints = 0;
            empireName = empire;
            relationEvents = firstEvents;
            foreach (Event e in relationEvents)
                relationPoints += e.EventPoints;
        }

        public RelationStatus GetStatus()
        {
            if (relationPoints < -150)
                return RelationStatus.WAR;
            else if (relationPoints < -25)
                return RelationStatus.ANGRY;
            else if (relationPoints < 25)
                return RelationStatus.NEUTRAL;
            else if (relationPoints < 150)
                return RelationStatus.HAPPY;
            else //if (relationPoints > 150)
                return RelationStatus.ALLY;
        }
    }
}
