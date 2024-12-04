using UnityEngine;
using UnityEngine.UI;


public class PlayerStatusHandler : MonoBehaviour
{
    [Header("Stamina Parameters")]
    public float playerStamina = 100f;
    [SerializeField] private float maxStamina = 100f;

    [Range(0, 50)] [SerializeField] private float staminaDrain = 0.5f; //for sprinting 
    [Range(0, 50)] [SerializeField] private float staminaRegen = 0.5f;

    [SerializeField] private Image staminaProgressUI = null;
    [SerializeField] private CanvasGroup sliderCanvasGroup = null;

    [Header("Actions")]
    [SerializeField] private float DashCost = 20;
    public bool canDash;

    [Header("References")]
    private Dashing dashingScript;

    private void Start()
    {
        dashingScript = GetComponent<Dashing>();
    }

    private void Update()
    {

        CheckActionAvailablity();

        UpdateStaminaUI();

        if (!dashingScript.dashing) 
        {
            RegenrateStamina();
        }

    }

    private void CheckActionAvailablity()
    {
        if ( playerStamina >= DashCost )
        {
            canDash = true;
        }
        else
        {
            canDash = false;
        }
    }

    private void RegenrateStamina()
    {
        if (playerStamina < maxStamina)
        {
            playerStamina += staminaRegen * Time.deltaTime;
            playerStamina = Mathf.Clamp(playerStamina, 0f, maxStamina);
        }
    }

    public void StaminaDash()
    {
        playerStamina -= DashCost;
    }

    
    private void UpdateStaminaUI(int value = 0)
    {
        staminaProgressUI.fillAmount = playerStamina / maxStamina;

        //for hiding and showing the bar when needed
        /**
        if (value == 0)
        {
            sliderCanvasGroup.alpha = 0;
        }
        else
        {
            sliderCanvasGroup.alpha = 1;
        }
        **/
    }
}
