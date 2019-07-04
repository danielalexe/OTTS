﻿using ARPC_WPF.Data;
using ARPC_WPF.Template;
using DataObjects;
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
using System.Windows.Shapes;

namespace ARPC_WPF.Profesori
{
    /// <summary>
    /// Interaction logic for WindowProfesoriColectie.xaml
    /// </summary>
    public partial class WindowProfesoriColectie : WindowBase
    {
        public WindowProfesoriColectie()
        {
            InitializeComponent();
            ReloadData();
        }

        private void ButonClose_Click(object sender, RoutedEventArgs e)
        {
            this.TabCtrl.Items.Remove(this.TabItem);
        }

        private void ButtonRefresh_Click(object sender, RoutedEventArgs e)
        {
            ReloadData();
        }

        private void ReloadData()
        {
            using (var db = new ARPCContext())
            {
                var getProfesori = (from u in db.PROFESORIs
                                    select new DTOProfesori
                                    {
                                        ID_PROFESOR = u.ID_PROFESOR,
                                        NUME = u.NUME,
                                        PRENUME = u.PRENUME
                                    }).ToList();
                DataGridProfesori.ItemsSource = getProfesori;
            }
        }

        private void ButtonDelete_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ButtonEdit_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ButtonAdd_Click(object sender, RoutedEventArgs e)
        {

        }


        private void RenderColumns()
        {
            foreach (DataGridColumn c in DataGridProfesori.Columns)
            {
                if (c.Header.ToString().StartsWith("ID_") || c.Header.ToString().StartsWith("PASSWORD") || c.Header.ToString().StartsWith("PAROLA"))
                    c.Visibility = Visibility.Collapsed;
                String aux = c.Header.ToString().ToLower();
                c.Header = ReplaceFirstCharacterFromString(aux);
            }
        }

        public string ReplaceFirstCharacterFromString(string text)
        {
            String aux = String.Format("{0}{1}", text.First().ToString().ToUpperInvariant(), text.Substring(1));
            return aux.Replace('_', ' ');
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            RenderColumns();
        }
    }
}
