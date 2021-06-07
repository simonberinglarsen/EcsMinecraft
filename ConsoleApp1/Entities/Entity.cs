namespace ConsoleApp1.Entities
{
    public class Entity
    {
        private string name;

        public Entity(string name)
        {
            this.name = name;
        }

        public string Name => name;
   }
}