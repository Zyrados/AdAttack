#pragma strict

@CustomEditor(ScalablePlane)
class ScalablePlaneEditor extends Editor
{
	//Fancy menu item
	@MenuItem("GameObject/Create Other/Scalable Plane")
	static function Create()
	{
		//let Unity create a standard plane
		EditorApplication.ExecuteMenuItem("GameObject/Create Other/Plane");
		//(scan it, send it, fax - )rename it
		Selection.activeGameObject.name = "ScalablePlane";
		//add magic
		Selection.activeGameObject.AddComponent.<ScalablePlane>();
	}

	function OnInspectorGUI()
	{
		DrawDefaultInspector();
	}
}