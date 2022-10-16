using Model.IKYS;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.Web.Script.Serialization;
using System.Web.UI.WebControls.WebParts;
using Utility.HelperClasses;
using Utility.ProjeGlobal;

namespace IKYS_WebParts.ProtokolSirasiWP
{
    [ToolboxItemAttribute(false)]
    public partial class ProtokolSirasiWP : WebPart
    {
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public ProtokolSirasiWP()
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
            if (!Page.IsPostBack)
            {
                KayitGetir();
            }
        }
        private void KayitGetir()
        {
            var jsonData = PersonelJson(); //veri çekilip json a çeviriliyor
            var jsString = CreateJsString(jsonData); //javascript kodu hazırlanıyor.
            System.Web.UI.ScriptManager.RegisterStartupScript((System.Web.UI.Page)System.Web.HttpContext.Current.Handler, typeof(System.Web.UI.Page), System.Guid.NewGuid().ToString(), jsString, true);
        }
        private string PersonelJson()
        {
            string jSon = string.Empty;

            List<PersonelListItem> list = GetDataList();
            var serializer = new JavaScriptSerializer();
            serializer.MaxJsonLength = Int32.MaxValue;
            jSon = serializer.Serialize(list);
            return jSon;
        }
        private List<PersonelListItem> GetDataList()
        {
            IFormatProvider culturInfo = new CultureInfo(ProjeConstants.CULTUREINFO, true);
            DateTime islemTarihiDateTime = new DateTime(2020, 1, 9);

            Personel personelDao = new Personel();
            DataTable dataTable = personelDao.SelectCalisanPersonelReturnDataTable();

            List<PersonelListItem> returnlist = new List<PersonelListItem>();
            if (dataTable != null)
            {
                foreach (DataRow dataRow in dataTable.Rows)
                {
                    PersonelListItem personelListItem = new PersonelListItem();

                    personelListItem.ProtokolSiraNo = dataRow["ProtokolSiraNo"].ToString();
                    personelListItem.PersonelId = dataRow["PersonelId"].ToString();
                    personelListItem.Adi = dataRow["Adi"].ToString();
                    personelListItem.Soyadi = dataRow["Soyadi"].ToString();
                    personelListItem.SicilNo = dataRow["SicilNo"].ToString();
                    personelListItem.Unvan = dataRow["Unvan"].ToString();
                    personelListItem.Gorev = dataRow["Gorev"].ToString();
                    personelListItem.BirimSube = dataRow["BirimSube"].ToString();
                    returnlist.Add(personelListItem);
                }
            }
            return returnlist;
        }
        private string CreateJsString(string jsonData)
        {

            //return '<span class=bagis-iade-edildi>'+rowData.IadeMiktari+ ' '+rowData.DovizCinsi+ ' Parası İade edildi</span>';
            string ekstretablestr = @" 
                $('#tblfilter').puidatatable({
                caption: '',
                editMode: 'cell',
                selectionMode: 'single',
                columns: [
                    { field: 'ProtokolSiraNo', headerText: 'S.No', headerStyle: 'width: 5%', bodyClass:'text-right'},
                    { field: 'PersonelId', headerText: 'P.Id', headerStyle: 'width: 7%', bodyClass:'text-right'},
                    { field: 'Adi', headerText: 'Adı', headerStyle: 'width: 12%' },
                    { field: 'Soyadi', headerText: 'Soyadı', headerStyle: 'width: 13%' },
                    { field: 'Unvan', headerText: 'Ünvan', headerStyle: 'width: 15%' },
                    { field: 'Gorev', headerText: 'Görev', headerStyle: 'width: 25%' },
                    { field: 'BirimSube', headerText: 'Birim', headerStyle: 'width: 28%' },
                    ],
                    datasource:" + jsonData + @",
                    draggableRows:true,
                    resizableColumns: true,
                    globalFilter:'#globalFilter'
                });
                $('#messages').puigrowl();
            ";
            return ekstretablestr;
        }
        protected void CloseBtn_Click(object sender, EventArgs e)
        {
            try
            {
                string currentUrl = System.Web.HttpContext.Current.Request.Url.ToString();
                string newUrl = currentUrl.Substring(0, currentUrl.LastIndexOf("/")) + "/" + ProjeConstants.PAGE_HOME;
                Page.Response.Redirect(newUrl);
            }
            catch (Exception ex)
            {
                ExceptionHelper exHelper = new ExceptionHelper(ex);
                exHelper.PublishException();
            }
        }
        private class PersonelListItem
        {
            public string ProtokolSiraNo { get; set; }
            public string PersonelId { get; set; }
            public string Adi { get; set; }
            public string Soyadi { get; set; }
            public string SicilNo { get; set; }
            public string Unvan { get; set; }
            public string Gorev { get; set; }
            public string BirimSube { get; set; }
        }

        protected void KaydetNowBtn_Click(object sender, EventArgs e)
        {
            try
            {
                string value = paramProtokolArray.Value;
                string[] idList = value.Split(',');
                int sira = 1;
                foreach (string item in idList)
                {
                    int personelId = item.ConvertToInt();
                    if (personelId > 0)
                    {
                        IsBilgileri isb = new IsBilgileri();
                        isb = isb.SelectByPersonelId(personelId);
                        isb.ProtokolSiraNo = sira++;
                        isb.Update();
                    }

                }
                KayitGetir();
                MessageHelper.PublishMessage("Sıralama kaydedildi", ProjeConstants.MESAJ_BASARILI, 2000);
            }
            catch (Exception)
            {
                MessageHelper.PublishMessage("Sıralama kaydedilemedi", ProjeConstants.MESAJ_HATA);
            }

        }
    }
}
