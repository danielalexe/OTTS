﻿using OTTS_WPF.Helpers;
using DataLink;
using DataObjects;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.EntityClient;
using System.Linq;
using System.Reflection;
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
using System.Diagnostics;
using System.Data.Common;
using System.Data;
using System.IO;

namespace OTTS_WPF
{
    /// <summary>
    /// Interaction logic for LoginScreen.xaml
    /// </summary>
    public partial class LoginScreen : Window
    {
        private static int iDBVersion = 2;
        public LoginScreen()
        {
            InitializeComponent();
            BindComboDatabaseType();
            BindComboAuthenticationType();
            CTextVersion.CString = Assembly.GetEntryAssembly().GetName().Version.ToString();
            LabelCopyright.Content = ((AssemblyCopyrightAttribute)Assembly.GetEntryAssembly().GetCustomAttributes(typeof(AssemblyCopyrightAttribute), true).FirstOrDefault()).Copyright;
        }

        private void BindComboAuthenticationType()
        {
            List<DTOPlaceholderCombo> list = new List<DTOPlaceholderCombo>();
            var enumlist = Enum.GetValues(typeof(EnumAuthenticationType)).Cast<EnumAuthenticationType>();
            foreach (var item in enumlist)
            {
                DTOPlaceholderCombo dto = new DTOPlaceholderCombo();
                dto.nvCOMBO_DISPLAY = item.ToString();
                list.Add(dto);
            }
            CComboAuthenticationType.CComboBox.ItemsSource = list;
            CComboAuthenticationType.CComboBox.DropDownClosed += CComboAuthenticationType_DropDownClosed;
            CComboAuthenticationType.CComboBox.SelectedItem = list.FirstOrDefault();
        }
        private void BindComboDatabaseType()
        {
            List<DTOPlaceholderCombo> list = new List<DTOPlaceholderCombo>();
            var enumlist = Enum.GetValues(typeof(EnumDatabaseType)).Cast<EnumDatabaseType>();
            foreach (var item in enumlist)
            {
                DTOPlaceholderCombo dto = new DTOPlaceholderCombo();
                dto.nvCOMBO_DISPLAY = item.ToString();
                list.Add(dto);
            }
            CComboDatabaseType.CComboBox.ItemsSource = list;
            CComboDatabaseType.CComboBox.SelectedItem = list.FirstOrDefault();
        }

        private void CComboAuthenticationType_DropDownClosed(object sender, EventArgs e)
        {
            if (((DTOPlaceholderCombo)CComboAuthenticationType.CComboBox.SelectedItem).nvCOMBO_DISPLAY == EnumAuthenticationType.WindowsAuth.ToString())
            {
                CTextUsername.IsEnabled = false;
                CPasswordPassword.IsEnabled = false;
            }
            else
            {
                CTextUsername.IsEnabled = true;
                CPasswordPassword.IsEnabled = true;
            }
        }

        private void RadioOnline_Checked(object sender, RoutedEventArgs e)
        {
            EnableOnlineControls();
        }

        private void RadioOffline_Checked(object sender, RoutedEventArgs e)
        {
            //check if the screen is initialized when it is first shown
            if (CComboDatabaseType!=null)
            {
                DisableOnlineControls();
            }
        }

        private void EnableOnlineControls()
        {
            CComboDatabaseType.IsEnabled = true;
            CTextDatabaseServer.IsEnabled = true;
            CTextDatabaseName.IsEnabled = true;
            CComboAuthenticationType.IsEnabled = true;
            BindComboDatabaseType();
            BindComboAuthenticationType();
        }

        private void DisableOnlineControls()
        {
            CComboDatabaseType.IsEnabled = false;
            CTextDatabaseServer.IsEnabled = false;
            CTextDatabaseName.IsEnabled = false;
            CComboAuthenticationType.IsEnabled = false;
            CTextUsername.IsEnabled = false;
            CPasswordPassword.IsEnabled = false;
        }

