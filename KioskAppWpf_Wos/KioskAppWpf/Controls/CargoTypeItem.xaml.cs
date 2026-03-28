using DbCommunication.Entities;
using System.Windows.Controls;

namespace KioskAppWpf.Controls
{
    public partial class CargoTypeItem : UserControl
    {
        public CargoType CargoType { get; set; }
        public string ShortcutKey { get; set; }
        public string CargoTypeLabel
        {
            get
            {
                return /*" - " + */CargoType.Name;
            }
        }

        public CargoTypeItem(CargoType cargoType, string shortcutKey)
        {
            InitializeComponent();

            CargoType = cargoType;
            ShortcutKey = shortcutKey;

            tbKey.Text = ShortcutKey;
            tbName.Text = CargoTypeLabel;
        }
    }
}
