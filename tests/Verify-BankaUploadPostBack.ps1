$ErrorActionPreference = 'Stop'

# Run with Windows PowerShell (the application targets .NET Framework 4.8).
Add-Type -AssemblyName System.Web
$sourcePath = Join-Path $PSScriptRoot '..\NBYS_WebParts\BankaDosyasiYuklemeWP\BankaDosyasiYuklemeWP.ascx.cs'
$source = Get-Content -LiteralPath $sourcePath -Raw -Encoding UTF8
function Get-SourceMethod([string]$startMarker, [string]$endMarker) {
    $start = $source.IndexOf($startMarker)
    $end = $source.IndexOf($endMarker, $start)
    if ($start -lt 0 -or $end -le $start) { throw "Method not found: $startMarker" }
    $source.Substring($start, $end - $start)
}
$init = Get-SourceMethod 'protected override void OnInit(EventArgs e)' 'bool isAkbankAktarildi'
$calendar = Get-SourceMethod 'private void AddDateCalendar(DateTime today)' 'private void InitializeLabels()'
$dateChanged = Get-SourceMethod 'protected void islemTarihiTxt_TextChanged(object sender, EventArgs e)' 'private void RedirectWithTarih(string pageUrl)'
$code = @'
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Reflection;
using System.IO;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;

public class UploadPostBackProbe : WebPart {
    private LiteralControl uploadCalendar;
    public readonly TextBox IslemTarihiTxt = new TextBox { ID = "IslemTarihiTxt", AutoPostBack = true };
    public readonly LinkButton Save = new LinkButton { ID = "KaydetBtn" };
    public readonly Label BankLabel = new Label { ID = "AkbankLbl" };
    public readonly FileUpload UploadedFile = new FileUpload { ID = "AkbankFU" };
    public bool DateChanged;
    public int SaveClicks;
    public void InitializeForTest(bool legacy) {
        OnInit(EventArgs.Empty);
        if (legacy) Controls.Remove(uploadCalendar);
    }
    private void InitializeControl() {
        // Same child order as __BuildControlTree in the generated WebPart.
        Controls.Add(new LiteralControl("<header>"));
        Controls.Add(new LinkButton { ID = "CloseBtn" });
        Controls.Add(new LiteralControl("</header><div>"));
        Controls.Add(IslemTarihiTxt);
        Controls.Add(new LiteralControl("</div>"));
        Controls.Add(BankLabel);
        Controls.Add(new LiteralControl("<div>"));
        Controls.Add(UploadedFile);
        Controls.Add(new LiteralControl("</div>"));
        Controls.Add(Save);
        IslemTarihiTxt.TextChanged += islemTarihiTxt_TextChanged;
        Save.Click += delegate { SaveClicks++; };
    }
    public void RenderCalendar() { AddDateCalendar(new DateTime(2026, 10, 15)); }
    public string CalendarMarkup { get { return uploadCalendar.Text; } }
    private void RedirectWithTarih(string pageUrl) { DateChanged = true; throw new DateRedirect(); }
    // Compile the actual production methods, not copies of the fixed implementation.
    __INIT__
    __CALENDAR__
    __DATE_CHANGED__
}

public sealed class DateRedirect : Exception { }
// Replace only external database/configuration dependencies in this regression test.
public sealed class EkstreAktarmaService {
    public IEnumerable<DateTime> GetProcessDatesWithUploads(DateTime start, DateTime end) {
        return new[] { new DateTime(2026, 10, 14) };
    }
}
public static class ProjeConstants {
    public const string DATE_TR = "dd.MM.yyyy";
    public const string PAGE_BANKA_DOSYAYUKLEME = "upload.aspx";
}
public static class DateExtensions {
    public static DateTime ConvertToDatetime(this string value) {
        return DateTime.ParseExact(value, "dd.MM.yyyy", CultureInfo.InvariantCulture);
    }
}

public static class UploadPostBackRegression {
    private const BindingFlags Internal = BindingFlags.Instance | BindingFlags.NonPublic;
    private static readonly MethodInfo Track = typeof(Control).GetMethod("TrackViewState", Internal);
    private static readonly MethodInfo SaveState = typeof(Control).GetMethod("SaveViewStateRecursive", Internal);
    private static readonly MethodInfo LoadState = typeof(Control).GetMethod("LoadViewStateRecursive", Internal);

