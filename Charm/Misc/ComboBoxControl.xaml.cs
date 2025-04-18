using System.Windows.Controls;

namespace Charm;

public partial class ComboBoxControl : UserControl
{
    public ComboBoxControl()
    {
        InitializeComponent();
        DataContext = this;
    }

    public string Text { get; set; }
    public int FontSize { get; set; } = 16;

    public string Label { get; set; }
    public int LabelFontSize { get; set; } = 12;
}
