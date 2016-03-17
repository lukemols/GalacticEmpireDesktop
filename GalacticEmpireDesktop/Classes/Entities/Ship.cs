using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace GalacticEmpire
{
    class Ship
    {
        Vector3 position;
        public Vector3 Position { get { return position; } }

        Vector3 targetPosition;
        public Vector3 TargetPosition { get { return targetPosition; } }

        int timeToReachPosition;
        public int TimeToReachPosition { get { return timeToReachPosition; } }

        int life;
        public int Life { get { return life; } }

        int maxLife;
        public int MaxLife { get { return maxLife; } }

        int speed;
        public int Speed { get { return speed; } }

        public Ship(Vector3 initialPosition, int life = 500, int speed = 5, int maxLife = 500)
            : this(initialPosition, initialPosition, life, speed, maxLife)
        { }

        public Ship(Vector3 initialPosition, Vector3 targetPosition, int life = 500, int speed = 5, int maxLife = 500)
        {
            position = initialPosition;
            this.targetPosition = targetPosition;
            this.life = life;
            this.maxLife = maxLife;
            this.speed = speed;
        }

        public void SetTarget(Vector3 target)
        {
            targetPosition = target;
            float d = Vector3.Distance(position, target);
            timeToReachPosition = (int)d / speed;
        }

        public void Update()
        {
            if(targetPosition != position)
            {
                float alpha = MathOperations.AngleBetweenVectors(position, targetPosition);
                position.X += speed * (float)Math.Cos(alpha);
                position.Z += speed * (float)Math.Sin(alpha);
                timeToReachPosition--;
            }
            if (Vector3.Distance(position, targetPosition) < 5)
                targetPosition = position;
        }
    }
}
