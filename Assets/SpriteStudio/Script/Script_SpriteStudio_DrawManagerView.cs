/**
	SpriteStudio5 Player for Unity

	Copyright(C) Web Technology Corp. 
	All rights reserved.
*/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[ExecuteInEditMode]
[System.Serializable]
public class Script_SpriteStudio_DrawManagerView : MonoBehaviour
{
	/* Valiable & Propaties */
	/* MEMO: Don't set/get "KindRenderQueueBase" and "OffsetDrawQueue". */
	/*       "KindRenderQueueBase" and "OffsetDrawQueue" are defined public for Setting on Inspector. */
	public Library_SpriteStudio.DrawManager.KindDrawQueue KindRenderQueueBase;
	public int OffsetDrawQueue;

	private List<Script_SpriteStudio_PartsRoot> DrawEntryPartsRoot = new List<Script_SpriteStudio_PartsRoot>();
	private Library_SpriteStudio.DrawManager.ArrayListMeshDraw arrayListMeshDraw;
	public Library_SpriteStudio.DrawManager.ArrayListMeshDraw ArrayListMeshDraw
	{
		get
		{
			return(arrayListMeshDraw);
		}
	}

	/* Functions */
	void Start()
	{
		arrayListMeshDraw = new Library_SpriteStudio.DrawManager.ArrayListMeshDraw();
		arrayListMeshDraw.BootUp();
		arrayListMeshDraw.RenderQueueSet(KindRenderQueueBase, OffsetDrawQueue);
	}
	
	void LateUpdate()
	{
		/* Clear Finalize ListDrawMesh */
		if(null == arrayListMeshDraw)
		{
			arrayListMeshDraw = new Library_SpriteStudio.DrawManager.ArrayListMeshDraw();
			arrayListMeshDraw.BootUp();
			arrayListMeshDraw.RenderQueueSet(KindRenderQueueBase, OffsetDrawQueue);
		}
		arrayListMeshDraw.Clear();

		/* Collect Draw-Parts from Root-Parts */
		Library_SpriteStudio.DrawManager.ArrayListMeshDraw ArrayListMeshDrawObject = null;
		Library_SpriteStudio.DrawManager.ListMeshDraw ListMeshDraw = null;
		foreach(var DrawObject in DrawEntryPartsRoot.OrderByDescending(x => x.GetDisplayOrder()))
		{
			ArrayListMeshDrawObject = DrawObject.ArrayListMeshDraw;
			int CountList = ArrayListMeshDrawObject.TableListMesh.Count;
			for(int j=0; j<CountList; j++)
			{
				/* Add Mesh-List */
#if false
				/* MEMO: Non-Generic List-Class */
				ListMeshDraw = ArrayListMeshDrawObject.TableListMesh[j] as Library_SpriteStudio.DrawManager.ListMeshDraw;
#else
				ListMeshDraw = ArrayListMeshDrawObject.TableListMesh[j];
#endif
				arrayListMeshDraw.TableListMesh.Add(ListMeshDraw);
			}

			/* Clear Original Draw-List */
			DrawObject.DrawListClear();
		}
		DrawEntryPartsRoot.Clear();

		/* Collect & Add Draw-Parts "Instance"-Object's Parts */
		Library_SpriteStudio.DrawManager.InformationMeshData InformationMeshData = null;
		Script_SpriteStudio_PartsRoot ScriptPartsRootSub = null;
		for(int i=0; i<arrayListMeshDraw.TableListMesh.Count; )
		{
#if false
			/* MEMO: Non-Generic List-Class */
			ListMeshDraw = arrayListMeshDraw.TableListMesh[i] as Library_SpriteStudio.DrawManager.ListMeshDraw;
#else
			ListMeshDraw = arrayListMeshDraw.TableListMesh[i];
#endif
			if(null == ListMeshDraw)
			{
				arrayListMeshDraw.TableListMesh.RemoveAt(i);
				continue;
			}
			InformationMeshData = ListMeshDraw.MeshDataTop;
			if(null != InformationMeshData.PartsInstance)
			{
				Library_SpriteStudio.DrawManager.ListMeshDraw ListMeshDrawSub = null;
				ScriptPartsRootSub = InformationMeshData.PartsInstance.ScriptPartsRootSub;
				if(null != ScriptPartsRootSub)
				{	/* Add Mesh-Table */
					/* Delete "Instance"-Data */
					arrayListMeshDraw.TableListMesh.RemoveAt(i);

					/* Insert "Instance"-Data's Draw-Mesh-List */
					ArrayListMeshDrawObject = ScriptPartsRootSub.ArrayListMeshDraw;
					for(int j=0; j<ArrayListMeshDrawObject.TableListMesh.Count; j++)
					{
#if false
						/* MEMO: Non-Generic List-Class */
						ListMeshDrawSub = ArrayListMeshDrawObject.TableListMesh[j] as Library_SpriteStudio.DrawManager.ListMeshDraw;
#else
						ListMeshDrawSub = ArrayListMeshDrawObject.TableListMesh[j];
#endif
						arrayListMeshDraw.TableListMesh.Insert((i + j), ListMeshDrawSub);
					}

					/* Clear Original Draw-List */
					ScriptPartsRootSub.DrawListClear();
					continue;
				}
			}
			i++;
		}

		/* Optimize Draw-Parts List */
		Library_SpriteStudio.DrawManager.ListMeshDraw ListMeshDrawNext = null;
#if false
		/* MEMO: Non-Generic List-Class */
		ArrayList TableListMesh = arrayListMeshDraw.TableListMesh;
#else
		List<Library_SpriteStudio.DrawManager.ListMeshDraw> TableListMesh = arrayListMeshDraw.TableListMesh;
#endif
		for(int i=0; i<(TableListMesh.Count - 1); )
		{
			var Count = i + 1;	/* "Count" is temporary */
#if false
			/* MEMO: Non-Generic List-Class */
			ListMeshDraw = TableListMesh[i] as Library_SpriteStudio.DrawManager.ListMeshDraw;
			ListMeshDrawNext = TableListMesh[Count] as Library_SpriteStudio.DrawManager.ListMeshDraw;
#else
			ListMeshDraw = TableListMesh[i];
			ListMeshDrawNext = TableListMesh[Count];
#endif
			if(ListMeshDraw.MaterialOriginal == ListMeshDrawNext.MaterialOriginal)
			{
				/* Mesh-List Merge */
				ListMeshDraw.ListMerge(ListMeshDrawNext);
				TableListMesh.RemoveAt(Count);

				/* Counter No-Incliment Continue */
				continue;
			}
			i++;
		}

		/* Counting Meshes */
		TableListMesh = arrayListMeshDraw.TableListMesh;
		int CountMesh = 0;
		for(int i=0; i< TableListMesh.Count; i++)
		{
#if false
			/* MEMO: Non-Generic List-Class */
			ListMeshDraw = TableListMesh[i] as Library_SpriteStudio.DrawManager.ListMeshDraw;
#else
			ListMeshDraw = TableListMesh[i];
#endif
			CountMesh += ListMeshDraw.Count;
		}

		/* Meshes Combine each Material & Set to MeshFilter/MeshRenderer */
		MeshFilter InstanceMeshFilter = GetComponent<MeshFilter>();
		MeshRenderer InstanceMeshRenderer = GetComponent<MeshRenderer>();
		arrayListMeshDraw.MeshSetCombine(InstanceMeshFilter, InstanceMeshRenderer, transform);
	}

