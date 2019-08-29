using OTTS_WPF.Helpers;
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
using DataLink;
using System.Security.Cryptography;

namespace OTTS_WPF.Planning
{
    /// <summary>
    /// Interaction logic for WindowPlanningEntity.xaml
    /// </summary>
    public partial class WindowPlanningEntity : Window
    {
        public WindowPlanningCollection WindowPlanningCollection { get; set; }
        public MainScreen MainScreen { get; set; }
        public EnumWindowType WindowType { get; set; }
        public WindowPlanningEntity()
        {
            InitializeComponent();
            BindComboGenerator();
        }

        private void BindComboGenerator()
        {
            List<DTOPlaceholderCombo> list = new List<DTOPlaceholderCombo>();
            DTOPlaceholderCombo dto = new DTOPlaceholderCombo();
            dto.iID_GENERIC = 1;
            dto.nvCOMBO_DISPLAY = "Daniel Alexe Planificare Calendaristica";
            list.Add(dto);
            CComboGenerator.CComboBox.ItemsSource = list;
            CComboGenerator.CComboBox.SelectedItem = dto;
        }

        private void ButtonGenerate_Click(object sender, RoutedEventArgs e)
        {
            var NumberOfGenerations = CDecimalNumberOfGenerations.CValue;
            if (NumberOfGenerations<1)
            {
                MessageBox.Show("Numarul de generatii trebuie sa fie cel putin 1", "Error");
                return;
            }
            var SelectedGenerator = ((DTOPlaceholderCombo)CComboGenerator.CComboBox.SelectedItem).iID_GENERIC;
            switch (SelectedGenerator)
            {
                case 1:
                    DanielAlexeARPCGenerator();
                    break;
                default:
                    break;
            }
        }

