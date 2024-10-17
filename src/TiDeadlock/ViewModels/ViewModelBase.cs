using CommunityToolkit.Mvvm.ComponentModel;

namespace TiDeadlock.ViewModels;

public abstract class ViewModelBase: ObservableObject
{
    public abstract void OnLoaded();
}