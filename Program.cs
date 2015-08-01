using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WordAcademySolver
{
    class Program
    {
        static void Main(string[] args)
        {
            if (!Reader.ReadFromFile(args[0])) return;
            new Solver();
            Console.Read();
        }
    }

    public class CustomRect
    {
        public Point loc;
        public CustomRect Parent;
        public bool Visited;
        public char Letter;
        public int Chained;
        public CustomRect[] SearchableNeighbors = new CustomRect[0];
        public string ChainedString;
        
        public CustomRect(Point pt, CustomRect p, char letter)
        {
            loc = pt;
            Parent = p;
            Visited = false;
            Letter = letter;
            Chained = GetChainValue();
        }
        public CustomRect Clone()
        {
            CustomRect cr = new CustomRect(loc, Parent, Letter);
            cr.Visited = this.Visited;
            cr.UpdateChainValue();
            return cr;
        }
        public int GetChainValue()
        {
            int c = 0;
            CustomRect tmp = this;
            do
            {
                c++; //will have at least 1
                tmp = tmp.Parent;
            } while (tmp != null) ;
            
            return c;
        }
        public void UpdateChainValue()
        {
            Chained = GetChainValue();
            ChainedString = ChainedText();
        }
        public override string ToString()
        {
            return string.Format("{0} :: {1} :: {2} -> {3}", Letter, loc, Visited ? "Visit" : "Not visit", ChainedText());
        }
        private string ChainedText()
        {
            string s = "";
            CustomRect tmp = this;
            do
            {
                s += tmp.Letter; //will have at least 1
                tmp = tmp.Parent;
            } while (tmp != null);

            return new string(s.Reverse().ToArray());
        }
        public override bool Equals(object obj)
        {
            CustomRect cr = (CustomRect)obj;
            if (loc != cr.loc) return false;
            if (Letter != cr.Letter) return false;
            return true;
    }
    }
    public struct Point
    {
        private int? _x;
        private int? _y;
        public int x
        {
            get { return (_x.HasValue) ? _x.Value : -1; }
            set { _x = value; }
        }
        public int y
        {
            get { return (_y.HasValue) ? _y.Value : -1; }
            set { _y = value; }
        }

        public Point(int X, int Y) : this()
        {
            x = X;
            y = Y;
        }
        public Point(Point p)
            : this()
        {
            x = p.x;
            y = p.y;
        }

        public static bool operator ==(Point p1, Point p2)
        {
            return (p1.x == p2.x) && (p1.y == p2.y);
        }
        public static bool operator !=(Point p1, Point p2)
        {
            return (p1.x != p2.x) || (p1.y != p2.y);
        }

        public override bool Equals(object obj)
        {
            Point p = (Point)obj;
            return this == p;
        }

        public override string ToString()
        {
            return string.Format("[{0},{1}]", x, y);
        }
    }
}
