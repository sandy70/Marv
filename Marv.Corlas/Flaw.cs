namespace Marv.Corlas
{
    // Just a test comment.
    public class Flaw
    {
        private double _depth;
        private int _id;
        private double _length;

        public Flaw(int id, double len, double d)
        {
            _id = id;
            _length = len;
            _depth = d;
        }

        public double Depth
        {
            get { return _depth; }
            set { _depth = value; }
        }

        public int ID
        {
            get { return _id; }
            set { _id = value; }
        }

        public double Length
        {
            get { return _length; }
            set { _length = value; }
        }
    }
}