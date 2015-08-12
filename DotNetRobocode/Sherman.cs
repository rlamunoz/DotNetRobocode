using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using Robocode;
using Robocode.Util;
using Robocode.RobotInterfaces.Peer;

namespace Sherman
{
    public class Sherman : TeamRobot
    {
        int count = 0;
        int hitBullet = 0;
        double gunTurnAmt;

        public override void Run()
        {

            BodyColor = (Color.FromArgb(128, 128, 30));
            GunColor = (Color.FromArgb(50, 50, 20));
            RadarColor = (Color.FromArgb(200, 200, 30));
            ScanColor = (Color.White);
            BulletColor = (Color.Red);

            IsAdjustGunForRobotTurn = true;
            gunTurnAmt = 10;

            while (true)
            {
                TurnGunRight(gunTurnAmt);
                count++;
                if (count > 2)
                {
                    gunTurnAmt = -10;
                }
                if (count > 5)
                {
                    gunTurnAmt = 10;
                }
            }
        }

        public override void OnScannedRobot(ScannedRobotEvent e)
        {

            if (IsTeammate(e.Name))
            {
                return;
            }
            if (hitBullet < 3)
            {
                SetTurnRight(e.Bearing + 90);
                Fire(Rules.MAX_BULLET_POWER);
                Back(80);
                Ahead(80);
            }
            Scan();
        }

        public override void OnHitWall(HitWallEvent evnt)
        {
            hitBullet++;
            if (hitBullet == 3)
            {
                hitBullet = 0;
                Ahead(180);
                Back(80);
            }
        }
        public override void OnHitRobot(HitRobotEvent evnt)
        {
            hitBullet = 0;
        }

        public override void OnWin(WinEvent e)
        {
            for (int i = 0; i < 10; i++)
            {
                TurnRight(30);
                TurnLeft(30);
                Fire(Rules.MAX_BULLET_POWER);
            }
        }
    }
}