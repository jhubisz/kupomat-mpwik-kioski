using KioskAppWpf.Classes;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace KioskAppWpf.Controls
{
    public partial class ServicePageItem: UserControl
    {
        private ServicePageItemState state;
        public ServicePageItemState State
        {
            get
            {
                return state;
            }
            set
            {
                state = value;
                BindState();
            }
        }
        public Visibility DiodeRedVisibility
        {
            get
            {
                return State.Error ? Visibility.Visible : Visibility.Hidden;
            }
        }
        public Visibility DiodeGreenVisibility
        {
            get
            {
                return State.Error ? Visibility.Hidden : Visibility.Visible;
            }
        }
        public string ItemLabel
        {
            get
            {
                return State.Label;
            }
        }

        public ServicePageItem(ServicePageItemState state)
        {
            InitializeComponent();
            State = state;
        }

        private void BindState()
        {
            if (state.Index % 2 != 0)
            {
                this.Background = new SolidColorBrush(Color.FromArgb(255, 163, 225, 248));
            }
            tbName.Text = ItemLabel;
            tbDiodeGreen.Visibility = DiodeGreenVisibility;
            tbDiodeRed.Visibility = DiodeRedVisibility;
        }
    }
}
