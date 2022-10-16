using Microsoft.SharePoint;
using System;
using System.ComponentModel;
using System.Data;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Utility.ProjeGlobal;

namespace Portal_WebParts.GunlukNobetcilerMoveWP
{
    [ToolboxItemAttribute(false)]
    public partial class GunlukNobetcilerMoveWP : WebPart
    {
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public GunlukNobetcilerMoveWP()
        {
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            InitializeControl();
            this.ChromeType = PartChromeType.None;
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            TitelLbl.Text = "Bugün Görevli Personel";
            GunlukNobetciGetir(NobetciRepeater);
            GunlukNobetciGetir(PopupRepeater);
        }
        private void GunlukNobetciGetir(Repeater repeater)
        {
            try
            {

                using (SPSite site = new SPSite(SPContext.Current.Site.Url))
                {
                    using (SPWeb web = site.OpenWeb())
                    {
                        if (web != null)
                        {
                            SPList docLib = web.Lists["Günlük Görevli Personel"];
                            SPQuery sorgu = new SPQuery();

                            sorgu.Query = "<Where><Eq><FieldRef Name='G_x00f6_rev_x0020_Tarihi' /><Value Type='DateTime'>" + DateTime.Now.ToString("yyyy-MM-dd") + "</Value></Eq></Where><OrderBy><FieldRef Name='Created' /></OrderBy>";
                            sorgu.ExpandUserField = true;
                            SPListItemCollection tumGorevler = docLib.GetItems(sorgu);
                            sorgu.RowLimit = 1;

                            if (tumGorevler.Count > 0)
                            {
                                SPListItem gorev = tumGorevler[0];

                                DataTable dt = new DataTable();
                                dt.Columns.Add("Bolum", typeof(String));
                                dt.Columns.Add("Isim", typeof(String));
                                dt.Columns.Add("KullaniciAdi", typeof(String));
                                dt.Columns.Add("PersonelResimleri", typeof(String));

                                if (gorev["Görevli Personel Mali İşl. ve İşt. Grp. Bşk. lığı (18:00-19:00)"] != null)
                                {
                                    SPFieldUserValue urs = new SPFieldUserValue(web, Convert.ToString(gorev["Görevli Personel Mali İşl. ve İşt. Grp. Bşk. lığı (18:00-19:00)"]));
                                    if (urs != null)
                                    {
                                        DataRow row = dt.NewRow();
                                        row["Bolum"] = "Mali İşl. ve İşt. Grp. Bşk. lığı ";
                                        row["Isim"] = Convert.ToString(urs.LookupValue);
                                        row["KullaniciAdi"] = urs.LoginName.Split('\\')[1];
                                        row["PersonelResimleri"] = ProjeConstants.PATH_RESIMLER_PERSONEL;
                                        dt.Rows.Add(row);
                                    }

                                    //MaliIsler = Convert.ToString(gorev["Görevli Personel Mali İşl. ve İşt. Grp. Bşk. lığı (18:00-19:00)"]).Split('|')[1];
                                    //DataRow row = dt.NewRow();
                                    //row["Bolum"] = "Mali Isl. ve Ist. Grp. Bsk. ligi";
                                    //UserProfile profile_mali = Utilities.Classes.Utilities.GetUserInfoFromProfile(MaliIsler,false);
                                    //row["Isim"] = Convert.ToString(profile_mali.DisplayName);
                                    //row["KullaniciAdi"] = MaliIsler.Split('\\')[1];
                                    //dt.Rows.Add(row);
                                }
                                //Muh Dir. ayrı nöbet tutarsa aşağısı açılacak
                                //if (gorev["Görevli Personel Muh. ve Fins.Dir.lüğü"] != null)
                                //{
                                //    SPFieldUserValue urs = new SPFieldUserValue(web, Convert.ToString(gorev["Görevli Personel Muh. ve Fins.Dir.lüğü"]));
                                //    if (urs != null)
                                //    {
                                //        DataRow row = dt.NewRow();
                                //        row["Bolum"] = "Muh. ve Fins. Dir. lugu";
                                //        row["Isim"] = Convert.ToString(urs.LookupValue);
                                //        row["KullaniciAdi"] = urs.LoginName.Split('\\')[1];
                                //        dt.Rows.Add(row);
                                //    }

                                //}

                                if (gorev["Görevli Personel Vakıf Hiz. Grp. Bşk. lığı (18:00-19:00)"] != null)
                                {
                                    SPFieldUserValue urs = new SPFieldUserValue(web, Convert.ToString(gorev["Görevli Personel Vakıf Hiz. Grp. Bşk. lığı (18:00-19:00)"]));
                                    if (urs != null)
                                    {
                                        DataRow row = dt.NewRow();
                                        row["Bolum"] = "Vakıf Hiz. Grp. Bşk. lığı";
                                        row["Isim"] = Convert.ToString(urs.LookupValue);
                                        row["KullaniciAdi"] = urs.LoginName.Split('\\')[1];
                                        row["PersonelResimleri"] = ProjeConstants.PATH_RESIMLER_PERSONEL;
                                        dt.Rows.Add(row);
                                    }
                                    //VakifHizmetleri = Convert.ToString(gorev["Görevli Personel Vakıf Hiz. Grp. Bşk. lığı (18:00-19:00)"]).Split('|')[1];
                                    //DataRow row1 = dt.NewRow();
                                    //row1["Bolum"] = "Vakif Hiz. Grp. Bsk. ligi";
                                    //UserProfile profile_vakif = Utilities.Classes.Utilities.GetUserInfoFromProfile(VakifHizmetleri,false);
                                    //row1["Isim"] = Convert.ToString(profile_vakif.DisplayName);
                                    //row1["KullaniciAdi"] = VakifHizmetleri.Split('\\')[1];
                                    //dt.Rows.Add(row1);
                                }

                                if (gorev["Görevli Personel Per. ve İd. İşl. Ş. Md.lüğü (18:00-19:00)"] != null)
                                {
                                    SPFieldUserValue urs = new SPFieldUserValue(web, Convert.ToString(gorev["Görevli Personel Per. ve İd. İşl. Ş. Md.lüğü (18:00-19:00)"]));
                                    if (urs != null)
                                    {
                                        DataRow row = dt.NewRow();
                                        row["Bolum"] = "Per. ve İd. İşl. Ş. Md.lüğü";
                                        row["Isim"] = Convert.ToString(urs.LookupValue);
                                        row["KullaniciAdi"] = urs.LoginName.Split('\\')[1];
                                        row["PersonelResimleri"] = ProjeConstants.PATH_RESIMLER_PERSONEL;
                                        dt.Rows.Add(row);
                                    }
                                    //PersonelIdariIsler = Convert.ToString(gorev["Görevli Personel Per. ve İd. İşl. Ş. Md.lüğü (18:00-19:00)"]).Split('|')[1];
                                    //DataRow row2 = dt.NewRow();
                                    //row2["Bolum"] = "Per. ve Id. Isl. S. Md.lügü";
                                    //UserProfile profile_personel = Utilities.Classes.Utilities.GetUserInfoFromProfile(PersonelIdariIsler,false);
                                    //row2["Isim"] = Convert.ToString(profile_personel.DisplayName);
                                    //row2["KullaniciAdi"] = PersonelIdariIsler.Split('\\')[1];
                                    //dt.Rows.Add(row2);
                                }

                                if (gorev["Hizmetli Personel (18:00-19:00)"] != null)
                                {
                                    SPFieldUserValue urs = new SPFieldUserValue(web, Convert.ToString(gorev["Hizmetli Personel (18:00-19:00)"]));
                                    if (urs != null)
                                    {
                                        DataRow row = dt.NewRow();
                                        row["Bolum"] = "Hizmetli Personel";
                                        row["Isim"] = Convert.ToString(urs.LookupValue);
                                        row["KullaniciAdi"] = urs.LoginName.Split('\\')[1];
                                        row["PersonelResimleri"] = ProjeConstants.PATH_RESIMLER_PERSONEL;
                                        dt.Rows.Add(row);
                                    }
                                    //HizmetliPersonel = Convert.ToString(gorev["Hizmetli Personel (18:00-19:00)"]).Split('|')[1];
                                    //DataRow row3 = dt.NewRow();
                                    //row3["Bolum"] = "Hizmetli Personel";
                                    //UserProfile profile_hizmetli = Utilities.Classes.Utilities.GetUserInfoFromProfile(HizmetliPersonel,false);
                                    //row3["Isim"] = Convert.ToString(profile_hizmetli.DisplayName);
                                    //row3["KullaniciAdi"] = HizmetliPersonel.Split('\\')[1];
                                    //dt.Rows.Add(row3);
                                }

                                repeater.DataSource = dt;
                                repeater.DataBind();
                            }
                            else
                            {
                                ulUsers.Visible = false;
                                divEmptyUsers.Visible = true;
                            }
                        }
                        else
                        {
                            ulUsers.Visible = false;
                            divEmptyUsers.Visible = true;
                        }
                    }
                }




            }
            catch (Exception )
            {
                ulUsers.Visible = false;
                divEmptyUsers.Visible = true;
            }
        }
    }
}
