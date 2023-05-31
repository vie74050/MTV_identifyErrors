using UnityEngine;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine.SceneManagement;
using System.Linq;

[RequireComponent(typeof(KGFOrbitCam))]

public class ActivityController : MonoBehaviour {
	

	// from Plugins/JSLibs to communicate with browser javascript
	[DllImport("__Internal")]
	private static extern void BrowserApplicationStarted();

	[DllImport("__Internal")]
	private static extern void BrowserItemListString(string str);

	[DllImport("__Internal")]
	private static extern void BrowserSelect(string str);

	/* set up options */

	[Tooltip("Required: the game object parent- anything not child of this will not be selectable")]
	public GameObject _MODEL;

	[Tooltip("Optional: distance from original position; part will snap back if under specified amount.  Use 0 = can't be removed ")]
	public float snapbackDistMin = 5;
	[Tooltip("Optional: max distance SOs can be before snap back. Should be greater than min.")]
	public float snapbackDistMax = 10;

	[Tooltip("Optional: if part name label is displayed when part selected")]
	public bool showlabels = false;

	[Tooltip("show or hide parts list")]
	public bool showPartsList = true;

	[Tooltip("Optional: tint color for selected items")]
	public Color selectedColor = Color.yellow;

    [Tooltip("Optional: show alpha mode toggle?")]
    public bool showAlphaMode = true;

	[Tooltip("Optional: start in alpha mode?")]
	public bool alphaMode = true;

	public Texture logo;

	private GUISkin _skin;

	// Task Activity gui vars

	[HideInInspector]
	public List<GameObject> activityList;

	[HideInInspector]
	public List<EditorGameObject> taskItems;

    [HideInInspector]
    public GameObject TargetEndPos;

	[HideInInspector]
	public bool showActivity = false;


	[HideInInspector]
	public bool showActivitySteps = false;

	[HideInInspector]
	public bool showLogBtn = false;

	[HideInInspector]
	public string activityTaskName = "Task";

	[HideInInspector]
	public Transform currentSelection;

	private List<ListChecklistItem> activityCheckList;
	private int activityCounter = 0;		// keep track of what step has been done\
	private int activityStepsTTL = 0;		// total number of steps
	private static List<string> _activityLog;	
	
	private KGFOrbitCam camsettings;
	
	// hierarchy list
	private List<ListItem> items;
	private int listId = 0; 

	/* gui vars */
	private bool overGUI = false;  // detect if mouse is over gui elements
	private Rect m_SelectionWindowRect = new Rect (600.0f, 10.0f, 120.0f, 50.0f);

	private bool showLog = false;

	// gui vars for parts list 
	private Vector2 scrollPosition = Vector2.zero;
	private Rect scrollviewOutter = new Rect(0, 0, 350, Screen.height);

	private int list_level = 0;

	// gui vars for activity list 
	private Vector2 scrollPosition2 = Vector2.zero;
	private Rect scrollviewOutter2 = new Rect(Screen.width-320, 0, 320, 110);
	private bool activityStepsOnOff = false;

	// gui vars for log list 
	private Vector2 scrollPosition3 = Vector2.zero;
	private Rect scrollviewOutter3 = new Rect(Screen.width-320, Screen.height-200, 320, 200);

	private string overpart_name = "";
	private Animation anim;
	private LayerMask layerMask;

	// scene management
	
	int totScenes;
	Scene activeScene;
	// Make sure to add scene to Build Settings in same order
	public string[] sceneNames;