        private void ButtonLogin_Click(object sender, RoutedEventArgs e)
        {
            if (RadioOffline.IsChecked==true)
            {
                DTOUser user = new DTOUser();
                user.iID_USER = 1;
                user.nvUSERNUME = "OfflineUser";
                PersistentData.LoggedUser = user;
                PersistentData.ConnectionString = PersistentData.GetConnectionString_Offline();
                using (var db = new EntityConnection(PersistentData.ConnectionString))
                {
                    try
                    {
                        db.Open();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                        if (MessageBox.Show("Este posibil ca versiunea de server local sa nu fie actualizata si de aceea modulul offline sa nu functioneze. Doriti actualizarea server-ului local?", "Atentie", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                        {
                            Process p = new Process();
                            p.StartInfo.FileName = "msiexec";
                            p.StartInfo.Arguments = string.Format("{0} {1}", "/i", @"Offline\SqlLocalDB.msi");
                            p.Start();
                            p.WaitForExit();
                            MessageBox.Show("Instalarea a avut loc cu succes. Va rugam reincercati sa accesati modulul offline.");
                        }
                        db.Close();
                        return;
                    }
                }
                using (var db = new OTTSContext(PersistentData.ConnectionString))
                {
                    var getDBVersion = db.SETTINGS.FirstOrDefault(z => z.iKEY == 1001 && z.bACTIVE == true);
                    if (getDBVersion!=null)
                    {
                        if (iDBVersion>getDBVersion.iVALUE)
                        {
                            /// Schema update is required
                            /// 
                            MessageBox.Show("O actualizare a schemei bazei de date exista. Actualizarea va fi aplicata si veti putea continua.");
                            UpdateSchemaFromVersion(getDBVersion.iVALUE);
                        }
                    }
                    else
                    {
                        /// first iteration of the DB
                        /// Apply all Schema updates.
                        /// 
                        MessageBox.Show("O actualizare a schemei bazei de date exista. Actualizarea va fi aplicata si veti putea continua.");
                        UpdateSchemaFromScratch();
                    }
                }
            }
            else
            {
                if (((DTOPlaceholderCombo)CComboDatabaseType.CComboBox.SelectedItem).nvCOMBO_DISPLAY==EnumDatabaseType.MicrosoftSQL.ToString())
                {
                    if (((DTOPlaceholderCombo)CComboAuthenticationType.CComboBox.SelectedItem).nvCOMBO_DISPLAY == EnumAuthenticationType.WindowsAuth.ToString())
                    {
                        PersistentData.DatabaseName = CTextDatabaseName.CString;
                        PersistentData.DatabaseServer = CTextDatabaseServer.CString;
                        PersistentData.DatabaseType = EnumDatabaseType.MicrosoftSQL;
                        PersistentData.AuthenticationType = EnumAuthenticationType.WindowsAuth;
                        PersistentData.ConnectionString = PersistentData.GetConnectionString_MSSQL();
                        using (var db = new EntityConnection(PersistentData.ConnectionString))
                        {
                            try
                            {
                                db.Open();
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message);
                                db.Close();
                                return;
                            }
                        }
                        using (var db = new OTTSContext(PersistentData.ConnectionString))
                        {
                            PersistentData.LoggedUser = new DTOUser();
                            PersistentData.LoggedUser.iID_USER = 1;
                            PersistentData.LoggedUser.nvUSERNUME = "Online";
                        }
                    }
                    else
                    {
                        PersistentData.UserName_SQL = CTextUsername.CString;
                        PersistentData.Password_SQL = CPasswordPassword.CPasswordBox.Password;
                        PersistentData.DatabaseName = CTextDatabaseName.CString;
                        PersistentData.DatabaseServer = CTextDatabaseServer.CString;
                        PersistentData.DatabaseType = EnumDatabaseType.MicrosoftSQL;
                        PersistentData.AuthenticationType = EnumAuthenticationType.SQLAuth;
                        PersistentData.ConnectionString = PersistentData.GetConnectionString_MSSQL();
                        using (var db = new EntityConnection(PersistentData.ConnectionString))
                        {
                            try
                            {
                                db.Open();
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message);
                                db.Close();
                                return;
                            }
                        }
                        using (var db = new OTTSContext(PersistentData.ConnectionString))
                        {
                            PersistentData.LoggedUser = new DTOUser();
                            PersistentData.LoggedUser.iID_USER = 1;
                            PersistentData.LoggedUser.nvUSERNUME = "Online";
                        }
                        
                    }
                }
                else
                {
                    MessageBox.Show("Only MicrosoftSQL database is supported in this version");
                    return;
                }
            }
            Hide();
            MainScreen mainmenu = new MainScreen();
            mainmenu.SourceScreen = this;
            mainmenu.Show();
        }

        private void ButtonExit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void UpdateSchemaFromScratch()
        {
            var Files = Directory.GetFiles("" + AppDomain.CurrentDomain.BaseDirectory + "" + "\\Offline\\Schemas\\");
            foreach (var item in Files)
            {
                var AllCommands = File.ReadAllText(item);
                string[] commands = AllCommands.Split(new string[] { "GO" }, StringSplitOptions.RemoveEmptyEntries);
                OTTSContext context = new OTTSContext(); //Instance new Context
                DbConnection conn = context.Database.Connection; // Get Database connection
                ConnectionState initialState = conn.State; // Get Initial connection state
                try
                {
                    if (initialState != ConnectionState.Open)
                        conn.Open();  // open connection if not already open

                    using (DbCommand cmd = conn.CreateCommand())
                    {
                        // Iterate the string array and execute each one.
                        foreach (string thisCommand in commands)
                        {
                            cmd.CommandText = thisCommand;
                            cmd.ExecuteNonQuery();
                        }
                    }
                }
                finally
                {
                    if (initialState != ConnectionState.Open)
                        conn.Close(); // only close connection if not initially open
                }
            }


        }

        private void UpdateSchemaFromVersion(int iVersion)
        {
            var AllFiles = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory + "\\Offline\\Schemas\\");
            List<string> Files = new List<string>();
            foreach (var item in AllFiles)
            {
                var Filename = System.IO.Path.GetFileNameWithoutExtension(item);
                var FileVersion = Convert.ToInt32(Filename[0]);
                if (FileVersion>=iVersion)
                {
                    Files.Add(item);
                }
            }
            
            foreach (var item in Files)
            {
                var AllCommands = File.ReadAllText(item);
                string[] commands = AllCommands.Split(new string[] { "GO" }, StringSplitOptions.RemoveEmptyEntries);
                OTTSContext context = new OTTSContext(); //Instance new Context
                DbConnection conn = context.Database.Connection; // Get Database connection
                ConnectionState initialState = conn.State; // Get Initial connection state
                try
                {
                    if (initialState != ConnectionState.Open)
                        conn.Open();  // open connection if not already open

                    using (DbCommand cmd = conn.CreateCommand())
                    {
                        // Iterate the string array and execute each one.
                        foreach (string thisCommand in commands)
                        {
                            cmd.CommandText = thisCommand;
                            cmd.ExecuteNonQuery();
                        }
                    }
                }
                finally
                {
                    if (initialState != ConnectionState.Open)
                        conn.Close(); // only close connection if not initially open
                }
            }


        }
    }
}