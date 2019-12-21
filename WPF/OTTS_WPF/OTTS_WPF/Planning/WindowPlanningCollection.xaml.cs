using DataLink;
using DataObjects;
using OTTS_WPF.Helpers;
using OTTS_WPF.Template;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Cryptography;
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
using System.Windows.Controls.Primitives;
using System.IO;

namespace OTTS_WPF.Planning
{
    /// <summary>
    /// Interaction logic for WindowPlanningCollection.xaml
    /// </summary>
    public partial class WindowPlanningCollection : WindowBase
    {
        public MainScreen MainScreen { get; set; }
        public WindowPlanningCollection()
        {
            InitializeComponent();
            BindComboSemigroup();
            BindComboGenerationNumber();
            ReloadData();
        }

        private void BindComboSemigroup()
        {
            using (var db = new OTTSContext(PersistentData.ConnectionString))
            {
                var getSemiGroups = (from u in db.SEMIGROUPS
                                     where u.bACTIVE==true
                                     select new DTOSemigroup
                                     {
                                         iID_SEMIGROUP = u.iID_SEMIGROUP,
                                         nvCOMBO_DISPLAY = u.nvNAME
                                     }).ToList();
                CComboSemigroup.CComboBox.ItemsSource = getSemiGroups;
                CComboSemigroup.CComboBox.SelectedItem = getSemiGroups.FirstOrDefault();
            }
        }

        public void BindComboGenerationNumber()
        {
            using (var db = new OTTSContext(PersistentData.ConnectionString))
            {
                int GenerationNumber = 0;
                var getSemester = db.SEMESTERS.FirstOrDefault(z => z.iID_SEMESTER == PersistentData.SelectedSemester && z.bACTIVE==true);
                if (getSemester != null)
                {
                    GenerationNumber = getSemester.iGENERATION_NUMBER;
                }
                if (GenerationNumber != 0)
                {
                    List<DTOGenerationNumber> list = new List<DTOGenerationNumber>();
                    for (int i = 1; i < GenerationNumber; i++)
                    {
                        DTOGenerationNumber dto = new DTOGenerationNumber();
                        dto.iGENERATION_NUMBER = i;
                        dto.nvCOMBO_DISPLAY = i.ToString();
                        var getFirstGeneratedItem = db.TIMETABLE_PLANNING.FirstOrDefault(z => z.iID_SEMESTER == PersistentData.SelectedSemester && z.bACTIVE == true && z.iGENERATION_NUMBER == i);
                        if (getFirstGeneratedItem!=null)
                        {
                            dto.nvCOMBO_DISPLAY = i.ToString()+" ("+getFirstGeneratedItem.dtCREATE_DATE.ToString("dd/MM/yyyy HH:mm:ss")+")";
                        }
                        list.Add(dto);
                    }

                    CComboGeneration.CComboBox.ItemsSource = list;
                    CComboGeneration.CComboBox.SelectedItem = list.FirstOrDefault();
                }
            }
        }

        private void ButonClose_Click(object sender, RoutedEventArgs e)
        {
            this.TabCtrl.Items.Remove(this.TabItem);
        }

        private void ButtonRefresh_Click(object sender, RoutedEventArgs e)
        {
            ReloadData();
        }

        List<DTODataGridCellsColors> listColors = new List<DTODataGridCellsColors>();

