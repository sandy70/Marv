namespace Marv.Common
{
    // Just a test comment.
    public class Flaw
    {
        public double Depth { get; set; }

        public int Id { get; set; }

        public double Length { get; set; }

        public Flaw(int id, double len, double d)
        {
            this.Id = id;
            this.Length = len;
            this.Depth = d;
        }
    }
}