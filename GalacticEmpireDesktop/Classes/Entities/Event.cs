using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GalacticEmpire
{
    class Event
    {
        public enum EventType { POSITIVE, NEGATIVE, NEUTRAL}

        EventType type;
        public EventType Type { get { return type; } }

        public enum EventMotivation { COMMERCE, WAR, RELIGION, TECNO, SCIENCE, GIFT }

        EventMotivation motivation;
        public EventMotivation Motivation { get { return motivation; } }

        int eventPoints;
        public int EventPoints { get { return eventPoints; } }

        public Event(EventType type, EventMotivation motive, int points)
        {
            this.type = type;
            motivation = motive;
            eventPoints = points;
        }
    }
}