        public void ReloadData()
        {
            using (var db = new OTTSContext(PersistentData.ConnectionString))
            {
                if (CComboGeneration.CComboBox.SelectedItem == null || CComboSemigroup.CComboBox.SelectedItem ==null)
                {
                    return;
                }

                var FilterGenerationNumber = ((DTOGenerationNumber)CComboGeneration.CComboBox.SelectedItem).iGENERATION_NUMBER;
                var FilterSemigroup = ((DTOSemigroup)CComboSemigroup.CComboBox.SelectedItem).iID_SEMIGROUP;

                List<DTOPlanningRow> list = new List<DTOPlanningRow>();
                listColors.Clear();

                var getSemigroup = db.SEMIGROUPS.FirstOrDefault(z =>z.bACTIVE==true && z.iID_SEMIGROUP == FilterSemigroup);
                if (getSemigroup != null)
                {
                    var getDays = db.DAYS.ToList();
                    var getModules = db.GROUPS_MODULES_LINK.Where(z => z.bACTIVE == true && z.iID_GROUP == getSemigroup.iID_GROUP).ToList();

                    for (int i = 0; i < getModules.Count; i++)
                    {
                        DTOPlanningRow dto = new DTOPlanningRow();
                        dto.MODULE_NAME = getModules[i].MODULES.nvNAME;
                        list.Add(dto);
                    }

                    for (int i = 0; i < getModules.Count; i++)
                    {
                        for (int j = 0; j < getDays.Count; j++)
                        {
                            var idzi = getDays[j].iID_DAY;
                            var idmodul = getModules[i].iID_MODULE;

                            var getPlanificareZi = db.TIMETABLE_PLANNING.FirstOrDefault(z => z.bACTIVE == true &&
                            z.iID_SEMIGROUP == getSemigroup.iID_SEMIGROUP
                            &&
                            z.iGENERATION_NUMBER == FilterGenerationNumber
                            &&
                            z.iID_DAY == idzi
                            &&
                            z.iID_SEMESTER == PersistentData.SelectedSemester
                            &&
                            z.iID_MODULE == idmodul);
                            if (getPlanificareZi != null)
                            {
                                var Prelegere = getPlanificareZi.iID_LECTURE;
                                var Profesor = getPlanificareZi.iID_TEACHER;
                                var TipExecutie = getPlanificareZi.iID_LECTURE_TYPE;

                                var TextDeAfisat = "";
                                var getPrelegere = db.LECTURES.FirstOrDefault(z => z.bACTIVE == true && z.iID_LECTURE == Prelegere && z.iID_SEMESTER == PersistentData.SelectedSemester);
                                if (getPrelegere != null)
                                {
                                    TextDeAfisat += getPrelegere.nvNAME;
                                }
                                var getTipExecutie = db.LECTURE_TYPE.FirstOrDefault(z => z.bACTIVE == true && z.iID_LECTURE_TYPE == TipExecutie);
                                if (getTipExecutie != null)
                                {
                                    TextDeAfisat += " (" + getTipExecutie.nvNAME + ")";
                                }
                                var getProfesor = db.TEACHERS.FirstOrDefault(z => z.bACTIVE == true && z.iID_TEACHER == Profesor);
                                if (getProfesor != null)
                                {
                                    TextDeAfisat += " (" + getProfesor.nvSURNAME + " " + getProfesor.nvNAME + ")";
                                }
                                switch (j)
                                {
                                    case 0:
                                        list[i].MONDAY = TextDeAfisat;
                                        break;
                                    case 1:
                                        list[i].TUESDAY = TextDeAfisat;
                                        break;
                                    case 2:
                                        list[i].WEDNESDAY = TextDeAfisat;
                                        break;
                                    case 3:
                                        list[i].THURSDAY = TextDeAfisat;
                                        break;
                                    case 4:
                                        list[i].FRIDAY = TextDeAfisat;
                                        break;
                                    case 5:
                                        list[i].SATURDAY = TextDeAfisat;
                                        break;
                                    case 6:
                                        list[i].SUNDAY = TextDeAfisat;
                                        break;
                                    default:
                                        break;
                                }

                                if (getPlanificareZi.iID_PLANNING_TYPE == 1)
                                {
                                    DTODataGridCellsColors cellcolor = new DTODataGridCellsColors();
                                    cellcolor.iROW = i;
                                    cellcolor.iCOLUMN = j+1;
                                    cellcolor.CColor = Colors.Green;
                                    listColors.Add(cellcolor);
                                }
                                else
                                {
                                    DTODataGridCellsColors cellcolor = new DTODataGridCellsColors();
                                    cellcolor.iROW = i;
                                    cellcolor.iCOLUMN = j+1;
                                    cellcolor.CColor = Colors.Orange;
                                    listColors.Add(cellcolor);
                                }
                            }
                            else
                            {
                                //nu pune nimic
                            }
                        }
                    }
                    DataGridPlanning.ItemsSource = list;
                    RenderColumns();
                    DataGridPlanning.ItemContainerGenerator.StatusChanged += (sender, e) =>
                    {
                        DataGridPlanning.Dispatcher.Invoke(() =>
                        {
                            foreach (var item in listColors)
                            {
                                DataGridCell cell = GetCell(item.iROW, item.iCOLUMN, DataGridPlanning);
                                if (cell != null)
                                {
                                    cell.Foreground = new SolidColorBrush(item.CColor);
                                }
                            }
                        });
                    };
                }

            }
        }

