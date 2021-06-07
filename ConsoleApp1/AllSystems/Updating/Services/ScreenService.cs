namespace ConsoleApp1.AllSystems.Updating.Services
{
    using ConsoleApp1.Components;
    using ConsoleApp1.Entities;

    public class ScreenService
    {
        private ScreenComponent comp;

        public void Bind(ScreenComponent comp)
        {
            this.comp = comp;
        }

        public void Unbind()
        {
            Bind(null);
        }

        public void PrintCol(int x, int y, string text)
        {
            int ofs = (y * comp.Width) + x;
            foreach (char ch in text)
            {
                if (ofs >= comp.ColBuffer.Length)
                {
                    break;
                }

                if (ofs >= 0)
                {
                    Color color = ch >= 'a' ? (Color)(ch - 'a') : (Color)(ch - 'A') | Color.Inverted;
                    comp.ColBuffer[ofs] = color;
                }

                ofs++;
            }

            comp.Dirty = true;
        }

        public void Clear(Color clearColor = Color.Black)
        {
            for (int i = 0; i < comp.Buffer.Length; i++)
            {
                comp.Buffer[i] = ' ';
                comp.ColBuffer[i] = clearColor;
            }

            comp.Dirty = true;
        }

        public void Print(int x, int y, string text, Color color = Color.White)
        {
            int ofs = (y * comp.Width) + x;
            foreach (char ch in comp.OnlyBlanks ? string.Empty.PadLeft(text.Length) : text)
            {
                if (ofs >= comp.Buffer.Length)
                {
                    break;
                }

                if (ofs >= 0)
                {
                    if (comp.Buffer[ofs] != ch || comp.ColBuffer[ofs] != color)
                    {
                        comp.Dirty = true;
                        comp.Buffer[ofs] = ch;
                        comp.ColBuffer[ofs] = color;
                    }
                }

                ofs++;
            }
        }
    }
}