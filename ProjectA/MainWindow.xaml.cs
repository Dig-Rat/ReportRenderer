using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Text.Json;
using System.Threading.Tasks;
using Shared;


namespace ProjectA;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    // When the button is clicked, do this stuff.
    private async void SendOrders_Click(object sender, RoutedEventArgs e)
    {        
        List<Order> orders = GetTestOrders();
        string json = SerializeOrdersJson(orders);

        NamedPipeClientStream client = new NamedPipeClientStream(".", "OrderPipe", PipeDirection.Out);
        using (client)
        {
            // wait for client to connect.
            await client.ConnectAsync(2000);
            // encode serialized json to byte array.
            byte[] bytes = Encoding.UTF8.GetBytes(json);
            // Write the byte array of serialized json to server.
            await client.WriteAsync(bytes, 0, bytes.Length); 
        }
        // disable button.
        SendButton.IsEnabled = false;
        // Make button text countdown
        for (int x = 100; x >= 0; x--)
        {
            SendButton.Content = $"{x}";            
            //Thread.Sleep(1000);
        }        
        const string DefaultButtonContent = "Send Orders to ProjectB";
        SendButton.Content = DefaultButtonContent;
        // enable button.
        SendButton.IsEnabled = true;

        //MessageBox.Show("Orders sent to ProjectB.");
    }

    // Create some sample records.
    private static List<Order> GetTestOrders()
    {
        List<Order> orders = new List<Order>
            {
                new Order { CompanyId = "075", OrderNumber = "1001", Units = 5, Charges = 99.95m, OrderDate = DateTime.Now },
                new Order { CompanyId = "081", OrderNumber = "1002", Units = 3, Charges = 59.95m, OrderDate = DateTime.Now.AddDays(-5) }
            };
        return orders;
    }

    // Serialize order records to json string.
    private static string SerializeOrdersJson(List<Order> orders) 
    {
        string json = JsonSerializer.Serialize(orders);
        return json;
    }


}