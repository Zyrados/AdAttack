
/**
 ScalableCube © Sascha Graeff

 This scripts purpose is to allow Unity users to create basic level geometry
 similar to the Hammer Editor of the Source Engine.
 It provides a special non-optimized cube mesh and recalculates
 the uv map of the instance depending on the cubes scale.
**/

/*
Quad UV scheme:
	1	2
	0	3
*/

#pragma strict

//Class for a quad of a cube.
//Nothing but 4 points and useful construct methods.
private class SWQuad
{
	var point1 : Vector3 = Vector3.zero;
	var point2 : Vector3 = Vector3.zero;
	var point3 : Vector3 = Vector3.zero;
	var point4 : Vector3 = Vector3.zero;

	function SWQuad(p1 : Vector3, p2 : Vector3, p3 : Vector3, p4 : Vector3)
	{
		point1 = p1;
		point2 = p2;
		point3 = p3;
		point4 = p4;
	}

	//methods for constructing the six quads of a cube
	static function Front()
	{
		return SWQuad(Vector3(-0.5,-0.5,-0.5), Vector3(-0.5,0.5,-0.5), Vector3(0.5,0.5,-0.5), Vector3(0.5,-0.5,-0.5));
	}

	static function Back()
	{
		return SWQuad(Vector3(0.5,-0.5,0.5), Vector3(0.5,0.5,0.5), Vector3(-0.5,0.5,0.5), Vector3(-0.5,-0.5,0.5));
	}

	static function Up()
	{
		return SWQuad(Vector3(-0.5,0.5,-0.5), Vector3(-0.5,0.5,0.5), Vector3(0.5,0.5,0.5), Vector3(0.5,0.5,-0.5));
	}

	static function Down()
	{
		return SWQuad(Vector3(0.5,-0.5,-0.5), Vector3(0.5,-0.5,0.5), Vector3(-0.5,-0.5,0.5), Vector3(-0.5,-0.5,-0.5));
	}

	static function Left()
	{
		return SWQuad(Vector3(-0.5,-0.5,0.5), Vector3(-0.5,0.5,0.5), Vector3(-0.5,0.5,-0.5), Vector3(-0.5,-0.5,-0.5));
	}

	static function Right()
	{
		return SWQuad(Vector3(0.5,-0.5,-0.5), Vector3(0.5,0.5,-0.5), Vector3(0.5,0.5,0.5), Vector3(0.5,-0.5,0.5));
	}
}


@script RequireComponent(MeshRenderer)
@script RequireComponent(MeshFilter)
@script ExecuteInEditMode()

//All quads of the cube, hardcoded.
//Modelled cubes are optimized to have the quads sharing points,
//which doesn't allow editing the uv map of each surface without maybe affecting another.
private static final var quads : SWQuad[] = [SWQuad.Front(),
																SWQuad.Back(),
																SWQuad.Up(),
																SWQuad.Down(),
																SWQuad.Left(),
																SWQuad.Right()];
//Stores the last localScale of the transform to decide
//whether the uv map should be recalculated or not.
//Saving the scene fires Update() in this script,
//this prevents the cube from becoming dirty afterwards.
private var lastScale : Vector3;
//we store the last uvPerMeter for the same purpose, too
private var lastUvPM : Vector2 = Vector2.one;
//Tiling without touching the material
var uvPerMeter : Vector2 = Vector2.one;

