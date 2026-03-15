using UnityEngine;

public class EquipmentButton : MonoBehaviour
{
    public EquipmentData Equipment;
    public StatusUI StatusUI;

    /// <summary>
    /// ƒ{ƒ^ƒ“‚ÌOnClick‚ÅŒÄ‚Î‚ê‚é‘•”õˆ—
    /// </summary>
    public void OnClickEquip()
    {
        if(Equipment == null)
        {
            return;
        }
        if(EquipmentManager.Instance == null)
        {
            return;
        }

        EquipmentManager.Instance.Equip(Equipment);

        if(StatusUI != null)
        {
            StatusUI.Refresh();
        }

    }
}
