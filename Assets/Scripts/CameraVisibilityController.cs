using UnityEngine;

public class CameraVisibilityController : MonoBehaviour
{
    [SerializeField]private Camera cameraKitchen;
    [SerializeField] private Camera cameraDeposit;

    public void EnableKitchenCamera()
    {
        cameraDeposit.gameObject.SetActive(false);
        cameraKitchen.gameObject.SetActive(true);
    }

    public void DisableSecondaryCameras()
    {
        cameraDeposit.gameObject.SetActive(false);
        cameraKitchen.gameObject.SetActive(false);
    }

    public void EnableDepositCamera()
    {
        cameraKitchen.gameObject.SetActive(false);
        cameraDeposit.gameObject.SetActive(true);
    }
}