	internal void DrawEntryObject(Script_SpriteStudio_PartsRoot PartsRootDrawObject)
	{
        DrawEntryPartsRoot.Add(PartsRootDrawObject);
	}

	void OnDestroy()
	{	/* MEMO: to make sure ... */
		MeshFilter InstanceMeshFilter = GetComponent<MeshFilter>();
		if(null != InstanceMeshFilter)
		{
			if(null != InstanceMeshFilter.sharedMesh)
			{
				InstanceMeshFilter.sharedMesh.Clear();
				Object.DestroyImmediate(InstanceMeshFilter.sharedMesh);
			}
			InstanceMeshFilter.sharedMesh = null;
		}
	}

	/* ******************************************************** */
	//! Set "Render-Queue"
	/*!
	@param	Kind
		"Render-Queue Base"<br>
		Library_SpriteStudio.DrawManager.KindDrawQueue.NON == use the settings in the Inspector.
	@param	Offset
		"Render-Queue Offset"<br>
		-1 == use the settings in the Inspector.
	@retval	Return-Value
		(None)

	Set to own "Render-Queue".<br>
	You must set "-1" to "Offset", when you set "Library_SpriteStudio.DrawManager.KindDrawQueue.NON" to "Kind".
	*/
	public void RenderQueueSet(Library_SpriteStudio.DrawManager.KindDrawQueue Kind=Library_SpriteStudio.DrawManager.KindDrawQueue.NON, int Offset=-1)
	{
		if(Library_SpriteStudio.DrawManager.KindDrawQueue.NON == Kind)
		{
			Kind = KindRenderQueueBase;
			Offset = OffsetDrawQueue;
		}
		if(-1 == OffsetDrawQueue)
		{	/* Error */
			Offset = OffsetDrawQueue;
		}
		if(null != arrayListMeshDraw)
		{
			arrayListMeshDraw.RenderQueueSet(Kind, Offset);
		}
	}
}