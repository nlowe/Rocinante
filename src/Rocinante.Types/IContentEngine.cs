namespace Rocinante.Types
{
    public interface IContentEngine
    {
         string Identifier { get;}

         string Render(Post post);
    }
}