	void Awake(){
		LoadScenes();

		layerMask = LayerMask.GetMask("Default");
					
		if (_skin == null) {
			_skin = Resources.Load ("default") as GUISkin;
		} 

		// set up global behavious: camera, selection, colliders...etc.
		camsettings = Camera.main.GetComponent<KGFOrbitCam>();
		if (_MODEL == null)
			_MODEL = gameObject;

		_MODEL.SetActive (true);
		
		Renderer[] rds = _MODEL.GetComponentsInChildren<Renderer>();

		foreach( Renderer objRenderer in rds){
			string partname = objRenderer.name; //Debug.Log (partname);

			// add selectable object using default options to children of _MODEL if doesn't already have one
			SelectableObject so = objRenderer.gameObject.GetComponent<SelectableObject>();
			Dropzone dz = objRenderer.gameObject.GetComponent<Dropzone>();
			if (so == null && dz == null) {
				objRenderer.gameObject.AddComponent<SelectableObject>();
				so = objRenderer.gameObject.GetComponent<SelectableObject>();
				so.snapbackDistMin = snapbackDistMin; //activity mode, default always snap back unless correct
				so.snapbackDistMax = snapbackDistMax;
				so.alphaMode = alphaMode;
				
				so.Init();
				
			}

			if (so != null)
			{
				if (so.highlighColour == Color.clear
				&& so.highlighColour != selectedColor)
				{
					so.highlighColour = selectedColor;
				}

				if (showActivity && TargetEndPos)
				{

					Transform[] targets = TargetEndPos.GetComponentsInChildren<Transform>();
					foreach (Transform targetT in targets)
					{

						if (targetT.name == partname)
						{

							so.SetTargetPos(targetT);

							break;
						}
					}

					TargetEndPos.SetActive(false);
				}
			}
			

		};	
			
	}

	void Start(){
		
		_activityLog = new List<string>();
		_activityLog.Add ("Task Start: " + System.DateTime.Now );
		
		if (showActivity) {
			MaketaskList ();
		}

		anim = _MODEL.GetComponent<Animation> ();

		items = new List<ListItem>();
		MakeListItems(_MODEL, 0);

		if (Application.platform == RuntimePlatform.WebGLPlayer) {
			//Application.ExternalCall ("FromUnity_ApplicationStarted", true);
			#if UNITY_WEBGL && !UNITY_EDITOR
			BrowserApplicationStarted();
			
			#endif
		}
		
	}

	void Update(){
				
		RaycastHit hit;
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

		if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
		{
			overpart_name = hit.transform.name;
			SelectableObject so = hit.transform.GetComponent<SelectableObject>();
			if (so != null){
				overpart_name = so.displayName;
				// object rotate with right mouse drag if over s.o
				if (Input.GetMouseButton(1) 
					&& so.isDraggable 
					&& currentSelection==null
					&& !so.IsAtOrigPos())
				{
					currentSelection = hit.transform;
				}
			
			}
		}
		else
		{
			overpart_name = "";
		}

		if (Input.GetMouseButton(1)){ 
			if (currentSelection !=null) {
				
				float h = 10 * Input.GetAxis("Mouse X");
				float v = 10 * Input.GetAxis("Mouse Y");

				currentSelection.Rotate(-v, 0, h);
			}
		}else {
			currentSelection = null; 
		}

		camsettings.SetZoomEnable(!overGUI);
		camsettings.SetRotationEnable(!overGUI && currentSelection==null);
		
	}
	
	void OnGUI(){
		GUI.skin = _skin;

		GUILayout.BeginHorizontal();

		if (logo != null) {
			GUI.Label(new Rect(Screen.width - 100, Screen.height - 100, 100, 100), logo);
		}

		// global control buttons ///

		// scene load btns
		if (sceneNames.Length > 0)
		{
			GUI_SceneSelection();
		}

		if (showAlphaMode || showPartsList) {
			if (GUILayout.Button("Deselect All")) {
				GameObject[] dragTagObjs = GameObject.FindGameObjectsWithTag ("Draggable");
				foreach (GameObject go in dragTagObjs) {
					SelectableObject so = go.GetComponent<SelectableObject> ();
					so.Deselect ();

				}
				ClearBrowserSelect();
			}
		}
		
        // transparencey btn
        if (showAlphaMode)
        {
            alphaMode = GUILayout.Toggle(alphaMode, "Isolate", new GUIStyle("button"));
        }

		if (showPartsList) {
			items[0].selected = GUILayout.Toggle (items[0].selected, items[0].node.name, new GUIStyle ("button"));
		} 
		else {
			items[0].selected = false;
		}

		// anim btn
		if (anim != null) {
			if ( GUILayout.Button("Expand") ){
				anim.Play ();
			}
		}

		GUILayout.EndHorizontal();

		// options
		if (showPartsList) {
			GUI_PartsList ();
		}

		if (showlabels)
		{
			GUI_Showlabel(overpart_name);
		}

		if (showActivity) {
			// if not empty, display list
			GUI_ActivityList();
		}

		if (showLog && showLogBtn) {
			GUI_TaskLog();
		}

		bool over_partslist = (showPartsList && scrollviewOutter.Contains(Event.current.mousePosition));
		bool over_activity = (showActivity && scrollviewOutter2.Contains(Event.current.mousePosition));
		bool over_log = (showLog && scrollviewOutter3.Contains(Event.current.mousePosition));
		overGUI = over_activity;

		//Debug.Log(overGUI);

		if (GUI.changed) {
			foreach(ListItem li in items){
				if (li.so != null){
					li.so.alphaMode = alphaMode;
					if (!li.so.isSelected) {
						li.so.Deselect (); 
					} else {
						li.so.Select ();
					}
				}

			}
           
		}

	}

