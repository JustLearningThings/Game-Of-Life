using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridOverlay : MonoBehaviour {
	private Material _lineMaterial;

	public bool showMain = true;
	public bool showSub  = false;

	public int gridSizeX = 64;
	public int gridSizeY = 48;

	public float startX = -0.5f;
	public float startY = -0.5f;
	public float startZ = 0f;

	public float smallStep = 1;
	public float largeStep = 8;

	public                  Color mainColor = new Color(0f, 1f,   0f, 0f);
	public                  Color subColor  = new Color(0f, 0.5f, 0f, 1f);
	private static readonly int   SrcBlend  = Shader.PropertyToID("_SrcBlend");
	private static readonly int   DstBlend  = Shader.PropertyToID("_DstBlend");
	private static readonly int   ZWrite    = Shader.PropertyToID("_ZWrite");
	private static readonly int   Cull      = Shader.PropertyToID("_Cull");

	private void CreateLineMaterial() {
		if (_lineMaterial) {
			return;
		}

		Shader shader = Shader.Find("Hidden/Internal-Colored");

		_lineMaterial = new Material(shader) {
			hideFlags = HideFlags.HideAndDontSave
		};

		_lineMaterial.SetInt(SrcBlend, (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
		_lineMaterial.SetInt(DstBlend, (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
		_lineMaterial.SetInt(ZWrite,   0);
		_lineMaterial.SetInt(Cull,     (int)UnityEngine.Rendering.CullMode.Off);
	}

	private void OnDisable() {
		DestroyImmediate(_lineMaterial);
	}

	private void OnPostRender() {
		CreateLineMaterial();

		_lineMaterial.SetPass(0);

		GL.Begin(GL.LINES);

		if (showSub) {
			GL.Color(subColor);

			for (float y = 0; y <= gridSizeY; y += smallStep) {
				GL.Vertex3(startX, startY + y,         startZ);
				GL.Vertex3(startX         + gridSizeX, startY + y, startZ);
			}

			for (float x = 0; x < gridSizeX; x += smallStep) {
				GL.Vertex3(startX + x, startY,             startZ);
				GL.Vertex3(startX + x, startY + gridSizeY, startZ);
			}
		}

		if (showMain) {
			GL.Color(subColor);

			for (float y = 0; y <= gridSizeY; y += largeStep) {
				GL.Vertex3(startX, startY + y,         startZ);
				GL.Vertex3(startX         + gridSizeX, startY + y, startZ);
			}

			for (float x = 0; x < gridSizeX; x += largeStep) {
				GL.Vertex3(startX + x, startY,             startZ);
				GL.Vertex3(startX + x, startY + gridSizeY, startZ);
			}
		}

		GL.End();
	}
}
