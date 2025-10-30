using DefaultNamespace;

namespace Features.Title.Presentation.Interfaces
{
    public interface ITitleView
    {
        GenericButton ToSettingsButton { get; }
        GenericButton ToCreditButton { get; }
        GenericButton ToMainMenuButton { get; }
    }
}