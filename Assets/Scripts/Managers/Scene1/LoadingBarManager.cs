using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingBarManager : MonoBehaviour {

	// Progress bar
	private RectTransform bg, fg;
	private Text percentage;
	private int maxWidth;

	public void Initialize() {
		foreach (Transform t in gameObject.transform) {
			if (t.gameObject.name == "ContentSection") {
				foreach (Transform t2 in t) {
					if (t2.gameObject.name == "Bar") {
						foreach (Transform bars in t2) {
							if (bars.gameObject.name == "BackgroundBar")
								this.bg = bars.gameObject.GetComponent<RectTransform> ();
							if (bars.gameObject.name == "ForegroundBar")
								this.fg = bars.gameObject.GetComponent<RectTransform> ();
							if (bars.gameObject.name == "Percentage_text")
								this.percentage = bars.gameObject.GetComponent<Text> ();
						}
					}
				}
			}
		}
		this.maxWidth = (int)this.bg.rect.width;

		this.fg.sizeDelta = new Vector2 (0f, this.fg.rect.height);
	}

	public void UpdateBar(int incrementValue) {
		float f_incrementValue = incrementValue * (this.maxWidth / 100f);
		int newWidth = Mathf.Clamp((int)(this.fg.rect.width + f_incrementValue), 0, maxWidth);
		this.fg.sizeDelta = new Vector2 (newWidth, this.fg.rect.height);
		this.percentage.text = (newWidth / (float)this.maxWidth * 100).ToString("F0") + " %";
	}

	public void ReInitialize() {
		this.fg.sizeDelta = new Vector2 (0, this.fg.rect.height);
		this.percentage.text = "0% done";
	}
}