	// Method for Browser to get updated item lists
	void SetBrowserItemsList() {
		List<string> listitems = GetSOListItems();
		string listitems_str = string.Join("\\", listitems); //Debug.Log(listitems_str);
		if (Application.platform == RuntimePlatform.WebGLPlayer) {
			//Application.ExternalCall ("FromUnity_ApplicationStarted", true);
			#if UNITY_WEBGL && !UNITY_EDITOR
			BrowserItemListString(listitems_str);
			#endif
		}
	}
	///	<returns> List<string> Names of all Selectable Objects that are isListItem true</returns>
	private List<string> GetSOListItems () {
		SelectableObject[] sos = _MODEL.GetComponentsInChildren<SelectableObject>();
		List<string> listSos = new List<string>();
		
		foreach (SelectableObject so in sos) {
			if (so.isListItem && so.isActiveAndEnabled) {
				listSos.Add(so.name); //Debug.Log(so.name);
			}
		}

		return listSos;
	}
	private void GUI_SceneSelection()
	{
		GUILayout.BeginVertical();
		for (int i = 0; i < totScenes; i++)
		{
			Scene scene = SceneManager.GetSceneByBuildIndex(i);
			
			string sceneName = (activeScene == scene) ? "Reset" : sceneNames[i];

			//print(scene.path);

			if (GUILayout.Button(sceneName, new GUIStyle("btn_menu")) ){
				SceneManager.LoadSceneAsync(i, LoadSceneMode.Single);

				ClearBrowserSelect();
			};
		}
		GUILayout.EndVertical();
		GUILayout.Space(30);

	}

	private void ClearBrowserSelect()
	{
		// clear dialogue in web
		if (Application.platform == RuntimePlatform.WebGLPlayer)
		{
			#if UNITY_WEBGL && !UNITY_EDITOR
				BrowserSelect("");
			#endif
		}
	}
	private void GUI_Showlabel(string s)
	{
		GUIStyle style;
		// GUIStyle for part label
		Texture2D texture = new Texture2D(1, 1);
		style = new GUIStyle();
		style.normal.background = texture;
		style.normal.textColor = Color.black;
		style.wordWrap = true;
		style.alignment = TextAnchor.MiddleCenter;
		style.padding = new RectOffset(3, 3, 5, 5);
		texture.SetPixel(1, 1, Color.white);
		texture.Apply();

		s.Replace("_", " ");
		if (s != "")
		{
			m_SelectionWindowRect.x = Input.mousePosition.x + 30;
			m_SelectionWindowRect.y = Screen.height - Input.mousePosition.y - 30;
			m_SelectionWindowRect.height = style.CalcHeight(new GUIContent(s), m_SelectionWindowRect.width);
			GUI.Label(m_SelectionWindowRect, s, style);
		}
	}		

