using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    public GameObject hitUI;
    public GameObject deathUI;

    public GameObject menuInicialUI;

    public GameObject comoJogarUI;

    public TextMeshProUGUI inimigosDerrotadosText;
    public int quantidadeInimigosDerrotados = 0;
    public TextMeshProUGUI municaoTexto;
    public Image barraSaude;
    public Gradient gradienteSaude;

    public void Start()
    {
        // começa com o jogo pausado 
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        menuInicialUI.SetActive(true);
    }

    public void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

   
    public void StartGame()
    {
        Time.timeScale = 1.0f; // Despausa o jogo
        menuInicialUI.SetActive(false); // Esconde o menu
       
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void InstantiateHitUI()
    {
        Instantiate(hitUI, transform);
    }

    public void RestartGame()
    {
        Time.timeScale = 1.0f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void EnableDeathUI()
    {
        deathUI.SetActive(true);
    }

    public void SetHealthValue(int health)
    {
        float floatHealth = Mathf.Clamp01((float)health / 100f);
        if (barraSaude != null)
        {
            barraSaude.color = gradienteSaude.Evaluate(floatHealth);
            barraSaude.fillAmount = floatHealth;
        }
    }

    public void EnableComoJogarUI()
    {
        menuInicialUI.SetActive(false);
        comoJogarUI.SetActive(true);
    }

    public void DisableComoJogarUI()
    {
        comoJogarUI.SetActive(false);
        menuInicialUI.SetActive(true);
    }
}