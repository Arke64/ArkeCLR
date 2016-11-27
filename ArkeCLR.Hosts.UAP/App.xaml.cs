using System;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace ArkeCLR.Hosts.UAP {
    public sealed partial class App : Application {
        public App() {
            this.InitializeComponent();

            this.Suspending += (s, e) => e.SuspendingOperation.GetDeferral().Complete();
        }

        protected override void OnLaunched(LaunchActivatedEventArgs e) {
            var rootFrame = Window.Current.Content as Frame;

            if (rootFrame == null) {
                rootFrame = new Frame();

                rootFrame.NavigationFailed += (s, ee) => { throw new Exception("Failed to load Page " + ee.SourcePageType.FullName); };

                Window.Current.Content = rootFrame;
            }

            if (e.PrelaunchActivated == false) {
                if (rootFrame.Content == null)
                    rootFrame.Navigate(typeof(MainPage), e.Arguments);

                Window.Current.Activate();
            }
        }
    }
}