#if UNITY_EDITOR
//If we're in the editor, this is only called when we
//copy a ScalableCube. We want to create a new mesh then.
//Reference types, ya know.
function Awake()
{
    var oldMesh : Mesh = GetComponent(MeshFilter).sharedMesh;
    var newMesh : Mesh = null;
    if(oldMesh != null)
    {
        newMesh = Mesh();
        //copy all properties from old mesh
        newMesh.vertices = Array(oldMesh.vertices).ToBuiltin(Vector3) as Vector3[];
        newMesh.uv = Array(oldMesh.uv).ToBuiltin(Vector2) as Vector2[];
        newMesh.uv2 = Array(oldMesh.uv2).ToBuiltin(Vector2) as Vector2[];
        newMesh.triangles = Array(oldMesh.triangles).ToBuiltin(int) as int[];
        //create normals
		newMesh.RecalculateNormals();
		//calculate bounds
		newMesh.RecalculateBounds();
        //calculate tangents
		CalculateTangents(newMesh);
        //set name
        newMesh.name = "Scalable Cube Mesh";
    }
    GetComponent(MeshFilter).sharedMesh = newMesh;
}
#endif

function OnEnable()
{
	if(GetComponent(MeshFilter).sharedMesh == null || GetComponent(MeshFilter).sharedMesh.vertices.length != 6*4)
	{
		//create a mesh
		var mesh : Mesh = Mesh();
		//assign vertices
		mesh.vertices = GetVertices();
		//assign uvs
		mesh.uv = GetUVs();
		mesh.uv2 = GetUV2();
		//create triangle array
		var triangles : int[] = new int[quads.length * 6];
		var index0 : int = 0;
		for(var i : int = 0; i < triangles.length; i+=6)
		{
			triangles[i] = index0;
			triangles[i+1] = index0+1;
			triangles[i+2] = index0+2;
			triangles[i+3] = index0;
			triangles[i+4] = index0+2;
			triangles[i+5] = index0+3;
			index0 += 4;
		}
		//assign triangle array
		mesh.triangles = triangles;
		//create normals
		mesh.RecalculateNormals();
		//calculate bounds
		mesh.RecalculateBounds();
		//calculate tangents
		CalculateTangents(mesh);
        //set name
        mesh.name = "Scalable Cube Mesh";
		//assign mesh to MeshFilter Component
		GetComponent(MeshFilter).sharedMesh = mesh;

        if(GetComponent.<Renderer>().sharedMaterial == null)
        {
            //Add standard gray material
            GetComponent.<Renderer>().material = Material(Shader.Find("Diffuse"));
        }
	}

	//initially store localScale
	lastScale = transform.localScale;
}

function Update()
{
	//has the cube been scaled or has uvPerMeter been changed?
	if(lastScale != transform.localScale || uvPerMeter != lastUvPM)
	{
		//get mesh >> logs error! but what can we do...
		var mesh : Mesh = GetComponent(MeshFilter).sharedMesh;
		//get uv map from mesh
		var uv : Vector2[] = mesh.uv;

		//recalculate uv map
		var uvindex : int = 0;
		for(var face : int = 0; face < 6; ++face)
		{
			for(var point : int = 0; point < 4; ++point)
			{
				//get original uv map data (0,0 / 0,1 / 1,0 / 1,1) - see "Quad UV Scheme"
				var originalUV : Vector2 = Vector2(point < 2 ? 0 : 1 , point == 0 || point == 3 ? 0 : 1);
				switch(face)
				{
					//front or back face
					case 0: case 1:
						uv[uvindex].x = originalUV.x * transform.localScale.x * uvPerMeter.x;
						uv[uvindex].y = originalUV.y * transform.localScale.y * uvPerMeter.y;
					break;
					//up or down face
					case 2: case 3:
						uv[uvindex].x = originalUV.x * transform.localScale.x * uvPerMeter.x;
						uv[uvindex].y = originalUV.y * transform.localScale.z * uvPerMeter.y;
					break;
					//left or right face
					case 4: case 5:
						uv[uvindex].x = originalUV.x * transform.localScale.z * uvPerMeter.x;
						uv[uvindex].y = originalUV.y * transform.localScale.y * uvPerMeter.y;
					break;
				}
				++uvindex;
			}
		}

		//assign uv map
		mesh.uv = uv;
		//store localScale and uvPerMeter
		lastScale = transform.localScale;
		lastUvPM = uvPerMeter;
	}
}

