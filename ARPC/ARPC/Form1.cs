using ARPC.DAL;
using ARPC.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ARPC
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            BindComboGenerare();
            BindComboGrupa();
        }

        private void BindComboGrupa()
        {
            using (var db = new ARPCContext())
            {
                var getSemiGrupe = db.SEMIGRUPEs.ToList();
                comboBoxSemiGrupa.DataSource = getSemiGrupe;
                comboBoxSemiGrupa.DisplayMember = "DENUMIRE";
                comboBoxSemiGrupa.ValueMember = "ID_SEMIGRUPA";
            }
        }

        private void BindComboGenerare()
        {
            using (var db = new ARPCContext())
            {
                int Numar_Generare = 0;
                var getSetare = db.SETARIs.FirstOrDefault(z => z.CHEIE == 1337);
                if (getSetare != null)
                {
                    Numar_Generare = getSetare.VALOARE;
                }
                if (Numar_Generare!=0)
                {
                    List<DTOGenerare> list = new List<DTOGenerare>();
                    for (int i = 1; i <= Numar_Generare; i++)
                    {
                        DTOGenerare dto = new DTOGenerare();
                        dto.iNUMAR_GENERARE = i;
                        dto.nvDENUMIRE = i.ToString();
                        list.Add(dto);
                    }

                    comboBoxGenerare.DataSource =list;
                    comboBoxGenerare.DisplayMember ="nvDENUMIRE";
                    comboBoxGenerare.ValueMember ="iNUMAR_GENERARE";
                }
            }
        }


        /// <summary>
        /// Probleme curente in planificator:
        /// DONE 1. Restrictiile de zile ale profesorilor nu sunt aplicate
        /// DONE 2. Restrictiile de module ale profesorilor nu sunt aplicate
        /// DONE 3. Programul pe semigrupe nu este functional (Laboratoare/Seminarii) (curs comun)
        /// DONE 4. Planificarea nu se face la fel pentru grupele din acelasi an (curs comun)
        /// DONE 5. Numarul de ore necesare pentru curs/seminar/laborator nu sunt luate in considerare (2 ore std)
        /// DONE 6. Planificarea semi-modul/Saptamanala nu functioneaza
        /// DONE 7. Restrictia de 4 module / zi nu e luata in considerare
        /// 8. Nu se aloca salile in functie de numarul de studenti (Temp disabled)
        /// 9.
        /// TODO:
        /// DONE 1. Preferintele de zile trebuie sa aiba o prioritate
        /// DONE 2. Preferintele de module trebuie sa aiba o prioritate
        /// 3. Unele Prelegeri sunt comune pentru mai multi ani => curs si laborator comun???
        /// 4. Preferinte profesor => par/impar saptamani sau jumatate de modul(legat daca se poate)
        /// 5. Preferinte profesor => numar maxim module pe zi daca se poate
        /// 6. Preferinte Studenti => Max 2 zile cu 4 module
        /// 7. Preferinte Studenti => Lectie de consolidare dupa laboratoare (se va seta care laboratoare si va fi dupa) (Profesor RobotAutonom) (Hack => Laborator , Seminar) (Marire numar ore materie laborator)
        /// 8. Prioritate Profesori => Admin le seteaza (Legat de prelegeri)
        /// 9. 
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void ButtonGenerare_Click(object sender, EventArgs e)
        {
            int NoTimeCounter = 0;

            using (var db = new ARPCContext())
            {

                #region Selectare SemiGrupe Master si Licenta
                //var getGrupeLicenta = db.GRUPEs.Where(z => z.ID_TIP_GRUPA == 1).OrderByDescending(z=>z.ID_GRUPA).ToList();
                //var getGrupeMaster = db.GRUPEs.Where(z => z.ID_TIP_GRUPA == 2).OrderByDescending(z=>z.ID_GRUPA).ToList();
                var getSemiGrupe = db.SEMIGRUPEs.OrderBy(z=>z.PRIORITATE).ToList();
                #endregion

                #region Selectare Numar Generare
                int Numar_Generare = 1;
                var getSetare = db.SETARIs.FirstOrDefault(z => z.CHEIE == 1337);
                if (getSetare!=null)
                {
                    Numar_Generare = getSetare.VALOARE;
                }
                #endregion

                #region Stabilire Ordine de generare a grupelor Master2=>Licenta3=>Master1=>Licenta2=>Licenta1 in functie de prioritate
                List<SEMIGRUPE> OrdineSemiGrupe = new List<SEMIGRUPE>();
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
                    var getZileGenerale = db.ZILEs.OrderBy(z=>z.PRIORITATE).ToList();
                    #endregion
                    #region Se Selecteaza modulele care sunt permise pentru a se plasa ore pentru grupa selectata in ordine descrescatoare a modulelor (Se pot seta prioritat)
                    var getModuleGenerale = db.LINK_MODULE_GRUPE.Where(z => z.ID_GRUPA == parsedsemigrupa.ID_GRUPA).OrderByDescending(z=>z.ID_MODUL).ToList();
                    #endregion
                    #region Se selecteaza prelegerile ce trebuie parcurse de grupa (Se pot seta prioritati si la acestea)
                    var getPrelegeriNecesare = db.LINK_PRELEGERI_GRUPE.Where(z => z.ID_GRUPA == parsedsemigrupa.ID_GRUPA).ToList();
                    #endregion
                    #region Se proceseaza fiecare prelegere in parte
                    foreach (var prelegere in getPrelegeriNecesare)
                    {
                        #region Se selecteaza profesorul eligibil pentru curs/seminar/laborator daca sunt mai multi se selecteaza random
                        LINK_PROFESORI_PRELEGERI ProfesorCurs = null;
                        LINK_PROFESORI_PRELEGERI ProfesorSeminar = null;
                        LINK_PROFESORI_PRELEGERI ProfesorLaborator = null;

                        var getProfesoriEligibiliCurs = db.LINK_PROFESORI_PRELEGERI.Where(z => z.ID_PRELEGERE == prelegere.ID_PRELEGERE && z.ID_TIP_EXECUTIE==1).ToList();
                        if (getProfesoriEligibiliCurs.Count!=0)
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

                        var getProfesoriEligibiliSeminar = db.LINK_PROFESORI_PRELEGERI.Where(z => z.ID_PRELEGERE == prelegere.ID_PRELEGERE && z.ID_TIP_EXECUTIE==2).ToList();
                        if (getProfesoriEligibiliSeminar.Count!=0)
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

                        var getProfesoriEligibiliLaborator = db.LINK_PROFESORI_PRELEGERI.Where(z => z.ID_PRELEGERE == prelegere.ID_PRELEGERE && z.ID_TIP_EXECUTIE==3).ToList();
                        if (getProfesoriEligibiliLaborator.Count!=0)
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
                        List<LINK_PROFESORI_PRELEGERI> OrdinePlanificare = new List<LINK_PROFESORI_PRELEGERI>();
                        if (ProfesorLaborator!=null)
                        {
                            OrdinePlanificare.Add(ProfesorLaborator);
                        }
                        if (ProfesorSeminar!=null)
                        {
                            OrdinePlanificare.Add(ProfesorSeminar);
                        }
                        if (ProfesorCurs!=null)
                        {
                            OrdinePlanificare.Add(ProfesorCurs);
                        }
                        #endregion

                        #region Se proceseaza ordinea de programare
                        foreach (var ordine in OrdinePlanificare)
                        {
                            //stabilire module si zile in functie de profesor
                            var getPrioritatiZileProfesor = db.PREFERINTE_PROFESORI_ZILE.Where(z => z.ID_PROFESOR == ordine.ID_PROFESOR).OrderBy(i => i.PRIORITATE).ToList();
                            var getPrioritatiModuleProfesor = db.PREFERINTE_PROFESORI_MODULE.Where(z => z.ID_PROFESOR == ordine.ID_PROFESOR).OrderBy(i => i.PRIORITATE).ToList();

                            List<ZILE> getZile = new List<ZILE>();
                            foreach (var zi in getZileGenerale)
                            {
                                ZILE dto = new ZILE();
                                dto.ID_ZI = zi.ID_ZI;
                                dto.PRIORITATE = zi.PRIORITATE;
                                dto.DENUMIRE = zi.DENUMIRE;
                                getZile.Add(dto);
                            }
                            foreach (var item in getPrioritatiZileProfesor)
                            {
                                var getZi = getZile.FirstOrDefault(z => z.ID_ZI == item.ID_ZI);
                                if (getZi!=null)
                                {
                                    getZi.PRIORITATE = item.PRIORITATE;
                                }
                            }
                            getZile = getZile.OrderBy(z => z.PRIORITATE).ToList();

                            List<DTOModule> getModule = new List<DTOModule>();
                            foreach (var modul in getModuleGenerale)
                            {
                                DTOModule dto = new DTOModule();
                                dto.iID_LINK_MODULE_GRUPE = modul.ID_LINK_MODULE_GRUPE;
                                dto.iID_MODUL = modul.ID_MODUL;
                                dto.iPRIORITATE = 10;
                                getModule.Add(dto);
                            }
                            foreach (var item in getPrioritatiModuleProfesor)
                            {
                                var getModul = getModule.FirstOrDefault(z => z.iID_MODUL == item.ID_MODUL);
                                if (getModul!=null)
                                {
                                    getModul.iPRIORITATE = item.PRIORITATE;
                                }
                            }
                            getModule = getModule.OrderBy(z => z.iPRIORITATE).ThenByDescending(z=>z.iID_MODUL).ToList();


                            //se planifica tot in functie de numarul de ore necesare
                            var numar_ore_necesare = ordine.NUMAR_ORE;
                            while (numar_ore_necesare!=0)
                            {
                                #region Profesorul curent este cel din ordine dar se va modifica in cazul in care nu poate fi programat cu acesta si se selecteaza altul din lista
                                var ProfesorCurent = ordine;
                                #endregion
                                #region Variabile Blocker utilizate pentru a da skip la anumite componente care nu pot fi utilizate in programare
                                List<LINK_PROFESORI_PRELEGERI> ProfesoriBlocati = new List<LINK_PROFESORI_PRELEGERI>();
                                List<ZILE> ZileBlocate = new List<ZILE>();
                                List<DTOModule> ModuleBlocate = new List<DTOModule>();
                                #endregion
                                bool Planificat = false;
                                #region Se verifica daca materia in cauza a fost deja planificata

                                var VerificaPlanificare = db.PLANIFICARE_ORAR.FirstOrDefault(
                                    z => z.ID_SEMIGRUPA == parsedsemigrupa.ID_SEMIGRUPA
                                    &&
                                    z.ID_PRELEGERE == ordine.ID_PRELEGERE
                                    &&
                                    z.ID_TIP_EXECUTIE == ordine.ID_TIP_EXECUTIE
                                    &&
                                    z.NUMAR_GENERARE == Numar_Generare
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

                                    var getModuleZiPlanificateGrupe = (from u in db.PLANIFICARE_ORAR
                                                                       where u.ID_SEMIGRUPA == parsedsemigrupa.ID_SEMIGRUPA
                                                                       && u.NUMAR_GENERARE == Numar_Generare
                                                                       select u).GroupBy(z => z.ID_ZI).ToList();
                                    foreach (var item in getModuleZiPlanificateGrupe)
                                    {
                                        if (item.Count() >= 4)
                                        {
                                            var idzilocal = item.FirstOrDefault().ID_ZI;
                                            var getZiLocal = db.ZILEs.FirstOrDefault(z => z.ID_ZI == idzilocal);
                                            if (getZiLocal != null)
                                            {
                                                ZileBlocate.Add(getZiLocal);
                                            }
                                        }
                                    }

                                    #endregion

                                    var ZileDisponibile = (from u in getZile
                                                           where !ZileBlocate.Any(z => z.ID_ZI == u.ID_ZI)
                                                           select u).ToList().FirstOrDefault();

                                    var ModuleDisponibile = (from u in getModule
                                                             where !ModuleBlocate.Any(z => z.iID_LINK_MODULE_GRUPE == u.iID_LINK_MODULE_GRUPE)
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

                                        var VerificaSuprapunere = db.PLANIFICARE_ORAR.FirstOrDefault(z =>
                                        z.ID_ZI == ZileDisponibile.ID_ZI
                                        &&
                                        z.ID_MODUL == ModuleDisponibile.iID_MODUL
                                        &&
                                        ((z.ID_PROFESOR == ProfesorCurent.ID_PROFESOR) || (z.ID_PROFESOR != ProfesorCurent.ID_PROFESOR && z.ID_SEMIGRUPA == parsedsemigrupa.ID_SEMIGRUPA))
                                        //&&
                                        //((z.ID_PROFESOR == ProfesorCurent.ID_PROFESOR&&((z.ID_PRELEGERE != ProfesorCurent.ID_PRELEGERE)||(z.ID_PRELEGERE == ProfesorCurent.ID_PRELEGERE &&z.ID_TIP_EXECUTIE != ProfesorCurent.ID_TIP_EXECUTIE)))
                                        //||z.ID_PROFESOR!=ProfesorCurent.ID_PROFESOR)
                                        &&
                                        z.NUMAR_GENERARE == Numar_Generare);
                                        if (VerificaSuprapunere != null)
                                        {
                                            //e suprapunere
                                            Planificat = false;
                                            ModuleBlocate.Add(ModuleDisponibile);
                                        }
                                        else
                                        {
                                            //cazul de curs se trateaza separat deoarece trebuie pusa si la ceilalti din acelasi an
                                            if (ProfesorCurent.ID_TIP_EXECUTIE == 1)
                                            {
                                                var CursEligibil = true;
                                                var getSemiGrupeLinkate = db.SEMIGRUPEs.Where(z =>
                                                z.ID_SEMIGRUPA != parsedsemigrupa.ID_SEMIGRUPA
                                                &&
                                                z.GRUPE.ID_TIP_GRUPA == parsedsemigrupa.GRUPE.ID_TIP_GRUPA
                                                &&
                                                z.GRUPE.AN == parsedsemigrupa.GRUPE.AN
                                                ).ToList();
                                                //pentru fiecare semigrupa din lista se verifica daca este libera ziua si modulul respectiv
                                                foreach (var semigrupa in getSemiGrupeLinkate)
                                                {
                                                    if (CursEligibil == true)
                                                    {
                                                        var CheckZiModulDisponibilSemigrupa = db.PLANIFICARE_ORAR.FirstOrDefault(z =>
                                                        z.NUMAR_GENERARE == Numar_Generare
                                                        &&
                                                        z.ID_ZI == ZileDisponibile.ID_ZI
                                                        &&
                                                        z.ID_MODUL == ModuleDisponibile.iID_MODUL
                                                        &&
                                                        z.ID_SEMIGRUPA == semigrupa.ID_SEMIGRUPA
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
                                                    PLANIFICARE_ORAR orarplan = new PLANIFICARE_ORAR();
                                                    orarplan.ID_SEMIGRUPA = parsedsemigrupa.ID_SEMIGRUPA;
                                                    orarplan.ID_MODUL = ModuleDisponibile.iID_MODUL;
                                                    orarplan.ID_ZI = ZileDisponibile.ID_ZI;
                                                    orarplan.ID_PROFESOR = ProfesorCurent.ID_PROFESOR;
                                                    orarplan.ID_PRELEGERE = ProfesorCurent.ID_PRELEGERE;
                                                    orarplan.ID_TIP_EXECUTIE = ProfesorCurent.ID_TIP_EXECUTIE;
                                                    orarplan.NUMAR_GENERARE = Numar_Generare;
                                                    if (numar_ore_necesare < 2)
                                                    {
                                                        orarplan.ID_TIP_PLANIFICARE = 2;
                                                        numar_ore_necesare = 0;
                                                    }
                                                    else
                                                    {
                                                        orarplan.ID_TIP_PLANIFICARE = 1;
                                                        numar_ore_necesare -= 2;
                                                    }

                                                    db.PLANIFICARE_ORAR.Add(orarplan);
                                                    db.SaveChanges();

                                                    foreach (var semigrupa in getSemiGrupeLinkate)
                                                    {
                                                        orarplan = new PLANIFICARE_ORAR();
                                                        orarplan.ID_SEMIGRUPA = semigrupa.ID_SEMIGRUPA;
                                                        orarplan.ID_MODUL = ModuleDisponibile.iID_MODUL;
                                                        orarplan.ID_ZI = ZileDisponibile.ID_ZI;
                                                        orarplan.ID_PROFESOR = ProfesorCurent.ID_PROFESOR;
                                                        orarplan.ID_PRELEGERE = ProfesorCurent.ID_PRELEGERE;
                                                        orarplan.ID_TIP_EXECUTIE = ProfesorCurent.ID_TIP_EXECUTIE;
                                                        orarplan.NUMAR_GENERARE = Numar_Generare;
                                                        if (numar_ore_necesare < 2)
                                                        {
                                                            orarplan.ID_TIP_PLANIFICARE = 2;
                                                        }
                                                        else
                                                        {
                                                            orarplan.ID_TIP_PLANIFICARE = 1;
                                                        }

                                                        db.PLANIFICARE_ORAR.Add(orarplan);
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
                                                PLANIFICARE_ORAR orarplan = new PLANIFICARE_ORAR();
                                                orarplan.ID_SEMIGRUPA = parsedsemigrupa.ID_SEMIGRUPA;
                                                orarplan.ID_MODUL = ModuleDisponibile.iID_MODUL;
                                                orarplan.ID_ZI = ZileDisponibile.ID_ZI;
                                                orarplan.ID_PROFESOR = ProfesorCurent.ID_PROFESOR;
                                                orarplan.ID_PRELEGERE = ProfesorCurent.ID_PRELEGERE;
                                                orarplan.ID_TIP_EXECUTIE = ProfesorCurent.ID_TIP_EXECUTIE;
                                                orarplan.NUMAR_GENERARE = Numar_Generare;
                                                if (numar_ore_necesare < 2)
                                                {
                                                    orarplan.ID_TIP_PLANIFICARE = 2;
                                                    numar_ore_necesare = 0;
                                                }
                                                else
                                                {
                                                    orarplan.ID_TIP_PLANIFICARE = 1;
                                                    numar_ore_necesare -= 2;
                                                }

                                                db.PLANIFICARE_ORAR.Add(orarplan);
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
                                                                          where !ProfesoriBlocati.Any(z => z.ID_LINK_PROFESORI_PRELEGERI == u.ID_LINK_PROFESORI_PRELEGERI)
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
                                                                          where !ProfesoriBlocati.Any(z => z.ID_LINK_PROFESORI_PRELEGERI == u.ID_LINK_PROFESORI_PRELEGERI)
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
                                                                          where !ProfesoriBlocati.Any(z => z.ID_LINK_PROFESORI_PRELEGERI == u.ID_LINK_PROFESORI_PRELEGERI)
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
                if (getSetare!=null)
                {
                    getSetare.VALOARE += 1;
                    db.SaveChanges();
                }
                #endregion
                MessageBox.Show("Generarea este Gata. Nu s-au putut plasa: "+NoTimeCounter.ToString());
            }
        }

        private void ButtonIncarca_Click(object sender, EventArgs e)
        {
            gridViewPlanificare.DataSource = null;
            gridViewPlanificare.RowCount = 0;
            gridViewPlanificare.ColumnCount = 0;

            var NumarGenerare = (int)comboBoxGenerare.SelectedValue;
            var IDSemiGrupa = (int)comboBoxSemiGrupa.SelectedValue;
            using (var db = new ARPCContext())
            {
                var getSemiGrupa = db.SEMIGRUPEs.FirstOrDefault(z => z.ID_SEMIGRUPA == IDSemiGrupa);
                if (getSemiGrupa!=null)
                {
                    var getZile = db.ZILEs.ToList();
                    var getModule = db.LINK_MODULE_GRUPE.Where(z => z.ID_GRUPA == getSemiGrupa.ID_GRUPA).ToList();
                    gridViewPlanificare.ColumnCount = getZile.Count + 1;
                    gridViewPlanificare.RowCount = getModule.Count + 1;

                    for (int i = 1; i <= getModule.Count; i++)
                    {
                        gridViewPlanificare.Rows[i].Cells[0].Value = getModule[i - 1].MODULE.DENUMIRE;
                    }

                    for (int i = 1; i <= getZile.Count; i++)
                    {
                        gridViewPlanificare.Rows[0].Cells[i].Value = getZile[i - 1].DENUMIRE;
                    }

                    for (int i = 1; i <= getModule.Count; i++)
                    {
                        for (int j = 1; j <= getZile.Count; j++)
                        {
                            var idzi = getZile[j - 1].ID_ZI;
                            var idmodul = getModule[i - 1].ID_MODUL;

                            var getPlanificareZi = db.PLANIFICARE_ORAR.FirstOrDefault(z =>
                            z.ID_SEMIGRUPA == getSemiGrupa.ID_SEMIGRUPA
                            &&
                            z.NUMAR_GENERARE == NumarGenerare
                            &&
                            z.ID_ZI == idzi
                            &&
                            z.ID_MODUL == idmodul);
                            if (getPlanificareZi != null)
                            {
                                var Prelegere = getPlanificareZi.ID_PRELEGERE;
                                var Profesor = getPlanificareZi.ID_PROFESOR;
                                var TipExecutie = getPlanificareZi.ID_TIP_EXECUTIE;

                                var TextDeAfisat = "";
                                var getPrelegere = db.PRELEGERIs.FirstOrDefault(z => z.ID_PRELEGERE == Prelegere);
                                if (getPrelegere != null)
                                {
                                    TextDeAfisat += getPrelegere.DENUMIRE;
                                }
                                var getTipExecutie = db.TIP_EXECUTIE.FirstOrDefault(z => z.ID_TIP_EXECUTIE == TipExecutie);
                                if (getTipExecutie != null)
                                {
                                    TextDeAfisat += " (" + getTipExecutie.DENUMIRE + ")";
                                }
                                var getProfesor = db.PROFESORIs.FirstOrDefault(z => z.ID_PROFESOR == Profesor);
                                if (getProfesor != null)
                                {
                                    TextDeAfisat += " (" + getProfesor.NUME + " " + getProfesor.PRENUME + ")";
                                }

                                gridViewPlanificare.Rows[i].Cells[j].Value = TextDeAfisat;
                                if (getPlanificareZi.ID_TIP_PLANIFICARE==1)
                                {
                                    gridViewPlanificare.Rows[i].Cells[j].Style.ForeColor = Color.Green;
                                }
                                else
                                {
                                    gridViewPlanificare.Rows[i].Cells[j].Style.ForeColor = Color.Orange;
                                }
                            }
                            else
                            {
                                //nu pune nimic
                            }
                        }
                    }
                }

                

                
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        /// Bun deci acum avem toate datele initiale pentru a efectua generarea
        /// Avem grupele, profesorii, modulele, zilele, alocarile intre ele si tot ce e necesar fara custom stuff al profilor deocamdata
        /// 1. Master 2, Licenta 3, Master 1 , Licenta 2 , Licenta 1
        /// 2. Master ore cat mai tarziu m7 -> m3
        /// 3. Random ???
        /// 3.1. Algoritm Genetic
        /// 4. Functie de evaluare solutie
        /// 5. Operator mutatie
        /// 6. Operator crossover
        /// 
        /// 
    }
}
