using UnityEngine; 
using UnityEditor; 
using TMPro; 
using UnityEngine.UI; 

public class CreateUIPrefab { 
    [MenuItem("Tools/Create Enemy UI Prefab")] 
    static void CreatePrefab() { 
        GameObject canvasObj = new GameObject("EnemyHPUI"); 
        Canvas canvas = canvasObj.AddComponent<Canvas>(); 
        canvas.renderMode = RenderMode.WorldSpace; 
        
        RectTransform rt = canvas.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(250, 100); 
        // WorldSpaceで頭上に自然に収まるようにさらにスケールを小さくし、Pivotを中央に配置する
        rt.localScale = new Vector3(0.005f, 0.005f, 0.005f);
        rt.pivot = new Vector2(0.5f, 0.5f);
        
        canvasObj.AddComponent<CanvasScaler>(); 
        
        GameObject bgObj = new GameObject("Background"); 
        bgObj.transform.SetParent(canvasObj.transform, false); 
        Image bgImage = bgObj.AddComponent<Image>(); 
        bgImage.color = new Color(0f, 0f, 0f, 0.5f); 
        bgObj.GetComponent<RectTransform>().anchorMin = Vector2.zero; 
        bgObj.GetComponent<RectTransform>().anchorMax = Vector2.one; 
        bgObj.GetComponent<RectTransform>().sizeDelta = Vector2.zero; 
        bgObj.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        
        GameObject nameObj = new GameObject("NameText"); 
        nameObj.transform.SetParent(canvasObj.transform, false); 
        TextMeshProUGUI nameText = nameObj.AddComponent<TextMeshProUGUI>(); 
        nameText.text = "Enemy Name"; 
        nameText.alignment = TextAlignmentOptions.Bottom; 
        nameText.fontSize = 24; 
        nameObj.GetComponent<RectTransform>().anchorMin = new Vector2(0, 0.5f); 
        nameObj.GetComponent<RectTransform>().anchorMax = new Vector2(1, 1); 
        nameObj.GetComponent<RectTransform>().sizeDelta = Vector2.zero; 
        
        GameObject hpObj = new GameObject("HPText"); 
        hpObj.transform.SetParent(canvasObj.transform, false); 
        TextMeshProUGUI hpText = hpObj.AddComponent<TextMeshProUGUI>(); 
        hpText.text = "HP: 10/10"; 
        hpText.alignment = TextAlignmentOptions.Top; 
        hpText.fontSize = 24; 
        hpObj.GetComponent<RectTransform>().anchorMin = new Vector2(0, 0); 
        hpObj.GetComponent<RectTransform>().anchorMax = new Vector2(1, 0.5f); 
        hpObj.GetComponent<RectTransform>().sizeDelta = Vector2.zero; 
        
        EnemyUI uiScript = canvasObj.AddComponent<EnemyUI>(); 
        uiScript.NameText = nameText; 
        uiScript.HPText = hpText; 
        
        PrefabUtility.SaveAsPrefabAsset(canvasObj, "Assets/Art/Prefabs/EnemyHPUI.prefab"); 
        Object.DestroyImmediate(canvasObj); 
        Debug.Log("EnemyHPUI prefab created successfully with correct scale."); 
    } 
}
