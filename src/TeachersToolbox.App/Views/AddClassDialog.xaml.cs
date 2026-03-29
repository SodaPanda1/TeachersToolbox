using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace TeachersToolbox.App.Views;

public sealed partial class AddClassDialog : ContentDialog
{
    public string ClassName { get; private set; } = string.Empty;

    public AddClassDialog()
    {
        this.InitializeComponent();
        this.PrimaryButtonClick += OnPrimaryButtonClick;
    }

    private void OnPrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
    {
        if (string.IsNullOrWhiteSpace(ClassNameBox.Text))
        {
            args.Cancel = true;
            return;
        }
        
        ClassName = ClassNameBox.Text.Trim();
    }
}
