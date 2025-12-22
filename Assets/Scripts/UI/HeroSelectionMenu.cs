using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

/*
 * backend for the hero selection menu UI
 */
public class HeroSelectionMenu : MonoBehaviour
{
    private struct ArmySlot
    {
        public string name;
        public GameObject hero;
    }

    [SerializeField] private GameObject[] heroPool;
    [SerializeField] private VisualTreeAsset attackButtonTemplate;

    private GameObject[] filteredHeroPool;
    private string[] filters = Enum.GetNames(typeof(HeroClasses));
    private int currentFilterIndex = -1;
    private LinkedList<Image> heroSelector = new();
    private int selectedHeroIndex = 0;
    private GameObject selectedHeroInstance;
    private ArmySlot[] army = new ArmySlot[5];

    private VisualElement root;
    private Button leftArrow;
    private Button rightArrow;
    private Button filterButton;
    private Image animationImage;
    private Label heroName;
    private Label heroClass;
    private Label heroHealth;
    private Label heroStamina;
    private VisualElement attacksContainer;
    private Button slot1Button;
    private Button slot2Button;
    private Button slot3Button;
    private Button slot4Button;
    private Button slot5Button;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // gets UI elements
        root = GetComponent<UIDocument>().rootVisualElement.Q<VisualElement>("HeroMenu");
        leftArrow = root.Q<Button>("LeftArrowButton");
        rightArrow = root.Q<Button>("RightArrowButton");
        filterButton = root.Q<Button>("FilterButton");
        animationImage = root.Q<Image>("Animation");
        heroName = root.Q<Label>("HeroName");
        heroClass = root.Q<Label>("Class");
        heroHealth = root.Q<Label>("Health");
        heroStamina = root.Q<Label>("Stamina");
        attacksContainer = root.Q<VisualElement>("AttacksContainer");
        slot1Button = root.Q<Button>("Slot1Button");
        slot2Button = root.Q<Button>("Slot2Button");
        slot3Button = root.Q<Button>("Slot3Button");
        slot4Button = root.Q<Button>("Slot4Button");
        slot5Button = root.Q<Button>("Slot5Button");

        // gets hero scrollbar
        heroSelector.AddFirst(root.Q<Image>("NextHero2"));
        heroSelector.AddFirst(root.Q<Image>("NextHero1"));
        heroSelector.AddFirst(root.Q<Image>("NextHero"));
        heroSelector.AddFirst(root.Q<Image>("SelectedHero"));
        heroSelector.AddFirst(root.Q<Image>("PreviousHero"));
        heroSelector.AddFirst(root.Q<Image>("PreviousHero1"));
        heroSelector.AddFirst(root.Q<Image>("PreviousHero2"));

        // sets filters and initial images
        filteredHeroPool = heroPool;
        int indexOffset = filteredHeroPool.Length - (heroSelector.Count / 2);
        for (LinkedListNode<Image> node = heroSelector.First; node != null; node = node.Next)
        {
            int index = (indexOffset + selectedHeroIndex) % filteredHeroPool.Length;
            Sprite sprite = filteredHeroPool[index].GetComponent<SpriteRenderer>().sprite;
            node.Value.Q<Image>("HeroImage").style.backgroundImage = new StyleBackground(sprite);
            indexOffset++;
        }
        updateStats();

