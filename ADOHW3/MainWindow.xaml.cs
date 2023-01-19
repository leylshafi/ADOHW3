using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data.Common;
using System.Data;
using System.Linq;

namespace ADOHW3;

public partial class MainWindow : Window
{
    DbConnection? connection = null;
    DbDataAdapter? adapter = null;
    DbProviderFactory? providerFactory = null;
    IConfigurationRoot? configuration = null;
    DataSet? dataSet = null;
    string providerName = string.Empty;
    public MainWindow()
    {
        InitializeComponent();
        Configure();
    }

    private void Configure()
    {
        
        DbProviderFactories.RegisterFactory("System.Data.SqlClient", typeof(SqlClientFactory));
        providerName = "System.Data.SqlClient";
        providerFactory = DbProviderFactories.GetFactory(providerName);
        configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
        dataSet = new DataSet();
        connection = providerFactory.CreateConnection();
        connection.ConnectionString = configuration.GetConnectionString(providerName);
        adapter = providerFactory.CreateDataAdapter();
        DataTableMapping[] mappings = {new DataTableMapping("Table","Books"),
            new DataTableMapping("Table1","Authors"),
        new DataTableMapping("Table2","Students")};
        adapter.TableMappings.AddRange(mappings);
        
    }

    private void btnExecute_Click(object sender, RoutedEventArgs e)
    {
        var command = providerFactory?.CreateCommand();
        command.CommandText = txtCommand.Text;
        command.Connection = connection;
        adapter.SelectCommand = command;
        if (Tabs.Items.Count > 1)
        {
            for (int i = Tabs.Items.Count - 1; i > 0; i--)
                Tabs.Items.RemoveAt(i);
        }
        dataSet.Tables.Clear();
        try
        {
            
            adapter.Fill(dataSet);
            
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
        }
        
        foreach (DataTable table in dataSet.Tables)
        {
            var tab = new TabItem();
            tab.Header = table.TableName;

            var dataGrid = new DataGrid();

            dataGrid.ItemsSource = table.AsDataView();

            tab.Content = dataGrid;

            Tabs.Items.Add(tab);
        }
       
    }
}
