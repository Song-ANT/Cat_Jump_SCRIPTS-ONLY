using System;
using UnityEngine;



public class Mail_Btn_SubUI : MonoBehaviour
{
    [Obsolete("Obsolete")]
    public void Contact()
    {
        string toMail = "actionfitspt@gmail.com";
        string subject = EscapeURL(GetSubject());
        string body = EscapeURL("\n\n\n\n" + GetDeviceInfo());
        Application.OpenURL($"mailto:{toMail}?subject={subject}&body={body}");
    }

    public void Contact(string feedback)
    {
        string toMail = "actionfitspt@gmail.com";
        string subject = EscapeURL(GetSubject());
        string body = EscapeURL(feedback + "\n\n\n\n" + GetDeviceInfo());
        Application.OpenURL($"mailto:{toMail}?subject={subject}&body={body}");
    }

    [Obsolete("Obsolete")]
    private string EscapeURL(string url)
    {
        return WWW.EscapeURL(url).Replace("+", "%20");
    }

    private string GetSubject()
    {
        return Application.systemLanguage switch
        {
            SystemLanguage.Korean => "점주에게 문의하기",
            SystemLanguage.English => "Inquiry to the Store Owner",
            SystemLanguage.ChineseTraditional => "对店主的咨询",
            SystemLanguage.Japanese => "店主へのお問い合わせ",
            _ => "Inquiry"
        };
    }

    private string GetDeviceInfo()
    {
        return $"Level: {Data_Manager.Instance.BestScore}\n" +
               $"Device Model: {SystemInfo.deviceModel}\n" +
               $"Device OS: {SystemInfo.operatingSystem}\n" +
               $"Version_Platform: {Application.version}_{UnityEngine.Device.Application.platform}";
    }
}

