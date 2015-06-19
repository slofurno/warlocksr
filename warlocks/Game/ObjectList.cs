using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace warlocks.Game
{
  public class ObjectList<T> where T : IProcessable
  {
    private ConcurrentQueue<T> _objectqueue;
    private List<T> _objectlist;
    private Func<T> _objectcreator;

    public ObjectList(Func<T> func)
    {
      _objectqueue = new ConcurrentQueue<T>();
      _objectlist = new List<T>();
      _objectcreator = func;

    }

    public T newObjectsReuse()
    {

      var obj = this.CreateInstance();

      _objectqueue.Enqueue(obj);

      return obj;

    }

    public void processNew()
    {

      T temp;

      while (_objectqueue.TryDequeue(out temp))
      {
        _objectlist.Add(temp);

      }

    }

    public List<T> getList()
    {
      return _objectlist;
    }

    public void free(T obj)
    {

      _objectlist.Remove(obj);
    }

    public void ProcessAll(WGame game)
    {

      for (int i = _objectlist.Count - 1; i >= 0; i--)
      {

        _objectlist[i].Process(game);

      }

    }

    public T CreateInstance()
    {

      return _objectcreator();

    }

    public object ToArray()
    {
      return this._objectlist.ToArray();
    }
  }
}
