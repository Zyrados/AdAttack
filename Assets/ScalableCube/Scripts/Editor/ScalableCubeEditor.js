#pragma strict

class ScalableCubeEditor
{
	//Fancy menu item
	@MenuItem("GameObject/Create Other/Scalable Cube")
	static function Create()
	{
		//Create GameObject
		var go : GameObject = GameObject("ScalableCube");
		//Add ScalableCube script (adding MeshRenderer and MeshFilter through RequireComponent())
		go.AddComponent.<ScalableCube>();
		//Add BoxCollider
		go.AddComponent.<BoxCollider>();
	}
}