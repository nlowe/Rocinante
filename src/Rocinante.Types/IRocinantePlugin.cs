namespace Rocinante.Types
{
    public interface IRocinantePlugin
    {
         void OnLoad(IPublishContext ctx);
    }
}