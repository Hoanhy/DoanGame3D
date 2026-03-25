using UnityEngine;
using UnityEditor; // Thư viện này giúp can thiệp vào menu Unity

public class ClearPlayerPrefs
{
    // Dòng này tạo ra menu Tools trên thanh công cụ của Unity
    [MenuItem("Tools/Xoa Sach Du Lieu Game")]
    public static void DeleteAllPrefs()
    {
        // Lệnh xóa sạch sành sanh PlayerPrefs
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();

        // Hiện thông báo ở khung Console để mình biết đã xong
        Debug.Log("<color=red>★★★ ĐÃ XÓA SẠCH DỮ LIỆU (LEVEL, SETTINGS...) ★★★</color>");
    }
}