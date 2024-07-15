using Firebase;
using Firebase.Auth;
using Firebase.Extensions;
using Google;
using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace VictorDev.FirebaseUtils
{

    public class FirebaseSignInManager : MonoBehaviour
    {
        [Header(">>> Google API: �����Τ��ID")]
        [SerializeField] private string GoogleAPI = "230591047517-o6k2qdog1giq9bcufu3coqhlk2umhvjq.apps.googleusercontent.com";

        /// <summary>
        /// ��n�J���\�ɵo�e
        /// </summary>
        [Header(">>> ��n�J���\�ɵo�e")]
        public UnityEvent onSignInSuccessed;
        /// <summary>
        /// ��ӤH�Y��Ū�������ɵo�e
        /// </summary>
        [Header(">>> ��ӤH�Y��Ū�������ɵo�e")]
        public UnityEvent<Sprite> onLoadPhotoCompleted;
        /// <summary>
        /// ��n�J���Ѯɵo�e
        /// </summary>
        [Header(">>> ��n�J���Ѯɵo�e")]
        public UnityEvent onSignInFailed;
        /// <summary>
        /// ��n�J�����ɵo�e
        /// </summary>
        [Header(">>> ��n�J�����ɵo�e")]
        public UnityEvent onSignInCancelled;

        private GoogleSignInConfiguration configuration;
        DependencyStatus dependencyStatus = DependencyStatus.UnavailableOther;
        FirebaseAuth auth;
        FirebaseUser user;

        /// <summary>
        /// ��ܦW��
        /// </summary>
        public string DisplayName => user.DisplayName;
        /// <summary>
        /// ���Email
        /// </summary>
        public string Email => user.Email;
        /// <summary>
        /// �ӤH�Y��URL
        /// </summary>
        public Uri PhotoUrl => user.PhotoUrl;
        /// <summary>
        /// �ӤH�Y��Sprite
        /// </summary>
        public Sprite Photo { get; private set; }

        private void Awake()
        {
            configuration = new GoogleSignInConfiguration
            {
                WebClientId = GoogleAPI,
                RequestIdToken = true,
            };
        }

        private void Start() => InitFirebase();

        void InitFirebase() => auth = FirebaseAuth.DefaultInstance;

        /// <summary>
        /// Google�n�J
        /// </summary>
        public void GoogleSignIn()
        {
            Google.GoogleSignIn.Configuration = configuration;
            Google.GoogleSignIn.Configuration.UseGameSignIn = false;
            Google.GoogleSignIn.Configuration.RequestIdToken = true;
            Google.GoogleSignIn.Configuration.RequestEmail = true;

            Google.GoogleSignIn.DefaultInstance.SignIn().ContinueWith(OnGoogleAuthenticatedFinished);
        }

        void OnGoogleAuthenticatedFinished(Task<GoogleSignInUser> task)
        {
            if (task.IsFaulted)
            {
                onSignInFailed?.Invoke();
                Debug.LogError("Faulted");
            }
            else if (task.IsCanceled)
            {
                onSignInCancelled?.Invoke();
                Debug.LogError("Cancelled");
            }
            else
            {
                Credential credential = GoogleAuthProvider.GetCredential(task.Result.IdToken, null);

                auth.SignInWithCredentialAsync(credential).ContinueWithOnMainThread(task =>
                {
                    if (task.IsCanceled)
                    {
                        onSignInCancelled?.Invoke();
                        return;
                    }

                    if (task.IsFaulted)
                    {
                        onSignInFailed?.Invoke();
                        Debug.LogError("SignInWithCredentialAsync encountered an error: " + task.Exception);
                        return;
                    }
                    user = auth.CurrentUser;
                    onSignInSuccessed?.Invoke();

                    StartCoroutine(LoadImage(user.PhotoUrl.ToString()));
                });
            }
        }

        /// <summary>
        /// Ū���ӤH�Y���Ϥ�
        /// </summary>
        IEnumerator LoadImage(string imageUri)
        {
            WWW www = new WWW(imageUri);
            yield return www;
            Photo = Sprite.Create(www.texture, new Rect(0, 0, www.texture.width, www.texture.height), new Vector2(0, 0));
            onLoadPhotoCompleted?.Invoke(Photo);
        }
    }
}