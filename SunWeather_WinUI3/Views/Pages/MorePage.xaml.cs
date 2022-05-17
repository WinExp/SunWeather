using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using SunWeather_WinUI3.Class.Tools.Net.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace SunWeather_WinUI3.Views.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MorePage : Page
    {
        private const string messagesUrl = "https://we-bucket.oss-cn-shenzhen.aliyuncs.com/Project/Download/SunWeather/Messages/messages.txt";
        private bool isMessageLoading = false;
        private string[] messages;

        public MorePage()
        {
            this.InitializeComponent();
        }

        private void MessageBorder_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            MessageBorder.Background.Opacity = 0.5;
        }

        private void MessageBorder_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            MessageBorder.Background.Opacity = 1;
        }

        private async void MessageBorder_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            if (isMessageLoading)
            {
                return;
            }
            isMessageLoading = true;
            bool isLoad = false;
            if (messages == null)
            {
                MessageLoadingProgressRing.Visibility = Visibility.Visible;
                MessageLoadingProgressRing.IsActive = true;
                MessageTextBlock.Visibility = Visibility.Collapsed;
                messages = (await WebRequests.GetStringAsync(messagesUrl)).Split('\n');
                MessageLoadingProgressRing.IsActive = false;
                MessageLoadingProgressRing.Visibility = Visibility.Collapsed;
                MessageTextBlock.Visibility = Visibility.Visible;
                isLoad = true;
            }
            Random random = new Random(Guid.NewGuid().GetHashCode());
            int messageIndex = random.Next(messages.Length);
            while (messageIndex == Array.IndexOf(messages, MessageTextBlock.Text))
            {
                messageIndex = random.Next(messages.Length);
            }
            string message = messages[messageIndex];
            if (!isLoad)
            {
                for (int i = 0; i < random.Next(3, 5); i++)
                {
                    MessageTextBlock.Visibility = Visibility.Collapsed;
                    await Task.Delay(50);
                    MessageTextBlock.Visibility = Visibility.Visible;
                    await Task.Delay(50);
                }
            }
            MessageTextBlock.Text = String.Empty;
            await Task.Delay(150);
            foreach (char c in message)
            {
                MessageTextBlock.Text += c;
                await Task.Delay(40);
            }
            isMessageLoading = false;
        }
    }
}
