using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DisagReferenceShot
{
    public class Shooter
    {
        public uint ShootersId;
        public string FirstName;
        public string LastName;
        public List<Shot> Shots;

        public Shot BestShotToReference = new Shot(99999, 99999, 99999, 0.0, 99999, 1, 99999);
        public double BestShotDistToReference = 99999;

        public Shooter(uint id, string first, string last, List<Shot> shots)
        {
            ShootersId = id;
            FirstName = first;
            LastName = last;
            Shots = shots;
            BestShotToReference.shootersId = id;
        }

        public override string ToString()
        {
            return String.Format("{0}, {1} (ID:{2})", LastName, FirstName, ShootersId);
        }



        public Shot FindBestShotByStandardMode()
        {
            Shot bestShot = new Shot(99999, 99999, 99999, 0.0, this.ShootersId, 1, 99999);

            // FIXME: Build a shot selector window instead of taking the best shot of the shooters shots
            foreach (Shot shot in this.Shots)
            {
                // Find best shot
                double dist = ShotResult.CalculateDistanceToRef(0, 0, shot);

                if (bestShot.distToCenter > shot.distToCenter)
                {
                    bestShot = shot;
                }
            }

            return bestShot;
        }


        public Shot FindBestShotByReference(Shot referenceShot, out double distMm)
        {
            Shot bestShot = new Shot(99999, 99999, 99999, 0.0, this.ShootersId, 1, 99999);
            distMm = 10000;

            // FIXME: Build a shot selector window instead of taking the best shot of the shooters shots
            foreach (Shot shot in this.Shots)
            {
                double d = ShotResult.CalculateDistanceToRef(referenceShot, shot);

                // Find best shot
                if (distMm > d)
                {
                    distMm = d;
                    bestShot = shot;
                }
            }

            BestShotToReference = bestShot;
            BestShotDistToReference = distMm;

            return bestShot;
        }
    }
}
