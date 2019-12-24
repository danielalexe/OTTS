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
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Controls.Primitives;
using System.IO;
using MigraDoc.DocumentObjectModel;
using MigraDoc.Rendering;
using MigraDoc.DocumentObjectModel.Tables;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;

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
            CComboGeneration.CComboBox.SelectionChanged += CComboBoxGeneration_SelectionChanged;
            CComboSemigroup.CComboBox.SelectionChanged += CComboBoxSemigroup_SelectionChanged;
        }

        private void CComboBoxSemigroup_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ReloadData();
        }

        private void CComboBoxGeneration_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
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
                                    cellcolor.CColor = System.Windows.Media.Colors.Green;
                                    listColors.Add(cellcolor);
                                }
                                else
                                {
                                    DTODataGridCellsColors cellcolor = new DTODataGridCellsColors();
                                    cellcolor.iROW = i;
                                    cellcolor.iCOLUMN = j+1;
                                    cellcolor.CColor = System.Windows.Media.Colors.Orange;
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

        private static MigraDoc.DocumentObjectModel.Color TableAccent = new MigraDoc.DocumentObjectModel.Color(255, 255, 204);
        private static MigraDoc.DocumentObjectModel.Color TableBorder = new MigraDoc.DocumentObjectModel.Color(0, 0, 0);
        private static MigraDoc.DocumentObjectModel.Color TableWhite = new MigraDoc.DocumentObjectModel.Color(255, 255, 255);



        private void ButtonExportPdf_Click(object sender, RoutedEventArgs e)
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
                foreach (var getSemigroup in getSemigroups)
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

            var ByGroup = listexport.GroupBy(z => new { z.iID_GROUP_TYPE,z.iYEAR}).ToList();

            Document document = new Document();
            document.Info.Title = "OTTS - Planning Export";
            document.Info.Subject = "";
            document.Info.Author = "OTTS";
            document.DefaultPageSetup.Orientation = MigraDoc.DocumentObjectModel.Orientation.Portrait;

            // Get the predefined style Normal.
            MigraDoc.DocumentObjectModel.Style style = document.Styles["Normal"];
            // Because all styles are derived from Normal, the next line changes the 
            // font of the whole document. Or, more exactly, it changes the font of
            // all styles and paragraphs that do not redefine the font.
            style.Font.Name = "Verdana";

            style = document.Styles[StyleNames.Header];
            style.ParagraphFormat.AddTabStop("16cm", TabAlignment.Right);

            style = document.Styles[StyleNames.Footer];
            style.ParagraphFormat.AddTabStop("8cm", TabAlignment.Center);

            // Create a new style called Table based on style Normal
            style = document.Styles.AddStyle("Table", "Normal");
            style.Font.Name = "Verdana";
            style.Font.Name = "Times New Roman";
            style.Font.Size = 9;

            // Create a new style called Reference based on style Normal
            style = document.Styles.AddStyle("Reference", "Normal");
            style.ParagraphFormat.SpaceBefore = "5mm";
            style.ParagraphFormat.SpaceAfter = "5mm";
            style.ParagraphFormat.TabStops.AddTabStop("16cm", TabAlignment.Right);

            foreach (var item in ByGroup)
            {
                if (item.Count() == 1)
                {
                    var TheSemiGroup = item.FirstOrDefault();
                    //single semigroup

                    // Each MigraDoc document needs at least one section.
                    MigraDoc.DocumentObjectModel.Section section = document.AddSection();
                    section.PageSetup.LeftMargin = Unit.FromCentimeter(0.8);
                    section.PageSetup.RightMargin = Unit.FromCentimeter(2.3);
                    section.PageSetup.TopMargin = Unit.FromCentimeter(1.5);
                    section.PageSetup.BottomMargin = Unit.FromCentimeter(1.8);

                    MigraDoc.DocumentObjectModel.Tables.Table table = new MigraDoc.DocumentObjectModel.Tables.Table();
                    table = document.LastSection.AddTable();
                    table.Style = "Table";

                    // Before you can add a row, you must define the columns
                    MigraDoc.DocumentObjectModel.Tables.Column column;
                    column = table.AddColumn(Unit.FromCentimeter(9.5));
                    column.Format.Alignment = ParagraphAlignment.Center;
                    column.Format.Font.Size = 8;
                    column = table.AddColumn(Unit.FromCentimeter(9.5));
                    column.Format.Alignment = ParagraphAlignment.Center;
                    column.Format.Font.Size = 8;

                    MigraDoc.DocumentObjectModel.Tables.Row row1;
                    row1 = table.AddRow();
                    row1.Cells[0].Shading.Color = TableWhite;
                    row1.Cells[0].Format.Font.Bold = true;
                    row1.Cells[0].Format.Font.Size = 12;
                    row1.Cells[0].Format.Alignment = ParagraphAlignment.Left;
                    row1.Cells[0].AddParagraph("Facultatea Litere și Științe");
                    row1.Cells[1].Shading.Color = TableWhite;
                    row1.Cells[1].Format.Font.Bold = true;
                    row1.Cells[1].Format.Font.Size = 12;
                    row1.Cells[1].Format.Alignment = ParagraphAlignment.Right;
                    row1.Cells[1].AddParagraph("2019-2020");



                    MigraDoc.DocumentObjectModel.Paragraph paragraphz = section.AddParagraph();
                    //paragraphz.AddText();
                    //paragraphz.Format.Font.Size = 10;
                    //paragraphz.Format.Font.Bold = true;
                    //paragraphz.Format.Alignment = ParagraphAlignment.Left;
                    //paragraphz = section.AddParagraph();
                    //paragraphz.AddText();
                    //paragraphz.Format.Font.Size = 10;
                    //paragraphz.Format.Font.Bold = true;
                    //paragraphz.Format.Alignment = ParagraphAlignment.Right;
                    //paragraphz.AddLineBreak();
                    //paragraphz = section.AddParagraph();
                    if (TheSemiGroup.iID_GROUP_TYPE == 1)
                    {
                        paragraphz.AddText("Specializarea INFORMATICĂ");
                    }
                    else
                    {
                        paragraphz.AddText("Specializarea TAPI");
                    }                    
                    paragraphz.Format.Font.Size = 10;
                    paragraphz.Format.Font.Bold = true;
                    paragraphz.Format.Alignment = ParagraphAlignment.Left;
                    paragraphz.AddLineBreak();
                    paragraphz = section.AddParagraph();
                    paragraphz.AddTab();
                    paragraphz.AddTab();
                    paragraphz.AddSpace(2);
                    paragraphz.AddText("Anul "+ TheSemiGroup.iYEAR.ToString());
                    paragraphz.Format.Font.Size = 10;
                    paragraphz.Format.Font.Bold = true;
                    paragraphz.Format.Alignment = ParagraphAlignment.Left;
                    paragraphz.AddLineBreak();
                    paragraphz = section.AddParagraph();
                    paragraphz.AddTab();
                    paragraphz.AddTab();
                    paragraphz.AddSpace(2);
                    paragraphz.AddText("Semestrul "+ PersistentData.SelectedSemester.ToString());
                    paragraphz.Format.Font.Size = 10;
                    paragraphz.Format.Font.Bold = true;
                    paragraphz.Format.Alignment = ParagraphAlignment.Left;

                    paragraphz.AddLineBreak();
                    paragraphz.AddLineBreak();
                    paragraphz.AddLineBreak();
                    paragraphz.AddLineBreak();
                    paragraphz.AddLineBreak();

                    table = new MigraDoc.DocumentObjectModel.Tables.Table();
                    table = document.LastSection.AddTable();
                    table.Style = "Table";
                    table.Borders.Color = TableBorder;
                    table.Borders.Width = 0.25;
                    table.Borders.Left.Width = 0.5;
                    table.Borders.Right.Width = 0.5;
                    table.Rows.LeftIndent = 0;

                    // Before you can add a row, you must define the columns
                    //Column column;
                    column = table.AddColumn(Unit.FromCentimeter(1.5));
                    column.Format.Alignment = ParagraphAlignment.Center;
                    column.Format.Font.Size = 8;
                    column = table.AddColumn(Unit.FromCentimeter(1.0));
                    column.Format.Alignment = ParagraphAlignment.Center;
                    column.Format.Font.Size = 8;
                    column = table.AddColumn(Unit.FromCentimeter(2.5));
                    column.Format.Alignment = ParagraphAlignment.Center;
                    column.Format.Font.Size = 8;
                    column = table.AddColumn(Unit.FromCentimeter(12.5));
                    column.Format.Alignment = ParagraphAlignment.Center;
                    column.Format.Font.Size = 8;

                    //Row row1;
                    row1 = table.AddRow();

                    row1.Cells[0].Borders.Top.Width = 0;
                    row1.Cells[0].Borders.Bottom.Width = 0;
                    row1.Cells[0].Borders.Left.Width = 0;
                    row1.Cells[0].Borders.Right.Width = 0;
                    row1.Cells[1].Borders.Top.Width = 0;
                    row1.Cells[1].Borders.Bottom.Width = 0;
                    row1.Cells[1].Borders.Left.Width = 0;
                    row1.Cells[1].Borders.Right.Width = 0;
                    row1.Cells[2].Borders.Top.Width = 0;
                    row1.Cells[2].Borders.Bottom.Width = 0;
                    row1.Cells[2].Borders.Left.Width = 0;
                    row1.Cells[2].Borders.Right.Width = Unit.FromMillimeter(1);

                    row1.Cells[3].Shading.Color = TableAccent;
                    row1.Cells[3].VerticalAlignment = MigraDoc.DocumentObjectModel.Tables.VerticalAlignment.Center;
                    row1.Cells[3].Format.Font.Bold = true;
                    row1.Cells[3].Format.Font.Size = 10;
                    row1.Cells[3].Format.Alignment = ParagraphAlignment.Center;
                    row1.Cells[3].Format.FirstLineIndent = 1;
                    row1.Cells[3].AddParagraph(TheSemiGroup.nvSEMIGROUP_NAME);
                    row1.TopPadding = 1.5;

                    for (int i = 0; i < TheSemiGroup.lLIST_EXPORT.Count; i++)
                    {
                        row1 = table.AddRow();
                        if (i == 0)
                        {
                            row1.Cells[0].Shading.Color = TableAccent;
                            row1.Cells[0].VerticalAlignment = MigraDoc.DocumentObjectModel.Tables.VerticalAlignment.Center;
                            row1.Cells[0].Format.Font.Bold = true;
                            row1.Cells[0].Format.Font.Size = 9;
                            row1.Cells[0].Format.Alignment = ParagraphAlignment.Center;
                            row1.Cells[0].Format.FirstLineIndent = 1;
                            row1.Cells[0].AddParagraph("Luni");
                            row1.Cells[0].MergeDown = TheSemiGroup.lLIST_EXPORT.Count-1;
                        }
                        else
                        {

                        }
                        row1.Cells[1].Shading.Color = TableAccent;
                        row1.Cells[1].VerticalAlignment = MigraDoc.DocumentObjectModel.Tables.VerticalAlignment.Center;
                        row1.Cells[1].Format.Font.Bold = true;
                        row1.Cells[1].Format.Font.Size = 9;
                        row1.Cells[1].Format.Alignment = ParagraphAlignment.Center;
                        row1.Cells[1].Format.FirstLineIndent = 1;
                        row1.Cells[1].AddParagraph(TheSemiGroup.lLIST_EXPORT[i].MODULE_NAME);

                        row1.Cells[2].Shading.Color = TableAccent;
                        row1.Cells[2].VerticalAlignment = MigraDoc.DocumentObjectModel.Tables.VerticalAlignment.Center;
                        row1.Cells[2].Format.Font.Bold = true;
                        row1.Cells[2].Format.Font.Size = 9;
                        row1.Cells[2].Borders.Right.Width = Unit.FromMillimeter(1);
                        row1.Cells[2].Format.Alignment = ParagraphAlignment.Center;
                        row1.Cells[2].Format.FirstLineIndent = 1;
                        row1.Cells[2].AddParagraph(Helpers.HelperModules.GetModuleInterval(TheSemiGroup.lLIST_EXPORT[i].MODULE_NAME));

                        row1.Cells[3].Shading.Color = TableWhite;
                        row1.Cells[3].VerticalAlignment = MigraDoc.DocumentObjectModel.Tables.VerticalAlignment.Center;
                        row1.Cells[3].Format.Font.Size = 8;
                        row1.Cells[3].Format.Alignment = ParagraphAlignment.Center;
                        row1.Cells[3].Format.FirstLineIndent = 1;
                        row1.Cells[3].AddParagraph(TheSemiGroup.lLIST_EXPORT[i].MONDAY == null ? "" : TheSemiGroup.lLIST_EXPORT[i].MONDAY);

                        if (i == TheSemiGroup.lLIST_EXPORT.Count-1)
                        {
                            row1.Borders.Bottom.Width = Unit.FromMillimeter(1);
                        }

                        row1.TopPadding = 1.5;
                    }
                    for (int i = 0; i < TheSemiGroup.lLIST_EXPORT.Count; i++)
                    {
                        row1 = table.AddRow();
                        if (i == 0)
                        {
                            row1.Cells[0].Shading.Color = TableAccent;
                            row1.Cells[0].VerticalAlignment = MigraDoc.DocumentObjectModel.Tables.VerticalAlignment.Center;
                            row1.Cells[0].Format.Font.Bold = true;
                            row1.Cells[0].Format.Font.Size = 9;
                            row1.Cells[0].Format.Alignment = ParagraphAlignment.Center;
                            row1.Cells[0].Format.FirstLineIndent = 1;
                            row1.Cells[0].AddParagraph("Marți");
                            row1.Cells[0].MergeDown = TheSemiGroup.lLIST_EXPORT.Count-1;
                        }
                        else
                        {

                        }
                        row1.Cells[1].Shading.Color = TableAccent;
                        row1.Cells[1].VerticalAlignment = MigraDoc.DocumentObjectModel.Tables.VerticalAlignment.Center;
                        row1.Cells[1].Format.Font.Bold = true;
                        row1.Cells[1].Format.Font.Size = 9;
                        row1.Cells[1].Format.Alignment = ParagraphAlignment.Center;
                        row1.Cells[1].Format.FirstLineIndent = 1;
                        row1.Cells[1].AddParagraph(TheSemiGroup.lLIST_EXPORT[i].MODULE_NAME);

                        row1.Cells[2].Shading.Color = TableAccent;
                        row1.Cells[2].VerticalAlignment = MigraDoc.DocumentObjectModel.Tables.VerticalAlignment.Center;
                        row1.Cells[2].Format.Font.Bold = true;
                        row1.Cells[2].Format.Font.Size = 9;
                        row1.Cells[2].Borders.Right.Width = Unit.FromMillimeter(1);
                        row1.Cells[2].Format.Alignment = ParagraphAlignment.Center;
                        row1.Cells[2].Format.FirstLineIndent = 1;
                        row1.Cells[2].AddParagraph(Helpers.HelperModules.GetModuleInterval(TheSemiGroup.lLIST_EXPORT[i].MODULE_NAME));

                        row1.Cells[3].Shading.Color = TableWhite;
                        row1.Cells[3].VerticalAlignment = MigraDoc.DocumentObjectModel.Tables.VerticalAlignment.Center;
                        row1.Cells[3].Format.Font.Size = 8;
                        row1.Cells[3].Format.Alignment = ParagraphAlignment.Center;
                        row1.Cells[3].Format.FirstLineIndent = 1;
                        row1.Cells[3].AddParagraph(TheSemiGroup.lLIST_EXPORT[i].TUESDAY == null ? "" : TheSemiGroup.lLIST_EXPORT[i].TUESDAY);

                        if (i == TheSemiGroup.lLIST_EXPORT.Count - 1)
                        {
                            row1.Borders.Bottom.Width = Unit.FromMillimeter(1);
                        }

                        row1.TopPadding = 1.5;
                    }
                    for (int i = 0; i < TheSemiGroup.lLIST_EXPORT.Count; i++)
                    {
                        row1 = table.AddRow();
                        if (i == 0)
                        {
                            row1.Cells[0].Shading.Color = TableAccent;
                            row1.Cells[0].VerticalAlignment = MigraDoc.DocumentObjectModel.Tables.VerticalAlignment.Center;
                            row1.Cells[0].Format.Font.Bold = true;
                            row1.Cells[0].Format.Font.Size = 9;
                            row1.Cells[0].Format.Alignment = ParagraphAlignment.Center;
                            row1.Cells[0].Format.FirstLineIndent = 1;
                            row1.Cells[0].AddParagraph("Miercuri");
                            row1.Cells[0].MergeDown = TheSemiGroup.lLIST_EXPORT.Count-1;
                        }
                        else
                        {

                        }
                        row1.Cells[1].Shading.Color = TableAccent;
                        row1.Cells[1].VerticalAlignment = MigraDoc.DocumentObjectModel.Tables.VerticalAlignment.Center;
                        row1.Cells[1].Format.Font.Bold = true;
                        row1.Cells[1].Format.Font.Size = 9;
                        row1.Cells[1].Format.Alignment = ParagraphAlignment.Center;
                        row1.Cells[1].Format.FirstLineIndent = 1;
                        row1.Cells[1].AddParagraph(TheSemiGroup.lLIST_EXPORT[i].MODULE_NAME);

                        row1.Cells[2].Shading.Color = TableAccent;
                        row1.Cells[2].VerticalAlignment = MigraDoc.DocumentObjectModel.Tables.VerticalAlignment.Center;
                        row1.Cells[2].Format.Font.Bold = true;
                        row1.Cells[2].Format.Font.Size = 9;
                        row1.Cells[2].Borders.Right.Width = Unit.FromMillimeter(1);
                        row1.Cells[2].Format.Alignment = ParagraphAlignment.Center;
                        row1.Cells[2].Format.FirstLineIndent = 1;
                        row1.Cells[2].AddParagraph(Helpers.HelperModules.GetModuleInterval(TheSemiGroup.lLIST_EXPORT[i].MODULE_NAME));

                        row1.Cells[3].Shading.Color = TableWhite;
                        row1.Cells[3].VerticalAlignment = MigraDoc.DocumentObjectModel.Tables.VerticalAlignment.Center;
                        row1.Cells[3].Format.Font.Size = 8;
                        row1.Cells[3].Format.Alignment = ParagraphAlignment.Center;
                        row1.Cells[3].Format.FirstLineIndent = 1;
                        row1.Cells[3].AddParagraph(TheSemiGroup.lLIST_EXPORT[i].WEDNESDAY == null ? "" : TheSemiGroup.lLIST_EXPORT[i].WEDNESDAY);

                        if (i == TheSemiGroup.lLIST_EXPORT.Count - 1)
                        {
                            row1.Borders.Bottom.Width = Unit.FromMillimeter(1);
                        }

                        row1.TopPadding = 1.5;
                    }
                    for (int i = 0; i < TheSemiGroup.lLIST_EXPORT.Count; i++)
                    {
                        row1 = table.AddRow();
                        if (i == 0)
                        {
                            row1.Cells[0].Shading.Color = TableAccent;
                            row1.Cells[0].VerticalAlignment = MigraDoc.DocumentObjectModel.Tables.VerticalAlignment.Center;
                            row1.Cells[0].Format.Font.Bold = true;
                            row1.Cells[0].Format.Font.Size = 9;
                            row1.Cells[0].Format.Alignment = ParagraphAlignment.Center;
                            row1.Cells[0].Format.FirstLineIndent = 1;
                            row1.Cells[0].AddParagraph("Joi");
                            row1.Cells[0].MergeDown = TheSemiGroup.lLIST_EXPORT.Count-1;
                        }
                        else
                        {

                        }
                        row1.Cells[1].Shading.Color = TableAccent;
                        row1.Cells[1].VerticalAlignment = MigraDoc.DocumentObjectModel.Tables.VerticalAlignment.Center;
                        row1.Cells[1].Format.Font.Bold = true;
                        row1.Cells[1].Format.Font.Size = 9;
                        row1.Cells[1].Format.Alignment = ParagraphAlignment.Center;
                        row1.Cells[1].Format.FirstLineIndent = 1;
                        row1.Cells[1].AddParagraph(TheSemiGroup.lLIST_EXPORT[i].MODULE_NAME);

                        row1.Cells[2].Shading.Color = TableAccent;
                        row1.Cells[2].VerticalAlignment = MigraDoc.DocumentObjectModel.Tables.VerticalAlignment.Center;
                        row1.Cells[2].Format.Font.Bold = true;
                        row1.Cells[2].Format.Font.Size = 9;
                        row1.Cells[2].Borders.Right.Width = Unit.FromMillimeter(1);
                        row1.Cells[2].Format.Alignment = ParagraphAlignment.Center;
                        row1.Cells[2].Format.FirstLineIndent = 1;
                        row1.Cells[2].AddParagraph(Helpers.HelperModules.GetModuleInterval(TheSemiGroup.lLIST_EXPORT[i].MODULE_NAME));

                        row1.Cells[3].Shading.Color = TableWhite;
                        row1.Cells[3].VerticalAlignment = MigraDoc.DocumentObjectModel.Tables.VerticalAlignment.Center;
                        row1.Cells[3].Format.Font.Size = 8;
                        row1.Cells[3].Format.Alignment = ParagraphAlignment.Center;
                        row1.Cells[3].Format.FirstLineIndent = 1;
                        row1.Cells[3].AddParagraph(TheSemiGroup.lLIST_EXPORT[i].THURSDAY == null ? "" : TheSemiGroup.lLIST_EXPORT[i].THURSDAY);

                        if (i == TheSemiGroup.lLIST_EXPORT.Count - 1)
                        {
                            row1.Borders.Bottom.Width = Unit.FromMillimeter(1);
                        }

                        row1.TopPadding = 1.5;
                    }
                    for (int i = 0; i < TheSemiGroup.lLIST_EXPORT.Count; i++)
                    {
                        row1 = table.AddRow();
                        if (i == 0)
                        {
                            row1.Cells[0].Shading.Color = TableAccent;
                            row1.Cells[0].VerticalAlignment = MigraDoc.DocumentObjectModel.Tables.VerticalAlignment.Center;
                            row1.Cells[0].Format.Font.Bold = true;
                            row1.Cells[0].Format.Font.Size = 9;
                            row1.Cells[0].Format.Alignment = ParagraphAlignment.Center;
                            row1.Cells[0].Format.FirstLineIndent = 1;
                            row1.Cells[0].AddParagraph("Vineri");
                            row1.Cells[0].MergeDown = TheSemiGroup.lLIST_EXPORT.Count-1;
                        }
                        else
                        {

                        }
                        row1.Cells[1].Shading.Color = TableAccent;
                        row1.Cells[1].VerticalAlignment = MigraDoc.DocumentObjectModel.Tables.VerticalAlignment.Center;
                        row1.Cells[1].Format.Font.Bold = true;
                        row1.Cells[1].Format.Font.Size = 9;
                        row1.Cells[1].Format.Alignment = ParagraphAlignment.Center;
                        row1.Cells[1].Format.FirstLineIndent = 1;
                        row1.Cells[1].AddParagraph(TheSemiGroup.lLIST_EXPORT[i].MODULE_NAME);

                        row1.Cells[2].Shading.Color = TableAccent;
                        row1.Cells[2].VerticalAlignment = MigraDoc.DocumentObjectModel.Tables.VerticalAlignment.Center;
                        row1.Cells[2].Format.Font.Bold = true;
                        row1.Cells[2].Format.Font.Size = 9;
                        row1.Cells[2].Borders.Right.Width = Unit.FromMillimeter(1);
                        row1.Cells[2].Format.Alignment = ParagraphAlignment.Center;
                        row1.Cells[2].Format.FirstLineIndent = 1;
                        row1.Cells[2].AddParagraph(Helpers.HelperModules.GetModuleInterval(TheSemiGroup.lLIST_EXPORT[i].MODULE_NAME));

                        row1.Cells[3].Shading.Color = TableWhite;
                        row1.Cells[3].VerticalAlignment = MigraDoc.DocumentObjectModel.Tables.VerticalAlignment.Center;
                        row1.Cells[3].Format.Font.Size = 8;
                        row1.Cells[3].Format.Alignment = ParagraphAlignment.Center;
                        row1.Cells[3].Format.FirstLineIndent = 1;
                        row1.Cells[3].AddParagraph(TheSemiGroup.lLIST_EXPORT[i].FRIDAY == null ? "" : TheSemiGroup.lLIST_EXPORT[i].FRIDAY);


                        row1.TopPadding = 1.5;
                    }

                    //table.SetEdge(0, table.Rows.Count - 1, 4, 1, Edge.Box, BorderStyle.Single, 0.75);
                }
                else
                {
                    //multiple semigroups
                    var OrderedByName = item.OrderBy(z => z.nvSEMIGROUP_NAME).ToList();

                    // Each MigraDoc document needs at least one section.
                    MigraDoc.DocumentObjectModel.Section section = document.AddSection();
                    section.PageSetup.LeftMargin = Unit.FromCentimeter(0.8);
                    section.PageSetup.RightMargin = Unit.FromCentimeter(2.3);
                    section.PageSetup.TopMargin = Unit.FromCentimeter(1.5);
                    section.PageSetup.BottomMargin = Unit.FromCentimeter(1.8);

                    MigraDoc.DocumentObjectModel.Tables.Table table = new MigraDoc.DocumentObjectModel.Tables.Table();
                    table = document.LastSection.AddTable();
                    table.Style = "Table";

                    // Before you can add a row, you must define the columns
                    MigraDoc.DocumentObjectModel.Tables.Column column;
                    column = table.AddColumn(Unit.FromCentimeter(9.5));
                    column.Format.Alignment = ParagraphAlignment.Center;
                    column.Format.Font.Size = 8;
                    column = table.AddColumn(Unit.FromCentimeter(9.5));
                    column.Format.Alignment = ParagraphAlignment.Center;
                    column.Format.Font.Size = 8;

                    MigraDoc.DocumentObjectModel.Tables.Row row1;
                    row1 = table.AddRow();
                    row1.Cells[0].Shading.Color = TableWhite;
                    row1.Cells[0].Format.Font.Bold = true;
                    row1.Cells[0].Format.Font.Size = 12;
                    row1.Cells[0].Format.Alignment = ParagraphAlignment.Left;
                    row1.Cells[0].AddParagraph("Facultatea Litere și Științe");
                    row1.Cells[1].Shading.Color = TableWhite;
                    row1.Cells[1].Format.Font.Bold = true;
                    row1.Cells[1].Format.Font.Size = 12;
                    row1.Cells[1].Format.Alignment = ParagraphAlignment.Right;
                    row1.Cells[1].AddParagraph("2019-2020");

                    MigraDoc.DocumentObjectModel.Paragraph paragraphz = section.AddParagraph();
                    //paragraphz.AddText("Facultatea Litere și Științe");
                    //paragraphz.Format.Font.Size = 10;
                    //paragraphz.Format.Font.Bold = true;
                    //paragraphz.Format.Alignment = ParagraphAlignment.Left;
                    //paragraphz = section.AddParagraph();
                    //paragraphz.AddText("2019-2020");
                    //paragraphz.Format.Font.Size = 10;
                    //paragraphz.Format.Font.Bold = true;
                    //paragraphz.Format.Alignment = ParagraphAlignment.Right;
                    //paragraphz.AddLineBreak();
                    if (OrderedByName.FirstOrDefault().iID_GROUP_TYPE == 1)
                    {
                        paragraphz.AddText("Specializarea INFORMATICĂ");
                    }
                    else
                    {
                        paragraphz.AddText("Specializarea TAPI");
                    }
                    paragraphz.Format.Font.Size = 10;
                    paragraphz.Format.Font.Bold = true;
                    paragraphz.Format.Alignment = ParagraphAlignment.Left;
                    paragraphz.AddLineBreak();
                    paragraphz = section.AddParagraph();
                    paragraphz.AddTab();
                    paragraphz.AddTab();
                    paragraphz.AddSpace(2);
                    paragraphz.AddText("Anul " + OrderedByName.FirstOrDefault().iYEAR.ToString());
                    paragraphz.Format.Font.Size = 10;
                    paragraphz.Format.Font.Bold = true;
                    paragraphz.Format.Alignment = ParagraphAlignment.Left;
                    paragraphz.AddLineBreak();
                    paragraphz = section.AddParagraph();
                    paragraphz.AddTab();
                    paragraphz.AddTab();
                    paragraphz.AddSpace(2);
                    paragraphz.AddText("Semestrul " + PersistentData.SelectedSemester.ToString());
                    paragraphz.Format.Font.Size = 10;
                    paragraphz.Format.Font.Bold = true;
                    paragraphz.Format.Alignment = ParagraphAlignment.Left;

                    paragraphz.AddLineBreak();
                    paragraphz.AddLineBreak();
                    paragraphz.AddLineBreak();
                    paragraphz.AddLineBreak();
                    paragraphz.AddLineBreak();

                    table = new MigraDoc.DocumentObjectModel.Tables.Table();
                    table = document.LastSection.AddTable();
                    table.Style = "Table";
                    table.Borders.Color = TableBorder;
                    table.Borders.Width = 0.25;
                    table.Borders.Left.Width = 0.5;
                    table.Borders.Right.Width = 0.5;
                    table.Rows.LeftIndent = 0;

                    // Before you can add a row, you must define the columns
                    //Column column;
                    column = table.AddColumn(Unit.FromCentimeter(1.5));
                    column.Format.Alignment = ParagraphAlignment.Center;
                    column.Format.Font.Size = 8;
                    column = table.AddColumn(Unit.FromCentimeter(1.0));
                    column.Format.Alignment = ParagraphAlignment.Center;
                    column.Format.Font.Size = 8;
                    column = table.AddColumn(Unit.FromCentimeter(2.5));
                    column.Format.Alignment = ParagraphAlignment.Center;
                    column.Format.Font.Size = 8;
                    foreach (var semigroup in OrderedByName)
                    {
                        column = table.AddColumn(Unit.FromCentimeter(12.5/OrderedByName.Count));
                        column.Format.Alignment = ParagraphAlignment.Center;
                        column.Format.Font.Size = 8;
                    }

                    //Row row1;
                    row1 = table.AddRow();

                    row1.Cells[0].Borders.Top.Width = 0;
                    row1.Cells[0].Borders.Bottom.Width = 0;
                    row1.Cells[0].Borders.Left.Width = 0;
                    row1.Cells[0].Borders.Right.Width = 0;
                    row1.Cells[1].Borders.Top.Width = 0;
                    row1.Cells[1].Borders.Bottom.Width = 0;
                    row1.Cells[1].Borders.Left.Width = 0;
                    row1.Cells[1].Borders.Right.Width = 0;
                    row1.Cells[2].Borders.Top.Width = 0;
                    row1.Cells[2].Borders.Bottom.Width = 0;
                    row1.Cells[2].Borders.Left.Width = 0;
                    row1.Cells[2].Borders.Right.Width = Unit.FromMillimeter(1);

                    for (int i = 0; i < OrderedByName.Count; i++)
                    {
                        row1.Cells[i+3].Shading.Color = TableAccent;
                        row1.Cells[i+3].VerticalAlignment = MigraDoc.DocumentObjectModel.Tables.VerticalAlignment.Center;
                        row1.Cells[i+3].Format.Font.Bold = true;
                        row1.Cells[i+3].Format.Font.Size = 10;
                        if (i!=OrderedByName.Count-1)
                        {
                            row1.Cells[i + 3].Borders.Right.Width = Unit.FromMillimeter(1);
                        }
                        row1.Cells[i+3].Format.Alignment = ParagraphAlignment.Center;
                        row1.Cells[i+3].Format.FirstLineIndent = 1;
                        row1.Cells[i+3].AddParagraph(OrderedByName[i].nvSEMIGROUP_NAME);
                    }
                    row1.TopPadding = 1.5;


                    List<MODULES> listmodules = new List<MODULES>();

                    using (var db = new OTTSContext(PersistentData.ConnectionString))
                    {
                        var getModules = db.MODULES.Where(z => z.bACTIVE == true).ToList();
                        listmodules = getModules;
                    }

                    for (int i = 0; i < listmodules.Count; i++)
                    {
                        row1 = table.AddRow();
                        if (i == 0)
                        {
                            row1.Cells[0].Shading.Color = TableAccent;
                            row1.Cells[0].VerticalAlignment = MigraDoc.DocumentObjectModel.Tables.VerticalAlignment.Center;
                            row1.Cells[0].Format.Font.Bold = true;
                            row1.Cells[0].Format.Font.Size = 9;
                            row1.Cells[0].Format.Alignment = ParagraphAlignment.Center;
                            row1.Cells[0].Format.FirstLineIndent = 1;
                            row1.Cells[0].AddParagraph("Luni");
                            row1.Cells[0].MergeDown = listmodules.Count - 1;
                        }
                        else
                        {

                        }
                        row1.Cells[1].Shading.Color = TableAccent;
                        row1.Cells[1].VerticalAlignment = MigraDoc.DocumentObjectModel.Tables.VerticalAlignment.Center;
                        row1.Cells[1].Format.Font.Bold = true;
                        row1.Cells[1].Format.Font.Size = 9;
                        row1.Cells[1].Format.Alignment = ParagraphAlignment.Center;
                        row1.Cells[1].Format.FirstLineIndent = 1;
                        row1.Cells[1].AddParagraph(listmodules[i].nvNAME);

                        row1.Cells[2].Shading.Color = TableAccent;
                        row1.Cells[2].VerticalAlignment = MigraDoc.DocumentObjectModel.Tables.VerticalAlignment.Center;
                        row1.Cells[2].Format.Font.Bold = true;
                        row1.Cells[2].Format.Font.Size = 9;
                        row1.Cells[2].Borders.Right.Width = Unit.FromMillimeter(1);
                        row1.Cells[2].Format.Alignment = ParagraphAlignment.Center;
                        row1.Cells[2].Format.FirstLineIndent = 1;
                        row1.Cells[2].AddParagraph(Helpers.HelperModules.GetModuleInterval(listmodules[i].nvNAME));

                        List<string> templist = new List<string>();
                        foreach (var stuff in OrderedByName)
                        {
                            var getStuff = stuff.lLIST_EXPORT.FirstOrDefault(z => z.MODULE_NAME == listmodules[i].nvNAME);
                            templist.Add(getStuff.MONDAY == null? "":getStuff.MONDAY);
                        }

                        var DistinctCount = templist.Distinct().ToList().Count;
                        if (DistinctCount == templist.Count)
                        {
                            for (int z = 0; z < templist.Count; z++)
                            {
                                row1.Cells[z+3].Shading.Color = TableWhite;
                                row1.Cells[z+3].VerticalAlignment = MigraDoc.DocumentObjectModel.Tables.VerticalAlignment.Center;
                                row1.Cells[z+3].Format.Font.Size = 8;
                                row1.Cells[z+3].Format.Alignment = ParagraphAlignment.Center;
                                row1.Cells[z+3].Format.FirstLineIndent = 1;
                                row1.Cells[z+3].AddParagraph(templist[z]);
                            }
                        }
                        else
                        {
                            row1.Cells[3].Shading.Color = TableWhite;
                            row1.Cells[3].VerticalAlignment = MigraDoc.DocumentObjectModel.Tables.VerticalAlignment.Center;
                            row1.Cells[3].Format.Font.Size = 8;
                            row1.Cells[3].Format.Alignment = ParagraphAlignment.Center;
                            row1.Cells[3].Format.FirstLineIndent = 1;
                            row1.Cells[3].AddParagraph(templist.FirstOrDefault());
                            row1.Cells[3].MergeRight = templist.Count - 1;
                        }

                        if (i == listmodules.Count - 1)
                        {
                            row1.Borders.Bottom.Width = Unit.FromMillimeter(1);
                        }

                        row1.TopPadding = 1.5;
                    }
                    for (int i = 0; i < listmodules.Count; i++)
                    {
                        row1 = table.AddRow();
                        if (i == 0)
                        {
                            row1.Cells[0].Shading.Color = TableAccent;
                            row1.Cells[0].VerticalAlignment = MigraDoc.DocumentObjectModel.Tables.VerticalAlignment.Center;
                            row1.Cells[0].Format.Font.Bold = true;
                            row1.Cells[0].Format.Font.Size = 9;
                            row1.Cells[0].Format.Alignment = ParagraphAlignment.Center;
                            row1.Cells[0].Format.FirstLineIndent = 1;
                            row1.Cells[0].AddParagraph("Marți");
                            row1.Cells[0].MergeDown = listmodules.Count - 1;
                        }
                        else
                        {

                        }
                        row1.Cells[1].Shading.Color = TableAccent;
                        row1.Cells[1].VerticalAlignment = MigraDoc.DocumentObjectModel.Tables.VerticalAlignment.Center;
                        row1.Cells[1].Format.Font.Bold = true;
                        row1.Cells[1].Format.Font.Size = 9;
                        row1.Cells[1].Format.Alignment = ParagraphAlignment.Center;
                        row1.Cells[1].Format.FirstLineIndent = 1;
                        row1.Cells[1].AddParagraph(listmodules[i].nvNAME);

                        row1.Cells[2].Shading.Color = TableAccent;
                        row1.Cells[2].VerticalAlignment = MigraDoc.DocumentObjectModel.Tables.VerticalAlignment.Center;
                        row1.Cells[2].Format.Font.Bold = true;
                        row1.Cells[2].Format.Font.Size = 9;
                        row1.Cells[2].Borders.Right.Width = Unit.FromMillimeter(1);
                        row1.Cells[2].Format.Alignment = ParagraphAlignment.Center;
                        row1.Cells[2].Format.FirstLineIndent = 1;
                        row1.Cells[2].AddParagraph(Helpers.HelperModules.GetModuleInterval(listmodules[i].nvNAME));

                        List<string> templist = new List<string>();
                        foreach (var stuff in OrderedByName)
                        {
                            var getStuff = stuff.lLIST_EXPORT.FirstOrDefault(z => z.MODULE_NAME == listmodules[i].nvNAME);
                            templist.Add(getStuff.TUESDAY == null? "":getStuff.TUESDAY);
                        }

                        var DistinctCount = templist.Distinct().ToList().Count;
                        if (DistinctCount == templist.Count)
                        {
                            for (int z = 0; z < templist.Count; z++)
                            {
                                row1.Cells[z+3].Shading.Color = TableWhite;
                                row1.Cells[z+3].VerticalAlignment = MigraDoc.DocumentObjectModel.Tables.VerticalAlignment.Center;
                                row1.Cells[z+3].Format.Font.Size = 8;
                                row1.Cells[z+3].Format.Alignment = ParagraphAlignment.Center;
                                row1.Cells[z+3].Format.FirstLineIndent = 1;
                                row1.Cells[z+3].AddParagraph(templist[z]);
                            }
                        }
                        else
                        {
                            row1.Cells[3].Shading.Color = TableWhite;
                            row1.Cells[3].VerticalAlignment = MigraDoc.DocumentObjectModel.Tables.VerticalAlignment.Center;
                            row1.Cells[3].Format.Font.Size = 8;
                            row1.Cells[3].Format.Alignment = ParagraphAlignment.Center;
                            row1.Cells[3].Format.FirstLineIndent = 1;
                            row1.Cells[3].AddParagraph(templist.FirstOrDefault());
                            row1.Cells[3].MergeRight = templist.Count - 1;
                        }

                        if (i == listmodules.Count - 1)
                        {
                            row1.Borders.Bottom.Width = Unit.FromMillimeter(1);
                        }

                        row1.TopPadding = 1.5;
                    }
                    for (int i = 0; i < listmodules.Count; i++)
                    {
                        row1 = table.AddRow();
                        if (i == 0)
                        {
                            row1.Cells[0].Shading.Color = TableAccent;
                            row1.Cells[0].VerticalAlignment = MigraDoc.DocumentObjectModel.Tables.VerticalAlignment.Center;
                            row1.Cells[0].Format.Font.Bold = true;
                            row1.Cells[0].Format.Font.Size = 9;
                            row1.Cells[0].Format.Alignment = ParagraphAlignment.Center;
                            row1.Cells[0].Format.FirstLineIndent = 1;
                            row1.Cells[0].AddParagraph("Miercuri");
                            row1.Cells[0].MergeDown = listmodules.Count - 1;
                        }
                        else
                        {

                        }
                        row1.Cells[1].Shading.Color = TableAccent;
                        row1.Cells[1].VerticalAlignment = MigraDoc.DocumentObjectModel.Tables.VerticalAlignment.Center;
                        row1.Cells[1].Format.Font.Bold = true;
                        row1.Cells[1].Format.Font.Size = 9;
                        row1.Cells[1].Format.Alignment = ParagraphAlignment.Center;
                        row1.Cells[1].Format.FirstLineIndent = 1;
                        row1.Cells[1].AddParagraph(listmodules[i].nvNAME);

                        row1.Cells[2].Shading.Color = TableAccent;
                        row1.Cells[2].VerticalAlignment = MigraDoc.DocumentObjectModel.Tables.VerticalAlignment.Center;
                        row1.Cells[2].Format.Font.Bold = true;
                        row1.Cells[2].Format.Font.Size = 9;
                        row1.Cells[2].Borders.Right.Width = Unit.FromMillimeter(1);
                        row1.Cells[2].Format.Alignment = ParagraphAlignment.Center;
                        row1.Cells[2].Format.FirstLineIndent = 1;
                        row1.Cells[2].AddParagraph(Helpers.HelperModules.GetModuleInterval(listmodules[i].nvNAME));

                        List<string> templist = new List<string>();
                        foreach (var stuff in OrderedByName)
                        {
                            var getStuff = stuff.lLIST_EXPORT.FirstOrDefault(z => z.MODULE_NAME == listmodules[i].nvNAME);
                            templist.Add(getStuff.WEDNESDAY == null? "":getStuff.WEDNESDAY);
                        }

                        var DistinctCount = templist.Distinct().ToList().Count;
                        if (DistinctCount == templist.Count)
                        {
                            for (int z = 0; z < templist.Count; z++)
                            {
                                row1.Cells[z+3].Shading.Color = TableWhite;
                                row1.Cells[z+3].VerticalAlignment = MigraDoc.DocumentObjectModel.Tables.VerticalAlignment.Center;
                                row1.Cells[z+3].Format.Font.Size = 8;
                                row1.Cells[z+3].Format.Alignment = ParagraphAlignment.Center;
                                row1.Cells[z+3].Format.FirstLineIndent = 1;
                                row1.Cells[z+3].AddParagraph(templist[z]);
                            }
                        }
                        else
                        {
                            row1.Cells[3].Shading.Color = TableWhite;
                            row1.Cells[3].VerticalAlignment = MigraDoc.DocumentObjectModel.Tables.VerticalAlignment.Center;
                            row1.Cells[3].Format.Font.Size = 8;
                            row1.Cells[3].Format.Alignment = ParagraphAlignment.Center;
                            row1.Cells[3].Format.FirstLineIndent = 1;
                            row1.Cells[3].AddParagraph(templist.FirstOrDefault());
                            row1.Cells[3].MergeRight = templist.Count - 1;
                        }

                        if (i == listmodules.Count - 1)
                        {
                            row1.Borders.Bottom.Width = Unit.FromMillimeter(1);
                        }

                        row1.TopPadding = 1.5;
                    }
                    for (int i = 0; i < listmodules.Count; i++)
                    {
                        row1 = table.AddRow();
                        if (i == 0)
                        {
                            row1.Cells[0].Shading.Color = TableAccent;
                            row1.Cells[0].VerticalAlignment = MigraDoc.DocumentObjectModel.Tables.VerticalAlignment.Center;
                            row1.Cells[0].Format.Font.Bold = true;
                            row1.Cells[0].Format.Font.Size = 9;
                            row1.Cells[0].Format.Alignment = ParagraphAlignment.Center;
                            row1.Cells[0].Format.FirstLineIndent = 1;
                            row1.Cells[0].AddParagraph("Joi");
                            row1.Cells[0].MergeDown = listmodules.Count - 1;
                        }
                        else
                        {

                        }
                        row1.Cells[1].Shading.Color = TableAccent;
                        row1.Cells[1].VerticalAlignment = MigraDoc.DocumentObjectModel.Tables.VerticalAlignment.Center;
                        row1.Cells[1].Format.Font.Bold = true;
                        row1.Cells[1].Format.Font.Size = 9;
                        row1.Cells[1].Format.Alignment = ParagraphAlignment.Center;
                        row1.Cells[1].Format.FirstLineIndent = 1;
                        row1.Cells[1].AddParagraph(listmodules[i].nvNAME);

                        row1.Cells[2].Shading.Color = TableAccent;
                        row1.Cells[2].VerticalAlignment = MigraDoc.DocumentObjectModel.Tables.VerticalAlignment.Center;
                        row1.Cells[2].Format.Font.Bold = true;
                        row1.Cells[2].Format.Font.Size = 9;
                        row1.Cells[2].Borders.Right.Width = Unit.FromMillimeter(1);
                        row1.Cells[2].Format.Alignment = ParagraphAlignment.Center;
                        row1.Cells[2].Format.FirstLineIndent = 1;
                        row1.Cells[2].AddParagraph(Helpers.HelperModules.GetModuleInterval(listmodules[i].nvNAME));

                        List<string> templist = new List<string>();
                        foreach (var stuff in OrderedByName)
                        {
                            var getStuff = stuff.lLIST_EXPORT.FirstOrDefault(z => z.MODULE_NAME == listmodules[i].nvNAME);
                            templist.Add(getStuff.THURSDAY == null? "":getStuff.THURSDAY);
                        }

                        var DistinctCount = templist.Distinct().ToList().Count;
                        if (DistinctCount == templist.Count)
                        {
                            for (int z = 0; z < templist.Count; z++)
                            {
                                row1.Cells[z+3].Shading.Color = TableWhite;
                                row1.Cells[z+3].VerticalAlignment = MigraDoc.DocumentObjectModel.Tables.VerticalAlignment.Center;
                                row1.Cells[z+3].Format.Font.Size = 8;
                                row1.Cells[z+3].Format.Alignment = ParagraphAlignment.Center;
                                row1.Cells[z+3].Format.FirstLineIndent = 1;
                                row1.Cells[z+3].AddParagraph(templist[z]);
                            }
                        }
                        else
                        {
                            row1.Cells[3].Shading.Color = TableWhite;
                            row1.Cells[3].VerticalAlignment = MigraDoc.DocumentObjectModel.Tables.VerticalAlignment.Center;
                            row1.Cells[3].Format.Font.Size = 8;
                            row1.Cells[3].Format.Alignment = ParagraphAlignment.Center;
                            row1.Cells[3].Format.FirstLineIndent = 1;
                            row1.Cells[3].AddParagraph(templist.FirstOrDefault());
                            row1.Cells[3].MergeRight = templist.Count - 1;
                        }

                        if (i == listmodules.Count - 1)
                        {
                            row1.Borders.Bottom.Width = Unit.FromMillimeter(1);
                        }

                        row1.TopPadding = 1.5;
                    }
                    for (int i = 0; i < listmodules.Count; i++)
                    {
                        row1 = table.AddRow();
                        if (i == 0)
                        {
                            row1.Cells[0].Shading.Color = TableAccent;
                            row1.Cells[0].VerticalAlignment = MigraDoc.DocumentObjectModel.Tables.VerticalAlignment.Center;
                            row1.Cells[0].Format.Font.Bold = true;
                            row1.Cells[0].Format.Font.Size = 9;
                            row1.Cells[0].Format.Alignment = ParagraphAlignment.Center;
                            row1.Cells[0].Format.FirstLineIndent = 1;
                            row1.Cells[0].AddParagraph("Vineri");
                            row1.Cells[0].MergeDown = listmodules.Count - 1;
                        }
                        else
                        {

                        }
                        row1.Cells[1].Shading.Color = TableAccent;
                        row1.Cells[1].VerticalAlignment = MigraDoc.DocumentObjectModel.Tables.VerticalAlignment.Center;
                        row1.Cells[1].Format.Font.Bold = true;
                        row1.Cells[1].Format.Font.Size = 9;
                        row1.Cells[1].Format.Alignment = ParagraphAlignment.Center;
                        row1.Cells[1].Format.FirstLineIndent = 1;
                        row1.Cells[1].AddParagraph(listmodules[i].nvNAME);

                        row1.Cells[2].Shading.Color = TableAccent;
                        row1.Cells[2].VerticalAlignment = MigraDoc.DocumentObjectModel.Tables.VerticalAlignment.Center;
                        row1.Cells[2].Format.Font.Bold = true;
                        row1.Cells[2].Format.Font.Size = 9;
                        row1.Cells[2].Borders.Right.Width = Unit.FromMillimeter(1);
                        row1.Cells[2].Format.Alignment = ParagraphAlignment.Center;
                        row1.Cells[2].Format.FirstLineIndent = 1;
                        row1.Cells[2].AddParagraph(Helpers.HelperModules.GetModuleInterval(listmodules[i].nvNAME));

                        List<string> templist = new List<string>();
                        foreach (var stuff in OrderedByName)
                        {
                            var getStuff = stuff.lLIST_EXPORT.FirstOrDefault(z => z.MODULE_NAME == listmodules[i].nvNAME);
                            templist.Add(getStuff.FRIDAY == null? "":getStuff.FRIDAY);
                        }

                        var DistinctCount = templist.Distinct().ToList().Count;
                        if (DistinctCount == templist.Count)
                        {
                            for (int z = 0; z < templist.Count; z++)
                            {
                                row1.Cells[z+3].Shading.Color = TableWhite;
                                row1.Cells[z+3].VerticalAlignment = MigraDoc.DocumentObjectModel.Tables.VerticalAlignment.Center;
                                row1.Cells[z+3].Format.Font.Size = 8;
                                row1.Cells[z+3].Format.Alignment = ParagraphAlignment.Center;
                                row1.Cells[z+3].Format.FirstLineIndent = 1;
                                row1.Cells[z+3].AddParagraph(templist[z]);
                            }
                        }
                        else
                        {
                            row1.Cells[3].Shading.Color = TableWhite;
                            row1.Cells[3].VerticalAlignment = MigraDoc.DocumentObjectModel.Tables.VerticalAlignment.Center;
                            row1.Cells[3].Format.Font.Size = 8;
                            row1.Cells[3].Format.Alignment = ParagraphAlignment.Center;
                            row1.Cells[3].Format.FirstLineIndent = 1;
                            row1.Cells[3].AddParagraph(templist.FirstOrDefault());
                            row1.Cells[3].MergeRight = templist.Count - 1;
                        }

                        row1.TopPadding = 1.5;
                    }

                    //table.SetEdge(0, table.Rows.Count - 1, 4, 1, Edge.Box, BorderStyle.Single, 0.75);
                }
            }

            document.UseCmykColor = true;


            // Create a renderer for PDF that uses Unicode font encoding
            PdfDocumentRenderer pdfRenderer = new PdfDocumentRenderer(true);

            // Set the MigraDoc document
            pdfRenderer.Document = document;

            // Create the PDF document
            pdfRenderer.RenderDocument();

            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.FileName = "ExportPlanificareOrar"; // Default file name
            dlg.DefaultExt = ".pdf"; // Default file extension
            dlg.Filter = "Pdf documents (.pdf)|*.pdf"; // Filter files by extension

            // Show save file dialog box
            Nullable<bool> result = dlg.ShowDialog();

            // Process save file dialog box results
            if (result == true)
            {
                // Save document
                string filename = dlg.FileName;
                // Save the PDF document...
                pdfRenderer.Save(filename);
                MessageBox.Show("Fisierul a fost exportat cu succes");
            }

        }

        private void ButtonExportExcel_Click(object sender, RoutedEventArgs e)
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
                foreach (var getSemigroup in getSemigroups)
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

            //var ByGroup = listexport.GroupBy(z => new { z.iID_GROUP_TYPE, z.iYEAR }).ToList();

            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.FileName = "ExportPlanificareOrar"; // Default file name
            dlg.DefaultExt = ".xlsx"; // Default file extension
            dlg.Filter = "Excel documents (.xlsx)|*.xlsx"; // Filter files by extension

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
                using (SpreadsheetDocument spreedDoc = SpreadsheetDocument.Create(filename, DocumentFormat.OpenXml.SpreadsheetDocumentType.Workbook))
                {
                    WorkbookPart wbPart = spreedDoc.WorkbookPart;
                    if (wbPart == null)
                    {
                        wbPart = spreedDoc.AddWorkbookPart();
                        wbPart.Workbook = new Workbook();
                    }

                    foreach (var item in listexport)
                    {
                        string sheetName = item.nvSEMIGROUP_NAME;
                        WorksheetPart worksheetPart = null;
                        worksheetPart = wbPart.AddNewPart<WorksheetPart>();
                        var sheetData = new SheetData();

                        worksheetPart.Worksheet = new Worksheet(sheetData);

                        if (wbPart.Workbook.Sheets == null)
                        {
                            wbPart.Workbook.AppendChild<Sheets>(new Sheets());
                        }

                        var sheet = new Sheet()
                        {
                            Id = wbPart.GetIdOfPart(worksheetPart),
                            SheetId = 1,
                            Name = sheetName
                        };

                        var workingSheet = ((WorksheetPart)wbPart.GetPartById(sheet.Id)).Worksheet;

                        //DocumentFormat.OpenXml.Spreadsheet.Columns columns = new DocumentFormat.OpenXml.Spreadsheet.Columns();

                        //columns.Append(new DocumentFormat.OpenXml.Spreadsheet.Column() { Min = 1, Max = 3, Width = 20, CustomWidth = true });
                        //columns.Append(new DocumentFormat.OpenXml.Spreadsheet.Column() { Min = 4, Max = 4, Width = 30, CustomWidth = true });

                        //workingSheet.Append(columns);


                        int rowindex = 1;
                        foreach (var module in item.lLIST_EXPORT)
                        {
                            DocumentFormat.OpenXml.Spreadsheet.Row row = new DocumentFormat.OpenXml.Spreadsheet.Row();
                            row.RowIndex = (UInt32)rowindex;

                            DocumentFormat.OpenXml.Spreadsheet.Cell c1 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                            c1.DataType = CellValues.InlineString;
                            InlineString inlineString = new InlineString();
                            DocumentFormat.OpenXml.Spreadsheet.Text t = new DocumentFormat.OpenXml.Spreadsheet.Text();
                            t.Text = "Luni";
                            inlineString.AppendChild(t);
                            c1.AppendChild(inlineString);
                            row.AppendChild(c1);
                            c1 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                            c1.DataType = CellValues.InlineString;
                            inlineString = new InlineString();
                            t = new DocumentFormat.OpenXml.Spreadsheet.Text();
                            t.Text = module.MODULE_NAME;
                            inlineString.AppendChild(t);
                            c1.AppendChild(inlineString);
                            row.AppendChild(c1);
                            c1 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                            c1.DataType = CellValues.InlineString;
                            inlineString = new InlineString();
                            t = new DocumentFormat.OpenXml.Spreadsheet.Text();
                            t.Text = Helpers.HelperModules.GetModuleInterval(module.MODULE_NAME);
                            inlineString.AppendChild(t);
                            c1.AppendChild(inlineString);
                            row.AppendChild(c1);
                            c1 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                            c1.DataType = CellValues.InlineString;
                            inlineString = new InlineString();
                            t = new DocumentFormat.OpenXml.Spreadsheet.Text();
                            t.Text = module.MONDAY == null ? "" : module.MONDAY;
                            inlineString.AppendChild(t);
                            c1.AppendChild(inlineString);
                            row.AppendChild(c1);

                            sheetData.AppendChild(row);
                            rowindex++;
                        }
                        foreach (var module in item.lLIST_EXPORT)
                        {
                            DocumentFormat.OpenXml.Spreadsheet.Row row = new DocumentFormat.OpenXml.Spreadsheet.Row();
                            row.RowIndex = (UInt32)rowindex;

                            DocumentFormat.OpenXml.Spreadsheet.Cell c1 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                            c1.DataType = CellValues.InlineString;
                            InlineString inlineString = new InlineString();
                            DocumentFormat.OpenXml.Spreadsheet.Text t = new DocumentFormat.OpenXml.Spreadsheet.Text();
                            t.Text = "Marți";
                            inlineString.AppendChild(t);
                            c1.AppendChild(inlineString);
                            row.AppendChild(c1);
                            c1 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                            c1.DataType = CellValues.InlineString;
                            inlineString = new InlineString();
                            t = new DocumentFormat.OpenXml.Spreadsheet.Text();
                            t.Text = module.MODULE_NAME;
                            inlineString.AppendChild(t);
                            c1.AppendChild(inlineString);
                            row.AppendChild(c1);
                            c1 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                            c1.DataType = CellValues.InlineString;
                            inlineString = new InlineString();
                            t = new DocumentFormat.OpenXml.Spreadsheet.Text();
                            t.Text = Helpers.HelperModules.GetModuleInterval(module.MODULE_NAME);
                            inlineString.AppendChild(t);
                            c1.AppendChild(inlineString);
                            row.AppendChild(c1);
                            c1 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                            c1.DataType = CellValues.InlineString;
                            inlineString = new InlineString();
                            t = new DocumentFormat.OpenXml.Spreadsheet.Text();
                            t.Text = module.TUESDAY==null?"":module.TUESDAY;
                            inlineString.AppendChild(t);
                            c1.AppendChild(inlineString);
                            row.AppendChild(c1);

                            sheetData.AppendChild(row);
                            rowindex++;
                        }
                        foreach (var module in item.lLIST_EXPORT)
                        {
                            DocumentFormat.OpenXml.Spreadsheet.Row row = new DocumentFormat.OpenXml.Spreadsheet.Row();
                            row.RowIndex = (UInt32)rowindex;

                            DocumentFormat.OpenXml.Spreadsheet.Cell c1 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                            c1.DataType = CellValues.InlineString;
                            InlineString inlineString = new InlineString();
                            DocumentFormat.OpenXml.Spreadsheet.Text t = new DocumentFormat.OpenXml.Spreadsheet.Text();
                            t.Text = "Miercuri";
                            inlineString.AppendChild(t);
                            c1.AppendChild(inlineString);
                            row.AppendChild(c1);
                            c1 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                            c1.DataType = CellValues.InlineString;
                            inlineString = new InlineString();
                            t = new DocumentFormat.OpenXml.Spreadsheet.Text();
                            t.Text = module.MODULE_NAME;
                            inlineString.AppendChild(t);
                            c1.AppendChild(inlineString);
                            row.AppendChild(c1);
                            c1 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                            c1.DataType = CellValues.InlineString;
                            inlineString = new InlineString();
                            t = new DocumentFormat.OpenXml.Spreadsheet.Text();
                            t.Text = Helpers.HelperModules.GetModuleInterval(module.MODULE_NAME);
                            inlineString.AppendChild(t);
                            c1.AppendChild(inlineString);
                            row.AppendChild(c1);
                            c1 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                            c1.DataType = CellValues.InlineString;
                            inlineString = new InlineString();
                            t = new DocumentFormat.OpenXml.Spreadsheet.Text();
                            t.Text = module.WEDNESDAY==null?"":module.WEDNESDAY;
                            inlineString.AppendChild(t);
                            c1.AppendChild(inlineString);
                            row.AppendChild(c1);

                            sheetData.AppendChild(row);
                            rowindex++;
                        }
                        foreach (var module in item.lLIST_EXPORT)
                        {
                            DocumentFormat.OpenXml.Spreadsheet.Row row = new DocumentFormat.OpenXml.Spreadsheet.Row();
                            row.RowIndex = (UInt32)rowindex;

                            DocumentFormat.OpenXml.Spreadsheet.Cell c1 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                            c1.DataType = CellValues.InlineString;
                            InlineString inlineString = new InlineString();
                            DocumentFormat.OpenXml.Spreadsheet.Text t = new DocumentFormat.OpenXml.Spreadsheet.Text();
                            t.Text = "Joi";
                            inlineString.AppendChild(t);
                            c1.AppendChild(inlineString);
                            row.AppendChild(c1);
                            c1 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                            c1.DataType = CellValues.InlineString;
                            inlineString = new InlineString();
                            t = new DocumentFormat.OpenXml.Spreadsheet.Text();
                            t.Text = module.MODULE_NAME;
                            inlineString.AppendChild(t);
                            c1.AppendChild(inlineString);
                            row.AppendChild(c1);
                            c1 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                            c1.DataType = CellValues.InlineString;
                            inlineString = new InlineString();
                            t = new DocumentFormat.OpenXml.Spreadsheet.Text();
                            t.Text = Helpers.HelperModules.GetModuleInterval(module.MODULE_NAME);
                            inlineString.AppendChild(t);
                            c1.AppendChild(inlineString);
                            row.AppendChild(c1);
                            c1 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                            c1.DataType = CellValues.InlineString;
                            inlineString = new InlineString();
                            t = new DocumentFormat.OpenXml.Spreadsheet.Text();
                            t.Text = module.THURSDAY==null?"":module.THURSDAY;
                            inlineString.AppendChild(t);
                            c1.AppendChild(inlineString);
                            row.AppendChild(c1);

                            sheetData.AppendChild(row);
                            rowindex++;
                        }
                        foreach (var module in item.lLIST_EXPORT)
                        {
                            DocumentFormat.OpenXml.Spreadsheet.Row row = new DocumentFormat.OpenXml.Spreadsheet.Row();
                            row.RowIndex = (UInt32)rowindex;

                            DocumentFormat.OpenXml.Spreadsheet.Cell c1 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                            c1.DataType = CellValues.InlineString;
                            InlineString inlineString = new InlineString();
                            DocumentFormat.OpenXml.Spreadsheet.Text t = new DocumentFormat.OpenXml.Spreadsheet.Text();
                            t.Text = "Vineri";
                            inlineString.AppendChild(t);
                            c1.AppendChild(inlineString);
                            row.AppendChild(c1);
                            c1 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                            c1.DataType = CellValues.InlineString;
                            inlineString = new InlineString();
                            t = new DocumentFormat.OpenXml.Spreadsheet.Text();
                            t.Text = module.MODULE_NAME;
                            inlineString.AppendChild(t);
                            c1.AppendChild(inlineString);
                            row.AppendChild(c1);
                            c1 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                            c1.DataType = CellValues.InlineString;
                            inlineString = new InlineString();
                            t = new DocumentFormat.OpenXml.Spreadsheet.Text();
                            t.Text = Helpers.HelperModules.GetModuleInterval(module.MODULE_NAME);
                            inlineString.AppendChild(t);
                            c1.AppendChild(inlineString);
                            row.AppendChild(c1);
                            c1 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                            c1.DataType = CellValues.InlineString;
                            inlineString = new InlineString();
                            t = new DocumentFormat.OpenXml.Spreadsheet.Text();
                            t.Text = module.FRIDAY==null?"":module.FRIDAY;
                            inlineString.AppendChild(t);
                            c1.AppendChild(inlineString);
                            row.AppendChild(c1);

                            sheetData.AppendChild(row);
                            rowindex++;
                        }

                        wbPart.Workbook.Sheets.AppendChild(sheet);
                    }

                    //Set Border
                    //wbPark

                    wbPart.Workbook.Save();
                }

                MessageBox.Show("Fisierul a fost exportat cu succes");
            }
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

            var ByGroup = listexport.GroupBy(z => new { z.iID_GROUP_TYPE, z.iYEAR }).ToList();

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
                MessageBox.Show("Fisierul a fost exportat cu succes");
            }
        }

        private void ButtonHelp_Click(object sender, RoutedEventArgs e)
        {
            var HelpText = "\tIn bara de sus a acestui meniu puteti efectua urmatoarele actiuni: Export Html, Export Excel, Export Pdf, Generare de Orare si Stergerea tuturor orarelor generate pana in acest moment.\r\n" +
                "\tDe asemenea mai jos puteti vedea o planificare prin filtrarea pe baza numarului atasat generarii si a semigrupei ce doriti sa o vedeti.\r\n";
            HelpScreen help = new HelpScreen();
            help.TitleHelp.Text = "Planning Help";
            help.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            help.TextHelp.Text = HelpText;
            help.Show();
        }
    }
}
