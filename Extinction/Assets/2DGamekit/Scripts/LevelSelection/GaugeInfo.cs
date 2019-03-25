using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GaugeInfo : Displayable
{
    public RectTransform greenUI;
    public RectTransform orangeUI;
    public RectTransform redUI;

    public TextMeshProUGUI valueDisplayer;
    public TextMeshProUGUI variationDisplayer;

    public float maxGreen = 100;
    public float maxOrange = 80;
    public float maxRed = 50;

    public float value = 100;

    private float variationValue;
    private float variationPerFrame;
    private float variationLeft = 0;
    private bool settingValue = false;
    private float variationTimer = float.MinValue;
    private float variationDisplayingDuration = 1;
    private bool addingVariation = false;
    private float addingDuration = 3;
    private bool disablingVariationDisplayer = false;
    private bool negativeVariation;

    public float Value
    {
        get
        {
            return value;
        }

        set
        {
            this.value = value > 100 ? 100 : value < 0 ? 0 : value;

            valueDisplayer.text = this.value.ToString("n0");

            if (this.value < maxGreen)
                greenUI.offsetMax = new Vector2(greenUI.offsetMax.x, this.value - 100);
            else
                greenUI.offsetMax = new Vector2(greenUI.offsetMax.x, maxGreen - 100);

            if(this.value < maxOrange)
                orangeUI.offsetMax = new Vector2(orangeUI.offsetMax.x, this.value - 100);
            else
                orangeUI.offsetMax = new Vector2(orangeUI.offsetMax.x, maxOrange - 100);

            if(this.value < maxRed)
                redUI.offsetMax = new Vector2(redUI.offsetMax.x, this.value - 100);
            else
                redUI.offsetMax = new Vector2(redUI.offsetMax.x, maxRed - 100);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        if(!greenUI || !orangeUI || !redUI)
        {
            Debug.LogWarning("A color of the gauge is missing. Gameobject disabled");
            gameObject.SetActive(false);
        }

        maxGreen = maxGreen > 100 ? 100 : maxGreen < 0 ? 0 : maxGreen;
        maxOrange = maxOrange > 100 ? 100 : maxOrange < 0 ? 0 : maxOrange;
        maxRed = maxRed > 100 ? 100 : maxRed < 0 ? 0 : maxRed;

        value = Value;
    }

    private void Update()
    {
        if (addingVariation)
        {
            variationPerFrame = Time.deltaTime * variationValue / addingDuration;
            variationLeft -= variationPerFrame;
            float oldValue = Value;
            Value += variationPerFrame;
            variationDisplayer.text = variationLeft.ToString("+#;-#;0");
            if (variationLeft == 0 || (negativeVariation && variationLeft > 0) || (!negativeVariation && variationLeft < 0))
            {
                //adding finished
                if(Value != oldValue)
                    Value += variationLeft;
                variationLeft = 0;
                variationDisplayer.text = variationLeft.ToString("+#;-#;0");
                addingVariation = false;
                variationTimer = Time.time;
                disablingVariationDisplayer = true;
            }
        }
        else if(Time.time - variationTimer > variationDisplayingDuration && settingValue)
        {
            if (disablingVariationDisplayer)
            {
                settingValue = false;
                disablingVariationDisplayer = false;
                variationDisplayer.text = "";
            }
            else
                addingVariation = true;
        }
    }

    public void IncreaseValue(float byValue)
    {
        Value = value + variationLeft;
        variationValue = byValue;
        variationLeft = variationValue;
        
        if (LearningAnalyticsGenerator.instance && LearningAnalyticsGenerator.instance.canGenerateLA)
        {
            float oldValue = Value;
            Value = oldValue + variationLeft;
            float valueAfterVariation = Value;
            Value = oldValue;
            GBL_Interface.SendStatement("modified", "gauge", gameObject.name, new Dictionary<string, List<string>>()
            {
                { "value", new List<string>() { valueAfterVariation.ToString() } },
                { "from", new List<string>() { oldValue.ToString() } }
            });
        }

        negativeVariation = variationValue < 0;
        if (negativeVariation)
            variationDisplayer.color = Color.red;
        else
            variationDisplayer.color = Color.green;
        variationDisplayer.text = variationLeft.ToString("+#;-#;0");
        settingValue = true;
        variationTimer = Time.time;
    }

    public void DecreaseValue(float byValue)
    {
        Value = value - variationLeft;
        variationValue = byValue;
        variationLeft = variationValue;

        if (LearningAnalyticsGenerator.instance && LearningAnalyticsGenerator.instance.canGenerateLA)
        {
            float oldValue = Value;
            Value = oldValue + variationLeft;
            float valueAfterVariation = Value;
            Value = oldValue;
            GBL_Interface.SendStatement("modified", "gauge", gameObject.name, new Dictionary<string, List<string>>()
            {
                { "value", new List<string>() { valueAfterVariation.ToString() } },
                { "from", new List<string>() { oldValue.ToString() } }
            });
        }

        negativeVariation = variationValue < 0;
        if (negativeVariation)
            variationDisplayer.color = Color.red;
        else
            variationDisplayer.color = Color.green;
        variationDisplayer.text = variationLeft.ToString("+#;-#;0");
        settingValue = true;
        variationTimer = Time.time;
    }

    public void SetValue(float newValue)
    {
        Value = value + variationLeft;
        variationValue = newValue - value;
        variationLeft = variationValue;

        if (LearningAnalyticsGenerator.instance && LearningAnalyticsGenerator.instance.canGenerateLA)
        {
            float oldValue = Value;
            Value = oldValue + variationLeft;
            float valueAfterVariation = Value;
            Value = oldValue;
            GBL_Interface.SendStatement("modified", "gauge", gameObject.name, new Dictionary<string, List<string>>()
            {
                { "value", new List<string>() { valueAfterVariation.ToString() } },
                { "from", new List<string>() { oldValue.ToString() } }
            });
        }

        negativeVariation = variationValue < 0;
        if (negativeVariation)
            variationDisplayer.color = Color.red;
        else
            variationDisplayer.color = Color.green;
        variationDisplayer.text = variationLeft.ToString("+#;-#;0");
        settingValue = true;
        variationTimer = Time.time;
    }
}
