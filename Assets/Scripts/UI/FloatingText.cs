using UnityEngine;
using TMPro;


public class FloatingText : MonoBehaviour
{
    //public string textToDisplay = "Hello!";
    private TextMeshProUGUI textMesh;
    private Camera camera;
    private Canvas canvas;
    [SerializeField] GameObject panel;

    private void Start()
    {
        canvas = GetComponentInParent<Canvas>();
        //camera = GameObject.Find("CameraRestaurant").GetComponent<Camera>();
        //canvas.worldCamera = camera;
        // Get the TextMeshPro component attached to this game object
        textMesh = GetComponent<TextMeshProUGUI>();
    }

    public void DisableMessageBox()
    {
        textMesh.text = "";
        panel.SetActive(false);
    }

    public void GenerateMessageBox(string name)
    {
        panel.SetActive(true);
        textMesh.text = name;
    }
}
