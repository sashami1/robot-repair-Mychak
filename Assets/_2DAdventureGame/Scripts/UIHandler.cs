using UnityEngine;
using UnityEngine.UIElements;

public class UIHandler : MonoBehaviour
{
    public static UIHandler instance { get; private set; }

    public float displayTime = 4.0f;

    // Оголошення змінних класу
    private VisualElement m_Healthbar;
    private VisualElement m_NonPlayerDialogue;
    private Label m_DialogueText;
    private VisualElement m_WinScreen;
    private VisualElement m_LoseScreen;
    private float m_TimerDisplay;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        UIDocument uiDocument = GetComponent<UIDocument>();
        if (uiDocument != null && uiDocument.rootVisualElement != null)
        {
            // Пошук смуги здоров'я
            m_Healthbar = uiDocument.rootVisualElement.Q<VisualElement>("HealthBar");
            SetHealthValue(1.0f);

            // Пошук діалогового вікна (підтримує назви DialogueBox та NPCDialogue)
            m_NonPlayerDialogue = uiDocument.rootVisualElement.Q<VisualElement>("DialogueBox")
                                 ?? uiDocument.rootVisualElement.Q<VisualElement>("NPCDialogue");
            m_DialogueText = uiDocument.rootVisualElement.Q<Label>("DialogueText");

            if (m_NonPlayerDialogue != null)
            {
                m_NonPlayerDialogue.style.display = DisplayStyle.None;
            }

            m_TimerDisplay = -1.0f;

            // Пошук екранів перемоги та поразки
            m_LoseScreen = uiDocument.rootVisualElement.Q<VisualElement>("LoseScreenContainer");
            m_WinScreen = uiDocument.rootVisualElement.Q<VisualElement>("WinScreenContainer");
        }
    }

    private void Update()
    {
        if (m_TimerDisplay > 0)
        {
            m_TimerDisplay -= Time.deltaTime;
            if (m_TimerDisplay <= 0 && m_NonPlayerDialogue != null)
            {
                m_NonPlayerDialogue.style.display = DisplayStyle.None;
            }
        }
    }

    public void SetHealthValue(float percentage)
    {
        if (m_Healthbar != null)
        {
            m_Healthbar.style.width = Length.Percent(100 * percentage);
        }
    }

    public void DisplayDialogue()
    {
        if (m_NonPlayerDialogue != null)
        {
            m_NonPlayerDialogue.style.display = DisplayStyle.Flex;
            m_TimerDisplay = displayTime;
        }
    }

    public void DisplayDialogue(string message, float time)
    {
        m_TimerDisplay = time;
        if (m_NonPlayerDialogue != null)
        {
            if (m_DialogueText != null)
            {
                m_DialogueText.text = message;
            }
            m_NonPlayerDialogue.style.display = DisplayStyle.Flex;
        }
    }

    public void DisplayWinScreen()
    {
        if (m_WinScreen != null)
        {
            m_WinScreen.style.opacity = 1.0f;
        }
    }

    public void DisplayLoseScreen()
    {
        if (m_LoseScreen != null)
        {
            m_LoseScreen.style.opacity = 1.0f;
        }
    }

    // --- Заглушки сумісності для демо-скриптів туторіалу ---
    public void SetCogCount(int amount) { }
    public void SetCount(int amount) { }
    public void SetCounter(int amount) { }
    public void SetCounter(int current, int max) { }
}