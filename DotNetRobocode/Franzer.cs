using Robocode;
using System;

namespace Franzer
{
    public class Franzer : TeamRobot
    {
        private int _shotsFired = 0;
        private RobotStatus _robotStatus;

        public override void Run()
        {

            TurnLeft(Heading - 90);
            TurnGunRight(90);

            while (true)
            {
                var random = new Random();
                if (random.Next() % 2 == 0)
                {
                    Back(500 * random.NextDouble());
                }
                else
                {
                    Ahead(500 * random.NextDouble());
                }

                if (random.Next() % 2 == 0)
                {
                    TurnLeft(45 * random.NextDouble());
                }
                else
                {
                    TurnRight(45 * random.NextDouble());
                }
                TurnRadarLeft(90);
                TurnGunLeft(random.NextDouble() * random.NextDouble() + 20);

            }
        }

        public override void OnStatus(StatusEvent e)
        {
            _robotStatus = e.Status;
        }

        public override void OnScannedRobot(ScannedRobotEvent e)
        {
            if (IsTeammate(e.Name))
            {
                return;
            }

            if (_shotsFired < 3)
            {

                double angleToEnemy = e.Bearing;
                // Calculate the angle to the scanned robot
                double angle = DegreeToRadian(_robotStatus.Heading + angleToEnemy % 360);
                Console.WriteLine(angle);


                // Calculate the coordinates of the robot
                double enemyX = (_robotStatus.X + Math.Sin(angle) * e.Distance);
                double enemyY = (_robotStatus.Y + Math.Cos(angle) * e.Distance);


                // calculate firepower based on distance
                double firePower = Math.Min(500 / e.Distance, 3);
                // calculate speed of bullet
                double bulletSpeed = 20 - firePower * 3;
                // distance = rate * time, solved for time
                long time = (long)(e.Distance / bulletSpeed);


                // calculate gun turn to predicted x,y location
                double futureX = GetEstimatedXPosition(enemyX, e.Velocity, e.Heading, time);
                double futureY = GetEstimatedXPosition(enemyY, e.Velocity, e.Heading, time);

                //double absDeg = AbsoluteBearing(X, Y, futureX, futureY);
                double absDeg = AbsoluteBearing(X, Y, enemyX, enemyY);
                // non-predictive firing can be done like this:
                //double absDeg = absoluteBearing(getX(), getY(), enemy.getX(), enemy.getY());

                // turn the gun to the predicted x,y location
                TurnGunRight(NormalizeBearing(absDeg - GunHeading));

                //TurnGunRight(Heading - GunHeading + e.Bearing);
                // if the gun is cool and we're pointed at the target, shoot!
                if (GunHeat == 0)
                {
                    Fire(Math.Min(400 / e.Distance, 4));
                }
                else
                {
                    TurnRight(e.Bearing);
                }

            }
            else
            {
                _shotsFired = 0;
            }
        }

        protected double GetEstimatedXPosition(double enemyx, double velocity, double targetHeading, double time)
        {
            return enemyx + velocity * time * Math.Sin(DegreeToRadian(targetHeading));
        }

        protected double GetEstimatedYPosition(double enemyy, double velocity, double targetHeading, double time)
        {
            return enemyy + velocity * time * Math.Cos(DegreeToRadian(targetHeading));
        }

        private double AbsoluteBearing(double x1, double y1, double x2, double y2)
        {
            double xo = x2 - x1;
            double yo = y2 - y1;
            double hyp = (Math.Pow(x1 - x2, 2) + Math.Pow(y1 - y2, 2));
            double arcSin = RadianToDegree(Math.Asin(xo / hyp));
            double bearing = 0;

            if (xo > 0 && yo > 0)
            { // both pos: lower-Left
                bearing = arcSin;
            }
            else if (xo < 0 && yo > 0)
            { // x neg, y pos: lower-right
                bearing = 360 + arcSin; // arcsin is negative here, actuall 360 - ang
            }
            else if (xo > 0 && yo < 0)
            { // x pos, y neg: upper-left
                bearing = 180 - arcSin;
            }
            else if (xo < 0 && yo < 0)
            { // both neg: upper-right
                bearing = 180 - arcSin; // arcsin is negative here, actually 180 + ang
            }

            return bearing;
        }

        private double NormalizeBearing(double angle)
        {
            while (angle > 180) angle -= 360;
            while (angle < -180) angle += 360;
            return angle;
        }

        private double RadianToDegree(double angle)
        {
            return angle * (180.0 / Math.PI);
        }

        private double DegreeToRadian(double angle)
        {
            return Math.PI * angle / 180.0;
        }

    }
}