//Get vertice (point) array from quad array
private function GetVertices()
{
	var result : Array = Array();
	for(var quad : SWQuad in quads)
	{
		result.Add(quad.point1);
		result.Add(quad.point2);
		result.Add(quad.point3);
		result.Add(quad.point4);
	}
	return result.ToBuiltin(Vector3) as Vector3[];
}

//Get uv array for all quads (see "Quad UV Scheme")
private function GetUVs()
{
	var result : Array = Array();
	for(var vertice : SWQuad in quads)
	{
		result.Add(Vector2(0,0));
		result.Add(Vector2(0,1));
		result.Add(Vector2(1,1));
		result.Add(Vector2(1,0));
	}
	return result.ToBuiltin(Vector2) as Vector2[];
}

//Returns lightmap uvs
private function GetUV2()
{
	var result : Array = Array();
	var third : float = 1.0/3.0;
	for(var x : int = 0; x < 3; ++x)
	{
		for(var y : int = 0; y < 2; ++y)
		{
			result.Add(Vector2(x*third,y*third));
			result.Add(Vector2(x*third,(y+1)*third));
			result.Add(Vector2((x+1)*third,(y+1)*third));
			result.Add(Vector2((x+1)*third,y*third));
		}
	}
	return result.ToBuiltin(Vector2) as Vector2[];
}

//Calculate tangents for nifty shaders who need them
//Function partly from Unity3d.com Forum
function CalculateTangents(theMesh : Mesh)
{
	var vertexCount = theMesh.vertexCount;
	var vertices = theMesh.vertices;
	var normals = theMesh.normals;
	var texcoords = theMesh.uv;
	var triangles = theMesh.triangles;
	var triangleCount = triangles.length/3;
	var tangents = new Vector4[vertexCount];
	var tan1 = new Vector3[vertexCount];
	var tan2 = new Vector3[vertexCount];
	var tri = 0;
	for (var i = 0; i < (triangleCount); i++)
	{
		var i1 = triangles[tri];
		var i2 = triangles[tri+1];
		var i3 = triangles[tri+2];

		var v1 = vertices[i1];
		var v2 = vertices[i2];
		var v3 = vertices[i3];

		var w1 = texcoords[i1];
		var w2 = texcoords[i2];
		var w3 = texcoords[i3];

		var x1 = v2.x - v1.x;
		var x2 = v3.x - v1.x;
		var y1 = v2.y - v1.y;
		var y2 = v3.y - v1.y;
		var z1 = v2.z - v1.z;
		var z2 = v3.z - v1.z;

		var s1 = w2.x - w1.x;
		var s2 = w3.x - w1.x;
		var t1 = w2.y - w1.y;
		var t2 = w3.y - w1.y;

		var r = 1.0 / (s1 * t2 - s2 * t1);
		var sdir = new Vector3((t2 * x1 - t1 * x2) * r, (t2 * y1 - t1 * y2) * r, (t2 * z1 - t1 * z2) * r);
		var tdir = new Vector3((s1 * x2 - s2 * x1) * r, (s1 * y2 - s2 * y1) * r, (s1 * z2 - s2 * z1) * r);

		tan1[i1] += sdir;
		tan1[i2] += sdir;
		tan1[i3] += sdir;

		tan2[i1] += tdir;
		tan2[i2] += tdir;
		tan2[i3] += tdir;

		tri += 3;
	}

	for (var j = 0; j < (vertexCount); j++)
	{
		var n = normals[j];
		var t = tan1[j];

		// Gram-Schmidt orthogonalize
		Vector3.OrthoNormalize(n,t);

		tangents[j].x = t.x;
		tangents[j].y = t.y;
		tangents[j].z = t.z;

		// Calculate handedness
		tangents[j].w = ( Vector3.Dot(Vector3.Cross(n, t), tan2[j]) < 0.0 ) ? -1.0 : 1.0;
	}       
	theMesh.tangents = tangents;
}