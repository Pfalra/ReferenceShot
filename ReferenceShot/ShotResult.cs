using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DisagReferenceShot
{
    public enum Discpline
    {
        LG,
        LP
    } 

    public class Shot
    {
        public uint id;
        public uint shootersId;
        public int x;
        public int y;
        public double decimalResult;
        public double factor;
        public double div;
        public double distToCenter;

        public Shot (uint id, int x, int y, double decimalResult, uint shootersId, double factor, double div)
        {
            this.id = id;
            this.x = x;
            this.y = y;
            this.decimalResult = decimalResult;
            this.shootersId = shootersId;
            this.factor = factor;
            this.div = div;
            distToCenter = ShotResult.CalculateDistanceToRef(0, 0, this);
        }

    }

    public class Series
    {
        uint id;
        List<Shot> Shots = new List<Shot>();

        public Series(uint id, List<Shot> shots)
        {
            Shots = shots;
        }
    }


    public class ShotResult
    {
        double distanceToReference;
        Shot referenceShot;
        Shot compareShot;

        public ShotResult(Shot reference, Shot comp)
        {
            referenceShot = reference;
            compareShot = comp;
            distanceToReference = CalculateDistanceToRef(reference, comp);
        }

        public Shot GetReferenceShot()
        {
            return referenceShot;
        }

        public Shot GetCompareShot()
        {
            return compareShot;
        }

        public string GetDistanceStr()
        {
            return (distanceToReference * 0.01) + "mm";
        }

        public double GetDistanceRaw()
        {
            return distanceToReference;
        }

        public static double CalculateDistanceToRef(Shot refShot, Shot compShot)
        {
            int xdist = refShot.x - compShot.x;
            int ydist = refShot.y - compShot.y;

            if (compShot.factor == 0)
            {
                compShot.factor = 1;
            }

            uint xSquare = (uint) (Math.Abs(xdist) * Math.Abs(xdist));
            uint ySquare = (uint) (Math.Abs(ydist) * Math.Abs(ydist));
            double root = Math.Sqrt((xSquare) + (ySquare));
            root *= 0.01;

            //0.01mm
            double result =  root / compShot.factor;
            return result;
        }



        public static double CalculateDistanceToRef(int x, int y, Shot compShot)
        {
            int xdist = x - compShot.x;
            int ydist = y - compShot.y;

            if (compShot.factor == 0)
            {
                compShot.factor = 1;
            }

            uint xSquare = (uint)(Math.Abs(xdist) * Math.Abs(xdist));
            uint ySquare = (uint)(Math.Abs(ydist) * Math.Abs(ydist));
            double root = Math.Sqrt((xSquare) + (ySquare));
            root *= 0.01;

            //0.01mm
            double result = root / compShot.factor;
            return result;
        }


        public static void OrderResults(ref List<Shooter> shootersList)
        {
            shootersList = shootersList.OrderBy(shooter => shooter.BestShotDistToReference).ToList();
        }

    }
}