        // assigns button callbacks
        leftArrow.clicked += leftArrowClicked;
        rightArrow.clicked += rightArrowClicked;
        filterButton.clicked += filterButtonClicked;
        slot1Button.clicked += () => slotButtonClicked(0);
        slot2Button.clicked += () => slotButtonClicked(1);
        slot3Button.clicked += () => slotButtonClicked(2);
        slot4Button.clicked += () => slotButtonClicked(3);
        slot5Button.clicked += () => slotButtonClicked(4);
    }

    // Update is called once per frame
    void Update()
    {
        if (selectedHeroInstance != null)
        {
            Sprite sprite = selectedHeroInstance.GetComponent<SpriteRenderer>().sprite;
            animationImage.style.backgroundImage = new StyleBackground(sprite);
        }
    }

    /*
     * cycles the hero section to the left
     */
    private void leftArrowClicked()
    {
        // transitions images by adjusting styles
        for (LinkedListNode<Image> node = heroSelector.Last; node.Previous != null; node = node.Previous)
        {
            LinkedListNode<Image> previous = node.Previous;
            node.Value.ClearClassList();
            foreach (string style in previous.Value.GetClasses())
                node.Value.AddToClassList(style);
        }
        heroSelector.First.Value.RemoveFromClassList("PreviousHero2");
        heroSelector.First.Value.AddToClassList("NextHero2");
        heroSelector.AddLast(heroSelector.First.Value);
        heroSelector.RemoveFirst();

        // update hero images
        selectedHeroIndex = selectedHeroIndex + 1 > filteredHeroPool.Length - 1 ? 0 : selectedHeroIndex + 1;
        int indexOffset = heroSelector.Count / 2;
        Sprite sprite = filteredHeroPool[(indexOffset + selectedHeroIndex) % filteredHeroPool.Length].GetComponent<SpriteRenderer>().sprite;
        heroSelector.Last.Value.Q<Image>("HeroImage").style.backgroundImage = new StyleBackground(sprite);
        updateStats();
    }

    /*
     * cycles the hero section to the right
     */
    private void rightArrowClicked()
    {
        // transitions images by adjusting styles
        for (LinkedListNode<Image> node = heroSelector.First; node.Next != null; node = node.Next)
        {
            LinkedListNode<Image> next = node.Next;
            node.Value.ClearClassList();
            foreach (string style in next.Value.GetClasses())
                node.Value.AddToClassList(style);
        }
        heroSelector.Last.Value.RemoveFromClassList("NextHero2");
        heroSelector.Last.Value.AddToClassList("PreviousHero2");
        heroSelector.AddFirst(heroSelector.Last.Value);
        heroSelector.RemoveLast();

        // update hero images
        selectedHeroIndex = selectedHeroIndex - 1 < 0 ? filteredHeroPool.Length - 1 : selectedHeroIndex - 1;
        int indexOffset = filteredHeroPool.Length - (heroSelector.Count / 2);
        Sprite sprite = filteredHeroPool[(indexOffset + selectedHeroIndex) % filteredHeroPool.Length].GetComponent<SpriteRenderer>().sprite;
        heroSelector.First.Value.Q<Image>("HeroImage").style.backgroundImage = new StyleBackground(sprite);
        updateStats();
    }

    /*
     * handles when the user clicks the filter button. applies filtering to the hero pool
     */
    private void filterButtonClicked()
    {
        // applies filtering 
        currentFilterIndex += 1;
        if (currentFilterIndex < filters.Length)
        {
            filterButton.text = filters[currentFilterIndex];
            filteredHeroPool = heroPool.Where(hero => hero.GetComponent<AttackBehavior>().HeroClass == (HeroClasses)Enum.Parse(typeof(HeroClasses), filters[currentFilterIndex])).ToArray();
        }
        else
        {
            currentFilterIndex = -1;
            filterButton.text = "All";
            filteredHeroPool = heroPool;
        }

        // adjusts hero images
        selectedHeroIndex = 0;
        int indexOffset = filteredHeroPool.Length - (heroSelector.Count / 2);
        for (LinkedListNode<Image> node = heroSelector.First; node != null; node = node.Next)
        {
            int index = (indexOffset + selectedHeroIndex) % filteredHeroPool.Length;
            Sprite sprite = filteredHeroPool[index].GetComponent<SpriteRenderer>().sprite;
            node.Value.Q<Image>("HeroImage").style.backgroundImage = new StyleBackground(sprite);
            indexOffset++;
        }
        updateStats();
    }

    /*
     * called every time the selected hero changes and updates the UI to reflect the new hero's stats
     */
    private void updateStats()
    {
        // spawns instance of hero for UI animation
        Destroy(selectedHeroInstance);
        selectedHeroInstance = null;
        GameObject instance = Instantiate(filteredHeroPool[selectedHeroIndex]);
        SpriteRenderer spriteRenderer = instance.GetComponent<SpriteRenderer>();
        BoxCollider2D hitbox = instance.GetComponent<BoxCollider2D>();

        // ensures instance is essentially non existent in the scene
        instance.GetComponent<Rigidbody2D>().gravityScale = 0;
        spriteRenderer.enabled = false;
        hitbox.isTrigger = true;

        // sets up the animation image
        animationImage.style.backgroundImage = null;
        animationImage.schedule.Execute(() =>
        {
            // scales image
            selectedHeroInstance = instance;
            float widthFactor = (spriteRenderer.sprite.rect.width * instance.gameObject.transform.localScale.x) / animationImage.resolvedStyle.width;
            float heightFactor = (spriteRenderer.sprite.rect.height * instance.gameObject.transform.localScale.y) / animationImage.resolvedStyle.height;
            Length width = new Length(widthFactor * 100, LengthUnit.Percent);
            Length height = new Length(heightFactor * 100, LengthUnit.Percent);
            animationImage.style.backgroundSize = new BackgroundSize(width, height);

            // positions image
            float hitboxRatioX = (hitbox.offset.x - spriteRenderer.sprite.bounds.min.x) / spriteRenderer.sprite.bounds.size.x;
            float hitboxRatioY = (hitbox.offset.y - (hitbox.size.y / 2) - spriteRenderer.sprite.bounds.min.y) / spriteRenderer.sprite.bounds.size.y;
            float offsetX = (hitboxRatioX * animationImage.resolvedStyle.width * widthFactor) - (animationImage.resolvedStyle.width / 2);
            float offsetY = hitboxRatioY * animationImage.resolvedStyle.height * heightFactor;
            animationImage.style.backgroundPositionX = new StyleBackgroundPosition(new BackgroundPosition(BackgroundPositionKeyword.Left, new Length(-offsetX - 50, LengthUnit.Pixel)));
            animationImage.style.backgroundPositionY = new StyleBackgroundPosition(new BackgroundPosition(BackgroundPositionKeyword.Bottom, new Length(-offsetY + 3, LengthUnit.Pixel)));

        }).StartingIn(100); // small delay to give animator time to start

        // updates hero stats UI
        AttackBehavior hero = instance.GetComponent<AttackBehavior>();
        heroName.text = filteredHeroPool[selectedHeroIndex].name;
        heroClass.text = hero.HeroClass.ToString();
        heroHealth.text = hero.Health.ToString();
        heroStamina.text = hero.Stamina.ToString();

        // adds attack buttons
        attacksContainer.Clear();
        for (int i = 0; i < hero.Attacks.Length; i++)
        {
            Button attackButton = attackButtonTemplate.CloneTree().Q<Button>("Button");
            attackButton.text = "Attack " + (i + 1);
            attackButton.Q<Label>("Damage").text = hero.Attacks[i].Damage.ToString();
            attacksContainer.Add(attackButton);
            int index = i;
            attackButton.clicked += () => hero.startAttack(index);
            attackButton.clicked += () => attackButton.schedule.Execute(() =>
            {
                hero.endAttack(index);
            }).StartingIn(1000);
        }
    }

    /*
     * handles when the user clicks one of the slot buttons to assign the selected hero to that slot
     */
    private void slotButtonClicked(int slotIndex)
    {
        // removed hero from previous slot if it was already assigned
        int oldSlotIndex = Array.FindIndex(army, s => s.name == filteredHeroPool[selectedHeroIndex].name);
        if (oldSlotIndex != -1)
        {
            Destroy(army[oldSlotIndex].hero);
            army[oldSlotIndex].name = null;
            army[oldSlotIndex].hero = null;
        }

        // calculates position to spawn hero in army
        float xRatio = root.resolvedStyle.width / root.parent.resolvedStyle.width;
        xRatio += ((1 - xRatio) / (army.Length + 1)) * (slotIndex + 1);
        Vector2 bottomLeft = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width * xRatio, 0, Camera.main.nearClipPlane));
        Vector2 topRight = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.nearClipPlane));

        // assigns hero to selected slot
        Destroy(army[slotIndex].hero);
        army[slotIndex].name = filteredHeroPool[selectedHeroIndex].name;
        army[slotIndex].hero = Instantiate(filteredHeroPool[selectedHeroIndex], new Vector2(bottomLeft.x, (topRight.y + bottomLeft.y) / 2), Quaternion.identity);
    }
}
