using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Timers;

namespace ChickenFarmModel
{
    public delegate void priceCutEvent(Int32 price);

    class Program
    {
        public static Int32 NUM_RETAILERS = 10;
        private static Thread farmer;
        public static OrderBuffer orderBuffer;
        private static ChickenFarm farmRamRod;
        private static Retailer chickenstore;

        static void Main(string[] args)
        {
            Random randNum = new Random();
            bool childIsAlive = true;

            farmRamRod = new ChickenFarm();
            orderBuffer = new OrderBuffer();
            farmer = new Thread(new ThreadStart(farmRamRod.farmerFunc));
            farmer.Start();         // Start one farmer thread
            chickenstore = new Retailer(-1);
            ChickenFarm.priceCut += new priceCutEvent(chickenstore.chickenOnSale);
            Thread[] retailers = new Thread[NUM_RETAILERS];
            for (int i = 0; i < NUM_RETAILERS; i++)
            {   // Start N retailer threads
                retailers[i] = new Thread(new ThreadStart(new Retailer(i).retailerFunc));
                retailers[i].Name = (i + 1).ToString();
                retailers[i].Start();
            }

            // wait until the retailer threads stop before ending
            // this implies the ChickenFarm thread is still running
            // because the retailer threads run until the CF ends
            while (childIsAlive)
            {
                Thread.Sleep(randNum.Next(1, 100));

                childIsAlive = false;
                for (int i = 0; i < retailers.Length; i++)
                {
                    if (retailers[i].IsAlive) childIsAlive = true;
                }
            }
        }

        // accessor method that allows retailers to check if the CF is alive still
        public static bool ChickenFarmIsAlive()
        {
            return farmer.IsAlive;
        }
    }
}
