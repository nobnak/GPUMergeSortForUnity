using UnityEngine;
using System.Collections;
using MergeSort;
using System.Text;

public class First : MonoBehaviour {
	public ComputeShader compute;
	public int powOfSize = 8;
	public int seed = 0;

	BitonicMergeSort _sort;
	DisposableBuffer<uint> _keys;
	DisposableBuffer<float> _values;

	void Start () {
		var count = 1 << powOfSize;
		Debug.LogFormat("Count={0}", count);

		_sort = new BitonicMergeSort(compute);
		_keys = new DisposableBuffer<uint>(count);
		_values = new DisposableBuffer<float>(count);

		Random.seed = seed;
		var data = new float[count];
		for (var i = 0; i < count; i++)
			data[i] = i;
		for (var i = 0; i < count; i++) {
			var j = Random.Range(0, count);
			var k = Random.Range(0, count);
			var tmp = data[j]; data[j] = data[k]; data[k] = tmp;
		}
		System.Array.Copy(data, _values.Data, count);
		_values.Upload();

		_sort.Init(_keys.Buffer);
		_sort.Sort(_keys.Buffer, _values.Buffer);

		_keys.Download();
		var failed = false;
		for (var i = 0; i < count; i++) {
			var j = Mathf.RoundToInt(_values.Data[_keys.Data[i]]);
			if (j != i) {
				failed = true;
				Debug.LogErrorFormat("Unexpected Key {0} at {1}", j, i);
			}
		}
		Debug.LogFormat("Sort Test Result = {0}", (failed ? "Wrong" : "Correct"));
	}
	void OnDestroy() {
		if (_sort != null)
			_sort.Dispose();
		if (_keys != null)
			_keys.Dispose();
		if (_values != null)
			_values.Dispose();
	}

	static StringBuilder PrintArray<T>(T[] values, StringBuilder buf) {
		var count = values.Length;
		for (var i = 0; i < count; i++)
			buf.AppendFormat ("{0},", values[i]);
		return buf;
	}
	static StringBuilder PrintArray<T>(uint[] keys, T[] values, StringBuilder buf) {
		var count = values.Length;
		for (var i = 0; i < count; i++)
			buf.AppendFormat ("{0},", values[keys[i]]);
		return buf;
	}
}
