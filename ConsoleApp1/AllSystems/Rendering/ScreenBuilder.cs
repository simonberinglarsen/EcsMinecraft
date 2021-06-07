namespace ConsoleApp1.AllSystems.Rendering
{
    using ConsoleApp1.AllSystems.Rendering.Gpu;
    using ConsoleApp1.Components;
    using ConsoleApp1.Entities;
    using OpenTK.Mathematics;

    public class ScreenBuilder
    {
        private static readonly Vector3[] ColorValues =
        {
            new Vector3(0, 0, 0),
            new Vector3(29, 43, 83),
            new Vector3(126, 37, 83),
            new Vector3(0, 135, 81),
            new Vector3(171, 82, 54),
            new Vector3(95, 87, 79),
            new Vector3(194, 195, 199),
            new Vector3(255, 241, 232),
            new Vector3(255, 0, 77),
            new Vector3(255, 163, 0),
            new Vector3(255, 236, 39),
            new Vector3(0, 228, 54),
            new Vector3(41, 173, 255),
            new Vector3(131, 118, 156),
            new Vector3(255, 119, 168),
            new Vector3(255, 204, 170),
        };

        private static readonly string CharOrder = string.Empty
            + @"@ABCDEFGHIJKLMNO"
            + @"PQRSTUVWXYZ[\]↑←"
            + @" !""#$%&'()*+,-./"
            + @"0123456789:;<=>?"
            + @"─♠¿¿¿¿¿¿¿╮╰╯¿¿¿¿"
            + @"¿●¿♥¿╭¿○♣¿♦┼¿¿✜¿"
            + @"¿¿¿¿¿¿¿¿¿¿¿├¿¿¿¿"
            + @"┌¿¿¿¿¿¿¿¿¿¿¿¿¿¿¿"
            + @"@abcdefghijklmno"
            + @"pqrstuvwxyz[\]↓→"
            + @" !""#$%&'()*+,-…/"
            + @"0123456789:;<=>?"
            + @"─웃☖CDEFGHIJKLMNO"
            + @"PQRSTUVWXYZ¿¿│¿¿"
            + @"¿▌▄▔▁¿¿¿¿▨¿¿▗└┐¿"
            + @"┌┴┬┤¿¿¿¿¿¿¿▖▝┘▘▚";

        private ScreenComponent screen;

        static ScreenBuilder()
        {
            for (var i = 0; i < ColorValues.Length; i++)
            {
                Vector3.Multiply(ColorValues[i], 1 / 255f, out ColorValues[i]);
            }
        }

        public ScreenBuilder(ScreenComponent comp)
        {
            this.screen = comp;
        }

        public static Vector4 GetColor(Color color)
        {
            if ((color & Color.Inverted) == Color.Inverted)
            {
                var actualColor = color & ~Color.Inverted;
                return new Vector4(ColorValues[(int)actualColor], 0f);
            }

            return new Vector4(ColorValues[(int)color], 1f);
        }

        public VertexBuffer Build()
        {
            VertexBuffer newVertices = new VertexBuffer();
            Vector2 pos = default(Vector2);
            newVertices.AddFloatRange(BuildBackground());
            for (var i = 0; i < screen.Buffer.Length; i++)
            {
                pos.X = i % screen.Width;
                pos.Y = -i / screen.Width;
                if (screen.Buffer[i] == ' ' && (screen.ColBuffer[i] & Color.Inverted) != Color.Inverted)
                {
                    continue;
                }

                var charVerts = BuildLetter(screen.Buffer[i], pos, GetColor(screen.ColBuffer[i]));
                newVertices.AddFloatRange(charVerts);
            }

            return newVertices;
        }

        private float[] BuildLetter(char ch, Vector2 pos, Vector4 col)
        {
            int index = CharOrder.IndexOf(ch);
            int x = ((index / 16) * 12) + 2;
            int y = ((index % 16) * 11) + 1;
            float tx = x / 256.0f;
            float ty = 1.0f - (y / 256.0f);
            float ts = 8f / 256.0f;
            float letterSize = 1f;
            float[] vertices =
            {
                pos.X, pos.Y, 0.0f, tx, ty, col.X, col.Y, col.Z, col.W,
                pos.X + letterSize, pos.Y - letterSize, 0.0f, tx + ts, ty - ts, col.X, col.Y, col.Z, col.W,
                pos.X + letterSize, pos.Y, 0.0f, tx + ts, ty, col.X, col.Y, col.Z, col.W,

                pos.X, pos.Y, 0.0f, tx, ty, col.X, col.Y, col.Z, col.W,
                pos.X, pos.Y - letterSize, 0.0f, tx, ty - ts, col.X, col.Y, col.Z, col.W,
                pos.X + letterSize, pos.Y - letterSize, 0.0f, tx + ts, ty - ts, col.X, col.Y, col.Z, col.W,
            };
            return vertices;
        }

        private float[] BuildBackground()
        {
            float[] vertices = BuildLetter(' ', new Vector2(0, 0), GetColor(screen.BackgroundColor));
            for (int i = 0; i < 6; i++)
            {
                vertices[(i * 9) + 0] = vertices[(i * 9) + 0] * screen.Width;
                vertices[(i * 9) + 1] = vertices[(i * 9) + 1] * screen.Height;
            }

            return vertices;
        }
    }
}
