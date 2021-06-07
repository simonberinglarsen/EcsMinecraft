namespace ConsoleApp1.AllSystems.Updating.Services
{
    using OpenTK.Mathematics;

    public interface IWorldAccess
    {
        int GetLight(Vector3i p);

        void SetLight(Vector3i p, int l);

        int GetSkyLight(Vector3i p);

        void SetSkyLight(Vector3i p, int l);

        bool NoLightPasses(Vector3i p);

        bool OutOfBounds(Vector3i p);
    }
}