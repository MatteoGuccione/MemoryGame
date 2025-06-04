using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    GameObject card;

    [SerializeField]
    Vector3[] cards;

    GameObject[] cardsObjects;

    [SerializeField]
    Texture2D[] images;

    [SerializeField]
    float startX;

    [SerializeField]
    float startY;

    [SerializeField]
    float planeZ;

    [SerializeField]
    float deltaX = 1.1f;

    [SerializeField]
    float deltaY = 1.1f;

    [SerializeField]
    int columns = 5;

    [SerializeField]
    int rows = 6;

    [SerializeField]
    GameObject winUI;

    [SerializeField]
    GameObject LostUI;

    [SerializeField]
    [Range (0f, 300f)]
    float totalTimer = 300;

    [SerializeField]
    TMP_Text totalTimerText;

    [SerializeField]
    [Range (0f,30f)]
    float roundTimer = 30;

    [SerializeField]
    TMP_Text roundTimerText;

    [SerializeField]
    [Range(0, 30)]
    int lives;

    [SerializeField]
    TMP_Text livesText;

    [SerializeField]
    AudioSource matchedPairAudioSource;

    int pairs;
    float rTimer;

    InteractiveCard selectedCard1;
    InteractiveCard selectedCard2;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rTimer = roundTimer;
        livesText.SetText($"Remaining lives: {lives}");
        if (rows * columns != images.Length * 2)
        {
            Debug.LogWarning("Number not valid");
            return;
        }

        pairs = rows * columns / 2;

        System.Random random = new System.Random();
        images = images.OrderBy(x  => random.Next()).ToArray();

        cards = new Vector3[rows * columns];

        float dx = startX;
        float dy = startY;

        int counter = 0;

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                cards[counter++] = new Vector3(dx, dy, planeZ);
                dx += deltaX;
            }

            dx = startX;
            dy += deltaY;
        }
        cards = cards.OrderBy(x => random.Next()).ToArray();

        counter = 0;

        int row = 0;

        foreach (Vector3 pos in cards)
        {
            GameObject go = Instantiate(card);
            
            go.SetActive(true);
            go.transform.position = pos;

            

            go.GetComponent<MeshRenderer>().material.SetTexture("_MainTexture",images[row]);

            go.GetComponent<InteractiveCard>().onClicked += SelectedCard;

            go.GetComponent<InteractiveCard>().imageName += images[row].name;

            counter++;

            if (counter % 2 == 0)
            {
                row++;
            }
        }

    }

    private void SelectedCard(InteractiveCard card, bool selected)
    {
        if (selectedCard1 == null && selected)
        {
            selectedCard1 = card;
        }
        else if (selectedCard1 != null && !selected)
        {
            selectedCard1.ResetMe();
            selectedCard1 = null;
        }
        else if (selectedCard2 != null && !selected)
        {
            selectedCard1.ResetMe();
            selectedCard2 = null;
        }
        else if (selectedCard2 == null && card != selectedCard1 && selected)
        {
            selectedCard2 = card;

            if (selectedCard1.Compare(selectedCard2))
            {
                //matchedPairAudioSource.Play();

                selectedCard1.HideAndDestroy();
                selectedCard2.HideAndDestroy();

                selectedCard1 = null;
                selectedCard2 = null;

                pairs--;

                totalTimer += 30;

                if (pairs == 0)
                {
                    winUI.SetActive(true);
                    winUI.GetComponent<AudioSource>().Play();
                }
                lives += 20;
                livesText.SetText($"Remaining lives: {lives}");
            }
            else
            {
                selectedCard1.ResetMe();
                selectedCard2.ResetMe();

                selectedCard1 = null;
                selectedCard2 = null;

                lives--;
                livesText.SetText($"Remaining lives: {lives}");

                if (lives == 0)
                {
                    lives = 30;
                    livesText.SetText($"Remaining lives: {lives}");
                    totalTimer -= 60;
                }
            }
            rTimer = roundTimer;
        }
    }

    public void Update()
    {
        totalTimer -= Time.deltaTime;
        rTimer -= Time.deltaTime;
        totalTimerText.SetText($"Remaining total time: \n{(int)totalTimer}");
        roundTimerText.SetText($"Remaining round time: \n{(int)rTimer}");
        if (rTimer < 0)
        {
            if (selectedCard1 != null)
            {
                selectedCard1.ResetMe();
                selectedCard1 = null;
            }
            if (selectedCard2 != null)
            {
                selectedCard2.ResetMe();
                selectedCard2 = null;
            }
            rTimer = roundTimer;
        }
        if (totalTimer < 0)
        {
            LostUI.SetActive(true);
        }
    }

    public void PlayAgain()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