        private void DanielAlexeARPCGenerator()
        {
            int NoTimeCounter = 0;            
            var NumarGenerari = Convert.ToInt32(CDecimalNumberOfGenerations.CValue);
            for (int m = 0; m < NumarGenerari; m++)
            {
                NoTimeCounter = 0;
                using (var db = new OTTSContext(PersistentData.ConnectionString))
                {

                    #region Selectare SemiGrupe Master si Licenta
                    //var getGrupeLicenta = db.GRUPEs.Where(z => z.ID_TIP_GRUPA == 1).OrderByDescending(z=>z.ID_GRUPA).ToList();
                    //var getGrupeMaster = db.GRUPEs.Where(z => z.ID_TIP_GRUPA == 2).OrderByDescending(z=>z.ID_GRUPA).ToList();
                    var getSemiGrupe = db.SEMIGROUPS.Where(z=>z.bACTIVE==true).OrderBy(z => z.iPRIORITY).ToList();
                    #endregion

                    #region Selectare Numar Generare
                    int Numar_Generare = 1;
                    var getSetare = db.SETTINGS.FirstOrDefault(z => z.iKEY == 1337);
                    if (getSetare != null)
                    {
                        Numar_Generare = getSetare.iVALUE;
                    }
                    #endregion

                    #region Stabilire Ordine de generare a grupelor Master2=>Licenta3=>Master1=>Licenta2=>Licenta1 in functie de prioritate
                    List<SEMIGROUPS> OrdineSemiGrupe = new List<SEMIGROUPS>();
                    OrdineSemiGrupe.AddRange(getSemiGrupe);
                    //OrdineGrupe.AddRange(getSemiGrupe.Where(z => z.GRUPE.AN == 2 && z.GRUPE.ID_TIP_GRUPA == 2).ToList());
                    //OrdineGrupe.AddRange(getSemiGrupe.Where(z => z.GRUPE.AN == 3 && z.GRUPE.ID_TIP_GRUPA == 1).ToList());
                    //OrdineGrupe.AddRange(getSemiGrupe.Where(z => z.GRUPE.AN == 1 && z.GRUPE.ID_TIP_GRUPA == 2).ToList());
                    //OrdineGrupe.AddRange(getSemiGrupe.Where(z => z.GRUPE.AN == 2 && z.GRUPE.ID_TIP_GRUPA == 1).ToList());
                    //OrdineGrupe.AddRange(getSemiGrupe.Where(z => z.GRUPE.AN == 1 && z.GRUPE.ID_TIP_GRUPA == 1).ToList());
                    //foreach (var item in getGrupeMaster)
                    //{
                    //    if (item != getGrupeMaster.FirstOrDefault())
                    //    {
                    //        OrdineGrupe.Add(item);
                    //    }
                    //}
                    //foreach (var item in getGrupeLicenta)
                    //{
                    //    if (item != getGrupeLicenta.FirstOrDefault())
                    //    {
                    //        OrdineGrupe.Add(item);
                    //    }
                    //}
                    #endregion

                    #region Se Proceseaza Fiecare Grupa in Parte
                    foreach (var parsedsemigrupa in OrdineSemiGrupe)
                    {
                        #region Se Selecteaza zilele disponibile din sistem in functie de prioritate
                        var getZileGenerale = db.DAYS.Where(z=>z.bACTIVE==true).OrderBy(z => z.iPRIORITY).ToList();
                        #endregion
                        #region Se Selecteaza modulele care sunt permise pentru a se plasa ore pentru grupa selectata in ordine descrescatoare a modulelor (Se pot seta prioritat)
                        var getModuleGenerale = db.GROUPS_MODULES_LINK.Where(z =>z.bACTIVE==true && z.iID_GROUP == parsedsemigrupa.iID_GROUP).OrderByDescending(z => z.iID_MODULE).ToList();
                        #endregion
                        #region Se selecteaza prelegerile ce trebuie parcurse de grupa (Se pot seta prioritati si la acestea)
                        var getPrelegeriNecesare = db.GROUPS_LECTURES_LINK.Where(z =>z.bACTIVE==true && z.iID_GROUP == parsedsemigrupa.iID_GROUP).ToList();
                        #endregion

                        #region Prioritizare prelegeri necesare in functie de prioritati profesori
                        var OrdinePrelegeri = new List<AssociationSet>();
                        var PrelegeriFinale = new List<GROUPS_LECTURES_LINK>();
                        foreach (var prelegere in getPrelegeriNecesare)
                        {
                            AssociationSet set = new AssociationSet();
                            set.LINK = prelegere;

                            var CurrentPriority = 9999;

                            var getProfesoriEligibiliCurs = db.TEACHERS_LECTURES_LINK.Where(z => z.bACTIVE == true && z.iID_LECTURE == prelegere.iID_LECTURE && z.iID_LECTURE_TYPE == 1).ToList();
                            if (getProfesoriEligibiliCurs.Count != 0)
                            {
                                foreach (var item in getProfesoriEligibiliCurs)
                                {
                                    if (item.TEACHERS.iPRIORITY<CurrentPriority)
                                    {
                                        CurrentPriority = item.TEACHERS.iPRIORITY;
                                    }
                                }
                            }

                            var getProfesoriEligibiliSeminar = db.TEACHERS_LECTURES_LINK.Where(z => z.bACTIVE == true && z.iID_LECTURE == prelegere.iID_LECTURE && z.iID_LECTURE_TYPE == 2).ToList();
                            if (getProfesoriEligibiliSeminar.Count != 0)
                            {
                                foreach (var item in getProfesoriEligibiliSeminar)
                                {
                                    if (item.TEACHERS.iPRIORITY < CurrentPriority)
                                    {
                                        CurrentPriority = item.TEACHERS.iPRIORITY;
                                    }
                                }
                            }

                            var getProfesoriEligibiliLaborator = db.TEACHERS_LECTURES_LINK.Where(z => z.bACTIVE == true && z.iID_LECTURE == prelegere.iID_LECTURE && z.iID_LECTURE_TYPE == 3).ToList();
                            if (getProfesoriEligibiliLaborator.Count != 0)
                            {
                                foreach (var item in getProfesoriEligibiliLaborator)
                                {
                                    if (item.TEACHERS.iPRIORITY < CurrentPriority)
                                    {
                                        CurrentPriority = item.TEACHERS.iPRIORITY;
                                    }
                                }
                            }

                            set.PRIORITY = CurrentPriority;
                            OrdinePrelegeri.Add(set);
                        }
                        OrdinePrelegeri = OrdinePrelegeri.OrderBy(z => z.PRIORITY).ToList();
                        foreach (var item in OrdinePrelegeri)
                        {
                            PrelegeriFinale.Add(item.LINK);
                        }
                        #endregion

                        #region Se proceseaza fiecare prelegere in parte
                        foreach (var prelegere in PrelegeriFinale)
                        {
                            #region Se selecteaza profesorul eligibil pentru curs/seminar/laborator daca sunt mai multi se selecteaza random
                            TEACHERS_LECTURES_LINK ProfesorCurs = null;
                            TEACHERS_LECTURES_LINK ProfesorSeminar = null;
                            TEACHERS_LECTURES_LINK ProfesorLaborator = null;

                            var getProfesoriEligibiliCurs = db.TEACHERS_LECTURES_LINK.Where(z =>z.bACTIVE==true && z.iID_LECTURE == prelegere.iID_LECTURE && z.iID_LECTURE_TYPE == 1).ToList();
                            if (getProfesoriEligibiliCurs.Count != 0)
                            {
                                if (getProfesoriEligibiliCurs.Count == 1)
                                {
                                    //doar ala
                                    ProfesorCurs = getProfesoriEligibiliCurs.FirstOrDefault();
                                }
                                else
                                {
                                    //alegem unul din ei
                                    Random rand = new Random();
                                    var ales = rand.Next(0, getProfesoriEligibiliCurs.Count);
                                    ProfesorCurs = getProfesoriEligibiliCurs[ales];
                                }
                            }

                            var getProfesoriEligibiliSeminar = db.TEACHERS_LECTURES_LINK.Where(z =>z.bACTIVE==true && z.iID_LECTURE == prelegere.iID_LECTURE && z.iID_LECTURE_TYPE == 2).ToList();
                            if (getProfesoriEligibiliSeminar.Count != 0)
                            {
                                if (getProfesoriEligibiliSeminar.Count == 1)
                                {
                                    //doar ala
                                    ProfesorSeminar = getProfesoriEligibiliSeminar.FirstOrDefault();
                                }
                                else
                                {
                                    //alegem unul din ei
                                    Random rand = new Random();
                                    var ales = rand.Next(0, getProfesoriEligibiliSeminar.Count);
                                    ProfesorSeminar = getProfesoriEligibiliSeminar[ales];
                                }
                            }

                            var getProfesoriEligibiliLaborator = db.TEACHERS_LECTURES_LINK.Where(z =>z.bACTIVE==true && z.iID_LECTURE == prelegere.iID_LECTURE && z.iID_LECTURE_TYPE == 3).ToList();
                            if (getProfesoriEligibiliLaborator.Count != 0)
                            {
                                if (getProfesoriEligibiliLaborator.Count == 1)
                                {
                                    //doar ala
                                    ProfesorLaborator = getProfesoriEligibiliLaborator.FirstOrDefault();
                                }
                                else
                                {
                                    //alegem unul din ei
                                    Random rand = new Random();
                                    var ales = rand.Next(0, getProfesoriEligibiliLaborator.Count);
                                    ProfesorLaborator = getProfesoriEligibiliLaborator[ales];
                                }
                            }
                            #endregion

                            #region Se seteaza ordinea de programare Laborator=>Seminar=>Curs
                            List<TEACHERS_LECTURES_LINK> OrdinePlanificare = new List<TEACHERS_LECTURES_LINK>();
                            if (ProfesorLaborator != null)
                            {
                                OrdinePlanificare.Add(ProfesorLaborator);
                            }
                            if (ProfesorSeminar != null)
                            {
                                OrdinePlanificare.Add(ProfesorSeminar);
                            }
                            if (ProfesorCurs != null)
                            {
                                OrdinePlanificare.Add(ProfesorCurs);
                            }
                            #endregion

                            #region Se proceseaza ordinea de programare
                            foreach (var ordine in OrdinePlanificare)
                            {
                                //stabilire module si zile in functie de profesor
                                var getPrioritatiZileProfesor = db.TEACHER_PREFERRED_DAYS.Where(z =>z.bACTIVE==true && z.iID_TEACHER == ordine.iID_TEACHER).OrderBy(i => i.iPRIORITY).ToList();
                                var getPrioritatiModuleProfesor = db.TEACHER_PREFERRED_MODULES.Where(z =>z.bACTIVE==true && z.iID_TEACHER == ordine.iID_TEACHER).OrderBy(i => i.iPRIORITY).ToList();

                                List<DAYS> getZile = new List<DAYS>();
                                foreach (var zi in getZileGenerale)
                                {
                                    DAYS dto = new DAYS();
                                    dto.iID_DAY = zi.iID_DAY;
                                    dto.iPRIORITY = zi.iPRIORITY;
                                    dto.nvNAME = zi.nvNAME;
                                    getZile.Add(dto);
                                }
                                foreach (var item in getPrioritatiZileProfesor)
                                {
                                    var getZi = getZile.FirstOrDefault(z => z.iID_DAY == item.iID_DAY);
                                    if (getZi != null)
                                    {
                                        getZi.iPRIORITY = item.iPRIORITY;
                                    }
                                }
                                getZile = getZile.OrderBy(z => z.iPRIORITY).ToList();

                                //Shuffle Zile pe baza prioritatii
                                List<DAYS> ShuffledZile = new List<DAYS>();
                                var ZileGrupate = getZile.GroupBy(z => z.iPRIORITY).ToList();
                                foreach (var grupare in ZileGrupate)
                                {
                                    var OrdineInitiala = grupare.ToList();
                                    Shuffle(OrdineInitiala);
                                    ShuffledZile.AddRange(OrdineInitiala);
                                }
                                getZile = ShuffledZile;

                                List<DTOGroupModule> getModule = new List<DTOGroupModule>();
                                foreach (var modul in getModuleGenerale)
                                {
                                    DTOGroupModule dto = new DTOGroupModule();
                                    dto.iID_GROUPS_MODULES_LINK = modul.iID_GROUPS_MODULES_LINK;
                                    dto.iID_MODULE = modul.iID_MODULE;
                                    dto.iID_GENERATOR_PRIORITY = 10;
                                    getModule.Add(dto);
                                }
                                foreach (var item in getPrioritatiModuleProfesor)
                                {
                                    var getModul = getModule.FirstOrDefault(z => z.iID_MODULE == item.iID_MODULE);
                                    if (getModul != null)
                                    {
                                        getModul.iID_GENERATOR_PRIORITY = item.iPRIORITY;
                                    }
                                }
                                getModule = getModule.OrderBy(z => z.iID_GENERATOR_PRIORITY).ThenByDescending(z => z.iID_MODULE).ToList();


                                //se planifica tot in functie de numarul de ore necesare
                                var numar_ore_necesare = ordine.iHOURS;
                                while (numar_ore_necesare != 0)
                                {
                                    #region Profesorul curent este cel din ordine dar se va modifica in cazul in care nu poate fi programat cu acesta si se selecteaza altul din lista
                                    var ProfesorCurent = ordine;
                                    #endregion
                                    #region Variabile Blocker utilizate pentru a da skip la anumite componente care nu pot fi utilizate in programare
                                    List<TEACHERS_LECTURES_LINK> ProfesoriBlocati = new List<TEACHERS_LECTURES_LINK>();
                                    List<DAYS> ZileBlocate = new List<DAYS>();
                                    List<DTOGroupModule> ModuleBlocate = new List<DTOGroupModule>();
                                    #endregion
                                    bool Planificat = false;
                                    #region Se verifica daca materia in cauza a fost deja planificata

                                    var VerificaPlanificare = db.TIMETABLE_PLANNING.FirstOrDefault(
                                        z => z.iID_SEMIGROUP == parsedsemigrupa.iID_SEMIGROUP && z.bACTIVE==true
                                        &&
                                        z.iID_LECTURE == ordine.iID_LECTURE
                                        &&
                                        z.iID_LECTURE_TYPE == ordine.iID_LECTURE_TYPE
                                        &&
                                        z.iGENERATION_NUMBER == Numar_Generare
                                        );
                                    if (VerificaPlanificare != null)
                                    {
                                        Planificat = true;
                                        numar_ore_necesare = 0;
                                    }
                                    #endregion
                                    while (Planificat == false)
                                    {

                                        #region Eliminare Zile care au peste 4 module programate deja prin inserarea in ZileBlocate

                                        var getModuleZiPlanificateGrupe = (from u in db.TIMETABLE_PLANNING
                                                                            where u.iID_SEMIGROUP == parsedsemigrupa.iID_SEMIGROUP
                                                                            && u.bACTIVE==true
                                                                            && u.iGENERATION_NUMBER == Numar_Generare
                                                                            select u).GroupBy(z => z.iID_DAY).ToList();
                                        foreach (var item in getModuleZiPlanificateGrupe)
                                        {
                                            if (item.Count() >= 4)
                                            {
                                                var idzilocal = item.FirstOrDefault().iID_DAY;
                                                var getZiLocal = db.DAYS.FirstOrDefault(z =>z.bACTIVE==true && z.iID_DAY == idzilocal);
                                                if (getZiLocal != null)
                                                {
                                                    ZileBlocate.Add(getZiLocal);
                                                }
                                            }
                                        }

                                        #endregion

                                        var ZileDisponibile = (from u in getZile
                                                                where !ZileBlocate.Any(z => z.iID_DAY == u.iID_DAY)
                                                                select u).ToList().FirstOrDefault();

                                        var ModuleDisponibile = (from u in getModule
                                                                    where !ModuleBlocate.Any(z => z.iID_GROUPS_MODULES_LINK == u.iID_GROUPS_MODULES_LINK)
                                                                    select u).ToList().FirstOrDefault();
                                        if (ZileDisponibile != null && ModuleDisponibile != null)
                                        {
                                            ///profesor diferit
                                            ///acelasi profesor dar care tine o prelegere diferita
                                            ///acelasi profesor dar care tine aceasi perelgere la un tip de executie diferit
                                            ///

                                            //var SuprapuneriGenerale = db.PLANIFICARE_ORAR.Where(z => z.ID_ZI == ZileDisponibile.ID_ZI && z.ID_MODUL == ModuleDisponibile.ID_MODUL && z.NUMAR_GENERARE == Numar_Generare).ToList();
                                            //var ProfesorulESuprapus = SuprapuneriGenerale.Where(z => z.ID_PROFESOR == ProfesorCurent.ID_PROFESOR).ToList();
                                            //if (ProfesorulESuprapus.Count>0)
                                            //{

                                            //}
                                            //else
                                            //{

                                            //}

                                            var VerificaSuprapunere = db.TIMETABLE_PLANNING.FirstOrDefault(z =>z.bACTIVE==true &&
                                            z.iID_DAY == ZileDisponibile.iID_DAY
                                            &&
                                            z.iID_MODULE == ModuleDisponibile.iID_MODULE
                                            &&
                                            ((z.iID_TEACHER == ProfesorCurent.iID_TEACHER) || (z.iID_TEACHER != ProfesorCurent.iID_TEACHER && z.iID_SEMIGROUP == parsedsemigrupa.iID_SEMIGROUP))
                                            //&&
                                            //((z.ID_PROFESOR == ProfesorCurent.ID_PROFESOR&&((z.ID_PRELEGERE != ProfesorCurent.ID_PRELEGERE)||(z.ID_PRELEGERE == ProfesorCurent.ID_PRELEGERE &&z.ID_TIP_EXECUTIE != ProfesorCurent.ID_TIP_EXECUTIE)))
                                            //||z.ID_PROFESOR!=ProfesorCurent.ID_PROFESOR)
                                            &&
                                            z.iGENERATION_NUMBER == Numar_Generare);
                                            if (VerificaSuprapunere != null)
                                            {
                                                //e suprapunere
                                                Planificat = false;
                                                ModuleBlocate.Add(ModuleDisponibile);
                                            }
                                            else
                                            {
                                                //cazul de curs se trateaza separat deoarece trebuie pusa si la ceilalti din acelasi an
                                                if (ProfesorCurent.iID_LECTURE_TYPE == 1)
                                                {
                                                    var CursEligibil = true;
                                                    var getSemiGrupeLinkate = db.SEMIGROUPS.Where(z =>z.bACTIVE==true &&
                                                    z.iID_SEMIGROUP != parsedsemigrupa.iID_SEMIGROUP
                                                    &&
                                                    z.GROUPS.iID_GROUP_TYPE == parsedsemigrupa.GROUPS.iID_GROUP_TYPE
                                                    &&
                                                    z.GROUPS.iYEAR == parsedsemigrupa.GROUPS.iYEAR
                                                    ).ToList();
                                                    //pentru fiecare semigrupa din lista se verifica daca este libera ziua si modulul respectiv
                                                    foreach (var semigrupa in getSemiGrupeLinkate)
                                                    {
                                                        if (CursEligibil == true)
                                                        {
                                                            var CheckZiModulDisponibilSemigrupa = db.TIMETABLE_PLANNING.FirstOrDefault(z => z.bACTIVE==true &&
                                                            z.iGENERATION_NUMBER == Numar_Generare
                                                            &&
                                                            z.iID_DAY == ZileDisponibile.iID_DAY
                                                            &&
                                                            z.iID_MODULE == ModuleDisponibile.iID_MODULE
                                                            &&
                                                            z.iID_SEMIGROUP == semigrupa.iID_SEMIGROUP
                                                            );
                                                            if (CheckZiModulDisponibilSemigrupa != null)
                                                            {
                                                                CursEligibil = false;
                                                            }
                                                        }
                                                    }
                                                    if (CursEligibil == true)
                                                    {
                                                        //se planifica cursul pentru toate semigrupele din lista si semigrupa curenta
                                                        Planificat = true;
                                                        TIMETABLE_PLANNING orarplan = new TIMETABLE_PLANNING();

                                                        orarplan.bACTIVE = true;
                                                        orarplan.dtCREATE_DATE = DateTime.UtcNow;
                                                        orarplan.iCREATE_USER = PersistentData.LoggedUser.iID_USER;

                                                        orarplan.iID_SEMIGROUP = parsedsemigrupa.iID_SEMIGROUP;
                                                        orarplan.iID_MODULE = ModuleDisponibile.iID_MODULE;
                                                        orarplan.iID_DAY = ZileDisponibile.iID_DAY;
                                                        orarplan.iID_TEACHER = ProfesorCurent.iID_TEACHER;
                                                        orarplan.iID_LECTURE = ProfesorCurent.iID_LECTURE;
                                                        orarplan.iID_LECTURE_TYPE = ProfesorCurent.iID_LECTURE_TYPE;
                                                        orarplan.iGENERATION_NUMBER = Numar_Generare;
                                                        if (numar_ore_necesare < 2)
                                                        {
                                                            orarplan.iID_PLANNING_TYPE = 2;
                                                            numar_ore_necesare = 0;
                                                        }
                                                        else
                                                        {
                                                            orarplan.iID_PLANNING_TYPE = 1;
                                                            numar_ore_necesare -= 2;
                                                        }

                                                        db.TIMETABLE_PLANNING.Add(orarplan);
                                                        db.SaveChanges();

                                                        var backupTipPlanificare = orarplan.iID_PLANNING_TYPE;

                                                        foreach (var semigrupa in getSemiGrupeLinkate)
                                                        {
                                                            orarplan = new TIMETABLE_PLANNING();

                                                            orarplan.bACTIVE = true;
                                                            orarplan.dtCREATE_DATE = DateTime.UtcNow;
                                                            orarplan.iCREATE_USER = PersistentData.LoggedUser.iID_USER;

                                                            orarplan.iID_SEMIGROUP = semigrupa.iID_SEMIGROUP;
                                                            orarplan.iID_MODULE = ModuleDisponibile.iID_MODULE;
                                                            orarplan.iID_DAY = ZileDisponibile.iID_DAY;
                                                            orarplan.iID_TEACHER = ProfesorCurent.iID_TEACHER;
                                                            orarplan.iID_LECTURE = ProfesorCurent.iID_LECTURE;
                                                            orarplan.iID_LECTURE_TYPE = ProfesorCurent.iID_LECTURE_TYPE;
                                                            orarplan.iGENERATION_NUMBER = Numar_Generare;
                                                            orarplan.iID_PLANNING_TYPE = backupTipPlanificare;
                                                            //if (numar_ore_necesare < 2)
                                                            //{
                                                            //    orarplan.ID_TIP_PLANIFICARE = 2;
                                                            //}
                                                            //else
                                                            //{
                                                            //    orarplan.ID_TIP_PLANIFICARE = 1;
                                                            //}

                                                            db.TIMETABLE_PLANNING.Add(orarplan);
                                                            db.SaveChanges();
                                                        }
                                                    }
                                                    else
                                                    {
                                                        Planificat = false;
                                                        ModuleBlocate.Add(ModuleDisponibile);
                                                    }
                                                }
                                                else
                                                {
                                                    //nu e deci pot sa il pun
                                                    Planificat = true;
                                                    TIMETABLE_PLANNING orarplan = new TIMETABLE_PLANNING();

                                                    orarplan.bACTIVE = true;
                                                    orarplan.dtCREATE_DATE = DateTime.UtcNow;
                                                    orarplan.iCREATE_USER = PersistentData.LoggedUser.iID_USER;

                                                    orarplan.iID_SEMIGROUP = parsedsemigrupa.iID_SEMIGROUP;
                                                    orarplan.iID_MODULE = ModuleDisponibile.iID_MODULE;
                                                    orarplan.iID_DAY = ZileDisponibile.iID_DAY;
                                                    orarplan.iID_TEACHER = ProfesorCurent.iID_TEACHER;
                                                    orarplan.iID_LECTURE = ProfesorCurent.iID_LECTURE;
                                                    orarplan.iID_LECTURE_TYPE = ProfesorCurent.iID_LECTURE_TYPE;
                                                    orarplan.iGENERATION_NUMBER = Numar_Generare;
                                                    if (numar_ore_necesare < 2)
                                                    {
                                                        orarplan.iID_PLANNING_TYPE = 2;
                                                        numar_ore_necesare = 0;
                                                    }
                                                    else
                                                    {
                                                        orarplan.iID_PLANNING_TYPE = 1;
                                                        numar_ore_necesare -= 2;
                                                    }

                                                    db.TIMETABLE_PLANNING.Add(orarplan);
                                                    db.SaveChanges();
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if (ModuleDisponibile == null)
                                            {
                                                ModuleBlocate.Clear();
                                                if (ZileDisponibile != null)
                                                {
                                                    ZileBlocate.Add(ZileDisponibile);
                                                }
                                            }
                                            if (ZileDisponibile == null)
                                            {
                                                //nu am unde sa il pun efectiv cu profesorul curent.....
                                                if (ProfesorCurent == ProfesorLaborator)
                                                {
                                                    if (getProfesoriEligibiliLaborator.Count > 1)
                                                    {
                                                        if (ProfesoriBlocati.Count == getProfesoriEligibiliLaborator.Count)
                                                        {
                                                            NoTimeCounter++;
                                                            numar_ore_necesare = 0;
                                                            break;
                                                        }
                                                        else
                                                        {
                                                            ProfesoriBlocati.Add(ProfesorCurent);
                                                            ProfesorCurent = (from u in getProfesoriEligibiliLaborator
                                                                                where !ProfesoriBlocati.Any(z => z.iID_TEACHERS_LECTURES_LINK == u.iID_TEACHERS_LECTURES_LINK)
                                                                                select u).ToList().FirstOrDefault();
                                                            ZileBlocate.Clear();
                                                            ModuleBlocate.Clear();
                                                        }
                                                    }
                                                    else
                                                    {
                                                        NoTimeCounter++;
                                                        numar_ore_necesare = 0;
                                                        break;
                                                    }
                                                }
                                                else
                                                if (ProfesorCurent == ProfesorSeminar)
                                                {
                                                    if (getProfesoriEligibiliSeminar.Count > 1)
                                                    {
                                                        if (ProfesoriBlocati.Count == getProfesoriEligibiliSeminar.Count)
                                                        {
                                                            NoTimeCounter++;
                                                            break;
                                                        }
                                                        else
                                                        {
                                                            ProfesoriBlocati.Add(ProfesorCurent);
                                                            ProfesorCurent = (from u in getProfesoriEligibiliSeminar
                                                                                where !ProfesoriBlocati.Any(z => z.iID_TEACHERS_LECTURES_LINK == u.iID_TEACHERS_LECTURES_LINK)
                                                                                select u).ToList().FirstOrDefault();
                                                            ZileBlocate.Clear();
                                                            ModuleBlocate.Clear();
                                                        }
                                                    }
                                                    else
                                                    {
                                                        NoTimeCounter++;
                                                        numar_ore_necesare = 0;
                                                        break;
                                                    }
                                                }
                                                else
                                                if (ProfesorCurent == ProfesorCurs)
                                                {
                                                    if (getProfesoriEligibiliCurs.Count > 1)
                                                    {
                                                        if (ProfesoriBlocati.Count == getProfesoriEligibiliCurs.Count)
                                                        {
                                                            NoTimeCounter++;
                                                            numar_ore_necesare = 0;
                                                            break;
                                                        }
                                                        else
                                                        {
                                                            ProfesoriBlocati.Add(ProfesorCurent);
                                                            ProfesorCurent = (from u in getProfesoriEligibiliCurs
                                                                                where !ProfesoriBlocati.Any(z => z.iID_TEACHERS_LECTURES_LINK == u.iID_TEACHERS_LECTURES_LINK)
                                                                                select u).ToList().FirstOrDefault();
                                                            ZileBlocate.Clear();
                                                            ModuleBlocate.Clear();
                                                        }
                                                    }
                                                    else
                                                    {
                                                        NoTimeCounter++;
                                                        numar_ore_necesare = 0;
                                                        break;
                                                    }
                                                }
                                                else
                                                {
                                                    NoTimeCounter++;
                                                    numar_ore_necesare = 0;
                                                    break;
                                                }

                                            }
                                        }

                                    }
                                }
                            }
                            #endregion

                        }
                        #endregion

                    }
                    #endregion
                    #region Update Numar Generare
                    if (getSetare != null)
                    {
                        getSetare.iVALUE += 1;
                        db.SaveChanges();
                    }
                    #endregion
                }
            }
            
            MessageBox.Show("Generarea este Gata. Nu s-au putut plasa: " + NoTimeCounter.ToString());
            WindowPlanningCollection.ReloadData();
            CloseWindow();
        }

        private class AssociationSet
        {
            public int PRIORITY { get; set; }
            public GROUPS_LECTURES_LINK LINK { get; set; }
        }

        private void Shuffle<T>(IList<T> list)
        {
            RNGCryptoServiceProvider provider = new RNGCryptoServiceProvider();
            int n = list.Count;
            while (n > 1)
            {
                byte[] box = new byte[1];
                do provider.GetBytes(box);
                while (!(box[0] < n * (Byte.MaxValue / n)));
                int k = (box[0] % n);
                n--;
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        public void CloseWindow()
        {
            MainScreen.LowerDownMenu();
        }

        private void ButtonClose_Click(object sender, RoutedEventArgs e)
        {
            CloseWindow();
        }
    }
}