        public DataGridCell GetCell(int rowIndex, int columnIndex, DataGrid dg)
        {
            DataGridRow row = dg.ItemContainerGenerator.ContainerFromIndex(rowIndex) as DataGridRow;
            if (row == null)
            {
                dg.UpdateLayout();
                dg.ScrollIntoView(dg.Items[rowIndex]);
                row = (DataGridRow)dg.ItemContainerGenerator.ContainerFromIndex(rowIndex);
            }
            if (row!=null)
            {
                DataGridCellsPresenter p = GetVisualChild<DataGridCellsPresenter>(row);
                DataGridCell cell = p.ItemContainerGenerator.ContainerFromIndex(columnIndex) as DataGridCell;
                return cell;
            }
            return null;
        }

        static T GetVisualChild<T>(Visual parent) where T : Visual
        {
            T child = default(T);
            int numVisuals = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < numVisuals; i++)
            {
                Visual v = (Visual)VisualTreeHelper.GetChild(parent, i);
                child = v as T;
                if (child == null)
                {
                    child = GetVisualChild<T>(v);
                }
                if (child != null)
                {
                    break;
                }
            }
            return child;
        }

        private void ButtonDelete_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Sunteti siguri ca vreti sa stergeti toate generarile efectuate pana acum?", "Atentie", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                using (var db = new OTTSContext(PersistentData.ConnectionString))
                {
                    db.Database.ExecuteSqlCommand("DELETE FROM TIMETABLE_PLANNING WHERE iID_SEMESTER = "+PersistentData.SelectedSemester);
                    var getSemester = db.SEMESTERS.FirstOrDefault(z => z.iID_SEMESTER == PersistentData.SelectedSemester && z.bACTIVE == true);
                    if (getSemester != null)
                    {
                        getSemester.iGENERATION_NUMBER = 1;
                        db.SaveChanges();
                    }
                }
                MessageBox.Show("Toate planificarile au fost sterse cu success");
                BindComboGenerationNumber();
                ReloadData();
            }
        }

        private void ButtonGenerate_Click(object sender, RoutedEventArgs e)
        {
            WindowPlanningEntity planningEntity = new WindowPlanningEntity();
            planningEntity.MainScreen = MainScreen;
            planningEntity.WindowType = Helpers.EnumWindowType.ADDMODE;
            planningEntity.WindowPlanningCollection = this;
            MainScreen.RaiseDownMenu(planningEntity, Helpers.EnumWindowType.ADDMODE);
        }


        private void RenderColumns()
        {
            foreach (DataGridColumn c in DataGridPlanning.Columns)
            {
                if (c.Header.ToString().StartsWith("iID_") || c.Header.ToString().StartsWith("nvPASSWORD") || c.Header.ToString().StartsWith("nvCOMBO_DISPLAY"))
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
            ReloadData();
        }

        private void ButtonExportPdf_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ButtonExportExcel_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ButtonExportHtml_Click(object sender, RoutedEventArgs e)
        {
            if (CComboGeneration.CComboBox.SelectedItem == null)
            {
                return;
            }

            var FilterGenerationNumber = ((DTOGenerationNumber)CComboGeneration.CComboBox.SelectedItem).iGENERATION_NUMBER;

            List<DTOPlanningExport> listexport = new List<DTOPlanningExport>();

            using (var db = new OTTSContext(PersistentData.ConnectionString))
            {
                var getSemigroups = db.SEMIGROUPS.Where(z => z.bACTIVE == true).ToList();
                foreach(var getSemigroup in getSemigroups)
                {
                    DTOPlanningExport dtoexport = new DTOPlanningExport();
                    dtoexport.iID_GROUP = getSemigroup.iID_GROUP;
                    dtoexport.iID_SEMIGROUP = getSemigroup.iID_SEMIGROUP;
                    dtoexport.nvSEMIGROUP_NAME = getSemigroup.nvNAME;
                    dtoexport.iID_GROUP_TYPE = getSemigroup.GROUPS.iID_GROUP_TYPE;
                    dtoexport.iYEAR = getSemigroup.GROUPS.iYEAR;
                    

                    List<DTOPlanningRow> list = new List<DTOPlanningRow>();

                    var getDays = db.DAYS.ToList();
                    var getModules = db.GROUPS_MODULES_LINK.Where(z => z.bACTIVE == true && z.iID_GROUP == getSemigroup.iID_GROUP).ToList();

                    for (int i = 0; i < getModules.Count; i++)
                    {
                        DTOPlanningRow dto = new DTOPlanningRow();
                        dto.MODULE_NAME = getModules[i].MODULES.nvNAME;
                        list.Add(dto);
                    }

                    for (int i = 0; i < getModules.Count; i++)
                    {
                        for (int j = 0; j < getDays.Count; j++)
                        {
                            var idzi = getDays[j].iID_DAY;
                            var idmodul = getModules[i].iID_MODULE;

                            var getPlanificareZi = db.TIMETABLE_PLANNING.FirstOrDefault(z => z.bACTIVE == true &&
                            z.iID_SEMIGROUP == getSemigroup.iID_SEMIGROUP
                            &&
                            z.iGENERATION_NUMBER == FilterGenerationNumber
                            &&
                            z.iID_DAY == idzi
                            &&
                            z.iID_SEMESTER == PersistentData.SelectedSemester
                            &&
                            z.iID_MODULE == idmodul);
                            if (getPlanificareZi != null)
                            {
                                var Prelegere = getPlanificareZi.iID_LECTURE;
                                var Profesor = getPlanificareZi.iID_TEACHER;
                                var TipExecutie = getPlanificareZi.iID_LECTURE_TYPE;

                                var TextDeAfisat = "";
                                var getPrelegere = db.LECTURES.FirstOrDefault(z => z.bACTIVE == true && z.iID_LECTURE == Prelegere && z.iID_SEMESTER == PersistentData.SelectedSemester);
                                if (getPrelegere != null)
                                {
                                    TextDeAfisat += getPrelegere.nvNAME;
                                }
                                var getTipExecutie = db.LECTURE_TYPE.FirstOrDefault(z => z.bACTIVE == true && z.iID_LECTURE_TYPE == TipExecutie);
                                if (getTipExecutie != null)
                                {
                                    TextDeAfisat += " (" + getTipExecutie.nvNAME + ")";
                                }
                                var getProfesor = db.TEACHERS.FirstOrDefault(z => z.bACTIVE == true && z.iID_TEACHER == Profesor);
                                if (getProfesor != null)
                                {
                                    TextDeAfisat += " (" + getProfesor.nvSURNAME + " " + getProfesor.nvNAME + ")";
                                }
                                switch (j)
                                {
                                    case 0:
                                        list[i].MONDAY = TextDeAfisat;
                                        break;
                                    case 1:
                                        list[i].TUESDAY = TextDeAfisat;
                                        break;
                                    case 2:
                                        list[i].WEDNESDAY = TextDeAfisat;
                                        break;
                                    case 3:
                                        list[i].THURSDAY = TextDeAfisat;
                                        break;
                                    case 4:
                                        list[i].FRIDAY = TextDeAfisat;
                                        break;
                                    case 5:
                                        list[i].SATURDAY = TextDeAfisat;
                                        break;
                                    case 6:
                                        list[i].SUNDAY = TextDeAfisat;
                                        break;
                                    default:
                                        break;
                                }
                            }
                            else
                            {
                                //nu pune nimic
                            }
                        }
                    }

                    dtoexport.lLIST_EXPORT = list;
                    listexport.Add(dtoexport);
                }
            }

            var ByGroup = listexport.GroupBy(z => z.iID_GROUP).ToList();

            var HTMLBIG = "";
            HTMLBIG += "<html>";
            HTMLBIG += "<head></head>";
            HTMLBIG += "<body>";

            foreach (var item in ByGroup)
            {
                if (item.Count()==1)
                {
                    var TheSemiGroup = item.FirstOrDefault();
                    //single semigroup
                    
                    HTMLBIG += "<p align=\"center\">Facultatea Litere și Științe</p>";
                    HTMLBIG += "<p align=\"center\">2019-2020</p>";
                    if (TheSemiGroup.iID_GROUP_TYPE==1)
                    {
                        HTMLBIG += "<p align=\"center\">Specializarea INFORMATICĂ</p>";
                    }
                    else
                    {
                        HTMLBIG += "<p align=\"center\">Specializarea TAPI</p>";
                    }
                    HTMLBIG += "<p align=\"center\">Anul " + TheSemiGroup.iYEAR.ToString()+"</p>";
                    HTMLBIG += "<p align=\"center\">Semestrul " + PersistentData.SelectedSemester.ToString()+"</p>";

                    HTMLBIG += "<table style=\"width: 100 %;border: 3px solid black; \" align=\"center\">";

                    HTMLBIG += "<tr>";
                    HTMLBIG += "<th align=\"center\">";
                    HTMLBIG += "Zi";
                    HTMLBIG += "</th>";
                    HTMLBIG += "<th align=\"center\">";
                    HTMLBIG += "Număr modul";
                    HTMLBIG += "</th>";
                    HTMLBIG += "<th align=\"center\">";
                    HTMLBIG += "Interval Orar";
                    HTMLBIG += "</th>";
                    HTMLBIG += "<th align=\"center\">";
                    HTMLBIG += TheSemiGroup.nvSEMIGROUP_NAME;
                    HTMLBIG += "</th>";
                    HTMLBIG += "</tr>";

                    foreach (var module in TheSemiGroup.lLIST_EXPORT)
                    {
                        HTMLBIG += "<tr>";
                        if (module == TheSemiGroup.lLIST_EXPORT[0])
                        {
                            HTMLBIG += "<td style=\"border: 1px solid black;\" rowspan=\"" + TheSemiGroup.lLIST_EXPORT.Count + "\" align=\"center\">";
                            HTMLBIG += "Luni";
                            HTMLBIG += "</td>";
                        }
                        else
                        {

                        }
                        HTMLBIG += "<td align=\"center\" style=\"border: 1px solid black;\">";
                        HTMLBIG += module.MODULE_NAME;
                        HTMLBIG += "</td>";
                        HTMLBIG += "<td align=\"center\" style=\"border: 1px solid black;\">";
                        HTMLBIG += Helpers.HelperModules.GetModuleInterval(module.MODULE_NAME);
                        HTMLBIG += "</td>";
                        HTMLBIG += "<td align=\"center\" style=\"border: 1px solid black;\">";
                        HTMLBIG += module.MONDAY;
                        HTMLBIG += "</td>";
                        HTMLBIG += "</tr>";
                    }
                    foreach (var module in TheSemiGroup.lLIST_EXPORT)
                    {
                        HTMLBIG += "<tr>";
                        if (module == TheSemiGroup.lLIST_EXPORT[0])
                        {
                            HTMLBIG += "<td style=\"border: 1px solid black;\" rowspan=\"" + TheSemiGroup.lLIST_EXPORT.Count + "\" align=\"center\">";
                            HTMLBIG += "Marti";
                            HTMLBIG += "</td>";
                        }
                        else
                        {

                        }
                        HTMLBIG += "<td align=\"center\" style=\"border: 1px solid black;\">";
                        HTMLBIG += module.MODULE_NAME;
                        HTMLBIG += "</td>";
                        HTMLBIG += "<td align=\"center\" style=\"border: 1px solid black;\">";
                        HTMLBIG += Helpers.HelperModules.GetModuleInterval(module.MODULE_NAME);
                        HTMLBIG += "</td>";
                        HTMLBIG += "<td align=\"center\" style=\"border: 1px solid black;\">";
                        HTMLBIG += module.TUESDAY;
                        HTMLBIG += "</td>";
                        HTMLBIG += "</tr>";
                    }
                    foreach (var module in TheSemiGroup.lLIST_EXPORT)
                    {
                        HTMLBIG += "<tr>";
                        if (module == TheSemiGroup.lLIST_EXPORT[0])
                        {
                            HTMLBIG += "<td style=\"border: 1px solid black;\" rowspan=\"" + TheSemiGroup.lLIST_EXPORT.Count + "\" align=\"center\">";
                            HTMLBIG += "Miercuri";
                            HTMLBIG += "</td>";
                        }
                        else
                        {

                        }
                        HTMLBIG += "<td align=\"center\" style=\"border: 1px solid black;\">";
                        HTMLBIG += module.MODULE_NAME;
                        HTMLBIG += "</td>";
                        HTMLBIG += "<td align=\"center\" style=\"border: 1px solid black;\">";
                        HTMLBIG += Helpers.HelperModules.GetModuleInterval(module.MODULE_NAME);
                        HTMLBIG += "</td>";
                        HTMLBIG += "<td align=\"center\" style=\"border: 1px solid black;\">";
                        HTMLBIG += module.WEDNESDAY;
                        HTMLBIG += "</td>";
                        HTMLBIG += "</tr>";
                    }
                    foreach (var module in TheSemiGroup.lLIST_EXPORT)
                    {
                        HTMLBIG += "<tr>";
                        if (module == TheSemiGroup.lLIST_EXPORT[0])
                        {
                            HTMLBIG += "<td style=\"border: 1px solid black;\" rowspan=\"" + TheSemiGroup.lLIST_EXPORT.Count + "\" align=\"center\">";
                            HTMLBIG += "Joi";
                            HTMLBIG += "</td>";
                        }
                        else
                        {

                        }
                        HTMLBIG += "<td align=\"center\" style=\"border: 1px solid black;\">";
                        HTMLBIG += module.MODULE_NAME;
                        HTMLBIG += "</td>";
                        HTMLBIG += "<td align=\"center\" style=\"border: 1px solid black;\">";
                        HTMLBIG += Helpers.HelperModules.GetModuleInterval(module.MODULE_NAME);
                        HTMLBIG += "</td>";
                        HTMLBIG += "<td align=\"center\" style=\"border: 1px solid black;\">";
                        HTMLBIG += module.THURSDAY;
                        HTMLBIG += "</td>";
                        HTMLBIG += "</tr>";
                    }
                    foreach (var module in TheSemiGroup.lLIST_EXPORT)
                    {
                        HTMLBIG += "<tr>";
                        if (module == TheSemiGroup.lLIST_EXPORT[0])
                        {
                            HTMLBIG += "<td style=\"border: 1px solid black;\" rowspan=\"" + TheSemiGroup.lLIST_EXPORT.Count + "\" align=\"center\">";
                            HTMLBIG += "Vineri";
                            HTMLBIG += "</td>";
                        }
                        else
                        {

                        }
                        HTMLBIG += "<td align=\"center\" style=\"border: 1px solid black;\">";
                        HTMLBIG += module.MODULE_NAME;
                        HTMLBIG += "</td>";
                        HTMLBIG += "<td align=\"center\" style=\"border: 1px solid black;\">";
                        HTMLBIG += Helpers.HelperModules.GetModuleInterval(module.MODULE_NAME);
                        HTMLBIG += "</td>";
                        HTMLBIG += "<td align=\"center\" style=\"border: 1px solid black;\">";
                        HTMLBIG += module.FRIDAY;
                        HTMLBIG += "</td>";
                        HTMLBIG += "</tr>";
                    }

                    HTMLBIG += "</table>";
                    HTMLBIG += "<br><br>";
                }
                else
                {
                    //multiple semigroups
                    var OrderedByName = item.OrderBy(z => z.nvSEMIGROUP_NAME).ToList();

                    HTMLBIG += "<p align=\"center\">Facultatea Litere și Științe</p>";
                    HTMLBIG += "<p align=\"center\">2019-2020</p>";
                    if (OrderedByName.FirstOrDefault().iID_GROUP_TYPE == 1)
                    {
                        HTMLBIG += "<p align=\"center\">Specializarea INFORMATICĂ</p>";
                    }
                    else
                    {
                        HTMLBIG += "<p align=\"center\">Specializarea TAPI</p>";
                    }
                    HTMLBIG += "<p align=\"center\">Anul " + OrderedByName.FirstOrDefault().iYEAR.ToString() + "</p>";
                    HTMLBIG += "<p align=\"center\">Semestrul " + PersistentData.SelectedSemester.ToString() + "</p>";

                    HTMLBIG += "<table style=\"width: 100 %;border: 3px solid black; \" align=\"center\">";

                    HTMLBIG += "<tr>";
                    HTMLBIG += "<th align=\"center\">";
                    HTMLBIG += "Zi";
                    HTMLBIG += "</th>";
                    HTMLBIG += "<th align=\"center\">";
                    HTMLBIG += "Număr modul";
                    HTMLBIG += "</th>";
                    HTMLBIG += "<th align=\"center\">";
                    HTMLBIG += "Interval Orar";
                    HTMLBIG += "</th>";
                    foreach (var semigroup in OrderedByName)
                    {
                        HTMLBIG += "<th align=\"center\">";
                        HTMLBIG += semigroup.nvSEMIGROUP_NAME;
                        HTMLBIG += "</th>";
                    }
                    HTMLBIG += "</tr>";


                    List<MODULES> listmodules = new List<MODULES>();

                    using (var db = new OTTSContext(PersistentData.ConnectionString))
                    {
                        var getModules = db.MODULES.Where(z => z.bACTIVE == true).ToList();
                        listmodules = getModules;
                    }

                    foreach (var module in listmodules)
                    {
                        //if (module==listmodules[listmodules.Count-1])
                        //{
                        //    HTMLBIG += "<tr style=\"border-bottom: 3px solid;\">";
                        //}
                        //else
                        {
                            HTMLBIG += "<tr>";
                        }
                        if (module==listmodules[0])
                        {
                            HTMLBIG += "<td style=\"border: 1px solid black;\" rowspan=\"" + listmodules.Count + "\" align=\"center\">";
                            HTMLBIG += "Luni";
                            HTMLBIG += "</td>";
                        }
                        else
                        {

                        }
                        HTMLBIG += "<td align=\"center\" style=\"border: 1px solid black;\">";
                        HTMLBIG += module.nvNAME;
                        HTMLBIG += "</td>";
                        HTMLBIG += "<td align=\"center\" style=\"border: 1px solid black;\">";
                        HTMLBIG += Helpers.HelperModules.GetModuleInterval(module.nvNAME);
                        HTMLBIG += "</td>";

                        List<string> templist = new List<string>();
                        foreach (var stuff in OrderedByName)
                        {
                            var getStuff = stuff.lLIST_EXPORT.FirstOrDefault(z => z.MODULE_NAME == module.nvNAME);
                            templist.Add(getStuff.MONDAY);
                        }

                        var DistinctCount = templist.Distinct().ToList().Count;
                        if (DistinctCount==templist.Count)
                        {
                            foreach (var getStuff in templist)
                            {
                                HTMLBIG += "<td style=\"border: 1px solid black;\">";
                                HTMLBIG += getStuff;
                                HTMLBIG += "</td>";
                            }
                        }
                        else
                        {
                            HTMLBIG += "<td style=\"border: 1px solid black;\" colspan=\"" + templist.Count + "\" align=\"center\">";
                            HTMLBIG += templist.FirstOrDefault();
                            HTMLBIG += "</td>";
                        }
                        HTMLBIG += "</tr>";
                    }
                    foreach (var module in listmodules)
                    {
                        HTMLBIG += "<tr>";
                        if (module == listmodules[0])
                        {
                            HTMLBIG += "<td style=\"border: 1px solid black;\" rowspan=\"" + listmodules.Count + "\" align=\"center\">";
                            HTMLBIG += "Marți";
                            HTMLBIG += "</td>";
                        }
                        else
                        {

                        }                        
                        HTMLBIG += "<td align=\"center\" style=\"border: 1px solid black;\">";
                        HTMLBIG += module.nvNAME;
                        HTMLBIG += "</td>";
                        HTMLBIG += "<td align=\"center\" style=\"border: 1px solid black;\">";
                        HTMLBIG += Helpers.HelperModules.GetModuleInterval(module.nvNAME);
                        HTMLBIG += "</td>";

                        List<string> templist = new List<string>();
                        foreach (var stuff in OrderedByName)
                        {
                            var getStuff = stuff.lLIST_EXPORT.FirstOrDefault(z => z.MODULE_NAME == module.nvNAME);
                            templist.Add(getStuff.TUESDAY);
                        }

                        var DistinctCount = templist.Distinct().ToList().Count;
                        if (DistinctCount == templist.Count)
                        {
                            foreach (var getStuff in templist)
                            {
                                HTMLBIG += "<td style=\"border: 1px solid black;\">";
                                HTMLBIG += getStuff;
                                HTMLBIG += "</td>";
                            }
                        }
                        else
                        {
                            HTMLBIG += "<td style=\"border: 1px solid black;\" colspan=\"" + templist.Count + "\" align=\"center\">";
                            HTMLBIG += templist.FirstOrDefault();
                            HTMLBIG += "</td>";
                        }
                        HTMLBIG += "</tr>";
                    }
                    foreach (var module in listmodules)
                    {
                        HTMLBIG += "<tr>";
                        if (module == listmodules[0])
                        {
                            HTMLBIG += "<td style=\"border: 1px solid black;\" rowspan=\"" + listmodules.Count + "\" align=\"center\">";
                            HTMLBIG += "Miercuri";
                            HTMLBIG += "</td>";
                        }
                        else
                        {

                        }                        
                        HTMLBIG += "<td align=\"center\" style=\"border: 1px solid black;\">";
                        HTMLBIG += module.nvNAME;
                        HTMLBIG += "</td>";
                        HTMLBIG += "<td align=\"center\" style=\"border: 1px solid black;\">";
                        HTMLBIG += Helpers.HelperModules.GetModuleInterval(module.nvNAME);
                        HTMLBIG += "</td>";

                        List<string> templist = new List<string>();
                        foreach (var stuff in OrderedByName)
                        {
                            var getStuff = stuff.lLIST_EXPORT.FirstOrDefault(z => z.MODULE_NAME == module.nvNAME);
                            templist.Add(getStuff.WEDNESDAY);
                        }

                        var DistinctCount = templist.Distinct().ToList().Count;
                        if (DistinctCount == templist.Count)
                        {
                            foreach (var getStuff in templist)
                            {
                                HTMLBIG += "<td style=\"border: 1px solid black;\">";
                                HTMLBIG += getStuff;
                                HTMLBIG += "</td>";
                            }
                        }
                        else
                        {
                            HTMLBIG += "<td style=\"border: 1px solid black;\" colspan=\"" + templist.Count + "\" align=\"center\">";
                            HTMLBIG += templist.FirstOrDefault();
                            HTMLBIG += "</td>";
                        }
                        HTMLBIG += "</tr>";
                    }
                    foreach (var module in listmodules)
                    {
                        HTMLBIG += "<tr>";
                        if (module == listmodules[0])
                        {
                            HTMLBIG += "<td style=\"border: 1px solid black;\" rowspan=\"" + listmodules.Count + "\" align=\"center\">";
                            HTMLBIG += "Joi";
                            HTMLBIG += "</td>";
                        }
                        else
                        {

                        }                        
                        HTMLBIG += "<td align=\"center\" style=\"border: 1px solid black;\">";
                        HTMLBIG += module.nvNAME;
                        HTMLBIG += "</td>";
                        HTMLBIG += "<td align=\"center\" style=\"border: 1px solid black;\">";
                        HTMLBIG += Helpers.HelperModules.GetModuleInterval(module.nvNAME);
                        HTMLBIG += "</td>";

                        List<string> templist = new List<string>();
                        foreach (var stuff in OrderedByName)
                        {
                            var getStuff = stuff.lLIST_EXPORT.FirstOrDefault(z => z.MODULE_NAME == module.nvNAME);
                            templist.Add(getStuff.THURSDAY);
                        }

                        var DistinctCount = templist.Distinct().ToList().Count;
                        if (DistinctCount == templist.Count)
                        {
                            foreach (var getStuff in templist)
                            {
                                HTMLBIG += "<td style=\"border: 1px solid black;\">";
                                HTMLBIG += getStuff;
                                HTMLBIG += "</td>";
                            }
                        }
                        else
                        {
                            HTMLBIG += "<td style=\"border: 1px solid black;\" colspan=\"" + templist.Count + "\" align=\"center\">";
                            HTMLBIG += templist.FirstOrDefault();
                            HTMLBIG += "</td>";
                        }
                        HTMLBIG += "</tr>";
                    }
                    foreach (var module in listmodules)
                    {
                        HTMLBIG += "<tr>";
                        if (module == listmodules[0])
                        {
                            HTMLBIG += "<td style=\"border: 1px solid black;\" rowspan=\"" + listmodules.Count + "\" align=\"center\">";
                            HTMLBIG += "Vineri";
                            HTMLBIG += "</td>";
                        }
                        else
                        {

                        }                        
                        HTMLBIG += "<td align=\"center\" style=\"border: 1px solid black;\">";
                        HTMLBIG += module.nvNAME;
                        HTMLBIG += "</td>";
                        HTMLBIG += "<td align=\"center\" style=\"border: 1px solid black;\">";
                        HTMLBIG += Helpers.HelperModules.GetModuleInterval(module.nvNAME);
                        HTMLBIG += "</td>";


                        List<string> templist = new List<string>();
                        foreach (var stuff in OrderedByName)
                        {
                            var getStuff = stuff.lLIST_EXPORT.FirstOrDefault(z => z.MODULE_NAME == module.nvNAME);
                            templist.Add(getStuff.FRIDAY);
                        }

                        var DistinctCount = templist.Distinct().ToList().Count;
                        if (DistinctCount == templist.Count)
                        {
                            foreach (var getStuff in templist)
                            {
                                HTMLBIG += "<td style=\"border: 1px solid black;\">";
                                HTMLBIG += getStuff;
                                HTMLBIG += "</td>";
                            }
                        }
                        else
                        {
                            HTMLBIG += "<td style=\"border: 1px solid black;\" colspan=\"" + templist.Count + "\" align=\"center\">";
                            HTMLBIG += templist.FirstOrDefault();
                            HTMLBIG += "</td>";
                        }

                        HTMLBIG += "</tr>";
                    }
                    HTMLBIG += "</table>";
                    HTMLBIG += "<br><br>";

                }
            }

            HTMLBIG += "</body>";
            HTMLBIG += "</html>";

            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.FileName = "ExportPlanificareOrar"; // Default file name
            dlg.DefaultExt = ".html"; // Default file extension
            dlg.Filter = "Html documents (.html)|*.html"; // Filter files by extension

            // Show save file dialog box
            Nullable<bool> result = dlg.ShowDialog();

            // Process save file dialog box results
            if (result == true)
            {
                // Save document
                string filename = dlg.FileName;
                bool exists = System.IO.Directory.Exists(System.IO.Path.GetDirectoryName(filename));
                if (!exists)
                    System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(filename));
                using (StreamWriter w = new StreamWriter(filename, true))
                {
                    w.Write(HTMLBIG);
                }
            }
            MessageBox.Show("Fisierul a fost exportat cu succes");
        }
    }
}
