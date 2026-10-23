using Model.IKYS;
using Model.Ortak;
using Model.Services.IKYS;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls.WebParts;
using Utility.HelperClasses;
using Utility.Logging;
using Utility.ProjeGlobal;
using static Model.IKYS.Personel;

namespace IKYS_WebParts.GorevSemasiWP
{
    [ToolboxItemAttribute(false)]
    public partial class GorevSemasiWP : WebPart
    {
        private PortalLogContext LoggingContext { get { return PortalLogContext.FromCurrentRequest("IKYS", "GorevSemasiWP"); } }

        private void ReportError(Exception error)
        {
            PortalLogContext context = LoggingContext;
            if (error is System.Threading.ThreadAbortException) return;
            PortalTechnicalLog.WriteError(context, "GorevSemasi.Load", error);
            Messages.Error("Görev şeması yüklenemedi. Başvuru kodu: " + context.OperationId.ToString("N"));
        }
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public GorevSemasiWP()
        {
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            InitializeControl();
            this.ChromeType = PartChromeType.None;
        }
        private string CurrentUserName
        {
            get
            {

                if (ViewState["CurrentUserName"] == null)
                {
                    ViewState["CurrentUserName"] = UtilityHelper.GetCurrentUserLoginName();
                }
                return ViewState["CurrentUserName"].ToString();
            }

            set
            {
                ViewState["CurrentUserName"] = value;
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!Page.IsPostBack) new GorevTanimService().TryRecordWebPartOpened(LoggingContext);
                GorevSemasiGetir();
            }
            catch (Exception error)
            {
                ReportError(error);
            }
        }

        private void GorevSemasiGetir()
        {
            string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
            string newUrl = currentUrl.Substring(0, currentUrl.LastIndexOf("/"));
            var csChartConfig = System.Environment.NewLine + "chart_config = [";
            var csString = @" var config = {
                                container: '#GorevSemasiDiv',

                                connectors:
                                    {
                                        type: 'step'
                                },
                                node:
                                    {
                                        HTMLclass: 'nodeExample1'
                                }
                                },";

            csChartConfig += "config,";

            BirimTanim birimTanimDao = new BirimTanim();
            PortalLogContext context = LoggingContext;
            var birimListesi = new BirimTanimService().GetByBirimKaldirildi(false, context);
            Dictionary<int, Personel> managers = new PersonelService().GetSchemaManagers(context).ToDictionary(personel => personel.Id);
            Dictionary<int, List<Personel>> employeesByUnit = null;
            Dictionary<int, GorevTanim> jobsByPerson = null;
            {
                employeesByUnit = new Dictionary<int, List<Personel>>();
                DataTable employees = new PersonelService().GetSchemaEmployees(context);
                if (employees != null)
                {
                    foreach (DataRow row in employees.Rows)
                    {
                        if (row.IsNull("BirimId")) continue;
                        int unitId = Convert.ToInt32(row["BirimId"]);
                        int employeeId = Convert.ToInt32(row["Id"]);
                        List<Personel> unitEmployees;
                        if (!employeesByUnit.TryGetValue(unitId, out unitEmployees))
                        {
                            unitEmployees = new List<Personel>();
                            employeesByUnit.Add(unitId, unitEmployees);
                        }
                        if (unitEmployees.Any(person => person.Id == employeeId)) continue;
                        unitEmployees.Add(new Personel
                        {
                            Id = employeeId,
                            Adi = row["Adi"] as string,
                            Soyadi = row["Soyadi"] as string,
                            KullaniciAdi = row["KullaniciAdi"] as string
                        });
                    }
                }
                jobsByPerson = new GorevTanimService().GetAll(context)
                    .Where(job => job.PersonelId > 0)
                    .GroupBy(job => job.PersonelId)
                    .ToDictionary(group => group.Key, group => group.OrderBy(job => job.Id).First());
            }

            BirimTanim parent = birimListesi.Where(a => a.ParentId == 0).FirstOrDefault<BirimTanim>();
            Personel ilknode = new Personel();
            managers.TryGetValue(parent.AmirId, out ilknode);
            string gmKullaniciAdi = ilknode == null ? "" : ilknode.KullaniciAdi.ReturnEmptyIfNull().ToString();
            string gmAdi = ilknode == null ? "" : ilknode.Adi.ReturnEmptyIfNull().ToString();
            string gmSoyadi = ilknode == null ? "" : ilknode.Soyadi.ReturnEmptyIfNull().ToString();
            string gmAdiSoyadi = gmAdi + " " + gmSoyadi;

            string gmfotostr = string.IsNullOrEmpty(gmKullaniciAdi) ? (!string.IsNullOrEmpty(gmAdi) ? gmAdi.Substring(0, 1) + gmSoyadi : "") : gmKullaniciAdi;
            gmfotostr = string.IsNullOrEmpty(gmfotostr) ? "person" : gmfotostr;

            string gmimgUrl = newUrl + "/../" + ProjeConstants.RESIMLER_PERSONEL + "/" + gmfotostr.ReplaceTrChars() + ".jpg";
            string gmerrImgUrl = newUrl + "/../" + ProjeConstants.RESIMLER_PERSONEL + "/person.jpg";

