using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ChickenFarmModel
{
    //the Encoder class takes an order object and turns it into a string
    class Encoder
    {
        public static string encode(Order order)
        {
            return (order.ToString());
        }
    }
}
