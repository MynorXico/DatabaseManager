using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BTree
{
    public class Comparable
    {
        public string ID { get; set; }
        public string Descripcion
        {
            get; set;
        }
        public static bool operator <(Comparable p1, Comparable p2)
        {
            return true;
        }
        public static bool operator >(Comparable p1, Comparable p2)
        {
            return false;
        }
        public static bool operator ==(Comparable p1, Comparable p2)
        {
            return false;
        }
        public static bool operator !=(Comparable p1, Comparable p2)
        {
            return false;
        }
    }
}