namespace Marv
{
    public class SectionEvidence : Dict<int, string, VertexEvidence>
    {
        public void Write(string filePath)
        {
            this.WriteJson(filePath);
        }
    }
}