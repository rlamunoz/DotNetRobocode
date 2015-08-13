using Robocode;
using System.Drawing;

namespace Ronny.Robots
{
    public class Demoledor : TeamRobot
    {
        private bool movingForward;
        string mode = "normal";
        int velocity = 5;
        public override void Run()
        {
            SetColors(Color.Black, Color.Black, Color.Turquoise);
            BulletColor = Color.Orange;

            while (true)
            {
                Out.WriteLine("Modo: {0} ", mode);

                if (mode.Equals("normal"))
                {
                    MaxVelocity = velocity;
                    SetAhead(1000);
                    movingForward = true;
                    SetTurnRight(90);
                    WaitFor(new TurnCompleteCondition(this));
                    SetTurnLeft(180);
                    WaitFor(new TurnCompleteCondition(this));
                    SetTurnRight(180);
                    WaitFor(new TurnCompleteCondition(this));
                }
                else if (mode.Equals("vuelticas"))
                {
                    TurnRight(100);
                    WaitFor(new TurnCompleteCondition(this));
                    MaxVelocity = velocity;
                    SetAhead(10);
                }
                else if (mode.Equals("huiga"))
                {
                    velocity = 8;
                    mode = "normal";
                }
            }
        }

        public override void OnHitWall(HitWallEvent e)
        {
            reverseDirection();
        }

        public void reverseDirection()
        {
            if (movingForward)
            {
                SetBack(1000);
                movingForward = false;
            }
            else
            {
                SetTurnLeft(90);
                SetAhead(1000);
                movingForward = true;
            }
        }

        public override void OnScannedRobot(ScannedRobotEvent e)
        {
            if (IsTeammate(e.Name))
            {
                return;
            }

            if (e.Distance < 300)
            {
                Fire(Rules.MAX_BULLET_POWER);
            }
            else
            {
                Fire(1);
                SetAhead(100);
                Fire(Rules.MAX_BULLET_POWER);
                SetAhead(100);
                Fire(Rules.MAX_BULLET_POWER);
            }

            if (e.Energy > 50 && Energy < 50)
            {
                mode = "huiga";
            }
        }

        public override void OnHitRobot(HitRobotEvent e)
        {
            if (e.Bearing > -10 && e.Bearing < 10)
            {
                Fire(Rules.MAX_BULLET_POWER);
            }
            if (e.IsMyFault)
            {
                MaxVelocity = 8;
                TurnRight(100);
                WaitFor(new TurnCompleteCondition(this));
                SetAhead(100);
            }
            if (e.Energy < 50 && Energy > 50)
            {
                mode = "vuelticas";
            }
        }

        public override void OnHitByBullet(HitByBulletEvent e)
        {
            reverseDirection();

            if (Energy > 50)
            {
                mode = "normal";
            }
            else
            {
                mode = "huiga";
            }

            Out.WriteLine("Oooouch" + e.Name + " me dio");
        }

        public override void OnWin(WinEvent evnt)
        {
            TurnRight(180);
            Ahead(10);
            TurnLeft(180);
            Ahead(-10);
            Out.WriteLine("Gane lero lero :P");
        }
    }
}
