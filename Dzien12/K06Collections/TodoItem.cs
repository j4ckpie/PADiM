using CommunityToolkit.Mvvm.ComponentModel;

namespace K06Collections;

public partial class TodoItem : ObservableObject
{
    [ObservableProperty]
    public partial string Title { get; set; }

    [ObservableProperty]
    public partial bool IsDone { get; set; }
}