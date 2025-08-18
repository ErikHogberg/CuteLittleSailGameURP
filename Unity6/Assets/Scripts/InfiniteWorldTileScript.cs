using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfiniteWorldTileScript : MonoBehaviour {

	[Tooltip("If true, copies this object 8 times and overwrites the neighbor tile references with the copies")]
	public bool CopyThisTile;

	public Vector2 NeighborOffset = Vector2.one;

	public InfiniteWorldTileScript NNeighbor;
	public InfiniteWorldTileScript NENeighbor;
	public InfiniteWorldTileScript ENeighbor;
	public InfiniteWorldTileScript SENeighbor;
	public InfiniteWorldTileScript SNeighbor;
	public InfiniteWorldTileScript SWNeighbor;
	public InfiniteWorldTileScript WNeighbor;
	public InfiniteWorldTileScript NWNeighbor;

	private void Start() {

		if (CopyThisTile)
			CopyThisTile = false;
		else
			return;

		NNeighbor = Instantiate(gameObject, transform.parent).GetComponent<InfiniteWorldTileScript>();
		NENeighbor = Instantiate(gameObject, transform.parent).GetComponent<InfiniteWorldTileScript>();
		ENeighbor = Instantiate(gameObject, transform.parent).GetComponent<InfiniteWorldTileScript>();
		SENeighbor = Instantiate(gameObject, transform.parent).GetComponent<InfiniteWorldTileScript>();
		SNeighbor = Instantiate(gameObject, transform.parent).GetComponent<InfiniteWorldTileScript>();
		SWNeighbor = Instantiate(gameObject, transform.parent).GetComponent<InfiniteWorldTileScript>();
		WNeighbor = Instantiate(gameObject, transform.parent).GetComponent<InfiniteWorldTileScript>();
		NWNeighbor = Instantiate(gameObject, transform.parent).GetComponent<InfiniteWorldTileScript>();

		NNeighbor.NNeighbor = SNeighbor;
		NNeighbor.NENeighbor = SENeighbor;
		NNeighbor.ENeighbor = NENeighbor;
		NNeighbor.SENeighbor = ENeighbor;
		NNeighbor.SNeighbor = this;
		NNeighbor.SWNeighbor = WNeighbor;
		NNeighbor.WNeighbor = NWNeighbor;
		NNeighbor.NWNeighbor = SWNeighbor;

		NENeighbor.NNeighbor = SENeighbor;
		NENeighbor.NENeighbor = SWNeighbor;
		NENeighbor.ENeighbor = NWNeighbor;
		NENeighbor.SENeighbor = WNeighbor;
		NENeighbor.SNeighbor = ENeighbor;
		NENeighbor.SWNeighbor = this;
		NENeighbor.WNeighbor = NNeighbor;
		NENeighbor.NWNeighbor = SNeighbor;

		ENeighbor.NNeighbor = NENeighbor;
		ENeighbor.NENeighbor = NWNeighbor;
		ENeighbor.ENeighbor = WNeighbor;
		ENeighbor.SENeighbor = SWNeighbor;
		ENeighbor.SNeighbor = SENeighbor;
		ENeighbor.SWNeighbor = SNeighbor;
		ENeighbor.WNeighbor = this;
		ENeighbor.NWNeighbor = NNeighbor;

		SENeighbor.NNeighbor = ENeighbor;
		SENeighbor.NENeighbor = WNeighbor;
		SENeighbor.ENeighbor = SWNeighbor;
		SENeighbor.SENeighbor = NWNeighbor;
		SENeighbor.SNeighbor = NENeighbor;
		SENeighbor.SWNeighbor = NNeighbor;
		SENeighbor.WNeighbor = SNeighbor;
		SENeighbor.NWNeighbor = this;

		SNeighbor.NNeighbor = this;
		SNeighbor.NENeighbor = ENeighbor;
		SNeighbor.ENeighbor = SENeighbor;
		SNeighbor.SENeighbor = NENeighbor;
		SNeighbor.SNeighbor = NNeighbor;
		SNeighbor.SWNeighbor = NWNeighbor;
		SNeighbor.WNeighbor = SWNeighbor;
		SNeighbor.NWNeighbor = WNeighbor;

		SWNeighbor.NNeighbor = WNeighbor;
		SWNeighbor.NENeighbor = this;
		SWNeighbor.ENeighbor = SNeighbor;
		SWNeighbor.SENeighbor = NNeighbor;
		SWNeighbor.SNeighbor = NWNeighbor;
		SWNeighbor.SWNeighbor = NENeighbor;
		SWNeighbor.WNeighbor = SENeighbor;
		SWNeighbor.NWNeighbor = ENeighbor;

		WNeighbor.NNeighbor = NWNeighbor;
		WNeighbor.NENeighbor = NNeighbor;
		WNeighbor.ENeighbor = this;
		WNeighbor.SENeighbor = SNeighbor;
		WNeighbor.SNeighbor = SWNeighbor;
		WNeighbor.SWNeighbor = SENeighbor;
		WNeighbor.WNeighbor = ENeighbor;
		WNeighbor.NWNeighbor = NENeighbor;

		NWNeighbor.NNeighbor = SWNeighbor;
		NWNeighbor.NENeighbor = SNeighbor;
		NWNeighbor.ENeighbor = NNeighbor;
		NWNeighbor.SENeighbor = this;
		NWNeighbor.SNeighbor = WNeighbor;
		NWNeighbor.SWNeighbor = ENeighbor;
		NWNeighbor.WNeighbor = NENeighbor;
		NWNeighbor.NWNeighbor = SENeighbor;

		MoveNeighbors();
	}

	private void OnCollisionEnter(Collision other) {
		MoveNeighbors();
	}

	private void MoveNeighbors() {
		NNeighbor.transform.position = transform.position + Vector3.forward * NeighborOffset.y;
		NENeighbor.transform.position = transform.position + Vector3.right * NeighborOffset.x + Vector3.forward * NeighborOffset.y;
		ENeighbor.transform.position = transform.position + Vector3.right * NeighborOffset.x;
		SENeighbor.transform.position = transform.position + Vector3.right * NeighborOffset.x - Vector3.forward * NeighborOffset.y;
		SNeighbor.transform.position = transform.position - Vector3.forward * NeighborOffset.y;
		SWNeighbor.transform.position = transform.position - Vector3.right * NeighborOffset.x - Vector3.forward * NeighborOffset.y;
		WNeighbor.transform.position = transform.position - Vector3.right * NeighborOffset.x;
		NWNeighbor.transform.position = transform.position - Vector3.right * NeighborOffset.x + Vector3.forward * NeighborOffset.y;
	}
}
