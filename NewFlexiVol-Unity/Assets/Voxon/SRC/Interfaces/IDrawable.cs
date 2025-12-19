
namespace Voxon
{
    public enum Flags
    {
        DOTS = 0,
        LINES = 1,
        SURFACES = 2,
        SOLID = 3
    };

    public interface IDrawable
    {
        void Draw();
    }
}
