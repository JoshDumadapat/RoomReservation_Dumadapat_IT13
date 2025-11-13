namespace RoomReservation_Dumadapat_IT13
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new MainPage()) { Title = "RoomReservation_Dumadapat_IT13" };
        }
    }
}
