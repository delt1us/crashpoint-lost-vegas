using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class Base_debugger : MonoBehaviour
{
    [SerializeField, Tooltip("The top hierarchy game object of the vehicle you want to debug.")] 
    private GameObject toDebug;
    private MovementController moveController;
    private WeaponController weaponController;

    [Header("Movement/Handling")]
    [SerializeField] private TextMeshProUGUI txt_highSpeed;
    
    [SerializeField] private TextMeshProUGUI txt_vehicleMass;
    [SerializeField] private TextMeshProUGUI txt_position;

    [SerializeField] private TextMeshProUGUI txt_currentFwdFriction;
    [SerializeField] private TextMeshProUGUI txt_currentSideFriction;


    [Header("Weapons")]
    [SerializeField] private TextMeshProUGUI txt_name;
    [SerializeField] private TextMeshProUGUI txt_maxAmmo;
    [SerializeField] private TextMeshProUGUI txt_mag;
    [SerializeField] private TextMeshProUGUI txt_heatRate;
    [SerializeField] private TextMeshProUGUI txt_cooldown;
    [SerializeField] private TextMeshProUGUI txt_shortDelay;
    [SerializeField] private TextMeshProUGUI txt_longDelay;


    [Header("Boost")]
    [SerializeField] private TextMeshProUGUI txt_currentBoost;
    [SerializeField] private TextMeshProUGUI txt_regenRate;
    [SerializeField] private TextMeshProUGUI txt_regenDelay;
    [SerializeField] private TextMeshProUGUI txt_power;

    [Header("Vechicle Selection")]
    [SerializeField] private GameObject carsMenu;
    [SerializeField] private List<GameObject> carTypes;

    // Movement prefixes
    private const string prefix_highSpeed = "High Speed:  ";

    private const string prefix_mass = "Mass:  ";
    
    private const string prefix_position = "World Position:\n";
    
    private const string prefix_currentFwdFriction = "Current Forward Friction:  ";
    private const string prefix_currentSideFriction = "Current Sideward Friction:  ";


    // Weapon prefixes
    private const string prefix_name = "NAME:  ";
    private const string prefix_maxAmmo = "Max Ammo:  ";
    private const string prefix_mag = "Ammo in Mag:  ";
    private const string prefix_heatRate = "Overheat Rate:  ";
    private const string prefix_cooldown = "Time of Cooldown:  ";
    private const string prefix_shortDelay = "Short Cooldown Delay:  ";
    private const string prefix_longDelay = "Long Cooldown Delay:  ";


    // Boost prefixes
    private const string prefix_currentBoost = "Current Boost:  ";
    private const string prefix_regenRate = "Boost Regen Rate:  ";
    private const string prefix_regenDelay = "Boost Regen Delay:  ";
    private const string prefix_power = "Boost Power:  ";


    // Movement / Handling
    private float highSpeed;
    private float currentSpeed;

    private float vehicleMass;
    private Vector3 position;

    private float currentFwdFriction;
    private float currentSideFriction;


    // Weapons
    private string wepName;
    private int maxAmmo;
    private float mag;
    private float heatRate;
    private float cooldown;
    private float shortDelay;
    private float longDelay;


    // Boosting
    private float currentBoost;
    private float regenRate;
    private float regenDelay;
    private float boostPower;

    [SerializeField] private InputManager inputManager;

    [SerializeField] private Slider lookSlider;

    [SerializeField] private Slider aimSlider_x;
    [SerializeField] private Slider aimSlider_y;

    [SerializeField] private TextMeshProUGUI lookSensTxt;
    [SerializeField] private TextMeshProUGUI horiSensTxt;
    [SerializeField] private TextMeshProUGUI vertSensTxt;

    private Vector2 aimSensitivity = new(1, 1);

    private void Start()
    {
        moveController = toDebug.GetComponentInChildren<MovementController>();

        // Destroy all of the previously created menus
        GameObject[] debugs = GameObject.FindGameObjectsWithTag("DebugMenu");
        foreach(GameObject menu in debugs) if(menu.GetComponentInParent<Transform>().gameObject != GetComponentInParent<Transform>().gameObject) Destroy(menu.GetComponentInParent<Transform>().gameObject);

        LoadCars();
    }

    private void LateUpdate()
    {
        DebugHandling();
        DebugBoost();
        DebugWeapons();
    }

    private void DebugHandling()
    {
        highSpeed = highSpeed < currentSpeed ? moveController.GetSpeed() : highSpeed;
        currentSpeed = moveController.GetSpeed();

        vehicleMass = moveController.GetMass();

        position = moveController.GetPosition();

        currentFwdFriction = moveController.GetCurrentFrictions()[0];
        currentSideFriction = moveController.GetCurrentFrictions()[1];

        txt_highSpeed.text = prefix_highSpeed + highSpeed.ToString("00.00");

        txt_vehicleMass.text = prefix_mass + vehicleMass.ToString();

        txt_position.text = prefix_position + position;

        txt_currentFwdFriction.text = prefix_currentFwdFriction + currentFwdFriction.ToString();
        txt_currentSideFriction.text = prefix_currentSideFriction + currentSideFriction.ToString();
    }

    private void DebugWeapons()
    {
        if(toDebug.GetComponentInChildren<ActionController>()) weaponController = toDebug.GetComponentInChildren<ActionController>().GetActiveWeapon().GetComponentInChildren<WeaponController>();

        if (!weaponController) return;

        wepName = weaponController.gameObject.name;
        maxAmmo = weaponController.GetMaxAmmo();
        mag = weaponController.GetCurrentMag();
        heatRate = weaponController.GetOverheatRate();
        cooldown = weaponController.GetCooldown();
        shortDelay = weaponController.GetShortDelay();
        longDelay = weaponController.GetLongDelay();

        txt_name.text = prefix_name + wepName;
        txt_maxAmmo.text = prefix_maxAmmo + maxAmmo.ToString();
        txt_mag.text = prefix_mag + mag.ToString("00");
        txt_heatRate.text = prefix_heatRate + heatRate.ToString();
        txt_cooldown.text = prefix_cooldown + cooldown.ToString("00.00");
        txt_shortDelay.text = prefix_shortDelay + shortDelay.ToString();
        txt_longDelay.text = prefix_longDelay + longDelay.ToString();
    }

    private void DebugBoost()
    {
        currentBoost = moveController.GetBoostManager().GetCurrentBoost();
        regenRate = moveController.GetBoostManager().GetRegenRate();
        regenDelay = moveController.GetBoostManager().GetRegenDelay()/1000;
        boostPower = moveController.GetBoostManager().GetPower();

        txt_currentBoost.text = prefix_currentBoost + currentBoost.ToString("00.00");
        txt_regenRate.text = prefix_regenRate + regenRate.ToString();
        txt_regenDelay.text = prefix_regenDelay + regenDelay.ToString() + "s";
        txt_power.text = prefix_power + boostPower.ToString();
    }

    public void ToggleCarsMenu()
    {
        if (carsMenu.activeSelf) carsMenu.SetActive(false);
        else carsMenu.SetActive(true);
        //carsMenu.SetActive(!carsMenu.activeSelf);

        // Hide/Show mouse depending on whether the window is open
        UnityEngine.Cursor.lockState = carsMenu.activeSelf? CursorLockMode.None: CursorLockMode.Locked;
    }

    private void LoadCars()
    {

        //GameObject drifter = Instantiate(carTypes[1], toDebug.transform.position, toDebug.transform.rotation);

        //GameObject speeder = Instantiate(carTypes[2], toDebug.transform.position, toDebug.transform.rotation);
        
        //GameObject hover = Instantiate(carTypes[3], toDebug.transform.position, toDebug.transform.rotation);

        //GameObject pointTurn = Instantiate(carTypes[4], toDebug.transform.position, toDebug.transform.rotation);

        //GameObject truck = Instantiate(carTypes[5], toDebug.transform.position, toDebug.transform.rotation);

        //GameObject hippy = Instantiate(carTypes[6], toDebug.transform.position, toDebug.transform.rotation);

        //GameObject delivery = Instantiate(carTypes[7], toDebug.transform.position, toDebug.transform.rotation);
        //delivery.SetActive(false);

        //cars = new[] { dasher, drifter, speeder, hover, pointTurn, truck, hippy, delivery };
        //foreach(GameObject car in cars) car.SetActive(false);
    }

    public void LoadDasher()
    {
        if (toDebug != carTypes[0])
        {
            DestroyCar();
            Instantiate(carTypes[0], toDebug.transform.position, toDebug.transform.rotation);
            ToggleCarsMenu();
            return;
        }

        toDebug.transform.position = new(toDebug.transform.position.x, toDebug.transform.position.y + 20, toDebug.transform.position.z - 10);
        ToggleCarsMenu();
    }

    public void LoadDrifter()
    {
        if (toDebug != carTypes[1])
        {
            DestroyCar();
            Instantiate(carTypes[1], toDebug.transform.position, toDebug.transform.rotation);
            ToggleCarsMenu();
            return;
        }

        toDebug.transform.position = new(toDebug.transform.position.x, toDebug.transform.position.y + 20, toDebug.transform.position.z - 10);
        ToggleCarsMenu();
    }

    public void LoadSpeeder()
    {
        if (toDebug != carTypes[2])
        {
            DestroyCar();
            Instantiate(carTypes[2], toDebug.transform.position, toDebug.transform.rotation);
            ToggleCarsMenu();
            return;
        }

        toDebug.transform.position = new(toDebug.transform.position.x, toDebug.transform.position.y + 20, toDebug.transform.position.z - 10);
        ToggleCarsMenu();
    }

    public void LoadHover()
    {
        if (toDebug != carTypes[3])
        {
            DestroyCar();
            Instantiate(carTypes[3], toDebug.transform.position, toDebug.transform.rotation);
            ToggleCarsMenu();
            return;
        }

        toDebug.transform.position = new(toDebug.transform.position.x, toDebug.transform.position.y + 20, toDebug.transform.position.z - 10);
        ToggleCarsMenu();
    }

    public void LoadPointTurn()
    {
        if (toDebug != carTypes[4])
        {
            DestroyCar();
            Instantiate(carTypes[4], toDebug.transform.position, toDebug.transform.rotation);
            ToggleCarsMenu();
            return;
        }

        toDebug.transform.position = new(toDebug.transform.position.x, toDebug.transform.position.y + 20, toDebug.transform.position.z - 10);
        ToggleCarsMenu();
    }


    public void LoadTruck()
    {
        if (toDebug != carTypes[5])
        {
            DestroyCar();
            Instantiate(carTypes[5], toDebug.transform.position, toDebug.transform.rotation);
            ToggleCarsMenu();
            return;
        }

        toDebug.transform.position = new(toDebug.transform.position.x, toDebug.transform.position.y + 20, toDebug.transform.position.z - 10);
        ToggleCarsMenu();
    }
    
    public void LoadHippy()
    {
        if (toDebug != carTypes[6])
        {
            DestroyCar();
            GameObject van = Instantiate(carTypes[6], toDebug.transform.position, toDebug.transform.rotation);
            van.GetComponentInChildren<Rigidbody>().velocity = Vector3.up * 3;
            ToggleCarsMenu();
            return;
        }

        toDebug.transform.position = new(toDebug.transform.position.x, toDebug.transform.position.y + 20, toDebug.transform.position.z - 10);
        ToggleCarsMenu();
    }

    public void LoadDelivery()
    {
        if (toDebug != carTypes[7])
        {
            DestroyCar();
            Instantiate(carTypes[7], toDebug.transform.position, toDebug.transform.rotation);
            ToggleCarsMenu();
            return;
        }

        toDebug.transform.position = new(toDebug.transform.position.x, toDebug.transform.position.y + 20, toDebug.transform.position.z - 10);
        ToggleCarsMenu();
    }

    private void DestroyCar()
    {
        Destroy(toDebug);

        moveController.ResetInputs();
    }

    public void ResetScene()
    {
        SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene().buildIndex);
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);

        Unpause();
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void SetLookSens(Single sensitivity)
    {
        if (!inputManager) return;

        inputManager.SetLookSens(sensitivity);
        lookSensTxt.text = sensitivity.ToString("00.00");
    }

    public void SetAimSens_x(Single sensitivity)
    {
        if (!inputManager) return;

        aimSensitivity = new(sensitivity, aimSensitivity.y);
        inputManager.SetAimSens(aimSensitivity.x, aimSensitivity.y);
        horiSensTxt.text = sensitivity.ToString("00.00");
    }

    public void SetAimSens_y(Single sensitivity)
    {
        if (!inputManager) return;

        aimSensitivity = new(aimSensitivity.x, sensitivity);
        inputManager.SetAimSens(aimSensitivity.x, aimSensitivity.y);
        vertSensTxt.text = sensitivity.ToString("00.00");
    }

    public void Unpause()
    {
        moveController.GetComponentInParent<PlayerController>().OnPause();
    }

    public void ToggleAim()
    {
        if(moveController) moveController.GetComponent<InputManager>().OnToggleAimToggle();
    }
}
