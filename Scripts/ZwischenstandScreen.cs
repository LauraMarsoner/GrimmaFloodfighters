using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ZwischenstandScreen : MonoBehaviour
{
    
    // UI Components
    public TMP_Text civiliansSavedText;
    public TMP_Text disastersPreventedText;
    public TMP_Text propertyDamageText;
    public TMP_Text housesFloodedText;
    public TMP_Text civiliansSavedTotalText;
    public TMP_Text disastersPreventedTotalText;
    public TMP_Text propertyDamageTotalText;
    public TMP_Text housesFloodedTotalText;
    public TMP_Text roundText;
    public GameObject loadingCircle;
    public GameObject continueButton;

    private bool shouldRotate = true;

    private void FixedUpdate()
    {
        if (shouldRotate) loadingCircle.transform.Rotate(0,0,-360/4*Time.deltaTime);
    }

    public void WaterHasIncreased(object e, float newWaterHeight)
    {
        //button aktivieren
        continueButton.GetComponent<Button>().enabled = true;
        continueButton.GetComponent<CanvasGroup>().alpha = 1f;
        
        //ladeteil deaktivieren
        shouldRotate = false;
        loadingCircle.SetActive(false);
    }
    
    public void WaterStartsAdvancing()
    {
        var _gameManager = GameManager.instance;
        //button deaktivieren/ausgrauen
        continueButton.GetComponent<Button>().enabled = false;
        continueButton.GetComponent<CanvasGroup>().alpha = 0.5f;
        
        //ladeteil aktivieren
        shouldRotate = true;
        loadingCircle.SetActive(true);
        
        //werte anzeigen
        civiliansSavedText.text = "Civilians saved: "+_gameManager.GetCiviliansSaved();
        civiliansSavedTotalText.text = "Civilians saved: "+_gameManager.GetciviliansSavedTotal()+"/"+(_gameManager.GetciviliansSavedTotal()+_gameManager.GetCiviliansTotal());
        
        disastersPreventedText.text = "Disasters prevented: "+_gameManager.GetDisasterPrevented();
        disastersPreventedTotalText.text = "Disasters prevented: "+_gameManager.GetDisastersPreventedTotal()+"/"+_gameManager.GetAmountDisasters();
        
        propertyDamageText.text = "Property damage: "+_gameManager.GetpropertyDamage();
        propertyDamageTotalText.text = "Property damage: "+_gameManager.GetpropertyDamageTotal();
        
        housesFloodedText.text = "Houses destroyed: "+_gameManager.GethousesDestroyed();
        housesFloodedTotalText.text = "Houses destroyed: "+_gameManager.GethousesDestroyedTotal();
        
        roundText.text = "Round: "+(NavManager.Instance.waterSteps+1)+"/"+NavManager.Instance.waterCap;
    }
    
    private void OnDestroy()
    {
        GameManager.instance.WaterIncreased -= WaterHasIncreased;
        GameManager.instance.AdvanceWaterStart -= WaterStartsAdvancing;
    }

    public void Continue()
    {
     
        GameManager.instance.gameIsPlaying = true;

        this.gameObject.SetActive(false);
    }
}
