using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Reflection.Emit;
using System.Linq;
using System;

public class TabManager : MonoBehaviour {

	// Main sections
	private Transform contentSection;

	// Contents
	private Transform cs_buttonTabsGroup;
	private Transform cs_tabsGroup;
	private GameObject[] cs_buttonTabs_selected;
	private GameObject[] cs_buttonTabs_target;
	private GameObject[] cs_tabs;



	// Start, get components and active the first tab
	public void Initialize () {
		InitializeVariables ();

		for (int i = 0; i < cs_buttonTabs_selected.Length; i++) this.cs_buttonTabs_selected [i].SetActive (false);
		for (int i = 0; i < cs_buttonTabs_target.Length; i++) this.cs_buttonTabs_target [i].SetActive (true);
		for (int i = 0; i < cs_tabs.Length; i++) this.cs_tabs [i].SetActive (false);
		this.cs_buttonTabs_selected[0].SetActive (true);
		this.cs_buttonTabs_target[0].SetActive (false);
		this.cs_tabs[0].SetActive (true);
	}

	// Initialize variables
	public void InitializeVariables () {
		foreach (Transform child in gameObject.transform) {
			if (child.name == "ContentSection") this.contentSection = child.transform;
		}

		foreach (Transform child in contentSection) {
			if (child.name == "ButtonTabs") this.cs_buttonTabsGroup = child.transform;
			if (child.name == "Tabs") this.cs_tabsGroup 			= child.transform;
		}

		int nbOfTabs = 0;
		foreach (Transform child in this.cs_buttonTabsGroup) {
			if (child.name.EndsWith ("selected")) nbOfTabs++;
		}

		this.cs_buttonTabs_selected = new GameObject[nbOfTabs];
		this.cs_buttonTabs_target = new GameObject[nbOfTabs];
		this.cs_tabs = new GameObject[nbOfTabs];
		foreach (Transform child in this.cs_buttonTabsGroup) {
			for (int i = 1; i <= nbOfTabs; i++) {
				if (child.name.StartsWith ("Button" + i)) {
					if (child.name.EndsWith ("selected")) this.cs_buttonTabs_selected [i - 1] = child.gameObject;
					else if (child.name.EndsWith ("target")) this.cs_buttonTabs_target [i - 1] = child.gameObject;
				}
			}
		}
		foreach (Transform child in cs_tabsGroup) {
			for (int i = 1; i <= nbOfTabs; i++) {
				if (child.name.StartsWith ("Tab" + i)) this.cs_tabs [i - 1] = child.gameObject;
			}
		}
	}

	// Reacts on a tab click
	public void ClickOnTab (GameObject tab) {
		for (int i = 0; i < cs_tabs.Length; i++) {
			cs_buttonTabs_target [i].SetActive (true);
			cs_buttonTabs_selected [i].SetActive (false);
		}
		int tabNumber = Int32.Parse(tab.name.Split (new[] { "Tab" }, StringSplitOptions.None).Last ());
		cs_buttonTabs_target [tabNumber - 1].SetActive (false);
		cs_buttonTabs_selected [tabNumber - 1].SetActive (true);

		for (int i = 0; i < cs_tabs.Length; i++) 
			cs_tabs [i].SetActive (false);
		cs_tabs [tabNumber - 1].SetActive (true);
	}


	// Get the tab number n
	public Transform GetTab (int number) {
		foreach (Transform child in contentSection) {
			if (child.name == "Tabs") {
				return child.GetChild (number - 1);
			}
		}
		return null;
	}
}