	private void GUI_PartsList(){

		// parts list
		if (items[0].selected){
			
			scrollPosition = GUILayout.BeginScrollView(scrollPosition,  new GUIStyle("scrollview"), GUILayout.MaxWidth(320) );

			foreach(ListItem li in items){
				//Debug.Log(li.id + ", " + li.objPathName);
				string pathname = li.objPathName;
				string btnstyle = (li.childIndices.Count > 0) ? "toggle_button_1" : "button_1";
				
				// object names should not have / in it
				list_level = pathname.Split('/').Length -1;
				
				int indent_amt = list_level * 10 + ((li.childIndices.Count > 0) ? 0 : 14);

				if (li.showBtn) {

					if (li.so == null) { 
						
						// make group container
						GUILayout.BeginHorizontal ();
						GUILayout.Space (indent_amt);

						if (li.id != 0) {
							Transform[] t = li.node.GetComponentsInChildren<Transform>();

							GUILayout.BeginHorizontal ();
							li.selected = GUILayout.Toggle (li.selected, li.node.name); 

							// select all children
							if ( GUILayout.Button("all", new GUIStyle ("smallbtn") ) ) {
								li.selected = true;

								foreach (Transform child in t) {
									SelectableObject so = child.GetComponent<SelectableObject> ();
									if (so!=null){
										so.Select ();
									}
								};
							}
							if ( GUILayout.Button("none", new GUIStyle ("smallbtn") ) ) {
								li.selected = false;
								foreach (Transform child in t) {
									SelectableObject so = child.GetComponent<SelectableObject> ();
									if (so!=null){
										so.Deselect ();
									}
								};
							}

							GUILayout.EndHorizontal ();
						}

						GUILayout.EndHorizontal ();
					} else {
						
						
						if (li.so.isSelected)
							btnstyle = "button_selected";
						
						li.so.alphaMode = alphaMode;

						// make button
						GUILayout.BeginHorizontal ();
						GUILayout.Space (indent_amt);

						if (GUILayout.Button (li.node.name, new GUIStyle (btnstyle))) {
							
							li.selected = !li.so.isSelected;

							if (!li.selected) {
								li.so.Deselect ();
							} else {
								li.so.Select ();
							}		
						}	
						GUILayout.EndHorizontal ();
					}

					if (li.childIndices.Count > 0) {
						bool showchildren = li.selected && li.showBtn;

						if (li.so != null)
							showchildren = li.so.isSelected;


						foreach (int i in li.childIndices) {
							if (items[i].so != null) {
								if (items[i].so.isListItem)
								{
									items[i].showBtn = (items[i].so.isSelected) ? items[i].so.isSelected : showchildren;
								}
								
							} else {
								items[i].showBtn = showchildren;
							}

						}

					}

				} else {
					
					foreach (int i in li.childIndices) {

						if (items[i].so != null) {
							li.showBtn = items[i].showBtn = items[i].so.isSelected;
							if (items[i].so.isSelected){

								break;
							}
						}

					}
				}

			}
			GUILayout.EndScrollView();
		}

		Rect scrollviewRect = GUILayoutUtility.GetLastRect();
		if (  scrollviewRect.width == scrollviewOutter.width ){
			scrollviewOutter.height = scrollviewRect.height;
		};
	}

