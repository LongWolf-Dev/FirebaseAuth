using UnityEngine;
using UnityEngine.UI;
using VictorDev.FirebaseUtils;

public class GameManager : MonoBehaviour
{
    public FirebaseSignInManager signInManager;

    public Button btnSignIn;

    public GameObject page_SignIn, page_AccountInfo;
    public Text txtDisplayName, txtEmail;
    public Image imgPhoto;


    private void Awake()
    {
        btnSignIn.onClick.AddListener(signInManager.GoogleSignIn);
        signInManager.onSignInSuccessed.AddListener(OnSuccessed);
        signInManager.onLoadPhotoCompleted.AddListener((sprite) => imgPhoto.sprite = sprite);
    }

    private void OnSuccessed()
    {
        page_SignIn.SetActive(false);
        page_AccountInfo.SetActive(true);

        txtDisplayName.text = signInManager.DisplayName;
        txtEmail.text = signInManager.Email;
    }
}
