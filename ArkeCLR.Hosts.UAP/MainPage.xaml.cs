using ArkeCLR.Runtime;
using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace ArkeCLR.Hosts.UAP {
    public sealed partial class MainPage : Page {
        private Host host;

        public MainPage() {
            this.InitializeComponent();

            this.FindRunnableAssembliesAsync();
        }

        private async void FindRunnableAssembliesAsync() {
            var files = (await ApplicationData.Current.LocalFolder.GetFilesAsync()).Where(f => Path.GetExtension(f.Name) == ".exe");

            if (files.Any()) {
                foreach (var f in files)
                    this.RunnableAssemblyList.Items.Add(new ComboBoxItem() { Content = f.Name });

                this.RunnableAssemblyList.IsEnabled = true;
                this.RunButton.IsEnabled = true;
            }
            else {
                this.RunnableAssemblyList.Items.Add(new ComboBoxItem() { Content = "No runnable assemblies found" });
            }

            this.RunnableAssemblyList.SelectedIndex = 0;
        }

        private async void RunButtonClickAsync(object sender, RoutedEventArgs e) {
            this.RunButton.IsEnabled = false;
            this.StatusField.Text = "Running";

            this.host = new Host(new AssemblyName((string)((ComboBoxItem)this.RunnableAssemblyList.SelectedItem).Content), new AssemblyResolver());

            try {
                this.StatusField.Text = "Exited with code " + await this.host.StartAsync();
            }
            catch (CouldNotResolveAssemblyException exception) {
                this.StatusField.Text = "Could not find assembly: " + exception.AssemblyName.FullName;
            }

            this.host = null;

            this.RunButton.IsEnabled = true;
        }

        private class AssemblyResolver : IAssemblyResolver {
            public async Task<(bool, byte[])> ResolveAsync(AssemblyName assemblyName) {
                try {
                    return (true, (await FileIO.ReadBufferAsync(await ApplicationData.Current.LocalFolder.GetFileAsync(assemblyName.Name))).ToArray());
                }
                catch (FileNotFoundException) {
                    return (false, null);
                }
            }
        }
    }
}