	// ordered sequence of tasks - gets checked off when user clicks item
	private void GUI_ActivityList(){
		string msg = "";
		string labelStyle = "activitylist";
		float w = scrollviewOutter2.width;
		float x = Screen.width - scrollviewOutter2.width;
		float h = (!activityStepsOnOff) ? 135 : 135 + activityStepsTTL * 20;
		Rect activityRect = new Rect(x, 0, w, h+20);
		
		scrollviewOutter2.height = h;

		GUILayout.BeginArea (activityRect, new GUIStyle ("window"));

		GUILayout.BeginHorizontal ();
		
		if (showActivitySteps)
		{
			if (GUILayout.Button("Show/Hide Steps", GUILayout.ExpandWidth(true)))
			{
				activityStepsOnOff = !activityStepsOnOff;
			}
		}
				
		if (showLogBtn) {
			showLog = GUILayout.Toggle (showLog, "Log", new GUIStyle ("button"));
		}
		GUILayout.EndHorizontal ();
		GUILayout.Space(10);
		GUILayout.Label (activityTaskName, new GUIStyle("activityDescription"));
		
		scrollPosition2 = GUILayout.BeginScrollView(scrollPosition2, new GUIStyle ("box"), GUILayout.MaxWidth(scrollviewOutter2.width) );

		//do GUI for activity list
		activityCounter = 0;
		foreach (GameObject go in activityList)
		{
			SelectableObject so = go.GetComponent<SelectableObject>();
			Dropzone dz = go.GetComponent<Dropzone>();

				
			if (dz)
			{
				// for dropzone
				foreach(DropzoneList dzl in dz.checklist)
				{
					labelStyle = dzl.isComplete ?  "complete" : "activitylist";
					if (activityStepsOnOff)
					{
						GUILayout.Label(dzl.GetName(), new GUIStyle(labelStyle));
					}
					if (labelStyle == "complete")
					{
						activityCounter++;
					}
				}
					
			}else if (so)
			{
				// for selectable object
				labelStyle = so.IsTaskComplete() ? "complete" : "activitylist";

				if (activityStepsOnOff)
				{
					GUILayout.Label(go.name, new GUIStyle(labelStyle));
				}
				if (labelStyle == "complete")
				{
					activityCounter++;
				}
			}

			
		}
		if (!activityStepsOnOff)
		{
			GUILayout.Label("Progress: " + activityCounter + " / " + activityStepsTTL, new GUIStyle("activitylist"));
		} 

		GUILayout.EndScrollView ();	

		if (activityCounter == activityStepsTTL) {
			msg = "Task Completed!";
		
			if (_activityLog[_activityLog.Count-1] != msg) {
				
				_activityLog.Add (msg);
			}

		}

		GUILayout.Label(msg, (msg != "" ? new GUIStyle("bigcomplete") : new GUIStyle("activitylist")));

		GUILayout.EndArea();

	}

	private void GUI_TaskLog(){
		string guitext = "";

		Rect tasklogArea = scrollviewOutter3;
		tasklogArea.x = Screen.width - scrollviewOutter3.width;
		tasklogArea.y = Screen.height - scrollviewOutter3.height;

		GUILayout.BeginArea (tasklogArea, new GUIStyle ("box"));
		GUILayout.Label ("Event Log");

		scrollPosition3 = GUILayout.BeginScrollView(scrollPosition3, new GUIStyle ("box"), GUILayout.MaxWidth(scrollviewOutter3.width) );

		foreach (string s in _activityLog) {
			guitext += s + "\n";

			GUILayout.Label (s);
		}

		GUILayout.EndScrollView ();
		GUILayout.EndArea ();
	}

	private void ResetAll()
	{
		activeScene = SceneManager.GetActiveScene();
		ClearBrowserSelect();
		SceneManager.LoadScene(activeScene.name);
	}
	private void LoadScenes()
	{
		// set scene vars
		totScenes = SceneManager.sceneCountInBuildSettings;
		activeScene = SceneManager.GetActiveScene();
		//string currentSceneName = activeScene.name;

	}

	private void MakeListItems(GameObject go, int parent_id){

		ListItem node = new ListItem(listId, parent_id, go);
		items.Add(node);

		foreach(Transform t in go.transform) {
			Renderer[] rds = t.GetComponentsInChildren<Renderer>();
			SelectableObject so = t.GetComponent<SelectableObject>();

			// ignore items with SO where isListItem prop is false; 
			if (so != null)
			{
				//print(so.transform.name + ": " + so.isListItem);
				if (so.isListItem != true)
				{
					continue;
					//break;
				}
			}
			// ignore helpers: items that have no renderers at all (parent or child)
			
			if ( rds.Length > 0 )
			{
				
				
				listId++;
				node.AddChildRef(listId);
				MakeListItems(t.gameObject, node.id);
			}
			

		}
	}

	private void MaketaskList() {
		//Debug.Log ("taskItems:" + taskItems.Count);
		foreach (EditorGameObject taskstep in taskItems) {
			if (taskstep.goRef != null) {
				GameObject go = taskstep.goRef;
				SelectableObject so = go.GetComponent<SelectableObject>();
				Dropzone dz = go.GetComponent<Dropzone>();
				
				activityList.Add(go);

				if (so)
				{
					activityStepsTTL++;
				}
				else if (dz)
				{
					foreach (DropzoneList dzl in dz.checklist)
					{
						activityStepsTTL++;
					}
				}
				
			}
		}

	}
}
