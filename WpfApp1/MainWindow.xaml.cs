using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
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

namespace WpfApp1;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private const string kapcsolatLeiro = "datasource=127.0.0.1;port=3306;username=root;password=;database=hardver;charset=utf8";
    ObservableCollection<Termek> termekek = new();
    MySqlConnection SQLkapcsolat;

    public MainWindow()
    {
        InitializeComponent();

        dgTermekek.ItemsSource = termekek;

        AdatbazisMegnyitas();
        KategoriakBetoltese();
        GyartokBetoltese();

        TermekekBetolteseListaba();

        AdatbazisLezarasa();
    }

    private void btnMentes_Click(object sender, RoutedEventArgs e)
    {
        AdatbazisMegnyitas();
        StreamWriter sw = new("blabla.csv");

        foreach(Termek termek in termekek)
        {
            sw.WriteLine(termek.ToCSVString());
        }
        sw.Close();
    }

    private void btnSzukit_Click(object sender, RoutedEventArgs e)
    {
        AdatbazisMegnyitas();
        termekek.Clear();
        string SQLSzukitettLista = SzukitoLekerdezesEloallitasa();
        MySqlCommand SQLparancs = new(SQLSzukitettLista, SQLkapcsolat);
        MySqlDataReader eredmenyOlvaso = SQLparancs.ExecuteReader();

        while (eredmenyOlvaso.Read())
            termekek.Add(new(eredmenyOlvaso));

        eredmenyOlvaso.Close();
    }

    private void TermekekBetolteseListaba()
    {
        string SQLOsszesTermek = "SELECT * FROM termékek;";
        MySqlCommand SQLparancs = new(SQLOsszesTermek, SQLkapcsolat);
        MySqlDataReader eredmenyOlvaso = SQLparancs.ExecuteReader();

        while (eredmenyOlvaso.Read())
            termekek.Add(new(eredmenyOlvaso));

        eredmenyOlvaso.Close();
    }

    private void KategoriakBetoltese()
    {
        string SQLKategoriakRendezve = "SELECT DISTINCT kategória FROM termékek ORDER BY 1;";
        MySqlCommand SQLparancs = new(SQLKategoriakRendezve, SQLkapcsolat);
        MySqlDataReader eredmenyOlvaso = SQLparancs.ExecuteReader();

        cbKategoria.Items.Add(" - Nincs megadva - ");
        while (eredmenyOlvaso.Read())
            cbKategoria.Items.Add(eredmenyOlvaso.GetString("Kategória"));

        eredmenyOlvaso.Close();
        cbKategoria.SelectedIndex = 0;
    }

    private void GyartokBetoltese()
    {
        string SQLKategoriakRendezve = "SELECT DISTINCT gyártó FROM termékek ORDER BY 1;";
        MySqlCommand SQLparancs = new(SQLKategoriakRendezve, SQLkapcsolat);
        MySqlDataReader eredmenyOlvaso = SQLparancs.ExecuteReader();

        cbGyarto.Items.Add(" - Nincs megadva - ");
        while (eredmenyOlvaso.Read())
            cbGyarto.Items.Add(eredmenyOlvaso.GetString("Gyártó"));

        eredmenyOlvaso.Close();
        cbGyarto.SelectedIndex = 0;
    }

    private void AdatbazisMegnyitas()
    {
        try
        {
            SQLkapcsolat = new(kapcsolatLeiro);
            SQLkapcsolat.Open();
        }
        catch(Exception e)
        {
            MessageBox.Show(e.Message);
            this.Close();
        }
    }

    private void AdatbazisLezarasa()
    {
        SQLkapcsolat.Close();
        SQLkapcsolat.Dispose();
    }

    private string SzukitoLekerdezesEloallitasa() => $"SELECT * FROM termékek WHERE { (cbKategoria.SelectedIndex > 0 ? $"Kategória LIKE '{cbKategoria.SelectedValue}'" : "true") } and {(cbGyarto.SelectedIndex > 0 ? $"Gyártó LIKE '{cbGyarto.SelectedValue}'" : "true")};";
}