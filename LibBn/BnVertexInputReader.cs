using System.Collections.Generic;
using System.Threading.Tasks;

namespace LibBn
{
    public class BnVertexInputReader
    {
        public List<BnVertexInput> Read(string fileName)
        {
            CsvReader csvReader = new CsvReader(fileName);
            string[,] data = csvReader.Read();

            return this.Parse(data);
        }

        public void Read(List<BnVertexInput> graphInput, string fileName)
        {
            CsvReader csvReader = new CsvReader(fileName);
            string[,] data = csvReader.Read();

            this.Parse(data, graphInput);
        }

        public async Task<List<BnVertexInput>> ReadAsync(string fileName)
        {
            CsvReader csvReader = new CsvReader(fileName);
            string[,] data = await csvReader.ReadAsync();

            return this.Parse(data);
        }

        private VertexInputType GetInputType(int inputTypeIndex)
        {
            if (inputTypeIndex == 1)
            {
                return VertexInputType.StateSelected;
            }
            else if (inputTypeIndex == 2)
            {
                return VertexInputType.Distribution;
            }
            else
            {
                return VertexInputType.PastYear;
            }
        }

        private List<BnVertexInput> Parse(string[,] data)
        {
            var inputs = new List<BnVertexInput>();
            int nVertices = data.GetLength(0);

            for (int v = 0; v < nVertices; v++)
            {
                var vertexInput = new BnVertexInput();

                vertexInput.Key = data[v, 0];
                vertexInput.InputType = this.GetInputType(int.Parse(data[v, 2]));

                if (vertexInput.InputType == VertexInputType.StateSelected)
                {
                    int.TryParse(data[v, 3], out vertexInput.SelectedStateIndex);
                }
                else if (vertexInput.InputType == VertexInputType.Distribution)
                {
                    int i = 3;

                    while (i < data.GetLength(1) && data[v, i].Length > 0)
                    {
                        double evidence;
                        double.TryParse(data[v, i], out evidence);
                        vertexInput.Evidence.Add(evidence);

                        i++;
                    }
                }
                else
                {
                    vertexInput.InputVertexKey = data[v, 1];
                }

                inputs.Add(vertexInput);
            }

            return inputs;
        }

        private List<BnVertexInput> Parse(string[,] data, List<BnVertexInput> inputs)
        {
            int nVertices = data.GetLength(0);

            for (int v = 0; v < nVertices; v++)
            {
                var vertexInput = new BnVertexInput();

                vertexInput.Key = data[v, 0];
                vertexInput.InputType = this.GetInputType(int.Parse(data[v, 2]));

                if (vertexInput.InputType == VertexInputType.StateSelected)
                {
                    int.TryParse(data[v, 3], out vertexInput.SelectedStateIndex);
                }
                else if (vertexInput.InputType == VertexInputType.Distribution)
                {
                    int i = 3;

                    while (i < data.GetLength(1) && data[v, i].Length > 0)
                    {
                        double evidence;
                        double.TryParse(data[v, i], out evidence);
                        vertexInput.Evidence.Add(evidence);

                        i++;
                    }
                }
                else
                {
                    vertexInput.InputVertexKey = data[v, 1];
                }

                inputs.Add(vertexInput);
            }

            return inputs;
        }
    }
}