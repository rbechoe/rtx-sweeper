using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class ColorPicker : MonoBehaviour
{
    /// <summary>
    /// Event that gets called by the ColorPicker
    /// </summary>
    /// <param name="c">received Color</param>
    public delegate void ColorEvent(Color c);

    private static ColorPicker instance;
    /// <returns>
    /// True when the ColorPicker is closed
    /// </returns>
    public static bool done = true;

    //onColorChanged event
    private static ColorEvent onCC;
    //onColorSelected event
    private static ColorEvent onCS;

    //Color before editing
    private static Color32 originalColor;
    //current Color
    private static Color32 modifiedColor;
    private static HSV modifiedHsv;

    //useAlpha bool
    private static bool useA;

    private bool interact;

    // these can only work with the prefab and its children
    public RectTransform positionIndicator;
    public Slider mainComponent;
    public Slider rComponent;
    public Slider gComponent;
    public Slider bComponent;
    public TMP_InputField rComp;
    public TMP_InputField gComp;
    public TMP_InputField bComp;
    public RawImage colorComponent;

    private void Awake()
    {
        instance = this;
        modifiedHsv = new HSV();
        //gameObject.SetActive(false);
    }


    /// <summary>
    /// Creates a new Colorpicker
    /// </summary>
    /// <param name="original">Color before editing</param>
    /// <param name="message">Display message</param>
    /// <param name="onColorChanged">Event that gets called when the color gets modified</param>
    /// <param name="onColorSelected">Event that gets called when one of the buttons done or cancel get pressed</param>
    /// <param name="useAlpha">When set to false the colors used don't have an alpha channel</param>
    /// <returns>
    /// False if the instance is already running
    /// </returns>
    public static bool Create(Color original, string message, ColorEvent onColorChanged, ColorEvent onColorSelected, bool useAlpha = false)
    {   
        if(instance is null)
        {
            Debug.LogError("No Colorpicker prefab active on 'Start' in scene");
            return false;
        }
        if(done)
        {
            done = false;
            originalColor = original;
            modifiedColor = original;
            onCC = onColorChanged;
            onCS = onColorSelected;
            useA = useAlpha;
            instance.gameObject.SetActive(true);
            instance.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = message;
            instance.RecalculateMenu(true);
            return true;
        }
        else
        {
            Done();
            return false;
        }
    }

    //called when color is modified, to update other UI components
    private void RecalculateMenu(bool recalculateHSV)
    {
        interact = false;
        if(recalculateHSV)
        {
            modifiedHsv = new HSV(modifiedColor);
        }
        else
        {
            modifiedColor = modifiedHsv.ToColor();
        }
        rComponent.value = modifiedColor.r;
        rComponent.transform.GetChild(3).GetComponent<TMP_InputField>().text = modifiedColor.r.ToString();
        gComponent.value = modifiedColor.g;
        gComponent.transform.GetChild(3).GetComponent<TMP_InputField>().text = modifiedColor.g.ToString();
        bComponent.value = modifiedColor.b;
        bComponent.transform.GetChild(3).GetComponent<TMP_InputField>().text = modifiedColor.b.ToString();
        mainComponent.value = (float)modifiedHsv.H;
        rComponent.transform.GetChild(0).GetComponent<RawImage>().color = new Color32(255, modifiedColor.g, modifiedColor.b, 255);
        rComponent.transform.GetChild(0).GetChild(0).GetComponent<RawImage>().color = new Color32(0, modifiedColor.g, modifiedColor.b, 255);
        gComponent.transform.GetChild(0).GetComponent<RawImage>().color = new Color32(modifiedColor.r, 255, modifiedColor.b, 255);
        gComponent.transform.GetChild(0).GetChild(0).GetComponent<RawImage>().color = new Color32(modifiedColor.r, 0, modifiedColor.b, 255);
        bComponent.transform.GetChild(0).GetComponent<RawImage>().color = new Color32(modifiedColor.r, modifiedColor.g, 255, 255);
        bComponent.transform.GetChild(0).GetChild(0).GetComponent<RawImage>().color = new Color32(modifiedColor.r, modifiedColor.g, 0, 255);
        positionIndicator.parent.GetChild(0).GetComponent<RawImage>().color = new HSV(modifiedHsv.H, 1d, 1d).ToColor();
        positionIndicator.anchorMin = new Vector2((float)modifiedHsv.S, (float)modifiedHsv.V);
        positionIndicator.anchorMax = positionIndicator.anchorMin;
        colorComponent.color = modifiedColor;
        onCC?.Invoke(modifiedColor);
        interact = true;
    }

    //used by EventTrigger to calculate the chosen value in color box
    public void SetChooser()
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(positionIndicator.parent as RectTransform, Input.mousePosition, GetComponentInParent<Canvas>().worldCamera, out Vector2 localpoint);
        localpoint = Rect.PointToNormalized((positionIndicator.parent as RectTransform).rect, localpoint);
        if (positionIndicator.anchorMin != localpoint)
        {
            positionIndicator.anchorMin = localpoint;
            positionIndicator.anchorMax = localpoint;
            modifiedHsv.S = localpoint.x;
            modifiedHsv.V = localpoint.y;
            RecalculateMenu(false);
        }
    }

    //gets main Slider value
    public void SetMain(float value)
    {
        if (interact)
        {
            modifiedHsv.H = value;
            RecalculateMenu(false);
        }
    }

    //gets r Slider value
    public void SetR()
    {
        if (interact)
        {
            modifiedColor.r = (byte)rComponent.value;
            RecalculateMenu(true);
        }
    }
    //sets r Slider value
    public void SetRV(float value)
    {
        modifiedColor.r = (byte)value;
        RecalculateMenu(true);
    }
    //gets r InputField value
    public void SetRI()
    {
        if(interact)
        {
            modifiedColor.r = (byte)Mathf.Clamp(int.Parse(rComp.text), 0, 255);
            RecalculateMenu(true);
        }
    }
    //gets g Slider value
    public void SetG()
    {
        if(interact)
        {
            modifiedColor.g = (byte)gComponent.value;
            RecalculateMenu(true);
        }
    }
    //sets g Slider value
    public void SetGV(float value)
    {
        modifiedColor.g = (byte)value;
        RecalculateMenu(true);
    }
    //gets g InputField value
    public void SetGI()
    {
        if (interact)
        {
            modifiedColor.g = (byte)Mathf.Clamp(int.Parse(gComp.text), 0, 255);
            RecalculateMenu(true);
        }
    }
    //gets b Slider value
    public void SetB()
    {
        if (interact)
        {
            modifiedColor.b = (byte)bComponent.value;
            RecalculateMenu(true);
        }
    }
    //sets b Slider value
    public void SetBV(float value)
    {
        modifiedColor.b = (byte)value;
        RecalculateMenu(true);
    }
    //gets b InputField value
    public void SetBI()
    {
        if (interact)
        {
            modifiedColor.b = (byte)Mathf.Clamp(int.Parse(bComp.text), 0, 255);
            RecalculateMenu(true);
        }
    }
    //gets a Slider value
    public void SetA(float value, bool forced = false)
    {
        if (interact || forced)
        {
            modifiedHsv.A = (byte)value;
            RecalculateMenu(false);
        }
    }
    //gets a InputField value
    public void SetA(string value)
    {
        if (interact)
        {
            modifiedHsv.A = (byte)Mathf.Clamp(int.Parse(value), 0, 255);
            RecalculateMenu(false);
        }
    }
    //gets hexa InputField value
    public void SetHexa(string value)
    {
        if (interact)
        {
            if (ColorUtility.TryParseHtmlString("#" + value, out Color c))
            {
                if (!useA) c.a = 1;
                modifiedColor = c;
                RecalculateMenu(true);
            }
        }
    }
    public static void Cancel()
    {
        modifiedColor = originalColor;
        Done();
    }
    public static void Done()
    {
        done = true;
        onCC?.Invoke(modifiedColor);
        onCS?.Invoke(modifiedColor);
    }
    //HSV helper class
    private sealed class HSV
    {
        public double H = 0, S = 1, V = 1;
        public byte A = 255;
        public HSV () { }
        public HSV (double h, double s, double v)
        {
            H = h;
            S = s;
            V = v;
        }
        public HSV (Color color)
        {
            float max = Mathf.Max(color.r, Mathf.Max(color.g, color.b));
            float min = Mathf.Min(color.r, Mathf.Min(color.g, color.b));

            float hue = (float)H;
            if (min != max)
            {
                if (max == color.r)
                {
                    hue = (color.g - color.b) / (max - min);

                }
                else if (max == color.g)
                {
                    hue = 2f + (color.b - color.r) / (max - min);

                }
                else
                {
                    hue = 4f + (color.r - color.g) / (max - min);
                }

                hue *= 60;
                if (hue < 0) hue += 360;
            }

            H = hue;
            S = (max == 0) ? 0 : 1d - ((double)min / max);
            V = max;
            A = (byte)(color.a * 255);
        }
        public Color32 ToColor()
        {
            int hi = Convert.ToInt32(Math.Floor(H / 60)) % 6;
            double f = H / 60 - Math.Floor(H / 60);

            double value = V * 255;
            byte v = (byte)Convert.ToInt32(value);
            byte p = (byte)Convert.ToInt32(value * (1 - S));
            byte q = (byte)Convert.ToInt32(value * (1 - f * S));
            byte t = (byte)Convert.ToInt32(value * (1 - (1 - f) * S));

            switch(hi)
            {
                case 0:
                    return new Color32(v, t, p, A);
                case 1:
                    return new Color32(q, v, p, A);
                case 2:
                    return new Color32(p, v, t, A);
                case 3:
                    return new Color32(p, q, v, A);
                case 4:
                    return new Color32(t, p, v, A);
                case 5:
                    return new Color32(v, p, q, A);
                default:
                    return new Color32();
            }
        }
    }
}