using System;
using Infrastructure.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;
using Image = UnityEngine.UI.Image;

namespace DefaultNamespace
{
  public class GenericProgressBar : MonoBehaviour
  {
    [SerializeField] public GameObject backgroundBar;
    [SerializeField] public GameObject currentBar;
    [SerializeField] protected float maxBarWidth;
    [SerializeField] public TextMeshProUGUI progressTMP;
    public event Action<float,float> onCurrentValueUpdated;
    
    public void SetProgressBar(float current,float max,ProgressBarDisplayFormat format=ProgressBarDisplayFormat.CurrentPerMaxWithValue)
    {
      onCurrentValueUpdated?.Invoke(current,max);
      float newWidth = current == 0 || max == 0 ? 0 : maxBarWidth * (float)((current / max)>1.0? 1.0f : (current / max));
      currentBar.GetComponent<RectTransform>().sizeDelta = new Vector2(newWidth, currentBar.GetComponent<RectTransform>().sizeDelta.y);
      
      if (progressTMP != null)
      {
        switch (format)
        {
          case ProgressBarDisplayFormat.CurrentWithValue:
            progressTMP.text = current.ToString("F0");
            break;
          case ProgressBarDisplayFormat.CurrentWithPercent:
            progressTMP.text = (current/max*100).ToString("F0")+"%";
            break;
          case ProgressBarDisplayFormat.CurrentPerMaxWithPercent:
            progressTMP.text = (current/max*100).ToString("F0")+"% / "+100.ToString("F0")+"%";
            break;
          case ProgressBarDisplayFormat.CurrentWithRestTime:
            progressTMP.text = StringFormatter.GetTimeDisplayString(max-current);
            break;
          default:// ProgressBarDisplayFormat.CurrentPerMaxWithValue
            progressTMP.text = current.ToString("F0")+" / "+max.ToString("F0");
            break;
        }
      }
    }
  }
  
  public enum ProgressBarDisplayFormat
  {
    CurrentWithValue,
    CurrentWithPercent,
    CurrentWithRestTime,
    CurrentPerMaxWithValue,
    CurrentPerMaxWithPercent
  }
}