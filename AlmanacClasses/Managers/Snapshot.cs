using System.Collections;
using System.Linq;
using UnityEngine;

namespace AlmanacClasses.Managers;

public static class Snapshot
{
    public static void SnapshotItem(ItemDrop item, float lightIntensity = 1.3f, Quaternion? cameraRotation = null, Quaternion? itemRotation = null)
	{
		void Do()
		{
			const int layer = 30;

			Camera camera = new GameObject("Camera", typeof(Camera)).GetComponent<Camera>();
			camera.backgroundColor = Color.clear;
			camera.clearFlags = CameraClearFlags.SolidColor;
			camera.fieldOfView = 0.5f;
			camera.farClipPlane = 10000000;
			camera.cullingMask = 1 << layer;
			camera.transform.rotation = cameraRotation ?? Quaternion.Euler(90, 0, 45);

			Light topLight = new GameObject("Light", typeof(Light)).GetComponent<Light>();
			topLight.transform.rotation = Quaternion.Euler(150, 0, -5f);
			topLight.type = LightType.Directional;
			topLight.cullingMask = 1 << layer;
			topLight.intensity = lightIntensity;

			Rect rect = new(0, 0, 64, 64);

			GameObject visual;
			if (item.transform.Find("attach") is { } attach)
			{
				visual = UnityEngine.Object.Instantiate(attach.gameObject);
			}
			else
			{
				ZNetView.m_forceDisableInit = true;
				visual = UnityEngine.Object.Instantiate(item.gameObject);
				ZNetView.m_forceDisableInit = false;
			}
			if (itemRotation is not null)
			{
				visual.transform.rotation = itemRotation.Value;
			}

			foreach (Transform child in visual.GetComponentsInChildren<Transform>())
			{
				child.gameObject.layer = layer;
			}

			Renderer[] renderers = visual.GetComponentsInChildren<Renderer>();
			Vector3 min = renderers.Aggregate(Vector3.positiveInfinity, (cur, renderer) => renderer is ParticleSystemRenderer ? cur : Vector3.Min(cur, renderer.bounds.min));
			Vector3 max = renderers.Aggregate(Vector3.negativeInfinity, (cur, renderer) => renderer is ParticleSystemRenderer ? cur : Vector3.Max(cur, renderer.bounds.max));
			Vector3 size = max - min;

			camera.targetTexture = RenderTexture.GetTemporary((int)rect.width, (int)rect.height);
			float maxDim = Mathf.Max(size.x, size.z);
			float minDim = Mathf.Min(size.x, size.z);
			float yDist = (maxDim + minDim) / Mathf.Sqrt(2) / Mathf.Tan(camera.fieldOfView * Mathf.Deg2Rad);
			Transform cameraTransform = camera.transform;
			cameraTransform.position = ((min + max) / 2) with { y = max.y } + new Vector3(0, yDist, 0);
			topLight.transform.position = cameraTransform.position + new Vector3(-2, 0, 0.2f) / 3 * -yDist;

			camera.Render();

			RenderTexture currentRenderTexture = RenderTexture.active;
			RenderTexture.active = camera.targetTexture;

			Texture2D texture = new((int)rect.width, (int)rect.height, TextureFormat.RGBA32, false);
			texture.ReadPixels(rect, 0, 0);
			texture.Apply();

			RenderTexture.active = currentRenderTexture;

			item.m_itemData.m_shared.m_icons = new[] { Sprite.Create(texture, rect, new Vector2(0.5f, 0.5f)) };

			UnityEngine.Object.DestroyImmediate(visual);
			camera.targetTexture.Release();

			UnityEngine.Object.Destroy(camera);
			UnityEngine.Object.Destroy(topLight);
		}
		IEnumerator Delay()
		{
			yield return null;
			Do();
		}
		if (ObjectDB.instance)
		{
			Do();
		}
		else
		{
			AlmanacClassesPlugin._Plugin.StartCoroutine(Delay());
		}
	}
}