    private static UploadPostBackProbe Create(bool postback, bool legacy) {
        var page = new Page { EnableEventValidation = false }; // Test has no rendered __EVENTVALIDATION.
        var request = new HttpRequest("", "http://localhost/upload.aspx", "");
        typeof(Page).GetField("_context", Internal).SetValue(page,
            new HttpContext(request, new HttpResponse(new StringWriter())));
        typeof(Page).GetField("_request", Internal).SetValue(page, request);
        if (postback) typeof(Page).GetField("_requestValueCollection", Internal)
            .SetValue(page, new NameValueCollection());
        var form = new HtmlForm { ID = "Form" };
        page.Controls.Add(form);
        var probe = new UploadPostBackProbe { ID = "Upload" };
        form.Controls.Add(probe);
        probe.InitializeForTest(legacy);
        TrackTree(probe);
        return probe;
    }

    private static void TrackTree(Control control) {
        Track.Invoke(control, null);
        foreach (Control child in control.Controls) TrackTree(child);
    }

    public static string ReproduceLateCalendarInsertion() {
        var initial = Create(false, true);
        initial.IslemTarihiTxt.Text = "15.10.2026";
        initial.Controls.AddAt(2, new LiteralControl("calendar"));
        var state = SaveState.Invoke(initial, new object[] { ViewStateMode.Enabled });
        var postback = Create(true, true);
        LoadState.Invoke(postback, new object[] { state });
        var restoredDate = postback.IslemTarihiTxt.Text;
        DispatchPostback(postback, "15.10.2026", true);
        if (!postback.DateChanged || postback.SaveClicks != 0)
            throw new Exception("The late calendar insertion did not reproduce the reported failure.");
        return "REPRODUCED: restored date='" + restoredDate + "'; unchanged posted date raises TextChanged; redirect prevents KaydetBtn_Click.";
    }

    private static void DispatchPostback(UploadPostBackProbe probe, string date, bool save) {
        var values = new NameValueCollection { { probe.IslemTarihiTxt.UniqueID, date } };
        bool changed = ((IPostBackDataHandler)probe.IslemTarihiTxt).LoadPostData(probe.IslemTarihiTxt.UniqueID, values);
        try {
            // WebForms raises changed-data events before the button event.
            if (changed) ((IPostBackDataHandler)probe.IslemTarihiTxt).RaisePostDataChangedEvent();
            if (save) ((IPostBackEventHandler)probe.Save).RaisePostBackEvent("");
        } catch (DateRedirect) { }
    }

    public static string VerifyFixedRoundTrips() {
        var current = Create(false, false);
        current.IslemTarihiTxt.Text = "15.10.2026";
        current.BankLabel.Text = "Akbank";
        current.UploadedFile.Enabled = false;
        for (int i = 0; i < 3; i++) {
            int dateIndex = current.Controls.IndexOf(current.IslemTarihiTxt);
            int count = current.Controls.Count;
            current.RenderCalendar();
            current.RenderCalendar();
            if (count != current.Controls.Count || dateIndex != current.Controls.IndexOf(current.IslemTarihiTxt))
                throw new Exception("Calendar rendering still changes the control tree.");
            if (!current.CalendarMarkup.Contains("bdy-calendar-day--uploaded") || !current.CalendarMarkup.Contains("?IslemTarihi="))
                throw new Exception("Calendar links/upload markers were lost.");
            var state = SaveState.Invoke(current, new object[] { ViewStateMode.Enabled });
            var next = Create(true, false);
            LoadState.Invoke(next, new object[] { state });
            if (next.IslemTarihiTxt.Text != "15.10.2026" || next.BankLabel.Text != "Akbank" || next.UploadedFile.Enabled)
                throw new Exception("ViewState did not restore the date/label/upload status.");
            DispatchPostback(next, "15.10.2026", true);
            if (next.DateChanged || next.SaveClicks != 1)
                throw new Exception("Unchanged date must reach the save event exactly once.");
            current = next;
        }
        return "PASS: three save postbacks reach Click exactly once; date/label/upload ViewState preserved; calendar rendering leaves child indexes unchanged.";
    }

    public static string VerifyRealDateChange() {
        var initial = Create(false, false);
        initial.IslemTarihiTxt.Text = "15.10.2026";
        initial.RenderCalendar();
        var state = SaveState.Invoke(initial, new object[] { ViewStateMode.Enabled });
        var next = Create(true, false);
        LoadState.Invoke(next, new object[] { state });
        DispatchPostback(next, "16.10.2026", false);
        if (!next.DateChanged || next.SaveClicks != 0)
            throw new Exception("Changing the process date must still redirect without saving.");
        return "PASS: a real date change still redirects without starting an upload.";
    }
}
'@
$code = $code.Replace('__INIT__', $init).Replace('__CALENDAR__', $calendar).Replace('__DATE_CHANGED__', $dateChanged)
Add-Type -TypeDefinition $code -ReferencedAssemblies System.Web,System.Drawing
[UploadPostBackRegression]::ReproduceLateCalendarInsertion()
[UploadPostBackRegression]::VerifyFixedRoundTrips()
[UploadPostBackRegression]::VerifyRealDateChange()
