using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
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
    private Image[] emptySlotImages = new Image[5];

    private VisualElement root;
    private Button leftArrow;
    private Button rightArrow;
    private Button filterButton;
    private Image heroAnimationImage;
    private Image projectileAnimationImage;
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
        root = GetComponent<UIDocument>().rootVisualElement;
        leftArrow = root.Q<Button>("LeftArrowButton");
        rightArrow = root.Q<Button>("RightArrowButton");
        filterButton = root.Q<Button>("FilterButton");
        heroAnimationImage = root.Q<Image>("HeroAnimation");
        projectileAnimationImage = root.Q<Image>("ProjectileAnimation");
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

        // gets empty slot images
        emptySlotImages[0] = root.Q<Image>("Slot1");
        emptySlotImages[1] = root.Q<Image>("Slot2");
        emptySlotImages[2] = root.Q<Image>("Slot3");
        emptySlotImages[3] = root.Q<Image>("Slot4");
        emptySlotImages[4] = root.Q<Image>("Slot5");

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

        // starts empty slot animations
        root.schedule.Execute(() =>
        {
            foreach (Image slot in emptySlotImages)
                slot.style.scale = slot.style.scale.value == Vector2.one ? Vector2.one * 1.1f : Vector2.one;
        }).Every(750);
    }

    // Update is called once per frame
    void Update()
    {
        // draws hero animation to UI
        if (selectedHeroInstance != null)
        {
            Sprite sprite = selectedHeroInstance.GetComponent<SpriteRenderer>().sprite;
            heroAnimationImage.style.backgroundImage = new StyleBackground(sprite);
            GameObject[] projectiles = selectedHeroInstance.GetComponent<AttackBehavior>().Projectiles;

            // draws projectile animation to UI
            if (projectiles.Length > 0)
            {
                Sprite projectileSprite = projectiles[0].GetComponent<SpriteRenderer>().sprite;
                formatImage(selectedHeroInstance.GetComponent<BoxCollider2D>().bounds.min, projectiles[0], projectileAnimationImage);
                projectileAnimationImage.style.backgroundImage = new StyleBackground(projectileSprite);
            }
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
        Vector2 slotCenter = emptySlotImages[slotIndex].worldBound.center * root.panel.scaledPixelsPerPoint;
        Vector2 worldPos = Camera.main.ScreenToWorldPoint(new Vector3(slotCenter.x, Screen.height - slotCenter.y));

        // assigns hero to selected slot
        Destroy(army[slotIndex].hero);
        army[slotIndex].name = filteredHeroPool[selectedHeroIndex].name;
        army[slotIndex].hero = Instantiate(filteredHeroPool[selectedHeroIndex], worldPos, Quaternion.identity);
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
        BoxCollider2D hitbox = instance.GetComponent<BoxCollider2D>();

        // ensures instance is essentially non existent in the scene
        instance.GetComponent<Rigidbody2D>().gravityScale = 0;
        instance.GetComponent<SpriteRenderer>().enabled = false;
        instance.GetComponent<ParticleSystem>().Stop();
        hitbox.isTrigger = true;

        // sets up the animation image
        heroAnimationImage.style.backgroundImage = null;
        heroAnimationImage.schedule.Execute(() =>
        {
            selectedHeroInstance = instance;
            formatImage(hitbox.bounds.min, instance, heroAnimationImage);

            // updates hero stats UI
            AttackBehavior hero = instance.GetComponent<AttackBehavior>();
            heroName.text = filteredHeroPool[selectedHeroIndex].name + " - " + hero.HeroType;
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

                // despawns existing projectiles and starts attack on click
                attackButton.clicked += () =>
                {
                    foreach (GameObject projectile in hero.Projectiles)
                        projectile.GetComponent<ProjectileBehavior>().destroy();
                    hero.startAttack(index);
                };

                // ends persistent attacks after 1 second
                attackButton.clicked += () => attackButton.schedule.Execute(() =>
                {
                    if (!hero.IsDestroyed())
                        hero.endAttack(index);
                }).StartingIn(1000);
            }

            // wait to update stats until selector animation is done
        }).StartingIn((long)(heroSelector.First.Value.resolvedStyle.transitionDuration.First().value * 1000));
    }

    /**
     * helper function to scale and position a UI image with respect to worldspace.
     * 
     * @param origin the origin point in worldspace, will be the bottom left corner of the image
     * @param instance the reference game object for scale/positioning
     * @param image the UI image to position
     */
    private void formatImage(Vector2 origin, GameObject instance, Image image)
    {
        // scales image
        Vector2 size = instance.GetComponent<SpriteRenderer>().sprite.rect.size * instance.transform.localScale;
        image.style.backgroundSize = new BackgroundSize(new Length(size.x, LengthUnit.Pixel), new Length(size.y, LengthUnit.Pixel));

        // positions image
        Sprite sprite = instance.GetComponent<SpriteRenderer>().sprite;
        Vector2 offset = -(sprite.rect.size * instance.transform.localScale) / 2; // centers image to bottom left
        offset -= (origin - (Vector2)instance.transform.position) * sprite.pixelsPerUnit; // finds offset from orgin and converts to pixels
        image.style.backgroundPositionX = new StyleBackgroundPosition(new BackgroundPosition(BackgroundPositionKeyword.Left, new Length(offset.x, LengthUnit.Pixel)));
        image.style.backgroundPositionY = new StyleBackgroundPosition(new BackgroundPosition(BackgroundPositionKeyword.Bottom, new Length(offset.y, LengthUnit.Pixel)));
    }
}
