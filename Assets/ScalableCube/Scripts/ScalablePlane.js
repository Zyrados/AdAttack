/**
 ScalablePlane © Sascha Graeff

 This script adds the ScalableCube functionality to planes.
**/

#pragma strict

private var me : Transform;
@script RequireComponent(MeshFilter)
private var f : MeshFilter;
@script ExecuteInEditMode

private var lastTransformScale : Vector3;
private var lastScale : Vector2;

var scale = Vector2.one;


function Awake()
{
	me = transform;
	f = GetComponent.<MeshFilter>();

	Update();
}

function Update()
{
	#if UNITY_EDITOR
		me = transform;
	    f = GetComponent.<MeshFilter>();
	#endif

	if(lastTransformScale != me.localScale || lastScale != scale)
	{
	    var s = Vector2(scale.x == 0 ? 0 : me.localScale.x / scale.x, scale.y == 0 ? 0 : me.localScale.z / scale.y);

	    var mesh = Mesh();
	    mesh.vertices = f.sharedMesh.vertices;
	    mesh.triangles = f.sharedMesh.triangles;
	    mesh.normals = f.sharedMesh.normals;
	    mesh.name = "Scalable Plane Mesh";
	    var uvs = new Vector2[mesh.vertices.length];
	    for(var i = 0; i < uvs.length; ++i)
	    {
	        var v = mesh.vertices[i];
            v /= 10;
            uvs[i] = Vector2(v.x * s.x, v.z * s.y) + Vector2(0.5f, 0.5f);
	    }
	    mesh.uv = uvs;
	    mesh.uv2 = uvs;

	    f.sharedMesh = mesh;


		lastScale = scale;
		lastTransformScale = me.localScale;
	}
}