            csString += "a" + parent.Id + @" = {
                        text: {
                            name: '" + parent.Adi + @"',
                            title: '" + gmAdiSoyadi + @"',
                            contact: '',
                        },
                        image: '" + gmimgUrl + @"',
                        onerror :this.src = '" + gmerrImgUrl + @"'
                    },";

            csChartConfig += "a" + parent.Id + ",";

            foreach (BirimTanim item in birimListesi)
            {
                Personel birimAmiri = new Personel();
                managers.TryGetValue(item.AmirId, out birimAmiri);
                if (birimAmiri == null)
                {
                    birimAmiri=new Personel();
                }
                string amirKullaniciAdi = birimAmiri == null ? "" : birimAmiri.KullaniciAdi.ReturnEmptyIfNull().ToString();
                string amirAdi = birimAmiri == null ? "" : birimAmiri.Adi.ReturnEmptyIfNull().ToString();
                string amirSoyadi = birimAmiri == null ? "" : birimAmiri.Soyadi.ReturnEmptyIfNull().ToString();
                string amirAdiSoyadi = amirAdi + " " + amirSoyadi;
                string fotostr = string.IsNullOrEmpty(amirKullaniciAdi) ? (!string.IsNullOrEmpty(amirAdi) ? amirAdi.Substring(0, 1) + amirSoyadi : "") : amirKullaniciAdi;
                fotostr = string.IsNullOrEmpty(fotostr) ? "person" : fotostr;
                string imgUrl = newUrl + "/../" + ProjeConstants.RESIMLER_PERSONEL + "/" + fotostr.ReplaceTrChars() + ".jpg";

                string errImgUrl = newUrl + "/../" + ProjeConstants.RESIMLER_PERSONEL + "/person.jpg";
                if (item.ParentId != 0)
                {
                    string aktifPasifStr = (!item.Aktif ? "nodeExample2" : "nodeExample1");
                    csString += "a" + item.Id + @" = {
                                parent: " + "a" + item.ParentId + @",
                                HTMLclass:'" + aktifPasifStr + @"',
                                text: {
                                    name: '" + item.Adi + @"',
                                    title: '" + amirAdiSoyadi + @"',
                                },
                                stackChildren: true,
                                image: '" + imgUrl + @"',
                                onerror :this.src = '" + errImgUrl + @"'
                            },";

                    csChartConfig += "a" + item.Id + ",";
                }


                Personel personelDao = new Personel();
                List<Personel> personelList;
                {
                    if (!employeesByUnit.TryGetValue(item.Id, out personelList))
                        personelList = new List<Personel>();
                }
                foreach (Personel personelItem in personelList)
                {
                    GorevTanim gorevTanim = new GorevTanim();
                    jobsByPerson.TryGetValue(personelItem.Id, out gorevTanim);
                    if (gorevTanim != null)
                    {
                        string gorevliKullaniciAdi = personelItem == null ? "" : personelItem.KullaniciAdi.ReturnEmptyIfNull().ToString();
                        string gorevliAdi = personelItem == null ? "" : personelItem.Adi.ReturnEmptyIfNull().ToString();
                        string gorevliSoyadi = personelItem == null ? "" : personelItem.Soyadi.ReturnEmptyIfNull().ToString();
                        string gorevliAdiSoyadi = gorevliAdi + " " + gorevliSoyadi;

                        string gorevliFotostr = string.IsNullOrEmpty(gorevliKullaniciAdi) ? (!string.IsNullOrEmpty(gorevliAdi) ? gorevliAdi.Substring(0, 1) + gorevliSoyadi : "") : gorevliKullaniciAdi;
                        gorevliFotostr = string.IsNullOrEmpty(gorevliFotostr) ? "person" : gorevliFotostr;
                        string gorevliImageUrl = newUrl + "/../" + ProjeConstants.RESIMLER_PERSONEL + "/" + gorevliFotostr.ReplaceTrChars() + ".jpg";

                        string gorevliErrImgUrl = newUrl + "/../" + ProjeConstants.RESIMLER_PERSONEL + "/person.jpg";

                        if (item.ParentId != 0)
                        {
                            if (personelItem.Id != birimAmiri.Id)
                            {
                                string aktifPasifStr = (!gorevTanim.Aktif ? "nodeExample2" : "nodeExample1");
                                csString += "b" + gorevTanim.Id + @" = {
                                parent: " + "a" + item.Id + @",
                                HTMLclass:'" + aktifPasifStr + @"',
                                text: {
                                    name: '" + gorevTanim.Adi + @"',
                                    title: '" + gorevliAdiSoyadi + @"',
                                },
                                stackChildren: true,
                                image: '" + gorevliImageUrl + @"',
                                onerror :this.src = '" + gorevliErrImgUrl + @"'
                            },";

                                csChartConfig += "b" + gorevTanim.Id + ",";
                            }
                        }
                    }

                }


            }

            csChartConfig = csChartConfig.Substring(0, csChartConfig.Length - 1);
            csChartConfig += "];" + System.Environment.NewLine;

            csString = csString.Substring(0, csString.Length - 1);

            csString += csChartConfig;
            csString += System.Environment.NewLine + "new Treant(chart_config);";


            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "test1", csString, true);
        }
    }
}
