
public class SList<T> {
  T[] vals;
  int first;
  int lastPlusOne;
  readonly object locker;

  /// <summary>
  /// Initialize a StaticList of the specified size.<br/>No memory allocations until the list is full.<br/>When there is no more space the size of the list is doubled.
  /// </summary>
  /// <param name="size"></param>
  public SList(int size) {
    vals = new T[size];
    first = 0;
    lastPlusOne = 0;
    Count = 0;
    locker = new object();
  }


  /// <summary>
  /// Adds a new element at the end of th elist
  /// </summary>
  /// <param name="val"></param>
  public void Add(T val) {
    lock (locker) {
      if (Count >= vals.Length) {
        // Extend
        T[] nv = new T[vals.Length * 2];
        for (int i = 0; i < vals.Length; i++) {
          int pos = (first + i) % vals.Length;
          nv[i] = vals[pos];
        }
        first = 0;
        lastPlusOne = vals.Length;
        vals = nv;
      }

      vals[lastPlusOne] = val;
      lastPlusOne++;
      if (lastPlusOne >= vals.Length) lastPlusOne = 0;
      Count++;
    }
  }

  /// <summary>
  /// Adds a new element at the begin of the list
  /// </summary>
  /// <param name="val"></param>
  public void Push(T val) {
    lock (locker) {
      if (Count >= vals.Length) {
        // Extend
        T[] nv = new T[vals.Length * 2];
        for (int i = 0; i < vals.Length; i++) {
          int npos = (first + i) % vals.Length;
          nv[i] = vals[npos];
        }
        first = 0;
        lastPlusOne = vals.Length;
        vals = nv;
      }

      int pos = first - 1;
      if (pos < 0) pos = vals.Length - 1;
      vals[pos] = val;
      first = pos;
      Count++;
    }
  }

  internal void Replace(object n) {
    lock (locker) {
      for (int i = 0; i < Count; i++) {
        int pos = (first + i) % Count;
        object p = (object)vals[pos];
        if (p.Equals(n)) {
          vals[pos] = (T)(object)n;
          return;
        }
      }
    }
  }

  /// <summary>
  /// Returns the Nth element of th elist, without removing it
  /// </summary>
  /// <param name="nth">the index of the element to get</param>
  /// <returns></returns>
  public T Get(int nth) {
    lock (locker) {
      if (nth < 0 || nth >= Count) return default;
      int pos = (first + nth) % vals.Length;
      return vals[pos];
    }
  }

  /// <summary>
  /// Returns the element at begin of the list.<br/>The element is removed
  /// </summary>
  /// <returns></returns>
  public T GetFirst() {
    lock (locker) {
      if (Count == 0) return default;
      T res = vals[first];
      first++;
      if (first >= vals.Length) first = 0;
      Count--;
      return res;
    }
  }

  /// <summary>
  /// Returns the element at end of the list.<br/>The element is removed
  /// </summary>
  /// <returns></returns>
  public T GetLast() {
    lock (locker) {
      if (Count == 0) return default;
      lastPlusOne--;
      if (lastPlusOne < 0) lastPlusOne = vals.Length - 1;
      Count--;
      return vals[lastPlusOne];
    }
  }

  /// <summary>
  /// Returns the element at begin of the list.<br/>The list is not changed, the element is kept
  /// </summary>
  /// <returns></returns>
  public T PeekFirst() {
    lock (locker) {
      if (Count == 0) return default;
      return vals[first];
    }
  }

  /// <summary>
  /// Returns the element at end of the list.<br/>The list is not changed, the element is kept
  /// </summary>
  /// <returns></returns>
  public T PeekLast() {
    lock (locker) {
      if (Count == 0) return default;
      int pos = lastPlusOne - 1;
      if (pos < 0) pos = vals.Length - 1;
      return vals[pos];
    }
  }

  /// <summary>
  /// Returns the number of elements in the list
  /// </summary>
  public int Count { get; private set; }

  /// <summary>
  /// Removes all elements from the list
  /// </summary>
  internal void Clear() {
    lock (locker) {
      for (int i = 0; i < vals.Length; i++)
        vals[i] = default;
      first = 0;
      lastPlusOne = 0;
      Count = 0;
    }
  }

  /// <summary>
  /// Checks if the item to add is already in the list, if missing the item will be inserted.
  /// Returns <i>true</i> if the object was already there
  /// </summary>
  /// <param name="t">The item to be inserted</param>
  internal bool AddIfMissing(T t) {
    int num = Count;
    for (int i = 0; i < num; i++)
      if (Get(i).Equals(t)) return true;
    Add(t);
    return false;
  }

  /// <summary>
  /// Remove the element from the list.
  /// The position of the element will be filles with the last element of the list, if any.
  /// </summary>
  /// <param name="t">The element to be removed</param>
  internal void Remove(T t) {
    lock (locker) {
      int num = Count;
      for (int i = 0; i < num; i++) {
        int pos = (first + i) % vals.Length;
        T ith = vals[pos];
        if (ith.Equals(t)) {
          if (Count == 1) {
            vals[pos] = default;
            Count = 0;
            return;
          }
          lastPlusOne--;
          if (lastPlusOne < 0) lastPlusOne = vals.Length - 1;
          Count--;
          vals[pos] = vals[lastPlusOne];
          vals[lastPlusOne] = default;
          return;
        }
      }
    }
  }
}
