using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Pipes;
using System.IO;
using System.Text.Json;
using Shared;
using System.Reflection;
using System.Windows.Forms;
using Microsoft.Reporting.WinForms;
using System.Runtime.Remoting.Metadata.W3cXsd2001;

namespace ProjectB
{
    internal class Program
    {
        [STAThread]
        static void Main(string[] args) 
        {
            Console.WriteLine("ProjectB - Listening...");
            while (true)
            {
                NamedPipeServerStream server = new NamedPipeServerStream("OrderPipe", PipeDirection.In);            
                using (server)
                {
                    Console.WriteLine("Waiting for client connection...");
                    server.WaitForConnection();
                    try
                    {
                        // Read stream from client to string.
                        string jsonResult = ReadServerStream(server);
                        // Attempt to deserialize the string to objects.
                        List<Order> orders = DeserializeOrderJson(jsonResult);
                        // write the result objects to console.
                        DisplayOrderRecords(orders);
                        // Attempt to render the RDLC report using the objects as a datasource.
                        TestReport(orders);
                    }
                    catch (Exception ex)
                    {
                        string pError = $"Error: handling pipe message: {ex.Message}";
                        Console.WriteLine(pError);
                    }                    
                }
                // pipe will be disposed and recreated on the next loop.
            }
        }

        // read jsson stream from server connection.
        public static string ReadServerStream(NamedPipeServerStream server)
        {
            StreamReader reader = new StreamReader(server, Encoding.UTF8);
            string readResult;
            using (reader)
            {
                readResult = reader.ReadToEnd();
            }
            readResult = string.IsNullOrEmpty(readResult) ? string.Empty : readResult;
            return readResult;
        }

        // deserialize json string to order objects.
        public static List<Order> DeserializeOrderJson(string jsonString)
        {
            List<Order> orders = new List<Order>();
            orders = JsonSerializer.Deserialize<List<Order>>(jsonString);
            return orders;
        }

        // write the records to console for debugging.
        public static void DisplayOrderRecords(List<Order> orders)
        {
            Console.WriteLine("Received Orders:");
            foreach (Order order in orders)
            {
                Console.WriteLine($"{order.OrderDate.ToShortDateString()} | {order.CompanyId} | {order.OrderNumber} | {order.Units} units | ${order.Charges}");
            }
        }

        // test rendering report.
        public static void TestReport(List<Order> orders)
        {
            const string DataName = "SharedDataSet";            
            ReportDataSource reportDataSource = new ReportDataSource();
            reportDataSource.Value = orders;            
            reportDataSource.Name = DataName;

            ReportParameter reportParameter = new ReportParameter();
            reportParameter.Name = "";

            const string ReportPath = "ProjectB.Reports.OrderReport.rdlc";
            LocalReport localReport = new LocalReport();
            localReport.DataSources.Clear();
            localReport.DataSources.Add(reportDataSource);
            localReport.ReportEmbeddedResource = ReportPath;

            const string ReportFormat = "PDF";
            byte[] bytes = localReport.Render(format: ReportFormat);

            const string ReportDir = @"C:\Reports\";
            string fileName = $"report-{DateTime.Now:yyyy-MM-dd-HH-mm-ss-ffff}.pdf";
            string filePath = Path.Combine(ReportDir, fileName);
            FileStream fs = new FileStream(path: filePath, mode: FileMode.Create);
            using (fs)
            {
                fs.Write(bytes, 0, bytes.Length);
                fs.Close();
            }
        }

    }
}
