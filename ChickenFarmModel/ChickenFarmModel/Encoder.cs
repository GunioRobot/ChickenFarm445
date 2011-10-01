using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ChickenFarmModel
{
    class Encoder
    {
        public static string encode(Order order)
        {
            return (order.ToString());
        }
    }
}
